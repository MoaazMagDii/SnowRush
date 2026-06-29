using System;
using API.Data;
using API.DTOs;
using API.Entities.OrderAggregate;
using API.ExtensionMethods;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace API.Controllers;

public class PaymentsController(PaymentService paymentService, 
    StoreContext context, ILogger<PaymentsController> logger, 
    IConfiguration config, IMapper mapper) : BaseApiController
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BasketDto>> CreateOrUpdatePaymentIntent()
    {
        var basket = await context.Baskets.GetBasketWithItems(Request.Cookies["basketId"]);

        if (basket == null) return BadRequest("Problem with the basket");

        var intent = await paymentService.CreateOrUpdatePaymentIntent(basket);

        if (intent == null) return BadRequest("Problem creating payment intent");

        basket.PaymentIntentId ??= intent.Id;
        basket.ClientSecret ??= intent.ClientSecret;

        if (context.ChangeTracker.HasChanges())
        {
            var result = await context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Problem updating basket with intent");
        }

        return basket.ToDto(mapper);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync();

        try
        {
            var stripeEvent = ConstructStripeEvent(json);

            if (stripeEvent.Data.Object is not PaymentIntent intent)  // If yes, casts and assigns it to this new variable (intent)
            {
                return BadRequest("Invalid event data");
            }

            if (intent.Status == "succeeded") await HandlePaymentIntentSucceeded(intent);
            else await HandlePaymentIntentFailed(intent);

            return Ok();
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe webhook error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Webhook error");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error has occurred");
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error");
        }
    }

    private async Task HandlePaymentIntentFailed(PaymentIntent intent)
    {
        var order = await context.Orders
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.PaymentIntentId == intent.Id)
                ?? throw new Exception("Order not found");

        foreach (var item in order.OrderItems)
        {
            var productItem = await context.Products
                .FindAsync(item.ProductId)
                    ?? throw new Exception("Problem updating order stock");

            productItem.QuantityInStock += item.Quantity;
        }

        order.OrderStatus = OrderStatus.PaymentFailed;
        await context.SaveChangesAsync();
    }

    private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
    {
        var order = await context.Orders
           .Include(x => x.OrderItems)
           .FirstOrDefaultAsync(x => x.PaymentIntentId == intent.Id)
               ?? throw new Exception("Order not found");

        if (order.GetTotal() != intent.Amount)
            order.OrderStatus = OrderStatus.PaymentMismatch;
        else
            order.OrderStatus = OrderStatus.PaymentReceived;
        

        var basket = await context.Baskets.FirstOrDefaultAsync(x => 
            x.PaymentIntentId == intent.Id);
            
        if (basket != null)
            context.Baskets.Remove(basket);
        

        await context.SaveChangesAsync();
    }

    private Event ConstructStripeEvent(string json)
    {
        try
        {
            return EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], config["Stripe:WebhookSecret"]);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to construct stripe event");
            throw new StripeException("Invalid signature");
        }
    }
}


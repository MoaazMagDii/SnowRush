using System;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Entities.OrderAggregate;
using API.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class OrdersController(StoreContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetOrders()
    {
        var userEmail = User.Identity?.Name ?? throw new UnauthorizedAccessException();
        var orders = await context.Orders
            .Include(x => x.OrderItems)
            .Where(x => x.BuyerEmail == userEmail)
            .ToListAsync();

        return orders;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Order>> GetOrderDetails(int id)
    {
        var userEmail = User.Identity?.Name ?? throw new UnauthorizedAccessException();
        var order = await context.Orders
            .Include(x => x.OrderItems)
            .Where(x => x.BuyerEmail == userEmail && id == x.Id)
            .FirstOrDefaultAsync();

        if (order == null) return NotFound();

        return order;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(ShippingAddress shippingAddress)
    {
        var basket = await context.Baskets.GetBasketWithItems(Request.Cookies["basketId"]);
        var userEmail = User.Identity?.Name ?? throw new UnauthorizedAccessException();

        if (basket == null || basket.Items.Count == 0 || string.IsNullOrEmpty(basket.PaymentIntentId))
            return BadRequest("Basket is empty or not found");

        var items = CreateOrderItems(basket.Items);
        if (items == null) return BadRequest("Some items out of stock");

        var subtotal = items.Sum(x => x.Price * x.Quantity);
        var deliveryFee = subtotal > 10000 ? 0 : 500; 

        var order = await context.Orders
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.PaymentIntentId == basket.PaymentIntentId);

        if (order == null)
        {
            order = new Order
            {
                OrderItems = items,
                BuyerEmail = userEmail,
                ShippingAddress = shippingAddress,
                DeliveryFee = deliveryFee,
                Subtotal = subtotal,
                PaymentIntentId = basket.PaymentIntentId
            };

            context.Orders.Add(order);
        }

        context.Baskets.Remove(basket);
        Response.Cookies.Delete("basketId");

        var result = await context.SaveChangesAsync() > 0;
        if (!result) return BadRequest("Problem creating order");
        
        return CreatedAtAction(nameof(GetOrderDetails), new { id = order.Id }, order);

        
    }

    private List<OrderItem>? CreateOrderItems(List<BasketItem> items)
    {
        var orderItems = new List<OrderItem>();

        foreach (var item in items)
        {
            if (item.Product.QuantityInStock < item.Quantity)
                return null;

            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                PictureUrl = item.Product.PictureUrl,
                ProductName = item.Product.Name,
                Price = item.Product.Price,
                Quantity = item.Quantity
            };
            orderItems.Add(orderItem);

            item.Product.QuantityInStock -= item.Quantity;
        }

        return orderItems;
    }
}

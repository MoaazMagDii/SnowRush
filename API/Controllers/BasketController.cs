using System;
using API.Data;
using API.DTOs;
using API.Entities;
using API.ExtensionMethods;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class BasketController(StoreContext context, IMapper mapper) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<BasketDto>> GetBasket()
    {
        var basket = await context.Baskets.GetBasketWithItems(Request.Cookies["basketId"]);
        if (basket == null) return NoContent();
        return mapper.Map<BasketDto>(basket);
    }

    [HttpPost]
    public async Task<ActionResult<BasketDto>> AddItemToBasket(int productId, int quantity)
    {
        var basket = await context.Baskets.GetBasketWithItems(Request.Cookies["basketId"])
        ?? CreateBasket();

        var product = await context.Products.FindAsync(productId);
        if (product == null) return BadRequest("Problem adding item to basket");

        
        basket.AddItem(product, quantity);
        product.QuantityInStock -= quantity;

        var result = await context.SaveChangesAsync() > 0;
        if (result) return CreatedAtAction(nameof(GetBasket), basket.ToDto(mapper));

        return BadRequest("Problem updating basket");
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveItemFromBasket(int productId, int quantity)
    {
        var product = await context.Products.FindAsync(productId);
        if (product == null) return BadRequest("Problem removing item from basket");

        var basket = await context.Baskets.GetBasketWithItems(Request.Cookies["basketId"]);
        if (basket == null) return BadRequest("Unable to retrieve basket");

            
        basket.RemoveItem(productId, quantity);

        var result = await context.SaveChangesAsync() > 0;
        if (result) return Ok();

        return BadRequest("Problem removing item from basket");
    }



    private Basket CreateBasket()
    {
        var basketId = Guid.NewGuid().ToString();

        var cookieOptions = new CookieOptions
        {
            IsEssential = true,
            Expires = DateTime.UtcNow.AddDays(30)
        };

        Response.Cookies.Append("basketId", basketId, cookieOptions);
        var basket = new Basket{ BasketId = basketId };
        context.Baskets.Add(basket);
        return basket;
    }
}

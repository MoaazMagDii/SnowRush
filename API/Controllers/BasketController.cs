using System;
using API.Data;
using API.DTOs;
using API.Entities;
using API.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class BasketController(StoreContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<BasketDto>> GetBasket()
    {
        var basket = await context.Baskets.GetBasketWithItems(Request.Cookies["basketId"]);
        if (basket == null) return NoContent();
        return basket.ToDto();
    }

    [HttpPost]
    public async Task<ActionResult<BasketDto>> AddItemToBasket(int productId, int quantity)
    {
        var basket = await context.Baskets.GetBasketWithItems(Request.Cookies["basketId"])
        ?? CreateBasket();

        var product = await context.Products.FindAsync(productId);
        if (product == null) return BadRequest("Problem adding item to basket");

        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
        if (quantity > product.QuantityInStock) throw new ArgumentException("Quantity exceeds available stock.");

        basket.AddItem(product, quantity);
        product.QuantityInStock -= quantity;

        var result = await context.SaveChangesAsync() > 0;
        if (result) return CreatedAtAction(nameof(GetBasket), basket.ToDto());

        return BadRequest("Problem updating basket");
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveItemFromBasket(int productId, int quantity)
    {
        var product = await context.Products.FindAsync(productId);
        if (product == null) return BadRequest("Problem removing item from basket");

        var basket = await context.Baskets.GetBasketWithItems(Request.Cookies["basketId"]);
        if (basket == null) return BadRequest("Unable to retrieve basket");


        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
        if (quantity > basket.Items.FirstOrDefault(i => i.ProductId == productId)?.Quantity) 
            throw new ArgumentException("Quantity to remove exceeds quantity in basket.");
            
        basket.RemoveItem(productId, quantity);
        product.QuantityInStock += quantity;


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

using System;
using API.DTOs;
using API.Entities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace API.ExtensionMethods;

public static class BasketExtension
{
    public static BasketDto ToDto(this Basket basket)
    {
        return new BasketDto
        {
            BasketId = basket.BasketId,
            ClientSecret = basket.ClientSecret,
            PaymentIntentId = basket.PaymentIntentId,

            Items = basket.Items.Select(item => new BasketItemDto
            {
                Quantity = item.Quantity,
                
                Product = new ProductDto
                {
                    Id = item.Product.Id,
                    Name = item.Product.Name,
                    Description = item.Product.Description,
                    Price = item.Product.Price,
                    PictureUrl = item.Product.PictureUrl,
                    Brand = item.Product.Brand,
                    Type = item.Product.Type,
                    QuantityInStock = item.Product.QuantityInStock
                }
            }).ToList()
        };
    }

    public static async Task<Basket?> GetBasketWithItems(this IQueryable<Basket> query, 
        string? basketId)
    {
        return await query
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.BasketId == basketId);
    }
}

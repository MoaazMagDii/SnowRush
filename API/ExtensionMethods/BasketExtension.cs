using System;
using API.DTOs;
using API.Entities;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace API.ExtensionMethods;

public static class BasketExtension
{
    public static BasketDto ToDto(this Basket basket)
    {
        return new BasketDto
        {
            BasketId = basket.BasketId,
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
}

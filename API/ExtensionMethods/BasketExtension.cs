using System;
using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace API.ExtensionMethods;

public static class BasketExtension
{
    public static BasketDto ToDto(this Basket basket, IMapper mapper)
    {
        return mapper.Map<BasketDto>(basket);
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

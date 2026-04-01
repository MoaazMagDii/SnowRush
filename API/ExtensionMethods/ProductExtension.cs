using System;
using API.Entities;

namespace API.ExtensionMethods;

public static class ProductExtension
{
    public static IQueryable<Product> Sort(this IQueryable<Product> query, string? orderBy)
    {
        query = orderBy switch
        {
            "priceAsc" => query.OrderBy(p => p.Price),
            "priceDesc" => query.OrderByDescending(p => p.Price),
            _ => query.OrderBy(p => p.Name)
        };
        return query;
    }

    public static IQueryable<Product> Search(this IQueryable<Product> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;

        var lowerCaseSearchTerm = searchTerm.Trim().ToLower();
        return query.Where(p => p.Name.ToLower().Contains(lowerCaseSearchTerm));
    }

    public static IQueryable<Product> Filter(this IQueryable<Product> query, string? brands, string? types)
    {
        var brandList = string.IsNullOrWhiteSpace(brands)
        ? new List<string>()
        : brands.ToLower().Split(',').ToList();

        var typeList = string.IsNullOrWhiteSpace(types) 
        ? new List<string>() 
        : types.ToLower().Split(',').ToList();

        if (brandList.Any())
            query = query.Where(p => brandList.Contains(p.Brand.ToLower()));

        if (typeList.Any())
            query = query.Where(p => typeList.Contains(p.Type.ToLower()));

        return query;
    }

    public static IQueryable<Product> Paginate(this IQueryable<Product> query, int pageNumber, int pageSize)
    {
        // Fetch pageSize + 1 to determine if there are more results without a separate count query
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize + 1);
    }
}

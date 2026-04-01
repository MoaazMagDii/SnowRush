using API.Data;
using API.Entities;
using API.ExtensionMethods;
using API.RequestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ProductsController(StoreContext context) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts(
            [FromQuery]ProductParams param)
        {
            var query = context.Products
            .AsNoTracking()               // Use AsNoTracking for read-only queries to improve performance
            .Sort(param.OrderBy)          // Using the Sort IQueryable extension method 
            .Search(param.SearchTerm)    // Using the Search IQueryable extension method
            .Filter(param.Brands, param.Types) // Using the Filter IQueryable extension method
            .Paginate(param.PageNumber, param.PageSize); // Using the Paginate IQueryable extension method


            return await query.ToListAsync();
        }

        [HttpGet("{id}")] // https://Localhost:5001/api/products/1
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await context.Products.FindAsync(id);

            if (product == null) return NotFound();
            
            return product;
        }

        [HttpGet("filters")]
        public async Task<ActionResult> GetProductFilters()
        {
            var brands = await context.Products.Select(p => p.Brand).Distinct().ToListAsync();
            var types = await context.Products.Select(p => p.Type).Distinct().ToListAsync();

            return Ok(new { brands, types });
        }
    }
}

using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")] // https://Localhost:5001/api/products
    [ApiController]
    public class ProductsController(StoreContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            return await context.Products.ToListAsync();
        }

        [HttpGet("{id}")] // https://Localhost:5001/api/products/1
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await context.Products.FindAsync(id);

            if (product == null) return NotFound();
            
            return product;
        }
    }
}

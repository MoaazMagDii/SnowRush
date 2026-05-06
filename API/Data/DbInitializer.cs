using API.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Data;

public class DbInitializer
{

    public static async Task InitializeDbAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        context.Database.EnsureCreated();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        
        // Create roles if they don't exist
        if (!await roleManager.RoleExistsAsync("Member"))
        {
            await roleManager.CreateAsync(new IdentityRole
            {
                Id = "e069461a-10cf-4abf-9930-d070b2a7e40f",
                ConcurrencyStamp = "Member",
                Name = "Member",
                NormalizedName = "MEMBER"
            });

            await roleManager.CreateAsync(new IdentityRole
            {
                Id = "ed2e9149-fa53-484c-a93f-bd33f9e9fcf6",
                ConcurrencyStamp = "Admin",
                Name = "Admin",
                NormalizedName = "ADMIN"
            });

        }
        

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        if (!userManager.Users.Any())
        {
            var user = new User
            {
                UserName = "bob@test.com",
                Email = "bob@test.com"
            };

            await userManager.CreateAsync(user, "Pa$$w0rd");
            await userManager.AddToRoleAsync(user, "Member");

            var admin = new User
            {
                UserName = "admin@test.com",
                Email = "admin@test.com"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, ["Member", "Admin"]);
        }


        if (context.Products.Any()) return;

        var products = new List<Product>
        {
            new() {
                Name = "Product 1",
                Description = "Lorem ipsum dolor tosuere, ma eros quis urna.",
                Price = 20000,
                PictureUrl = "/images/products/sb-ang1.png",
                Brand = "Angular",
                Type = "Boards",
                QuantityInStock = 100
            },
            new() {
                Name = "Product 2",
                Description = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.",
                Price = 15000,
                PictureUrl = "/images/products/sb-ang2.png",
                Brand = "React",
                Type = "Hats",
                QuantityInStock = 100
            },
            new() {
                Name = "Product 3",
                Description = "Lorem ipsum dolor tosuere, ma eros quis urna.",
                Price = 18000,
                PictureUrl = "/images/products/sb-core1.png",
                Brand = "NetCore",
                Type = "Gloves",
                QuantityInStock = 100
            },
            new() {
                Name = "Product 4",
                Description = "Lorem ipsum dolor tosuere, ma eros quis urna.",
                Price = 30000,
                PictureUrl = "/images/products/sb-core2.png",
                Brand = "Redis",
                Type = "Boots",
                QuantityInStock = 100
            },
        };


        
        context.Products.AddRange(products);
        context.SaveChanges();
    }
}

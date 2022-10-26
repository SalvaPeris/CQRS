using Microsoft.AspNetCore.Identity;
using ApplicationCore.Common.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationCore.Infrastructure.Persistence.Seed
{
    public static class Seed
    {
        public static async Task SeedProducts(this IServiceProvider services )
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            context.Database.EnsureCreated();

            if (!context.Products.Any())
            {
                context.Products.AddRange(new List<Product>
                {
                    new Product
                    {
                        Description = "Product 01",
                        Price = 16000
                    },
                    new Product
                    {
                        Description = "Product 02",
                        Price = 52200
                    }
                });

                await context.SaveChangesAsync();
            }

            var testUser = await userManager.FindByNameAsync("other_user");
            if (testUser is null)
            {
                testUser = new IdentityUser
                {
                    UserName = "test_user"
                };

                await userManager.CreateAsync(testUser, "Pass1234");
                await userManager.CreateAsync(new IdentityUser
                {
                    UserName = "other_user"
                }, "Pass1234");
            }

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole is null)
            {
                await roleManager.CreateAsync(new IdentityRole
                {
                    Name = "Admin"
                });

                await userManager.AddToRoleAsync(testUser, "Admin");
            }
        }
    }
}


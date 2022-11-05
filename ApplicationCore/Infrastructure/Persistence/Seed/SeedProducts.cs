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
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.EnsureCreated();

            await MyAppDbContextSeed.SeedProductsAsync(context);
            await MyAppDbContextSeed.SeedUsersAsync(userManager, roleManager);

        }
    }
}


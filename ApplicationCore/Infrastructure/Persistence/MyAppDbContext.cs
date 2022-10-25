using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Domain;

namespace ApplicationCore.Infrastructure.Persistence
{
    public class MyAppDbContext : IdentityDbContext<IdentityUser>
    {
        public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options)
        { }

        public DbSet<Product> Products => Set<Product>();
    }
}

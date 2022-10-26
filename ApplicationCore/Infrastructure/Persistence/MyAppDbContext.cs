using ApplicationCore.Common.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Infrastructure.Persistence
{
    public class MyAppDbContext : IdentityDbContext<IdentityUser>
    {
        public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options)
        { }

        public DbSet<Product> Products => Set<Product>();
    }
}

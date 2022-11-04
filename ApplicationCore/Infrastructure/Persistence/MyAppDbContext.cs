using ApplicationCore.Common.Domain;
using ApplicationCore.Common.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Infrastructure.Persistence
{
    public class MyAppDbContext : IdentityDbContext<IdentityUser>
    {
        private readonly CurrentUser _user;

        public MyAppDbContext(DbContextOptions<MyAppDbContext> options,
            ICurrentUserService currentUserService) : base(options)
        {
            _user = currentUserService.User;
        }

        public DbSet<Product> Products => Set<Product>();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntidad>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreadoPor = _user.Id;
                        entry.Entity.CreadoEn = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModificadoPor = _user.Id;
                        entry.Entity.ModificadoEn = DateTime.UtcNow;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Unilevel.Data;

namespace Unilevel.Data
{
    public class UnilevelContext : DbContext
    {
        public UnilevelContext(DbContextOptions<UnilevelContext> options)
            : base(options)
        {
        }

        public DbSet<Unilevel.Data.RefreshToken> RefreshTokens { get; set; } = default!;
        public DbSet<Unilevel.Data.User> Users { get; set; } = default!;
        public DbSet<Unilevel.Data.Role> Roles { get; set; } = default!;
        public DbSet<Unilevel.Data.Distributor> Distributors { get; set; } = default!;
        public DbSet<Unilevel.Data.Area> Areas { get; set; } = default!;
    }
}

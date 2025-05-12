using Microsoft.EntityFrameworkCore;
using SDKServer.Models;

namespace SDKServer
{
    public class SDKDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public SDKDbContext(DbContextOptions<SDKDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.username)
                .IsUnique();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SalesTracker.InfraStructure.Models.Entities;
using SalesTracker.InfraStructure.Responses;
namespace SalesTracker.InfraStructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<DailySalesData> DailySalesReport { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DailySalesData>().HasNoKey();
            modelBuilder.Ignore<DailySalesData>();

        }

    }
   
    }

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesTracker.InfraStructure.Models.Entities;

namespace SalesTracker.InfraStructure.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Check if roles already exist
                if (context.Roles.Any())
                    return;

                // Seed roles
                context.Roles.AddRange(
                    new Role {  Name = "admin", Description = "Administrator privileges", CreatedAt = DateTime.UtcNow },
                    new Role {  Name = "cashier", Description = "Cashier privileges", CreatedAt = DateTime.UtcNow }
                );

                context.SaveChanges();
            }
        }
    }
}


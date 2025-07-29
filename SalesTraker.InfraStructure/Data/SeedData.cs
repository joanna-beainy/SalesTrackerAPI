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
                var rolesToSeed = new[]
                {
                    new Role { Name = "admin", Description = "Administrator privileges", CreatedAt = DateTime.UtcNow },
                    new Role { Name = "cashier", Description = "Cashier privileges", CreatedAt = DateTime.UtcNow },
                    new Role { Name = "manager", Description = "Manager privileges", CreatedAt = DateTime.UtcNow },
                    new Role { Name = "user", Description = "General user access", CreatedAt = DateTime.UtcNow }
                };

                foreach (var role in rolesToSeed)
                {
                    if (!context.Roles.Any(r => r.Name == role.Name))
                    {
                        context.Roles.Add(role);
                    }
                }

                context.SaveChanges();
            }
        }
    }
}


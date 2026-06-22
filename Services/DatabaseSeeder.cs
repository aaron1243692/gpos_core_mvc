using gpos.Data;
using gpos.Models;
using Microsoft.EntityFrameworkCore;

namespace gpos.Services
{
    public static class DatabaseSeeder
    {
        public static async Task SeedDefaultAdminAsync(ApplicationDbContext context, ILogger logger)
        {
            var now = DateTime.UtcNow;

            var adminRole = await context.Roles.FirstOrDefaultAsync(role => role.Code == "admin");
            if (adminRole is null)
            {
                adminRole = new Role
                {
                    Name = "Admin",
                    Code = "admin",
                    CreatedAt = now,
                    UpdatedAt = now
                };

                context.Roles.Add(adminRole);
                await context.SaveChangesAsync();
                logger.LogInformation("Created default Admin role.");
            }

            if (await context.Users.AnyAsync())
            {
                return;
            }

            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@gpos.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                CreatedAt = now,
                UpdatedAt = now
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();

            context.UserRoles.Add(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id,
                CreatedAt = now,
                UpdatedAt = now
            });

            await context.SaveChangesAsync();
            logger.LogInformation("Created default admin user and assigned Admin role.");
        }
    }
}

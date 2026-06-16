using gpos.Data;
using gpos.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

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

        public static async Task EnsureDefaultSalesmanEmployeeAsync(ApplicationDbContext context, ILogger logger)
        {
            await context.Database.ExecuteSqlRawAsync("""
                CREATE TABLE IF NOT EXISTS employees (
                id INT AUTO_INCREMENT PRIMARY KEY,
                name VARCHAR(150) NOT NULL,
                username VARCHAR(100) NOT NULL UNIQUE,
                password VARCHAR(255) NOT NULL,
                role VARCHAR(50) NOT NULL,
                status VARCHAR(20) NOT NULL DEFAULT 'active',
                created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
                );
                """);

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("salesman123");

            await context.Database.ExecuteSqlRawAsync("""
                INSERT INTO employees (name, username, password, role, status)
                SELECT @name, @username, @password, @role, @status
                WHERE NOT EXISTS (
                    SELECT 1 FROM employees WHERE username = @username
                );
                """,
                new MySqlParameter("@name", "Salesman"),
                new MySqlParameter("@username", "salesman"),
                new MySqlParameter("@password", hashedPassword),
                new MySqlParameter("@role", "salesman"),
                new MySqlParameter("@status", "active"));

            logger.LogInformation("Ensured employees table and default salesman account.");
        }
    }
}

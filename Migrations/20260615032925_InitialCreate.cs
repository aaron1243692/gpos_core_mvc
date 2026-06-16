using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `users` (
                  `id` INT NOT NULL AUTO_INCREMENT,
                  `username` VARCHAR(255) NOT NULL,
                  `email` VARCHAR(255) NOT NULL,
                  `password_hash` VARCHAR(255) NOT NULL,
                  `created_at` DATETIME NULL,
                  `updated_at` DATETIME NULL,
                  PRIMARY KEY (`id`),
                  UNIQUE KEY `IX_users_username` (`username`),
                  UNIQUE KEY `IX_users_email` (`email`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                """);

            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `roles` (
                  `id` INT NOT NULL AUTO_INCREMENT,
                  `name` VARCHAR(255) NOT NULL,
                  `code` VARCHAR(255) NOT NULL,
                  `created_at` DATETIME NULL,
                  `updated_at` DATETIME NULL,
                  PRIMARY KEY (`id`),
                  UNIQUE KEY `IX_roles_code` (`code`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                """);

            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `permissions` (
                  `id` INT NOT NULL AUTO_INCREMENT,
                  `name` VARCHAR(255) NOT NULL,
                  `code` VARCHAR(255) NOT NULL,
                  `parent_id` INT NULL,
                  `created_at` DATETIME NULL,
                  `updated_at` DATETIME NULL,
                  PRIMARY KEY (`id`),
                  UNIQUE KEY `IX_permissions_code` (`code`),
                  KEY `IX_permissions_parent_id` (`parent_id`),
                  CONSTRAINT `FK_permissions_permissions_parent_id`
                    FOREIGN KEY (`parent_id`) REFERENCES `permissions` (`id`)
                    ON DELETE RESTRICT
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                """);

            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `user_roles` (
                  `id` INT NOT NULL AUTO_INCREMENT,
                  `user_id` INT NOT NULL,
                  `role_id` INT NOT NULL,
                  `created_at` DATETIME NULL,
                  `updated_at` DATETIME NULL,
                  PRIMARY KEY (`id`),
                  UNIQUE KEY `IX_user_roles_user_id_role_id` (`user_id`, `role_id`),
                  KEY `IX_user_roles_role_id` (`role_id`),
                  CONSTRAINT `FK_user_roles_users_user_id`
                    FOREIGN KEY (`user_id`) REFERENCES `users` (`id`)
                    ON DELETE CASCADE,
                  CONSTRAINT `FK_user_roles_roles_role_id`
                    FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`)
                    ON DELETE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                """);

            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `role_permissions` (
                  `id` INT NOT NULL AUTO_INCREMENT,
                  `role_id` INT NOT NULL,
                  `permission_id` INT NOT NULL,
                  `created_at` DATETIME NULL,
                  `updated_at` DATETIME NULL,
                  PRIMARY KEY (`id`),
                  UNIQUE KEY `IX_role_permissions_role_id_permission_id` (`role_id`, `permission_id`),
                  KEY `IX_role_permissions_permission_id` (`permission_id`),
                  CONSTRAINT `FK_role_permissions_roles_role_id`
                    FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`)
                    ON DELETE CASCADE,
                  CONSTRAINT `FK_role_permissions_permissions_permission_id`
                    FOREIGN KEY (`permission_id`) REFERENCES `permissions` (`id`)
                    ON DELETE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally non-destructive. Do not drop auth tables or delete existing data automatically.
        }
    }
}

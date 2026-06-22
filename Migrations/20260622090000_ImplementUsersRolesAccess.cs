using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260622090000_ImplementUsersRolesAccess")]
    public partial class ImplementUsersRolesAccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddColumnIfMissing(migrationBuilder, "users", "status", "int NOT NULL DEFAULT 1");
            AddColumnIfMissing(migrationBuilder, "roles", "code", "varchar(255) NOT NULL DEFAULT ''");
            AddColumnIfMissing(migrationBuilder, "roles", "status", "int NOT NULL DEFAULT 1");
            CreateUserRolesTableIfMissing(migrationBuilder);
            AddColumnIfMissing(migrationBuilder, "user_roles", "created_at", "datetime(6) NULL");
            AddColumnIfMissing(migrationBuilder, "user_roles", "updated_at", "datetime(6) NULL");
            CreateIndexIfMissing(migrationBuilder, "user_roles", "IX_user_roles_role_id", "`role_id`");
            CreateIndexIfMissing(migrationBuilder, "user_roles", "IX_user_roles_user_id_role_id", "`user_id`, `role_id`", unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }

        private static void AddColumnIfMissing(MigrationBuilder migrationBuilder, string table, string column, string definition)
        {
            migrationBuilder.Sql(
                $"""
                SET @column_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND COLUMN_NAME = '{column}'
                );
                SET @statement = IF(
                    @column_exists = 0,
                    'ALTER TABLE `{table}` ADD COLUMN `{column}` {definition}',
                    'SELECT 1'
                );
                PREPARE implement_access_add_column FROM @statement;
                EXECUTE implement_access_add_column;
                DEALLOCATE PREPARE implement_access_add_column;
                """);
        }

        private static void CreateUserRolesTableIfMissing(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                SET @table_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'user_roles'
                );
                SET @statement = IF(
                    @table_exists = 0,
                    'CREATE TABLE `user_roles` (
                        `id` int NOT NULL AUTO_INCREMENT,
                        `user_id` int NOT NULL,
                        `role_id` int NOT NULL,
                        `created_at` datetime(6) NULL,
                        `updated_at` datetime(6) NULL,
                        CONSTRAINT `PK_user_roles` PRIMARY KEY (`id`),
                        CONSTRAINT `FK_user_roles_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE,
                        CONSTRAINT `FK_user_roles_roles_role_id` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`) ON DELETE CASCADE
                    )',
                    'SELECT 1'
                );
                PREPARE implement_access_create_user_roles FROM @statement;
                EXECUTE implement_access_create_user_roles;
                DEALLOCATE PREPARE implement_access_create_user_roles;
                """);
        }

        private static void CreateIndexIfMissing(MigrationBuilder migrationBuilder, string table, string index, string columns, bool unique = false)
        {
            var uniqueSql = unique ? "UNIQUE " : string.Empty;

            migrationBuilder.Sql(
                $"""
                SET @index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND INDEX_NAME = '{index}'
                );
                SET @statement = IF(
                    @index_exists = 0,
                    'CREATE {uniqueSql}INDEX `{index}` ON `{table}` ({columns})',
                    'SELECT 1'
                );
                PREPARE implement_access_create_index FROM @statement;
                EXECUTE implement_access_create_index;
                DEALLOCATE PREPARE implement_access_create_index;
                """);
        }
    }
}

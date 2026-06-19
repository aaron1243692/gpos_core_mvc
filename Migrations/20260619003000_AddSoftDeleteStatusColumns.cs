using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260619003000_AddSoftDeleteStatusColumns")]
    public partial class AddSoftDeleteStatusColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddColumnIfMissing(migrationBuilder, "products", "status", "int NOT NULL DEFAULT 1");
            AddColumnIfMissing(migrationBuilder, "product_categories", "status", "int NOT NULL DEFAULT 1");
            AddColumnIfMissing(migrationBuilder, "fuels", "status", "int NOT NULL DEFAULT 1");
            AddColumnIfMissing(migrationBuilder, "tanks", "status", "int NOT NULL DEFAULT 1");
            AddColumnIfMissing(migrationBuilder, "pumps", "status", "int NOT NULL DEFAULT 1");
            AddColumnIfMissing(migrationBuilder, "suppliers", "status", "int NOT NULL DEFAULT 1");
            AddColumnIfMissing(migrationBuilder, "fuel_price_history", "status", "int NOT NULL DEFAULT 1");
            AddColumnIfMissing(migrationBuilder, "fuel_price_history", "updated_at", "datetime(6) NULL");
            AddColumnIfMissing(migrationBuilder, "users", "status", "int NOT NULL DEFAULT 1");
            AddColumnIfMissing(migrationBuilder, "roles", "status", "int NOT NULL DEFAULT 1");
            AddColumnIfMissing(migrationBuilder, "permissions", "status", "int NOT NULL DEFAULT 1");
            AddColumnIfMissing(migrationBuilder, "role_permissions", "status", "int NOT NULL DEFAULT 1");

            migrationBuilder.Sql(
                """
                UPDATE `products` SET `status` = CASE WHEN `is_active` = 1 THEN 1 ELSE 0 END;
                UPDATE `product_categories` SET `status` = CASE WHEN `is_active` = 1 THEN 1 ELSE 0 END;
                UPDATE `fuels` SET `status` = CASE WHEN `is_active` = 1 THEN 1 ELSE 0 END;
                UPDATE `tanks` SET `status` = CASE WHEN `is_active` = 1 THEN 1 ELSE 0 END;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            DropColumnIfExists(migrationBuilder, "role_permissions", "status");
            DropColumnIfExists(migrationBuilder, "permissions", "status");
            DropColumnIfExists(migrationBuilder, "roles", "status");
            DropColumnIfExists(migrationBuilder, "users", "status");
            DropColumnIfExists(migrationBuilder, "fuel_price_history", "updated_at");
            DropColumnIfExists(migrationBuilder, "fuel_price_history", "status");
            DropColumnIfExists(migrationBuilder, "suppliers", "status");
            DropColumnIfExists(migrationBuilder, "pumps", "status");
            DropColumnIfExists(migrationBuilder, "tanks", "status");
            DropColumnIfExists(migrationBuilder, "fuels", "status");
            DropColumnIfExists(migrationBuilder, "product_categories", "status");
            DropColumnIfExists(migrationBuilder, "products", "status");
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
                PREPARE add_soft_delete_column FROM @statement;
                EXECUTE add_soft_delete_column;
                DEALLOCATE PREPARE add_soft_delete_column;
                """);
        }

        private static void DropColumnIfExists(MigrationBuilder migrationBuilder, string table, string column)
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
                    @column_exists > 0,
                    'ALTER TABLE `{table}` DROP COLUMN `{column}`',
                    'SELECT 1'
                );
                PREPARE drop_soft_delete_column FROM @statement;
                EXECUTE drop_soft_delete_column;
                DEALLOCATE PREPARE drop_soft_delete_column;
                """);
        }
    }
}

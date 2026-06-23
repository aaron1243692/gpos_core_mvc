using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260623002000_CreateDiscountRulesOnly")]
    public partial class CreateDiscountRulesOnly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `discount_rules` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `name` varchar(100) NOT NULL DEFAULT '',
                    `discount_id` int NOT NULL,
                    `applies_to` varchar(50) NOT NULL,
                    `discount_type` varchar(50) NOT NULL,
                    `discount_value` decimal(18,2) NOT NULL,
                    `minimum_amount` decimal(18,2) NOT NULL DEFAULT 0,
                    `member_required` int NOT NULL DEFAULT 0,
                    `start_date` datetime NULL,
                    `end_date` datetime NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`),
                    KEY `IX_discount_rules_discount_id` (`discount_id`),
                    CONSTRAINT `FK_discount_rules_discounts_discount_id` FOREIGN KEY (`discount_id`) REFERENCES `discounts` (`id`) ON DELETE RESTRICT
                ) CHARACTER SET=utf8mb4;
                """);

            AddColumnIfMissing(migrationBuilder, "discount_rules", "name", "varchar(100) NOT NULL DEFAULT ''");
            AddColumnIfMissing(migrationBuilder, "discount_rules", "discount_id", "int NULL");
            AddColumnIfMissing(migrationBuilder, "discount_rules", "minimum_amount", "decimal(18,2) NOT NULL DEFAULT 0");
            AddColumnIfMissing(migrationBuilder, "discount_rules", "member_required", "int NOT NULL DEFAULT 0");
            AddColumnIfMissing(migrationBuilder, "discount_rules", "start_date", "datetime NULL");
            AddColumnIfMissing(migrationBuilder, "discount_rules", "end_date", "datetime NULL");

            migrationBuilder.Sql(
                """
                SET @first_discount_id = (SELECT `id` FROM `discounts` ORDER BY `id` LIMIT 1);
                UPDATE `discount_rules`
                SET `discount_id` = @first_discount_id
                WHERE `discount_id` IS NULL
                  AND @first_discount_id IS NOT NULL;
                """);

            AddIndexIfMissing(migrationBuilder, "discount_rules", "IX_discount_rules_discount_id", "discount_id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_discount_rules_discounts_discount_id", "discount_rules", "discount_id", "discounts", "id");
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
                PREPARE add_discount_rules_column FROM @statement;
                EXECUTE add_discount_rules_column;
                DEALLOCATE PREPARE add_discount_rules_column;
                """);
        }

        private static void AddIndexIfMissing(MigrationBuilder migrationBuilder, string table, string index, string column)
        {
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
                    'CREATE INDEX `{index}` ON `{table}` (`{column}`)',
                    'SELECT 1'
                );
                PREPARE add_discount_rules_index FROM @statement;
                EXECUTE add_discount_rules_index;
                DEALLOCATE PREPARE add_discount_rules_index;
                """);
        }

        private static void AddForeignKeyIfMissing(MigrationBuilder migrationBuilder, string constraint, string table, string column, string principalTable, string principalColumn)
        {
            migrationBuilder.Sql(
                $"""
                SET @constraint_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND CONSTRAINT_NAME = '{constraint}'
                );
                SET @invalid_rows = (
                    SELECT COUNT(*)
                    FROM `{table}` child
                    LEFT JOIN `{principalTable}` parent ON child.`{column}` = parent.`{principalColumn}`
                    WHERE child.`{column}` IS NOT NULL
                      AND parent.`{principalColumn}` IS NULL
                );
                SET @statement = IF(
                    @constraint_exists = 0 AND @invalid_rows = 0,
                    'ALTER TABLE `{table}` ADD CONSTRAINT `{constraint}` FOREIGN KEY (`{column}`) REFERENCES `{principalTable}` (`{principalColumn}`) ON DELETE RESTRICT',
                    'SELECT 1'
                );
                PREPARE add_discount_rules_fk FROM @statement;
                EXECUTE add_discount_rules_fk;
                DEALLOCATE PREPARE add_discount_rules_fk;
                """);
        }
    }
}

using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260623003000_UpdateRebatesForAppliesTo")]
    public partial class UpdateRebatesForAppliesTo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `rebate_rules` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `name` varchar(100) NOT NULL,
                    `applies_to` varchar(50) NOT NULL DEFAULT 'Both',
                    `points_required` decimal(18,2) NOT NULL,
                    `rebate_value` decimal(18,2) NOT NULL,
                    `minimum_purchase` decimal(18,2) NOT NULL DEFAULT 0,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET=utf8mb4;
                """);

            AddColumnIfMissing(migrationBuilder, "rebate_rules", "applies_to", "varchar(50) NOT NULL DEFAULT 'Both'");
            AddColumnIfMissing(migrationBuilder, "rebate_rules", "minimum_purchase", "decimal(18,2) NOT NULL DEFAULT 0");
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
                PREPARE add_rebate_column FROM @statement;
                EXECUTE add_rebate_column;
                DEALLOCATE PREPARE add_rebate_column;
                """);
        }
    }
}

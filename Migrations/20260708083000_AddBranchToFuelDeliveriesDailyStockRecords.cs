using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260708083000_AddBranchToFuelDeliveriesDailyStockRecords")]
    public partial class AddBranchToFuelDeliveriesDailyStockRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddColumnIfMissing(migrationBuilder, "fuel_deliveries", "branch_id", "int NULL");
            AddColumnIfMissing(migrationBuilder, "daily_stock_records", "branch_id", "int NULL");

            CreateIndexIfMissing(migrationBuilder, "fuel_deliveries", "IX_fuel_deliveries_branch_id", "`branch_id`");
            CreateIndexIfMissing(migrationBuilder, "daily_stock_records", "IX_daily_stock_records_branch_id", "`branch_id`");

            AddForeignKeyIfMissing(migrationBuilder, "FK_fuel_deliveries_branches_branch_id", "fuel_deliveries", "branch_id", "branches", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_daily_stock_records_branches_branch_id", "daily_stock_records", "branch_id", "branches", "id");
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
                PREPARE add_branch_column FROM @statement;
                EXECUTE add_branch_column;
                DEALLOCATE PREPARE add_branch_column;
                """);
        }

        private static void CreateIndexIfMissing(MigrationBuilder migrationBuilder, string table, string index, string columns)
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
                    'CREATE INDEX `{index}` ON `{table}` ({columns})',
                    'SELECT 1'
                );
                PREPARE add_branch_index FROM @statement;
                EXECUTE add_branch_index;
                DEALLOCATE PREPARE add_branch_index;
                """);
        }

        private static void AddForeignKeyIfMissing(MigrationBuilder migrationBuilder, string constraint, string table, string column, string principalTable, string principalColumn)
        {
            migrationBuilder.Sql(
                $"""
                SET @constraint_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND CONSTRAINT_NAME = '{constraint}'
                );
                SET @statement = IF(
                    @constraint_exists = 0,
                    'ALTER TABLE `{table}` ADD CONSTRAINT `{constraint}` FOREIGN KEY (`{column}`) REFERENCES `{principalTable}` (`{principalColumn}`) ON DELETE RESTRICT',
                    'SELECT 1'
                );
                PREPARE add_branch_fk FROM @statement;
                EXECUTE add_branch_fk;
                DEALLOCATE PREPARE add_branch_fk;
                """);
        }
    }
}

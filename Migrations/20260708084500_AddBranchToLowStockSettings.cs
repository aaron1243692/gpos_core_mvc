using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260708084500_AddBranchToLowStockSettings")]
    public partial class AddBranchToLowStockSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddColumnIfMissing(migrationBuilder, "low_stock_settings", "branch_id", "int NULL");
            CreateIndexIfMissing(migrationBuilder, "low_stock_settings", "IX_low_stock_settings_branch_id", "`branch_id`");
            AddForeignKeyIfMissing(migrationBuilder, "FK_low_stock_settings_branches_branch_id", "low_stock_settings", "branch_id", "branches", "id");
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
                PREPARE add_low_stock_branch_column FROM @statement;
                EXECUTE add_low_stock_branch_column;
                DEALLOCATE PREPARE add_low_stock_branch_column;
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
                PREPARE add_low_stock_branch_index FROM @statement;
                EXECUTE add_low_stock_branch_index;
                DEALLOCATE PREPARE add_low_stock_branch_index;
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
                PREPARE add_low_stock_branch_fk FROM @statement;
                EXECUTE add_low_stock_branch_fk;
                DEALLOCATE PREPARE add_low_stock_branch_fk;
                """);
        }
    }
}

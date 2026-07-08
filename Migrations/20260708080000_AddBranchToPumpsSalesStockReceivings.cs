using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260708080000_AddBranchToPumpsSalesStockReceivings")]
    public partial class AddBranchToPumpsSalesStockReceivings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddColumnIfMissing(migrationBuilder, "pumps", "branch_id", "int NULL");
            AddColumnIfMissing(migrationBuilder, "sales", "branch_id", "int NULL");
            AddColumnIfMissing(migrationBuilder, "stock_receivings", "branch_id", "int NULL");

            CreateIndexIfMissing(migrationBuilder, "pumps", "IX_pumps_branch_id", "`branch_id`");
            CreateIndexIfMissing(migrationBuilder, "sales", "IX_sales_branch_id", "`branch_id`");
            CreateIndexIfMissing(migrationBuilder, "stock_receivings", "IX_stock_receivings_branch_id", "`branch_id`");

            AddForeignKeyIfMissing(migrationBuilder, "FK_pumps_branches_branch_id", "pumps", "branch_id", "branches", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_sales_branches_branch_id", "sales", "branch_id", "branches", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_stock_receivings_branches_branch_id", "stock_receivings", "branch_id", "branches", "id");
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

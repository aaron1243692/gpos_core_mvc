using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260623001000_AddOldNewPointsToPointsLedger")]
    public partial class AddOldNewPointsToPointsLedger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddColumnIfMissing(migrationBuilder, "points_ledger", "old_points", "decimal(18,2) NOT NULL DEFAULT 0");
            AddColumnIfMissing(migrationBuilder, "points_ledger", "new_points", "decimal(18,2) NOT NULL DEFAULT 0");
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
                PREPARE add_points_ledger_points_column FROM @statement;
                EXECUTE add_points_ledger_points_column;
                DEALLOCATE PREPARE add_points_ledger_points_column;
                """);
        }
    }
}

using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260629000000_RefactorVoucherRulesLikeDiscountRules")]
    public partial class RefactorVoucherRulesLikeDiscountRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "member_id",
                table: "vouchers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            AddColumnIfMissing(migrationBuilder, "voucher_rules", "voucher_id", "int NULL");
            AddColumnIfMissing(migrationBuilder, "voucher_rules", "member_required", "int NOT NULL DEFAULT 0");
            AddIndexIfMissing(migrationBuilder, "voucher_rules", "IX_voucher_rules_voucher_id", "voucher_id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_voucher_rules_vouchers_voucher_id", "voucher_rules", "voucher_id", "vouchers", "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_voucher_rules_vouchers_voucher_id",
                table: "voucher_rules");

            migrationBuilder.DropIndex(
                name: "IX_voucher_rules_voucher_id",
                table: "voucher_rules");

            migrationBuilder.DropColumn(
                name: "voucher_id",
                table: "voucher_rules");

            migrationBuilder.DropColumn(
                name: "member_required",
                table: "voucher_rules");

            migrationBuilder.AlterColumn<int>(
                name: "member_id",
                table: "vouchers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        private static void AddColumnIfMissing(MigrationBuilder migrationBuilder, string table, string column, string definition)
        {
            migrationBuilder.Sql($@"
                SET @statement = IF(
                    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
                     WHERE TABLE_SCHEMA = DATABASE()
                       AND TABLE_NAME = '{table}'
                       AND COLUMN_NAME = '{column}') = 0,
                    'ALTER TABLE `{table}` ADD COLUMN `{column}` {definition}',
                    'SELECT 1');
                PREPARE add_voucher_column FROM @statement;
                EXECUTE add_voucher_column;
                DEALLOCATE PREPARE add_voucher_column;");
        }

        private static void AddIndexIfMissing(MigrationBuilder migrationBuilder, string table, string index, string column)
        {
            migrationBuilder.Sql($@"
                SET @statement = IF(
                    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS
                     WHERE TABLE_SCHEMA = DATABASE()
                       AND TABLE_NAME = '{table}'
                       AND INDEX_NAME = '{index}') = 0,
                    'CREATE INDEX `{index}` ON `{table}` (`{column}`)',
                    'SELECT 1');
                PREPARE add_voucher_index FROM @statement;
                EXECUTE add_voucher_index;
                DEALLOCATE PREPARE add_voucher_index;");
        }

        private static void AddForeignKeyIfMissing(MigrationBuilder migrationBuilder, string name, string table, string column, string principalTable, string principalColumn)
        {
            migrationBuilder.Sql($@"
                SET @statement = IF(
                    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                     WHERE CONSTRAINT_SCHEMA = DATABASE()
                       AND CONSTRAINT_NAME = '{name}') = 0,
                    'ALTER TABLE `{table}` ADD CONSTRAINT `{name}` FOREIGN KEY (`{column}`) REFERENCES `{principalTable}` (`{principalColumn}`) ON DELETE RESTRICT',
                    'SELECT 1');
                PREPARE add_voucher_fk FROM @statement;
                EXECUTE add_voucher_fk;
                DEALLOCATE PREPARE add_voucher_fk;");
        }
    }
}

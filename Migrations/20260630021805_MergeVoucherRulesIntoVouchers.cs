using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    public partial class MergeVoucherRulesIntoVouchers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE voucher_rules rule
                LEFT JOIN vouchers voucher ON voucher.id = rule.voucher_id
                SET rule.name = COALESCE(NULLIF(TRIM(voucher.code), ''), NULLIF(TRIM(rule.name), ''), CONCAT('Voucher #', rule.id))
                WHERE rule.voucher_id IS NOT NULL OR rule.name IS NULL OR TRIM(rule.name) = '';
                """);

            migrationBuilder.Sql("""
                UPDATE voucher_rules rule
                JOIN (
                    SELECT name
                    FROM (
                        SELECT name
                        FROM voucher_rules
                        GROUP BY name
                        HAVING COUNT(*) > 1
                    ) duplicate_names
                ) duplicates ON duplicates.name = rule.name
                SET rule.name = CONCAT(LEFT(rule.name, 90), ' #', rule.id);
                """);

            migrationBuilder.AlterColumn<int>(
                name: "voucher_id",
                table: "voucher_redemptions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_rules_name",
                table: "voucher_rules",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_voucher_rules_name",
                table: "voucher_rules");

            migrationBuilder.AlterColumn<int>(
                name: "voucher_id",
                table: "voucher_redemptions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}

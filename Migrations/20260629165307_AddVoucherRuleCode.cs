using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherRuleCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "voucher_rules",
                type: "varchar(6)",
                maxLength: 6,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_rules_code",
                table: "voucher_rules",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_voucher_rules_code",
                table: "voucher_rules");

            migrationBuilder.DropColumn(
                name: "code",
                table: "voucher_rules");
        }
    }
}

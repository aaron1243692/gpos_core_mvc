using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberEarningRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "earning_rules",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    earn_rate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_earning_rules", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "earning_rule_id",
                table: "members",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sale_id",
                table: "points_ledger",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_earning_rules_is_active",
                table: "earning_rules",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_earning_rules_name",
                table: "earning_rules",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_members_earning_rule_id",
                table: "members",
                column: "earning_rule_id");

            migrationBuilder.CreateIndex(
                name: "IX_points_ledger_sale_id",
                table: "points_ledger",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "IX_points_ledger_member_id_sale_id_transaction_type",
                table: "points_ledger",
                columns: new[] { "member_id", "sale_id", "transaction_type" });

            migrationBuilder.AddForeignKey(
                name: "FK_members_earning_rules_earning_rule_id",
                table: "members",
                column: "earning_rule_id",
                principalTable: "earning_rules",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_points_ledger_sales_sale_id",
                table: "points_ledger",
                column: "sale_id",
                principalTable: "sales",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_members_earning_rules_earning_rule_id",
                table: "members");

            migrationBuilder.DropForeignKey(
                name: "FK_points_ledger_sales_sale_id",
                table: "points_ledger");

            migrationBuilder.DropIndex(
                name: "IX_members_earning_rule_id",
                table: "members");

            migrationBuilder.DropIndex(
                name: "IX_points_ledger_sale_id",
                table: "points_ledger");

            migrationBuilder.DropIndex(
                name: "IX_points_ledger_member_id_sale_id_transaction_type",
                table: "points_ledger");

            migrationBuilder.DropColumn(
                name: "earning_rule_id",
                table: "members");

            migrationBuilder.DropColumn(
                name: "sale_id",
                table: "points_ledger");

            migrationBuilder.DropTable(
                name: "earning_rules");
        }
    }
}

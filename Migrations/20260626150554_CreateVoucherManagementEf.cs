using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    public partial class CreateVoucherManagementEf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "voucher_rules",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reward_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reward_value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    max_discount_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    minimum_purchase_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    applicable_product_ids = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    applicable_category_ids = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    applies_to = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    effective_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    expiration_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    no_expiration = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    max_redemptions = table.Column<int>(type: "int", nullable: true),
                    usage_limit_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    limited_use_count = table.Column<int>(type: "int", nullable: true),
                    priority = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voucher_rules", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vouchers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "Active")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vouchers", x => x.id);
                    table.ForeignKey(
                        name: "FK_vouchers_members_member_id",
                        column: x => x.member_id,
                        principalTable: "members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "voucher_redemptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    voucher_id = table.Column<int>(type: "int", nullable: false),
                    voucher_rule_id = table.Column<int>(type: "int", nullable: false),
                    sale_id = table.Column<int>(type: "int", nullable: false),
                    discount_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voucher_redemptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_voucher_redemptions_sales_sale_id",
                        column: x => x.sale_id,
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_voucher_redemptions_voucher_rules_voucher_rule_id",
                        column: x => x.voucher_rule_id,
                        principalTable: "voucher_rules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_voucher_redemptions_vouchers_voucher_id",
                        column: x => x.voucher_id,
                        principalTable: "vouchers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_redemptions_sale_id",
                table: "voucher_redemptions",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_redemptions_voucher_id",
                table: "voucher_redemptions",
                column: "voucher_id");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_redemptions_voucher_rule_id",
                table: "voucher_redemptions",
                column: "voucher_rule_id");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_rules_priority",
                table: "voucher_rules",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_rules_status",
                table: "voucher_rules",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_vouchers_code",
                table: "vouchers",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vouchers_member_id",
                table: "vouchers",
                column: "member_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "voucher_redemptions");

            migrationBuilder.DropTable(
                name: "voucher_rules");

            migrationBuilder.DropTable(
                name: "vouchers");
        }
    }
}

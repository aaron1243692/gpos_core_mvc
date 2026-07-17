using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditableProductSaleVoid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "reversed_points_ledger_id",
                table: "points_ledger",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sale_void_id",
                table: "points_ledger",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sale_voids",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sale_id = table.Column<int>(type: "int", nullable: false),
                    branch_id = table.Column<int>(type: "int", nullable: false),
                    original_daily_cash_id = table.Column<int>(type: "int", nullable: true),
                    daily_cash_id = table.Column<int>(type: "int", nullable: false),
                    requested_by_user_id = table.Column<int>(type: "int", nullable: false),
                    reason_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    original_receipt_no = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    original_business_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    void_business_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    original_gross_total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    original_discount_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    original_rebate_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    original_vat_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    original_vat_rate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    original_taxable_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    original_vat_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    reversed_vat_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    original_net_total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    original_applied_payment_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    completed_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale_voids", x => x.id);
                    table.ForeignKey(
                        name: "FK_sale_voids_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_voids_daily_cash_daily_cash_id",
                        column: x => x.daily_cash_id,
                        principalTable: "daily_cash",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_voids_daily_cash_original_daily_cash_id",
                        column: x => x.original_daily_cash_id,
                        principalTable: "daily_cash",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_voids_sales_sale_id",
                        column: x => x.sale_id,
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_voids_users_requested_by_user_id",
                        column: x => x.requested_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sale_void_cash_adjustments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sale_void_id = table.Column<int>(type: "int", nullable: false),
                    sale_id = table.Column<int>(type: "int", nullable: false),
                    daily_cash_id = table.Column<int>(type: "int", nullable: false),
                    branch_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    business_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    reason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale_void_cash_adjustments", x => x.id);
                    table.ForeignKey(
                        name: "FK_sale_void_cash_adjustments_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_void_cash_adjustments_daily_cash_daily_cash_id",
                        column: x => x.daily_cash_id,
                        principalTable: "daily_cash",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_void_cash_adjustments_sale_voids_sale_void_id",
                        column: x => x.sale_void_id,
                        principalTable: "sale_voids",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_void_cash_adjustments_sales_sale_id",
                        column: x => x.sale_id,
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_void_cash_adjustments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sale_void_payments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sale_void_id = table.Column<int>(type: "int", nullable: false),
                    payment_id = table.Column<int>(type: "int", nullable: false),
                    payment_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    original_applied_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    reversed_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    tendered_amount_snapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    change_amount_snapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    reference_no_snapshot = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    external_refund_status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by_user_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale_void_payments", x => x.id);
                    table.ForeignKey(
                        name: "FK_sale_void_payments_payments_payment_id",
                        column: x => x.payment_id,
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_void_payments_sale_voids_sale_void_id",
                        column: x => x.sale_void_id,
                        principalTable: "sale_voids",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_void_payments_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sale_void_product_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sale_void_id = table.Column<int>(type: "int", nullable: false),
                    product_sale_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    display_stock_id = table.Column<int>(type: "int", nullable: false),
                    batch_id = table.Column<int>(type: "int", nullable: false),
                    quantity_restored = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    unit_cost_snapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    unit_price_snapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    restored_value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    before_quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    after_quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    stock_movement_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale_void_product_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_sale_void_product_items_display_stocks_display_stock_id",
                        column: x => x.display_stock_id,
                        principalTable: "display_stocks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_void_product_items_product_batches_batch_id",
                        column: x => x.batch_id,
                        principalTable: "product_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_void_product_items_product_sales_product_sale_id",
                        column: x => x.product_sale_id,
                        principalTable: "product_sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_void_product_items_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_void_product_items_sale_voids_sale_void_id",
                        column: x => x.sale_void_id,
                        principalTable: "sale_voids",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_void_product_items_stock_movements_stock_movement_id",
                        column: x => x.stock_movement_id,
                        principalTable: "stock_movements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "voucher_redemption_reversals",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sale_void_id = table.Column<int>(type: "int", nullable: false),
                    voucher_redemption_id = table.Column<int>(type: "int", nullable: false),
                    voucher_id = table.Column<int>(type: "int", nullable: true),
                    member_id = table.Column<int>(type: "int", nullable: true),
                    voucher_code_snapshot = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    applied_discount_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    created_by_user_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    reason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voucher_redemption_reversals", x => x.id);
                    table.ForeignKey(
                        name: "FK_voucher_redemption_reversals_members_member_id",
                        column: x => x.member_id,
                        principalTable: "members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_voucher_redemption_reversals_sale_voids_sale_void_id",
                        column: x => x.sale_void_id,
                        principalTable: "sale_voids",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_voucher_redemption_reversals_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_voucher_redemption_reversals_voucher_redemptions_voucher_red~",
                        column: x => x.voucher_redemption_id,
                        principalTable: "voucher_redemptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_voucher_redemption_reversals_vouchers_voucher_id",
                        column: x => x.voucher_id,
                        principalTable: "vouchers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_points_ledger_reversed_points_ledger_id",
                table: "points_ledger",
                column: "reversed_points_ledger_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_points_ledger_sale_void_id",
                table: "points_ledger",
                column: "sale_void_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_void_cash_adjustments_branch_id",
                table: "sale_void_cash_adjustments",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_void_cash_adjustments_daily_cash_id",
                table: "sale_void_cash_adjustments",
                column: "daily_cash_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_void_cash_adjustments_sale_id",
                table: "sale_void_cash_adjustments",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_void_cash_adjustments_sale_void_id",
                table: "sale_void_cash_adjustments",
                column: "sale_void_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sale_void_cash_adjustments_user_id",
                table: "sale_void_cash_adjustments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_void_payments_created_by_user_id",
                table: "sale_void_payments",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_void_payments_payment_id",
                table: "sale_void_payments",
                column: "payment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sale_void_payments_sale_void_id",
                table: "sale_void_payments",
                column: "sale_void_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_void_product_items_batch_id",
                table: "sale_void_product_items",
                column: "batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_void_product_items_display_stock_id",
                table: "sale_void_product_items",
                column: "display_stock_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_void_product_items_product_id",
                table: "sale_void_product_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_void_product_items_product_sale_id",
                table: "sale_void_product_items",
                column: "product_sale_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sale_void_product_items_sale_void_id",
                table: "sale_void_product_items",
                column: "sale_void_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_void_product_items_stock_movement_id",
                table: "sale_void_product_items",
                column: "stock_movement_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sale_voids_branch_id",
                table: "sale_voids",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_voids_daily_cash_id",
                table: "sale_voids",
                column: "daily_cash_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_voids_original_daily_cash_id",
                table: "sale_voids",
                column: "original_daily_cash_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_voids_requested_by_user_id",
                table: "sale_voids",
                column: "requested_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_voids_sale_id",
                table: "sale_voids",
                column: "sale_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_voucher_redemption_reversals_created_by_user_id",
                table: "voucher_redemption_reversals",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_redemption_reversals_member_id",
                table: "voucher_redemption_reversals",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_redemption_reversals_sale_void_id",
                table: "voucher_redemption_reversals",
                column: "sale_void_id");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_redemption_reversals_voucher_id",
                table: "voucher_redemption_reversals",
                column: "voucher_id");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_redemption_reversals_voucher_redemption_id",
                table: "voucher_redemption_reversals",
                column: "voucher_redemption_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_points_ledger_points_ledger_reversed_points_ledger_id",
                table: "points_ledger",
                column: "reversed_points_ledger_id",
                principalTable: "points_ledger",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_points_ledger_sale_voids_sale_void_id",
                table: "points_ledger",
                column: "sale_void_id",
                principalTable: "sale_voids",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_points_ledger_points_ledger_reversed_points_ledger_id",
                table: "points_ledger");

            migrationBuilder.DropForeignKey(
                name: "FK_points_ledger_sale_voids_sale_void_id",
                table: "points_ledger");

            migrationBuilder.DropTable(
                name: "sale_void_cash_adjustments");

            migrationBuilder.DropTable(
                name: "sale_void_payments");

            migrationBuilder.DropTable(
                name: "sale_void_product_items");

            migrationBuilder.DropTable(
                name: "voucher_redemption_reversals");

            migrationBuilder.DropTable(
                name: "sale_voids");

            migrationBuilder.DropIndex(
                name: "IX_points_ledger_reversed_points_ledger_id",
                table: "points_ledger");

            migrationBuilder.DropIndex(
                name: "IX_points_ledger_sale_void_id",
                table: "points_ledger");

            migrationBuilder.DropColumn(
                name: "reversed_points_ledger_id",
                table: "points_ledger");

            migrationBuilder.DropColumn(
                name: "sale_void_id",
                table: "points_ledger");
        }
    }
}

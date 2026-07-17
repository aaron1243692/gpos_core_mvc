using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    /// <inheritdoc />
    public partial class RestoreFuelReconciliationAndAddImmutableSaleSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "eligible_base_snapshot",
                table: "voucher_redemptions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reward_type_snapshot",
                table: "voucher_redemptions",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "reward_value_snapshot",
                table: "voucher_redemptions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rule_name_snapshot",
                table: "voucher_redemptions",
                type: "varchar(150)",
                maxLength: 150,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "voucher_code_snapshot",
                table: "voucher_redemptions",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "adjustment_type",
                table: "stock_adjustments",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "approved_at",
                table: "stock_adjustments",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "approved_by",
                table: "stock_adjustments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cost_input_mode",
                table: "stock_adjustments",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "created_fuel_batch_id",
                table: "stock_adjustments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "evidence_reference",
                table: "stock_adjustments",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "total_cost",
                table: "stock_adjustments",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "unit_cost",
                table: "stock_adjustments",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "business_date",
                table: "sales",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "change_amount",
                table: "sales",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "completed_at",
                table: "sales",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "shift_id",
                table: "sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "taxable_amount",
                table: "sales",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "vat_amount",
                table: "sales",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "vat_exempt_amount",
                table: "sales",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "vat_name_snapshot",
                table: "sales",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "vat_rate",
                table: "sales",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "vat_setting_id",
                table: "sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "vat_type_snapshot",
                table: "sales",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "monetary_value_snapshot",
                table: "points_ledger",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "points_required_snapshot",
                table: "points_ledger",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rule_id_snapshot",
                table: "points_ledger",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rule_name_snapshot",
                table: "points_ledger",
                type: "varchar(150)",
                maxLength: 150,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "applied_amount",
                table: "payments",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "change_amount",
                table: "payments",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "tendered_amount",
                table: "payments",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sale_discount_applications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sale_id = table.Column<int>(type: "int", nullable: false),
                    source_type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rule_id = table.Column<int>(type: "int", nullable: true),
                    rule_name_snapshot = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    calculation_type_snapshot = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rate_or_value_snapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    eligible_base_snapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    applied_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale_discount_applications", x => x.id);
                    table.ForeignKey(
                        name: "FK_sale_discount_applications_sales_sale_id",
                        column: x => x.sale_id,
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_approved_by",
                table: "stock_adjustments",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_created_fuel_batch_id",
                table: "stock_adjustments",
                column: "created_fuel_batch_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sales_shift_id",
                table: "sales",
                column: "shift_id");

            migrationBuilder.CreateIndex(
                name: "IX_sales_vat_setting_id",
                table: "sales",
                column: "vat_setting_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_discount_applications_sale_id",
                table: "sale_discount_applications",
                column: "sale_id");

            migrationBuilder.AddForeignKey(
                name: "FK_sales_shift_settings_shift_id",
                table: "sales",
                column: "shift_id",
                principalTable: "shift_settings",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sales_vat_settings_vat_setting_id",
                table: "sales",
                column: "vat_setting_id",
                principalTable: "vat_settings",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_adjustments_fuel_batches_created_fuel_batch_id",
                table: "stock_adjustments",
                column: "created_fuel_batch_id",
                principalTable: "fuel_batches",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_adjustments_users_approved_by",
                table: "stock_adjustments",
                column: "approved_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
                INSERT INTO permissions (name, code, parent_id, created_at, updated_at, status)
                SELECT 'Fuel Adjustment', 'tranfueladjust', NULL, NOW(), NOW(), 1
                WHERE NOT EXISTS (SELECT 1 FROM permissions WHERE code = 'tranfueladjust');
                INSERT INTO permissions (name, code, parent_id, created_at, updated_at, status)
                SELECT 'Prepare Fuel Reconciliation', 'tranfueladjust.reconcile', p.id, NOW(), NOW(), 1
                FROM permissions p WHERE p.code = 'tranfueladjust'
                AND NOT EXISTS (SELECT 1 FROM permissions WHERE code = 'tranfueladjust.reconcile');
                INSERT INTO permissions (name, code, parent_id, created_at, updated_at, status)
                SELECT 'Approve Fuel Reconciliation', 'tranfueladjust.approve', p.id, NOW(), NOW(), 1
                FROM permissions p WHERE p.code = 'tranfueladjust'
                AND NOT EXISTS (SELECT 1 FROM permissions WHERE code = 'tranfueladjust.approve');");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM permissions WHERE code IN ('tranfueladjust.reconcile', 'tranfueladjust.approve'); DELETE FROM permissions WHERE code = 'tranfueladjust';");
            migrationBuilder.DropForeignKey(
                name: "FK_sales_shift_settings_shift_id",
                table: "sales");

            migrationBuilder.DropForeignKey(
                name: "FK_sales_vat_settings_vat_setting_id",
                table: "sales");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_adjustments_fuel_batches_created_fuel_batch_id",
                table: "stock_adjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_adjustments_users_approved_by",
                table: "stock_adjustments");

            migrationBuilder.DropTable(
                name: "sale_discount_applications");

            migrationBuilder.DropIndex(
                name: "IX_stock_adjustments_approved_by",
                table: "stock_adjustments");

            migrationBuilder.DropIndex(
                name: "IX_stock_adjustments_created_fuel_batch_id",
                table: "stock_adjustments");

            migrationBuilder.DropIndex(
                name: "IX_sales_shift_id",
                table: "sales");

            migrationBuilder.DropIndex(
                name: "IX_sales_vat_setting_id",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "eligible_base_snapshot",
                table: "voucher_redemptions");

            migrationBuilder.DropColumn(
                name: "reward_type_snapshot",
                table: "voucher_redemptions");

            migrationBuilder.DropColumn(
                name: "reward_value_snapshot",
                table: "voucher_redemptions");

            migrationBuilder.DropColumn(
                name: "rule_name_snapshot",
                table: "voucher_redemptions");

            migrationBuilder.DropColumn(
                name: "voucher_code_snapshot",
                table: "voucher_redemptions");

            migrationBuilder.DropColumn(
                name: "approved_at",
                table: "stock_adjustments");

            migrationBuilder.DropColumn(
                name: "approved_by",
                table: "stock_adjustments");

            migrationBuilder.DropColumn(
                name: "cost_input_mode",
                table: "stock_adjustments");

            migrationBuilder.DropColumn(
                name: "created_fuel_batch_id",
                table: "stock_adjustments");

            migrationBuilder.DropColumn(
                name: "evidence_reference",
                table: "stock_adjustments");

            migrationBuilder.DropColumn(
                name: "total_cost",
                table: "stock_adjustments");

            migrationBuilder.DropColumn(
                name: "unit_cost",
                table: "stock_adjustments");

            migrationBuilder.DropColumn(
                name: "business_date",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "change_amount",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "completed_at",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "shift_id",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "taxable_amount",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "vat_amount",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "vat_exempt_amount",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "vat_name_snapshot",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "vat_rate",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "vat_setting_id",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "vat_type_snapshot",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "monetary_value_snapshot",
                table: "points_ledger");

            migrationBuilder.DropColumn(
                name: "points_required_snapshot",
                table: "points_ledger");

            migrationBuilder.DropColumn(
                name: "rule_id_snapshot",
                table: "points_ledger");

            migrationBuilder.DropColumn(
                name: "rule_name_snapshot",
                table: "points_ledger");

            migrationBuilder.DropColumn(
                name: "applied_amount",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "change_amount",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "tendered_amount",
                table: "payments");

            migrationBuilder.AlterColumn<string>(
                name: "adjustment_type",
                table: "stock_adjustments",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldMaxLength: 30)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}

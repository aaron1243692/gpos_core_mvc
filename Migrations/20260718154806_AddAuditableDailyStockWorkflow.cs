using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditableDailyStockWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "adjustment_quantity",
                table: "daily_stock_records",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "confirmed_at",
                table: "daily_stock_records",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "confirmed_by",
                table: "daily_stock_records",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "current_official_quantity",
                table: "daily_stock_records",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "display_stock_id",
                table: "daily_stock_records",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "expected_quantity",
                table: "daily_stock_records",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "new_official_quantity",
                table: "daily_stock_records",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "received_quantity",
                table: "daily_stock_records",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "reconciliation_adjustment",
                table: "daily_stock_records",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "record_no",
                table: "daily_stock_records",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "daily_stock_records",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "transfer_in_quantity",
                table: "daily_stock_records",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "transfer_out_quantity",
                table: "daily_stock_records",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "variance_quantity",
                table: "daily_stock_records",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "warehouse_stock_id",
                table: "daily_stock_records",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE daily_stock_records
SET record_no = CONCAT('DS-LEGACY-', LPAD(id, 10, '0')),
    status = 'Legacy',
    expected_quantity = actual_quantity,
    variance_quantity = ending_quantity - actual_quantity,
    current_official_quantity = ending_quantity,
    new_official_quantity = ending_quantity");

            migrationBuilder.CreateIndex(
                name: "IX_daily_stock_records_confirmed_by",
                table: "daily_stock_records",
                column: "confirmed_by");

            migrationBuilder.CreateIndex(
                name: "IX_daily_stock_records_created_by",
                table: "daily_stock_records",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_daily_stock_records_record_no",
                table: "daily_stock_records",
                column: "record_no",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_daily_stock_records_users_confirmed_by",
                table: "daily_stock_records",
                column: "confirmed_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_daily_stock_records_users_created_by",
                table: "daily_stock_records",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_daily_stock_records_users_confirmed_by",
                table: "daily_stock_records");

            migrationBuilder.DropForeignKey(
                name: "FK_daily_stock_records_users_created_by",
                table: "daily_stock_records");

            migrationBuilder.DropIndex(
                name: "IX_daily_stock_records_confirmed_by",
                table: "daily_stock_records");

            migrationBuilder.DropIndex(
                name: "IX_daily_stock_records_created_by",
                table: "daily_stock_records");

            migrationBuilder.DropIndex(
                name: "IX_daily_stock_records_record_no",
                table: "daily_stock_records");

            migrationBuilder.DropColumn(
                name: "adjustment_quantity",
                table: "daily_stock_records");

            migrationBuilder.DropColumn(
                name: "confirmed_at",
                table: "daily_stock_records");

            migrationBuilder.DropColumn(
                name: "confirmed_by",
                table: "daily_stock_records");

            migrationBuilder.DropColumn(
                name: "current_official_quantity",
                table: "daily_stock_records");

            migrationBuilder.DropColumn(
                name: "display_stock_id",
                table: "daily_stock_records");

            migrationBuilder.DropColumn(
                name: "expected_quantity",
                table: "daily_stock_records");

            migrationBuilder.DropColumn(
                name: "new_official_quantity",
                table: "daily_stock_records");

            migrationBuilder.DropColumn(
                name: "received_quantity",
                table: "daily_stock_records");

            migrationBuilder.DropColumn(
                name: "reconciliation_adjustment",
                table: "daily_stock_records");

            migrationBuilder.DropColumn(
                name: "record_no",
                table: "daily_stock_records");

            migrationBuilder.DropColumn(
                name: "status",
                table: "daily_stock_records");

            migrationBuilder.DropColumn(
                name: "transfer_in_quantity",
                table: "daily_stock_records");

            migrationBuilder.DropColumn(
                name: "transfer_out_quantity",
                table: "daily_stock_records");

            migrationBuilder.DropColumn(
                name: "variance_quantity",
                table: "daily_stock_records");

            migrationBuilder.DropColumn(
                name: "warehouse_stock_id",
                table: "daily_stock_records");
        }
    }
}

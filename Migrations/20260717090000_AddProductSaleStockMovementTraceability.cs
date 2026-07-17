using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260717090000_AddProductSaleStockMovementTraceability")]
    public partial class AddProductSaleStockMovementTraceability : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "branch_id",
                table: "stock_movements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "display_stock_id",
                table: "stock_movements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "before_quantity",
                table: "stock_movements",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "after_quantity",
                table: "stock_movements",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_movements_branch_id",
                table: "stock_movements",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_movements_display_stock_id",
                table: "stock_movements",
                column: "display_stock_id");

            migrationBuilder.AddForeignKey(
                name: "FK_stock_movements_branches_branch_id",
                table: "stock_movements",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_movements_display_stocks_display_stock_id",
                table: "stock_movements",
                column: "display_stock_id",
                principalTable: "display_stocks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_stock_movements_branches_branch_id",
                table: "stock_movements");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_movements_display_stocks_display_stock_id",
                table: "stock_movements");

            migrationBuilder.DropIndex(
                name: "IX_stock_movements_branch_id",
                table: "stock_movements");

            migrationBuilder.DropIndex(
                name: "IX_stock_movements_display_stock_id",
                table: "stock_movements");

            migrationBuilder.DropColumn(name: "branch_id", table: "stock_movements");
            migrationBuilder.DropColumn(name: "display_stock_id", table: "stock_movements");
            migrationBuilder.DropColumn(name: "before_quantity", table: "stock_movements");
            migrationBuilder.DropColumn(name: "after_quantity", table: "stock_movements");
        }
    }
}

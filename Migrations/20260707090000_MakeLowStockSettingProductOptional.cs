using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260707090000_MakeLowStockSettingProductOptional")]
    public partial class MakeLowStockSettingProductOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_low_stock_settings_products_product_id",
                table: "low_stock_settings");

            migrationBuilder.AlterColumn<int>(
                name: "product_id",
                table: "low_stock_settings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_low_stock_settings_products_product_id",
                table: "low_stock_settings",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_low_stock_settings_products_product_id",
                table: "low_stock_settings");

            migrationBuilder.AlterColumn<int>(
                name: "product_id",
                table: "low_stock_settings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_low_stock_settings_products_product_id",
                table: "low_stock_settings",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260707100000_UpdateLowStockSettingsPerItem")]
    public partial class UpdateLowStockSettingsPerItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "tank_id",
                table: "low_stock_settings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "unit_label",
                table: "low_stock_settings",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_low_stock_settings_tank_id",
                table: "low_stock_settings",
                column: "tank_id");

            migrationBuilder.AddForeignKey(
                name: "FK_low_stock_settings_tanks_tank_id",
                table: "low_stock_settings",
                column: "tank_id",
                principalTable: "tanks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_low_stock_settings_tanks_tank_id",
                table: "low_stock_settings");

            migrationBuilder.DropIndex(
                name: "IX_low_stock_settings_tank_id",
                table: "low_stock_settings");

            migrationBuilder.DropColumn(
                name: "tank_id",
                table: "low_stock_settings");

            migrationBuilder.DropColumn(
                name: "unit_label",
                table: "low_stock_settings");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    /// <inheritdoc />
    public partial class FuelTankUnitPumpHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fuels_fuel_units_fuel_unit_id",
                table: "fuels");

            migrationBuilder.DropForeignKey(
                name: "FK_pumps_tanks_tank_id",
                table: "pumps");

            migrationBuilder.DropIndex(
                name: "IX_fuels_fuel_unit_id",
                table: "fuels");

            migrationBuilder.DropColumn(
                name: "fuel_unit_id",
                table: "fuels");

            migrationBuilder.RenameColumn(
                name: "tank_id",
                table: "pumps",
                newName: "fuel_unit_id");

            migrationBuilder.RenameIndex(
                name: "IX_pumps_tank_id",
                table: "pumps",
                newName: "IX_pumps_fuel_unit_id");

            migrationBuilder.AddColumn<int>(
                name: "tank_id",
                table: "fuel_units",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_fuel_units_tank_id",
                table: "fuel_units",
                column: "tank_id");

            migrationBuilder.AddForeignKey(
                name: "FK_fuel_units_tanks_tank_id",
                table: "fuel_units",
                column: "tank_id",
                principalTable: "tanks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_pumps_fuel_units_fuel_unit_id",
                table: "pumps",
                column: "fuel_unit_id",
                principalTable: "fuel_units",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fuel_units_tanks_tank_id",
                table: "fuel_units");

            migrationBuilder.DropForeignKey(
                name: "FK_pumps_fuel_units_fuel_unit_id",
                table: "pumps");

            migrationBuilder.DropIndex(
                name: "IX_fuel_units_tank_id",
                table: "fuel_units");

            migrationBuilder.DropColumn(
                name: "tank_id",
                table: "fuel_units");

            migrationBuilder.RenameColumn(
                name: "fuel_unit_id",
                table: "pumps",
                newName: "tank_id");

            migrationBuilder.RenameIndex(
                name: "IX_pumps_fuel_unit_id",
                table: "pumps",
                newName: "IX_pumps_tank_id");

            migrationBuilder.AddColumn<int>(
                name: "fuel_unit_id",
                table: "fuels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_fuels_fuel_unit_id",
                table: "fuels",
                column: "fuel_unit_id");

            migrationBuilder.AddForeignKey(
                name: "FK_fuels_fuel_units_fuel_unit_id",
                table: "fuels",
                column: "fuel_unit_id",
                principalTable: "fuel_units",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_pumps_tanks_tank_id",
                table: "pumps",
                column: "tank_id",
                principalTable: "tanks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

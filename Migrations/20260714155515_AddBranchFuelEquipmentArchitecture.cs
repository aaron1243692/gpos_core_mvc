using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchFuelEquipmentArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nozzles_pumps_pump_id",
                table: "nozzles");

            migrationBuilder.DropIndex(
                name: "IX_nozzles_pump_id",
                table: "nozzles");

            migrationBuilder.AddColumn<int>(
                name: "dispenser_id",
                table: "pumps",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "remarks",
                table: "pumps",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "nozzles",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "remarks",
                table: "nozzles",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "dispenser_id",
                table: "fuel_sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "pump_id",
                table: "fuel_sales",
                type: "int",
                nullable: true);

            // Deterministic legacy repair only: a Pump with no Branch inherits the
            // Branch of its existing Tank. No Dispenser or historical sale mapping
            // is guessed here.
            migrationBuilder.Sql("""
                UPDATE `pumps` AS p
                INNER JOIN `tanks` AS t ON t.`id` = p.`tank_id`
                SET p.`branch_id` = t.`branch_id`
                WHERE p.`branch_id` IS NULL AND t.`branch_id` IS NOT NULL;
                """);

            migrationBuilder.CreateTable(
                name: "dispensers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    branch_id = table.Column<int>(type: "int", nullable: false),
                    dispenser_code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    location = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    remarks = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispensers", x => x.id);
                    table.ForeignKey(
                        name: "FK_dispensers_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_pumps_dispenser_id",
                table: "pumps",
                column: "dispenser_id");

            migrationBuilder.CreateIndex(
                name: "IX_nozzles_pump_id",
                table: "nozzles",
                column: "pump_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_nozzles_pumps_pump_id",
                table: "nozzles",
                column: "pump_id",
                principalTable: "pumps",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.CreateIndex(
                name: "IX_fuel_sales_dispenser_id",
                table: "fuel_sales",
                column: "dispenser_id");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_sales_pump_id",
                table: "fuel_sales",
                column: "pump_id");

            migrationBuilder.CreateIndex(
                name: "IX_dispensers_branch_id_dispenser_code",
                table: "dispensers",
                columns: new[] { "branch_id", "dispenser_code" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_fuel_sales_dispensers_dispenser_id",
                table: "fuel_sales",
                column: "dispenser_id",
                principalTable: "dispensers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_fuel_sales_pumps_pump_id",
                table: "fuel_sales",
                column: "pump_id",
                principalTable: "pumps",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_pumps_dispensers_dispenser_id",
                table: "pumps",
                column: "dispenser_id",
                principalTable: "dispensers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nozzles_pumps_pump_id",
                table: "nozzles");

            migrationBuilder.DropForeignKey(
                name: "FK_fuel_sales_dispensers_dispenser_id",
                table: "fuel_sales");

            migrationBuilder.DropForeignKey(
                name: "FK_fuel_sales_pumps_pump_id",
                table: "fuel_sales");

            migrationBuilder.DropForeignKey(
                name: "FK_pumps_dispensers_dispenser_id",
                table: "pumps");

            migrationBuilder.DropTable(
                name: "dispensers");

            migrationBuilder.DropIndex(
                name: "IX_pumps_dispenser_id",
                table: "pumps");

            migrationBuilder.DropIndex(
                name: "IX_nozzles_pump_id",
                table: "nozzles");

            migrationBuilder.DropIndex(
                name: "IX_fuel_sales_dispenser_id",
                table: "fuel_sales");

            migrationBuilder.DropIndex(
                name: "IX_fuel_sales_pump_id",
                table: "fuel_sales");

            migrationBuilder.DropColumn(
                name: "dispenser_id",
                table: "pumps");

            migrationBuilder.DropColumn(
                name: "remarks",
                table: "pumps");

            migrationBuilder.DropColumn(
                name: "name",
                table: "nozzles");

            migrationBuilder.DropColumn(
                name: "remarks",
                table: "nozzles");

            migrationBuilder.DropColumn(
                name: "dispenser_id",
                table: "fuel_sales");

            migrationBuilder.DropColumn(
                name: "pump_id",
                table: "fuel_sales");

            migrationBuilder.CreateIndex(
                name: "IX_nozzles_pump_id",
                table: "nozzles",
                column: "pump_id");

            migrationBuilder.AddForeignKey(
                name: "FK_nozzles_pumps_pump_id",
                table: "nozzles",
                column: "pump_id",
                principalTable: "pumps",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

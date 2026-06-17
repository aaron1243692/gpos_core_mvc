using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFuelUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pumps_fuel_units_fuel_unit_id",
                table: "pumps");

            migrationBuilder.DropIndex(
                name: "IX_pumps_fuel_unit_id",
                table: "pumps");

            migrationBuilder.AddColumn<int>(
                name: "tank_id",
                table: "pumps",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE pumps p
                INNER JOIN fuel_units u ON p.fuel_unit_id = u.id
                SET p.tank_id = u.tank_id
                """);

            migrationBuilder.Sql("""
                UPDATE pumps p
                SET p.tank_id = (SELECT t.id FROM tanks t ORDER BY t.id LIMIT 1)
                WHERE p.tank_id IS NULL
                  AND EXISTS (SELECT 1 FROM tanks)
                """);

            migrationBuilder.Sql("DELETE FROM pumps WHERE tank_id IS NULL");

            migrationBuilder.DropColumn(
                name: "fuel_unit_id",
                table: "pumps");

            migrationBuilder.AlterColumn<int>(
                name: "tank_id",
                table: "pumps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropTable(
                name: "fuel_units");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "tanks",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "pumps",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "pumps",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Available")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "fuels",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_pumps_tank_id",
                table: "pumps",
                column: "tank_id");

            migrationBuilder.AddForeignKey(
                name: "FK_pumps_tanks_tank_id",
                table: "pumps",
                column: "tank_id",
                principalTable: "tanks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pumps_tanks_tank_id",
                table: "pumps");

            migrationBuilder.DropIndex(
                name: "IX_pumps_tank_id",
                table: "pumps");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "tanks");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "pumps");

            migrationBuilder.DropColumn(
                name: "status",
                table: "pumps");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "fuels");

            migrationBuilder.CreateTable(
                name: "fuel_units",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tank_id = table.Column<int>(type: "int", nullable: false),
                    abbreviation = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fuel_units", x => x.id);
                    table.ForeignKey(
                        name: "FK_fuel_units_tanks_tank_id",
                        column: x => x.tank_id,
                        principalTable: "tanks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_units_tank_id",
                table: "fuel_units",
                column: "tank_id");

            migrationBuilder.Sql("""
                INSERT INTO fuel_units (tank_id, abbreviation, created_at, name, updated_at)
                SELECT t.id, 'L', NOW(6), CONCAT('Unit ', t.tank_no), NOW(6)
                FROM tanks t
                """);

            migrationBuilder.AddColumn<int>(
                name: "fuel_unit_id",
                table: "pumps",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE pumps p
                INNER JOIN fuel_units u ON p.tank_id = u.tank_id
                SET p.fuel_unit_id = u.id
                """);

            migrationBuilder.Sql("DELETE FROM pumps WHERE fuel_unit_id IS NULL");

            migrationBuilder.DropColumn(
                name: "tank_id",
                table: "pumps");

            migrationBuilder.AlterColumn<int>(
                name: "fuel_unit_id",
                table: "pumps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_pumps_fuel_unit_id",
                table: "pumps",
                column: "fuel_unit_id");

            migrationBuilder.AddForeignKey(
                name: "FK_pumps_fuel_units_fuel_unit_id",
                table: "pumps",
                column: "fuel_unit_id",
                principalTable: "fuel_units",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260707013000_FixDispenserNozzleTankRelationship")]
    public partial class FixDispenserNozzleTankRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pumps_tanks_tank_id",
                table: "pumps");

            migrationBuilder.AlterColumn<int>(
                name: "tank_id",
                table: "pumps",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "tank_id",
                table: "nozzles",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE `nozzles` AS n
                INNER JOIN `pumps` AS p ON n.`pump_id` = p.`id`
                SET n.`tank_id` = p.`tank_id`
                WHERE n.`tank_id` IS NULL;");

            migrationBuilder.CreateIndex(
                name: "IX_nozzles_tank_id",
                table: "nozzles",
                column: "tank_id");

            migrationBuilder.AddForeignKey(
                name: "FK_nozzles_tanks_tank_id",
                table: "nozzles",
                column: "tank_id",
                principalTable: "tanks",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nozzles_tanks_tank_id",
                table: "nozzles");

            migrationBuilder.DropForeignKey(
                name: "FK_pumps_tanks_tank_id",
                table: "pumps");

            migrationBuilder.DropIndex(
                name: "IX_nozzles_tank_id",
                table: "nozzles");

            migrationBuilder.DropColumn(
                name: "tank_id",
                table: "nozzles");

            migrationBuilder.Sql(@"
                UPDATE `pumps`
                SET `tank_id` = (SELECT `id` FROM `tanks` ORDER BY `id` LIMIT 1)
                WHERE `tank_id` IS NULL;");

            migrationBuilder.AlterColumn<int>(
                name: "tank_id",
                table: "pumps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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

using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260707004000_AddNameAndRemarksToPumpMeterReadings")]
    public partial class AddNameAndRemarksToPumpMeterReadings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "pump_meter_readings",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Meter Reading")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "remarks",
                table: "pump_meter_readings",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "remarks",
                table: "pump_meter_readings");

            migrationBuilder.DropColumn(
                name: "name",
                table: "pump_meter_readings");
        }
    }
}

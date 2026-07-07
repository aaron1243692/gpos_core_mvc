using System;
using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260708003000_AddBatchIdToDailyStockRecords")]
    public partial class AddBatchIdToDailyStockRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "daily_stock_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    stock_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    record_date = table.Column<DateTime>(type: "date", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: true),
                    batch_id = table.Column<int>(type: "int", nullable: true),
                    tank_id = table.Column<int>(type: "int", nullable: true),
                    fuel_id = table.Column<int>(type: "int", nullable: true),
                    beginning_quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    sold_quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    actual_quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ending_quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    loss_quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    remarks = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_stock_records", x => x.id);
                    table.ForeignKey(
                        name: "FK_daily_stock_records_fuels_fuel_id",
                        column: x => x.fuel_id,
                        principalTable: "fuels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_daily_stock_records_product_batches_batch_id",
                        column: x => x.batch_id,
                        principalTable: "product_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_daily_stock_records_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_daily_stock_records_tanks_tank_id",
                        column: x => x.tank_id,
                        principalTable: "tanks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(name: "IX_daily_stock_records_batch_id", table: "daily_stock_records", column: "batch_id");
            migrationBuilder.CreateIndex(name: "IX_daily_stock_records_fuel_id", table: "daily_stock_records", column: "fuel_id");
            migrationBuilder.CreateIndex(name: "IX_daily_stock_records_product_id", table: "daily_stock_records", column: "product_id");
            migrationBuilder.CreateIndex(name: "IX_daily_stock_records_record_date", table: "daily_stock_records", column: "record_date");
            migrationBuilder.CreateIndex(name: "IX_daily_stock_records_stock_type", table: "daily_stock_records", column: "stock_type");
            migrationBuilder.CreateIndex(name: "IX_daily_stock_records_tank_id", table: "daily_stock_records", column: "tank_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "daily_stock_records");
        }
    }
}

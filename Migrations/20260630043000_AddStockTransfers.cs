using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    [Migration("20260630043000_AddStockTransfers")]
    public partial class AddStockTransfers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "branch_id",
                table: "warehouse_stocks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "branch_id",
                table: "tanks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "branch_id",
                table: "display_stocks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "stock_transfers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    transfer_no = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    transfer_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    source_branch_id = table.Column<int>(type: "int", nullable: true),
                    destination_branch_id = table.Column<int>(type: "int", nullable: true),
                    source_location = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    destination_location = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    remarks = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    transferred_by = table.Column<int>(type: "int", nullable: true),
                    completed_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    cancelled_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_transfers", x => x.id);
                    table.ForeignKey("FK_stock_transfers_branches_destination_branch_id", x => x.destination_branch_id, "branches", "id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_stock_transfers_branches_source_branch_id", x => x.source_branch_id, "branches", "id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_stock_transfers_users_transferred_by", x => x.transferred_by, "users", "id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_transfer_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    stock_transfer_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: true),
                    batch_id = table.Column<int>(type: "int", nullable: true),
                    fuel_id = table.Column<int>(type: "int", nullable: true),
                    source_tank_id = table.Column<int>(type: "int", nullable: true),
                    destination_tank_id = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    liters = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    source_before = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    source_after = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    destination_before = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    destination_after = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_transfer_items", x => x.id);
                    table.ForeignKey("FK_stock_transfer_items_stock_transfers_stock_transfer_id", x => x.stock_transfer_id, "stock_transfers", "id", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_stock_transfer_items_products_product_id", x => x.product_id, "products", "id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_stock_transfer_items_product_batches_batch_id", x => x.batch_id, "product_batches", "id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_stock_transfer_items_fuels_fuel_id", x => x.fuel_id, "fuels", "id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_stock_transfer_items_tanks_source_tank_id", x => x.source_tank_id, "tanks", "id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_stock_transfer_items_tanks_destination_tank_id", x => x.destination_tank_id, "tanks", "id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex("IX_warehouse_stocks_branch_id", "warehouse_stocks", "branch_id");
            migrationBuilder.CreateIndex("IX_tanks_branch_id", "tanks", "branch_id");
            migrationBuilder.CreateIndex("IX_display_stocks_branch_id", "display_stocks", "branch_id");
            migrationBuilder.CreateIndex("IX_stock_transfers_transfer_no", "stock_transfers", "transfer_no", unique: true);
            migrationBuilder.CreateIndex("IX_stock_transfers_source_branch_id", "stock_transfers", "source_branch_id");
            migrationBuilder.CreateIndex("IX_stock_transfers_destination_branch_id", "stock_transfers", "destination_branch_id");
            migrationBuilder.CreateIndex("IX_stock_transfers_transferred_by", "stock_transfers", "transferred_by");
            migrationBuilder.CreateIndex("IX_stock_transfer_items_stock_transfer_id", "stock_transfer_items", "stock_transfer_id");
            migrationBuilder.CreateIndex("IX_stock_transfer_items_product_id", "stock_transfer_items", "product_id");
            migrationBuilder.CreateIndex("IX_stock_transfer_items_batch_id", "stock_transfer_items", "batch_id");
            migrationBuilder.CreateIndex("IX_stock_transfer_items_fuel_id", "stock_transfer_items", "fuel_id");
            migrationBuilder.CreateIndex("IX_stock_transfer_items_source_tank_id", "stock_transfer_items", "source_tank_id");
            migrationBuilder.CreateIndex("IX_stock_transfer_items_destination_tank_id", "stock_transfer_items", "destination_tank_id");
            migrationBuilder.AddForeignKey("FK_warehouse_stocks_branches_branch_id", "warehouse_stocks", "branch_id", "branches", principalColumn: "id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_tanks_branches_branch_id", "tanks", "branch_id", "branches", principalColumn: "id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey("FK_display_stocks_branches_branch_id", "display_stocks", "branch_id", "branches", principalColumn: "id", onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "stock_transfer_items");
            migrationBuilder.DropTable(name: "stock_transfers");
            migrationBuilder.DropForeignKey("FK_warehouse_stocks_branches_branch_id", "warehouse_stocks");
            migrationBuilder.DropForeignKey("FK_tanks_branches_branch_id", "tanks");
            migrationBuilder.DropForeignKey("FK_display_stocks_branches_branch_id", "display_stocks");
            migrationBuilder.DropIndex("IX_warehouse_stocks_branch_id", "warehouse_stocks");
            migrationBuilder.DropIndex("IX_tanks_branch_id", "tanks");
            migrationBuilder.DropIndex("IX_display_stocks_branch_id", "display_stocks");
            migrationBuilder.DropColumn(name: "branch_id", table: "warehouse_stocks");
            migrationBuilder.DropColumn(name: "branch_id", table: "tanks");
            migrationBuilder.DropColumn(name: "branch_id", table: "display_stocks");
        }
    }
}

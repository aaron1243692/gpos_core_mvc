using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerReturnWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customer_returns",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    return_no = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sale_id = table.Column<int>(type: "int", nullable: false),
                    original_receipt_no = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    branch_id = table.Column<int>(type: "int", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: true),
                    return_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    refund_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    reason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by_user_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    inspected_by_user_id = table.Column<int>(type: "int", nullable: true),
                    inspected_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    inspection_decision = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    inspection_notes = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    processed_by_user_id = table.Column<int>(type: "int", nullable: true),
                    completed_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_returns", x => x.id);
                    table.ForeignKey(
                        name: "FK_customer_returns_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_returns_members_member_id",
                        column: x => x.member_id,
                        principalTable: "members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_returns_sales_sale_id",
                        column: x => x.sale_id,
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_returns_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_returns_users_inspected_by_user_id",
                        column: x => x.inspected_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_returns_users_processed_by_user_id",
                        column: x => x.processed_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "customer_fuel_return_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    customer_return_id = table.Column<int>(type: "int", nullable: false),
                    fuel_sale_id = table.Column<int>(type: "int", nullable: false),
                    fuel_id = table.Column<int>(type: "int", nullable: false),
                    original_tank_id = table.Column<int>(type: "int", nullable: false),
                    original_pump_id = table.Column<int>(type: "int", nullable: true),
                    original_nozzle_id = table.Column<int>(type: "int", nullable: true),
                    liters = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    original_price_per_liter = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    return_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    inspection_result = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    disposition = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_fuel_return_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_customer_fuel_return_items_customer_returns_customer_return_~",
                        column: x => x.customer_return_id,
                        principalTable: "customer_returns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_fuel_return_items_fuel_sales_fuel_sale_id",
                        column: x => x.fuel_sale_id,
                        principalTable: "fuel_sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_fuel_return_items_fuels_fuel_id",
                        column: x => x.fuel_id,
                        principalTable: "fuels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_fuel_return_items_nozzles_original_nozzle_id",
                        column: x => x.original_nozzle_id,
                        principalTable: "nozzles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_fuel_return_items_pumps_original_pump_id",
                        column: x => x.original_pump_id,
                        principalTable: "pumps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_fuel_return_items_tanks_original_tank_id",
                        column: x => x.original_tank_id,
                        principalTable: "tanks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "customer_product_return_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    customer_return_id = table.Column<int>(type: "int", nullable: false),
                    product_sale_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    original_batch_id = table.Column<int>(type: "int", nullable: true),
                    original_display_stock_id = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    original_unit_price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    return_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    inspection_result = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    disposition = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    stock_movement_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_product_return_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_customer_product_return_items_customer_returns_customer_retu~",
                        column: x => x.customer_return_id,
                        principalTable: "customer_returns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_product_return_items_display_stocks_original_displa~",
                        column: x => x.original_display_stock_id,
                        principalTable: "display_stocks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_product_return_items_product_batches_original_batch~",
                        column: x => x.original_batch_id,
                        principalTable: "product_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_product_return_items_product_sales_product_sale_id",
                        column: x => x.product_sale_id,
                        principalTable: "product_sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_product_return_items_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_product_return_items_stock_movements_stock_movement~",
                        column: x => x.stock_movement_id,
                        principalTable: "stock_movements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_customer_fuel_return_items_customer_return_id",
                table: "customer_fuel_return_items",
                column: "customer_return_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_fuel_return_items_fuel_id",
                table: "customer_fuel_return_items",
                column: "fuel_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_fuel_return_items_fuel_sale_id",
                table: "customer_fuel_return_items",
                column: "fuel_sale_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_fuel_return_items_original_nozzle_id",
                table: "customer_fuel_return_items",
                column: "original_nozzle_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_fuel_return_items_original_pump_id",
                table: "customer_fuel_return_items",
                column: "original_pump_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_fuel_return_items_original_tank_id",
                table: "customer_fuel_return_items",
                column: "original_tank_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_product_return_items_customer_return_id",
                table: "customer_product_return_items",
                column: "customer_return_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_product_return_items_original_batch_id",
                table: "customer_product_return_items",
                column: "original_batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_product_return_items_original_display_stock_id",
                table: "customer_product_return_items",
                column: "original_display_stock_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_product_return_items_product_id",
                table: "customer_product_return_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_product_return_items_product_sale_id",
                table: "customer_product_return_items",
                column: "product_sale_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_product_return_items_stock_movement_id",
                table: "customer_product_return_items",
                column: "stock_movement_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customer_returns_branch_id_status_created_at",
                table: "customer_returns",
                columns: new[] { "branch_id", "status", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_customer_returns_created_by_user_id",
                table: "customer_returns",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_returns_inspected_by_user_id",
                table: "customer_returns",
                column: "inspected_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_returns_member_id",
                table: "customer_returns",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_returns_processed_by_user_id",
                table: "customer_returns",
                column: "processed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_returns_return_no",
                table: "customer_returns",
                column: "return_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customer_returns_sale_id",
                table: "customer_returns",
                column: "sale_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_fuel_return_items");

            migrationBuilder.DropTable(
                name: "customer_product_return_items");

            migrationBuilder.DropTable(
                name: "customer_returns");
        }
    }
}

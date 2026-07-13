using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    public partial class CreateStockAdjustmentAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stock_adjustments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    adjustment_no = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    scope = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    business_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    branch_id = table.Column<int>(type: "int", nullable: false),
                    warehouse_stock_id = table.Column<int>(type: "int", nullable: true),
                    display_stock_id = table.Column<int>(type: "int", nullable: true),
                    tank_id = table.Column<int>(type: "int", nullable: true),
                    product_id = table.Column<int>(type: "int", nullable: true),
                    batch_id = table.Column<int>(type: "int", nullable: true),
                    fuel_id = table.Column<int>(type: "int", nullable: true),
                    adjustment_type = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    before_quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    adjustment_quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    signed_quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    after_quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    reason = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    remarks = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    adjusted_by = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    posted_by = table.Column<int>(type: "int", nullable: true),
                    posted_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    cancelled_by = table.Column<int>(type: "int", nullable: true),
                    cancelled_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    reversal_of_adjustment_id = table.Column<int>(type: "int", nullable: true),
                    reversed_by_adjustment_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_adjustments", x => x.id);
                    table.ForeignKey(
                        name: "FK_stock_adjustments_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_adjustments_display_stocks_display_stock_id",
                        column: x => x.display_stock_id,
                        principalTable: "display_stocks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_adjustments_fuels_fuel_id",
                        column: x => x.fuel_id,
                        principalTable: "fuels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_adjustments_product_batches_batch_id",
                        column: x => x.batch_id,
                        principalTable: "product_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_adjustments_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_adjustments_stock_adjustments_reversal_of_adjustment_id",
                        column: x => x.reversal_of_adjustment_id,
                        principalTable: "stock_adjustments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_adjustments_tanks_tank_id",
                        column: x => x.tank_id,
                        principalTable: "tanks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_adjustments_users_adjusted_by",
                        column: x => x.adjusted_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_adjustments_users_posted_by",
                        column: x => x.posted_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_adjustments_warehouse_stocks_warehouse_stock_id",
                        column: x => x.warehouse_stock_id,
                        principalTable: "warehouse_stocks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_adjusted_by",
                table: "stock_adjustments",
                column: "adjusted_by");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_adjustment_no",
                table: "stock_adjustments",
                column: "adjustment_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_batch_id",
                table: "stock_adjustments",
                column: "batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_branch_id",
                table: "stock_adjustments",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_display_stock_id",
                table: "stock_adjustments",
                column: "display_stock_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_fuel_id",
                table: "stock_adjustments",
                column: "fuel_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_posted_by",
                table: "stock_adjustments",
                column: "posted_by");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_product_id",
                table: "stock_adjustments",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_reversal_of_adjustment_id",
                table: "stock_adjustments",
                column: "reversal_of_adjustment_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_scope_branch_id_business_date_status",
                table: "stock_adjustments",
                columns: new[] { "scope", "branch_id", "business_date", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_tank_id",
                table: "stock_adjustments",
                column: "tank_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_warehouse_stock_id",
                table: "stock_adjustments",
                column: "warehouse_stock_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stock_adjustments");
        }
    }
}

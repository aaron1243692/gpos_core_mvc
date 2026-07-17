using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthoritativeFuelFifo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "gross_profit_snapshot",
                table: "fuel_sales",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "total_cost_snapshot",
                table: "fuel_sales",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "unit_cost_snapshot",
                table: "fuel_sales",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "fuel_stock_movements",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tank_id = table.Column<int>(type: "int", nullable: false),
                    fuel_id = table.Column<int>(type: "int", nullable: false),
                    fuel_batch_id = table.Column<int>(type: "int", nullable: false),
                    branch_id = table.Column<int>(type: "int", nullable: false),
                    movement_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    liters_in = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    liters_out = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    batch_liters_before = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    batch_liters_after = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    tank_liters_before = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    tank_liters_after = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    unit_cost_snapshot = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    reference_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reference_id = table.Column<int>(type: "int", nullable: false),
                    remarks = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by_user_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fuel_stock_movements", x => x.id);
                    table.ForeignKey(
                        name: "FK_fuel_stock_movements_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fuel_stock_movements_fuel_batches_fuel_batch_id",
                        column: x => x.fuel_batch_id,
                        principalTable: "fuel_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fuel_stock_movements_fuels_fuel_id",
                        column: x => x.fuel_id,
                        principalTable: "fuels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fuel_stock_movements_tanks_tank_id",
                        column: x => x.tank_id,
                        principalTable: "tanks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fuel_stock_movements_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "fuel_sale_batch_allocations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fuel_sale_id = table.Column<int>(type: "int", nullable: false),
                    fuel_batch_id = table.Column<int>(type: "int", nullable: false),
                    tank_id = table.Column<int>(type: "int", nullable: false),
                    fuel_id = table.Column<int>(type: "int", nullable: false),
                    branch_id = table.Column<int>(type: "int", nullable: false),
                    liters_allocated = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    batch_liters_before = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    batch_liters_after = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    unit_cost_snapshot = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    total_cost_snapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    unit_price_snapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    revenue_snapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    gross_profit_snapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    fuel_stock_movement_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fuel_sale_batch_allocations", x => x.id);
                    table.ForeignKey(
                        name: "FK_fuel_sale_batch_allocations_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fuel_sale_batch_allocations_fuel_batches_fuel_batch_id",
                        column: x => x.fuel_batch_id,
                        principalTable: "fuel_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fuel_sale_batch_allocations_fuel_sales_fuel_sale_id",
                        column: x => x.fuel_sale_id,
                        principalTable: "fuel_sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fuel_sale_batch_allocations_fuel_stock_movements_fuel_stock_~",
                        column: x => x.fuel_stock_movement_id,
                        principalTable: "fuel_stock_movements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fuel_sale_batch_allocations_fuels_fuel_id",
                        column: x => x.fuel_id,
                        principalTable: "fuels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fuel_sale_batch_allocations_tanks_tank_id",
                        column: x => x.tank_id,
                        principalTable: "tanks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_sale_batch_allocations_branch_id",
                table: "fuel_sale_batch_allocations",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_sale_batch_allocations_fuel_batch_id",
                table: "fuel_sale_batch_allocations",
                column: "fuel_batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_sale_batch_allocations_fuel_id",
                table: "fuel_sale_batch_allocations",
                column: "fuel_id");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_sale_batch_allocations_fuel_sale_id",
                table: "fuel_sale_batch_allocations",
                column: "fuel_sale_id");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_sale_batch_allocations_fuel_sale_id_fuel_batch_id",
                table: "fuel_sale_batch_allocations",
                columns: new[] { "fuel_sale_id", "fuel_batch_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fuel_sale_batch_allocations_fuel_stock_movement_id",
                table: "fuel_sale_batch_allocations",
                column: "fuel_stock_movement_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fuel_sale_batch_allocations_tank_id",
                table: "fuel_sale_batch_allocations",
                column: "tank_id");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_stock_movements_branch_id",
                table: "fuel_stock_movements",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_stock_movements_created_by_user_id",
                table: "fuel_stock_movements",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_stock_movements_fuel_batch_id",
                table: "fuel_stock_movements",
                column: "fuel_batch_id");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_stock_movements_fuel_id",
                table: "fuel_stock_movements",
                column: "fuel_id");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_stock_movements_reference_type_reference_id",
                table: "fuel_stock_movements",
                columns: new[] { "reference_type", "reference_id" });

            migrationBuilder.CreateIndex(
                name: "IX_fuel_stock_movements_tank_id",
                table: "fuel_stock_movements",
                column: "tank_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fuel_sale_batch_allocations");

            migrationBuilder.DropTable(
                name: "fuel_stock_movements");

            migrationBuilder.DropColumn(
                name: "gross_profit_snapshot",
                table: "fuel_sales");

            migrationBuilder.DropColumn(
                name: "total_cost_snapshot",
                table: "fuel_sales");

            migrationBuilder.DropColumn(
                name: "unit_cost_snapshot",
                table: "fuel_sales");
        }
    }
}

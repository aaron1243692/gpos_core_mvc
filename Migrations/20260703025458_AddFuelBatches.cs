using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddFuelBatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fuel_batches",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fuel_id = table.Column<int>(type: "int", nullable: false),
                    supplier_id = table.Column<int>(type: "int", nullable: true),
                    tank_id = table.Column<int>(type: "int", nullable: true),
                    branch_id = table.Column<int>(type: "int", nullable: true),
                    fuel_delivery_id = table.Column<int>(type: "int", nullable: true),
                    batch_no = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cost_price_per_liter = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    selling_price_per_liter = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    received_liters = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    remaining_liters = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    received_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    remarks = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fuel_batches", x => x.id);
                    table.CheckConstraint("CK_fuel_batches_cost_price_non_negative", "cost_price_per_liter >= 0");
                    table.CheckConstraint("CK_fuel_batches_received_liters_non_negative", "received_liters >= 0");
                    table.CheckConstraint("CK_fuel_batches_remaining_liters_non_negative", "remaining_liters >= 0");
                    table.CheckConstraint("CK_fuel_batches_selling_price_non_negative", "selling_price_per_liter >= 0");
                    table.ForeignKey(
                        name: "FK_fuel_batches_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fuel_batches_fuel_deliveries_fuel_delivery_id",
                        column: x => x.fuel_delivery_id,
                        principalTable: "fuel_deliveries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fuel_batches_fuels_fuel_id",
                        column: x => x.fuel_id,
                        principalTable: "fuels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fuel_batches_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fuel_batches_tanks_tank_id",
                        column: x => x.tank_id,
                        principalTable: "tanks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_batches_batch_no",
                table: "fuel_batches",
                column: "batch_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fuel_batches_branch_id",
                table: "fuel_batches",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_batches_fuel_delivery_id",
                table: "fuel_batches",
                column: "fuel_delivery_id");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_batches_fuel_id",
                table: "fuel_batches",
                column: "fuel_id");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_batches_fuel_id_status_is_active_received_date_id",
                table: "fuel_batches",
                columns: new[] { "fuel_id", "status", "is_active", "received_date", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_fuel_batches_supplier_id",
                table: "fuel_batches",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_fuel_batches_tank_id",
                table: "fuel_batches",
                column: "tank_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fuel_batches");
        }
    }
}

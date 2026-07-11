using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260710164000_AddBranchToPriceAdjustments")]
    public partial class AddBranchToPriceAdjustments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(name: "branch_id", table: "fuel_price_history", type: "int", nullable: true);
            migrationBuilder.AddColumn<string>(name: "reason", table: "fuel_price_history", type: "varchar(255)", maxLength: 255, nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
            migrationBuilder.AddColumn<int>(name: "branch_id", table: "product_price_history", type: "int", nullable: true);
            migrationBuilder.AddColumn<string>(name: "reason", table: "product_price_history", type: "varchar(255)", maxLength: 255, nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
            migrationBuilder.CreateIndex(name: "IX_fuel_price_history_branch_id", table: "fuel_price_history", column: "branch_id");
            migrationBuilder.CreateIndex(name: "IX_product_price_history_branch_id", table: "product_price_history", column: "branch_id");
            migrationBuilder.AddForeignKey(name: "FK_fuel_price_history_branches_branch_id", table: "fuel_price_history", column: "branch_id", principalTable: "branches", principalColumn: "id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "FK_product_price_history_branches_branch_id", table: "product_price_history", column: "branch_id", principalTable: "branches", principalColumn: "id", onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_fuel_price_history_branches_branch_id", table: "fuel_price_history");
            migrationBuilder.DropForeignKey(name: "FK_product_price_history_branches_branch_id", table: "product_price_history");
            migrationBuilder.DropIndex(name: "IX_fuel_price_history_branch_id", table: "fuel_price_history");
            migrationBuilder.DropIndex(name: "IX_product_price_history_branch_id", table: "product_price_history");
            migrationBuilder.DropColumn(name: "branch_id", table: "fuel_price_history");
            migrationBuilder.DropColumn(name: "reason", table: "fuel_price_history");
            migrationBuilder.DropColumn(name: "branch_id", table: "product_price_history");
            migrationBuilder.DropColumn(name: "reason", table: "product_price_history");
        }
    }
}

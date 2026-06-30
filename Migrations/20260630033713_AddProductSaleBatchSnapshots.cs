using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSaleBatchSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "display_stock_id",
                table: "product_sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "gross_profit",
                table: "product_sales",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "unit_cost",
                table: "product_sales",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "unit_price",
                table: "product_sales",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql("""
                UPDATE `product_sales` ps
                LEFT JOIN `product_batches` pb ON pb.`id` = ps.`batch_id`
                SET
                    ps.`unit_price` = COALESCE(NULLIF(ps.`price`, 0), pb.`selling_price`, 0),
                    ps.`unit_cost` = COALESCE(pb.`cost_price`, 0),
                    ps.`gross_profit` = (COALESCE(NULLIF(ps.`price`, 0), pb.`selling_price`, 0) - COALESCE(pb.`cost_price`, 0)) * ps.`quantity`
                WHERE ps.`unit_price` = 0
                    AND ps.`unit_cost` = 0
                    AND ps.`gross_profit` = 0;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_product_sales_display_stock_id",
                table: "product_sales",
                column: "display_stock_id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_sales_display_stocks_display_stock_id",
                table: "product_sales",
                column: "display_stock_id",
                principalTable: "display_stocks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_sales_display_stocks_display_stock_id",
                table: "product_sales");

            migrationBuilder.DropIndex(
                name: "IX_product_sales_display_stock_id",
                table: "product_sales");

            migrationBuilder.DropColumn(
                name: "display_stock_id",
                table: "product_sales");

            migrationBuilder.DropColumn(
                name: "gross_profit",
                table: "product_sales");

            migrationBuilder.DropColumn(
                name: "unit_cost",
                table: "product_sales");

            migrationBuilder.DropColumn(
                name: "unit_price",
                table: "product_sales");
        }
    }
}

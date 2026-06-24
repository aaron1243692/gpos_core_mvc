using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260624093000_SeparateProductAndFuelSales")]
    public partial class SeparateProductAndFuelSales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `product_sales` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `sale_id` int NOT NULL,
                    `product_id` int NOT NULL,
                    `batch_id` int NULL,
                    `quantity` decimal(18,2) NOT NULL,
                    `price` decimal(18,2) NOT NULL,
                    `subtotal` decimal(18,2) NOT NULL,
                    `display_stock_before` decimal(18,2) NULL,
                    `display_stock_after` decimal(18,2) NULL,
                    `status` varchar(50) NOT NULL DEFAULT 'Completed',
                    `created_at` datetime(6) NULL,
                    `updated_at` datetime(6) NULL,
                    CONSTRAINT `pk_product_sales` PRIMARY KEY (`id`),
                    KEY `ix_product_sales_sale_id` (`sale_id`),
                    KEY `ix_product_sales_product_id` (`product_id`),
                    KEY `ix_product_sales_batch_id` (`batch_id`),
                    CONSTRAINT `fk_product_sales_sales_sale_id` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`) ON DELETE CASCADE,
                    CONSTRAINT `fk_product_sales_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `fk_product_sales_product_batches_batch_id` FOREIGN KEY (`batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT
                ) CHARACTER SET=utf8mb4;
                """);

            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `fuel_sales` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `sale_id` int NOT NULL,
                    `fuel_id` int NOT NULL,
                    `tank_id` int NOT NULL,
                    `nozzle_id` int NULL,
                    `liters` decimal(18,2) NOT NULL,
                    `price_per_liter` decimal(18,2) NOT NULL,
                    `subtotal` decimal(18,2) NOT NULL,
                    `tank_liters_before` decimal(18,2) NULL,
                    `tank_liters_after` decimal(18,2) NULL,
                    `status` varchar(50) NOT NULL DEFAULT 'Completed',
                    `created_at` datetime(6) NULL,
                    `updated_at` datetime(6) NULL,
                    CONSTRAINT `pk_fuel_sales` PRIMARY KEY (`id`),
                    KEY `ix_fuel_sales_sale_id` (`sale_id`),
                    KEY `ix_fuel_sales_fuel_id` (`fuel_id`),
                    KEY `ix_fuel_sales_tank_id` (`tank_id`),
                    KEY `ix_fuel_sales_nozzle_id` (`nozzle_id`),
                    CONSTRAINT `fk_fuel_sales_sales_sale_id` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`) ON DELETE CASCADE,
                    CONSTRAINT `fk_fuel_sales_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `fk_fuel_sales_tanks_tank_id` FOREIGN KEY (`tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `fk_fuel_sales_nozzles_nozzle_id` FOREIGN KEY (`nozzle_id`) REFERENCES `nozzles` (`id`) ON DELETE RESTRICT
                ) CHARACTER SET=utf8mb4;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS `fuel_sales`;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS `product_sales`;");
        }
    }
}

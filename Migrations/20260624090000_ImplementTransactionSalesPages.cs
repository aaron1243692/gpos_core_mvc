using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260624090000_ImplementTransactionSalesPages")]
    public partial class ImplementTransactionSalesPages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `sales` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `receipt_no` varchar(100) NOT NULL,
                    `user_id` int NOT NULL,
                    `member_id` int NULL,
                    `gross_total` decimal(18,2) NOT NULL,
                    `discount_amount` decimal(18,2) NOT NULL DEFAULT 0.00,
                    `rebate_amount` decimal(18,2) NOT NULL DEFAULT 0.00,
                    `net_total` decimal(18,2) NOT NULL,
                    `cash_amount` decimal(18,2) NOT NULL DEFAULT 0.00,
                    `status` varchar(50) NOT NULL DEFAULT 'Completed',
                    `created_at` datetime(6) NULL,
                    `updated_at` datetime(6) NULL,
                    CONSTRAINT `pk_sales` PRIMARY KEY (`id`),
                    CONSTRAINT `ak_sales_receipt_no` UNIQUE (`receipt_no`),
                    KEY `ix_sales_user_id` (`user_id`),
                    KEY `ix_sales_member_id` (`member_id`),
                    CONSTRAINT `fk_sales_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `fk_sales_members_member_id` FOREIGN KEY (`member_id`) REFERENCES `members` (`id`) ON DELETE RESTRICT
                ) CHARACTER SET=utf8mb4;
                """);

            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `sale_items` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `sale_id` int NOT NULL,
                    `item_type` varchar(50) NOT NULL,
                    `product_id` int NULL,
                    `fuel_id` int NULL,
                    `tank_id` int NULL,
                    `nozzle_id` int NULL,
                    `batch_id` int NULL,
                    `quantity` decimal(18,2) NULL,
                    `liters` decimal(18,2) NULL,
                    `price` decimal(18,2) NOT NULL,
                    `subtotal` decimal(18,2) NOT NULL,
                    `status` varchar(50) NOT NULL DEFAULT 'Completed',
                    `created_at` datetime(6) NULL,
                    `updated_at` datetime(6) NULL,
                    CONSTRAINT `pk_sale_items` PRIMARY KEY (`id`),
                    KEY `ix_sale_items_sale_id` (`sale_id`),
                    KEY `ix_sale_items_item_type` (`item_type`),
                    KEY `ix_sale_items_product_id` (`product_id`),
                    KEY `ix_sale_items_fuel_id` (`fuel_id`),
                    KEY `ix_sale_items_tank_id` (`tank_id`),
                    KEY `ix_sale_items_nozzle_id` (`nozzle_id`),
                    KEY `ix_sale_items_batch_id` (`batch_id`),
                    CONSTRAINT `fk_sale_items_sales_sale_id` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`) ON DELETE CASCADE,
                    CONSTRAINT `fk_sale_items_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `fk_sale_items_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `fk_sale_items_tanks_tank_id` FOREIGN KEY (`tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `fk_sale_items_nozzles_nozzle_id` FOREIGN KEY (`nozzle_id`) REFERENCES `nozzles` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `fk_sale_items_product_batches_batch_id` FOREIGN KEY (`batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT
                ) CHARACTER SET=utf8mb4;
                """);

            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `payments` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `sale_id` int NOT NULL,
                    `payment_method_id` int NULL,
                    `payment_type` varchar(50) NOT NULL,
                    `amount` decimal(18,2) NOT NULL,
                    `reference_no` varchar(100) NULL,
                    `status` varchar(50) NOT NULL DEFAULT 'Completed',
                    `created_at` datetime(6) NULL,
                    `updated_at` datetime(6) NULL,
                    CONSTRAINT `pk_payments` PRIMARY KEY (`id`),
                    KEY `ix_payments_sale_id` (`sale_id`),
                    KEY `ix_payments_payment_method_id` (`payment_method_id`),
                    CONSTRAINT `fk_payments_sales_sale_id` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`) ON DELETE CASCADE,
                    CONSTRAINT `fk_payments_payment_methods_payment_method_id` FOREIGN KEY (`payment_method_id`) REFERENCES `payment_methods` (`id`) ON DELETE RESTRICT
                ) CHARACTER SET=utf8mb4;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS `payments`;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS `sale_items`;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS `sales`;");
        }
    }
}

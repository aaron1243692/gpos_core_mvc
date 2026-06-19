using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260619001000_AddMissingSetupModules")]
    public partial class AddMissingSetupModules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `product_units` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `name` varchar(100) NOT NULL,
                    `abbreviation` varchar(30) NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET=utf8mb4;

                SET @product_unit_id_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'products' AND COLUMN_NAME = 'product_unit_id');
                SET @statement = IF(@product_unit_id_exists = 0, 'ALTER TABLE `products` ADD COLUMN `product_unit_id` int NULL AFTER `category_id`', 'SELECT 1');
                PREPARE add_product_unit_id FROM @statement; EXECUTE add_product_unit_id; DEALLOCATE PREPARE add_product_unit_id;

                SET @batch_supplier_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'product_batches' AND COLUMN_NAME = 'supplier_id');
                SET @statement = IF(@batch_supplier_exists = 0, 'ALTER TABLE `product_batches` ADD COLUMN `supplier_id` int NULL AFTER `product_id`', 'SELECT 1');
                PREPARE add_batch_supplier FROM @statement; EXECUTE add_batch_supplier; DEALLOCATE PREPARE add_batch_supplier;

                SET @batch_expiry_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'product_batches' AND COLUMN_NAME = 'expiry_date');
                SET @statement = IF(@batch_expiry_exists = 0, 'ALTER TABLE `product_batches` ADD COLUMN `expiry_date` datetime NULL AFTER `selling_price`', 'SELECT 1');
                PREPARE add_batch_expiry FROM @statement; EXECUTE add_batch_expiry; DEALLOCATE PREPARE add_batch_expiry;

                SET @batch_status_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'product_batches' AND COLUMN_NAME = 'status');
                SET @statement = IF(@batch_status_exists = 0, 'ALTER TABLE `product_batches` ADD COLUMN `status` int NOT NULL DEFAULT 1 AFTER `expiry_date`', 'SELECT 1');
                PREPARE add_batch_status FROM @statement; EXECUTE add_batch_status; DEALLOCATE PREPARE add_batch_status;

                SET @capacity_liters_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tanks' AND COLUMN_NAME = 'capacity_liters');
                SET @statement = IF(@capacity_liters_exists = 0, 'ALTER TABLE `tanks` ADD COLUMN `capacity_liters` decimal(18,2) NOT NULL DEFAULT 0 AFTER `tank_no`', 'SELECT 1');
                PREPARE add_capacity_liters FROM @statement; EXECUTE add_capacity_liters; DEALLOCATE PREPARE add_capacity_liters;

                SET @current_liters_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'tanks' AND COLUMN_NAME = 'current_liters');
                SET @statement = IF(@current_liters_exists = 0, 'ALTER TABLE `tanks` ADD COLUMN `current_liters` decimal(18,2) NOT NULL DEFAULT 0 AFTER `capacity_liters`', 'SELECT 1');
                PREPARE add_current_liters FROM @statement; EXECUTE add_current_liters; DEALLOCATE PREPARE add_current_liters;

                CREATE TABLE IF NOT EXISTS `stock_receivings` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `receiving_no` varchar(100) NOT NULL,
                    `supplier_id` int NULL,
                    `received_date` datetime NOT NULL,
                    `total_amount` decimal(18,2) NOT NULL DEFAULT 0,
                    `remarks` varchar(255) NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`),
                    UNIQUE KEY `IX_stock_receivings_receiving_no` (`receiving_no`),
                    KEY `IX_stock_receivings_supplier_id` (`supplier_id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `stock_receiving_items` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `stock_receiving_id` int NOT NULL,
                    `product_id` int NOT NULL,
                    `product_batch_id` int NULL,
                    `quantity` decimal(18,2) NOT NULL,
                    `cost_price` decimal(18,2) NOT NULL,
                    `selling_price` decimal(18,2) NULL,
                    `expiry_date` datetime NULL,
                    `subtotal` decimal(18,2) NOT NULL,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`),
                    KEY `IX_stock_receiving_items_stock_receiving_id` (`stock_receiving_id`),
                    KEY `IX_stock_receiving_items_product_id` (`product_id`),
                    KEY `IX_stock_receiving_items_product_batch_id` (`product_batch_id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `stock_movements` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `product_id` int NOT NULL,
                    `product_batch_id` int NULL,
                    `source_location` varchar(50) NULL,
                    `destination_location` varchar(50) NULL,
                    `movement_type` varchar(50) NOT NULL,
                    `quantity` decimal(18,2) NOT NULL,
                    `reference_type` varchar(100) NULL,
                    `reference_id` int NULL,
                    `remarks` varchar(255) NULL,
                    `created_by` int NULL,
                    `created_at` datetime NULL,
                    PRIMARY KEY (`id`),
                    KEY `IX_stock_movements_product_id` (`product_id`),
                    KEY `IX_stock_movements_product_batch_id` (`product_batch_id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `low_stock_settings` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `product_id` int NOT NULL,
                    `product_batch_id` int NULL,
                    `location` varchar(50) NOT NULL,
                    `minimum_quantity` decimal(18,2) NOT NULL DEFAULT 0,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`),
                    KEY `IX_low_stock_settings_product_id` (`product_id`),
                    KEY `IX_low_stock_settings_product_batch_id` (`product_batch_id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `nozzles` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `pump_id` int NOT NULL,
                    `nozzle_no` varchar(100) NOT NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`),
                    KEY `IX_nozzles_pump_id` (`pump_id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `fuel_deliveries` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `delivery_no` varchar(100) NOT NULL,
                    `supplier_id` int NULL,
                    `fuel_id` int NOT NULL,
                    `tank_id` int NOT NULL,
                    `delivered_liters` decimal(18,2) NOT NULL,
                    `cost_per_liter` decimal(18,2) NULL,
                    `total_cost` decimal(18,2) NULL,
                    `delivery_date` datetime NOT NULL,
                    `remarks` varchar(255) NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`),
                    UNIQUE KEY `IX_fuel_deliveries_delivery_no` (`delivery_no`),
                    KEY `IX_fuel_deliveries_supplier_id` (`supplier_id`),
                    KEY `IX_fuel_deliveries_fuel_id` (`fuel_id`),
                    KEY `IX_fuel_deliveries_tank_id` (`tank_id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `fuel_price_history` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `fuel_id` int NOT NULL,
                    `old_price` decimal(18,2) NULL,
                    `new_price` decimal(18,2) NOT NULL,
                    `effective_at` datetime NOT NULL,
                    `remarks` varchar(255) NULL,
                    `created_by` int NULL,
                    `created_at` datetime NULL,
                    PRIMARY KEY (`id`),
                    KEY `IX_fuel_price_history_fuel_id` (`fuel_id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `pump_meter_readings` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `pump_id` int NOT NULL,
                    `nozzle_id` int NULL,
                    `shift_id` int NULL,
                    `opening_meter` decimal(18,2) NOT NULL,
                    `closing_meter` decimal(18,2) NULL,
                    `liters_sold` decimal(18,2) NULL,
                    `reading_date` datetime NOT NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`),
                    KEY `IX_pump_meter_readings_pump_id` (`pump_id`),
                    KEY `IX_pump_meter_readings_nozzle_id` (`nozzle_id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `rebate_rules` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `name` varchar(100) NOT NULL,
                    `points_required` decimal(18,2) NOT NULL,
                    `rebate_value` decimal(18,2) NOT NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `points_ledger` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `member_id` int NOT NULL,
                    `transaction_type` varchar(50) NOT NULL,
                    `points` decimal(18,2) NOT NULL,
                    `reference_type` varchar(100) NULL,
                    `reference_id` int NULL,
                    `remarks` varchar(255) NULL,
                    `created_at` datetime NULL,
                    PRIMARY KEY (`id`),
                    KEY `IX_points_ledger_member_id` (`member_id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `discount_rules` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `name` varchar(100) NOT NULL,
                    `discount_type` varchar(50) NOT NULL,
                    `discount_value` decimal(18,2) NOT NULL,
                    `applies_to` varchar(50) NOT NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `station_settings` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `station_name` varchar(150) NOT NULL,
                    `business_name` varchar(150) NULL,
                    `address` varchar(255) NULL,
                    `tin` varchar(100) NULL,
                    `receipt_header` varchar(255) NULL,
                    `receipt_footer` varchar(255) NULL,
                    `default_branch_id` int NULL,
                    `currency` varchar(20) NOT NULL DEFAULT 'PHP',
                    `tax_enabled` int NOT NULL DEFAULT 0,
                    `tax_rate` decimal(5,2) NOT NULL DEFAULT 0,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`),
                    KEY `IX_station_settings_default_branch_id` (`default_branch_id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `payment_methods` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `name` varchar(100) NOT NULL,
                    `code` varchar(50) NOT NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`),
                    UNIQUE KEY `IX_payment_methods_code` (`code`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `shift_settings` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `name` varchar(100) NOT NULL,
                    `start_time` time NULL,
                    `end_time` time NULL,
                    `require_opening_cash` int NOT NULL DEFAULT 1,
                    `allow_cash_in` int NOT NULL DEFAULT 1,
                    `allow_cash_out` int NOT NULL DEFAULT 1,
                    `require_closing_approval` int NOT NULL DEFAULT 0,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `activity_logs` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `user_id` int NULL,
                    `username` varchar(100) NULL,
                    `action` varchar(100) NOT NULL,
                    `module` varchar(100) NULL,
                    `description` varchar(255) NULL,
                    `ip_address` varchar(100) NULL,
                    `created_at` datetime NULL,
                    PRIMARY KEY (`id`),
                    KEY `IX_activity_logs_user_id` (`user_id`)
                ) CHARACTER SET=utf8mb4;
                """);

            AddForeignKeyIfMissing(migrationBuilder, "FK_products_product_units_product_unit_id", "products", "product_unit_id", "product_units", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_product_batches_suppliers_supplier_id", "product_batches", "supplier_id", "suppliers", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_stock_receivings_suppliers_supplier_id", "stock_receivings", "supplier_id", "suppliers", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_stock_receiving_items_stock_receivings_stock_receiving_id", "stock_receiving_items", "stock_receiving_id", "stock_receivings", "id", "CASCADE");
            AddForeignKeyIfMissing(migrationBuilder, "FK_stock_receiving_items_products_product_id", "stock_receiving_items", "product_id", "products", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_stock_receiving_items_product_batches_product_batch_id", "stock_receiving_items", "product_batch_id", "product_batches", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_stock_movements_products_product_id", "stock_movements", "product_id", "products", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_stock_movements_product_batches_product_batch_id", "stock_movements", "product_batch_id", "product_batches", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_low_stock_settings_products_product_id", "low_stock_settings", "product_id", "products", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_low_stock_settings_product_batches_product_batch_id", "low_stock_settings", "product_batch_id", "product_batches", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_nozzles_pumps_pump_id", "nozzles", "pump_id", "pumps", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_fuel_deliveries_suppliers_supplier_id", "fuel_deliveries", "supplier_id", "suppliers", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_fuel_deliveries_fuels_fuel_id", "fuel_deliveries", "fuel_id", "fuels", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_fuel_deliveries_tanks_tank_id", "fuel_deliveries", "tank_id", "tanks", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_fuel_price_history_fuels_fuel_id", "fuel_price_history", "fuel_id", "fuels", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_pump_meter_readings_pumps_pump_id", "pump_meter_readings", "pump_id", "pumps", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_pump_meter_readings_nozzles_nozzle_id", "pump_meter_readings", "nozzle_id", "nozzles", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_points_ledger_members_member_id", "points_ledger", "member_id", "members", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_station_settings_branches_default_branch_id", "station_settings", "default_branch_id", "branches", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_activity_logs_users_user_id", "activity_logs", "user_id", "users", "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }

        private static void AddForeignKeyIfMissing(MigrationBuilder migrationBuilder, string name, string table, string column, string principalTable, string principalColumn, string onDelete = "RESTRICT")
        {
            migrationBuilder.Sql($"""
                SET @fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND CONSTRAINT_NAME = '{name}'
                );
                SET @statement = IF(
                    @fk_exists = 0,
                    'ALTER TABLE `{table}` ADD CONSTRAINT `{name}` FOREIGN KEY (`{column}`) REFERENCES `{principalTable}` (`{principalColumn}`) ON DELETE {onDelete}',
                    'SELECT 1'
                );
                PREPARE add_fk FROM @statement;
                EXECUTE add_fk;
                DEALLOCATE PREPARE add_fk;
                """);
        }
    }
}

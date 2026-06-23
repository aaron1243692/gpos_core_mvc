/*
 Navicat Premium Dump SQL

 Source Server         : newlocal
 Source Server Type    : MySQL
 Source Server Version : 90700 (9.7.0)
 Source Host           : localhost:3306
 Source Schema         : gpos

 Target Server Type    : MySQL
 Target Server Version : 90700 (9.7.0)
 File Encoding         : 65001

 Date: 23/06/2026 15:11:20
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for __efmigrationshistory
-- ----------------------------
DROP TABLE IF EXISTS `__efmigrationshistory`;
CREATE TABLE `__efmigrationshistory`  (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of __efmigrationshistory
-- ----------------------------
INSERT INTO `__efmigrationshistory` VALUES ('20260615032925_InitialCreate', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260617100048_FuelSetupRelationships', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260617100643_FuelTankUnitPumpHierarchy', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260617102622_AddProductBatchAndWarehouseStockTables', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260617103316_AddProductCategoriesAndDisplayStocks', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260617151518_RemoveCapacityAndCurrentLitersFromTanks', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260617152124_RemoveFuelActiveAndPumpStatusFields', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260617153053_RemoveFuelUnits', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260617154146_RemovePumpStatusAndActiveFields', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260617160103_AddSuppliersAndFuelSupplier', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260617232000_AddEmailToSuppliers', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260617233000_SetupDiscountsAndMembers', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260618064841_SetupConfigTables', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260618070307_RemoveEmployeeNoFromEmployees', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260618071320_TieEmployeeToEmployeeAccount', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260618075539_UseEmployeeAccountOnly', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260619000000_AddTankCapacityAndCurrentLiters', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260619001000_AddMissingSetupModules', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260619002000_MakeEmployeeAccountRoleNullable', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260619003000_AddSoftDeleteStatusColumns', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260619004000_UseNozzleForPumpMeterReadings', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260619005000_AddEmployeeShiftSchedules', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260619006000_AddSchedulesAndEmployeeScheduleAssignment', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260622090000_ImplementUsersRolesAccess', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260622100000_AddBranchDepartmentToUsers', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260623000000_RemoveEmployeeScheduleUsageUseUsersBranchDepartment', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260623001000_AddOldNewPointsToPointsLedger', '9.0.0');

-- ----------------------------
-- Table structure for activity_logs
-- ----------------------------
DROP TABLE IF EXISTS `activity_logs`;
CREATE TABLE `activity_logs`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NULL DEFAULT NULL,
  `username` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `action` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `module` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `ip_address` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `created_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_activity_logs_user_id`(`user_id` ASC) USING BTREE,
  CONSTRAINT `FK_activity_logs_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of activity_logs
-- ----------------------------

-- ----------------------------
-- Table structure for branches
-- ----------------------------
DROP TABLE IF EXISTS `branches`;
CREATE TABLE `branches`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of branches
-- ----------------------------
INSERT INTO `branches` VALUES (1, 'b1', NULL, 1, '2026-06-18 06:58:23.109938', '2026-06-18 06:58:23.109938');
INSERT INTO `branches` VALUES (2, 'b2', NULL, 1, '2026-06-18 06:58:29.289810', '2026-06-18 06:58:29.289810');

-- ----------------------------
-- Table structure for config
-- ----------------------------
DROP TABLE IF EXISTS `config`;
CREATE TABLE `config`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `status` int NULL DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of config
-- ----------------------------

-- ----------------------------
-- Table structure for departments
-- ----------------------------
DROP TABLE IF EXISTS `departments`;
CREATE TABLE `departments`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `branch_id` int NOT NULL,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_departments_branch_id`(`branch_id` ASC) USING BTREE,
  CONSTRAINT `FK_departments_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of departments
-- ----------------------------
INSERT INTO `departments` VALUES (1, 1, 'dept1', NULL, 1, '2026-06-18 06:58:48.429157', '2026-06-19 02:55:12.858755');
INSERT INTO `departments` VALUES (2, 2, 'dept', NULL, 1, '2026-06-18 06:58:56.467334', '2026-06-19 02:55:21.088099');

-- ----------------------------
-- Table structure for discount_rules
-- ----------------------------
DROP TABLE IF EXISTS `discount_rules`;
CREATE TABLE `discount_rules`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `discount_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `discount_value` decimal(18, 2) NOT NULL,
  `applies_to` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of discount_rules
-- ----------------------------

-- ----------------------------
-- Table structure for discounts
-- ----------------------------
DROP TABLE IF EXISTS `discounts`;
CREATE TABLE `discounts`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `earn_rate` decimal(5, 2) NOT NULL DEFAULT 0.00,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of discounts
-- ----------------------------
INSERT INTO `discounts` VALUES (1, 'Member', 5.00, 1, '2026-06-18 05:10:47', '2026-06-18 05:10:47');
INSERT INTO `discounts` VALUES (2, 'VIP', 10.00, 1, '2026-06-18 05:11:03', '2026-06-18 05:11:03');
INSERT INTO `discounts` VALUES (3, 'VIP+', 15.00, 1, '2026-06-18 05:11:13', '2026-06-18 05:11:13');
INSERT INTO `discounts` VALUES (4, 'Elite', 20.00, 1, '2026-06-18 05:11:36', '2026-06-18 05:31:18');

-- ----------------------------
-- Table structure for display_stocks
-- ----------------------------
DROP TABLE IF EXISTS `display_stocks`;
CREATE TABLE `display_stocks`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NOT NULL,
  `batch_id` int NOT NULL,
  `quantity` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_display_stocks_batch_id`(`batch_id` ASC) USING BTREE,
  INDEX `IX_display_stocks_product_id`(`product_id` ASC) USING BTREE,
  CONSTRAINT `FK_display_stocks_product_batches_batch_id` FOREIGN KEY (`batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_display_stocks_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `CK_display_stocks_quantity_non_negative` CHECK (`quantity` >= 0)
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of display_stocks
-- ----------------------------
INSERT INTO `display_stocks` VALUES (1, 2, 2, 12.00, '2026-06-18 06:37:43.782840', '2026-06-18 06:37:43.782840');

-- ----------------------------
-- Table structure for employee_account
-- ----------------------------
DROP TABLE IF EXISTS `employee_account`;
CREATE TABLE `employee_account`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `password_hash` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `full_name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `email` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `contact_number` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `department_id` int NOT NULL,
  `position_id` int NULL DEFAULT NULL,
  `role` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  `schedule_id` int NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `username`(`username` ASC) USING BTREE,
  UNIQUE INDEX `IX_employee_accounts_username`(`username` ASC) USING BTREE,
  INDEX `IX_employee_account_department_id`(`department_id` ASC) USING BTREE,
  INDEX `IX_employee_account_position_id`(`position_id` ASC) USING BTREE,
  INDEX `IX_employee_account_schedule_id`(`schedule_id` ASC) USING BTREE,
  CONSTRAINT `FK_employee_account_departments_department_id` FOREIGN KEY (`department_id`) REFERENCES `departments` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_employee_account_positions_position_id` FOREIGN KEY (`position_id`) REFERENCES `positions` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_employee_account_schedules_schedule_id` FOREIGN KEY (`schedule_id`) REFERENCES `schedules` (`id`) ON DELETE SET NULL ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of employee_account
-- ----------------------------
INSERT INTO `employee_account` VALUES (1, 'salesman', '$2a$11$dNrSyYn6S/McWP6GbLL3LuAEmvKjbi/rBFq17OSrLdbH19X.rNK6u', 'Salesman', NULL, NULL, NULL, 1, 2, 'salesman', 1, '2026-06-15 15:37:47', '2026-06-19 14:48:36', 1);

-- ----------------------------
-- Table structure for employee_shift_schedules
-- ----------------------------
DROP TABLE IF EXISTS `employee_shift_schedules`;
CREATE TABLE `employee_shift_schedules`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `employee_account_id` int NOT NULL,
  `shift_setting_id` int NULL DEFAULT NULL,
  `day_of_week` int NOT NULL,
  `start_time` time NOT NULL,
  `end_time` time NOT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_employee_shift_schedules_employee_account_id`(`employee_account_id` ASC) USING BTREE,
  INDEX `IX_employee_shift_schedules_shift_setting_id`(`shift_setting_id` ASC) USING BTREE,
  CONSTRAINT `FK_employee_shift_schedules_employee_account_employee_account_id` FOREIGN KEY (`employee_account_id`) REFERENCES `employee_account` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_employee_shift_schedules_shift_settings_shift_setting_id` FOREIGN KEY (`shift_setting_id`) REFERENCES `shift_settings` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of employee_shift_schedules
-- ----------------------------

-- ----------------------------
-- Table structure for employees
-- ----------------------------
DROP TABLE IF EXISTS `employees`;
CREATE TABLE `employees`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `full_name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `email` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `contact_number` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `department_id` int NOT NULL,
  `position_id` int NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_employees_department_id`(`department_id` ASC) USING BTREE,
  INDEX `IX_employees_position_id`(`position_id` ASC) USING BTREE,
  CONSTRAINT `FK_employees_departments_department_id` FOREIGN KEY (`department_id`) REFERENCES `departments` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_employees_positions_position_id` FOREIGN KEY (`position_id`) REFERENCES `positions` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of employees
-- ----------------------------
INSERT INTO `employees` VALUES (1, 'ew', NULL, NULL, NULL, 2, 1, 1, '2026-06-18 07:00:26', '2026-06-18 07:00:26');

-- ----------------------------
-- Table structure for fuel_deliveries
-- ----------------------------
DROP TABLE IF EXISTS `fuel_deliveries`;
CREATE TABLE `fuel_deliveries`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `delivery_no` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `supplier_id` int NULL DEFAULT NULL,
  `fuel_id` int NOT NULL,
  `tank_id` int NOT NULL,
  `delivered_liters` decimal(18, 2) NOT NULL,
  `cost_per_liter` decimal(18, 2) NULL DEFAULT NULL,
  `total_cost` decimal(18, 2) NULL DEFAULT NULL,
  `delivery_date` datetime NOT NULL,
  `remarks` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_fuel_deliveries_delivery_no`(`delivery_no` ASC) USING BTREE,
  INDEX `IX_fuel_deliveries_supplier_id`(`supplier_id` ASC) USING BTREE,
  INDEX `IX_fuel_deliveries_fuel_id`(`fuel_id` ASC) USING BTREE,
  INDEX `IX_fuel_deliveries_tank_id`(`tank_id` ASC) USING BTREE,
  CONSTRAINT `FK_fuel_deliveries_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_deliveries_suppliers_supplier_id` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_deliveries_tanks_tank_id` FOREIGN KEY (`tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of fuel_deliveries
-- ----------------------------

-- ----------------------------
-- Table structure for fuel_price_history
-- ----------------------------
DROP TABLE IF EXISTS `fuel_price_history`;
CREATE TABLE `fuel_price_history`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `fuel_id` int NOT NULL,
  `old_price` decimal(18, 2) NULL DEFAULT NULL,
  `new_price` decimal(18, 2) NOT NULL,
  `effective_at` datetime NOT NULL,
  `remarks` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `created_by` int NULL DEFAULT NULL,
  `created_at` datetime NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_fuel_price_history_fuel_id`(`fuel_id` ASC) USING BTREE,
  CONSTRAINT `FK_fuel_price_history_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of fuel_price_history
-- ----------------------------
INSERT INTO `fuel_price_history` VALUES (1, 1, 60.00, 75.00, '2026-06-19 00:00:00', NULL, NULL, '2026-06-19 05:09:24', 1, NULL);
INSERT INTO `fuel_price_history` VALUES (2, 2, 55.00, 75.00, '2026-06-19 00:00:00', NULL, NULL, '2026-06-19 05:09:34', 1, NULL);
INSERT INTO `fuel_price_history` VALUES (3, 3, 65.00, 75.00, '2026-06-19 00:00:00', NULL, NULL, '2026-06-19 05:09:44', 1, NULL);

-- ----------------------------
-- Table structure for fuels
-- ----------------------------
DROP TABLE IF EXISTS `fuels`;
CREATE TABLE `fuels`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `code` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `current_price_per_liter` decimal(18, 2) NOT NULL,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `supplier_id` int NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_fuels_supplier_id`(`supplier_id` ASC) USING BTREE,
  CONSTRAINT `FK_fuels_suppliers_supplier_id` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of fuels
-- ----------------------------
INSERT INTO `fuels` VALUES (1, 'gas1', 'g1', 75.00, '2026-06-17 15:00:21.609264', '2026-06-19 05:13:10.713576', 1, 1, 1);
INSERT INTO `fuels` VALUES (2, 'gas2', 'g2', 75.00, '2026-06-17 15:00:36.409137', '2026-06-19 05:13:15.816448', 1, 1, 1);
INSERT INTO `fuels` VALUES (3, 'gas3', 'g3', 75.00, '2026-06-17 15:00:47.641750', '2026-06-19 05:13:19.946547', 1, 2, 1);

-- ----------------------------
-- Table structure for low_stock_settings
-- ----------------------------
DROP TABLE IF EXISTS `low_stock_settings`;
CREATE TABLE `low_stock_settings`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NOT NULL,
  `product_batch_id` int NULL DEFAULT NULL,
  `location` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `minimum_quantity` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_low_stock_settings_product_id`(`product_id` ASC) USING BTREE,
  INDEX `IX_low_stock_settings_product_batch_id`(`product_batch_id` ASC) USING BTREE,
  CONSTRAINT `FK_low_stock_settings_product_batches_product_batch_id` FOREIGN KEY (`product_batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_low_stock_settings_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of low_stock_settings
-- ----------------------------
INSERT INTO `low_stock_settings` VALUES (1, 2, NULL, 'roever', 100.00, 0, '2026-06-19 02:42:30', '2026-06-23 06:56:44');

-- ----------------------------
-- Table structure for members
-- ----------------------------
DROP TABLE IF EXISTS `members`;
CREATE TABLE `members`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `member_no` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `card_no` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `full_name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `contact_number` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `email` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `discount_id` int NULL DEFAULT NULL,
  `points` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_members_member_no`(`member_no` ASC) USING BTREE,
  UNIQUE INDEX `IX_members_card_no`(`card_no` ASC) USING BTREE,
  INDEX `IX_members_discount_id`(`discount_id` ASC) USING BTREE,
  CONSTRAINT `FK_members_discounts_discount_id` FOREIGN KEY (`discount_id`) REFERENCES `discounts` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of members
-- ----------------------------
INSERT INTO `members` VALUES (1, '121212', NULL, 'hello', NULL, NULL, NULL, NULL, 1000.00, 1, '2026-06-17 17:01:12', '2026-06-23 07:41:01');

-- ----------------------------
-- Table structure for nozzles
-- ----------------------------
DROP TABLE IF EXISTS `nozzles`;
CREATE TABLE `nozzles`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `pump_id` int NOT NULL,
  `nozzle_no` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_nozzles_pump_id`(`pump_id` ASC) USING BTREE,
  CONSTRAINT `FK_nozzles_pumps_pump_id` FOREIGN KEY (`pump_id`) REFERENCES `pumps` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of nozzles
-- ----------------------------
INSERT INTO `nozzles` VALUES (1, 1, 'saasd', 1, '2026-06-19 02:37:33', '2026-06-19 02:37:33');

-- ----------------------------
-- Table structure for payment_methods
-- ----------------------------
DROP TABLE IF EXISTS `payment_methods`;
CREATE TABLE `payment_methods`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_payment_methods_code`(`code` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of payment_methods
-- ----------------------------
INSERT INTO `payment_methods` VALUES (1, 'Cash', 'cash', 1, '2026-06-19 02:30:03', '2026-06-19 02:30:03');

-- ----------------------------
-- Table structure for permissions
-- ----------------------------
DROP TABLE IF EXISTS `permissions`;
CREATE TABLE `permissions`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `code` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `parent_id` int NULL DEFAULT NULL,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_permissions_code`(`code` ASC) USING BTREE,
  INDEX `IX_permissions_parent_id`(`parent_id` ASC) USING BTREE,
  CONSTRAINT `FK_permissions_permissions_parent_id` FOREIGN KEY (`parent_id`) REFERENCES `permissions` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 8 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of permissions
-- ----------------------------
INSERT INTO `permissions` VALUES (1, 'users', 'users', NULL, NULL, NULL, 1);
INSERT INTO `permissions` VALUES (2, 'View', 'view.view', 1, NULL, NULL, 1);
INSERT INTO `permissions` VALUES (3, 'roles', 'roles', NULL, NULL, NULL, 1);
INSERT INTO `permissions` VALUES (4, 'view', 'roles.view', 3, NULL, NULL, 1);
INSERT INTO `permissions` VALUES (5, 'add', 'roles.add', 3, NULL, NULL, 1);
INSERT INTO `permissions` VALUES (6, 'update', 'roles.update', 3, NULL, NULL, 1);
INSERT INTO `permissions` VALUES (7, 'delete ', 'roles.delete', 3, NULL, NULL, 1);

-- ----------------------------
-- Table structure for points_ledger
-- ----------------------------
DROP TABLE IF EXISTS `points_ledger`;
CREATE TABLE `points_ledger`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `member_id` int NOT NULL,
  `transaction_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `points` decimal(18, 2) NOT NULL,
  `reference_type` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `reference_id` int NULL DEFAULT NULL,
  `remarks` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `created_at` datetime NULL DEFAULT NULL,
  `old_points` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `new_points` decimal(18, 2) NOT NULL DEFAULT 0.00,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_points_ledger_member_id`(`member_id` ASC) USING BTREE,
  CONSTRAINT `FK_points_ledger_members_member_id` FOREIGN KEY (`member_id`) REFERENCES `members` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of points_ledger
-- ----------------------------
INSERT INTO `points_ledger` VALUES (1, 1, 'set', 1000.00, NULL, NULL, NULL, '2026-06-23 07:41:01', 0.00, 1000.00);

-- ----------------------------
-- Table structure for positions
-- ----------------------------
DROP TABLE IF EXISTS `positions`;
CREATE TABLE `positions`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of positions
-- ----------------------------
INSERT INTO `positions` VALUES (1, 'Gas man', NULL, 1, '2026-06-18 06:59:48.211853', '2026-06-18 06:59:48.211853');
INSERT INTO `positions` VALUES (2, 'cashier', NULL, 1, '2026-06-18 06:59:56.113053', '2026-06-18 06:59:56.113053');

-- ----------------------------
-- Table structure for product_batches
-- ----------------------------
DROP TABLE IF EXISTS `product_batches`;
CREATE TABLE `product_batches`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NOT NULL,
  `supplier_id` int NULL DEFAULT NULL,
  `batch_no` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `cost_price` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `selling_price` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `expiry_date` datetime NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_product_batches_batch_no`(`batch_no` ASC) USING BTREE,
  INDEX `IX_product_batches_product_id`(`product_id` ASC) USING BTREE,
  INDEX `FK_product_batches_suppliers_supplier_id`(`supplier_id` ASC) USING BTREE,
  CONSTRAINT `FK_product_batches_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_product_batches_suppliers_supplier_id` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `CK_product_batches_cost_price_non_negative` CHECK (`cost_price` >= 0),
  CONSTRAINT `CK_product_batches_selling_price_non_negative` CHECK (`selling_price` >= 0)
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of product_batches
-- ----------------------------
INSERT INTO `product_batches` VALUES (1, 1, NULL, 'BATCH-000001', 7665.00, 896.00, NULL, 1, 1, '2026-06-18 04:02:06.529349', '2026-06-18 04:02:06.529349');
INSERT INTO `product_batches` VALUES (2, 2, NULL, 'BATCH-000002', 34.00, 0.00, NULL, 1, 1, '2026-06-18 06:37:43.782840', '2026-06-18 06:37:43.782840');

-- ----------------------------
-- Table structure for product_categories
-- ----------------------------
DROP TABLE IF EXISTS `product_categories`;
CREATE TABLE `product_categories`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of product_categories
-- ----------------------------
INSERT INTO `product_categories` VALUES (1, 'c1', 'g', 1, '2026-06-17 16:40:13.199221', '2026-06-17 16:40:13.199221', 1);

-- ----------------------------
-- Table structure for product_units
-- ----------------------------
DROP TABLE IF EXISTS `product_units`;
CREATE TABLE `product_units`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `abbreviation` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of product_units
-- ----------------------------
INSERT INTO `product_units` VALUES (1, 'kape', 'pack', 1, '2026-06-19 02:43:36', '2026-06-19 02:43:36');
INSERT INTO `product_units` VALUES (2, 'bravo', 'pack', 1, '2026-06-19 02:43:46', '2026-06-19 02:43:46');

-- ----------------------------
-- Table structure for products
-- ----------------------------
DROP TABLE IF EXISTS `products`;
CREATE TABLE `products`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `is_active` tinyint(1) NOT NULL,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `category_id` int NULL DEFAULT NULL,
  `product_unit_id` int NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_products_category_id`(`category_id` ASC) USING BTREE,
  INDEX `FK_products_product_units_product_unit_id`(`product_unit_id` ASC) USING BTREE,
  CONSTRAINT `FK_products_product_categories_category_id` FOREIGN KEY (`category_id`) REFERENCES `product_categories` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_products_product_units_product_unit_id` FOREIGN KEY (`product_unit_id`) REFERENCES `product_units` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of products
-- ----------------------------
INSERT INTO `products` VALUES (1, 'kjh', 1, '2026-06-18 04:02:06.529349', '2026-06-18 04:02:06.529349', 1, NULL, 1);
INSERT INTO `products` VALUES (2, 'asd', 1, '2026-06-18 06:37:43.782840', '2026-06-18 06:37:43.782840', 1, NULL, 1);

-- ----------------------------
-- Table structure for pump_meter_readings
-- ----------------------------
DROP TABLE IF EXISTS `pump_meter_readings`;
CREATE TABLE `pump_meter_readings`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `pump_id` int NULL DEFAULT NULL,
  `nozzle_id` int NULL DEFAULT NULL,
  `shift_id` int NULL DEFAULT NULL,
  `opening_meter` decimal(18, 2) NOT NULL,
  `closing_meter` decimal(18, 2) NULL DEFAULT NULL,
  `liters_sold` decimal(18, 2) NULL DEFAULT NULL,
  `reading_date` datetime NOT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_pump_meter_readings_pump_id`(`pump_id` ASC) USING BTREE,
  INDEX `IX_pump_meter_readings_nozzle_id`(`nozzle_id` ASC) USING BTREE,
  CONSTRAINT `FK_pump_meter_readings_nozzles_nozzle_id` FOREIGN KEY (`nozzle_id`) REFERENCES `nozzles` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_pump_meter_readings_pumps_pump_id` FOREIGN KEY (`pump_id`) REFERENCES `pumps` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of pump_meter_readings
-- ----------------------------
INSERT INTO `pump_meter_readings` VALUES (1, 1, 1, NULL, 100.00, 100.00, 0.00, '2026-06-19 00:00:00', 0, '2026-06-19 02:41:34', '2026-06-19 05:09:08');

-- ----------------------------
-- Table structure for pumps
-- ----------------------------
DROP TABLE IF EXISTS `pumps`;
CREATE TABLE `pumps`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `pump_no` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `tank_id` int NOT NULL,
  `status` int NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_pumps_tank_id`(`tank_id` ASC) USING BTREE,
  CONSTRAINT `FK_pumps_tanks_tank_id` FOREIGN KEY (`tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of pumps
-- ----------------------------
INSERT INTO `pumps` VALUES (1, 'pump1', 'pump1', '2026-06-19 02:37:11.955469', '2026-06-19 02:37:11.955469', 1, 1);

-- ----------------------------
-- Table structure for rebate_rules
-- ----------------------------
DROP TABLE IF EXISTS `rebate_rules`;
CREATE TABLE `rebate_rules`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `points_required` decimal(18, 2) NOT NULL,
  `rebate_value` decimal(18, 2) NOT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of rebate_rules
-- ----------------------------
INSERT INTO `rebate_rules` VALUES (1, '1k to 1h', 1000.00, 100.00, 1, '2026-06-19 02:39:06', '2026-06-19 02:39:06');

-- ----------------------------
-- Table structure for role_permissions
-- ----------------------------
DROP TABLE IF EXISTS `role_permissions`;
CREATE TABLE `role_permissions`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `role_id` int NOT NULL,
  `permission_id` int NOT NULL,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_role_permissions_role_id_permission_id`(`role_id` ASC, `permission_id` ASC) USING BTREE,
  INDEX `IX_role_permissions_permission_id`(`permission_id` ASC) USING BTREE,
  CONSTRAINT `FK_role_permissions_permissions_permission_id` FOREIGN KEY (`permission_id`) REFERENCES `permissions` (`id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `FK_role_permissions_roles_role_id` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of role_permissions
-- ----------------------------

-- ----------------------------
-- Table structure for roles
-- ----------------------------
DROP TABLE IF EXISTS `roles`;
CREATE TABLE `roles`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `code` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_roles_code`(`code` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of roles
-- ----------------------------
INSERT INTO `roles` VALUES (1, 'Admin', 'admin', '2026-06-15 03:30:45', '2026-06-15 03:30:45', 1);
INSERT INTO `roles` VALUES (2, 'Cashier', 'cashier', '2026-06-23 06:49:35', '2026-06-23 06:49:35', 1);

-- ----------------------------
-- Table structure for schedule_details
-- ----------------------------
DROP TABLE IF EXISTS `schedule_details`;
CREATE TABLE `schedule_details`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `schedule_id` int NOT NULL,
  `day_of_week` int NOT NULL,
  `am_in` time NULL DEFAULT NULL,
  `am_out` time NULL DEFAULT NULL,
  `pm_in` time NULL DEFAULT NULL,
  `pm_out` time NULL DEFAULT NULL,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_schedule_details_schedule_id_day_of_week`(`schedule_id` ASC, `day_of_week` ASC) USING BTREE,
  INDEX `IX_schedule_details_schedule_id`(`schedule_id` ASC) USING BTREE,
  CONSTRAINT `FK_schedule_details_schedules_schedule_id` FOREIGN KEY (`schedule_id`) REFERENCES `schedules` (`id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of schedule_details
-- ----------------------------

-- ----------------------------
-- Table structure for schedules
-- ----------------------------
DROP TABLE IF EXISTS `schedules`;
CREATE TABLE `schedules`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_schedules_name`(`name` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of schedules
-- ----------------------------
INSERT INTO `schedules` VALUES (1, 'hello', 1, '2026-06-19 09:00:50.068552', '2026-06-19 09:00:50.068552');
INSERT INTO `schedules` VALUES (2, 'Full Time', 1, '2026-06-19 14:49:31.568135', '2026-06-19 14:49:31.568135');

-- ----------------------------
-- Table structure for shift_settings
-- ----------------------------
DROP TABLE IF EXISTS `shift_settings`;
CREATE TABLE `shift_settings`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `start_time` time NULL DEFAULT NULL,
  `end_time` time NULL DEFAULT NULL,
  `require_opening_cash` int NOT NULL DEFAULT 1,
  `allow_cash_in` int NOT NULL DEFAULT 1,
  `allow_cash_out` int NOT NULL DEFAULT 1,
  `require_closing_approval` int NOT NULL DEFAULT 0,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of shift_settings
-- ----------------------------

-- ----------------------------
-- Table structure for station_settings
-- ----------------------------
DROP TABLE IF EXISTS `station_settings`;
CREATE TABLE `station_settings`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `station_name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `business_name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `tin` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `receipt_header` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `receipt_footer` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `default_branch_id` int NULL DEFAULT NULL,
  `currency` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'PHP',
  `tax_enabled` int NOT NULL DEFAULT 0,
  `tax_rate` decimal(5, 2) NOT NULL DEFAULT 0.00,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_station_settings_default_branch_id`(`default_branch_id` ASC) USING BTREE,
  CONSTRAINT `FK_station_settings_branches_default_branch_id` FOREIGN KEY (`default_branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of station_settings
-- ----------------------------

-- ----------------------------
-- Table structure for stock_movements
-- ----------------------------
DROP TABLE IF EXISTS `stock_movements`;
CREATE TABLE `stock_movements`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NOT NULL,
  `product_batch_id` int NULL DEFAULT NULL,
  `source_location` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `destination_location` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `movement_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `quantity` decimal(18, 2) NOT NULL,
  `reference_type` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `reference_id` int NULL DEFAULT NULL,
  `remarks` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `created_by` int NULL DEFAULT NULL,
  `created_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_stock_movements_product_id`(`product_id` ASC) USING BTREE,
  INDEX `IX_stock_movements_product_batch_id`(`product_batch_id` ASC) USING BTREE,
  CONSTRAINT `FK_stock_movements_product_batches_product_batch_id` FOREIGN KEY (`product_batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_movements_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock_movements
-- ----------------------------

-- ----------------------------
-- Table structure for stock_receiving_items
-- ----------------------------
DROP TABLE IF EXISTS `stock_receiving_items`;
CREATE TABLE `stock_receiving_items`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `stock_receiving_id` int NOT NULL,
  `product_id` int NOT NULL,
  `product_batch_id` int NULL DEFAULT NULL,
  `quantity` decimal(18, 2) NOT NULL,
  `cost_price` decimal(18, 2) NOT NULL,
  `selling_price` decimal(18, 2) NULL DEFAULT NULL,
  `expiry_date` datetime NULL DEFAULT NULL,
  `subtotal` decimal(18, 2) NOT NULL,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_stock_receiving_items_stock_receiving_id`(`stock_receiving_id` ASC) USING BTREE,
  INDEX `IX_stock_receiving_items_product_id`(`product_id` ASC) USING BTREE,
  INDEX `IX_stock_receiving_items_product_batch_id`(`product_batch_id` ASC) USING BTREE,
  CONSTRAINT `FK_stock_receiving_items_product_batches_product_batch_id` FOREIGN KEY (`product_batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_receiving_items_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_receiving_items_stock_receivings_stock_receiving_id` FOREIGN KEY (`stock_receiving_id`) REFERENCES `stock_receivings` (`id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock_receiving_items
-- ----------------------------

-- ----------------------------
-- Table structure for stock_receivings
-- ----------------------------
DROP TABLE IF EXISTS `stock_receivings`;
CREATE TABLE `stock_receivings`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `receiving_no` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `supplier_id` int NULL DEFAULT NULL,
  `received_date` datetime NOT NULL,
  `total_amount` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `remarks` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_stock_receivings_receiving_no`(`receiving_no` ASC) USING BTREE,
  INDEX `IX_stock_receivings_supplier_id`(`supplier_id` ASC) USING BTREE,
  CONSTRAINT `FK_stock_receivings_suppliers_supplier_id` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock_receivings
-- ----------------------------

-- ----------------------------
-- Table structure for suppliers
-- ----------------------------
DROP TABLE IF EXISTS `suppliers`;
CREATE TABLE `suppliers`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `contact_person` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `contact_number` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `email` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of suppliers
-- ----------------------------
INSERT INTO `suppliers` VALUES (1, 'sup1', 'sup1', '123', 'adasd', 1, '2026-06-17 16:28:34.393344', '2026-06-17 16:28:34.393344', NULL, 1);
INSERT INTO `suppliers` VALUES (2, 'sup2', NULL, NULL, NULL, 1, '2026-06-17 16:28:41.415871', '2026-06-17 16:28:41.415871', NULL, 1);

-- ----------------------------
-- Table structure for tanks
-- ----------------------------
DROP TABLE IF EXISTS `tanks`;
CREATE TABLE `tanks`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `fuel_id` int NOT NULL,
  `tank_no` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `capacity_liters` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `current_liters` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `status` int NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_tanks_fuel_id`(`fuel_id` ASC) USING BTREE,
  CONSTRAINT `FK_tanks_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of tanks
-- ----------------------------
INSERT INTO `tanks` VALUES (1, 2, 'tank1', 0.00, 0.00, '2026-06-17 15:45:14.746149', '2026-06-17 15:45:14.746149', 1, 1);
INSERT INTO `tanks` VALUES (2, 3, 'tank3', 0.00, 0.00, '2026-06-17 15:45:27.903450', '2026-06-17 15:45:27.903450', 1, 1);
INSERT INTO `tanks` VALUES (3, 1, 'tank1', 1000.00, 0.00, '2026-06-17 15:45:45.855127', '2026-06-23 07:00:02.801294', 1, 1);

-- ----------------------------
-- Table structure for user_roles
-- ----------------------------
DROP TABLE IF EXISTS `user_roles`;
CREATE TABLE `user_roles`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `role_id` int NOT NULL,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_user_roles_user_id_role_id`(`user_id` ASC, `role_id` ASC) USING BTREE,
  INDEX `IX_user_roles_role_id`(`role_id` ASC) USING BTREE,
  CONSTRAINT `FK_user_roles_roles_role_id` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `FK_user_roles_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of user_roles
-- ----------------------------
INSERT INTO `user_roles` VALUES (1, 1, 1, '2026-06-15 03:30:45', '2026-06-15 03:30:45');

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS `users`;
CREATE TABLE `users`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `password_hash` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `full_name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `contact_number` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `branch_id` int NULL DEFAULT NULL,
  `department_id` int NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_users_username`(`username` ASC) USING BTREE,
  UNIQUE INDEX `IX_users_email`(`email` ASC) USING BTREE,
  INDEX `IX_users_branch_id`(`branch_id` ASC) USING BTREE,
  INDEX `IX_users_department_id`(`department_id` ASC) USING BTREE,
  CONSTRAINT `FK_users_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_users_departments_department_id` FOREIGN KEY (`department_id`) REFERENCES `departments` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of users
-- ----------------------------
INSERT INTO `users` VALUES (1, 'admin', 'admin@gpos.local', '$2a$11$9TL7CKN.0e9TzMN9xWvBU...lh3rjgKTD2suXoC8qJOgyb7VjGvNW', '2026-06-15 03:30:45', '2026-06-15 03:30:45', 1, NULL, NULL, NULL, NULL, NULL);

-- ----------------------------
-- Table structure for warehouse_stocks
-- ----------------------------
DROP TABLE IF EXISTS `warehouse_stocks`;
CREATE TABLE `warehouse_stocks`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NOT NULL,
  `batch_id` int NOT NULL,
  `quantity` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_warehouse_stocks_batch_id`(`batch_id` ASC) USING BTREE,
  INDEX `IX_warehouse_stocks_product_id`(`product_id` ASC) USING BTREE,
  CONSTRAINT `FK_warehouse_stocks_product_batches_batch_id` FOREIGN KEY (`batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_warehouse_stocks_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `CK_warehouse_stocks_quantity_non_negative` CHECK (`quantity` >= 0)
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of warehouse_stocks
-- ----------------------------
INSERT INTO `warehouse_stocks` VALUES (1, 1, 1, 876.00, '2026-06-18 04:02:06.529349', '2026-06-18 04:02:06.529349');

SET FOREIGN_KEY_CHECKS = 1;

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

 Date: 21/07/2026 21:53:07
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
INSERT INTO `__efmigrationshistory` VALUES ('20260623002000_CreateDiscountRulesOnly', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260623003000_UpdateRebatesForAppliesTo', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260624090000_ImplementTransactionSalesPages', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260624093000_SeparateProductAndFuelSales', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260625090000_CreateLoyaltySettings', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260625172033_AddMemberEarningRules', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260626030148_RefactorEarningsAndEarningRules', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260626150554_CreateVoucherManagementEf', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260629000000_RefactorVoucherRulesLikeDiscountRules', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260629155956_CreateFinancialMetrics', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260629162622_MakeFinancialMetricsAppendOnly', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260629163736_AddFinancialMetricHistoryAmounts', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260629165307_AddVoucherRuleCode', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260630021805_MergeVoucherRulesIntoVouchers', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260630033713_AddProductSaleBatchSnapshots', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260701042403_AddVatSettings', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260703025458_AddFuelBatches', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260706154418_AddProductPriceHistory', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260707004000_AddNameAndRemarksToPumpMeterReadings', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260707013000_FixDispenserNozzleTankRelationship', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260707090000_MakeLowStockSettingProductOptional', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260707100000_UpdateLowStockSettingsPerItem', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260708003000_AddBatchIdToDailyStockRecords', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260708080000_AddBranchToPumpsSalesStockReceivings', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260708083000_AddBranchToFuelDeliveriesDailyStockRecords', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260708084500_AddBranchToLowStockSettings', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260710090000_AddOpeningCashAmountToShiftSettings', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260710100000_CreateCashModuleTables', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260710110000_AddDailyCashSessionColumns', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260710164000_AddBranchToPriceAdjustments', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260711145601_AddBranchFuelPrices', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260713041537_CreateStockAdjustmentAudit', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260714155515_AddBranchFuelEquipmentArchitecture', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260716235557_RestoreFuelReconciliationAndAddImmutableSaleSnapshots', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260717090000_AddProductSaleStockMovementTraceability', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260717110000_AddAuditableProductSaleVoid', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260717120000_AddAuthoritativeFuelFifo', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260718020916_AddCustomerReturnWorkflow', '9.0.0');
INSERT INTO `__efmigrationshistory` VALUES ('20260718154806_AddAuditableDailyStockWorkflow', '9.0.0');

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
-- Table structure for branch_fuel_prices
-- ----------------------------
DROP TABLE IF EXISTS `branch_fuel_prices`;
CREATE TABLE `branch_fuel_prices`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `branch_id` int NOT NULL,
  `fuel_id` int NOT NULL,
  `current_price_per_liter` decimal(18, 2) NOT NULL,
  `effective_at` datetime(6) NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_branch_fuel_prices_branch_id_fuel_id`(`branch_id` ASC, `fuel_id` ASC) USING BTREE,
  INDEX `IX_branch_fuel_prices_fuel_id`(`fuel_id` ASC) USING BTREE,
  CONSTRAINT `FK_branch_fuel_prices_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_branch_fuel_prices_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of branch_fuel_prices
-- ----------------------------
INSERT INTO `branch_fuel_prices` VALUES (1, 3, 2, 100.00, '2026-07-13 00:00:00.000000', 1, '2026-07-13 07:06:59.158750', '2026-07-13 07:06:59.158750');
INSERT INTO `branch_fuel_prices` VALUES (2, 3, 4, 1.00, '2026-07-15 00:00:00.000000', 1, '2026-07-14 17:08:08.573529', '2026-07-14 17:08:08.573529');

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
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of branches
-- ----------------------------
INSERT INTO `branches` VALUES (1, 'b1', NULL, 1, '2026-06-18 06:58:23.109938', '2026-06-18 06:58:23.109938');
INSERT INTO `branches` VALUES (2, 'b2', NULL, 1, '2026-06-18 06:58:29.289810', '2026-06-18 06:58:29.289810');
INSERT INTO `branches` VALUES (3, 'gtech', '123 anywhere street cauayan isabela', 1, '2026-07-07 16:41:01.456397', '2026-07-07 16:41:01.456397');

-- ----------------------------
-- Table structure for cash_ins
-- ----------------------------
DROP TABLE IF EXISTS `cash_ins`;
CREATE TABLE `cash_ins`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `branch_id` int NOT NULL,
  `shift_id` int NOT NULL,
  `user_id` int NOT NULL,
  `daily_cash_id` int NULL DEFAULT NULL,
  `transaction_datetime` datetime(6) NOT NULL,
  `amount` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `reason` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `remarks` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `created_by_user_id` int NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_cash_ins_scope`(`branch_id` ASC, `shift_id` ASC, `user_id` ASC, `transaction_datetime` ASC) USING BTREE,
  INDEX `IX_cash_ins_daily_cash_id`(`daily_cash_id` ASC) USING BTREE,
  INDEX `IX_cash_ins_shift_id`(`shift_id` ASC) USING BTREE,
  INDEX `IX_cash_ins_user_id`(`user_id` ASC) USING BTREE,
  INDEX `IX_cash_ins_created_by_user_id`(`created_by_user_id` ASC) USING BTREE,
  CONSTRAINT `FK_cash_ins_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_cash_ins_daily_cash_daily_cash_id` FOREIGN KEY (`daily_cash_id`) REFERENCES `daily_cash` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_cash_ins_shift_settings_shift_id` FOREIGN KEY (`shift_id`) REFERENCES `shift_settings` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_cash_ins_users_created_by_user_id` FOREIGN KEY (`created_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_cash_ins_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of cash_ins
-- ----------------------------
INSERT INTO `cash_ins` VALUES (1, 3, 2, 1, 1, '2026-07-10 20:46:44.202000', 100.00, 'hhbsd', NULL, 1, 1, '2026-07-10 13:47:41.861258', '2026-07-10 13:47:41.861258');

-- ----------------------------
-- Table structure for cash_outs
-- ----------------------------
DROP TABLE IF EXISTS `cash_outs`;
CREATE TABLE `cash_outs`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `branch_id` int NOT NULL,
  `shift_id` int NOT NULL,
  `user_id` int NOT NULL,
  `daily_cash_id` int NULL DEFAULT NULL,
  `transaction_datetime` datetime(6) NOT NULL,
  `amount` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `reason` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `remarks` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `created_by_user_id` int NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_cash_outs_scope`(`branch_id` ASC, `shift_id` ASC, `user_id` ASC, `transaction_datetime` ASC) USING BTREE,
  INDEX `IX_cash_outs_daily_cash_id`(`daily_cash_id` ASC) USING BTREE,
  INDEX `IX_cash_outs_shift_id`(`shift_id` ASC) USING BTREE,
  INDEX `IX_cash_outs_user_id`(`user_id` ASC) USING BTREE,
  INDEX `IX_cash_outs_created_by_user_id`(`created_by_user_id` ASC) USING BTREE,
  CONSTRAINT `FK_cash_outs_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_cash_outs_daily_cash_daily_cash_id` FOREIGN KEY (`daily_cash_id`) REFERENCES `daily_cash` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_cash_outs_shift_settings_shift_id` FOREIGN KEY (`shift_id`) REFERENCES `shift_settings` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_cash_outs_users_created_by_user_id` FOREIGN KEY (`created_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_cash_outs_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of cash_outs
-- ----------------------------
INSERT INTO `cash_outs` VALUES (1, 3, 2, 1, 1, '2026-07-10 22:22:21.319000', 50.00, 'kj', NULL, 1, 1, '2026-07-10 15:22:48.419783', '2026-07-10 15:22:48.419783');

-- ----------------------------
-- Table structure for cash_remittances
-- ----------------------------
DROP TABLE IF EXISTS `cash_remittances`;
CREATE TABLE `cash_remittances`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `remittance_no` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `branch_id` int NOT NULL,
  `shift_id` int NOT NULL,
  `user_id` int NOT NULL,
  `daily_cash_id` int NOT NULL,
  `expected_cash` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `actual_cash` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `remitted_amount` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `remittance_difference` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `received_by_user_id` int NOT NULL,
  `received_datetime` datetime(6) NOT NULL,
  `remarks` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_by_user_id` int NULL DEFAULT NULL,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_cash_remittances_remittance_no`(`remittance_no` ASC) USING BTREE,
  INDEX `IX_cash_remittances_daily_cash_id`(`daily_cash_id` ASC) USING BTREE,
  INDEX `IX_cash_remittances_branch_id`(`branch_id` ASC) USING BTREE,
  INDEX `IX_cash_remittances_shift_id`(`shift_id` ASC) USING BTREE,
  INDEX `IX_cash_remittances_user_id`(`user_id` ASC) USING BTREE,
  INDEX `IX_cash_remittances_received_by_user_id`(`received_by_user_id` ASC) USING BTREE,
  INDEX `IX_cash_remittances_created_by_user_id`(`created_by_user_id` ASC) USING BTREE,
  CONSTRAINT `FK_cash_remittances_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_cash_remittances_daily_cash_daily_cash_id` FOREIGN KEY (`daily_cash_id`) REFERENCES `daily_cash` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_cash_remittances_shift_settings_shift_id` FOREIGN KEY (`shift_id`) REFERENCES `shift_settings` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_cash_remittances_users_created_by_user_id` FOREIGN KEY (`created_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_cash_remittances_users_received_by_user_id` FOREIGN KEY (`received_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_cash_remittances_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of cash_remittances
-- ----------------------------
INSERT INTO `cash_remittances` VALUES (1, 'REMIT-20260713-0001', 3, 2, 1, 1, 50.00, 0.00, 50.00, 0.00, 1, '2026-07-13 17:09:39.141000', NULL, 1, 1, '2026-07-13 10:10:18.152812', '2026-07-13 10:10:18.152812');
INSERT INTO `cash_remittances` VALUES (2, 'REMIT-20260713-0002', 3, 2, 1, 2, 0.00, 0.00, 0.00, 0.00, 1, '2026-07-13 17:10:19.387000', NULL, 1, 1, '2026-07-13 10:10:33.535638', '2026-07-13 10:10:33.535638');

-- ----------------------------
-- Table structure for config
-- ----------------------------
DROP TABLE IF EXISTS `config`;
CREATE TABLE `config`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `intval` int NULL DEFAULT NULL,
  `txtval` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of config
-- ----------------------------
INSERT INTO `config` VALUES (1, 'com_name', '2026-06-25 13:06:29', '2026-06-25 13:06:29', NULL, 'Gtech Gas Station');
INSERT INTO `config` VALUES (2, 'com_addess', '2026-06-25 13:07:01', '2026-06-25 13:09:19', NULL, '123 Anywhere Street, Cauayan City, Isabela');
INSERT INTO `config` VALUES (3, 'com_contact', '2026-06-25 13:07:11', '2026-06-25 13:07:42', NULL, '+63912345678');
INSERT INTO `config` VALUES (4, 'com_owner', '2026-06-25 13:08:43', '2026-06-25 13:08:43', NULL, 'Gtech Business Solutions');
INSERT INTO `config` VALUES (5, 'tin', '2026-06-25 13:11:44', '2026-06-25 13:11:44', NULL, '123-456-789');
INSERT INTO `config` VALUES (6, 'mcompute', '2026-06-30 12:42:31', '2026-06-30 12:42:51', 1, NULL);
INSERT INTO `config` VALUES (7, 'fcompute', '2026-06-30 12:42:41', '2026-06-30 13:16:47', 2, NULL);
INSERT INTO `config` VALUES (8, 'pcompute', '2026-06-30 12:42:47', '2026-06-30 12:46:31', 1, NULL);
INSERT INTO `config` VALUES (9, 'sname', '2026-07-07 00:22:17', '2026-07-07 00:22:17', NULL, 'GPOS');

-- ----------------------------
-- Table structure for customer_fuel_return_items
-- ----------------------------
DROP TABLE IF EXISTS `customer_fuel_return_items`;
CREATE TABLE `customer_fuel_return_items`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `customer_return_id` int NOT NULL,
  `fuel_sale_id` int NOT NULL,
  `fuel_id` int NOT NULL,
  `original_tank_id` int NOT NULL,
  `original_pump_id` int NULL DEFAULT NULL,
  `original_nozzle_id` int NULL DEFAULT NULL,
  `liters` decimal(18, 2) NOT NULL,
  `original_price_per_liter` decimal(18, 2) NOT NULL,
  `return_amount` decimal(18, 2) NOT NULL,
  `inspection_result` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `disposition` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_customer_fuel_return_items_customer_return_id`(`customer_return_id` ASC) USING BTREE,
  INDEX `IX_customer_fuel_return_items_fuel_id`(`fuel_id` ASC) USING BTREE,
  INDEX `IX_customer_fuel_return_items_fuel_sale_id`(`fuel_sale_id` ASC) USING BTREE,
  INDEX `IX_customer_fuel_return_items_original_nozzle_id`(`original_nozzle_id` ASC) USING BTREE,
  INDEX `IX_customer_fuel_return_items_original_pump_id`(`original_pump_id` ASC) USING BTREE,
  INDEX `IX_customer_fuel_return_items_original_tank_id`(`original_tank_id` ASC) USING BTREE,
  CONSTRAINT `FK_customer_fuel_return_items_customer_returns_customer_return_~` FOREIGN KEY (`customer_return_id`) REFERENCES `customer_returns` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_customer_fuel_return_items_fuel_sales_fuel_sale_id` FOREIGN KEY (`fuel_sale_id`) REFERENCES `fuel_sales` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_customer_fuel_return_items_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_customer_fuel_return_items_nozzles_original_nozzle_id` FOREIGN KEY (`original_nozzle_id`) REFERENCES `nozzles` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_customer_fuel_return_items_pumps_original_pump_id` FOREIGN KEY (`original_pump_id`) REFERENCES `pumps` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_customer_fuel_return_items_tanks_original_tank_id` FOREIGN KEY (`original_tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of customer_fuel_return_items
-- ----------------------------

-- ----------------------------
-- Table structure for customer_product_return_items
-- ----------------------------
DROP TABLE IF EXISTS `customer_product_return_items`;
CREATE TABLE `customer_product_return_items`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `customer_return_id` int NOT NULL,
  `product_sale_id` int NOT NULL,
  `product_id` int NOT NULL,
  `original_batch_id` int NULL DEFAULT NULL,
  `original_display_stock_id` int NULL DEFAULT NULL,
  `quantity` decimal(18, 2) NOT NULL,
  `original_unit_price` decimal(18, 2) NOT NULL,
  `return_amount` decimal(18, 2) NOT NULL,
  `inspection_result` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `disposition` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `stock_movement_id` int NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_customer_product_return_items_stock_movement_id`(`stock_movement_id` ASC) USING BTREE,
  INDEX `IX_customer_product_return_items_customer_return_id`(`customer_return_id` ASC) USING BTREE,
  INDEX `IX_customer_product_return_items_original_batch_id`(`original_batch_id` ASC) USING BTREE,
  INDEX `IX_customer_product_return_items_original_display_stock_id`(`original_display_stock_id` ASC) USING BTREE,
  INDEX `IX_customer_product_return_items_product_id`(`product_id` ASC) USING BTREE,
  INDEX `IX_customer_product_return_items_product_sale_id`(`product_sale_id` ASC) USING BTREE,
  CONSTRAINT `FK_customer_product_return_items_customer_returns_customer_retu~` FOREIGN KEY (`customer_return_id`) REFERENCES `customer_returns` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_customer_product_return_items_display_stocks_original_displa~` FOREIGN KEY (`original_display_stock_id`) REFERENCES `display_stocks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_customer_product_return_items_product_batches_original_batch~` FOREIGN KEY (`original_batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_customer_product_return_items_product_sales_product_sale_id` FOREIGN KEY (`product_sale_id`) REFERENCES `product_sales` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_customer_product_return_items_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_customer_product_return_items_stock_movements_stock_movement~` FOREIGN KEY (`stock_movement_id`) REFERENCES `stock_movements` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of customer_product_return_items
-- ----------------------------

-- ----------------------------
-- Table structure for customer_returns
-- ----------------------------
DROP TABLE IF EXISTS `customer_returns`;
CREATE TABLE `customer_returns`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `return_no` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `sale_id` int NOT NULL,
  `original_receipt_no` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `branch_id` int NOT NULL,
  `member_id` int NULL DEFAULT NULL,
  `return_type` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `refund_amount` decimal(18, 2) NOT NULL,
  `reason` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `status` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `created_by_user_id` int NOT NULL,
  `created_at` datetime(6) NOT NULL,
  `inspected_by_user_id` int NULL DEFAULT NULL,
  `inspected_at` datetime(6) NULL DEFAULT NULL,
  `inspection_decision` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `inspection_notes` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `processed_by_user_id` int NULL DEFAULT NULL,
  `completed_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_customer_returns_return_no`(`return_no` ASC) USING BTREE,
  INDEX `IX_customer_returns_branch_id_status_created_at`(`branch_id` ASC, `status` ASC, `created_at` ASC) USING BTREE,
  INDEX `IX_customer_returns_created_by_user_id`(`created_by_user_id` ASC) USING BTREE,
  INDEX `IX_customer_returns_inspected_by_user_id`(`inspected_by_user_id` ASC) USING BTREE,
  INDEX `IX_customer_returns_member_id`(`member_id` ASC) USING BTREE,
  INDEX `IX_customer_returns_processed_by_user_id`(`processed_by_user_id` ASC) USING BTREE,
  INDEX `IX_customer_returns_sale_id`(`sale_id` ASC) USING BTREE,
  CONSTRAINT `FK_customer_returns_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_customer_returns_members_member_id` FOREIGN KEY (`member_id`) REFERENCES `members` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_customer_returns_sales_sale_id` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_customer_returns_users_created_by_user_id` FOREIGN KEY (`created_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_customer_returns_users_inspected_by_user_id` FOREIGN KEY (`inspected_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_customer_returns_users_processed_by_user_id` FOREIGN KEY (`processed_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of customer_returns
-- ----------------------------

-- ----------------------------
-- Table structure for daily_cash
-- ----------------------------
DROP TABLE IF EXISTS `daily_cash`;
CREATE TABLE `daily_cash`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `branch_id` int NOT NULL,
  `shift_id` int NOT NULL,
  `user_id` int NOT NULL,
  `business_date` date NOT NULL,
  `opening_cash` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `cash_sales` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `total_cash_in` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `total_cash_out` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `expected_cash` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `actual_cash` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `difference` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `remitted_amount` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `received_by_user_id` int NULL DEFAULT NULL,
  `remarks` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_by_user_id` int NULL DEFAULT NULL,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `opened_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_daily_cash_scope`(`branch_id` ASC, `shift_id` ASC, `user_id` ASC, `business_date` ASC) USING BTREE,
  INDEX `IX_daily_cash_shift_id`(`shift_id` ASC) USING BTREE,
  INDEX `IX_daily_cash_user_id`(`user_id` ASC) USING BTREE,
  INDEX `IX_daily_cash_received_by_user_id`(`received_by_user_id` ASC) USING BTREE,
  INDEX `IX_daily_cash_created_by_user_id`(`created_by_user_id` ASC) USING BTREE,
  CONSTRAINT `FK_daily_cash_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_daily_cash_shift_settings_shift_id` FOREIGN KEY (`shift_id`) REFERENCES `shift_settings` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_daily_cash_users_created_by_user_id` FOREIGN KEY (`created_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_daily_cash_users_received_by_user_id` FOREIGN KEY (`received_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_daily_cash_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of daily_cash
-- ----------------------------
INSERT INTO `daily_cash` VALUES (1, 3, 2, 1, '2026-07-10', 0.00, 0.00, 100.00, 50.00, 50.00, 0.00, -50.00, 50.00, NULL, NULL, 4, 1, '2026-07-10 03:17:59.013245', '2026-07-13 10:10:18.152812', NULL);
INSERT INTO `daily_cash` VALUES (2, 3, 2, 1, '2026-07-13', 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, NULL, NULL, 4, 1, '2026-07-13 07:12:33.347570', '2026-07-13 10:10:33.535638', '2026-07-13 07:12:33.347570');
INSERT INTO `daily_cash` VALUES (3, 3, 2, 1, '2026-07-17', 100.00, 905.00, 0.00, 0.00, 1005.00, 100.00, -905.00, 0.00, NULL, 'QA Product Void runtime acceptance', 1, 1, '2026-07-17 10:23:42.000000', '2026-07-17 16:53:19.360743', '2026-07-17 10:23:42.000000');

-- ----------------------------
-- Table structure for daily_stock_records
-- ----------------------------
DROP TABLE IF EXISTS `daily_stock_records`;
CREATE TABLE `daily_stock_records`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `stock_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `record_date` date NOT NULL,
  `product_id` int NULL DEFAULT NULL,
  `batch_id` int NULL DEFAULT NULL,
  `tank_id` int NULL DEFAULT NULL,
  `fuel_id` int NULL DEFAULT NULL,
  `beginning_quantity` decimal(18, 2) NOT NULL,
  `sold_quantity` decimal(18, 2) NOT NULL,
  `actual_quantity` decimal(18, 2) NOT NULL,
  `ending_quantity` decimal(18, 2) NOT NULL,
  `loss_quantity` decimal(18, 2) NOT NULL,
  `remarks` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `created_by` int NULL DEFAULT NULL,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `branch_id` int NULL DEFAULT NULL,
  `adjustment_quantity` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `confirmed_at` datetime(6) NULL DEFAULT NULL,
  `confirmed_by` int NULL DEFAULT NULL,
  `current_official_quantity` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `display_stock_id` int NULL DEFAULT NULL,
  `expected_quantity` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `new_official_quantity` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `received_quantity` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `reconciliation_adjustment` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `record_no` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  `status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  `transfer_in_quantity` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `transfer_out_quantity` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `variance_quantity` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `warehouse_stock_id` int NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_daily_stock_records_record_no`(`record_no` ASC) USING BTREE,
  INDEX `IX_daily_stock_records_batch_id`(`batch_id` ASC) USING BTREE,
  INDEX `IX_daily_stock_records_fuel_id`(`fuel_id` ASC) USING BTREE,
  INDEX `IX_daily_stock_records_product_id`(`product_id` ASC) USING BTREE,
  INDEX `IX_daily_stock_records_record_date`(`record_date` ASC) USING BTREE,
  INDEX `IX_daily_stock_records_stock_type`(`stock_type` ASC) USING BTREE,
  INDEX `IX_daily_stock_records_tank_id`(`tank_id` ASC) USING BTREE,
  INDEX `IX_daily_stock_records_branch_id`(`branch_id` ASC) USING BTREE,
  INDEX `IX_daily_stock_records_confirmed_by`(`confirmed_by` ASC) USING BTREE,
  INDEX `IX_daily_stock_records_created_by`(`created_by` ASC) USING BTREE,
  CONSTRAINT `FK_daily_stock_records_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_daily_stock_records_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_daily_stock_records_product_batches_batch_id` FOREIGN KEY (`batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_daily_stock_records_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_daily_stock_records_tanks_tank_id` FOREIGN KEY (`tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_daily_stock_records_users_confirmed_by` FOREIGN KEY (`confirmed_by`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_daily_stock_records_users_created_by` FOREIGN KEY (`created_by`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of daily_stock_records
-- ----------------------------
INSERT INTO `daily_stock_records` VALUES (1, 'Warehouse', '2026-07-18', 4, 6, NULL, NULL, 10.00, 0.00, 10.00, 9.00, 1.00, 'na', 1, '2026-07-18 16:02:21.028115', '2026-07-18 16:02:21.028115', 3, 0.00, NULL, NULL, 10.00, NULL, 10.00, 9.00, 0.00, -1.00, 'DS-20260718-000001', 'Draft', 0.00, 0.00, -1.00, 3);
INSERT INTO `daily_stock_records` VALUES (2, 'Warehouse', '2026-07-19', 4, 6, NULL, NULL, 10.00, 0.00, 10.00, 0.00, 10.00, NULL, 1, '2026-07-19 13:30:19.672561', '2026-07-19 15:45:09.665691', 3, 0.00, '2026-07-19 15:45:09.665691', 1, 10.00, NULL, 10.00, 0.00, 0.00, -10.00, 'DS-20260719-000001', 'Confirmed', 0.00, 0.00, -10.00, 3);
INSERT INTO `daily_stock_records` VALUES (3, 'Warehouse', '2026-07-19', 1, 8, NULL, NULL, 10.00, 0.00, 10.00, 0.00, 10.00, NULL, 1, '2026-07-19 15:14:11.911951', '2026-07-19 15:30:55.815318', 3, 0.00, '2026-07-19 15:30:55.815318', 1, 10.00, NULL, 10.00, 0.00, 0.00, -10.00, 'DS-20260719-000002', 'Confirmed', 0.00, 0.00, -10.00, 6);
INSERT INTO `daily_stock_records` VALUES (4, 'Warehouse', '2026-07-19', 1, 7, NULL, NULL, 10.00, 0.00, 10.00, 9.00, 1.00, NULL, 1, '2026-07-19 16:40:05.963900', '2026-07-19 16:40:05.963900', 3, 0.00, NULL, NULL, 10.00, NULL, 10.00, 9.00, 0.00, -1.00, 'DS-20260719-000003', 'Draft', 0.00, 0.00, -1.00, 5);
INSERT INTO `daily_stock_records` VALUES (5, 'Warehouse', '2026-07-20', 1, 7, NULL, NULL, 10.00, 0.00, 10.00, 0.00, 10.00, NULL, 1, '2026-07-20 09:28:53.025944', '2026-07-20 09:28:53.025944', 3, 0.00, NULL, NULL, 10.00, NULL, 10.00, 0.00, 0.00, -10.00, 'DS-20260720-000001', 'Draft', 0.00, 0.00, -10.00, 5);

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
  `discount_id` int NULL DEFAULT NULL,
  `minimum_amount` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `member_required` int NOT NULL DEFAULT 0,
  `start_date` datetime NULL DEFAULT NULL,
  `end_date` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_discount_rules_discount_id`(`discount_id` ASC) USING BTREE,
  CONSTRAINT `FK_discount_rules_discounts_discount_id` FOREIGN KEY (`discount_id`) REFERENCES `discounts` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of discount_rules
-- ----------------------------
INSERT INTO `discount_rules` VALUES (1, 'owner', 'Percentage', 90.00, 'Both', 1, '2026-06-25 08:25:19', '2026-06-25 08:29:55', 5, 100.00, 1, NULL, NULL);
INSERT INTO `discount_rules` VALUES (2, 'owner', 'Percentage', 90.00, 'Fuel', 1, '2026-06-25 08:26:09', '2026-06-29 05:54:09', 5, 100.00, 1, '2026-05-25 00:00:00', '2026-09-25 00:00:00');
INSERT INTO `discount_rules` VALUES (3, 'Sr person', 'Percentage', 20.00, 'Fuel', 1, '2026-06-26 06:47:16', '2026-06-26 06:47:16', 6, 1.00, 0, '2026-06-26 00:00:00', NULL);

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
) ENGINE = InnoDB AUTO_INCREMENT = 7 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of discounts
-- ----------------------------
INSERT INTO `discounts` VALUES (1, 'Member', 5.00, 1, '2026-06-18 05:10:47', '2026-06-18 05:10:47');
INSERT INTO `discounts` VALUES (2, 'VIP', 10.00, 1, '2026-06-18 05:11:03', '2026-06-18 05:11:03');
INSERT INTO `discounts` VALUES (3, 'VIP+', 15.00, 1, '2026-06-18 05:11:13', '2026-06-18 05:11:13');
INSERT INTO `discounts` VALUES (4, 'Elite', 20.00, 1, '2026-06-18 05:11:36', '2026-06-18 05:31:18');
INSERT INTO `discounts` VALUES (5, 'owner', 90.00, 1, '2026-06-24 07:57:19', '2026-06-25 07:21:20');
INSERT INTO `discounts` VALUES (6, 'Sr person', 0.00, 1, '2026-06-26 06:46:39', '2026-06-26 06:46:39');

-- ----------------------------
-- Table structure for dispensers
-- ----------------------------
DROP TABLE IF EXISTS `dispensers`;
CREATE TABLE `dispensers`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `branch_id` int NOT NULL,
  `dispenser_code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `location` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `remarks` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_dispensers_branch_id_dispenser_code`(`branch_id` ASC, `dispenser_code` ASC) USING BTREE,
  CONSTRAINT `FK_dispensers_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of dispensers
-- ----------------------------
INSERT INTO `dispensers` VALUES (1, 3, 'jhjh', 'jhjh', 'hh', NULL, 1, '2026-07-14 16:33:36.641380', '2026-07-14 16:33:36.641380');
INSERT INTO `dispensers` VALUES (2, 2, 'hfgh', 'ghhgh', 'tetr', NULL, 1, '2026-07-14 17:10:28.644706', '2026-07-14 17:10:28.644706');
INSERT INTO `dispensers` VALUES (3, 3, 'QA-FIFO-DISP', 'QA FIFO Dispenser', NULL, 'QA Fuel FIFO', 1, '2026-07-17 10:46:17.000000', '2026-07-17 10:46:17.000000');

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
  `branch_id` int NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_display_stocks_batch_id`(`batch_id` ASC) USING BTREE,
  INDEX `IX_display_stocks_product_id`(`product_id` ASC) USING BTREE,
  CONSTRAINT `FK_display_stocks_product_batches_batch_id` FOREIGN KEY (`batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_display_stocks_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `CK_display_stocks_quantity_non_negative` CHECK (`quantity` >= 0)
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of display_stocks
-- ----------------------------
INSERT INTO `display_stocks` VALUES (1, 2, 2, 32.00, '2026-06-18 06:37:43.782840', '2026-06-29 23:21:10.106350', NULL);
INSERT INTO `display_stocks` VALUES (2, 3, 3, 81.00, '2026-06-29 15:40:46.495327', '2026-07-07 01:32:11.886912', NULL);
INSERT INTO `display_stocks` VALUES (3, 4, 4, 884.00, '2026-06-29 16:24:45.052767', '2026-07-16 00:27:55.736237', NULL);
INSERT INTO `display_stocks` VALUES (4, 4, 4, 177.00, '2026-07-13 06:33:20.046865', '2026-07-17 16:53:18.568575', 3);
INSERT INTO `display_stocks` VALUES (5, 4, 9, 0.00, '2026-07-17 10:23:42.000000', '2026-07-17 10:34:25.918212', 3);

-- ----------------------------
-- Table structure for earning_rules
-- ----------------------------
DROP TABLE IF EXISTS `earning_rules`;
CREATE TABLE `earning_rules`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `earnings_id` int NOT NULL,
  `earn_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `earn_value` decimal(18, 2) NOT NULL,
  `applies_to` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `minimum_amount` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `member_required` int NOT NULL DEFAULT 0,
  `start_date` datetime NULL DEFAULT NULL,
  `end_date` datetime NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_earning_rules_earnings_id`(`earnings_id` ASC) USING BTREE,
  CONSTRAINT `FK_earning_rules_earnings_earnings_id` FOREIGN KEY (`earnings_id`) REFERENCES `earnings` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of earning_rules
-- ----------------------------
INSERT INTO `earning_rules` VALUES (1, 'earn1', '2026-06-26 03:12:21.833215', '2026-06-26 03:18:15.869597', 1, 'Percentage', 10.00, 'Both', 10.00, 1, '2026-05-13 00:00:00', '2027-06-26 00:00:00', 1);

-- ----------------------------
-- Table structure for earnings
-- ----------------------------
DROP TABLE IF EXISTS `earnings`;
CREATE TABLE `earnings`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_earnings_name`(`name` ASC) USING BTREE,
  INDEX `IX_earnings_status`(`status` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of earnings
-- ----------------------------
INSERT INTO `earnings` VALUES (1, 'kjhkj', 'fghfg', 1, '2026-06-26 03:11:28', '2026-06-26 03:11:28');

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
-- Table structure for financial_metrics
-- ----------------------------
DROP TABLE IF EXISTS `financial_metrics`;
CREATE TABLE `financial_metrics`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `metric_code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `current_amount` decimal(18, 2) NOT NULL,
  `metric_date` date NOT NULL,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `new_amount` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `old_amount` decimal(18, 2) NOT NULL DEFAULT 0.00,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_financial_metrics_metric_code`(`metric_code` ASC) USING BTREE,
  INDEX `IX_financial_metrics_metric_date_metric_code`(`metric_date` ASC, `metric_code` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 89 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of financial_metrics
-- ----------------------------
INSERT INTO `financial_metrics` VALUES (1, 'gross_sales', 462.00, '2026-06-29', '2026-06-29 23:12:54.981718', '2026-06-29 16:25:30.732782', 462.00, 0.00);
INSERT INTO `financial_metrics` VALUES (2, 'net_sales', 462.00, '2026-06-29', '2026-06-29 23:12:55.012386', '2026-06-29 16:25:30.734552', 462.00, 0.00);
INSERT INTO `financial_metrics` VALUES (3, 'gross_profit', 109.50, '2026-06-29', '2026-06-29 23:12:55.017238', '2026-06-29 16:25:30.739690', 109.50, 0.00);
INSERT INTO `financial_metrics` VALUES (4, 'cost_of_goods_sold', 353.50, '2026-06-29', '2026-06-29 23:12:55.021595', '2026-06-29 16:25:30.740947', 353.50, 0.00);
INSERT INTO `financial_metrics` VALUES (5, 'product_sales', 462.00, '2026-06-29', '2026-06-29 23:12:55.026281', '2026-06-29 16:25:30.741790', 462.00, 0.00);
INSERT INTO `financial_metrics` VALUES (6, 'gross_sales', 664.00, '2026-06-29', '2026-06-29 16:30:46.481195', '2026-06-29 16:30:46.481195', 664.00, 0.00);
INSERT INTO `financial_metrics` VALUES (7, 'net_sales', 664.00, '2026-06-29', '2026-06-29 16:30:46.554085', '2026-06-29 16:30:46.554085', 664.00, 0.00);
INSERT INTO `financial_metrics` VALUES (8, 'gross_profit', 158.50, '2026-06-29', '2026-06-29 16:30:46.561165', '2026-06-29 16:30:46.561165', 158.50, 0.00);
INSERT INTO `financial_metrics` VALUES (9, 'cost_of_goods_sold', 506.50, '2026-06-29', '2026-06-29 16:30:46.567722', '2026-06-29 16:30:46.567722', 506.50, 0.00);
INSERT INTO `financial_metrics` VALUES (10, 'product_sales', 664.00, '2026-06-29', '2026-06-29 16:30:46.573866', '2026-06-29 16:30:46.573866', 664.00, 0.00);
INSERT INTO `financial_metrics` VALUES (11, 'gross_sales', 666.00, '2026-06-29', '2026-06-29 16:40:20.182966', '2026-06-29 16:40:20.182966', 2.00, 664.00);
INSERT INTO `financial_metrics` VALUES (12, 'net_sales', 666.00, '2026-06-29', '2026-06-29 16:40:20.242361', '2026-06-29 16:40:20.242361', 2.00, 664.00);
INSERT INTO `financial_metrics` VALUES (13, 'gross_profit', 159.50, '2026-06-29', '2026-06-29 16:40:20.247416', '2026-06-29 16:40:20.247416', 1.00, 158.50);
INSERT INTO `financial_metrics` VALUES (14, 'cost_of_goods_sold', 507.50, '2026-06-29', '2026-06-29 16:40:20.250876', '2026-06-29 16:40:20.250876', 1.00, 506.50);
INSERT INTO `financial_metrics` VALUES (15, 'product_sales', 666.00, '2026-06-29', '2026-06-29 16:40:20.254523', '2026-06-29 16:40:20.254523', 2.00, 664.00);
INSERT INTO `financial_metrics` VALUES (16, 'gross_sales', 668.00, '2026-06-29', '2026-06-29 16:41:23.124956', '2026-06-29 16:41:23.124956', 2.00, 666.00);
INSERT INTO `financial_metrics` VALUES (17, 'net_sales', 668.00, '2026-06-29', '2026-06-29 16:41:23.130961', '2026-06-29 16:41:23.130961', 2.00, 666.00);
INSERT INTO `financial_metrics` VALUES (18, 'gross_profit', 160.50, '2026-06-29', '2026-06-29 16:41:23.134507', '2026-06-29 16:41:23.134507', 1.00, 159.50);
INSERT INTO `financial_metrics` VALUES (19, 'cost_of_goods_sold', 508.50, '2026-06-29', '2026-06-29 16:41:23.140814', '2026-06-29 16:41:23.140814', 1.00, 507.50);
INSERT INTO `financial_metrics` VALUES (20, 'product_sales', 668.00, '2026-06-29', '2026-06-29 16:41:23.145223', '2026-06-29 16:41:23.145223', 2.00, 666.00);
INSERT INTO `financial_metrics` VALUES (21, 'gross_sales', 670.00, '2026-06-29', '2026-06-29 16:42:13.259790', '2026-06-29 16:42:13.259790', 2.00, 668.00);
INSERT INTO `financial_metrics` VALUES (22, 'net_sales', 670.00, '2026-06-29', '2026-06-29 16:42:13.262307', '2026-06-29 16:42:13.262307', 2.00, 668.00);
INSERT INTO `financial_metrics` VALUES (23, 'gross_profit', 161.50, '2026-06-29', '2026-06-29 16:42:13.264779', '2026-06-29 16:42:13.264779', 1.00, 160.50);
INSERT INTO `financial_metrics` VALUES (24, 'cost_of_goods_sold', 509.50, '2026-06-29', '2026-06-29 16:42:13.269240', '2026-06-29 16:42:13.269240', 1.00, 508.50);
INSERT INTO `financial_metrics` VALUES (25, 'product_sales', 670.00, '2026-06-29', '2026-06-29 16:42:13.272258', '2026-06-29 16:42:13.272258', 2.00, 668.00);
INSERT INTO `financial_metrics` VALUES (26, 'gross_sales', 770.00, '2026-06-29', '2026-06-29 16:42:52.042030', '2026-06-29 16:42:52.042030', 100.00, 670.00);
INSERT INTO `financial_metrics` VALUES (27, 'net_sales', 680.00, '2026-06-29', '2026-06-29 16:42:52.046083', '2026-06-29 16:42:52.046083', 10.00, 670.00);
INSERT INTO `financial_metrics` VALUES (28, 'gross_profit', 95.50, '2026-06-29', '2026-06-29 16:42:52.048475', '2026-06-29 16:42:52.048475', -66.00, 161.50);
INSERT INTO `financial_metrics` VALUES (29, 'cost_of_goods_sold', 585.50, '2026-06-29', '2026-06-29 16:42:52.052514', '2026-06-29 16:42:52.052514', 76.00, 509.50);
INSERT INTO `financial_metrics` VALUES (30, 'product_sales', 770.00, '2026-06-29', '2026-06-29 16:42:52.054956', '2026-06-29 16:42:52.054956', 100.00, 670.00);
INSERT INTO `financial_metrics` VALUES (31, 'total_discount', 90.00, '2026-06-29', '2026-06-29 16:42:52.057170', '2026-06-29 16:42:52.057170', 90.00, 0.00);
INSERT INTO `financial_metrics` VALUES (32, 'gross_sales', 200.00, '2026-06-30', '2026-06-30 02:07:29.298323', '2026-06-30 02:07:29.298323', 200.00, 0.00);
INSERT INTO `financial_metrics` VALUES (33, 'net_sales', 100.00, '2026-06-30', '2026-06-30 02:07:29.364076', '2026-06-30 02:07:29.364076', 100.00, 0.00);
INSERT INTO `financial_metrics` VALUES (34, 'gross_profit', -52.00, '2026-06-30', '2026-06-30 02:07:29.371391', '2026-06-30 02:07:29.371391', -52.00, 0.00);
INSERT INTO `financial_metrics` VALUES (35, 'cost_of_goods_sold', 152.00, '2026-06-30', '2026-06-30 02:07:29.377432', '2026-06-30 02:07:29.377432', 152.00, 0.00);
INSERT INTO `financial_metrics` VALUES (36, 'product_sales', 200.00, '2026-06-30', '2026-06-30 02:07:29.381929', '2026-06-30 02:07:29.381929', 200.00, 0.00);
INSERT INTO `financial_metrics` VALUES (37, 'total_discount', 100.00, '2026-06-30', '2026-06-30 02:07:29.389160', '2026-06-30 02:07:29.389160', 100.00, 0.00);
INSERT INTO `financial_metrics` VALUES (38, 'gross_sales', 202.00, '2026-06-30', '2026-06-30 02:44:48.141948', '2026-06-30 02:44:48.141948', 2.00, 200.00);
INSERT INTO `financial_metrics` VALUES (39, 'net_sales', 102.00, '2026-06-30', '2026-06-30 02:44:48.191654', '2026-06-30 02:44:48.191654', 2.00, 100.00);
INSERT INTO `financial_metrics` VALUES (40, 'gross_profit', -51.00, '2026-06-30', '2026-06-30 02:44:48.195200', '2026-06-30 02:44:48.195200', 1.00, -52.00);
INSERT INTO `financial_metrics` VALUES (41, 'cost_of_goods_sold', 153.00, '2026-06-30', '2026-06-30 02:44:48.198456', '2026-06-30 02:44:48.198456', 1.00, 152.00);
INSERT INTO `financial_metrics` VALUES (42, 'product_sales', 202.00, '2026-06-30', '2026-06-30 02:44:48.201598', '2026-06-30 02:44:48.201598', 2.00, 200.00);
INSERT INTO `financial_metrics` VALUES (43, 'gross_sales', 200.00, '2026-07-07', '2026-07-06 18:32:13.759728', '2026-07-06 18:32:13.759728', 200.00, 0.00);
INSERT INTO `financial_metrics` VALUES (44, 'net_sales', -80.00, '2026-07-07', '2026-07-06 18:32:13.910411', '2026-07-06 18:32:13.910411', -80.00, 0.00);
INSERT INTO `financial_metrics` VALUES (45, 'gross_profit', -232.00, '2026-07-07', '2026-07-06 18:32:13.922638', '2026-07-06 18:32:13.922638', -232.00, 0.00);
INSERT INTO `financial_metrics` VALUES (46, 'cost_of_goods_sold', 152.00, '2026-07-07', '2026-07-06 18:32:13.933195', '2026-07-06 18:32:13.933195', 152.00, 0.00);
INSERT INTO `financial_metrics` VALUES (47, 'product_sales', 200.00, '2026-07-07', '2026-07-06 18:32:13.943452', '2026-07-06 18:32:13.943452', 200.00, 0.00);
INSERT INTO `financial_metrics` VALUES (48, 'total_discount', 280.00, '2026-07-07', '2026-07-06 18:32:13.955888', '2026-07-06 18:32:13.955888', 280.00, 0.00);
INSERT INTO `financial_metrics` VALUES (49, 'gross_sales', 4.00, '2026-07-16', '2026-07-15 17:27:56.256433', '2026-07-15 17:27:56.256433', 4.00, 0.00);
INSERT INTO `financial_metrics` VALUES (50, 'net_sales', 4.00, '2026-07-16', '2026-07-15 17:27:56.329123', '2026-07-15 17:27:56.329123', 4.00, 0.00);
INSERT INTO `financial_metrics` VALUES (51, 'gross_profit', 2.00, '2026-07-16', '2026-07-15 17:27:56.334391', '2026-07-15 17:27:56.334391', 2.00, 0.00);
INSERT INTO `financial_metrics` VALUES (52, 'cost_of_goods_sold', 2.00, '2026-07-16', '2026-07-15 17:27:56.338508', '2026-07-15 17:27:56.338508', 2.00, 0.00);
INSERT INTO `financial_metrics` VALUES (53, 'product_sales', 4.00, '2026-07-16', '2026-07-15 17:27:56.342215', '2026-07-15 17:27:56.342215', 4.00, 0.00);
INSERT INTO `financial_metrics` VALUES (54, 'gross_sales', 2.00, '2026-07-17', '2026-07-17 03:24:08.648806', '2026-07-17 03:24:08.648806', 2.00, 0.00);
INSERT INTO `financial_metrics` VALUES (55, 'net_sales', 2.00, '2026-07-17', '2026-07-17 03:24:08.726391', '2026-07-17 03:24:08.726391', 2.00, 0.00);
INSERT INTO `financial_metrics` VALUES (56, 'gross_profit', 1.25, '2026-07-17', '2026-07-17 03:24:08.735355', '2026-07-17 03:24:08.735355', 1.25, 0.00);
INSERT INTO `financial_metrics` VALUES (57, 'cost_of_goods_sold', 0.75, '2026-07-17', '2026-07-17 03:24:08.743622', '2026-07-17 03:24:08.743622', 0.75, 0.00);
INSERT INTO `financial_metrics` VALUES (58, 'product_sales', 2.00, '2026-07-17', '2026-07-17 03:24:08.749865', '2026-07-17 03:24:08.749865', 2.00, 0.00);
INSERT INTO `financial_metrics` VALUES (59, 'gross_sales', 3.00, '2026-07-17', '2026-07-17 03:28:06.402425', '2026-07-17 03:28:06.402425', 1.00, 2.00);
INSERT INTO `financial_metrics` VALUES (60, 'net_sales', 3.00, '2026-07-17', '2026-07-17 03:28:06.456256', '2026-07-17 03:28:06.456256', 1.00, 2.00);
INSERT INTO `financial_metrics` VALUES (61, 'gross_profit', 2.00, '2026-07-17', '2026-07-17 03:28:06.467113', '2026-07-17 03:28:06.467113', 0.75, 1.25);
INSERT INTO `financial_metrics` VALUES (62, 'cost_of_goods_sold', 1.00, '2026-07-17', '2026-07-17 03:28:06.477522', '2026-07-17 03:28:06.477522', 0.25, 0.75);
INSERT INTO `financial_metrics` VALUES (63, 'product_sales', 3.00, '2026-07-17', '2026-07-17 03:28:06.487672', '2026-07-17 03:28:06.487672', 1.00, 2.00);
INSERT INTO `financial_metrics` VALUES (64, 'gross_sales', 4.00, '2026-07-17', '2026-07-17 03:30:15.085416', '2026-07-17 03:30:15.085416', 1.00, 3.00);
INSERT INTO `financial_metrics` VALUES (65, 'net_sales', 4.00, '2026-07-17', '2026-07-17 03:30:15.089450', '2026-07-17 03:30:15.089450', 1.00, 3.00);
INSERT INTO `financial_metrics` VALUES (66, 'gross_profit', 2.75, '2026-07-17', '2026-07-17 03:30:15.095693', '2026-07-17 03:30:15.095693', 0.75, 2.00);
INSERT INTO `financial_metrics` VALUES (67, 'cost_of_goods_sold', 1.25, '2026-07-17', '2026-07-17 03:30:15.098697', '2026-07-17 03:30:15.098697', 0.25, 1.00);
INSERT INTO `financial_metrics` VALUES (68, 'product_sales', 4.00, '2026-07-17', '2026-07-17 03:30:15.102515', '2026-07-17 03:30:15.102515', 1.00, 3.00);
INSERT INTO `financial_metrics` VALUES (69, 'gross_sales', 5.00, '2026-07-17', '2026-07-17 03:34:25.979165', '2026-07-17 03:34:25.979165', 1.00, 4.00);
INSERT INTO `financial_metrics` VALUES (70, 'net_sales', 5.00, '2026-07-17', '2026-07-17 03:34:25.985298', '2026-07-17 03:34:25.985298', 1.00, 4.00);
INSERT INTO `financial_metrics` VALUES (71, 'gross_profit', 3.50, '2026-07-17', '2026-07-17 03:34:25.992354', '2026-07-17 03:34:25.992354', 0.75, 2.75);
INSERT INTO `financial_metrics` VALUES (72, 'cost_of_goods_sold', 1.50, '2026-07-17', '2026-07-17 03:34:25.996553', '2026-07-17 03:34:25.996553', 0.25, 1.25);
INSERT INTO `financial_metrics` VALUES (73, 'product_sales', 5.00, '2026-07-17', '2026-07-17 03:34:26.001003', '2026-07-17 03:34:26.001003', 1.00, 4.00);
INSERT INTO `financial_metrics` VALUES (74, 'gross_sales', 530.00, '2026-07-17', '2026-07-17 03:46:48.529457', '2026-07-17 03:46:48.529457', 525.00, 5.00);
INSERT INTO `financial_metrics` VALUES (75, 'net_sales', 530.00, '2026-07-17', '2026-07-17 03:46:48.591873', '2026-07-17 03:46:48.591873', 525.00, 5.00);
INSERT INTO `financial_metrics` VALUES (76, 'gross_profit', 438.50, '2026-07-17', '2026-07-17 03:46:48.606278', '2026-07-17 03:46:48.606278', 435.00, 3.50);
INSERT INTO `financial_metrics` VALUES (77, 'cost_of_goods_sold', 91.50, '2026-07-17', '2026-07-17 03:46:48.645841', '2026-07-17 03:46:48.645841', 90.00, 1.50);
INSERT INTO `financial_metrics` VALUES (78, 'fuel_sales', 525.00, '2026-07-17', '2026-07-17 03:46:48.655902', '2026-07-17 03:46:48.655902', 525.00, 0.00);
INSERT INTO `financial_metrics` VALUES (79, 'gross_sales', 905.00, '2026-07-17', '2026-07-17 03:47:28.051551', '2026-07-17 03:47:28.051551', 375.00, 530.00);
INSERT INTO `financial_metrics` VALUES (80, 'net_sales', 905.00, '2026-07-17', '2026-07-17 03:47:28.072019', '2026-07-17 03:47:28.072019', 375.00, 530.00);
INSERT INTO `financial_metrics` VALUES (81, 'gross_profit', 713.50, '2026-07-17', '2026-07-17 03:47:28.084251', '2026-07-17 03:47:28.084251', 275.00, 438.50);
INSERT INTO `financial_metrics` VALUES (82, 'cost_of_goods_sold', 191.50, '2026-07-17', '2026-07-17 03:47:28.100711', '2026-07-17 03:47:28.100711', 100.00, 91.50);
INSERT INTO `financial_metrics` VALUES (83, 'fuel_sales', 900.00, '2026-07-17', '2026-07-17 03:47:28.110567', '2026-07-17 03:47:28.110567', 375.00, 525.00);
INSERT INTO `financial_metrics` VALUES (84, 'gross_sales', 907.00, '2026-07-17', '2026-07-17 14:51:07.226728', '2026-07-17 14:51:07.226728', 2.00, 905.00);
INSERT INTO `financial_metrics` VALUES (85, 'net_sales', 907.00, '2026-07-17', '2026-07-17 14:51:07.267366', '2026-07-17 14:51:07.267366', 2.00, 905.00);
INSERT INTO `financial_metrics` VALUES (86, 'gross_profit', 714.50, '2026-07-17', '2026-07-17 14:51:07.273782', '2026-07-17 14:51:07.273782', 1.00, 713.50);
INSERT INTO `financial_metrics` VALUES (87, 'cost_of_goods_sold', 192.50, '2026-07-17', '2026-07-17 14:51:07.278681', '2026-07-17 14:51:07.278681', 1.00, 191.50);
INSERT INTO `financial_metrics` VALUES (88, 'product_sales', 7.00, '2026-07-17', '2026-07-17 14:51:07.283617', '2026-07-17 14:51:07.283617', 2.00, 5.00);

-- ----------------------------
-- Table structure for fuel_batches
-- ----------------------------
DROP TABLE IF EXISTS `fuel_batches`;
CREATE TABLE `fuel_batches`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `fuel_id` int NOT NULL,
  `supplier_id` int NULL DEFAULT NULL,
  `tank_id` int NULL DEFAULT NULL,
  `branch_id` int NULL DEFAULT NULL,
  `fuel_delivery_id` int NULL DEFAULT NULL,
  `batch_no` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `cost_price_per_liter` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `selling_price_per_liter` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `received_liters` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `remaining_liters` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `received_date` datetime(6) NOT NULL,
  `expiry_date` datetime(6) NULL DEFAULT NULL,
  `remarks` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_fuel_batches_batch_no`(`batch_no` ASC) USING BTREE,
  INDEX `IX_fuel_batches_branch_id`(`branch_id` ASC) USING BTREE,
  INDEX `IX_fuel_batches_fuel_delivery_id`(`fuel_delivery_id` ASC) USING BTREE,
  INDEX `IX_fuel_batches_fuel_id`(`fuel_id` ASC) USING BTREE,
  INDEX `IX_fuel_batches_fuel_id_status_is_active_received_date_id`(`fuel_id` ASC, `status` ASC, `is_active` ASC, `received_date` ASC, `id` ASC) USING BTREE,
  INDEX `IX_fuel_batches_supplier_id`(`supplier_id` ASC) USING BTREE,
  INDEX `IX_fuel_batches_tank_id`(`tank_id` ASC) USING BTREE,
  CONSTRAINT `FK_fuel_batches_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_batches_fuel_deliveries_fuel_delivery_id` FOREIGN KEY (`fuel_delivery_id`) REFERENCES `fuel_deliveries` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_batches_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_batches_suppliers_supplier_id` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_batches_tanks_tank_id` FOREIGN KEY (`tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `CK_fuel_batches_cost_price_non_negative` CHECK (`cost_price_per_liter` >= 0),
  CONSTRAINT `CK_fuel_batches_received_liters_non_negative` CHECK (`received_liters` >= 0),
  CONSTRAINT `CK_fuel_batches_remaining_liters_non_negative` CHECK (`remaining_liters` >= 0),
  CONSTRAINT `CK_fuel_batches_selling_price_non_negative` CHECK (`selling_price_per_liter` >= 0)
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of fuel_batches
-- ----------------------------
INSERT INTO `fuel_batches` VALUES (1, 3, 1, 2, 3, 1, '00000001', 87.00, 99.00, 5.00, 5.00, '2026-07-09 07:23:30.748065', NULL, NULL, 1, 1, '2026-07-09 07:23:30.748065', '2026-07-09 07:23:30.748065');
INSERT INTO `fuel_batches` VALUES (2, 3, 1, 2, 3, 2, '00000002', 88.00, 99.00, 5.00, 5.00, '2026-07-09 00:00:00.000000', NULL, NULL, 1, 1, '2026-07-09 08:03:29.652956', '2026-07-09 08:03:29.652956');
INSERT INTO `fuel_batches` VALUES (3, 3, 2, 2, 3, 3, '00000003', 95.00, 75.00, 6.00, 6.00, '2026-07-14 00:00:00.000000', NULL, NULL, 1, 1, '2026-07-14 06:37:13.807321', '2026-07-14 06:37:13.807321');
INSERT INTO `fuel_batches` VALUES (4, 3, NULL, 7, 3, NULL, 'QA-FIFO-FUEL-001', 10.00, 30.00, 5.00, 0.00, '2026-01-01 00:00:00.000000', NULL, 'QA authoritative FIFO layer 1', 1, 1, '2026-07-17 10:46:17.000000', '2026-07-17 10:46:46.727862');
INSERT INTO `fuel_batches` VALUES (5, 3, NULL, 7, 3, NULL, 'QA-FIFO-FUEL-002', 20.00, 30.00, 10.00, 3.00, '2026-02-01 00:00:00.000000', NULL, 'QA authoritative FIFO layer 2', 1, 1, '2026-07-17 10:46:17.000000', '2026-07-17 10:47:27.904356');

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
  `branch_id` int NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_fuel_deliveries_delivery_no`(`delivery_no` ASC) USING BTREE,
  INDEX `IX_fuel_deliveries_supplier_id`(`supplier_id` ASC) USING BTREE,
  INDEX `IX_fuel_deliveries_fuel_id`(`fuel_id` ASC) USING BTREE,
  INDEX `IX_fuel_deliveries_tank_id`(`tank_id` ASC) USING BTREE,
  INDEX `IX_fuel_deliveries_branch_id`(`branch_id` ASC) USING BTREE,
  CONSTRAINT `FK_fuel_deliveries_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_deliveries_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_deliveries_suppliers_supplier_id` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_deliveries_tanks_tank_id` FOREIGN KEY (`tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of fuel_deliveries
-- ----------------------------
INSERT INTO `fuel_deliveries` VALUES (1, 'FR-20260709-0001', 1, 3, 2, 5.00, 87.00, 435.00, '2026-07-09 07:23:31', NULL, 1, '2026-07-09 07:23:31', '2026-07-09 07:23:31', 3);
INSERT INTO `fuel_deliveries` VALUES (2, 'FR-20260709-0002', 1, 3, 2, 5.00, 88.00, 440.00, '2026-07-09 00:00:00', NULL, 1, '2026-07-09 08:03:30', '2026-07-09 08:03:30', 3);
INSERT INTO `fuel_deliveries` VALUES (3, 'FR-20260714-0003', 2, 3, 2, 6.00, 95.00, 570.00, '2026-07-14 00:00:00', NULL, 1, '2026-07-14 06:37:14', '2026-07-14 06:37:14', 3);

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
  `branch_id` int NULL DEFAULT NULL,
  `reason` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_fuel_price_history_fuel_id`(`fuel_id` ASC) USING BTREE,
  INDEX `IX_fuel_price_history_branch_id`(`branch_id` ASC) USING BTREE,
  CONSTRAINT `FK_fuel_price_history_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_price_history_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 7 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of fuel_price_history
-- ----------------------------
INSERT INTO `fuel_price_history` VALUES (1, 1, 60.00, 75.00, '2026-06-19 00:00:00', NULL, NULL, '2026-06-19 05:09:24', 1, NULL, NULL, NULL);
INSERT INTO `fuel_price_history` VALUES (2, 2, 55.00, 75.00, '2026-06-19 00:00:00', NULL, NULL, '2026-06-19 05:09:34', 1, NULL, NULL, NULL);
INSERT INTO `fuel_price_history` VALUES (3, 3, 65.00, 75.00, '2026-06-19 00:00:00', NULL, NULL, '2026-06-19 05:09:44', 1, NULL, NULL, NULL);
INSERT INTO `fuel_price_history` VALUES (4, 1, 75.00, 95.00, '2026-07-06 00:00:00', NULL, NULL, '2026-07-06 03:03:02', 1, NULL, NULL, NULL);
INSERT INTO `fuel_price_history` VALUES (5, 2, 75.00, 100.00, '2026-07-13 00:00:00', 's', 1, '2026-07-13 07:06:59', 1, '2026-07-13 07:06:59.158750', 3, '2');
INSERT INTO `fuel_price_history` VALUES (6, 4, 99.00, 1.00, '2026-07-15 00:00:00', NULL, 1, '2026-07-14 17:08:09', 1, '2026-07-14 17:08:08.573529', 3, 'idk');

-- ----------------------------
-- Table structure for fuel_sale_batch_allocations
-- ----------------------------
DROP TABLE IF EXISTS `fuel_sale_batch_allocations`;
CREATE TABLE `fuel_sale_batch_allocations`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `fuel_sale_id` int NOT NULL,
  `fuel_batch_id` int NOT NULL,
  `tank_id` int NOT NULL,
  `fuel_id` int NOT NULL,
  `branch_id` int NOT NULL,
  `liters_allocated` decimal(18, 2) NOT NULL,
  `batch_liters_before` decimal(18, 2) NOT NULL,
  `batch_liters_after` decimal(18, 2) NOT NULL,
  `unit_cost_snapshot` decimal(18, 4) NOT NULL,
  `total_cost_snapshot` decimal(18, 2) NOT NULL,
  `unit_price_snapshot` decimal(18, 2) NOT NULL,
  `revenue_snapshot` decimal(18, 2) NOT NULL,
  `gross_profit_snapshot` decimal(18, 2) NOT NULL,
  `fuel_stock_movement_id` int NULL DEFAULT NULL,
  `created_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_fuel_sale_batch_allocations_fuel_sale_id_fuel_batch_id`(`fuel_sale_id` ASC, `fuel_batch_id` ASC) USING BTREE,
  UNIQUE INDEX `IX_fuel_sale_batch_allocations_fuel_stock_movement_id`(`fuel_stock_movement_id` ASC) USING BTREE,
  INDEX `IX_fuel_sale_batch_allocations_branch_id`(`branch_id` ASC) USING BTREE,
  INDEX `IX_fuel_sale_batch_allocations_fuel_batch_id`(`fuel_batch_id` ASC) USING BTREE,
  INDEX `IX_fuel_sale_batch_allocations_fuel_id`(`fuel_id` ASC) USING BTREE,
  INDEX `IX_fuel_sale_batch_allocations_fuel_sale_id`(`fuel_sale_id` ASC) USING BTREE,
  INDEX `IX_fuel_sale_batch_allocations_tank_id`(`tank_id` ASC) USING BTREE,
  CONSTRAINT `FK_fuel_sale_batch_allocations_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_sale_batch_allocations_fuel_batches_fuel_batch_id` FOREIGN KEY (`fuel_batch_id`) REFERENCES `fuel_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_sale_batch_allocations_fuel_sales_fuel_sale_id` FOREIGN KEY (`fuel_sale_id`) REFERENCES `fuel_sales` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_sale_batch_allocations_fuel_stock_movements_fuel_stock_~` FOREIGN KEY (`fuel_stock_movement_id`) REFERENCES `fuel_stock_movements` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_sale_batch_allocations_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_sale_batch_allocations_tanks_tank_id` FOREIGN KEY (`tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of fuel_sale_batch_allocations
-- ----------------------------
INSERT INTO `fuel_sale_batch_allocations` VALUES (1, 5, 4, 7, 3, 3, 5.00, 5.00, 0.00, 10.0000, 50.00, 75.00, 375.00, 325.00, 1, '2026-07-17 10:46:46.727862');
INSERT INTO `fuel_sale_batch_allocations` VALUES (2, 5, 5, 7, 3, 3, 2.00, 10.00, 8.00, 20.0000, 40.00, 75.00, 150.00, 110.00, 2, '2026-07-17 10:46:46.727862');
INSERT INTO `fuel_sale_batch_allocations` VALUES (3, 6, 5, 7, 3, 3, 5.00, 8.00, 3.00, 20.0000, 100.00, 75.00, 375.00, 275.00, 3, '2026-07-17 10:47:27.904356');

-- ----------------------------
-- Table structure for fuel_sales
-- ----------------------------
DROP TABLE IF EXISTS `fuel_sales`;
CREATE TABLE `fuel_sales`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `sale_id` int NOT NULL,
  `fuel_id` int NOT NULL,
  `tank_id` int NOT NULL,
  `nozzle_id` int NULL DEFAULT NULL,
  `liters` decimal(18, 2) NOT NULL,
  `price_per_liter` decimal(18, 2) NOT NULL,
  `subtotal` decimal(18, 2) NOT NULL,
  `tank_liters_before` decimal(18, 2) NULL DEFAULT NULL,
  `tank_liters_after` decimal(18, 2) NULL DEFAULT NULL,
  `status` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Completed',
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `dispenser_id` int NULL DEFAULT NULL,
  `pump_id` int NULL DEFAULT NULL,
  `gross_profit_snapshot` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `total_cost_snapshot` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `unit_cost_snapshot` decimal(18, 4) NOT NULL DEFAULT 0.0000,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `ix_fuel_sales_sale_id`(`sale_id` ASC) USING BTREE,
  INDEX `ix_fuel_sales_fuel_id`(`fuel_id` ASC) USING BTREE,
  INDEX `ix_fuel_sales_tank_id`(`tank_id` ASC) USING BTREE,
  INDEX `ix_fuel_sales_nozzle_id`(`nozzle_id` ASC) USING BTREE,
  INDEX `IX_fuel_sales_dispenser_id`(`dispenser_id` ASC) USING BTREE,
  INDEX `IX_fuel_sales_pump_id`(`pump_id` ASC) USING BTREE,
  CONSTRAINT `FK_fuel_sales_dispensers_dispenser_id` FOREIGN KEY (`dispenser_id`) REFERENCES `dispensers` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_fuel_sales_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_fuel_sales_nozzles_nozzle_id` FOREIGN KEY (`nozzle_id`) REFERENCES `nozzles` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_sales_pumps_pump_id` FOREIGN KEY (`pump_id`) REFERENCES `pumps` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_fuel_sales_sales_sale_id` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `fk_fuel_sales_tanks_tank_id` FOREIGN KEY (`tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 9 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of fuel_sales
-- ----------------------------
INSERT INTO `fuel_sales` VALUES (1, 4, 1, 3, NULL, 4.00, 75.00, 300.00, 1000.00, 996.00, 'Completed', '2026-06-25 13:46:48.655943', '2026-06-25 13:46:48.655974', NULL, NULL, 0.00, 0.00, 0.0000);
INSERT INTO `fuel_sales` VALUES (2, 5, 3, 2, NULL, 6.00, 75.00, 450.00, 1000.00, 994.00, 'Completed', '2026-06-25 14:20:11.232587', '2026-06-25 14:20:11.232632', NULL, NULL, 0.00, 0.00, 0.0000);
INSERT INTO `fuel_sales` VALUES (3, 49, 2, 1, NULL, 1.00, 1.00, 1.00, NULL, NULL, 'Completed', '2026-07-17 10:33:46.000000', '2026-07-17 10:33:46.000000', NULL, NULL, 0.00, 0.00, 0.0000);
INSERT INTO `fuel_sales` VALUES (4, 50, 2, 1, NULL, 1.00, 1.00, 1.00, NULL, NULL, 'Completed', '2026-07-17 10:33:46.000000', '2026-07-17 10:33:46.000000', NULL, NULL, 0.00, 0.00, 0.0000);
INSERT INTO `fuel_sales` VALUES (5, 52, 3, 7, 4, 7.00, 75.00, 525.00, 15.00, 8.00, 'Completed', '2026-07-17 10:46:47.031542', '2026-07-17 10:46:47.031596', 3, 4, 435.00, 90.00, 12.8571);
INSERT INTO `fuel_sales` VALUES (6, 53, 3, 7, 4, 5.00, 75.00, 375.00, 8.00, 3.00, 'Completed', '2026-07-17 10:47:27.915108', '2026-07-17 10:47:27.915111', 3, 4, 275.00, 100.00, 20.0000);

-- ----------------------------
-- Table structure for fuel_stock_movements
-- ----------------------------
DROP TABLE IF EXISTS `fuel_stock_movements`;
CREATE TABLE `fuel_stock_movements`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `tank_id` int NOT NULL,
  `fuel_id` int NOT NULL,
  `fuel_batch_id` int NOT NULL,
  `branch_id` int NOT NULL,
  `movement_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `liters_in` decimal(18, 2) NOT NULL,
  `liters_out` decimal(18, 2) NOT NULL,
  `batch_liters_before` decimal(18, 2) NOT NULL,
  `batch_liters_after` decimal(18, 2) NOT NULL,
  `tank_liters_before` decimal(18, 2) NOT NULL,
  `tank_liters_after` decimal(18, 2) NOT NULL,
  `unit_cost_snapshot` decimal(18, 4) NOT NULL,
  `reference_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `reference_id` int NOT NULL,
  `remarks` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `created_by_user_id` int NOT NULL,
  `created_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_fuel_stock_movements_branch_id`(`branch_id` ASC) USING BTREE,
  INDEX `IX_fuel_stock_movements_created_by_user_id`(`created_by_user_id` ASC) USING BTREE,
  INDEX `IX_fuel_stock_movements_fuel_batch_id`(`fuel_batch_id` ASC) USING BTREE,
  INDEX `IX_fuel_stock_movements_fuel_id`(`fuel_id` ASC) USING BTREE,
  INDEX `IX_fuel_stock_movements_reference_type_reference_id`(`reference_type` ASC, `reference_id` ASC) USING BTREE,
  INDEX `IX_fuel_stock_movements_tank_id`(`tank_id` ASC) USING BTREE,
  CONSTRAINT `FK_fuel_stock_movements_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_stock_movements_fuel_batches_fuel_batch_id` FOREIGN KEY (`fuel_batch_id`) REFERENCES `fuel_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_stock_movements_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_stock_movements_tanks_tank_id` FOREIGN KEY (`tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_fuel_stock_movements_users_created_by_user_id` FOREIGN KEY (`created_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of fuel_stock_movements
-- ----------------------------
INSERT INTO `fuel_stock_movements` VALUES (1, 7, 3, 4, 3, 'Sale', 0.00, 5.00, 5.00, 0.00, 15.00, 10.00, 10.0000, 'FuelSale', 5, 'FIFO Fuel Sale allocation for FuelSale 5', 1, '2026-07-17 10:46:46.727862');
INSERT INTO `fuel_stock_movements` VALUES (2, 7, 3, 5, 3, 'Sale', 0.00, 2.00, 10.00, 8.00, 10.00, 8.00, 20.0000, 'FuelSale', 5, 'FIFO Fuel Sale allocation for FuelSale 5', 1, '2026-07-17 10:46:46.727862');
INSERT INTO `fuel_stock_movements` VALUES (3, 7, 3, 5, 3, 'Sale', 0.00, 5.00, 8.00, 3.00, 8.00, 3.00, 20.0000, 'FuelSale', 6, 'FIFO Fuel Sale allocation for FuelSale 6', 1, '2026-07-17 10:47:27.904356');

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
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of fuels
-- ----------------------------
INSERT INTO `fuels` VALUES (1, 'gas1', 'g1', 95.00, '2026-06-17 15:00:21.609264', '2026-07-06 03:03:01.895621', 1, 1, 1);
INSERT INTO `fuels` VALUES (2, 'gas2', 'g2', 75.00, '2026-06-17 15:00:36.409137', '2026-06-19 05:13:15.816448', 1, 1, 1);
INSERT INTO `fuels` VALUES (3, 'gas3', 'g3', 75.00, '2026-06-17 15:00:47.641750', '2026-06-19 05:13:19.946547', 1, 2, 1);
INSERT INTO `fuels` VALUES (4, 'gas4', 'hhg', 99.00, '2026-07-14 13:53:20.399381', '2026-07-14 13:53:20.399381', 1, NULL, 1);

-- ----------------------------
-- Table structure for low_stock_settings
-- ----------------------------
DROP TABLE IF EXISTS `low_stock_settings`;
CREATE TABLE `low_stock_settings`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NULL DEFAULT NULL,
  `product_batch_id` int NULL DEFAULT NULL,
  `location` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `minimum_quantity` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime NULL DEFAULT NULL,
  `updated_at` datetime NULL DEFAULT NULL,
  `tank_id` int NULL DEFAULT NULL,
  `unit_label` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `branch_id` int NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_low_stock_settings_product_id`(`product_id` ASC) USING BTREE,
  INDEX `IX_low_stock_settings_product_batch_id`(`product_batch_id` ASC) USING BTREE,
  INDEX `IX_low_stock_settings_tank_id`(`tank_id` ASC) USING BTREE,
  INDEX `IX_low_stock_settings_branch_id`(`branch_id` ASC) USING BTREE,
  CONSTRAINT `FK_low_stock_settings_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_low_stock_settings_product_batches_product_batch_id` FOREIGN KEY (`product_batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_low_stock_settings_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_low_stock_settings_tanks_tank_id` FOREIGN KEY (`tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of low_stock_settings
-- ----------------------------
INSERT INTO `low_stock_settings` VALUES (1, 2, NULL, 'roever', 100.00, 0, '2026-06-19 02:42:30', '2026-06-23 06:56:44', NULL, NULL, NULL);
INSERT INTO `low_stock_settings` VALUES (2, NULL, NULL, 'Warehouse', 7676.00, 1, '2026-07-07 16:05:54', '2026-07-07 16:05:54', NULL, NULL, NULL);
INSERT INTO `low_stock_settings` VALUES (3, 4, NULL, 'Display', 100.00, 1, '2026-07-07 16:21:17', '2026-07-07 16:22:17', NULL, 'Units', NULL);

-- ----------------------------
-- Table structure for loyalty_settings
-- ----------------------------
DROP TABLE IF EXISTS `loyalty_settings`;
CREATE TABLE `loyalty_settings`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `setting_key` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `decimal_value` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_loyalty_settings_setting_key`(`setting_key` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of loyalty_settings
-- ----------------------------
INSERT INTO `loyalty_settings` VALUES (1, 'POINTS_EARN_RATE', 90.00, '2026-06-25 09:22:10.415662', '2026-06-25 09:22:10.415662');

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
  `earnings_id` int NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_members_member_no`(`member_no` ASC) USING BTREE,
  UNIQUE INDEX `IX_members_card_no`(`card_no` ASC) USING BTREE,
  INDEX `IX_members_discount_id`(`discount_id` ASC) USING BTREE,
  INDEX `IX_members_earnings_id`(`earnings_id` ASC) USING BTREE,
  CONSTRAINT `FK_members_discounts_discount_id` FOREIGN KEY (`discount_id`) REFERENCES `discounts` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_members_earnings_earnings_id` FOREIGN KEY (`earnings_id`) REFERENCES `earnings` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of members
-- ----------------------------
INSERT INTO `members` VALUES (1, '121212', '121212', 'hello', NULL, NULL, NULL, 5, 99996011.00, 1, '2026-06-17 17:01:12', '2026-06-29 23:42:52', 1);

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
  `tank_id` int NULL DEFAULT NULL,
  `name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `remarks` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_nozzles_pump_id`(`pump_id` ASC) USING BTREE,
  INDEX `IX_nozzles_tank_id`(`tank_id` ASC) USING BTREE,
  CONSTRAINT `FK_nozzles_pumps_pump_id` FOREIGN KEY (`pump_id`) REFERENCES `pumps` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_nozzles_tanks_tank_id` FOREIGN KEY (`tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of nozzles
-- ----------------------------
INSERT INTO `nozzles` VALUES (1, 1, 'saasd', 1, '2026-06-19 02:37:33', '2026-06-19 02:37:33', 1, NULL, NULL);
INSERT INTO `nozzles` VALUES (2, 2, '12345', 1, '2026-07-14 16:34:37', '2026-07-14 16:34:37', 5, 'nozzle1', NULL);
INSERT INTO `nozzles` VALUES (3, 3, 'hk', 1, '2026-07-14 17:11:05', '2026-07-14 17:11:05', 6, 'kjhjk', NULL);
INSERT INTO `nozzles` VALUES (4, 4, 'QA-FIFO-NOZZLE', 1, '2026-07-17 10:46:17', '2026-07-17 10:46:17', 7, 'QA FIFO Nozzle', 'QA Fuel FIFO');

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
-- Table structure for payments
-- ----------------------------
DROP TABLE IF EXISTS `payments`;
CREATE TABLE `payments`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `sale_id` int NOT NULL,
  `payment_method_id` int NULL DEFAULT NULL,
  `payment_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `amount` decimal(18, 2) NOT NULL,
  `reference_no` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `status` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Completed',
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `applied_amount` decimal(18, 2) NULL DEFAULT NULL,
  `change_amount` decimal(18, 2) NULL DEFAULT NULL,
  `tendered_amount` decimal(18, 2) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `ix_payments_sale_id`(`sale_id` ASC) USING BTREE,
  INDEX `ix_payments_payment_method_id`(`payment_method_id` ASC) USING BTREE,
  CONSTRAINT `fk_payments_payment_methods_payment_method_id` FOREIGN KEY (`payment_method_id`) REFERENCES `payment_methods` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_payments_sales_sale_id` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 55 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of payments
-- ----------------------------
INSERT INTO `payments` VALUES (1, 1, NULL, 'Cash', 101.00, NULL, 'Completed', '2026-06-25 09:13:42.073002', '2026-06-25 09:13:42.073002', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (2, 2, NULL, 'Cash', 222.00, NULL, 'Completed', '2026-06-25 09:13:57.789748', '2026-06-25 09:13:57.789748', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (3, 3, NULL, 'Cash', 10000.00, NULL, 'Completed', '2026-06-25 09:17:36.469548', '2026-06-25 09:17:36.469548', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (4, 4, NULL, 'Cash', 1000.00, NULL, 'Completed', '2026-06-25 13:46:48.467047', '2026-06-25 13:46:48.467047', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (5, 5, NULL, 'Cash', 10000.00, NULL, 'Completed', '2026-06-25 14:20:10.938068', '2026-06-25 14:20:10.938068', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (6, 6, NULL, 'Cash', 1000.00, NULL, 'Completed', '2026-06-25 14:20:41.249290', '2026-06-25 14:20:41.249290', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (7, 7, NULL, 'Cash', 300.00, NULL, 'Completed', '2026-06-25 14:53:15.902309', '2026-06-25 14:53:15.902309', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (8, 8, NULL, 'Cash', 1212.00, NULL, 'Completed', '2026-06-25 14:57:37.430304', '2026-06-25 14:57:37.430304', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (9, 9, NULL, 'Cash', 1000.00, NULL, 'Completed', '2026-06-25 15:16:50.796611', '2026-06-25 15:16:50.796611', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (10, 10, NULL, 'Cash', 1212.00, NULL, 'Completed', '2026-06-25 15:26:37.471895', '2026-06-25 15:26:37.471895', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (11, 11, NULL, 'Cash', 1212.00, NULL, 'Completed', '2026-06-25 15:30:05.653906', '2026-06-25 15:30:05.653906', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (12, 12, NULL, 'Cash', 121212.00, NULL, 'Completed', '2026-06-25 15:35:24.608723', '2026-06-25 15:35:24.608723', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (13, 13, NULL, 'Cash', 1212.00, NULL, 'Completed', '2026-06-25 15:44:04.447209', '2026-06-25 15:44:04.447209', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (14, 14, NULL, 'Cash', 1212.00, NULL, 'Completed', '2026-06-25 16:24:58.055301', '2026-06-25 16:24:58.055301', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (15, 15, NULL, 'Cash', 20.00, NULL, 'Completed', '2026-06-26 09:48:46.025957', '2026-06-26 09:48:46.025957', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (16, 16, NULL, 'Cash', 20.00, NULL, 'Completed', '2026-06-26 10:12:42.553359', '2026-06-26 10:12:42.553359', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (17, 17, NULL, 'Cash', 21.00, NULL, 'Completed', '2026-06-26 10:14:50.067953', '2026-06-26 10:14:50.067953', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (18, 18, NULL, 'Cash', 20.00, NULL, 'Completed', '2026-06-26 10:16:05.042093', '2026-06-26 10:16:05.042093', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (19, 19, NULL, 'Cash', 20.00, NULL, 'Completed', '2026-06-26 10:17:20.403685', '2026-06-26 10:17:20.403685', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (20, 20, NULL, 'Cash', 21.00, NULL, 'Completed', '2026-06-26 10:18:31.872711', '2026-06-26 10:18:31.872711', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (21, 21, NULL, 'Points', 100.00, NULL, 'Completed', '2026-06-26 15:48:22.085874', '2026-06-26 15:48:22.085874', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (22, 22, NULL, 'Cash', 100.00, NULL, 'Completed', '2026-06-26 16:33:59.167888', '2026-06-26 16:33:59.167888', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (23, 23, NULL, 'Cash', 1212.00, NULL, 'Completed', '2026-06-26 16:43:29.502134', '2026-06-26 16:43:29.502134', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (24, 24, NULL, 'Points', 200.00, NULL, 'Completed', '2026-06-26 16:43:56.327884', '2026-06-26 16:43:56.327884', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (25, 25, NULL, 'Cash', 1212.00, NULL, 'Completed', '2026-06-26 16:48:45.569774', '2026-06-26 16:48:45.569774', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (26, 26, NULL, 'Points', 200.00, NULL, 'Completed', '2026-06-29 15:25:10.433365', '2026-06-29 15:25:10.433365', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (27, 27, NULL, 'Cash', 300.00, NULL, 'Completed', '2026-06-29 15:37:52.707759', '2026-06-29 15:37:52.707759', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (28, 28, NULL, 'Cash', 23.00, NULL, 'Completed', '2026-06-29 15:38:11.957876', '2026-06-29 15:38:11.957876', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (29, 29, NULL, 'Cash', 999.00, NULL, 'Completed', '2026-06-29 23:04:02.182043', '2026-06-29 23:04:02.182043', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (30, 30, NULL, 'Cash', 9898.00, NULL, 'Completed', '2026-06-29 23:12:54.505869', '2026-06-29 23:12:54.505869', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (31, 31, NULL, 'Cash', 9898.00, NULL, 'Completed', '2026-06-29 23:13:41.424036', '2026-06-29 23:13:41.424036', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (32, 32, NULL, 'Cash', 9898.00, NULL, 'Completed', '2026-06-29 23:19:31.743993', '2026-06-29 23:19:31.743993', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (33, 33, NULL, 'Cash', 988.00, NULL, 'Completed', '2026-06-29 23:20:28.091583', '2026-06-29 23:20:28.091583', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (34, 34, NULL, 'Cash', 9898.00, NULL, 'Completed', '2026-06-29 23:21:10.100773', '2026-06-29 23:21:10.100773', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (35, 35, NULL, 'Cash', 2.00, NULL, 'Completed', '2026-06-29 23:25:30.713365', '2026-06-29 23:25:30.713365', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (36, 36, NULL, 'Cash', 9898.00, NULL, 'Completed', '2026-06-29 23:30:45.878283', '2026-06-29 23:30:45.878283', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (37, 37, NULL, 'Cash', 9898.00, NULL, 'Completed', '2026-06-29 23:40:19.650367', '2026-06-29 23:40:19.650367', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (38, 38, NULL, 'Cash', 9898.00, NULL, 'Completed', '2026-06-29 23:41:23.101794', '2026-06-29 23:41:23.101794', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (39, 39, NULL, 'Cash', 9898.00, NULL, 'Completed', '2026-06-29 23:42:13.246669', '2026-06-29 23:42:13.246669', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (40, 40, NULL, 'Cash', 99.00, NULL, 'Completed', '2026-06-29 23:42:51.821803', '2026-06-29 23:42:51.821803', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (41, 41, NULL, 'Cash', 200.00, NULL, 'Completed', '2026-06-30 09:07:28.806211', '2026-06-30 09:07:28.806211', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (42, 42, NULL, 'Cash', 12.00, NULL, 'Completed', '2026-06-30 09:44:47.770496', '2026-06-30 09:44:47.770496', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (43, 43, NULL, 'Cash', 0.00, NULL, 'Completed', '2026-07-07 01:32:11.475781', '2026-07-07 01:32:11.475781', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (44, 44, NULL, 'Cash', 9.00, NULL, 'Completed', '2026-07-16 00:27:55.572284', '2026-07-16 00:27:55.572284', NULL, NULL, NULL);
INSERT INTO `payments` VALUES (45, 45, NULL, 'Cash', 2.00, NULL, 'Completed', '2026-07-17 10:24:02.529759', '2026-07-17 10:24:02.529759', 2.00, 8.00, 10.00);
INSERT INTO `payments` VALUES (46, 46, NULL, 'Cash', 1.00, NULL, 'Completed', '2026-07-17 10:28:04.965851', '2026-07-17 10:28:04.965851', 1.00, 0.00, 1.00);
INSERT INTO `payments` VALUES (47, 47, NULL, 'Cash', 1.00, NULL, 'Completed', '2026-07-17 10:30:15.038194', '2026-07-17 10:30:15.038194', 1.00, 0.00, 1.00);
INSERT INTO `payments` VALUES (48, 48, NULL, 'Cash', 1.00, NULL, 'Completed', '2026-07-17 10:33:46.000000', '2026-07-17 10:33:46.000000', 1.00, 0.00, 1.00);
INSERT INTO `payments` VALUES (49, 49, NULL, 'Cash', 1.00, NULL, 'Completed', '2026-07-17 10:33:46.000000', '2026-07-17 10:33:46.000000', 1.00, 0.00, 1.00);
INSERT INTO `payments` VALUES (50, 50, NULL, 'Cash', 2.00, NULL, 'Completed', '2026-07-17 10:33:46.000000', '2026-07-17 10:33:46.000000', 2.00, 0.00, 2.00);
INSERT INTO `payments` VALUES (51, 51, NULL, 'Cash', 1.00, NULL, 'Completed', '2026-07-17 10:34:25.898355', '2026-07-17 10:34:25.898355', 1.00, 0.00, 1.00);
INSERT INTO `payments` VALUES (52, 52, NULL, 'Cash', 525.00, NULL, 'Completed', '2026-07-17 10:46:46.727862', '2026-07-17 10:46:46.727862', 525.00, 475.00, 1000.00);
INSERT INTO `payments` VALUES (53, 53, NULL, 'Cash', 375.00, NULL, 'Completed', '2026-07-17 10:47:27.904356', '2026-07-17 10:47:27.904356', 375.00, 125.00, 500.00);
INSERT INTO `payments` VALUES (54, 56, NULL, 'Cash', 2.00, NULL, 'Completed', '2026-07-17 21:51:06.323464', '2026-07-17 21:51:06.323464', 2.00, 97.00, 99.00);

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
) ENGINE = InnoDB AUTO_INCREMENT = 19 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

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
INSERT INTO `permissions` VALUES (8, 'Transaction Void Sale', 'tranvsale', NULL, NULL, NULL, 1);
INSERT INTO `permissions` VALUES (9, 'Create', 'tranvosale.create', 8, NULL, NULL, 1);
INSERT INTO `permissions` VALUES (10, 'Update', 'tranvsale.update', 8, NULL, NULL, 1);
INSERT INTO `permissions` VALUES (11, 'Delete ', 'tranvsale.delete', 8, NULL, NULL, 1);
INSERT INTO `permissions` VALUES (12, 'Print', 'tanvsale.print', 8, NULL, NULL, 1);
INSERT INTO `permissions` VALUES (16, 'Fuel Adjustment', 'tranfueladjust', NULL, '2026-07-17 06:59:21', '2026-07-17 06:59:21', 1);
INSERT INTO `permissions` VALUES (17, 'Prepare Fuel Reconciliation', 'tranfueladjust.reconcile', 16, '2026-07-17 06:59:21', '2026-07-17 06:59:21', 1);
INSERT INTO `permissions` VALUES (18, 'Approve Fuel Reconciliation', 'tranfueladjust.approve', 16, '2026-07-17 06:59:21', '2026-07-17 06:59:21', 1);

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
  `sale_id` int NULL DEFAULT NULL,
  `monetary_value_snapshot` decimal(18, 2) NULL DEFAULT NULL,
  `points_required_snapshot` decimal(18, 2) NULL DEFAULT NULL,
  `rule_id_snapshot` int NULL DEFAULT NULL,
  `rule_name_snapshot` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `reversed_points_ledger_id` int NULL DEFAULT NULL,
  `sale_void_id` int NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_points_ledger_reversed_points_ledger_id`(`reversed_points_ledger_id` ASC) USING BTREE,
  INDEX `IX_points_ledger_member_id`(`member_id` ASC) USING BTREE,
  INDEX `IX_points_ledger_sale_id`(`sale_id` ASC) USING BTREE,
  INDEX `IX_points_ledger_member_id_sale_id_transaction_type`(`member_id` ASC, `sale_id` ASC, `transaction_type` ASC) USING BTREE,
  INDEX `IX_points_ledger_sale_void_id`(`sale_void_id` ASC) USING BTREE,
  CONSTRAINT `FK_points_ledger_members_member_id` FOREIGN KEY (`member_id`) REFERENCES `members` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_points_ledger_points_ledger_reversed_points_ledger_id` FOREIGN KEY (`reversed_points_ledger_id`) REFERENCES `points_ledger` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_points_ledger_sale_voids_sale_void_id` FOREIGN KEY (`sale_void_id`) REFERENCES `sale_voids` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_points_ledger_sales_sale_id` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 20 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of points_ledger
-- ----------------------------
INSERT INTO `points_ledger` VALUES (1, 1, 'set', 1000.00, NULL, NULL, NULL, '2026-06-23 07:41:01', 0.00, 1000.00, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (2, 1, 'Earned', 18.00, 'POS', 14, 'Earned from POS sale', '2026-06-25 16:24:58', 1000.00, 1018.00, 14, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (3, 1, 'Earned', 2.00, 'POS', 20, 'Earned from POS sale', '2026-06-26 10:18:32', 1018.00, 1020.00, 20, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (4, 1, 'Earned', 2.00, 'POS', 15, 'Earned from POS sale backfill', '2026-06-26 10:20:29', 1020.00, 1022.00, 15, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (5, 1, 'Earned', 2.00, 'POS', 16, 'Earned from POS sale backfill', '2026-06-26 10:20:29', 1022.00, 1024.00, 16, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (6, 1, 'Earned', 2.00, 'POS', 17, 'Earned from POS sale backfill', '2026-06-26 10:20:29', 1024.00, 1026.00, 17, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (7, 1, 'Earned', 2.00, 'POS', 18, 'Earned from POS sale backfill', '2026-06-26 10:20:29', 1026.00, 1028.00, 18, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (8, 1, 'Earned', 2.00, 'POS', 19, 'Earned from POS sale backfill', '2026-06-26 10:20:29', 1028.00, 1030.00, 19, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (11, 1, 'Used', 1000.00, 'POS', 21, 'Used as points payment in POS', '2026-06-26 15:48:22', 1030.00, 30.00, 21, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (12, 1, 'set', 99999999.00, NULL, NULL, 'from admin', '2026-06-26 08:49:55', 30.00, 99999999.00, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (13, 1, 'Earned', 7.00, 'POS', 22, 'Earned from POS sale', '2026-06-26 16:33:59', 99999999.00, 100000006.00, 22, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (14, 1, 'Earned', 2.00, 'POS', 23, 'Earned from POS sale', '2026-06-26 16:43:30', 100000006.00, 100000008.00, 23, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (15, 1, 'Used', 2000.00, 'POS', 24, 'Used as points payment in POS', '2026-06-26 16:43:56', 100000008.00, 99998008.00, 24, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (16, 1, 'Used', 2000.00, 'POS', 26, 'Used as points payment in POS', '2026-06-29 15:25:10', 99998008.00, 99996008.00, 26, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (17, 1, 'Earned', 2.00, 'POS', 28, 'Earned from POS sale', '2026-06-29 15:38:12', 99996008.00, 99996010.00, 28, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (18, 1, 'Earned', 1.00, 'POS', 40, 'Earned from POS sale', '2026-06-29 23:42:52', 99996010.00, 99996011.00, 40, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `points_ledger` VALUES (19, 1, 'QAUnsupported', 1.00, 'Sale', 51, 'QA forced rollback after inventory compensation', '2026-07-17 10:34:34', 99996011.00, 99996011.00, 51, NULL, NULL, NULL, NULL, NULL, NULL);

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
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of product_batches
-- ----------------------------
INSERT INTO `product_batches` VALUES (1, 1, NULL, 'BATCH-000001', 7665.00, 896.00, NULL, 1, 1, '2026-06-18 04:02:06.529349', '2026-06-18 04:02:06.529349');
INSERT INTO `product_batches` VALUES (2, 2, NULL, 'BATCH-000002', 100.00, 130.00, NULL, 1, 1, '2026-06-18 06:37:43.782840', '2026-06-29 15:39:43.727583');
INSERT INTO `product_batches` VALUES (3, 3, NULL, 'BATCH-000003', 76.00, 100.00, NULL, 1, 1, '2026-06-29 15:40:46.495327', '2026-06-29 15:40:46.495327');
INSERT INTO `product_batches` VALUES (4, 4, NULL, 'BATCH-000004', 0.50, 1.00, NULL, 1, 1, '2026-06-29 16:24:45.052767', '2026-07-13 07:04:14.177882');
INSERT INTO `product_batches` VALUES (5, 4, 2, '70196410', 1.50, 1.00, NULL, 1, 1, '2026-06-30 15:34:54.936117', '2026-07-13 07:04:21.120195');
INSERT INTO `product_batches` VALUES (6, 4, 1, '54433473', 1.00, 1.00, NULL, 1, 1, '2026-06-30 15:35:34.202721', '2026-07-13 07:04:33.458630');
INSERT INTO `product_batches` VALUES (7, 1, 2, '70196411', 1.00, 2.00, NULL, 1, 1, '2026-07-09 08:29:11.997218', '2026-07-09 08:29:11.997218');
INSERT INTO `product_batches` VALUES (8, 1, 2, '70196412', 2.00, 3.00, NULL, 1, 1, '2026-07-09 08:30:25.437677', '2026-07-09 08:30:25.437677');
INSERT INTO `product_batches` VALUES (9, 4, NULL, 'QA-VOID-FIFO-20260717', 0.25, 1.00, NULL, 1, 1, '2020-01-01 00:00:00.000000', '2026-07-17 10:23:42.000000');

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
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of product_categories
-- ----------------------------
INSERT INTO `product_categories` VALUES (1, 'c1', 'g', 1, '2026-06-17 16:40:13.199221', '2026-06-17 16:40:13.199221', 1);
INSERT INTO `product_categories` VALUES (2, 'c2', 'asd', 1, '2026-06-29 15:41:10.107548', '2026-06-29 15:41:10.107548', 1);
INSERT INTO `product_categories` VALUES (3, 'c3', 'asd', 1, '2026-06-29 15:41:16.515698', '2026-06-29 15:41:16.515698', 1);

-- ----------------------------
-- Table structure for product_price_history
-- ----------------------------
DROP TABLE IF EXISTS `product_price_history`;
CREATE TABLE `product_price_history`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NOT NULL,
  `batch_id` int NULL DEFAULT NULL,
  `old_price` decimal(10, 2) NOT NULL,
  `new_price` decimal(10, 2) NOT NULL,
  `effective_date` datetime(6) NOT NULL,
  `remarks` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `created_by` int NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `branch_id` int NULL DEFAULT NULL,
  `reason` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_product_price_history_batch_id`(`batch_id` ASC) USING BTREE,
  INDEX `IX_product_price_history_created_by`(`created_by` ASC) USING BTREE,
  INDEX `IX_product_price_history_product_id`(`product_id` ASC) USING BTREE,
  INDEX `IX_product_price_history_branch_id`(`branch_id` ASC) USING BTREE,
  CONSTRAINT `FK_product_price_history_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_product_price_history_product_batches_batch_id` FOREIGN KEY (`batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_product_price_history_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_product_price_history_users_created_by` FOREIGN KEY (`created_by`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of product_price_history
-- ----------------------------
INSERT INTO `product_price_history` VALUES (1, 4, 4, 1.00, 2.00, '2026-07-13 00:00:00.000000', 'jj', 1, 1, '2026-07-13 07:02:33.704478', '2026-07-13 07:02:33.704478', 3, 'jj');
INSERT INTO `product_price_history` VALUES (2, 4, 4, 1.00, 3.00, '2026-07-13 00:00:00.000000', 'k', 1, 1, '2026-07-13 07:05:33.191847', '2026-07-13 07:05:33.191847', 3, 'k');

-- ----------------------------
-- Table structure for product_sales
-- ----------------------------
DROP TABLE IF EXISTS `product_sales`;
CREATE TABLE `product_sales`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `sale_id` int NOT NULL,
  `product_id` int NOT NULL,
  `batch_id` int NULL DEFAULT NULL,
  `quantity` decimal(18, 2) NOT NULL,
  `price` decimal(18, 2) NOT NULL,
  `subtotal` decimal(18, 2) NOT NULL,
  `display_stock_before` decimal(18, 2) NULL DEFAULT NULL,
  `display_stock_after` decimal(18, 2) NULL DEFAULT NULL,
  `status` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Completed',
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `display_stock_id` int NULL DEFAULT NULL,
  `gross_profit` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `unit_cost` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `unit_price` decimal(18, 2) NOT NULL DEFAULT 0.00,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `ix_product_sales_sale_id`(`sale_id` ASC) USING BTREE,
  INDEX `ix_product_sales_product_id`(`product_id` ASC) USING BTREE,
  INDEX `ix_product_sales_batch_id`(`batch_id` ASC) USING BTREE,
  INDEX `IX_product_sales_display_stock_id`(`display_stock_id` ASC) USING BTREE,
  CONSTRAINT `FK_product_sales_display_stocks_display_stock_id` FOREIGN KEY (`display_stock_id`) REFERENCES `display_stocks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_product_sales_product_batches_batch_id` FOREIGN KEY (`batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_product_sales_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_product_sales_sales_sale_id` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 58 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of product_sales
-- ----------------------------
INSERT INTO `product_sales` VALUES (1, 1, 2, 2, 1.00, 100.00, 100.00, 100.00, 99.00, 'Completed', '2026-06-25 09:13:42.086491', '2026-06-25 09:13:42.086539', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (2, 2, 2, 2, 2.00, 100.00, 200.00, 99.00, 97.00, 'Completed', '2026-06-25 09:13:57.792172', '2026-06-25 09:13:57.792176', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (3, 3, 2, 2, 1.00, 100.00, 100.00, 97.00, 96.00, 'Completed', '2026-06-25 09:17:36.474590', '2026-06-25 09:17:36.474592', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (4, 4, 2, 2, 4.00, 100.00, 400.00, 96.00, 92.00, 'Completed', '2026-06-25 13:46:48.512233', '2026-06-25 13:46:48.512521', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (5, 5, 2, 2, 1.00, 100.00, 100.00, 92.00, 91.00, 'Completed', '2026-06-25 14:20:10.980815', '2026-06-25 14:20:10.980869', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (6, 6, 2, 2, 2.00, 100.00, 200.00, 91.00, 89.00, 'Completed', '2026-06-25 14:20:41.251241', '2026-06-25 14:20:41.251243', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (7, 7, 2, 2, 2.00, 100.00, 200.00, 89.00, 87.00, 'Completed', '2026-06-25 14:53:15.924714', '2026-06-25 14:53:15.924783', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (8, 8, 2, 2, 1.00, 100.00, 100.00, 87.00, 86.00, 'Completed', '2026-06-25 14:57:37.433039', '2026-06-25 14:57:37.433057', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (9, 9, 2, 2, 1.00, 100.00, 100.00, 86.00, 85.00, 'Completed', '2026-06-25 15:16:50.835640', '2026-06-25 15:16:50.835690', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (10, 10, 2, 2, 2.00, 100.00, 200.00, 85.00, 83.00, 'Completed', '2026-06-25 15:26:37.487246', '2026-06-25 15:26:37.487321', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (11, 11, 2, 2, 2.00, 100.00, 200.00, 83.00, 81.00, 'Completed', '2026-06-25 15:30:05.656288', '2026-06-25 15:30:05.656289', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (12, 12, 2, 2, 3.00, 100.00, 300.00, 81.00, 78.00, 'Completed', '2026-06-25 15:35:24.633286', '2026-06-25 15:35:24.633336', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (13, 13, 2, 2, 1.00, 100.00, 100.00, 78.00, 77.00, 'Completed', '2026-06-25 15:44:04.451543', '2026-06-25 15:44:04.451546', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (14, 14, 2, 2, 2.00, 100.00, 200.00, 77.00, 75.00, 'Completed', '2026-06-25 16:24:58.074508', '2026-06-25 16:24:58.074568', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (15, 15, 2, 2, 2.00, 100.00, 200.00, 75.00, 73.00, 'Completed', '2026-06-26 09:48:46.080761', '2026-06-26 09:48:46.080916', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (16, 16, 2, 2, 2.00, 100.00, 200.00, 73.00, 71.00, 'Completed', '2026-06-26 10:12:42.572586', '2026-06-26 10:12:42.572702', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (17, 17, 2, 2, 2.00, 100.00, 200.00, 71.00, 69.00, 'Completed', '2026-06-26 10:14:50.069876', '2026-06-26 10:14:50.069878', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (18, 18, 2, 2, 2.00, 100.00, 200.00, 69.00, 67.00, 'Completed', '2026-06-26 10:16:05.043571', '2026-06-26 10:16:05.043571', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (19, 19, 2, 2, 2.00, 100.00, 200.00, 67.00, 65.00, 'Completed', '2026-06-26 10:17:20.405332', '2026-06-26 10:17:20.405333', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (20, 20, 2, 2, 2.00, 100.00, 200.00, 65.00, 63.00, 'Completed', '2026-06-26 10:18:31.873599', '2026-06-26 10:18:31.873600', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (21, 21, 2, 2, 1.00, 100.00, 100.00, 63.00, 62.00, 'Completed', '2026-06-26 15:48:22.129021', '2026-06-26 15:48:22.129095', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (22, 22, 2, 2, 7.00, 100.00, 700.00, 62.00, 55.00, 'Completed', '2026-06-26 16:33:59.193223', '2026-06-26 16:33:59.193260', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (23, 23, 2, 2, 2.00, 100.00, 200.00, 55.00, 53.00, 'Completed', '2026-06-26 16:43:29.537384', '2026-06-26 16:43:29.537912', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (24, 24, 2, 2, 2.00, 100.00, 200.00, 53.00, 51.00, 'Completed', '2026-06-26 16:43:56.333566', '2026-06-26 16:43:56.333569', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (25, 25, 2, 2, 2.00, 100.00, 200.00, 51.00, 49.00, 'Completed', '2026-06-26 16:48:45.580267', '2026-06-26 16:48:45.580299', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (26, 26, 2, 2, 2.00, 100.00, 200.00, 49.00, 47.00, 'Completed', '2026-06-29 15:25:10.443842', '2026-06-29 15:25:10.443887', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (27, 27, 2, 2, 1.00, 100.00, 100.00, 47.00, 46.00, 'Completed', '2026-06-29 15:37:52.726242', '2026-06-29 15:37:52.726303', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (28, 28, 2, 2, 2.00, 100.00, 200.00, 46.00, 44.00, 'Completed', '2026-06-29 15:38:11.960616', '2026-06-29 15:38:11.960618', NULL, 0.00, 100.00, 100.00);
INSERT INTO `product_sales` VALUES (29, 29, 3, 3, 2.00, 100.00, 200.00, 100.00, 98.00, 'Completed', '2026-06-29 23:04:02.198551', '2026-06-29 23:04:02.198604', NULL, 48.00, 76.00, 100.00);
INSERT INTO `product_sales` VALUES (30, 29, 2, 2, 4.00, 130.00, 520.00, 44.00, 40.00, 'Completed', '2026-06-29 23:04:02.203595', '2026-06-29 23:04:02.203597', NULL, 120.00, 100.00, 130.00);
INSERT INTO `product_sales` VALUES (31, 30, 2, 2, 2.00, 130.00, 260.00, 40.00, 38.00, 'Completed', '2026-06-29 23:12:54.526985', '2026-06-29 23:12:54.527093', NULL, 60.00, 100.00, 130.00);
INSERT INTO `product_sales` VALUES (32, 30, 3, 3, 2.00, 100.00, 200.00, 98.00, 96.00, 'Completed', '2026-06-29 23:12:54.530521', '2026-06-29 23:12:54.530523', NULL, 48.00, 76.00, 100.00);
INSERT INTO `product_sales` VALUES (33, 31, 2, 2, 2.00, 130.00, 260.00, 38.00, 36.00, 'Completed', '2026-06-29 23:13:41.425393', '2026-06-29 23:13:41.425395', NULL, 60.00, 100.00, 130.00);
INSERT INTO `product_sales` VALUES (34, 31, 3, 3, 3.00, 100.00, 300.00, 96.00, 93.00, 'Completed', '2026-06-29 23:13:41.426470', '2026-06-29 23:13:41.426473', NULL, 72.00, 76.00, 100.00);
INSERT INTO `product_sales` VALUES (35, 32, 3, 3, 3.00, 100.00, 300.00, 93.00, 90.00, 'Completed', '2026-06-29 23:19:31.769610', '2026-06-29 23:19:31.769650', NULL, 72.00, 76.00, 100.00);
INSERT INTO `product_sales` VALUES (36, 32, 2, 2, 2.00, 130.00, 260.00, 36.00, 34.00, 'Completed', '2026-06-29 23:19:31.772231', '2026-06-29 23:19:31.772234', NULL, 60.00, 100.00, 130.00);
INSERT INTO `product_sales` VALUES (37, 33, 3, 3, 2.00, 100.00, 200.00, 90.00, 88.00, 'Completed', '2026-06-29 23:20:28.093334', '2026-06-29 23:20:28.093337', NULL, 48.00, 76.00, 100.00);
INSERT INTO `product_sales` VALUES (38, 34, 2, 2, 2.00, 130.00, 260.00, 34.00, 32.00, 'Completed', '2026-06-29 23:21:10.102234', '2026-06-29 23:21:10.102237', NULL, 60.00, 100.00, 130.00);
INSERT INTO `product_sales` VALUES (39, 35, 4, 4, 1.00, 1.00, 1.00, 999.00, 998.00, 'Completed', '2026-06-29 23:25:30.716329', '2026-06-29 23:25:30.716331', NULL, 0.50, 0.50, 1.00);
INSERT INTO `product_sales` VALUES (40, 36, 4, 4, 2.00, 1.00, 2.00, 998.00, 996.00, 'Completed', '2026-06-29 23:30:45.905272', '2026-06-29 23:30:45.905323', NULL, 1.00, 0.50, 1.00);
INSERT INTO `product_sales` VALUES (41, 36, 3, 3, 2.00, 100.00, 200.00, 88.00, 86.00, 'Completed', '2026-06-29 23:30:45.908422', '2026-06-29 23:30:45.908425', NULL, 48.00, 76.00, 100.00);
INSERT INTO `product_sales` VALUES (42, 37, 4, 4, 2.00, 1.00, 2.00, 996.00, 994.00, 'Completed', '2026-06-29 23:40:19.672491', '2026-06-29 23:40:19.672581', NULL, 1.00, 0.50, 1.00);
INSERT INTO `product_sales` VALUES (43, 38, 4, 4, 2.00, 1.00, 2.00, 994.00, 992.00, 'Completed', '2026-06-29 23:41:23.105300', '2026-06-29 23:41:23.105304', NULL, 1.00, 0.50, 1.00);
INSERT INTO `product_sales` VALUES (44, 39, 4, 4, 2.00, 1.00, 2.00, 992.00, 990.00, 'Completed', '2026-06-29 23:42:13.247831', '2026-06-29 23:42:13.247834', NULL, 1.00, 0.50, 1.00);
INSERT INTO `product_sales` VALUES (45, 40, 3, 3, 1.00, 100.00, 100.00, 86.00, 85.00, 'Completed', '2026-06-29 23:42:51.824286', '2026-06-29 23:42:51.824302', NULL, 24.00, 76.00, 100.00);
INSERT INTO `product_sales` VALUES (46, 41, 3, 3, 2.00, 100.00, 200.00, 85.00, 83.00, 'Completed', '2026-06-30 09:07:28.808331', '2026-06-30 09:07:28.808332', NULL, 48.00, 76.00, 100.00);
INSERT INTO `product_sales` VALUES (47, 42, 4, 4, 2.00, 1.00, 2.00, 990.00, 988.00, 'Completed', '2026-06-30 09:44:47.790387', '2026-06-30 09:44:47.790418', NULL, 1.00, 0.50, 1.00);
INSERT INTO `product_sales` VALUES (48, 43, 3, 3, 2.00, 100.00, 200.00, 83.00, 81.00, 'Completed', '2026-07-07 01:32:11.697315', '2026-07-07 01:32:11.697445', 2, 48.00, 76.00, 100.00);
INSERT INTO `product_sales` VALUES (49, 44, 4, 4, 4.00, 1.00, 4.00, 888.00, 884.00, 'Completed', '2026-07-16 00:27:55.637780', '2026-07-16 00:27:55.637866', 3, 2.00, 0.50, 1.00);
INSERT INTO `product_sales` VALUES (50, 45, 4, 9, 1.00, 1.00, 1.00, 1.00, 0.00, 'Completed', '2026-07-17 10:24:04.209538', '2026-07-17 10:24:04.209599', 5, 0.75, 0.25, 1.00);
INSERT INTO `product_sales` VALUES (51, 45, 4, 4, 1.00, 1.00, 1.00, 177.00, 176.00, 'Completed', '2026-07-17 10:24:04.210881', '2026-07-17 10:24:04.210883', 4, 0.50, 0.50, 1.00);
INSERT INTO `product_sales` VALUES (52, 46, 4, 9, 1.00, 1.00, 1.00, 1.00, 0.00, 'Completed', '2026-07-17 10:28:05.228364', '2026-07-17 10:28:05.228535', 5, 0.75, 0.25, 1.00);
INSERT INTO `product_sales` VALUES (53, 47, 4, 9, 1.00, 1.00, 1.00, 1.00, 0.00, 'Completed', '2026-07-17 10:30:15.049788', '2026-07-17 10:30:15.049789', 5, 0.75, 0.25, 1.00);
INSERT INTO `product_sales` VALUES (54, 48, 4, 4, 1.00, 1.00, 1.00, NULL, NULL, 'Completed', '2026-07-17 10:33:46.000000', '2026-07-17 10:33:46.000000', NULL, 0.50, 0.50, 1.00);
INSERT INTO `product_sales` VALUES (55, 50, 4, 4, 1.00, 1.00, 1.00, NULL, NULL, 'Completed', '2026-07-17 10:33:46.000000', '2026-07-17 10:33:46.000000', 4, 0.50, 0.50, 1.00);
INSERT INTO `product_sales` VALUES (56, 51, 4, 9, 1.00, 1.00, 1.00, 1.00, 0.00, 'Completed', '2026-07-17 10:34:25.912972', '2026-07-17 10:34:25.913188', 5, 0.75, 0.25, 1.00);
INSERT INTO `product_sales` VALUES (57, 56, 4, 4, 2.00, 1.00, 2.00, 177.00, 175.00, 'Completed', '2026-07-17 21:51:06.441409', '2026-07-17 21:51:06.441529', 4, 1.00, 0.50, 1.00);

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
INSERT INTO `product_units` VALUES (1, 'pack', 'pack', 1, '2026-06-19 02:43:36', '2026-07-03 02:27:21');
INSERT INTO `product_units` VALUES (2, 'pack', 'pack', 1, '2026-06-19 02:43:46', '2026-07-03 02:27:34');

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
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of products
-- ----------------------------
INSERT INTO `products` VALUES (1, 'kjh', 1, '2026-06-18 04:02:06.529349', '2026-07-01 08:46:30.195710', 1, NULL, 1);
INSERT INTO `products` VALUES (2, 'asd', 1, '2026-06-18 06:37:43.782840', '2026-07-07 08:53:10.278938', 1, NULL, 1);
INSERT INTO `products` VALUES (3, 'qwe', 1, '2026-06-29 15:40:46.495327', '2026-07-07 08:53:11.399865', 1, NULL, 1);
INSERT INTO `products` VALUES (4, 'candy', 1, '2026-06-29 16:24:45.052767', '2026-07-06 23:57:46.597750', 3, NULL, 1);

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
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Meter Reading',
  `remarks` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_pump_meter_readings_pump_id`(`pump_id` ASC) USING BTREE,
  INDEX `IX_pump_meter_readings_nozzle_id`(`nozzle_id` ASC) USING BTREE,
  CONSTRAINT `FK_pump_meter_readings_nozzles_nozzle_id` FOREIGN KEY (`nozzle_id`) REFERENCES `nozzles` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_pump_meter_readings_pumps_pump_id` FOREIGN KEY (`pump_id`) REFERENCES `pumps` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of pump_meter_readings
-- ----------------------------
INSERT INTO `pump_meter_readings` VALUES (1, 1, 1, NULL, 100.00, 100.00, 0.00, '2026-06-19 00:00:00', 0, '2026-06-19 02:41:34', '2026-06-19 05:09:08', 'Meter Reading', NULL);

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
  `tank_id` int NULL DEFAULT NULL,
  `status` int NOT NULL DEFAULT 1,
  `branch_id` int NULL DEFAULT NULL,
  `dispenser_id` int NULL DEFAULT NULL,
  `remarks` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_pumps_tank_id`(`tank_id` ASC) USING BTREE,
  INDEX `IX_pumps_branch_id`(`branch_id` ASC) USING BTREE,
  INDEX `IX_pumps_dispenser_id`(`dispenser_id` ASC) USING BTREE,
  CONSTRAINT `FK_pumps_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_pumps_dispensers_dispenser_id` FOREIGN KEY (`dispenser_id`) REFERENCES `dispensers` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_pumps_tanks_tank_id` FOREIGN KEY (`tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of pumps
-- ----------------------------
INSERT INTO `pumps` VALUES (1, 'pump1', 'pump1', '2026-06-19 02:37:11.955469', '2026-06-19 02:37:11.955469', 1, 1, 3, NULL, NULL);
INSERT INTO `pumps` VALUES (2, '1234', 'pump1', '2026-07-14 16:34:37.125994', '2026-07-14 16:34:37.125994', 5, 1, 3, 1, NULL);
INSERT INTO `pumps` VALUES (3, 'kjhj', 'hkjh', '2026-07-14 17:11:04.580613', '2026-07-14 17:11:04.580613', 6, 1, 2, 2, 'jghg');
INSERT INTO `pumps` VALUES (4, 'QA-FIFO-PUMP', 'QA FIFO Pump', '2026-07-17 10:46:17.000000', '2026-07-17 10:46:17.000000', 7, 1, 3, 3, 'QA Fuel FIFO');

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
  `applies_to` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Both',
  `minimum_purchase` decimal(18, 2) NOT NULL DEFAULT 0.00,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of rebate_rules
-- ----------------------------
INSERT INTO `rebate_rules` VALUES (1, '100 to 1', 100.00, 1.00, 1, '2026-06-19 02:39:06', '2026-06-25 08:51:01', 'Both', 10.00);
INSERT INTO `rebate_rules` VALUES (2, 'hello', 10.00, 1.00, 1, '2026-06-26 14:14:49', '2026-06-26 14:14:49', 'Both', 1.00);

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
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of role_permissions
-- ----------------------------
INSERT INTO `role_permissions` VALUES (1, 1, 9, '2026-07-17 10:23:42', '2026-07-17 10:23:42', 1);

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
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of roles
-- ----------------------------
INSERT INTO `roles` VALUES (1, 'SupperAdmin', 'supperadmin', '2026-06-15 03:30:45', '2026-06-26 14:37:16', 1);
INSERT INTO `roles` VALUES (2, 'Admin', 'admin', '2026-06-23 06:49:35', '2026-06-26 14:37:37', 1);
INSERT INTO `roles` VALUES (3, 'Manager', 'manager', '2026-06-26 14:37:31', '2026-06-26 14:37:31', 1);
INSERT INTO `roles` VALUES (4, 'Accountant', 'accountant', '2026-06-26 14:38:01', '2026-06-26 14:38:01', 1);
INSERT INTO `roles` VALUES (5, 'Cashier', 'cashier', '2026-06-26 14:38:08', '2026-06-26 14:38:08', 1);

-- ----------------------------
-- Table structure for sale_discount_applications
-- ----------------------------
DROP TABLE IF EXISTS `sale_discount_applications`;
CREATE TABLE `sale_discount_applications`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `sale_id` int NOT NULL,
  `source_type` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `rule_id` int NULL DEFAULT NULL,
  `rule_name_snapshot` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `calculation_type_snapshot` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `rate_or_value_snapshot` decimal(18, 2) NULL DEFAULT NULL,
  `eligible_base_snapshot` decimal(18, 2) NOT NULL,
  `applied_amount` decimal(18, 2) NOT NULL,
  `created_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_sale_discount_applications_sale_id`(`sale_id` ASC) USING BTREE,
  CONSTRAINT `FK_sale_discount_applications_sales_sale_id` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sale_discount_applications
-- ----------------------------

-- ----------------------------
-- Table structure for sale_items
-- ----------------------------
DROP TABLE IF EXISTS `sale_items`;
CREATE TABLE `sale_items`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `sale_id` int NOT NULL,
  `item_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `product_id` int NULL DEFAULT NULL,
  `fuel_id` int NULL DEFAULT NULL,
  `tank_id` int NULL DEFAULT NULL,
  `nozzle_id` int NULL DEFAULT NULL,
  `batch_id` int NULL DEFAULT NULL,
  `quantity` decimal(18, 2) NULL DEFAULT NULL,
  `liters` decimal(18, 2) NULL DEFAULT NULL,
  `price` decimal(18, 2) NOT NULL,
  `subtotal` decimal(18, 2) NOT NULL,
  `status` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Completed',
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `ix_sale_items_sale_id`(`sale_id` ASC) USING BTREE,
  INDEX `ix_sale_items_item_type`(`item_type` ASC) USING BTREE,
  INDEX `ix_sale_items_product_id`(`product_id` ASC) USING BTREE,
  INDEX `ix_sale_items_fuel_id`(`fuel_id` ASC) USING BTREE,
  INDEX `ix_sale_items_tank_id`(`tank_id` ASC) USING BTREE,
  INDEX `ix_sale_items_nozzle_id`(`nozzle_id` ASC) USING BTREE,
  INDEX `ix_sale_items_batch_id`(`batch_id` ASC) USING BTREE,
  CONSTRAINT `fk_sale_items_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_sale_items_nozzles_nozzle_id` FOREIGN KEY (`nozzle_id`) REFERENCES `nozzles` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_sale_items_product_batches_batch_id` FOREIGN KEY (`batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_sale_items_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_sale_items_sales_sale_id` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `fk_sale_items_tanks_tank_id` FOREIGN KEY (`tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sale_items
-- ----------------------------

-- ----------------------------
-- Table structure for sale_void_cash_adjustments
-- ----------------------------
DROP TABLE IF EXISTS `sale_void_cash_adjustments`;
CREATE TABLE `sale_void_cash_adjustments`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `sale_void_id` int NOT NULL,
  `sale_id` int NOT NULL,
  `daily_cash_id` int NOT NULL,
  `branch_id` int NOT NULL,
  `user_id` int NOT NULL,
  `business_date` datetime(6) NOT NULL,
  `amount` decimal(18, 2) NOT NULL,
  `reason` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `created_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_sale_void_cash_adjustments_sale_void_id`(`sale_void_id` ASC) USING BTREE,
  INDEX `IX_sale_void_cash_adjustments_branch_id`(`branch_id` ASC) USING BTREE,
  INDEX `IX_sale_void_cash_adjustments_daily_cash_id`(`daily_cash_id` ASC) USING BTREE,
  INDEX `IX_sale_void_cash_adjustments_sale_id`(`sale_id` ASC) USING BTREE,
  INDEX `IX_sale_void_cash_adjustments_user_id`(`user_id` ASC) USING BTREE,
  CONSTRAINT `FK_sale_void_cash_adjustments_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sale_void_cash_adjustments_daily_cash_daily_cash_id` FOREIGN KEY (`daily_cash_id`) REFERENCES `daily_cash` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sale_void_cash_adjustments_sale_voids_sale_void_id` FOREIGN KEY (`sale_void_id`) REFERENCES `sale_voids` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sale_void_cash_adjustments_sales_sale_id` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sale_void_cash_adjustments_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sale_void_cash_adjustments
-- ----------------------------
INSERT INTO `sale_void_cash_adjustments` VALUES (1, 1, 45, 3, 3, 1, '2026-07-17 00:00:00.000000', -2.00, 'QA runtime acceptance overpayment and multi-batch FIFO', '2026-07-17 03:24:43.483675');
INSERT INTO `sale_void_cash_adjustments` VALUES (2, 2, 46, 3, 3, 1, '2026-07-17 00:00:00.000000', -1.00, 'QA verify Daily Cash single compensation', '2026-07-17 03:28:07.568950');
INSERT INTO `sale_void_cash_adjustments` VALUES (3, 5, 47, 3, 3, 1, '2026-07-17 00:00:00.000000', -1.00, 'QA concurrent race A', '2026-07-17 03:30:44.285840');
INSERT INTO `sale_void_cash_adjustments` VALUES (4, 7, 56, 3, 3, 1, '2026-07-17 00:00:00.000000', -2.00, 'vnvnbvbv', '2026-07-17 16:53:18.568575');

-- ----------------------------
-- Table structure for sale_void_payments
-- ----------------------------
DROP TABLE IF EXISTS `sale_void_payments`;
CREATE TABLE `sale_void_payments`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `sale_void_id` int NOT NULL,
  `payment_id` int NOT NULL,
  `payment_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `original_applied_amount` decimal(18, 2) NOT NULL,
  `reversed_amount` decimal(18, 2) NOT NULL,
  `tendered_amount_snapshot` decimal(18, 2) NULL DEFAULT NULL,
  `change_amount_snapshot` decimal(18, 2) NULL DEFAULT NULL,
  `reference_no_snapshot` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `external_refund_status` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `created_by_user_id` int NOT NULL,
  `created_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_sale_void_payments_payment_id`(`payment_id` ASC) USING BTREE,
  INDEX `IX_sale_void_payments_created_by_user_id`(`created_by_user_id` ASC) USING BTREE,
  INDEX `IX_sale_void_payments_sale_void_id`(`sale_void_id` ASC) USING BTREE,
  CONSTRAINT `FK_sale_void_payments_payments_payment_id` FOREIGN KEY (`payment_id`) REFERENCES `payments` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sale_void_payments_sale_voids_sale_void_id` FOREIGN KEY (`sale_void_id`) REFERENCES `sale_voids` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sale_void_payments_users_created_by_user_id` FOREIGN KEY (`created_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sale_void_payments
-- ----------------------------
INSERT INTO `sale_void_payments` VALUES (1, 1, 45, 'Cash', 2.00, -2.00, 10.00, 8.00, NULL, 'Not Applicable', 1, '2026-07-17 03:24:43.483675');
INSERT INTO `sale_void_payments` VALUES (2, 2, 46, 'Cash', 1.00, -1.00, 1.00, 0.00, NULL, 'Not Applicable', 1, '2026-07-17 03:28:07.568950');
INSERT INTO `sale_void_payments` VALUES (3, 5, 47, 'Cash', 1.00, -1.00, 1.00, 0.00, NULL, 'Not Applicable', 1, '2026-07-17 03:30:44.285840');
INSERT INTO `sale_void_payments` VALUES (4, 7, 54, 'Cash', 2.00, -2.00, 99.00, 97.00, NULL, 'Not Applicable', 1, '2026-07-17 16:53:18.568575');

-- ----------------------------
-- Table structure for sale_void_product_items
-- ----------------------------
DROP TABLE IF EXISTS `sale_void_product_items`;
CREATE TABLE `sale_void_product_items`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `sale_void_id` int NOT NULL,
  `product_sale_id` int NOT NULL,
  `product_id` int NOT NULL,
  `display_stock_id` int NOT NULL,
  `batch_id` int NOT NULL,
  `quantity_restored` decimal(18, 2) NOT NULL,
  `unit_cost_snapshot` decimal(18, 2) NOT NULL,
  `unit_price_snapshot` decimal(18, 2) NOT NULL,
  `restored_value` decimal(18, 2) NOT NULL,
  `before_quantity` decimal(18, 2) NOT NULL,
  `after_quantity` decimal(18, 2) NOT NULL,
  `stock_movement_id` int NULL DEFAULT NULL,
  `created_at` datetime(6) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_sale_void_product_items_product_sale_id`(`product_sale_id` ASC) USING BTREE,
  UNIQUE INDEX `IX_sale_void_product_items_stock_movement_id`(`stock_movement_id` ASC) USING BTREE,
  INDEX `IX_sale_void_product_items_batch_id`(`batch_id` ASC) USING BTREE,
  INDEX `IX_sale_void_product_items_display_stock_id`(`display_stock_id` ASC) USING BTREE,
  INDEX `IX_sale_void_product_items_product_id`(`product_id` ASC) USING BTREE,
  INDEX `IX_sale_void_product_items_sale_void_id`(`sale_void_id` ASC) USING BTREE,
  CONSTRAINT `FK_sale_void_product_items_display_stocks_display_stock_id` FOREIGN KEY (`display_stock_id`) REFERENCES `display_stocks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sale_void_product_items_product_batches_batch_id` FOREIGN KEY (`batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sale_void_product_items_product_sales_product_sale_id` FOREIGN KEY (`product_sale_id`) REFERENCES `product_sales` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sale_void_product_items_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sale_void_product_items_sale_voids_sale_void_id` FOREIGN KEY (`sale_void_id`) REFERENCES `sale_voids` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sale_void_product_items_stock_movements_stock_movement_id` FOREIGN KEY (`stock_movement_id`) REFERENCES `stock_movements` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sale_void_product_items
-- ----------------------------
INSERT INTO `sale_void_product_items` VALUES (1, 1, 51, 4, 4, 4, 1.00, 0.50, 1.00, 0.50, 176.00, 177.00, 57, '2026-07-17 03:24:43.483675');
INSERT INTO `sale_void_product_items` VALUES (2, 1, 50, 4, 5, 9, 1.00, 0.25, 1.00, 0.25, 0.00, 1.00, 58, '2026-07-17 03:24:43.483675');
INSERT INTO `sale_void_product_items` VALUES (3, 2, 52, 4, 5, 9, 1.00, 0.25, 1.00, 0.25, 0.00, 1.00, 60, '2026-07-17 03:28:07.568950');
INSERT INTO `sale_void_product_items` VALUES (4, 5, 53, 4, 5, 9, 1.00, 0.25, 1.00, 0.25, 0.00, 1.00, 62, '2026-07-17 03:30:44.285840');
INSERT INTO `sale_void_product_items` VALUES (5, 7, 57, 4, 4, 4, 2.00, 0.50, 1.00, 1.00, 175.00, 177.00, 66, '2026-07-17 16:53:18.568575');

-- ----------------------------
-- Table structure for sale_voids
-- ----------------------------
DROP TABLE IF EXISTS `sale_voids`;
CREATE TABLE `sale_voids`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `sale_id` int NOT NULL,
  `branch_id` int NOT NULL,
  `original_daily_cash_id` int NULL DEFAULT NULL,
  `daily_cash_id` int NOT NULL,
  `requested_by_user_id` int NOT NULL,
  `reason_code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `reason` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `original_receipt_no` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `original_business_date` datetime(6) NULL DEFAULT NULL,
  `void_business_date` datetime(6) NOT NULL,
  `original_gross_total` decimal(18, 2) NOT NULL,
  `original_discount_amount` decimal(18, 2) NOT NULL,
  `original_rebate_amount` decimal(18, 2) NOT NULL,
  `original_vat_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `original_vat_rate` decimal(18, 4) NULL DEFAULT NULL,
  `original_taxable_amount` decimal(18, 2) NOT NULL,
  `original_vat_amount` decimal(18, 2) NOT NULL,
  `reversed_vat_amount` decimal(18, 2) NOT NULL,
  `original_net_total` decimal(18, 2) NOT NULL,
  `original_applied_payment_amount` decimal(18, 2) NOT NULL,
  `status` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `created_at` datetime(6) NOT NULL,
  `completed_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_sale_voids_sale_id`(`sale_id` ASC) USING BTREE,
  INDEX `IX_sale_voids_branch_id`(`branch_id` ASC) USING BTREE,
  INDEX `IX_sale_voids_daily_cash_id`(`daily_cash_id` ASC) USING BTREE,
  INDEX `IX_sale_voids_original_daily_cash_id`(`original_daily_cash_id` ASC) USING BTREE,
  INDEX `IX_sale_voids_requested_by_user_id`(`requested_by_user_id` ASC) USING BTREE,
  CONSTRAINT `FK_sale_voids_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sale_voids_daily_cash_daily_cash_id` FOREIGN KEY (`daily_cash_id`) REFERENCES `daily_cash` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sale_voids_daily_cash_original_daily_cash_id` FOREIGN KEY (`original_daily_cash_id`) REFERENCES `daily_cash` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sale_voids_sales_sale_id` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sale_voids_users_requested_by_user_id` FOREIGN KEY (`requested_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 8 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sale_voids
-- ----------------------------
INSERT INTO `sale_voids` VALUES (1, 45, 3, 3, 3, 1, NULL, 'QA runtime acceptance overpayment and multi-batch FIFO', 'POS-20260717-000001', '2026-07-17 00:00:00.000000', '2026-07-17 00:00:00.000000', 2.00, 0.00, 0.00, 'Inclusive', 13.0000, 2.00, 0.23, 0.23, 2.00, 2.00, 'Completed', '2026-07-17 03:24:43.483675', '2026-07-17 03:24:43.483675');
INSERT INTO `sale_voids` VALUES (2, 46, 3, 3, 3, 1, NULL, 'QA verify Daily Cash single compensation', 'POS-20260717-000002', '2026-07-17 00:00:00.000000', '2026-07-17 00:00:00.000000', 1.00, 0.00, 0.00, 'Inclusive', 13.0000, 1.00, 0.12, 0.12, 1.00, 1.00, 'Completed', '2026-07-17 03:28:07.568950', '2026-07-17 03:28:07.568950');
INSERT INTO `sale_voids` VALUES (5, 47, 3, 3, 3, 1, NULL, 'QA concurrent race A', 'POS-20260717-000003', '2026-07-17 00:00:00.000000', '2026-07-17 00:00:00.000000', 1.00, 0.00, 0.00, 'Inclusive', 13.0000, 1.00, 0.12, 0.12, 1.00, 1.00, 'Completed', '2026-07-17 03:30:44.285840', '2026-07-17 03:30:44.285840');
INSERT INTO `sale_voids` VALUES (7, 56, 3, 3, 3, 1, NULL, 'vnvnbvbv', 'POS-20260717-000010', '2026-07-17 00:00:00.000000', '2026-07-17 00:00:00.000000', 2.00, 0.00, 0.00, 'Inclusive', 13.0000, 2.00, 0.23, 0.23, 2.00, 2.00, 'Completed', '2026-07-17 16:53:18.568575', '2026-07-17 16:53:18.568575');

-- ----------------------------
-- Table structure for sales
-- ----------------------------
DROP TABLE IF EXISTS `sales`;
CREATE TABLE `sales`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `receipt_no` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `user_id` int NOT NULL,
  `member_id` int NULL DEFAULT NULL,
  `gross_total` decimal(18, 2) NOT NULL,
  `discount_amount` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `rebate_amount` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `net_total` decimal(18, 2) NOT NULL,
  `cash_amount` decimal(18, 2) NOT NULL DEFAULT 0.00,
  `status` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Completed',
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `branch_id` int NULL DEFAULT NULL,
  `daily_cash_id` int NULL DEFAULT NULL,
  `business_date` date NULL DEFAULT NULL,
  `change_amount` decimal(18, 2) NULL DEFAULT NULL,
  `completed_at` datetime(6) NULL DEFAULT NULL,
  `shift_id` int NULL DEFAULT NULL,
  `taxable_amount` decimal(18, 2) NULL DEFAULT NULL,
  `vat_amount` decimal(18, 2) NULL DEFAULT NULL,
  `vat_exempt_amount` decimal(18, 2) NULL DEFAULT NULL,
  `vat_name_snapshot` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `vat_rate` decimal(10, 2) NULL DEFAULT NULL,
  `vat_setting_id` int NULL DEFAULT NULL,
  `vat_type_snapshot` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `ak_sales_receipt_no`(`receipt_no` ASC) USING BTREE,
  INDEX `ix_sales_user_id`(`user_id` ASC) USING BTREE,
  INDEX `ix_sales_member_id`(`member_id` ASC) USING BTREE,
  INDEX `IX_sales_branch_id`(`branch_id` ASC) USING BTREE,
  INDEX `IX_sales_daily_cash_id`(`daily_cash_id` ASC) USING BTREE,
  INDEX `IX_sales_shift_id`(`shift_id` ASC) USING BTREE,
  INDEX `IX_sales_vat_setting_id`(`vat_setting_id` ASC) USING BTREE,
  CONSTRAINT `FK_sales_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sales_daily_cash_daily_cash_id` FOREIGN KEY (`daily_cash_id`) REFERENCES `daily_cash` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_sales_members_member_id` FOREIGN KEY (`member_id`) REFERENCES `members` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sales_shift_settings_shift_id` FOREIGN KEY (`shift_id`) REFERENCES `shift_settings` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_sales_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_sales_vat_settings_vat_setting_id` FOREIGN KEY (`vat_setting_id`) REFERENCES `vat_settings` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 57 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sales
-- ----------------------------
INSERT INTO `sales` VALUES (1, 'POS-20260625-000001', 1, NULL, 100.00, 0.00, 0.00, 100.00, 101.00, 'Completed', '2026-06-25 09:13:42.073002', '2026-06-25 09:13:42.073002', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (2, 'POS-20260625-000002', 1, NULL, 200.00, 0.00, 0.00, 200.00, 222.00, 'Completed', '2026-06-25 09:13:57.789748', '2026-06-25 09:13:57.789748', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (3, 'POS-20260625-000003', 1, NULL, 100.00, 0.00, 0.00, 100.00, 10000.00, 'Completed', '2026-06-25 09:17:36.469548', '2026-06-25 09:17:36.469548', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (4, 'POS-20260625-000004', 1, NULL, 700.00, 0.00, 0.00, 700.00, 1000.00, 'Completed', '2026-06-25 13:46:48.467047', '2026-06-25 13:46:48.467047', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (5, 'POS-20260625-000005', 1, NULL, 550.00, 0.00, 0.00, 550.00, 10000.00, 'Completed', '2026-06-25 14:20:10.938068', '2026-06-25 14:20:10.938068', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (6, 'POS-20260625-000006', 1, 1, 200.00, 0.00, 0.00, 200.00, 1000.00, 'Completed', '2026-06-25 14:20:41.249290', '2026-06-25 14:20:41.249290', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (7, 'POS-20260625-000007', 1, 1, 200.00, 0.00, 0.00, 200.00, 300.00, 'Completed', '2026-06-25 14:53:15.902309', '2026-06-25 14:53:15.902309', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (8, 'POS-20260625-000008', 1, 1, 100.00, 0.00, 0.00, 100.00, 1212.00, 'Completed', '2026-06-25 14:57:37.430304', '2026-06-25 14:57:37.430304', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (9, 'POS-20260625-000009', 1, 1, 100.00, 0.00, 0.00, 100.00, 1000.00, 'Completed', '2026-06-25 15:16:50.796611', '2026-06-25 15:16:50.796611', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (10, 'POS-20260625-000010', 1, 1, 200.00, 0.00, 0.00, 200.00, 1212.00, 'Completed', '2026-06-25 15:26:37.471895', '2026-06-25 15:26:37.471895', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (11, 'POS-20260625-000011', 1, 1, 200.00, 180.00, 0.00, 20.00, 1212.00, 'Completed', '2026-06-25 15:30:05.653906', '2026-06-25 15:30:05.653906', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (12, 'POS-20260625-000012', 1, NULL, 300.00, 0.00, 0.00, 300.00, 121212.00, 'Completed', '2026-06-25 15:35:24.608723', '2026-06-25 15:35:24.608723', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (13, 'POS-20260625-000013', 1, 1, 100.00, 90.00, 0.00, 10.00, 1212.00, 'Completed', '2026-06-25 15:44:04.447209', '2026-06-25 15:44:04.447209', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (14, 'POS-20260625-000014', 1, 1, 200.00, 180.00, 0.00, 20.00, 1212.00, 'Completed', '2026-06-25 16:24:58.055301', '2026-06-25 16:24:58.055301', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (15, 'POS-20260626-000001', 1, 1, 200.00, 180.00, 0.00, 20.00, 20.00, 'Completed', '2026-06-26 09:48:46.025957', '2026-06-26 09:48:46.025957', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (16, 'POS-20260626-000002', 1, 1, 200.00, 180.00, 0.00, 20.00, 20.00, 'Completed', '2026-06-26 10:12:42.553359', '2026-06-26 10:12:42.553359', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (17, 'POS-20260626-000003', 1, 1, 200.00, 180.00, 0.00, 20.00, 21.00, 'Completed', '2026-06-26 10:14:50.067953', '2026-06-26 10:14:50.067953', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (18, 'POS-20260626-000004', 1, 1, 200.00, 180.00, 0.00, 20.00, 20.00, 'Completed', '2026-06-26 10:16:05.042093', '2026-06-26 10:16:05.042093', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (19, 'POS-20260626-000005', 1, 1, 200.00, 180.00, 0.00, 20.00, 20.00, 'Completed', '2026-06-26 10:17:20.403685', '2026-06-26 10:17:20.403685', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (20, 'POS-20260626-000006', 1, 1, 200.00, 180.00, 0.00, 20.00, 21.00, 'Completed', '2026-06-26 10:18:31.872711', '2026-06-26 10:18:31.872711', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (21, 'POS-20260626-000007', 1, 1, 100.00, 0.00, 100.00, 0.00, 0.00, 'Completed', '2026-06-26 15:48:22.085874', '2026-06-26 15:48:22.085874', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (22, 'POS-20260626-000008', 1, 1, 700.00, 630.00, 0.00, 70.00, 100.00, 'Completed', '2026-06-26 16:33:59.167888', '2026-06-26 16:33:59.167888', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (23, 'POS-20260626-000009', 1, 1, 200.00, 180.00, 0.00, 20.00, 1212.00, 'Completed', '2026-06-26 16:43:29.502134', '2026-06-26 16:43:29.502134', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (24, 'POS-20260626-000010', 1, 1, 200.00, 0.00, 200.00, 0.00, 0.00, 'Completed', '2026-06-26 16:43:56.327884', '2026-06-26 16:43:56.327884', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (25, 'POS-20260626-000011', 1, NULL, 200.00, 0.00, 0.00, 200.00, 1212.00, 'Completed', '2026-06-26 16:48:45.569774', '2026-06-26 16:48:45.569774', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (26, 'POS-20260629-000001', 1, 1, 200.00, 0.00, 200.00, 0.00, 0.00, 'Completed', '2026-06-29 15:25:10.433365', '2026-06-29 15:25:10.433365', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (27, 'POS-20260629-000002', 1, NULL, 100.00, 0.00, 0.00, 100.00, 300.00, 'Completed', '2026-06-29 15:37:52.707759', '2026-06-29 15:37:52.707759', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (28, 'POS-20260629-000003', 1, 1, 200.00, 180.00, 0.00, 20.00, 23.00, 'Completed', '2026-06-29 15:38:11.957876', '2026-06-29 15:38:11.957876', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (29, 'POS-20260629-000004', 1, NULL, 720.00, 0.00, 0.00, 720.00, 999.00, 'Completed', '2026-06-29 23:04:02.182043', '2026-06-29 23:04:02.182043', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (30, 'POS-20260629-000005', 1, NULL, 460.00, 0.00, 0.00, 460.00, 9898.00, 'Completed', '2026-06-29 23:12:54.505869', '2026-06-29 23:12:54.505869', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (31, 'POS-20260629-000006', 1, NULL, 560.00, 0.00, 0.00, 560.00, 9898.00, 'Completed', '2026-06-29 23:13:41.424036', '2026-06-29 23:13:41.424036', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (32, 'POS-20260629-000007', 1, NULL, 560.00, 0.00, 0.00, 560.00, 9898.00, 'Completed', '2026-06-29 23:19:31.743993', '2026-06-29 23:19:31.743993', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (33, 'POS-20260629-000008', 1, NULL, 200.00, 0.00, 0.00, 200.00, 988.00, 'Completed', '2026-06-29 23:20:28.091583', '2026-06-29 23:20:28.091583', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (34, 'POS-20260629-000009', 1, NULL, 260.00, 0.00, 0.00, 260.00, 9898.00, 'Completed', '2026-06-29 23:21:10.100773', '2026-06-29 23:21:10.100773', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (35, 'POS-20260629-000010', 1, NULL, 1.00, 0.00, 0.00, 1.00, 2.00, 'Completed', '2026-06-29 23:25:30.713365', '2026-06-29 23:25:30.713365', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (36, 'POS-20260629-000011', 1, NULL, 202.00, 0.00, 0.00, 202.00, 9898.00, 'Completed', '2026-06-29 23:30:45.878283', '2026-06-29 23:30:45.878283', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (37, 'POS-20260629-000012', 1, NULL, 2.00, 0.00, 0.00, 2.00, 9898.00, 'Completed', '2026-06-29 23:40:19.650367', '2026-06-29 23:40:19.650367', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (38, 'POS-20260629-000013', 1, NULL, 2.00, 0.00, 0.00, 2.00, 9898.00, 'Completed', '2026-06-29 23:41:23.101794', '2026-06-29 23:41:23.101794', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (39, 'POS-20260629-000014', 1, NULL, 2.00, 0.00, 0.00, 2.00, 9898.00, 'Completed', '2026-06-29 23:42:13.246669', '2026-06-29 23:42:13.246669', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (40, 'POS-20260629-000015', 1, 1, 100.00, 90.00, 0.00, 10.00, 99.00, 'Completed', '2026-06-29 23:42:51.821803', '2026-06-29 23:42:51.821803', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (41, 'POS-20260630-000001', 1, NULL, 200.00, 100.00, 0.00, 100.00, 200.00, 'Completed', '2026-06-30 09:07:28.806211', '2026-06-30 09:07:28.806211', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (42, 'POS-20260630-000002', 1, NULL, 2.00, 0.00, 0.00, 2.00, 12.00, 'Completed', '2026-06-30 09:44:47.770496', '2026-06-30 09:44:47.770496', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (43, 'POS-20260707-000001', 1, 1, 200.00, 280.00, 0.00, 0.00, 0.00, 'Completed', '2026-07-07 01:32:11.475781', '2026-07-07 01:32:11.475781', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (44, 'POS-20260716-000001', 1, NULL, 4.00, 0.00, 0.00, 4.00, 9.00, 'Completed', '2026-07-16 00:27:55.572284', '2026-07-16 00:27:55.572284', 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `sales` VALUES (45, 'POS-20260717-000001', 1, NULL, 2.00, 0.00, 0.00, 2.00, 10.00, 'Voided', '2026-07-17 10:24:02.529759', '2026-07-17 03:24:43.483675', 3, 3, '2026-07-17', 8.00, '2026-07-17 10:24:02.529759', 2, 2.00, 0.23, 0.00, 'VAT 13%', 13.00, 1, 'Inclusive');
INSERT INTO `sales` VALUES (46, 'POS-20260717-000002', 1, NULL, 1.00, 0.00, 0.00, 1.00, 1.00, 'Voided', '2026-07-17 10:28:04.965851', '2026-07-17 03:28:07.568950', 3, 3, '2026-07-17', 0.00, '2026-07-17 10:28:04.965851', 2, 1.00, 0.12, 0.00, 'VAT 13%', 13.00, 1, 'Inclusive');
INSERT INTO `sales` VALUES (47, 'POS-20260717-000003', 1, NULL, 1.00, 0.00, 0.00, 1.00, 1.00, 'Voided', '2026-07-17 10:30:15.038194', '2026-07-17 03:30:44.285840', 3, 3, '2026-07-17', 0.00, '2026-07-17 10:30:15.038194', 2, 1.00, 0.12, 0.00, 'VAT 13%', 13.00, 1, 'Inclusive');
INSERT INTO `sales` VALUES (48, 'QA-VOID-LEGACY-001', 1, NULL, 1.00, 0.00, 0.00, 1.00, 1.00, 'Completed', '2026-07-17 10:33:46.000000', '2026-07-17 10:33:46.000000', 3, 3, '2026-07-17', 0.00, '2026-07-17 10:33:46.000000', 2, 1.00, 0.12, 0.00, 'VAT 13%', 13.00, NULL, 'Inclusive');
INSERT INTO `sales` VALUES (49, 'QA-VOID-FUEL-001', 1, NULL, 1.00, 0.00, 0.00, 1.00, 1.00, 'Completed', '2026-07-17 10:33:46.000000', '2026-07-17 10:33:46.000000', 3, 3, '2026-07-17', 0.00, '2026-07-17 10:33:46.000000', 2, 1.00, 0.12, 0.00, 'VAT 13%', 13.00, NULL, 'Inclusive');
INSERT INTO `sales` VALUES (50, 'QA-VOID-MIXED-001', 1, NULL, 2.00, 0.00, 0.00, 2.00, 2.00, 'Completed', '2026-07-17 10:33:46.000000', '2026-07-17 10:33:46.000000', 3, 3, '2026-07-17', 0.00, '2026-07-17 10:33:46.000000', 2, 2.00, 0.23, 0.00, 'VAT 13%', 13.00, NULL, 'Inclusive');
INSERT INTO `sales` VALUES (51, 'POS-20260717-000007', 1, NULL, 1.00, 0.00, 0.00, 1.00, 1.00, 'Completed', '2026-07-17 10:34:25.898355', '2026-07-17 10:34:25.898355', 3, 3, '2026-07-17', 0.00, '2026-07-17 10:34:25.898355', 2, 1.00, 0.12, 0.00, 'VAT 13%', 13.00, 1, 'Inclusive');
INSERT INTO `sales` VALUES (52, 'POS-20260717-000008', 1, NULL, 525.00, 0.00, 0.00, 525.00, 1000.00, 'Completed', '2026-07-17 10:46:46.727862', '2026-07-17 10:46:46.727862', 3, 3, '2026-07-17', 475.00, '2026-07-17 10:46:46.727862', 2, 525.00, 60.40, 0.00, 'VAT 13%', 13.00, 1, 'Inclusive');
INSERT INTO `sales` VALUES (53, 'POS-20260717-000009', 1, NULL, 375.00, 0.00, 0.00, 375.00, 500.00, 'Completed', '2026-07-17 10:47:27.904356', '2026-07-17 10:47:27.904356', 3, 3, '2026-07-17', 125.00, '2026-07-17 10:47:27.904356', 2, 375.00, 43.14, 0.00, 'VAT 13%', 13.00, 1, 'Inclusive');
INSERT INTO `sales` VALUES (56, 'POS-20260717-000010', 1, NULL, 2.00, 0.00, 0.00, 2.00, 99.00, 'Voided', '2026-07-17 21:51:06.323464', '2026-07-17 16:53:18.568575', 3, 3, '2026-07-17', 97.00, '2026-07-17 21:51:06.323464', 2, 2.00, 0.23, 0.00, 'VAT 13%', 13.00, 1, 'Inclusive');

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
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

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
  `opening_cash_amount` decimal(18, 2) NULL DEFAULT NULL,
  `remarks` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of shift_settings
-- ----------------------------
INSERT INTO `shift_settings` VALUES (1, 'Morning', '06:00:00', '10:00:00', 1, 1, 1, 0, 1, '2026-07-10 02:48:06', '2026-07-10 02:49:38', 0.00, NULL);
INSERT INTO `shift_settings` VALUES (2, 'afternoon', '14:00:00', '18:00:00', 1, 1, 1, 0, 1, '2026-07-10 02:49:24', '2026-07-10 02:50:47', 0.00, NULL);

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
-- Table structure for stock_adjustments
-- ----------------------------
DROP TABLE IF EXISTS `stock_adjustments`;
CREATE TABLE `stock_adjustments`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `adjustment_no` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `scope` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `business_date` datetime(6) NOT NULL,
  `branch_id` int NOT NULL,
  `warehouse_stock_id` int NULL DEFAULT NULL,
  `display_stock_id` int NULL DEFAULT NULL,
  `tank_id` int NULL DEFAULT NULL,
  `product_id` int NULL DEFAULT NULL,
  `batch_id` int NULL DEFAULT NULL,
  `fuel_id` int NULL DEFAULT NULL,
  `adjustment_type` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `before_quantity` decimal(18, 3) NOT NULL,
  `adjustment_quantity` decimal(18, 3) NOT NULL,
  `signed_quantity` decimal(18, 3) NOT NULL,
  `after_quantity` decimal(18, 3) NOT NULL,
  `reason` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `remarks` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `status` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `adjusted_by` int NOT NULL,
  `created_at` datetime(6) NOT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `posted_by` int NULL DEFAULT NULL,
  `posted_at` datetime(6) NULL DEFAULT NULL,
  `cancelled_by` int NULL DEFAULT NULL,
  `cancelled_at` datetime(6) NULL DEFAULT NULL,
  `reversal_of_adjustment_id` int NULL DEFAULT NULL,
  `reversed_by_adjustment_id` int NULL DEFAULT NULL,
  `approved_at` datetime(6) NULL DEFAULT NULL,
  `approved_by` int NULL DEFAULT NULL,
  `cost_input_mode` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `created_fuel_batch_id` int NULL DEFAULT NULL,
  `evidence_reference` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `total_cost` decimal(18, 2) NULL DEFAULT NULL,
  `unit_cost` decimal(18, 2) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_stock_adjustments_adjustment_no`(`adjustment_no` ASC) USING BTREE,
  UNIQUE INDEX `IX_stock_adjustments_created_fuel_batch_id`(`created_fuel_batch_id` ASC) USING BTREE,
  INDEX `IX_stock_adjustments_adjusted_by`(`adjusted_by` ASC) USING BTREE,
  INDEX `IX_stock_adjustments_batch_id`(`batch_id` ASC) USING BTREE,
  INDEX `IX_stock_adjustments_branch_id`(`branch_id` ASC) USING BTREE,
  INDEX `IX_stock_adjustments_display_stock_id`(`display_stock_id` ASC) USING BTREE,
  INDEX `IX_stock_adjustments_fuel_id`(`fuel_id` ASC) USING BTREE,
  INDEX `IX_stock_adjustments_posted_by`(`posted_by` ASC) USING BTREE,
  INDEX `IX_stock_adjustments_product_id`(`product_id` ASC) USING BTREE,
  INDEX `IX_stock_adjustments_reversal_of_adjustment_id`(`reversal_of_adjustment_id` ASC) USING BTREE,
  INDEX `IX_stock_adjustments_scope_branch_id_business_date_status`(`scope` ASC, `branch_id` ASC, `business_date` ASC, `status` ASC) USING BTREE,
  INDEX `IX_stock_adjustments_tank_id`(`tank_id` ASC) USING BTREE,
  INDEX `IX_stock_adjustments_warehouse_stock_id`(`warehouse_stock_id` ASC) USING BTREE,
  INDEX `IX_stock_adjustments_approved_by`(`approved_by` ASC) USING BTREE,
  CONSTRAINT `FK_stock_adjustments_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_adjustments_display_stocks_display_stock_id` FOREIGN KEY (`display_stock_id`) REFERENCES `display_stocks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_adjustments_fuel_batches_created_fuel_batch_id` FOREIGN KEY (`created_fuel_batch_id`) REFERENCES `fuel_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_adjustments_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_adjustments_product_batches_batch_id` FOREIGN KEY (`batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_adjustments_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_adjustments_stock_adjustments_reversal_of_adjustment_id` FOREIGN KEY (`reversal_of_adjustment_id`) REFERENCES `stock_adjustments` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_adjustments_tanks_tank_id` FOREIGN KEY (`tank_id`) REFERENCES `tanks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_adjustments_users_adjusted_by` FOREIGN KEY (`adjusted_by`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_adjustments_users_approved_by` FOREIGN KEY (`approved_by`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_adjustments_users_posted_by` FOREIGN KEY (`posted_by`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_adjustments_warehouse_stocks_warehouse_stock_id` FOREIGN KEY (`warehouse_stock_id`) REFERENCES `warehouse_stocks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock_adjustments
-- ----------------------------
INSERT INTO `stock_adjustments` VALUES (1, 'DS-20260719-000002-ADJ', 'Warehouse', '2026-07-19 00:00:00.000000', 3, 6, NULL, NULL, 1, 8, NULL, 'Decrease', 10.000, 10.000, -10.000, 0.000, 'Daily Stock reconciliation DS-20260719-000002', NULL, 'Posted', 1, '2026-07-19 15:30:53.430756', '2026-07-19 15:30:53.430756', 1, '2026-07-19 15:30:53.430756', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO `stock_adjustments` VALUES (2, 'DS-20260719-000001-ADJ', 'Warehouse', '2026-07-19 00:00:00.000000', 3, 3, NULL, NULL, 4, 6, NULL, 'Decrease', 10.000, 10.000, -10.000, 0.000, 'Daily Stock reconciliation DS-20260719-000001', NULL, 'Posted', 1, '2026-07-19 15:45:09.456197', '2026-07-19 15:45:09.456197', 1, '2026-07-19 15:45:09.456197', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

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
  `branch_id` int NULL DEFAULT NULL,
  `display_stock_id` int NULL DEFAULT NULL,
  `before_quantity` decimal(18, 2) NULL DEFAULT NULL,
  `after_quantity` decimal(18, 2) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_stock_movements_product_id`(`product_id` ASC) USING BTREE,
  INDEX `IX_stock_movements_product_batch_id`(`product_batch_id` ASC) USING BTREE,
  INDEX `IX_stock_movements_branch_id`(`branch_id` ASC) USING BTREE,
  INDEX `IX_stock_movements_display_stock_id`(`display_stock_id` ASC) USING BTREE,
  CONSTRAINT `FK_stock_movements_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_movements_display_stocks_display_stock_id` FOREIGN KEY (`display_stock_id`) REFERENCES `display_stocks` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_movements_product_batches_product_batch_id` FOREIGN KEY (`product_batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_movements_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 69 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock_movements
-- ----------------------------
INSERT INTO `stock_movements` VALUES (48, 4, 5, 'Supplier', 'Warehouse', 'Receiving', 99.00, 'ProductReceiving', 1, NULL, 1, '2026-06-30 15:34:55', NULL, NULL, NULL, NULL);
INSERT INTO `stock_movements` VALUES (49, 4, 6, 'Supplier', 'Warehouse', 'Receiving', 10.00, 'ProductReceiving', 2, NULL, 1, '2026-06-30 15:35:34', NULL, NULL, NULL, NULL);
INSERT INTO `stock_movements` VALUES (50, 4, 4, 'Display', 'Warehouse', 'Transfer', 100.00, 'DisplayToWarehouseProduct', 1, NULL, 1, '2026-07-01 12:15:33', NULL, NULL, NULL, NULL);
INSERT INTO `stock_movements` VALUES (51, 3, 3, 'Display', NULL, 'Sale', 2.00, 'POS', 43, 'POS FIFO sale for qwe', 1, '2026-07-07 01:32:12', NULL, NULL, NULL, NULL);
INSERT INTO `stock_movements` VALUES (52, 1, 7, 'Supplier', 'Warehouse', 'Receiving', 10.00, 'ProductReceiving', 3, 'jhg', 1, '2026-07-09 08:29:12', NULL, NULL, NULL, NULL);
INSERT INTO `stock_movements` VALUES (53, 1, 8, 'Supplier', 'Warehouse', 'Receiving', 10.00, 'ProductReceiving', 4, NULL, 1, '2026-07-09 08:30:25', NULL, NULL, NULL, NULL);
INSERT INTO `stock_movements` VALUES (54, 4, 4, 'Display', NULL, 'Sale', 4.00, 'POS', 44, 'POS FIFO sale for candy', 1, '2026-07-16 00:27:56', NULL, NULL, NULL, NULL);
INSERT INTO `stock_movements` VALUES (55, 4, 9, 'Display', NULL, 'Sale', 1.00, 'ProductSale', 50, 'POS FIFO sale for candy', 1, '2026-07-17 10:24:04', 3, 5, 1.00, 0.00);
INSERT INTO `stock_movements` VALUES (56, 4, 4, 'Display', NULL, 'Sale', 1.00, 'ProductSale', 51, 'POS FIFO sale for candy', 1, '2026-07-17 10:24:04', 3, 4, 177.00, 176.00);
INSERT INTO `stock_movements` VALUES (57, 4, 4, 'SaleVoid', 'Display', 'SaleVoid', 1.00, 'SaleVoid', 1, 'Void of receipt POS-20260717-000001', 1, '2026-07-17 03:24:43', 3, 4, 176.00, 177.00);
INSERT INTO `stock_movements` VALUES (58, 4, 9, 'SaleVoid', 'Display', 'SaleVoid', 1.00, 'SaleVoid', 1, 'Void of receipt POS-20260717-000001', 1, '2026-07-17 03:24:43', 3, 5, 0.00, 1.00);
INSERT INTO `stock_movements` VALUES (59, 4, 9, 'Display', NULL, 'Sale', 1.00, 'ProductSale', 52, 'POS FIFO sale for candy', 1, '2026-07-17 10:28:05', 3, 5, 1.00, 0.00);
INSERT INTO `stock_movements` VALUES (60, 4, 9, 'SaleVoid', 'Display', 'SaleVoid', 1.00, 'SaleVoid', 2, 'Void of receipt POS-20260717-000002', 1, '2026-07-17 03:28:08', 3, 5, 0.00, 1.00);
INSERT INTO `stock_movements` VALUES (61, 4, 9, 'Display', NULL, 'Sale', 1.00, 'ProductSale', 53, 'POS FIFO sale for candy', 1, '2026-07-17 10:30:15', 3, 5, 1.00, 0.00);
INSERT INTO `stock_movements` VALUES (62, 4, 9, 'SaleVoid', 'Display', 'SaleVoid', 1.00, 'SaleVoid', 5, 'Void of receipt POS-20260717-000003', 1, '2026-07-17 03:30:44', 3, 5, 0.00, 1.00);
INSERT INTO `stock_movements` VALUES (63, 4, 9, 'Display', NULL, 'Sale', 1.00, 'ProductSale', 56, 'POS FIFO sale for candy', 1, '2026-07-17 10:34:26', 3, 5, 1.00, 0.00);
INSERT INTO `stock_movements` VALUES (65, 4, 4, 'Display', NULL, 'Sale', 2.00, 'ProductSale', 57, 'POS FIFO sale for candy', 1, '2026-07-17 21:51:06', 3, 4, 177.00, 175.00);
INSERT INTO `stock_movements` VALUES (66, 4, 4, 'SaleVoid', 'Display', 'SaleVoid', 2.00, 'SaleVoid', 7, 'Void of receipt POS-20260717-000010', 1, '2026-07-17 16:53:19', 3, 4, 175.00, 177.00);
INSERT INTO `stock_movements` VALUES (67, 1, 8, 'Warehouse', 'Warehouse', 'DailyStockDecrease', 10.00, 'DailyStockRecord', 3, 'Daily Stock reconciliation DS-20260719-000002', 1, '2026-07-19 15:30:53', 3, NULL, 10.00, 0.00);
INSERT INTO `stock_movements` VALUES (68, 4, 6, 'Warehouse', 'Warehouse', 'DailyStockDecrease', 10.00, 'DailyStockRecord', 2, 'Daily Stock reconciliation DS-20260719-000001', 1, '2026-07-19 15:45:09', 3, NULL, 10.00, 0.00);

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
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock_receiving_items
-- ----------------------------
INSERT INTO `stock_receiving_items` VALUES (1, 1, 4, 5, 99.00, 1.50, 2.50, NULL, 148.50, '2026-06-30 15:34:55', '2026-06-30 15:34:55');
INSERT INTO `stock_receiving_items` VALUES (2, 2, 4, 6, 10.00, 1.00, 10.00, NULL, 10.00, '2026-06-30 15:35:34', '2026-06-30 15:35:34');
INSERT INTO `stock_receiving_items` VALUES (3, 3, 1, 7, 10.00, 1.00, 2.00, NULL, 10.00, '2026-07-09 08:29:12', '2026-07-09 08:29:12');
INSERT INTO `stock_receiving_items` VALUES (4, 4, 1, 8, 10.00, 2.00, 3.00, NULL, 20.00, '2026-07-09 08:30:25', '2026-07-09 08:30:25');

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
  `branch_id` int NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_stock_receivings_receiving_no`(`receiving_no` ASC) USING BTREE,
  INDEX `IX_stock_receivings_supplier_id`(`supplier_id` ASC) USING BTREE,
  INDEX `IX_stock_receivings_branch_id`(`branch_id` ASC) USING BTREE,
  CONSTRAINT `FK_stock_receivings_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_stock_receivings_suppliers_supplier_id` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock_receivings
-- ----------------------------
INSERT INTO `stock_receivings` VALUES (1, 'PR-20260630-0001', 2, '2026-06-30 15:34:55', 148.50, NULL, 1, '2026-06-30 15:34:55', '2026-06-30 15:34:55', NULL);
INSERT INTO `stock_receivings` VALUES (2, 'PR-20260630-0002', 1, '2026-06-30 15:35:34', 10.00, NULL, 1, '2026-06-30 15:35:34', '2026-06-30 15:35:34', NULL);
INSERT INTO `stock_receivings` VALUES (3, 'PR-20260709-0003', 2, '2026-07-09 00:00:00', 10.00, 'jhg', 1, '2026-07-09 08:29:12', '2026-07-09 08:29:12', 3);
INSERT INTO `stock_receivings` VALUES (4, 'PR-20260709-0004', 2, '2026-07-09 00:00:00', 20.00, NULL, 1, '2026-07-09 08:30:25', '2026-07-09 08:30:25', 3);

-- ----------------------------
-- Table structure for stock_transfer_items
-- ----------------------------
DROP TABLE IF EXISTS `stock_transfer_items`;
CREATE TABLE `stock_transfer_items`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `stock_transfer_id` int NOT NULL,
  `product_id` int NULL DEFAULT NULL,
  `batch_id` int NULL DEFAULT NULL,
  `fuel_id` int NULL DEFAULT NULL,
  `source_tank_id` int NULL DEFAULT NULL,
  `destination_tank_id` int NULL DEFAULT NULL,
  `quantity` decimal(18, 2) NULL DEFAULT NULL,
  `liters` decimal(18, 2) NULL DEFAULT NULL,
  `source_before` decimal(18, 2) NULL DEFAULT NULL,
  `source_after` decimal(18, 2) NULL DEFAULT NULL,
  `destination_before` decimal(18, 2) NULL DEFAULT NULL,
  `destination_after` decimal(18, 2) NULL DEFAULT NULL,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_stock_transfer_items_stock_transfer_id`(`stock_transfer_id` ASC) USING BTREE,
  INDEX `IX_stock_transfer_items_product_id`(`product_id` ASC) USING BTREE,
  INDEX `IX_stock_transfer_items_batch_id`(`batch_id` ASC) USING BTREE,
  INDEX `IX_stock_transfer_items_fuel_id`(`fuel_id` ASC) USING BTREE,
  INDEX `IX_stock_transfer_items_source_tank_id`(`source_tank_id` ASC) USING BTREE,
  INDEX `IX_stock_transfer_items_destination_tank_id`(`destination_tank_id` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock_transfer_items
-- ----------------------------
INSERT INTO `stock_transfer_items` VALUES (1, 1, 4, 4, NULL, NULL, NULL, 100.00, NULL, 988.00, 888.00, 0.00, 100.00, '2026-07-01 12:15:33.152011', '2026-07-01 12:15:33.152011');

-- ----------------------------
-- Table structure for stock_transfers
-- ----------------------------
DROP TABLE IF EXISTS `stock_transfers`;
CREATE TABLE `stock_transfers`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `transfer_no` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `transfer_type` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `source_branch_id` int NULL DEFAULT NULL,
  `destination_branch_id` int NULL DEFAULT NULL,
  `source_location` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `destination_location` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `status` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Pending',
  `remarks` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `transferred_by` int NULL DEFAULT NULL,
  `completed_at` datetime(6) NULL DEFAULT NULL,
  `cancelled_at` datetime(6) NULL DEFAULT NULL,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_stock_transfers_transfer_no`(`transfer_no` ASC) USING BTREE,
  INDEX `IX_stock_transfers_source_branch_id`(`source_branch_id` ASC) USING BTREE,
  INDEX `IX_stock_transfers_destination_branch_id`(`destination_branch_id` ASC) USING BTREE,
  INDEX `IX_stock_transfers_transferred_by`(`transferred_by` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of stock_transfers
-- ----------------------------
INSERT INTO `stock_transfers` VALUES (1, 'TRF-20260701-0001', 'DisplayToWarehouseProduct', NULL, NULL, 'Display', 'Warehouse', 'Completed', NULL, 1, '2026-07-01 12:15:33.152011', NULL, '2026-07-01 12:15:33.152011', '2026-07-01 12:15:33.152011');

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
INSERT INTO `suppliers` VALUES (1, 'sup1', 'sup1', '123', 'adasd', 1, '2026-06-17 16:28:34.393344', '2026-06-30 17:32:17.628289', NULL, 1);
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
  `branch_id` int NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_tanks_fuel_id`(`fuel_id` ASC) USING BTREE,
  CONSTRAINT `FK_tanks_fuels_fuel_id` FOREIGN KEY (`fuel_id`) REFERENCES `fuels` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 8 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of tanks
-- ----------------------------
INSERT INTO `tanks` VALUES (1, 2, 'tank1', 10000.00, 1000.00, '2026-06-17 15:45:14.746149', '2026-07-08 16:37:23.218310', 1, 1, 3);
INSERT INTO `tanks` VALUES (2, 3, 'tank3', 10000.00, 1000.00, '2026-06-17 15:45:27.903450', '2026-07-14 06:37:13.807321', 1, 1, 3);
INSERT INTO `tanks` VALUES (3, 1, 'tank1', 10000.00, 996.00, '2026-06-17 15:45:45.855127', '2026-07-08 16:37:37.699244', 1, 1, 3);
INSERT INTO `tanks` VALUES (4, 4, 'tank4', 10000.00, 991.00, '2026-07-14 13:52:45.532462', '2026-07-14 13:53:51.326678', 1, 1, 3);
INSERT INTO `tanks` VALUES (5, 4, 'tank5', 10000.00, 992.00, '2026-07-14 14:43:06.510528', '2026-07-14 14:43:06.510528', 1, 1, 3);
INSERT INTO `tanks` VALUES (6, 4, 'sdasd', 9999.00, 999.00, '2026-07-14 17:09:46.918680', '2026-07-14 17:09:46.918680', 1, 1, 2);
INSERT INTO `tanks` VALUES (7, 3, 'QA-FIFO-TANK-20260717', 100.00, 3.00, '2026-07-17 10:46:17.000000', '2026-07-17 10:47:27.904356', 1, 1, 3);

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
) ENGINE = InnoDB AUTO_INCREMENT = 7 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of user_roles
-- ----------------------------
INSERT INTO `user_roles` VALUES (2, 2, 2, '2026-06-29 06:29:24', '2026-06-29 06:29:24');
INSERT INTO `user_roles` VALUES (5, 1, 1, '2026-07-14 17:23:05', '2026-07-14 17:23:05');
INSERT INTO `user_roles` VALUES (6, 3, 2, '2026-07-17 10:32:32', '2026-07-17 10:32:32');

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
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of users
-- ----------------------------
INSERT INTO `users` VALUES (1, 'admin', 'admin@gpos.local', '$2a$11$9TL7CKN.0e9TzMN9xWvBU...lh3rjgKTD2suXoC8qJOgyb7VjGvNW', '2026-06-15 03:30:45', '2026-07-14 17:23:05', 1, NULL, NULL, NULL, 3, NULL);
INSERT INTO `users` VALUES (2, 'testuser', 'testuser@gmail.com', '$2a$11$Zyq3VYudKWhZJaf/RBKVt.vIudv7tlq2Mh2mRryYJUvOOZBcbQEPS', '2026-06-29 06:29:24', '2026-06-29 06:29:24', 1, NULL, NULL, NULL, 1, 1);
INSERT INTO `users` VALUES (3, 'qa_void_no_permission', 'qa-void@gpos.local', '$2a$11$9TL7CKN.0e9TzMN9xWvBU...lh3rjgKTD2suXoC8qJOgyb7VjGvNW', '2026-07-17 10:32:20', '2026-07-17 10:32:20', 1, 'QA Void No Permission', NULL, NULL, 1, NULL);

-- ----------------------------
-- Table structure for vat_settings
-- ----------------------------
DROP TABLE IF EXISTS `vat_settings`;
CREATE TABLE `vat_settings`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `rate` decimal(10, 2) NOT NULL,
  `type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `is_default` tinyint(1) NOT NULL DEFAULT 0,
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` datetime(6) NOT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_vat_settings_is_active`(`is_active` ASC) USING BTREE,
  INDEX `IX_vat_settings_is_default`(`is_default` ASC) USING BTREE,
  INDEX `IX_vat_settings_type`(`type` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of vat_settings
-- ----------------------------
INSERT INTO `vat_settings` VALUES (1, 'VAT 13%', 13.00, 'Inclusive', 1, 1, '2026-07-01 04:25:34.112403', '2026-07-06 00:33:18.880442');

-- ----------------------------
-- Table structure for voucher_redemption_reversals
-- ----------------------------
DROP TABLE IF EXISTS `voucher_redemption_reversals`;
CREATE TABLE `voucher_redemption_reversals`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `sale_void_id` int NOT NULL,
  `voucher_redemption_id` int NOT NULL,
  `voucher_id` int NULL DEFAULT NULL,
  `member_id` int NULL DEFAULT NULL,
  `voucher_code_snapshot` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `applied_discount_amount` decimal(18, 2) NOT NULL,
  `created_by_user_id` int NOT NULL,
  `created_at` datetime(6) NOT NULL,
  `reason` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_voucher_redemption_reversals_voucher_redemption_id`(`voucher_redemption_id` ASC) USING BTREE,
  INDEX `IX_voucher_redemption_reversals_created_by_user_id`(`created_by_user_id` ASC) USING BTREE,
  INDEX `IX_voucher_redemption_reversals_member_id`(`member_id` ASC) USING BTREE,
  INDEX `IX_voucher_redemption_reversals_sale_void_id`(`sale_void_id` ASC) USING BTREE,
  INDEX `IX_voucher_redemption_reversals_voucher_id`(`voucher_id` ASC) USING BTREE,
  CONSTRAINT `FK_voucher_redemption_reversals_members_member_id` FOREIGN KEY (`member_id`) REFERENCES `members` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_voucher_redemption_reversals_sale_voids_sale_void_id` FOREIGN KEY (`sale_void_id`) REFERENCES `sale_voids` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_voucher_redemption_reversals_users_created_by_user_id` FOREIGN KEY (`created_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_voucher_redemption_reversals_voucher_redemptions_voucher_red~` FOREIGN KEY (`voucher_redemption_id`) REFERENCES `voucher_redemptions` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_voucher_redemption_reversals_vouchers_voucher_id` FOREIGN KEY (`voucher_id`) REFERENCES `vouchers` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of voucher_redemption_reversals
-- ----------------------------

-- ----------------------------
-- Table structure for voucher_redemptions
-- ----------------------------
DROP TABLE IF EXISTS `voucher_redemptions`;
CREATE TABLE `voucher_redemptions`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `voucher_id` int NULL DEFAULT NULL,
  `voucher_rule_id` int NOT NULL,
  `sale_id` int NOT NULL,
  `discount_amount` decimal(18, 2) NOT NULL,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `eligible_base_snapshot` decimal(18, 2) NULL DEFAULT NULL,
  `reward_type_snapshot` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `reward_value_snapshot` decimal(18, 2) NULL DEFAULT NULL,
  `rule_name_snapshot` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `voucher_code_snapshot` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_voucher_redemptions_sale_id`(`sale_id` ASC) USING BTREE,
  INDEX `IX_voucher_redemptions_voucher_id`(`voucher_id` ASC) USING BTREE,
  INDEX `IX_voucher_redemptions_voucher_rule_id`(`voucher_rule_id` ASC) USING BTREE,
  CONSTRAINT `FK_voucher_redemptions_sales_sale_id` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_voucher_redemptions_voucher_rules_voucher_rule_id` FOREIGN KEY (`voucher_rule_id`) REFERENCES `voucher_rules` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_voucher_redemptions_vouchers_voucher_id` FOREIGN KEY (`voucher_id`) REFERENCES `vouchers` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of voucher_redemptions
-- ----------------------------
INSERT INTO `voucher_redemptions` VALUES (1, 1, 1, 41, 100.00, '2026-06-30 09:07:28.806211', NULL, NULL, NULL, NULL, NULL);
INSERT INTO `voucher_redemptions` VALUES (2, 1, 1, 43, 100.00, '2026-07-07 01:32:11.475781', NULL, NULL, NULL, NULL, NULL);

-- ----------------------------
-- Table structure for voucher_rules
-- ----------------------------
DROP TABLE IF EXISTS `voucher_rules`;
CREATE TABLE `voucher_rules`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `reward_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `reward_value` decimal(18, 2) NOT NULL,
  `max_discount_amount` decimal(18, 2) NULL DEFAULT NULL,
  `minimum_purchase_amount` decimal(18, 2) NOT NULL,
  `applicable_product_ids` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `applicable_category_ids` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `applies_to` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `effective_date` datetime(6) NULL DEFAULT NULL,
  `expiration_date` datetime(6) NULL DEFAULT NULL,
  `no_expiration` tinyint(1) NOT NULL DEFAULT 0,
  `max_redemptions` int NULL DEFAULT NULL,
  `usage_limit_type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `limited_use_count` int NULL DEFAULT NULL,
  `priority` int NOT NULL DEFAULT 0,
  `status` int NOT NULL DEFAULT 1,
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  `voucher_id` int NULL DEFAULT NULL,
  `member_required` int NOT NULL DEFAULT 0,
  `code` varchar(6) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_voucher_rules_name`(`name` ASC) USING BTREE,
  UNIQUE INDEX `IX_voucher_rules_code`(`code` ASC) USING BTREE,
  INDEX `IX_voucher_rules_priority`(`priority` ASC) USING BTREE,
  INDEX `IX_voucher_rules_status`(`status` ASC) USING BTREE,
  INDEX `IX_voucher_rules_voucher_id`(`voucher_id` ASC) USING BTREE,
  CONSTRAINT `FK_voucher_rules_vouchers_voucher_id` FOREIGN KEY (`voucher_id`) REFERENCES `vouchers` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of voucher_rules
-- ----------------------------
INSERT INTO `voucher_rules` VALUES (1, 'FREE 100', 'Fixed Amount', 100.00, NULL, 1.00, '1,2,3,4', '1,2,3', 'Both', NULL, NULL, 1, 10, 'Unlimited', NULL, 0, 1, '2026-06-29 16:44:58.583356', '2026-06-30 02:43:23.821273', 1, 0, 'W7JI1L');

-- ----------------------------
-- Table structure for vouchers
-- ----------------------------
DROP TABLE IF EXISTS `vouchers`;
CREATE TABLE `vouchers`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `code` varchar(8) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `member_id` int NULL DEFAULT NULL,
  `status` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Active',
  `created_at` datetime(6) NULL DEFAULT NULL,
  `updated_at` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `IX_vouchers_code`(`code` ASC) USING BTREE,
  INDEX `IX_vouchers_member_id`(`member_id` ASC) USING BTREE,
  CONSTRAINT `FK_vouchers_members_member_id` FOREIGN KEY (`member_id`) REFERENCES `members` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of vouchers
-- ----------------------------
INSERT INTO `vouchers` VALUES (1, 'FREE100', NULL, 'Active', '2026-06-29 16:43:36.161446', '2026-07-07 01:32:11.475781');

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
  `branch_id` int NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_warehouse_stocks_batch_id`(`batch_id` ASC) USING BTREE,
  INDEX `IX_warehouse_stocks_product_id`(`product_id` ASC) USING BTREE,
  CONSTRAINT `FK_warehouse_stocks_product_batches_batch_id` FOREIGN KEY (`batch_id`) REFERENCES `product_batches` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_warehouse_stocks_products_product_id` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `CK_warehouse_stocks_quantity_non_negative` CHECK (`quantity` >= 0)
) ENGINE = InnoDB AUTO_INCREMENT = 7 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of warehouse_stocks
-- ----------------------------
INSERT INTO `warehouse_stocks` VALUES (1, 1, 1, 876.00, '2026-06-18 04:02:06.529349', '2026-06-18 04:02:06.529349', NULL);
INSERT INTO `warehouse_stocks` VALUES (2, 4, 5, 99.00, '2026-06-30 15:34:54.936117', '2026-06-30 15:34:54.936117', NULL);
INSERT INTO `warehouse_stocks` VALUES (3, 4, 6, 0.00, '2026-06-30 15:35:34.202721', '2026-07-19 15:45:09.419442', 3);
INSERT INTO `warehouse_stocks` VALUES (4, 4, 4, 100.00, '2026-07-01 12:15:33.152011', '2026-07-01 12:15:33.152011', NULL);
INSERT INTO `warehouse_stocks` VALUES (5, 1, 7, 10.00, '2026-07-09 08:29:11.997218', '2026-07-09 08:29:11.997218', 3);
INSERT INTO `warehouse_stocks` VALUES (6, 1, 8, 0.00, '2026-07-09 08:30:25.437677', '2026-07-19 15:30:52.079698', 3);

SET FOREIGN_KEY_CHECKS = 1;

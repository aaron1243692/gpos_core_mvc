using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260710100000_CreateCashModuleTables")]
    public partial class CreateCashModuleTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `daily_cash` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `branch_id` int NOT NULL,
                    `shift_id` int NOT NULL,
                    `user_id` int NOT NULL,
                    `business_date` date NOT NULL,
                    `opening_cash` decimal(18,2) NOT NULL DEFAULT 0,
                    `cash_sales` decimal(18,2) NOT NULL DEFAULT 0,
                    `total_cash_in` decimal(18,2) NOT NULL DEFAULT 0,
                    `total_cash_out` decimal(18,2) NOT NULL DEFAULT 0,
                    `expected_cash` decimal(18,2) NOT NULL DEFAULT 0,
                    `actual_cash` decimal(18,2) NOT NULL DEFAULT 0,
                    `difference` decimal(18,2) NOT NULL DEFAULT 0,
                    `remitted_amount` decimal(18,2) NOT NULL DEFAULT 0,
                    `received_by_user_id` int NULL,
                    `remarks` varchar(500) NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_by_user_id` int NULL,
                    `created_at` datetime(6) NULL,
                    `updated_at` datetime(6) NULL,
                    CONSTRAINT `PK_daily_cash` PRIMARY KEY (`id`),
                    INDEX `IX_daily_cash_scope` (`branch_id`, `shift_id`, `user_id`, `business_date`),
                    INDEX `IX_daily_cash_shift_id` (`shift_id`),
                    INDEX `IX_daily_cash_user_id` (`user_id`),
                    INDEX `IX_daily_cash_received_by_user_id` (`received_by_user_id`),
                    INDEX `IX_daily_cash_created_by_user_id` (`created_by_user_id`),
                    CONSTRAINT `FK_daily_cash_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_daily_cash_shift_settings_shift_id` FOREIGN KEY (`shift_id`) REFERENCES `shift_settings` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_daily_cash_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_daily_cash_users_received_by_user_id` FOREIGN KEY (`received_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_daily_cash_users_created_by_user_id` FOREIGN KEY (`created_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT
                );
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `cash_ins` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `branch_id` int NOT NULL,
                    `shift_id` int NOT NULL,
                    `user_id` int NOT NULL,
                    `daily_cash_id` int NULL,
                    `transaction_datetime` datetime(6) NOT NULL,
                    `amount` decimal(18,2) NOT NULL DEFAULT 0,
                    `reason` varchar(200) NOT NULL,
                    `remarks` varchar(500) NULL,
                    `created_by_user_id` int NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime(6) NULL,
                    `updated_at` datetime(6) NULL,
                    CONSTRAINT `PK_cash_ins` PRIMARY KEY (`id`),
                    INDEX `IX_cash_ins_scope` (`branch_id`, `shift_id`, `user_id`, `transaction_datetime`),
                    INDEX `IX_cash_ins_daily_cash_id` (`daily_cash_id`),
                    INDEX `IX_cash_ins_shift_id` (`shift_id`),
                    INDEX `IX_cash_ins_user_id` (`user_id`),
                    INDEX `IX_cash_ins_created_by_user_id` (`created_by_user_id`),
                    CONSTRAINT `FK_cash_ins_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_cash_ins_shift_settings_shift_id` FOREIGN KEY (`shift_id`) REFERENCES `shift_settings` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_cash_ins_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_cash_ins_daily_cash_daily_cash_id` FOREIGN KEY (`daily_cash_id`) REFERENCES `daily_cash` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_cash_ins_users_created_by_user_id` FOREIGN KEY (`created_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT
                );
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `cash_outs` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `branch_id` int NOT NULL,
                    `shift_id` int NOT NULL,
                    `user_id` int NOT NULL,
                    `daily_cash_id` int NULL,
                    `transaction_datetime` datetime(6) NOT NULL,
                    `amount` decimal(18,2) NOT NULL DEFAULT 0,
                    `reason` varchar(200) NOT NULL,
                    `remarks` varchar(500) NULL,
                    `created_by_user_id` int NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime(6) NULL,
                    `updated_at` datetime(6) NULL,
                    CONSTRAINT `PK_cash_outs` PRIMARY KEY (`id`),
                    INDEX `IX_cash_outs_scope` (`branch_id`, `shift_id`, `user_id`, `transaction_datetime`),
                    INDEX `IX_cash_outs_daily_cash_id` (`daily_cash_id`),
                    INDEX `IX_cash_outs_shift_id` (`shift_id`),
                    INDEX `IX_cash_outs_user_id` (`user_id`),
                    INDEX `IX_cash_outs_created_by_user_id` (`created_by_user_id`),
                    CONSTRAINT `FK_cash_outs_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_cash_outs_shift_settings_shift_id` FOREIGN KEY (`shift_id`) REFERENCES `shift_settings` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_cash_outs_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_cash_outs_daily_cash_daily_cash_id` FOREIGN KEY (`daily_cash_id`) REFERENCES `daily_cash` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_cash_outs_users_created_by_user_id` FOREIGN KEY (`created_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT
                );
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `cash_remittances` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `remittance_no` varchar(50) NOT NULL,
                    `branch_id` int NOT NULL,
                    `shift_id` int NOT NULL,
                    `user_id` int NOT NULL,
                    `daily_cash_id` int NOT NULL,
                    `expected_cash` decimal(18,2) NOT NULL DEFAULT 0,
                    `actual_cash` decimal(18,2) NOT NULL DEFAULT 0,
                    `remitted_amount` decimal(18,2) NOT NULL DEFAULT 0,
                    `remittance_difference` decimal(18,2) NOT NULL DEFAULT 0,
                    `received_by_user_id` int NOT NULL,
                    `received_datetime` datetime(6) NOT NULL,
                    `remarks` varchar(500) NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_by_user_id` int NULL,
                    `created_at` datetime(6) NULL,
                    `updated_at` datetime(6) NULL,
                    CONSTRAINT `PK_cash_remittances` PRIMARY KEY (`id`),
                    UNIQUE INDEX `IX_cash_remittances_remittance_no` (`remittance_no`),
                    INDEX `IX_cash_remittances_daily_cash_id` (`daily_cash_id`),
                    INDEX `IX_cash_remittances_branch_id` (`branch_id`),
                    INDEX `IX_cash_remittances_shift_id` (`shift_id`),
                    INDEX `IX_cash_remittances_user_id` (`user_id`),
                    INDEX `IX_cash_remittances_received_by_user_id` (`received_by_user_id`),
                    INDEX `IX_cash_remittances_created_by_user_id` (`created_by_user_id`),
                    CONSTRAINT `FK_cash_remittances_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_cash_remittances_shift_settings_shift_id` FOREIGN KEY (`shift_id`) REFERENCES `shift_settings` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_cash_remittances_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_cash_remittances_daily_cash_daily_cash_id` FOREIGN KEY (`daily_cash_id`) REFERENCES `daily_cash` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_cash_remittances_users_received_by_user_id` FOREIGN KEY (`received_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_cash_remittances_users_created_by_user_id` FOREIGN KEY (`created_by_user_id`) REFERENCES `users` (`id`) ON DELETE RESTRICT
                );
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}

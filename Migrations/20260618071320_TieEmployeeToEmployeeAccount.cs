using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    /// <inheritdoc />
    public partial class TieEmployeeToEmployeeAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                SET @employee_no_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employees'
                      AND COLUMN_NAME = 'employee_no'
                );

                SET @statement = IF(
                    @employee_no_exists > 0,
                    'ALTER TABLE `employees` DROP COLUMN `employee_no`',
                    'SELECT 1'
                );
                PREPARE drop_employee_no FROM @statement;
                EXECUTE drop_employee_no;
                DEALLOCATE PREPARE drop_employee_no;

                SET @singular_account_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                );

                SET @plural_account_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_accounts'
                );

                SET @statement = IF(
                    @singular_account_exists = 0 AND @plural_account_exists > 0,
                    'RENAME TABLE `employee_accounts` TO `employee_account`',
                    'SELECT 1'
                );
                PREPARE rename_employee_account FROM @statement;
                EXECUTE rename_employee_account;
                DEALLOCATE PREPARE rename_employee_account;

                CREATE TABLE IF NOT EXISTS `employee_account` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `employee_id` int NULL,
                    `username` varchar(100) NOT NULL,
                    `password_hash` varchar(255) NOT NULL,
                    `role` varchar(50) NOT NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET=utf8mb4;

                SET @password_hash_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'password_hash'
                );

                SET @password_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'password'
                );

                SET @statement = IF(
                    @password_hash_exists = 0 AND @password_exists > 0,
                    'ALTER TABLE `employee_account` RENAME COLUMN `password` TO `password_hash`',
                    'SELECT 1'
                );
                PREPARE rename_password_hash FROM @statement;
                EXECUTE rename_password_hash;
                DEALLOCATE PREPARE rename_password_hash;

                SET @password_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'password'
                );

                SET @statement = IF(
                    @password_exists > 0,
                    'ALTER TABLE `employee_account` DROP COLUMN `password`',
                    'SELECT 1'
                );
                PREPARE drop_password FROM @statement;
                EXECUTE drop_password;
                DEALLOCATE PREPARE drop_password;

                SET @employee_id_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'employee_id'
                );

                SET @statement = IF(
                    @employee_id_exists = 0,
                    'ALTER TABLE `employee_account` ADD COLUMN `employee_id` int NULL AFTER `id`',
                    'SELECT 1'
                );
                PREPARE add_employee_id FROM @statement;
                EXECUTE add_employee_id;
                DEALLOCATE PREPARE add_employee_id;

                SET @status_data_type = (
                    SELECT DATA_TYPE
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'status'
                    LIMIT 1
                );

                SET @statement = IF(
                    @status_data_type IS NOT NULL AND @status_data_type <> 'int',
                    'UPDATE `employee_account` SET `status` = CASE WHEN LOWER(`status`) = ''active'' THEN ''1'' WHEN LOWER(`status`) = ''inactive'' THEN ''0'' ELSE `status` END',
                    'SELECT 1'
                );
                PREPARE normalize_account_status FROM @statement;
                EXECUTE normalize_account_status;
                DEALLOCATE PREPARE normalize_account_status;

                SET @statement = IF(
                    @status_data_type IS NOT NULL AND @status_data_type <> 'int',
                    'ALTER TABLE `employee_account` MODIFY COLUMN `status` int NOT NULL DEFAULT 1',
                    'SELECT 1'
                );
                PREPARE alter_account_status FROM @statement;
                EXECUTE alter_account_status;
                DEALLOCATE PREPARE alter_account_status;

                SET @password_hash_length = (
                    SELECT CHARACTER_MAXIMUM_LENGTH
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'password_hash'
                    LIMIT 1
                );

                SET @statement = IF(
                    @password_hash_length IS NULL OR @password_hash_length <> 255,
                    'ALTER TABLE `employee_account` MODIFY COLUMN `password_hash` varchar(255) NOT NULL',
                    'SELECT 1'
                );
                PREPARE alter_password_hash FROM @statement;
                EXECUTE alter_password_hash;
                DEALLOCATE PREPARE alter_password_hash;

                SET @username_unique_index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'username'
                      AND NON_UNIQUE = 0
                );

                SET @statement = IF(
                    @username_unique_index_exists = 0,
                    'CREATE UNIQUE INDEX `IX_employee_account_username` ON `employee_account` (`username`)',
                    'SELECT 1'
                );
                PREPARE add_employee_account_username_index FROM @statement;
                EXECUTE add_employee_account_username_index;
                DEALLOCATE PREPARE add_employee_account_username_index;

                SET @employee_id_unique_index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'employee_id'
                      AND NON_UNIQUE = 0
                );

                SET @statement = IF(
                    @employee_id_unique_index_exists = 0,
                    'CREATE UNIQUE INDEX `IX_employee_account_employee_id` ON `employee_account` (`employee_id`)',
                    'SELECT 1'
                );
                PREPARE add_employee_account_employee_id_index FROM @statement;
                EXECUTE add_employee_account_employee_id_index;
                DEALLOCATE PREPARE add_employee_account_employee_id_index;

                SET @employee_account_fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND CONSTRAINT_NAME = 'FK_employee_account_employees_employee_id'
                );

                SET @statement = IF(
                    @employee_account_fk_exists = 0,
                    'ALTER TABLE `employee_account` ADD CONSTRAINT `FK_employee_account_employees_employee_id` FOREIGN KEY (`employee_id`) REFERENCES `employees` (`id`) ON DELETE RESTRICT',
                    'SELECT 1'
                );
                PREPARE add_employee_account_fk FROM @statement;
                EXECUTE add_employee_account_fk;
                DEALLOCATE PREPARE add_employee_account_fk;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                SET @employee_account_fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND CONSTRAINT_NAME = 'FK_employee_account_employees_employee_id'
                );

                SET @statement = IF(
                    @employee_account_fk_exists > 0,
                    'ALTER TABLE `employee_account` DROP FOREIGN KEY `FK_employee_account_employees_employee_id`',
                    'SELECT 1'
                );
                PREPARE drop_employee_account_fk FROM @statement;
                EXECUTE drop_employee_account_fk;
                DEALLOCATE PREPARE drop_employee_account_fk;
                """);
        }
    }
}

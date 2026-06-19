using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    /// <inheritdoc />
    public partial class UseEmployeeAccountOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `employee_account` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `username` varchar(100) NOT NULL,
                    `password_hash` varchar(255) NOT NULL,
                    `full_name` varchar(150) NOT NULL,
                    `email` varchar(150) NULL,
                    `contact_number` varchar(50) NULL,
                    `address` varchar(255) NULL,
                    `department_id` int NOT NULL,
                    `position_id` int NULL,
                    `role` varchar(50) NOT NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET=utf8mb4;

                SET @full_name_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'full_name'
                );
                SET @statement = IF(
                    @full_name_exists = 0,
                    'ALTER TABLE `employee_account` ADD COLUMN `full_name` varchar(150) NULL AFTER `password_hash`',
                    'SELECT 1'
                );
                PREPARE add_full_name FROM @statement;
                EXECUTE add_full_name;
                DEALLOCATE PREPARE add_full_name;

                SET @email_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'email'
                );
                SET @statement = IF(
                    @email_exists = 0,
                    'ALTER TABLE `employee_account` ADD COLUMN `email` varchar(150) NULL AFTER `full_name`',
                    'SELECT 1'
                );
                PREPARE add_email FROM @statement;
                EXECUTE add_email;
                DEALLOCATE PREPARE add_email;

                SET @contact_number_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'contact_number'
                );
                SET @statement = IF(
                    @contact_number_exists = 0,
                    'ALTER TABLE `employee_account` ADD COLUMN `contact_number` varchar(50) NULL AFTER `email`',
                    'SELECT 1'
                );
                PREPARE add_contact_number FROM @statement;
                EXECUTE add_contact_number;
                DEALLOCATE PREPARE add_contact_number;

                SET @address_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'address'
                );
                SET @statement = IF(
                    @address_exists = 0,
                    'ALTER TABLE `employee_account` ADD COLUMN `address` varchar(255) NULL AFTER `contact_number`',
                    'SELECT 1'
                );
                PREPARE add_address FROM @statement;
                EXECUTE add_address;
                DEALLOCATE PREPARE add_address;

                SET @department_id_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'department_id'
                );
                SET @statement = IF(
                    @department_id_exists = 0,
                    'ALTER TABLE `employee_account` ADD COLUMN `department_id` int NULL AFTER `address`',
                    'SELECT 1'
                );
                PREPARE add_department_id FROM @statement;
                EXECUTE add_department_id;
                DEALLOCATE PREPARE add_department_id;

                SET @position_id_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'position_id'
                );
                SET @statement = IF(
                    @position_id_exists = 0,
                    'ALTER TABLE `employee_account` ADD COLUMN `position_id` int NULL AFTER `department_id`',
                    'SELECT 1'
                );
                PREPARE add_position_id FROM @statement;
                EXECUTE add_position_id;
                DEALLOCATE PREPARE add_position_id;

                SET @employee_id_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'employee_id'
                );

                SET @employees_table_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employees'
                );

                SET @name_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'name'
                );

                SET @statement = IF(
                    @employee_id_exists > 0 AND @employees_table_exists > 0,
                    'UPDATE `employee_account` account JOIN `employees` employee ON employee.id = account.employee_id SET account.full_name = COALESCE(NULLIF(account.full_name, ''''), employee.full_name), account.email = COALESCE(account.email, employee.email), account.contact_number = COALESCE(account.contact_number, employee.contact_number), account.address = COALESCE(account.address, employee.address), account.department_id = COALESCE(account.department_id, employee.department_id), account.position_id = COALESCE(account.position_id, employee.position_id), account.status = COALESCE(account.status, employee.status)',
                    'SELECT 1'
                );
                PREPARE copy_linked_employee_values FROM @statement;
                EXECUTE copy_linked_employee_values;
                DEALLOCATE PREPARE copy_linked_employee_values;

                SET @statement = IF(
                    @name_exists > 0,
                    'UPDATE `employee_account` SET `full_name` = COALESCE(NULLIF(`full_name`, ''''), NULLIF(`name`, ''''), `username`)',
                    'UPDATE `employee_account` SET `full_name` = COALESCE(NULLIF(`full_name`, ''''), `username`)'
                );
                PREPARE backfill_full_name FROM @statement;
                EXECUTE backfill_full_name;
                DEALLOCATE PREPARE backfill_full_name;

                UPDATE `employee_account`
                SET `department_id` = COALESCE(
                    `department_id`,
                    (SELECT `id` FROM `departments` ORDER BY `id` LIMIT 1)
                );

                SET @position_fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND CONSTRAINT_NAME = 'FK_employee_account_positions_position_id'
                );
                SET @statement = IF(
                    @position_fk_exists > 0,
                    'ALTER TABLE `employee_account` DROP FOREIGN KEY `FK_employee_account_positions_position_id`',
                    'SELECT 1'
                );
                PREPARE drop_existing_position_fk FROM @statement;
                EXECUTE drop_existing_position_fk;
                DEALLOCATE PREPARE drop_existing_position_fk;

                SET @department_fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND CONSTRAINT_NAME = 'FK_employee_account_departments_department_id'
                );
                SET @statement = IF(
                    @department_fk_exists > 0,
                    'ALTER TABLE `employee_account` DROP FOREIGN KEY `FK_employee_account_departments_department_id`',
                    'SELECT 1'
                );
                PREPARE drop_existing_department_fk FROM @statement;
                EXECUTE drop_existing_department_fk;
                DEALLOCATE PREPARE drop_existing_department_fk;

                SET @employee_fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND CONSTRAINT_NAME = 'FK_employee_account_employees_employee_id'
                );
                SET @statement = IF(
                    @employee_fk_exists > 0,
                    'ALTER TABLE `employee_account` DROP FOREIGN KEY `FK_employee_account_employees_employee_id`',
                    'SELECT 1'
                );
                PREPARE drop_employee_fk FROM @statement;
                EXECUTE drop_employee_fk;
                DEALLOCATE PREPARE drop_employee_fk;

                SET @employee_id_index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND INDEX_NAME = 'IX_employee_account_employee_id'
                );
                SET @statement = IF(
                    @employee_id_index_exists > 0,
                    'DROP INDEX `IX_employee_account_employee_id` ON `employee_account`',
                    'SELECT 1'
                );
                PREPARE drop_employee_id_index FROM @statement;
                EXECUTE drop_employee_id_index;
                DEALLOCATE PREPARE drop_employee_id_index;

                SET @employee_id_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'employee_id'
                );
                SET @statement = IF(
                    @employee_id_exists > 0,
                    'ALTER TABLE `employee_account` DROP COLUMN `employee_id`',
                    'SELECT 1'
                );
                PREPARE drop_employee_id FROM @statement;
                EXECUTE drop_employee_id;
                DEALLOCATE PREPARE drop_employee_id;

                SET @name_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'name'
                );
                SET @statement = IF(
                    @name_exists > 0,
                    'ALTER TABLE `employee_account` DROP COLUMN `name`',
                    'SELECT 1'
                );
                PREPARE drop_legacy_name FROM @statement;
                EXECUTE drop_legacy_name;
                DEALLOCATE PREPARE drop_legacy_name;

                ALTER TABLE `employee_account`
                    MODIFY COLUMN `username` varchar(100) NOT NULL,
                    MODIFY COLUMN `password_hash` varchar(255) NOT NULL,
                    MODIFY COLUMN `full_name` varchar(150) NOT NULL,
                    MODIFY COLUMN `email` varchar(150) NULL,
                    MODIFY COLUMN `contact_number` varchar(50) NULL,
                    MODIFY COLUMN `address` varchar(255) NULL,
                    MODIFY COLUMN `department_id` int NOT NULL,
                    MODIFY COLUMN `position_id` int NULL,
                    MODIFY COLUMN `role` varchar(50) NOT NULL,
                    MODIFY COLUMN `status` int NOT NULL DEFAULT 1,
                    MODIFY COLUMN `created_at` datetime NULL,
                    MODIFY COLUMN `updated_at` datetime NULL;

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
                PREPARE add_username_index FROM @statement;
                EXECUTE add_username_index;
                DEALLOCATE PREPARE add_username_index;

                SET @department_index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND INDEX_NAME = 'IX_employee_account_department_id'
                );
                SET @statement = IF(
                    @department_index_exists = 0,
                    'CREATE INDEX `IX_employee_account_department_id` ON `employee_account` (`department_id`)',
                    'SELECT 1'
                );
                PREPARE add_department_index FROM @statement;
                EXECUTE add_department_index;
                DEALLOCATE PREPARE add_department_index;

                SET @position_index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND INDEX_NAME = 'IX_employee_account_position_id'
                );
                SET @statement = IF(
                    @position_index_exists = 0,
                    'CREATE INDEX `IX_employee_account_position_id` ON `employee_account` (`position_id`)',
                    'SELECT 1'
                );
                PREPARE add_position_index FROM @statement;
                EXECUTE add_position_index;
                DEALLOCATE PREPARE add_position_index;

                SET @department_fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND CONSTRAINT_NAME = 'FK_employee_account_departments_department_id'
                );
                SET @statement = IF(
                    @department_fk_exists = 0,
                    'ALTER TABLE `employee_account` ADD CONSTRAINT `FK_employee_account_departments_department_id` FOREIGN KEY (`department_id`) REFERENCES `departments` (`id`) ON DELETE RESTRICT',
                    'SELECT 1'
                );
                PREPARE add_department_fk FROM @statement;
                EXECUTE add_department_fk;
                DEALLOCATE PREPARE add_department_fk;

                SET @position_fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND CONSTRAINT_NAME = 'FK_employee_account_positions_position_id'
                );
                SET @statement = IF(
                    @position_fk_exists = 0,
                    'ALTER TABLE `employee_account` ADD CONSTRAINT `FK_employee_account_positions_position_id` FOREIGN KEY (`position_id`) REFERENCES `positions` (`id`) ON DELETE RESTRICT',
                    'SELECT 1'
                );
                PREPARE add_position_fk FROM @statement;
                EXECUTE add_position_fk;
                DEALLOCATE PREPARE add_position_fk;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                SET @department_fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND CONSTRAINT_NAME = 'FK_employee_account_departments_department_id'
                );
                SET @statement = IF(
                    @department_fk_exists > 0,
                    'ALTER TABLE `employee_account` DROP FOREIGN KEY `FK_employee_account_departments_department_id`',
                    'SELECT 1'
                );
                PREPARE drop_department_fk FROM @statement;
                EXECUTE drop_department_fk;
                DEALLOCATE PREPARE drop_department_fk;

                SET @position_fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND CONSTRAINT_NAME = 'FK_employee_account_positions_position_id'
                );
                SET @statement = IF(
                    @position_fk_exists > 0,
                    'ALTER TABLE `employee_account` DROP FOREIGN KEY `FK_employee_account_positions_position_id`',
                    'SELECT 1'
                );
                PREPARE drop_position_fk FROM @statement;
                EXECUTE drop_position_fk;
                DEALLOCATE PREPARE drop_position_fk;
                """);
        }
    }
}

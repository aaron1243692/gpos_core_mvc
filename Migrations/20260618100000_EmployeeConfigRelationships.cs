using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeConfigRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `branches` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `name` varchar(150) NOT NULL,
                    `address` varchar(255) NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `positions` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `name` varchar(150) NOT NULL,
                    `description` varchar(255) NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `departments` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `branch_id` int NOT NULL,
                    `name` varchar(150) NOT NULL,
                    `description` varchar(255) NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET=utf8mb4;

                CREATE TABLE IF NOT EXISTS `employees` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `employee_no` varchar(100) NOT NULL,
                    `full_name` varchar(150) NOT NULL,
                    `email` varchar(150) NULL,
                    `contact_number` varchar(50) NULL,
                    `address` varchar(255) NULL,
                    `department_id` int NOT NULL,
                    `position_id` int NOT NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET=utf8mb4;

                SET @department_branch_index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'departments'
                      AND INDEX_NAME = 'IX_departments_branch_id'
                );

                SET @statement = IF(
                    @department_branch_index_exists = 0,
                    'CREATE INDEX `IX_departments_branch_id` ON `departments` (`branch_id`)',
                    'SELECT 1'
                );

                PREPARE add_department_branch_index FROM @statement;
                EXECUTE add_department_branch_index;
                DEALLOCATE PREPARE add_department_branch_index;

                SET @employee_no_index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employees'
                      AND INDEX_NAME = 'IX_employees_employee_no'
                );

                SET @statement = IF(
                    @employee_no_index_exists = 0,
                    'CREATE UNIQUE INDEX `IX_employees_employee_no` ON `employees` (`employee_no`)',
                    'SELECT 1'
                );

                PREPARE add_employee_no_index FROM @statement;
                EXECUTE add_employee_no_index;
                DEALLOCATE PREPARE add_employee_no_index;

                SET @employee_department_index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employees'
                      AND INDEX_NAME = 'IX_employees_department_id'
                );

                SET @statement = IF(
                    @employee_department_index_exists = 0,
                    'CREATE INDEX `IX_employees_department_id` ON `employees` (`department_id`)',
                    'SELECT 1'
                );

                PREPARE add_employee_department_index FROM @statement;
                EXECUTE add_employee_department_index;
                DEALLOCATE PREPARE add_employee_department_index;

                SET @employee_position_index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employees'
                      AND INDEX_NAME = 'IX_employees_position_id'
                );

                SET @statement = IF(
                    @employee_position_index_exists = 0,
                    'CREATE INDEX `IX_employees_position_id` ON `employees` (`position_id`)',
                    'SELECT 1'
                );

                PREPARE add_employee_position_index FROM @statement;
                EXECUTE add_employee_position_index;
                DEALLOCATE PREPARE add_employee_position_index;

                SET @department_branch_fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'departments'
                      AND CONSTRAINT_NAME = 'FK_departments_branches_branch_id'
                );

                SET @statement = IF(
                    @department_branch_fk_exists = 0,
                    'ALTER TABLE `departments` ADD CONSTRAINT `FK_departments_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE RESTRICT',
                    'SELECT 1'
                );

                PREPARE add_department_branch_fk FROM @statement;
                EXECUTE add_department_branch_fk;
                DEALLOCATE PREPARE add_department_branch_fk;

                SET @employee_department_fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employees'
                      AND CONSTRAINT_NAME = 'FK_employees_departments_department_id'
                );

                SET @statement = IF(
                    @employee_department_fk_exists = 0,
                    'ALTER TABLE `employees` ADD CONSTRAINT `FK_employees_departments_department_id` FOREIGN KEY (`department_id`) REFERENCES `departments` (`id`) ON DELETE RESTRICT',
                    'SELECT 1'
                );

                PREPARE add_employee_department_fk FROM @statement;
                EXECUTE add_employee_department_fk;
                DEALLOCATE PREPARE add_employee_department_fk;

                SET @employee_position_fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employees'
                      AND CONSTRAINT_NAME = 'FK_employees_positions_position_id'
                );

                SET @statement = IF(
                    @employee_position_fk_exists = 0,
                    'ALTER TABLE `employees` ADD CONSTRAINT `FK_employees_positions_position_id` FOREIGN KEY (`position_id`) REFERENCES `positions` (`id`) ON DELETE RESTRICT',
                    'SELECT 1'
                );

                PREPARE add_employee_position_fk FROM @statement;
                EXECUTE add_employee_position_fk;
                DEALLOCATE PREPARE add_employee_position_fk;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "employees");
            migrationBuilder.DropTable(name: "departments");
            migrationBuilder.DropTable(name: "positions");
            migrationBuilder.DropTable(name: "branches");
        }
    }
}

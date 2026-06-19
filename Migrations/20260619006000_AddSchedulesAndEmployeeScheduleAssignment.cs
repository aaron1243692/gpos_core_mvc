using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260619006000_AddSchedulesAndEmployeeScheduleAssignment")]
    public partial class AddSchedulesAndEmployeeScheduleAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `schedules` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `name` varchar(100) NOT NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime(6) NULL,
                    `updated_at` datetime(6) NULL,
                    CONSTRAINT `PK_schedules` PRIMARY KEY (`id`)
                );
                """);

            AddIndexIfMissing(migrationBuilder, "IX_schedules_name", "schedules", "name", unique: true);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `schedule_details` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `schedule_id` int NOT NULL,
                    `day_of_week` int NOT NULL,
                    `am_in` time NULL,
                    `am_out` time NULL,
                    `pm_in` time NULL,
                    `pm_out` time NULL,
                    `created_at` datetime(6) NULL,
                    `updated_at` datetime(6) NULL,
                    CONSTRAINT `PK_schedule_details` PRIMARY KEY (`id`),
                    CONSTRAINT `FK_schedule_details_schedules_schedule_id` FOREIGN KEY (`schedule_id`) REFERENCES `schedules` (`id`) ON DELETE CASCADE
                );
                """);

            AddIndexIfMissing(migrationBuilder, "IX_schedule_details_schedule_id", "schedule_details", "schedule_id");
            AddIndexIfMissing(migrationBuilder, "IX_schedule_details_schedule_id_day_of_week", "schedule_details", "schedule_id`, `day_of_week", unique: true);
            AddColumnIfMissing(migrationBuilder, "employee_account", "schedule_id", "int NULL");
            AddIndexIfMissing(migrationBuilder, "IX_employee_account_schedule_id", "employee_account", "schedule_id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_employee_account_schedules_schedule_id", "employee_account", "schedule_id", "schedules", "id", "SET NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            DropForeignKeyIfExists(migrationBuilder, "FK_employee_account_schedules_schedule_id", "employee_account");
            DropIndexIfExists(migrationBuilder, "IX_employee_account_schedule_id", "employee_account");
            DropColumnIfExists(migrationBuilder, "employee_account", "schedule_id");
            migrationBuilder.Sql("DROP TABLE IF EXISTS `schedule_details`;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS `schedules`;");
        }

        private static void AddColumnIfMissing(MigrationBuilder migrationBuilder, string table, string column, string definition)
        {
            migrationBuilder.Sql(
                $"""
                SET @column_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND COLUMN_NAME = '{column}'
                );
                SET @statement = IF(
                    @column_exists = 0,
                    'ALTER TABLE `{table}` ADD COLUMN `{column}` {definition}',
                    'SELECT 1'
                );
                PREPARE add_schedule_column FROM @statement;
                EXECUTE add_schedule_column;
                DEALLOCATE PREPARE add_schedule_column;
                """);
        }

        private static void DropColumnIfExists(MigrationBuilder migrationBuilder, string table, string column)
        {
            migrationBuilder.Sql(
                $"""
                SET @column_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND COLUMN_NAME = '{column}'
                );
                SET @statement = IF(
                    @column_exists > 0,
                    'ALTER TABLE `{table}` DROP COLUMN `{column}`',
                    'SELECT 1'
                );
                PREPARE drop_schedule_column FROM @statement;
                EXECUTE drop_schedule_column;
                DEALLOCATE PREPARE drop_schedule_column;
                """);
        }

        private static void AddIndexIfMissing(MigrationBuilder migrationBuilder, string name, string table, string columns, bool unique = false)
        {
            var uniqueSql = unique ? "UNIQUE " : string.Empty;
            migrationBuilder.Sql(
                $"""
                SET @index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND INDEX_NAME = '{name}'
                );
                SET @statement = IF(
                    @index_exists = 0,
                    'CREATE {uniqueSql}INDEX `{name}` ON `{table}` (`{columns}`)',
                    'SELECT 1'
                );
                PREPARE add_schedule_index FROM @statement;
                EXECUTE add_schedule_index;
                DEALLOCATE PREPARE add_schedule_index;
                """);
        }

        private static void DropIndexIfExists(MigrationBuilder migrationBuilder, string name, string table)
        {
            migrationBuilder.Sql(
                $"""
                SET @index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND INDEX_NAME = '{name}'
                );
                SET @statement = IF(
                    @index_exists > 0,
                    'DROP INDEX `{name}` ON `{table}`',
                    'SELECT 1'
                );
                PREPARE drop_schedule_index FROM @statement;
                EXECUTE drop_schedule_index;
                DEALLOCATE PREPARE drop_schedule_index;
                """);
        }

        private static void AddForeignKeyIfMissing(MigrationBuilder migrationBuilder, string name, string table, string column, string principalTable, string principalColumn, string onDelete)
        {
            migrationBuilder.Sql(
                $"""
                SET @fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND CONSTRAINT_NAME = '{name}'
                      AND CONSTRAINT_TYPE = 'FOREIGN KEY'
                );
                SET @statement = IF(
                    @fk_exists = 0,
                    'ALTER TABLE `{table}` ADD CONSTRAINT `{name}` FOREIGN KEY (`{column}`) REFERENCES `{principalTable}` (`{principalColumn}`) ON DELETE {onDelete}',
                    'SELECT 1'
                );
                PREPARE add_schedule_fk FROM @statement;
                EXECUTE add_schedule_fk;
                DEALLOCATE PREPARE add_schedule_fk;
                """);
        }

        private static void DropForeignKeyIfExists(MigrationBuilder migrationBuilder, string name, string table)
        {
            migrationBuilder.Sql(
                $"""
                SET @fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND CONSTRAINT_NAME = '{name}'
                      AND CONSTRAINT_TYPE = 'FOREIGN KEY'
                );
                SET @statement = IF(
                    @fk_exists > 0,
                    'ALTER TABLE `{table}` DROP FOREIGN KEY `{name}`',
                    'SELECT 1'
                );
                PREPARE drop_schedule_fk FROM @statement;
                EXECUTE drop_schedule_fk;
                DEALLOCATE PREPARE drop_schedule_fk;
                """);
        }
    }
}

using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260619005000_AddEmployeeShiftSchedules")]
    public partial class AddEmployeeShiftSchedules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `employee_shift_schedules` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `employee_account_id` int NOT NULL,
                    `shift_setting_id` int NULL,
                    `day_of_week` int NOT NULL,
                    `start_time` time NOT NULL,
                    `end_time` time NOT NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime(6) NULL,
                    `updated_at` datetime(6) NULL,
                    CONSTRAINT `PK_employee_shift_schedules` PRIMARY KEY (`id`),
                    CONSTRAINT `FK_employee_shift_schedules_employee_account_employee_account_id` FOREIGN KEY (`employee_account_id`) REFERENCES `employee_account` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_employee_shift_schedules_shift_settings_shift_setting_id` FOREIGN KEY (`shift_setting_id`) REFERENCES `shift_settings` (`id`) ON DELETE RESTRICT
                );
                """);

            AddIndexIfMissing(migrationBuilder, "IX_employee_shift_schedules_employee_account_id", "employee_shift_schedules", "employee_account_id");
            AddIndexIfMissing(migrationBuilder, "IX_employee_shift_schedules_shift_setting_id", "employee_shift_schedules", "shift_setting_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS `employee_shift_schedules`;");
        }

        private static void AddIndexIfMissing(MigrationBuilder migrationBuilder, string name, string table, string column)
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
                    @index_exists = 0,
                    'CREATE INDEX `{name}` ON `{table}` (`{column}`)',
                    'SELECT 1'
                );
                PREPARE add_employee_shift_schedule_index FROM @statement;
                EXECUTE add_employee_shift_schedule_index;
                DEALLOCATE PREPARE add_employee_shift_schedule_index;
                """);
        }
    }
}

using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260619004000_UseNozzleForPumpMeterReadings")]
    public partial class UseNozzleForPumpMeterReadings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddColumnIfMissing(migrationBuilder, "pump_meter_readings", "nozzle_id", "int NULL");
            AddColumnIfMissing(migrationBuilder, "pump_meter_readings", "shift_id", "int NULL");
            AddColumnIfMissing(migrationBuilder, "pump_meter_readings", "status", "int NOT NULL DEFAULT 1");
            AddColumnIfMissing(migrationBuilder, "pump_meter_readings", "updated_at", "datetime(6) NULL");

            migrationBuilder.Sql(
                """
                UPDATE `pump_meter_readings` AS reading
                JOIN (
                    SELECT `pump_id`, MIN(`id`) AS `nozzle_id`
                    FROM `nozzles`
                    GROUP BY `pump_id`
                ) AS nozzle_map ON nozzle_map.`pump_id` = reading.`pump_id`
                SET reading.`nozzle_id` = nozzle_map.`nozzle_id`
                WHERE reading.`nozzle_id` IS NULL;
                """);

            ModifyColumnIfExists(migrationBuilder, "pump_meter_readings", "pump_id", "int NULL");
            AddIndexIfMissing(migrationBuilder, "IX_pump_meter_readings_nozzle_id", "pump_meter_readings", "nozzle_id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_pump_meter_readings_nozzles_nozzle_id", "pump_meter_readings", "nozzle_id", "nozzles", "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            DropForeignKeyIfExists(migrationBuilder, "FK_pump_meter_readings_nozzles_nozzle_id", "pump_meter_readings");
            DropIndexIfExists(migrationBuilder, "IX_pump_meter_readings_nozzle_id", "pump_meter_readings");
            ModifyColumnIfExists(migrationBuilder, "pump_meter_readings", "pump_id", "int NOT NULL");
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
                PREPARE add_pump_meter_column FROM @statement;
                EXECUTE add_pump_meter_column;
                DEALLOCATE PREPARE add_pump_meter_column;
                """);
        }

        private static void ModifyColumnIfExists(MigrationBuilder migrationBuilder, string table, string column, string definition)
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
                    'ALTER TABLE `{table}` MODIFY COLUMN `{column}` {definition}',
                    'SELECT 1'
                );
                PREPARE modify_pump_meter_column FROM @statement;
                EXECUTE modify_pump_meter_column;
                DEALLOCATE PREPARE modify_pump_meter_column;
                """);
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
                PREPARE add_pump_meter_index FROM @statement;
                EXECUTE add_pump_meter_index;
                DEALLOCATE PREPARE add_pump_meter_index;
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
                PREPARE drop_pump_meter_index FROM @statement;
                EXECUTE drop_pump_meter_index;
                DEALLOCATE PREPARE drop_pump_meter_index;
                """);
        }

        private static void AddForeignKeyIfMissing(MigrationBuilder migrationBuilder, string name, string table, string column, string principalTable, string principalColumn)
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
                    'ALTER TABLE `{table}` ADD CONSTRAINT `{name}` FOREIGN KEY (`{column}`) REFERENCES `{principalTable}` (`{principalColumn}`) ON DELETE RESTRICT',
                    'SELECT 1'
                );
                PREPARE add_pump_meter_fk FROM @statement;
                EXECUTE add_pump_meter_fk;
                DEALLOCATE PREPARE add_pump_meter_fk;
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
                PREPARE drop_pump_meter_fk FROM @statement;
                EXECUTE drop_pump_meter_fk;
                DEALLOCATE PREPARE drop_pump_meter_fk;
                """);
        }
    }
}

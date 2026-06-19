using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    /// <inheritdoc />
    public partial class AddTankCapacityAndCurrentLiters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                SET @capacity_liters_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'tanks'
                      AND COLUMN_NAME = 'capacity_liters'
                );
                SET @statement = IF(
                    @capacity_liters_exists = 0,
                    'ALTER TABLE `tanks` ADD COLUMN `capacity_liters` decimal(18,2) NOT NULL DEFAULT 0 AFTER `tank_no`',
                    'SELECT 1'
                );
                PREPARE add_capacity_liters FROM @statement;
                EXECUTE add_capacity_liters;
                DEALLOCATE PREPARE add_capacity_liters;

                SET @current_liters_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'tanks'
                      AND COLUMN_NAME = 'current_liters'
                );
                SET @statement = IF(
                    @current_liters_exists = 0,
                    'ALTER TABLE `tanks` ADD COLUMN `current_liters` decimal(18,2) NOT NULL DEFAULT 0 AFTER `capacity_liters`',
                    'SELECT 1'
                );
                PREPARE add_current_liters FROM @statement;
                EXECUTE add_current_liters;
                DEALLOCATE PREPARE add_current_liters;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                SET @current_liters_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'tanks'
                      AND COLUMN_NAME = 'current_liters'
                );
                SET @statement = IF(
                    @current_liters_exists > 0,
                    'ALTER TABLE `tanks` DROP COLUMN `current_liters`',
                    'SELECT 1'
                );
                PREPARE drop_current_liters FROM @statement;
                EXECUTE drop_current_liters;
                DEALLOCATE PREPARE drop_current_liters;

                SET @capacity_liters_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'tanks'
                      AND COLUMN_NAME = 'capacity_liters'
                );
                SET @statement = IF(
                    @capacity_liters_exists > 0,
                    'ALTER TABLE `tanks` DROP COLUMN `capacity_liters`',
                    'SELECT 1'
                );
                PREPARE drop_capacity_liters FROM @statement;
                EXECUTE drop_capacity_liters;
                DEALLOCATE PREPARE drop_capacity_liters;
                """);
        }
    }
}

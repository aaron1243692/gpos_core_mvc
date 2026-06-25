using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260625090000_CreateLoyaltySettings")]
    public partial class CreateLoyaltySettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `loyalty_settings` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `setting_key` varchar(100) NOT NULL,
                    `decimal_value` decimal(18,2) NOT NULL DEFAULT 0,
                    `created_at` datetime(6) NULL,
                    `updated_at` datetime(6) NULL,
                    PRIMARY KEY (`id`),
                    UNIQUE KEY `IX_loyalty_settings_setting_key` (`setting_key`)
                ) CHARACTER SET=utf8mb4;

                SET @legacy_earn_rate = 5;
                SET @discount_earn_rate_column_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'discounts'
                      AND COLUMN_NAME = 'earn_rate'
                );
                SET @statement = IF(
                    @discount_earn_rate_column_exists = 1,
                    'SELECT COALESCE(NULLIF(MAX(`earn_rate`), 0), 5) INTO @legacy_earn_rate FROM `discounts`',
                    'SELECT 5 INTO @legacy_earn_rate'
                );
                PREPARE seed_loyalty_earn_rate FROM @statement;
                EXECUTE seed_loyalty_earn_rate;
                DEALLOCATE PREPARE seed_loyalty_earn_rate;

                INSERT INTO `loyalty_settings` (`setting_key`, `decimal_value`, `created_at`, `updated_at`)
                SELECT 'POINTS_EARN_RATE', @legacy_earn_rate, UTC_TIMESTAMP(6), UTC_TIMESTAMP(6)
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM `loyalty_settings`
                    WHERE `setting_key` = 'POINTS_EARN_RATE'
                );
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS `loyalty_settings`;");
        }
    }
}

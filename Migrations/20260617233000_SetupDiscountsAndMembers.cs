using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    /// <inheritdoc />
    public partial class SetupDiscountsAndMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `discounts` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `name` varchar(100) NOT NULL,
                    `earn_rate` decimal(5,2) NOT NULL DEFAULT 0,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET=utf8mb4;

                SET @discount_status_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'discounts'
                      AND COLUMN_NAME = 'status'
                );

                SET @statement = IF(
                    @discount_status_exists = 0,
                    'ALTER TABLE `discounts` ADD COLUMN `status` int NOT NULL DEFAULT 1',
                    'SELECT 1'
                );

                PREPARE add_discount_status FROM @statement;
                EXECUTE add_discount_status;
                DEALLOCATE PREPARE add_discount_status;

                ALTER TABLE `discounts` MODIFY COLUMN `earn_rate` decimal(5,2) NOT NULL DEFAULT 0;

                SET @discount_is_active_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'discounts'
                      AND COLUMN_NAME = 'is_active'
                );

                SET @statement = IF(
                    @discount_is_active_exists = 1,
                    'UPDATE `discounts` SET `status` = CASE WHEN `is_active` = 1 THEN 1 ELSE 0 END',
                    'SELECT 1'
                );

                PREPARE copy_discount_status FROM @statement;
                EXECUTE copy_discount_status;
                DEALLOCATE PREPARE copy_discount_status;

                SET @statement = IF(
                    @discount_is_active_exists = 1,
                    'ALTER TABLE `discounts` DROP COLUMN `is_active`',
                    'SELECT 1'
                );

                PREPARE drop_discount_is_active FROM @statement;
                EXECUTE drop_discount_is_active;
                DEALLOCATE PREPARE drop_discount_is_active;

                UPDATE `discounts` SET `status` = 1 WHERE `status` IS NULL;
                ALTER TABLE `discounts` MODIFY COLUMN `status` int NOT NULL DEFAULT 1;

                CREATE TABLE IF NOT EXISTS `members` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `member_no` varchar(100) NOT NULL,
                    `card_no` varchar(100) NULL,
                    `full_name` varchar(150) NOT NULL,
                    `contact_number` varchar(50) NULL,
                    `email` varchar(150) NULL,
                    `address` varchar(255) NULL,
                    `discount_id` int NULL,
                    `points` decimal(18,2) NOT NULL DEFAULT 0,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET=utf8mb4;

                SET @member_status_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'members'
                      AND COLUMN_NAME = 'status'
                );

                SET @statement = IF(
                    @member_status_exists = 0,
                    'ALTER TABLE `members` ADD COLUMN `status` int NOT NULL DEFAULT 1',
                    'SELECT 1'
                );

                PREPARE add_member_status FROM @statement;
                EXECUTE add_member_status;
                DEALLOCATE PREPARE add_member_status;

                SET @member_is_active_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'members'
                      AND COLUMN_NAME = 'is_active'
                );

                SET @statement = IF(
                    @member_is_active_exists = 1,
                    'UPDATE `members` SET `status` = CASE WHEN `is_active` = 1 THEN 1 ELSE 0 END',
                    'SELECT 1'
                );

                PREPARE copy_member_status FROM @statement;
                EXECUTE copy_member_status;
                DEALLOCATE PREPARE copy_member_status;

                SET @statement = IF(
                    @member_is_active_exists = 1,
                    'ALTER TABLE `members` DROP COLUMN `is_active`',
                    'SELECT 1'
                );

                PREPARE drop_member_is_active FROM @statement;
                EXECUTE drop_member_is_active;
                DEALLOCATE PREPARE drop_member_is_active;

                UPDATE `members` SET `status` = 1 WHERE `status` IS NULL;
                ALTER TABLE `members` MODIFY COLUMN `status` int NOT NULL DEFAULT 1;
                ALTER TABLE `members` MODIFY COLUMN `points` decimal(18,2) NOT NULL DEFAULT 0;

                SET @member_no_index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'members'
                      AND INDEX_NAME = 'IX_members_member_no'
                );

                SET @statement = IF(
                    @member_no_index_exists = 0,
                    'CREATE UNIQUE INDEX `IX_members_member_no` ON `members` (`member_no`)',
                    'SELECT 1'
                );

                PREPARE add_member_no_index FROM @statement;
                EXECUTE add_member_no_index;
                DEALLOCATE PREPARE add_member_no_index;

                SET @card_no_index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'members'
                      AND INDEX_NAME = 'IX_members_card_no'
                );

                SET @statement = IF(
                    @card_no_index_exists = 0,
                    'CREATE UNIQUE INDEX `IX_members_card_no` ON `members` (`card_no`)',
                    'SELECT 1'
                );

                PREPARE add_card_no_index FROM @statement;
                EXECUTE add_card_no_index;
                DEALLOCATE PREPARE add_card_no_index;

                SET @discount_id_index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'members'
                      AND INDEX_NAME = 'IX_members_discount_id'
                );

                SET @statement = IF(
                    @discount_id_index_exists = 0,
                    'CREATE INDEX `IX_members_discount_id` ON `members` (`discount_id`)',
                    'SELECT 1'
                );

                PREPARE add_discount_id_index FROM @statement;
                EXECUTE add_discount_id_index;
                DEALLOCATE PREPARE add_discount_id_index;

                SET @member_discount_fk_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'members'
                      AND CONSTRAINT_NAME = 'FK_members_discounts_discount_id'
                );

                SET @statement = IF(
                    @member_discount_fk_exists = 0,
                    'ALTER TABLE `members` ADD CONSTRAINT `FK_members_discounts_discount_id` FOREIGN KEY (`discount_id`) REFERENCES `discounts` (`id`) ON DELETE RESTRICT',
                    'SELECT 1'
                );

                PREPARE add_member_discount_fk FROM @statement;
                EXECUTE add_member_discount_fk;
                DEALLOCATE PREPARE add_member_discount_fk;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "discounts");
        }
    }
}

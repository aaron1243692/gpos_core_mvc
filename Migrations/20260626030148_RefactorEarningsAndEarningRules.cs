using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    /// <inheritdoc />
    public partial class RefactorEarningsAndEarningRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `earnings` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `name` varchar(100) NOT NULL,
                    `description` varchar(255) NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime NULL,
                    `updated_at` datetime NULL,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET=utf8mb4;
                """);

            AddColumnIfMissing(migrationBuilder, "members", "earnings_id", "int NULL");
            AddColumnIfMissing(migrationBuilder, "points_ledger", "sale_id", "int NULL");

            migrationBuilder.Sql(
                """
                SET @earning_rules_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'earning_rules'
                );

                SET @statement = IF(
                    @earning_rules_exists = 0,
                    'CREATE TABLE `earning_rules` (
                        `id` int NOT NULL AUTO_INCREMENT,
                        `name` varchar(100) NOT NULL,
                        `earnings_id` int NOT NULL,
                        `earn_type` varchar(50) NOT NULL,
                        `earn_value` decimal(18,2) NOT NULL,
                        `applies_to` varchar(50) NOT NULL,
                        `minimum_amount` decimal(18,2) NOT NULL DEFAULT 0,
                        `member_required` int NOT NULL DEFAULT 0,
                        `start_date` datetime NULL,
                        `end_date` datetime NULL,
                        `status` int NOT NULL DEFAULT 1,
                        `created_at` datetime NULL,
                        `updated_at` datetime NULL,
                        PRIMARY KEY (`id`)
                    ) CHARACTER SET=utf8mb4',
                    'SELECT 1'
                );
                PREPARE create_earning_rules_table FROM @statement;
                EXECUTE create_earning_rules_table;
                DEALLOCATE PREPARE create_earning_rules_table;
                """);

            AddColumnIfMissing(migrationBuilder, "earning_rules", "earnings_id", "int NULL");
            AddColumnIfMissing(migrationBuilder, "earning_rules", "earn_type", "varchar(50) NULL");
            AddColumnIfMissing(migrationBuilder, "earning_rules", "earn_value", "decimal(18,2) NULL");
            AddColumnIfMissing(migrationBuilder, "earning_rules", "applies_to", "varchar(50) NULL");
            AddColumnIfMissing(migrationBuilder, "earning_rules", "minimum_amount", "decimal(18,2) NOT NULL DEFAULT 0");
            AddColumnIfMissing(migrationBuilder, "earning_rules", "member_required", "int NOT NULL DEFAULT 0");
            AddColumnIfMissing(migrationBuilder, "earning_rules", "start_date", "datetime NULL");
            AddColumnIfMissing(migrationBuilder, "earning_rules", "end_date", "datetime NULL");
            AddColumnIfMissing(migrationBuilder, "earning_rules", "status", "int NOT NULL DEFAULT 1");

            migrationBuilder.Sql(
                """
                SET @old_earning_rule_shape = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'earning_rules'
                      AND COLUMN_NAME = 'earn_rate'
                );

                SET @statement = IF(
                    @old_earning_rule_shape > 0,
                    'INSERT IGNORE INTO `earnings` (`id`, `name`, `description`, `status`, `created_at`, `updated_at`)
                     SELECT `id`, `name`, `description`, CASE WHEN `is_active` = 1 THEN 1 ELSE 0 END, `created_at`, `updated_at`
                     FROM `earning_rules`',
                    'SELECT 1'
                );
                PREPARE migrate_earnings_profiles FROM @statement;
                EXECUTE migrate_earnings_profiles;
                DEALLOCATE PREPARE migrate_earnings_profiles;

                SET @statement = IF(
                    @old_earning_rule_shape > 0,
                    'UPDATE `earning_rules`
                     SET `earnings_id` = `id`,
                         `earn_type` = ''Percentage'',
                         `earn_value` = `earn_rate`,
                         `applies_to` = ''Both'',
                         `minimum_amount` = 0,
                         `member_required` = 1,
                         `status` = CASE WHEN `is_active` = 1 THEN 1 ELSE 0 END
                     WHERE `earnings_id` IS NULL',
                    'SELECT 1'
                );
                PREPARE migrate_earning_rules FROM @statement;
                EXECUTE migrate_earning_rules;
                DEALLOCATE PREPARE migrate_earning_rules;

                SET @member_old_earning_rule_id_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'members'
                      AND COLUMN_NAME = 'earning_rule_id'
                );

                SET @statement = IF(
                    @member_old_earning_rule_id_exists > 0,
                    'UPDATE `members` SET `earnings_id` = `earning_rule_id` WHERE `earnings_id` IS NULL',
                    'SELECT 1'
                );
                PREPARE migrate_member_earnings FROM @statement;
                EXECUTE migrate_member_earnings;
                DEALLOCATE PREPARE migrate_member_earnings;

                UPDATE `earning_rules`
                SET `earn_type` = 'Percentage'
                WHERE `earn_type` IS NULL OR `earn_type` = '';

                UPDATE `earning_rules`
                SET `earn_value` = 0
                WHERE `earn_value` IS NULL;

                UPDATE `earning_rules`
                SET `applies_to` = 'Both'
                WHERE `applies_to` IS NULL OR `applies_to` = '';
                """);

            DropForeignKeyIfExists(migrationBuilder, "FK_members_earning_rules_earning_rule_id", "members");
            DropIndexIfExists(migrationBuilder, "members", "IX_members_earning_rule_id");
            DropIndexIfExists(migrationBuilder, "earning_rules", "IX_earning_rules_name");
            DropIndexIfExists(migrationBuilder, "earning_rules", "IX_earning_rules_is_active");
            DropColumnIfExists(migrationBuilder, "members", "earning_rule_id");
            DropColumnIfExists(migrationBuilder, "earning_rules", "earn_rate");
            DropColumnIfExists(migrationBuilder, "earning_rules", "description");
            DropColumnIfExists(migrationBuilder, "earning_rules", "is_active");

            migrationBuilder.Sql(
                """
                ALTER TABLE `earning_rules` MODIFY COLUMN `earnings_id` int NOT NULL;
                ALTER TABLE `earning_rules` MODIFY COLUMN `earn_type` varchar(50) NOT NULL;
                ALTER TABLE `earning_rules` MODIFY COLUMN `earn_value` decimal(18,2) NOT NULL;
                ALTER TABLE `earning_rules` MODIFY COLUMN `applies_to` varchar(50) NOT NULL;
                ALTER TABLE `earning_rules` MODIFY COLUMN `status` int NOT NULL DEFAULT 1;
                """);

            AddIndexIfMissing(migrationBuilder, "earnings", "IX_earnings_name", "name", unique: true);
            AddIndexIfMissing(migrationBuilder, "earnings", "IX_earnings_status", "status");
            AddIndexIfMissing(migrationBuilder, "members", "IX_members_earnings_id", "earnings_id");
            AddIndexIfMissing(migrationBuilder, "earning_rules", "IX_earning_rules_earnings_id", "earnings_id");
            AddIndexIfMissing(migrationBuilder, "points_ledger", "IX_points_ledger_sale_id", "sale_id");
            AddCompositeIndexIfMissing(migrationBuilder, "points_ledger", "IX_points_ledger_member_id_sale_id_transaction_type", "`member_id`, `sale_id`, `transaction_type`");
            AddForeignKeyIfMissing(migrationBuilder, "FK_members_earnings_earnings_id", "members", "earnings_id", "earnings", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_earning_rules_earnings_earnings_id", "earning_rules", "earnings_id", "earnings", "id");
            migrationBuilder.Sql(
                """
                UPDATE `points_ledger`
                SET `sale_id` = `reference_id`
                WHERE `sale_id` IS NULL
                  AND `reference_type` = 'POS'
                  AND `reference_id` IS NOT NULL
                  AND EXISTS (SELECT 1 FROM `sales` WHERE `sales`.`id` = `points_ledger`.`reference_id`);
                """);
            AddForeignKeyIfMissing(migrationBuilder, "FK_points_ledger_sales_sale_id", "points_ledger", "sale_id", "sales", "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            DropForeignKeyIfExists(migrationBuilder, "FK_members_earnings_earnings_id", "members");
            DropForeignKeyIfExists(migrationBuilder, "FK_earning_rules_earnings_earnings_id", "earning_rules");
            DropIndexIfExists(migrationBuilder, "members", "IX_members_earnings_id");
            DropIndexIfExists(migrationBuilder, "earning_rules", "IX_earning_rules_earnings_id");
            DropColumnIfExists(migrationBuilder, "members", "earnings_id");
            migrationBuilder.DropTable(name: "earnings");
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
                PREPARE add_column_if_missing FROM @statement;
                EXECUTE add_column_if_missing;
                DEALLOCATE PREPARE add_column_if_missing;
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
                PREPARE drop_column_if_exists FROM @statement;
                EXECUTE drop_column_if_exists;
                DEALLOCATE PREPARE drop_column_if_exists;
                """);
        }

        private static void AddIndexIfMissing(MigrationBuilder migrationBuilder, string table, string index, string column, bool unique = false)
        {
            var uniqueSql = unique ? "UNIQUE " : string.Empty;
            migrationBuilder.Sql(
                $"""
                SET @index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND INDEX_NAME = '{index}'
                );
                SET @statement = IF(
                    @index_exists = 0,
                    'CREATE {uniqueSql}INDEX `{index}` ON `{table}` (`{column}`)',
                    'SELECT 1'
                );
                PREPARE add_index_if_missing FROM @statement;
                EXECUTE add_index_if_missing;
                DEALLOCATE PREPARE add_index_if_missing;
                """);
        }

        private static void AddCompositeIndexIfMissing(MigrationBuilder migrationBuilder, string table, string index, string columns)
        {
            migrationBuilder.Sql(
                $"""
                SET @index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND INDEX_NAME = '{index}'
                );
                SET @statement = IF(
                    @index_exists = 0,
                    'CREATE INDEX `{index}` ON `{table}` ({columns})',
                    'SELECT 1'
                );
                PREPARE add_composite_index_if_missing FROM @statement;
                EXECUTE add_composite_index_if_missing;
                DEALLOCATE PREPARE add_composite_index_if_missing;
                """);
        }

        private static void DropIndexIfExists(MigrationBuilder migrationBuilder, string table, string index)
        {
            migrationBuilder.Sql(
                $"""
                SET @index_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND INDEX_NAME = '{index}'
                );
                SET @statement = IF(
                    @index_exists > 0,
                    'DROP INDEX `{index}` ON `{table}`',
                    'SELECT 1'
                );
                PREPARE drop_index_if_exists FROM @statement;
                EXECUTE drop_index_if_exists;
                DEALLOCATE PREPARE drop_index_if_exists;
                """);
        }

        private static void AddForeignKeyIfMissing(MigrationBuilder migrationBuilder, string constraint, string table, string column, string principalTable, string principalColumn)
        {
            migrationBuilder.Sql(
                $"""
                SET @constraint_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND CONSTRAINT_NAME = '{constraint}'
                );
                SET @invalid_rows = (
                    SELECT COUNT(*)
                    FROM `{table}` child
                    LEFT JOIN `{principalTable}` parent ON child.`{column}` = parent.`{principalColumn}`
                    WHERE child.`{column}` IS NOT NULL
                      AND parent.`{principalColumn}` IS NULL
                );
                SET @statement = IF(
                    @constraint_exists = 0 AND @invalid_rows = 0,
                    'ALTER TABLE `{table}` ADD CONSTRAINT `{constraint}` FOREIGN KEY (`{column}`) REFERENCES `{principalTable}` (`{principalColumn}`) ON DELETE RESTRICT',
                    'SELECT 1'
                );
                PREPARE add_fk_if_missing FROM @statement;
                EXECUTE add_fk_if_missing;
                DEALLOCATE PREPARE add_fk_if_missing;
                """);
        }

        private static void DropForeignKeyIfExists(MigrationBuilder migrationBuilder, string constraint, string table)
        {
            migrationBuilder.Sql(
                $"""
                SET @constraint_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND CONSTRAINT_NAME = '{constraint}'
                );
                SET @statement = IF(
                    @constraint_exists > 0,
                    'ALTER TABLE `{table}` DROP FOREIGN KEY `{constraint}`',
                    'SELECT 1'
                );
                PREPARE drop_fk_if_exists FROM @statement;
                EXECUTE drop_fk_if_exists;
                DEALLOCATE PREPARE drop_fk_if_exists;
                """);
        }
    }
}

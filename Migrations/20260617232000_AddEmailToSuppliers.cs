using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToSuppliers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                SET @column_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'suppliers'
                      AND COLUMN_NAME = 'email'
                );

                SET @statement = IF(
                    @column_exists = 0,
                    'ALTER TABLE `suppliers` ADD COLUMN `email` varchar(150) NULL',
                    'SELECT 1'
                );

                PREPARE add_email_to_suppliers FROM @statement;
                EXECUTE add_email_to_suppliers;
                DEALLOCATE PREPARE add_email_to_suppliers;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                SET @column_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'suppliers'
                      AND COLUMN_NAME = 'email'
                );

                SET @statement = IF(
                    @column_exists = 1,
                    'ALTER TABLE `suppliers` DROP COLUMN `email`',
                    'SELECT 1'
                );

                PREPARE drop_email_from_suppliers FROM @statement;
                EXECUTE drop_email_from_suppliers;
                DEALLOCATE PREPARE drop_email_from_suppliers;
                """);
        }
    }
}

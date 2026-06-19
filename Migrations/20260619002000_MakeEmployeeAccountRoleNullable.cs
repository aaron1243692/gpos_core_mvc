using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using gpos.Data;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260619002000_MakeEmployeeAccountRoleNullable")]
    public partial class MakeEmployeeAccountRoleNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                SET @role_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'role'
                );
                SET @statement = IF(
                    @role_exists > 0,
                    'ALTER TABLE `employee_account` MODIFY COLUMN `role` varchar(50) NULL',
                    'SELECT 1'
                );
                PREPARE make_employee_role_nullable FROM @statement;
                EXECUTE make_employee_role_nullable;
                DEALLOCATE PREPARE make_employee_role_nullable;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                SET @role_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'employee_account'
                      AND COLUMN_NAME = 'role'
                );
                SET @statement = IF(
                    @role_exists > 0,
                    'ALTER TABLE `employee_account` MODIFY COLUMN `role` varchar(50) NOT NULL',
                    'SELECT 1'
                );
                PREPARE make_employee_role_required FROM @statement;
                EXECUTE make_employee_role_required;
                DEALLOCATE PREPARE make_employee_role_required;
                """);
        }
    }
}

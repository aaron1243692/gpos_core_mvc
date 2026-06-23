using gpos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260622100000_AddBranchDepartmentToUsers")]
    public partial class AddBranchDepartmentToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddColumnIfMissing(migrationBuilder, "users", "branch_id", "int NULL");
            AddColumnIfMissing(migrationBuilder, "users", "department_id", "int NULL");
            CreateIndexIfMissing(migrationBuilder, "users", "IX_users_branch_id", "`branch_id`");
            CreateIndexIfMissing(migrationBuilder, "users", "IX_users_department_id", "`department_id`");
            AddForeignKeyIfMissing(migrationBuilder, "FK_users_branches_branch_id", "users", "branch_id", "branches", "id");
            AddForeignKeyIfMissing(migrationBuilder, "FK_users_departments_department_id", "users", "department_id", "departments", "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                PREPARE add_branch_department_users_column FROM @statement;
                EXECUTE add_branch_department_users_column;
                DEALLOCATE PREPARE add_branch_department_users_column;
                """);
        }

        private static void CreateIndexIfMissing(MigrationBuilder migrationBuilder, string table, string index, string columns)
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
                PREPARE add_branch_department_users_index FROM @statement;
                EXECUTE add_branch_department_users_index;
                DEALLOCATE PREPARE add_branch_department_users_index;
                """);
        }

        private static void AddForeignKeyIfMissing(MigrationBuilder migrationBuilder, string constraint, string table, string column, string principalTable, string principalColumn)
        {
            migrationBuilder.Sql(
                $"""
                SET @constraint_exists = (
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                      AND CONSTRAINT_NAME = '{constraint}'
                );
                SET @statement = IF(
                    @constraint_exists = 0,
                    'ALTER TABLE `{table}` ADD CONSTRAINT `{constraint}` FOREIGN KEY (`{column}`) REFERENCES `{principalTable}` (`{principalColumn}`) ON DELETE RESTRICT',
                    'SELECT 1'
                );
                PREPARE add_branch_department_users_fk FROM @statement;
                EXECUTE add_branch_department_users_fk;
                DEALLOCATE PREPARE add_branch_department_users_fk;
                """);
        }
    }
}

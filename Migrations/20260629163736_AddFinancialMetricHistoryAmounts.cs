using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialMetricHistoryAmounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "amount",
                table: "financial_metrics",
                newName: "current_amount");

            migrationBuilder.AddColumn<decimal>(
                name: "new_amount",
                table: "financial_metrics",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "old_amount",
                table: "financial_metrics",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(
                "UPDATE financial_metrics SET new_amount = current_amount, old_amount = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "new_amount",
                table: "financial_metrics");

            migrationBuilder.DropColumn(
                name: "old_amount",
                table: "financial_metrics");

            migrationBuilder.RenameColumn(
                name: "current_amount",
                table: "financial_metrics",
                newName: "amount");
        }
    }
}

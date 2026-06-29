using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gpos_core_mvc.Migrations
{
    /// <inheritdoc />
    public partial class MakeFinancialMetricsAppendOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_financial_metrics_metric_date_metric_code",
                table: "financial_metrics");

            migrationBuilder.CreateIndex(
                name: "IX_financial_metrics_metric_date_metric_code",
                table: "financial_metrics",
                columns: new[] { "metric_date", "metric_code" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_financial_metrics_metric_date_metric_code",
                table: "financial_metrics");

            migrationBuilder.CreateIndex(
                name: "IX_financial_metrics_metric_date_metric_code",
                table: "financial_metrics",
                columns: new[] { "metric_date", "metric_code" },
                unique: true);
        }
    }
}

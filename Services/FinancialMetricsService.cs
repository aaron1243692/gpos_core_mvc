using gpos.Data;
using gpos.Models;
using Microsoft.EntityFrameworkCore;

namespace gpos.Services
{
    public class FinancialMetricsService
    {
        private static readonly HashSet<string> SupportedMetricCodes = new(StringComparer.OrdinalIgnoreCase)
        {
            "gross_sales",
            "net_sales",
            "fuel_sales",
            "product_sales",
            "total_discount",
            "total_rebate",
            "total_points_redeemed",
            "total_returns",
            "cost_of_goods_sold",
            "gross_profit",
            "total_expenses",
            "net_profit",
            "sales_loss",
            "fuel_loss",
            "product_loss",
            "cash_expected",
            "cash_actual",
            "cash_shortage",
            "cash_overage"
        };

        private readonly ApplicationDbContext _db;

        public FinancialMetricsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddToMetric(DateTime date, string metricCode, decimal amount)
        {
            var normalizedCode = NormalizeMetricCode(metricCode);
            var metricDate = NormalizeMetricDate(date);
            var now = DateTime.UtcNow;
            var oldAmount = await GetLatestMetricAmount(metricDate, normalizedCode);
            var currentAmount = oldAmount + amount;

            var metric = new FinancialMetric
            {
                MetricCode = normalizedCode,
                OldAmount = oldAmount,
                NewAmount = amount,
                CurrentAmount = currentAmount,
                MetricDate = metricDate,
                CreatedAt = now,
                UpdatedAt = now
            };

            _db.FinancialMetrics.Add(metric);
            await _db.SaveChangesAsync();
        }

        public async Task SetMetric(DateTime date, string metricCode, decimal amount)
        {
            var normalizedCode = NormalizeMetricCode(metricCode);
            var metricDate = NormalizeMetricDate(date);
            var now = DateTime.UtcNow;

            _db.FinancialMetrics.Add(new FinancialMetric
            {
                MetricCode = normalizedCode,
                OldAmount = await GetLatestMetricAmount(metricDate, normalizedCode),
                NewAmount = amount,
                CurrentAmount = amount,
                MetricDate = metricDate,
                CreatedAt = now,
                UpdatedAt = now
            });

            await _db.SaveChangesAsync();
        }

        public async Task<decimal> GetMetricAmount(DateTime date, string metricCode)
        {
            var normalizedCode = NormalizeMetricCode(metricCode);
            var metricDate = NormalizeMetricDate(date);

            return await _db.FinancialMetrics
                .AsNoTracking()
                .Where(metric => metric.MetricDate == metricDate && metric.MetricCode == normalizedCode)
                .OrderByDescending(metric => metric.Id)
                .Select(metric => metric.CurrentAmount)
                .FirstOrDefaultAsync();
        }

        public async Task<decimal> GetMetricAmount(string metricCode)
        {
            var normalizedCode = NormalizeMetricCode(metricCode);

            return await _db.FinancialMetrics
                .AsNoTracking()
                .Where(metric => metric.MetricCode == normalizedCode)
                .OrderByDescending(metric => metric.MetricDate)
                .ThenByDescending(metric => metric.Id)
                .Select(metric => metric.CurrentAmount)
                .FirstOrDefaultAsync();
        }

        public static decimal ComputeGrossSales(decimal fuelSales, decimal productSales)
        {
            return fuelSales + productSales;
        }

        public static decimal ComputeNetSales(decimal grossSales, decimal discount, decimal rebate, decimal pointsRedeemed, decimal returns)
        {
            return grossSales - discount - rebate - pointsRedeemed - returns;
        }

        public static decimal ComputeGrossProfit(decimal netSales, decimal costOfGoodsSold)
        {
            return netSales - costOfGoodsSold;
        }

        public static decimal ComputeSalesLoss(decimal costOfGoodsSold, decimal netSales)
        {
            return Math.Max(costOfGoodsSold - netSales, 0m);
        }

        public static decimal ComputeCashShortage(decimal cashExpected, decimal cashActual)
        {
            return Math.Max(cashExpected - cashActual, 0m);
        }

        public static decimal ComputeCashOverage(decimal cashExpected, decimal cashActual)
        {
            return Math.Max(cashActual - cashExpected, 0m);
        }

        public static decimal ComputeNetProfit(decimal grossProfit, decimal totalExpenses, decimal fuelLoss, decimal productLoss, decimal cashShortage)
        {
            return grossProfit - totalExpenses - fuelLoss - productLoss - cashShortage;
        }

        private async Task<decimal> GetLatestMetricAmount(DateTime metricDate, string metricCode)
        {
            return await _db.FinancialMetrics
                .AsNoTracking()
                .Where(metric => metric.MetricDate == metricDate && metric.MetricCode == metricCode)
                .OrderByDescending(metric => metric.Id)
                .Select(metric => metric.CurrentAmount)
                .FirstOrDefaultAsync();
        }

        private static DateTime NormalizeMetricDate(DateTime date)
        {
            return date.Date;
        }

        private static string NormalizeMetricCode(string metricCode)
        {
            var normalizedCode = (metricCode ?? string.Empty).Trim().ToLowerInvariant();
            if (!SupportedMetricCodes.Contains(normalizedCode))
            {
                throw new ArgumentException("Unsupported financial metric code.", nameof(metricCode));
            }

            return normalizedCode;
        }
    }
}

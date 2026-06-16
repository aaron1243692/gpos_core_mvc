using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class ReportsController : Controller
    {
        public IActionResult DailySalesReport() => View();
        public IActionResult DailySales() => View();
        public IActionResult ShiftSalesReport() => View();
        public IActionResult ShiftReport() => View();
        public IActionResult FuelSalesReport() => View();
        public IActionResult FuelSales() => View();
        public IActionResult ProductSalesReport() => View();
        public IActionResult ProductSales() => View();
        public IActionResult CashierSalesReport() => View();
        public IActionResult CashReport() => View();
        public IActionResult PaymentMethodReport() => View();
        public IActionResult DiscountReport() => View();
        public IActionResult PointsReport() => View();
        public IActionResult CustomerReport() => View();
        public IActionResult InventoryReport() => View();
        public IActionResult FuelTankReport() => View();
        public IActionResult FuelBatchReport() => View();
        public IActionResult ItemBatchReport() => View();
        public IActionResult CashDifferenceReport() => View();
        public IActionResult VoidTransactionReport() => View();
    }
}

using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class TransactionController : Controller
    {
        public IActionResult OpenShift() => View();
        public IActionResult DailyBeginningBalance() => View();
        public IActionResult CashDrawerMovement() => View();
        public IActionResult PumpTransaction() => View();
        public IActionResult PosProductSales() => View();
        public IActionResult ProductSales() => View();
        public IActionResult FuelSales() => View();
        public IActionResult EmployeePOS() => View();
        public IActionResult EmployeeProductSales() => View();
        public IActionResult EmployeeFuelSales() => View();
        public IActionResult DailyCash() => View();
        public IActionResult WarehouseTransfer() => View();
        public IActionResult DisplayTransfer() => View();
        public IActionResult FuelTransfer() => View();
        public IActionResult WarehouseAdjustment() => View();
        public IActionResult ProductAdjustment() => View();
        public IActionResult FuelAdjustment() => View();
        public IActionResult WarehouseReturn() => View();
        public IActionResult ProductReturn() => View();
        public IActionResult FuelReturn() => View();
        public IActionResult WarehouseVoid() => View();
        public IActionResult ProductVoid() => View();
        public IActionResult FuelVoid() => View();
        public IActionResult TransactionPayments() => View();
        public IActionResult PointsRedemption() => View();
        public IActionResult DailyClosingBalance() => View();
        public IActionResult CashTurnover() => View();
        public IActionResult VoidTransactions() => View();
        public IActionResult ReceiptPreview() => View();
    }
}

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
        public IActionResult TransactionPayments() => View();
        public IActionResult PointsRedemption() => View();
        public IActionResult DailyClosingBalance() => View();
        public IActionResult CashTurnover() => View();
        public IActionResult VoidTransactions() => View();
        public IActionResult ReceiptPreview() => View();
    }
}

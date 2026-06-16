using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class TransactionsController : Controller
    {
        public IActionResult OpenShift() => View();
        public IActionResult CloseShift() => View();
        public IActionResult FuelTransactions() => View();
        public IActionResult ProductSales() => View();
        public IActionResult CashIn() => View();
        public IActionResult CashOut() => View();
    }
}

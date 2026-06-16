using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class DashboardController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Admin() => RedirectToAction(nameof(Index));
        public IActionResult Manager() => RedirectToAction(nameof(Index));
        public IActionResult Cashier() => RedirectToAction(nameof(Index));
        public IActionResult Inventory() => RedirectToAction(nameof(Index));
    }
}

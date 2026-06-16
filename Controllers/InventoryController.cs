using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class InventoryController : Controller
    {
        public IActionResult DailyStock() => RedirectToAction("WarehouseDailyStock", "Reports");
        public IActionResult WarehouseDailyStock() => RedirectToAction("WarehouseDailyStock", "Reports");
        public IActionResult DisplayDailyStock() => RedirectToAction("DisplayDailyStock", "Reports");
        public IActionResult TankDailyStock() => RedirectToAction("TankDailyStock", "Reports");
    }
}

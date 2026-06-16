using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class SetupController : Controller
    {
        public IActionResult Dashboard() => RedirectToAction("Index", "Dashboard");
        public IActionResult Users() => View();
        public IActionResult Customers() => View();
        public IActionResult Categories() => View();
        public IActionResult ItemUnits() => View();
        public IActionResult FuelUnits() => View();
        public IActionResult Products() => View();
        public IActionResult FuelTypes() => View();
        public IActionResult FuelPriceHistory() => View();
        public IActionResult PumpUnits() => View();
        public IActionResult Pumps() => View();
        public IActionResult FuelTanks() => View();
        public IActionResult Suppliers() => View();
        public IActionResult Discounts() => View();
        public IActionResult Settings() => View();
        public IActionResult Profile() => View();
    }
}

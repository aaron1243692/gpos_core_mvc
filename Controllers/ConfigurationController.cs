using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class ConfigurationController : Controller
    {
        public IActionResult Products() => View();
        public IActionResult ProductCategories() => View();
        public IActionResult ItemUnits() => View();
        public IActionResult FuelTypes() => View();
        public IActionResult FuelUnits() => View();
        public IActionResult FuelPrices() => View();
        public IActionResult Pumps() => View();
        public IActionResult PumpUnits() => View();
        public IActionResult Tanks() => View();
        public IActionResult Discounts() => View();
    }
}

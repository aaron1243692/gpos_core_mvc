using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class ConfigurationController : Controller
    {
        public IActionResult Products() => View();
        public IActionResult DisplayProducts() => View();
        public IActionResult WarehouseProducts() => View();
        public IActionResult ProductCategories() => View();
        public IActionResult ItemUnits() => View();
        public IActionResult Fuels() => View();
        public IActionResult FuelTypes() => View();
        public IActionResult FuelUnits() => View();
        public IActionResult FuelPrices() => View();
        public IActionResult Pumps() => View();
        public IActionResult PumpUnits() => View();
        public IActionResult Tanks() => View();
        public IActionResult Discounts() => View();
        public IActionResult Members() => View();
        public IActionResult Rebate() => View();
        public IActionResult Position() => View();
    }
}

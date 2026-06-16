using gpos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class InventoryController : Controller
    {
        public IActionResult WarehouseInventory() => View();
        public IActionResult DisplayInventory() => View();
        public IActionResult ProductsStock() => View();
        public IActionResult WarehouseStock() => View();
        public IActionResult DisplayStock() => View();
        public IActionResult DailyStock() => View();
        public IActionResult WarehouseDailyStock() => View();
        public IActionResult DisplayDailyStock() => View();
        public IActionResult TankDailyStock() => View();
        public IActionResult ItemBatches() => View();
        public IActionResult FuelBatches() => View();
        public IActionResult StockReceiving() => View();
        public IActionResult StockTransfer() => View();
        public IActionResult StockAdjustment() => View();
        public IActionResult FuelInventory() => View();
        public IActionResult FuelDelivery() => View();
        public IActionResult FuelInventoryMovement() => View();
        public IActionResult ItemInventoryMovement() => View();
        public IActionResult LowStockItems() => View();
    }
}

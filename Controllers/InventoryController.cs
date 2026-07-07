using gpos.Data;
using gpos.Filters;
using gpos.Models;
using gpos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class InventoryController : Controller
    {
        private const string WarehouseStockType = "Warehouse";
        private const string DisplayStockType = "Display";
        private const string FuelStockType = "Fuel";
        private const string LowStockModalId = "lowStockSettingModal";

        private readonly ApplicationDbContext _db;

        public InventoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult DailyStock() => RedirectToAction("WarehouseDailyStock", "Reports");
        public IActionResult WarehouseDailyStock() => RedirectToAction("WarehouseDailyStock", "Reports");
        public IActionResult DisplayDailyStock() => RedirectToAction("DisplayDailyStock", "Reports");
        public IActionResult TankDailyStock() => RedirectToAction("TankDailyStock", "Reports");

        public async Task<IActionResult> LowStockWarehouse(string? search, int? editId)
        {
            return View("LowStockSettings", await BuildLowStockSettingsPageAsync(WarehouseStockType, search, editId: editId, activeModalId: editId.HasValue ? LowStockModalId : string.Empty));
        }

        public async Task<IActionResult> LowStockDisplay(string? search, int? editId)
        {
            return View("LowStockSettings", await BuildLowStockSettingsPageAsync(DisplayStockType, search, editId: editId, activeModalId: editId.HasValue ? LowStockModalId : string.Empty));
        }

        public async Task<IActionResult> LowStockFuel(string? search, int? editId)
        {
            return View("LowStockSettings", await BuildLowStockSettingsPageAsync(FuelStockType, search, editId: editId, activeModalId: editId.HasValue ? LowStockModalId : string.Empty));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLowStockWarehouse([Bind(Prefix = "Form")] LowStockThresholdForm form, string? search)
        {
            return await SaveLowStockSettingAsync(WarehouseStockType, form, search, nameof(LowStockWarehouse));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLowStockDisplay([Bind(Prefix = "Form")] LowStockThresholdForm form, string? search)
        {
            return await SaveLowStockSettingAsync(DisplayStockType, form, search, nameof(LowStockDisplay));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLowStockFuel([Bind(Prefix = "Form")] LowStockThresholdForm form, string? search)
        {
            return await SaveLowStockSettingAsync(FuelStockType, form, search, nameof(LowStockFuel));
        }

        private async Task<IActionResult> SaveLowStockSettingAsync(string stockType, LowStockThresholdForm form, string? search, string redirectAction)
        {
            form.StockType = stockType;
            form.UnitLabel = UnitLabelFor(stockType);
            ValidateLowStockForm(stockType, form);

            if (!ModelState.IsValid)
            {
                return View("LowStockSettings", await BuildLowStockSettingsPageAsync(stockType, search, form, activeModalId: LowStockModalId));
            }

            var now = DateTime.UtcNow;
            LowStockSetting? setting;

            if (form.Id > 0)
            {
                setting = await _db.LowStockSettings.FirstOrDefaultAsync(item => item.Id == form.Id && item.Location == stockType && item.Status == 1);
                if (setting is null)
                {
                    return NotFound();
                }
            }
            else
            {
                setting = new LowStockSetting
                {
                    Location = stockType,
                    CreatedAt = now,
                    Status = 1
                };
                _db.LowStockSettings.Add(setting);
            }

            if (stockType == FuelStockType)
            {
                setting.ProductId = null;
                setting.ProductBatchId = null;
                setting.TankId = form.Id > 0 ? setting.TankId : form.TankId;
            }
            else
            {
                setting.ProductId = form.Id > 0 ? setting.ProductId : form.ProductId;
                setting.ProductBatchId = null;
                setting.TankId = null;
            }

            setting.Location = stockType;
            setting.MinimumQuantity = form.Threshold!.Value;
            setting.UnitLabel = UnitLabelFor(stockType);
            setting.Status = 1;
            setting.UpdatedAt = now;

            await _db.SaveChangesAsync();
            return RedirectToAction(redirectAction, new { search });
        }

        private void ValidateLowStockForm(string stockType, LowStockThresholdForm form)
        {
            if (stockType == FuelStockType)
            {
                if (form.Id == 0 && (!form.TankId.HasValue || form.TankId.Value <= 0))
                {
                    ModelState.AddModelError("Form.TankId", "Tank is required.");
                }

                if (form.Id == 0 && form.TankId.HasValue)
                {
                    var duplicateExists = _db.LowStockSettings.Any(setting => setting.Location == stockType
                        && setting.Status == 1
                        && setting.TankId == form.TankId
                        && setting.Id != form.Id);

                    if (duplicateExists)
                    {
                        ModelState.AddModelError("Form.TankId", "An active low stock setting already exists for this tank.");
                    }
                }

                return;
            }

            if (form.Id == 0 && (!form.ProductId.HasValue || form.ProductId.Value <= 0))
            {
                ModelState.AddModelError("Form.ProductId", "Product is required.");
            }

            if (form.Id == 0 && form.ProductId.HasValue)
            {
                var duplicateExists = _db.LowStockSettings.Any(setting => setting.Location == stockType
                    && setting.Status == 1
                    && setting.ProductId == form.ProductId
                    && setting.Id != form.Id);

                if (duplicateExists)
                {
                    ModelState.AddModelError("Form.ProductId", "An active low stock setting already exists for this product.");
                }
            }
        }

        private async Task<LowStockSettingsPageViewModel> BuildLowStockSettingsPageAsync(string stockType, string? search, LowStockThresholdForm? form = null, int? editId = null, string activeModalId = "")
        {
            var searchText = (search ?? string.Empty).Trim();
            var rows = await BuildSettingRowsAsync(stockType);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                rows = rows
                    .Where(row => row.ProductName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                        || row.TankName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                        || row.FuelName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                        || row.Status.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return new LowStockSettingsPageViewModel
            {
                StockType = stockType,
                PageTitle = $"{stockType} Low Stock Settings",
                Description = DescriptionFor(stockType),
                UnitLabel = UnitLabelFor(stockType),
                QuantityLabel = QuantityLabelFor(stockType),
                SaveAction = SaveActionFor(stockType),
                PageAction = PageActionFor(stockType),
                EmptyMessage = "No low stock settings found.",
                Search = searchText,
                ActiveModalId = activeModalId,
                Form = form ?? await BuildLowStockFormAsync(stockType, editId),
                ProductOptions = await BuildProductOptionsAsync(),
                TankOptions = await BuildTankOptionsAsync(),
                Settings = rows
            };
        }

        private async Task<LowStockThresholdForm> BuildLowStockFormAsync(string stockType, int? editId)
        {
            if (!editId.HasValue)
            {
                return new LowStockThresholdForm
                {
                    StockType = stockType,
                    UnitLabel = UnitLabelFor(stockType)
                };
            }

            var setting = await _db.LowStockSettings.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value && item.Location == stockType);
            if (setting is null)
            {
                return new LowStockThresholdForm
                {
                    StockType = stockType,
                    UnitLabel = UnitLabelFor(stockType)
                };
            }

            return new LowStockThresholdForm
            {
                Id = setting.Id,
                StockType = stockType,
                ProductId = setting.ProductId,
                TankId = setting.TankId,
                Threshold = setting.MinimumQuantity,
                UnitLabel = setting.UnitLabel ?? UnitLabelFor(stockType)
            };
        }

        private async Task<List<LowStockSettingRow>> BuildSettingRowsAsync(string stockType)
        {
            var settings = await _db.LowStockSettings
                .AsNoTracking()
                .Include(setting => setting.Product)
                .Include(setting => setting.Tank)
                .ThenInclude(tank => tank!.Fuel)
                .Where(setting => setting.Location == stockType
                    && setting.Status == 1
                    && (stockType == FuelStockType ? setting.TankId != null : setting.ProductId != null))
                .OrderBy(setting => stockType == FuelStockType ? setting.Tank!.TankNo : setting.Product!.Name)
                .ToListAsync();

            if (stockType == FuelStockType)
            {
                return settings.Select(setting =>
                {
                    var currentLiters = setting.Tank?.CurrentLiters ?? 0m;
                    return new LowStockSettingRow
                    {
                        Id = setting.Id,
                        TankId = setting.TankId,
                        TankName = setting.Tank?.TankNo ?? "-",
                        FuelName = setting.Tank?.Fuel?.Name ?? "-",
                        UnitLabel = setting.UnitLabel ?? UnitLabelFor(stockType),
                        CurrentQuantity = currentLiters,
                        Threshold = setting.MinimumQuantity,
                        Status = StatusFor(currentLiters, setting.MinimumQuantity),
                        UpdatedAt = setting.UpdatedAt
                    };
                }).ToList();
            }

            var productIds = settings
                .Where(setting => setting.ProductId.HasValue)
                .Select(setting => setting.ProductId!.Value)
                .Distinct()
                .ToList();

            var totals = stockType == WarehouseStockType
                ? await _db.WarehouseStocks.AsNoTracking()
                    .Where(stock => productIds.Contains(stock.ProductId))
                    .GroupBy(stock => stock.ProductId)
                    .Select(group => new { ProductId = group.Key, Quantity = group.Sum(stock => stock.Quantity) })
                    .ToDictionaryAsync(item => item.ProductId, item => item.Quantity)
                : await _db.DisplayStocks.AsNoTracking()
                    .Where(stock => productIds.Contains(stock.ProductId))
                    .GroupBy(stock => stock.ProductId)
                    .Select(group => new { ProductId = group.Key, Quantity = group.Sum(stock => stock.Quantity) })
                    .ToDictionaryAsync(item => item.ProductId, item => item.Quantity);

            return settings.Select(setting =>
            {
                var currentQuantity = setting.ProductId.HasValue && totals.TryGetValue(setting.ProductId.Value, out var total) ? total : 0m;
                return new LowStockSettingRow
                {
                    Id = setting.Id,
                    ProductId = setting.ProductId,
                    ProductName = setting.Product?.Name ?? "-",
                    UnitLabel = setting.UnitLabel ?? UnitLabelFor(stockType),
                    CurrentQuantity = currentQuantity,
                    Threshold = setting.MinimumQuantity,
                    Status = StatusFor(currentQuantity, setting.MinimumQuantity),
                    UpdatedAt = setting.UpdatedAt
                };
            }).ToList();
        }

        private async Task<List<SelectListItem>> BuildProductOptionsAsync()
        {
            return await _db.Products
                .AsNoTracking()
                .Where(product => product.Status == 1 && product.IsActive)
                .OrderBy(product => product.Name)
                .Select(product => new SelectListItem { Value = product.Id.ToString(), Text = product.Name })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> BuildTankOptionsAsync()
        {
            return await _db.Tanks
                .AsNoTracking()
                .Include(tank => tank.Fuel)
                .Where(tank => tank.Status == 1 && tank.IsActive)
                .OrderBy(tank => tank.TankNo)
                .Select(tank => new SelectListItem { Value = tank.Id.ToString(), Text = tank.Fuel == null ? tank.TankNo : $"{tank.TankNo} - {tank.Fuel.Name}" })
                .ToListAsync();
        }

        private static string DescriptionFor(string stockType) => stockType switch
        {
            WarehouseStockType => "Set minimum quantities per product for warehouse stock warnings.",
            DisplayStockType => "Set minimum quantities per product for display stock warnings.",
            FuelStockType => "Set minimum liters per tank for fuel stock warnings.",
            _ => "Set low stock thresholds."
        };

        private static string UnitLabelFor(string stockType) => stockType == FuelStockType ? "Liters" : "Units";

        private static string QuantityLabelFor(string stockType) => stockType switch
        {
            WarehouseStockType => "Current Total Quantity",
            DisplayStockType => "Current Total Quantity",
            FuelStockType => "Current Liters",
            _ => "Current Quantity"
        };

        private static string SaveActionFor(string stockType) => stockType switch
        {
            WarehouseStockType => nameof(SaveLowStockWarehouse),
            DisplayStockType => nameof(SaveLowStockDisplay),
            FuelStockType => nameof(SaveLowStockFuel),
            _ => nameof(SaveLowStockWarehouse)
        };

        private static string PageActionFor(string stockType) => stockType switch
        {
            WarehouseStockType => nameof(LowStockWarehouse),
            DisplayStockType => nameof(LowStockDisplay),
            FuelStockType => nameof(LowStockFuel),
            _ => nameof(LowStockWarehouse)
        };

        private static string StatusFor(decimal currentQuantity, decimal threshold) => currentQuantity <= threshold ? "Low Stock" : "OK";
    }
}

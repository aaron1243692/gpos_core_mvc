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

        public async Task<IActionResult> LowStockWarehouse(string? search, int? editId, int? branchId, int? filterBranchId)
        {
            branchId = filterBranchId ?? branchId;
            return View("LowStockSettings", await BuildLowStockSettingsPageAsync(WarehouseStockType, search, branchId, editId: editId, activeModalId: editId.HasValue ? LowStockModalId : string.Empty));
        }

        public async Task<IActionResult> LowStockDisplay(string? search, int? editId, int? branchId, int? filterBranchId)
        {
            branchId = filterBranchId ?? branchId;
            return View("LowStockSettings", await BuildLowStockSettingsPageAsync(DisplayStockType, search, branchId, editId: editId, activeModalId: editId.HasValue ? LowStockModalId : string.Empty));
        }

        public async Task<IActionResult> LowStockFuel(string? search, int? editId, int? branchId, int? filterBranchId)
        {
            branchId = filterBranchId ?? branchId;
            return View("LowStockSettings", await BuildLowStockSettingsPageAsync(FuelStockType, search, branchId, editId: editId, activeModalId: editId.HasValue ? LowStockModalId : string.Empty));
        }

        [HttpGet]
        public async Task<IActionResult> SearchWarehouseLowStockProducts(string? search, int? branchId, int take = 20)
        {
            var searchText = (search ?? string.Empty).Trim();
            var resultLimit = Math.Clamp(take, 1, 50);
            var query = _db.Products
                .AsNoTracking()
                .Include(product => product.Category)
                .Include(product => product.ProductUnit)
                .Where(product => product.Status == 1 && product.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(product =>
                    product.Name.Contains(searchText)
                    || (product.Category != null && product.Category.Name.Contains(searchText))
                    || (product.ProductUnit != null && product.ProductUnit.Name.Contains(searchText)));
            }

            var products = await query
                .OrderBy(product => product.Name)
                .Take(resultLimit)
                .Select(product => new
                {
                    productId = product.Id,
                    productName = product.Name,
                    categoryName = product.Category != null ? product.Category.Name : "-",
                    unitName = product.ProductUnit != null ? product.ProductUnit.Name : "-",
                    totalWarehouseQuantity = _db.WarehouseStocks
                        .Where(stock => stock.ProductId == product.Id
                            && (!branchId.HasValue || branchId.Value <= 0 || stock.BranchId == branchId.Value))
                        .Sum(stock => (decimal?)stock.Quantity) ?? 0m,
                    thresholdQuantity = _db.LowStockSettings
                        .Where(setting => setting.Location == WarehouseStockType
                            && setting.Status == 1
                            && setting.ProductId == product.Id
                            && setting.BranchId == branchId)
                        .Select(setting => (decimal?)setting.MinimumQuantity)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Json(products);
        }

        [HttpGet]
        public async Task<IActionResult> SearchDisplayLowStockProducts(string? search, int? branchId, int take = 20)
        {
            var searchText = (search ?? string.Empty).Trim();
            var resultLimit = Math.Clamp(take, 1, 50);
            var query = _db.Products
                .AsNoTracking()
                .Include(product => product.Category)
                .Include(product => product.ProductUnit)
                .Where(product => product.Status == 1 && product.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(product =>
                    product.Name.Contains(searchText)
                    || (product.Category != null && product.Category.Name.Contains(searchText))
                    || (product.ProductUnit != null && product.ProductUnit.Name.Contains(searchText)));
            }

            var products = await query
                .OrderBy(product => product.Name)
                .Take(resultLimit)
                .Select(product => new
                {
                    productId = product.Id,
                    productName = product.Name,
                    categoryName = product.Category != null ? product.Category.Name : "-",
                    unitName = product.ProductUnit != null ? product.ProductUnit.Name : "-",
                    totalDisplayQuantity = _db.DisplayStocks
                        .Where(stock => stock.ProductId == product.Id
                            && (!branchId.HasValue || branchId.Value <= 0 || stock.BranchId == branchId.Value))
                        .Sum(stock => (decimal?)stock.Quantity) ?? 0m,
                    thresholdQuantity = _db.LowStockSettings
                        .Where(setting => setting.Location == DisplayStockType
                            && setting.Status == 1
                            && setting.ProductId == product.Id
                            && setting.BranchId == branchId)
                        .Select(setting => (decimal?)setting.MinimumQuantity)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Json(products);
        }

        [HttpGet]
        public async Task<IActionResult> SearchLowStockTanks(string? search, int? branchId, int take = 20)
        {
            var searchText = (search ?? string.Empty).Trim();
            var resultLimit = Math.Clamp(take, 1, 50);
            var query = _db.Tanks
                .AsNoTracking()
                .Include(tank => tank.Branch)
                .Include(tank => tank.Fuel)
                .Where(tank => tank.Status == 1 && tank.IsActive)
                .AsQueryable();

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(tank => tank.BranchId == branchId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(tank =>
                    tank.TankNo.Contains(searchText)
                    || (tank.Fuel != null && tank.Fuel.Name.Contains(searchText))
                    || (tank.Branch != null && tank.Branch.Name.Contains(searchText)));
            }

            var tanks = await query
                .OrderBy(tank => tank.Branch != null ? tank.Branch.Name : string.Empty)
                .ThenBy(tank => tank.TankNo)
                .Take(resultLimit)
                .Select(tank => new
                {
                    tankId = tank.Id,
                    tankName = tank.TankNo,
                    fuelId = tank.FuelId,
                    fuelName = tank.Fuel != null ? tank.Fuel.Name : "-",
                    branchId = tank.BranchId,
                    branchName = tank.Branch != null ? tank.Branch.Name : "Unassigned",
                    currentLiters = tank.CurrentLiters,
                    thresholdLiters = _db.LowStockSettings
                        .Where(setting => setting.Location == FuelStockType
                            && setting.Status == 1
                            && setting.TankId == tank.Id
                            && setting.BranchId == tank.BranchId)
                        .Select(setting => (decimal?)setting.MinimumQuantity)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Json(tanks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLowStockWarehouse([Bind(Prefix = "Form")] LowStockThresholdForm form, string? search, int? filterBranchId)
        {
            return await SaveLowStockSettingAsync(WarehouseStockType, form, search, filterBranchId, nameof(LowStockWarehouse));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLowStockDisplay([Bind(Prefix = "Form")] LowStockThresholdForm form, string? search, int? filterBranchId)
        {
            return await SaveLowStockSettingAsync(DisplayStockType, form, search, filterBranchId, nameof(LowStockDisplay));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLowStockFuel([Bind(Prefix = "Form")] LowStockThresholdForm form, string? search, int? filterBranchId)
        {
            return await SaveLowStockSettingAsync(FuelStockType, form, search, filterBranchId, nameof(LowStockFuel));
        }

        private async Task<IActionResult> SaveLowStockSettingAsync(string stockType, LowStockThresholdForm form, string? search, int? filterBranchId, string redirectAction)
        {
            form.StockType = stockType;
            form.UnitLabel = UnitLabelFor(stockType);
            ValidateLowStockForm(stockType, form);

            if (!ModelState.IsValid)
            {
                return View("LowStockSettings", await BuildLowStockSettingsPageAsync(stockType, search, filterBranchId, form, activeModalId: LowStockModalId));
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
                setting.BranchId = form.BranchId;
                setting.TankId = form.TankId;
            }
            else if (stockType == WarehouseStockType)
            {
                setting.ProductId = form.ProductId;
                setting.ProductBatchId = null;
                setting.BranchId = form.BranchId;
                setting.TankId = null;
            }
            else
            {
                setting.ProductId = form.ProductId;
                setting.ProductBatchId = null;
                setting.BranchId = form.BranchId;
                setting.TankId = null;
            }

            setting.Location = stockType;
            setting.MinimumQuantity = form.Threshold!.Value;
            setting.UnitLabel = UnitLabelFor(stockType);
            setting.Status = 1;
            setting.UpdatedAt = now;

            await _db.SaveChangesAsync();
            return RedirectToAction(redirectAction, new { search, filterBranchId });
        }

        private void ValidateLowStockForm(string stockType, LowStockThresholdForm form)
        {
            if (stockType == FuelStockType)
            {
                if (!form.BranchId.HasValue || form.BranchId.Value <= 0)
                {
                    ModelState.AddModelError("Form.BranchId", "Branch is required.");
                    return;
                }

                if (!form.TankId.HasValue || form.TankId.Value <= 0)
                {
                    ModelState.AddModelError("Form.TankId", "Tank is required.");
                    return;
                }

                var tankExists = _db.Tanks.Any(tank => tank.Id == form.TankId && tank.BranchId == form.BranchId && tank.Status == 1 && tank.IsActive);
                if (!tankExists)
                {
                    ModelState.AddModelError("Form.TankId", "Select a valid tank for the selected branch.");
                    return;
                }

                if (form.TankId.HasValue)
                {
                    var duplicateExists = _db.LowStockSettings.Any(setting => setting.Location == stockType
                        && setting.Status == 1
                        && setting.BranchId == form.BranchId
                        && setting.TankId == form.TankId
                        && setting.Id != form.Id);

                    if (duplicateExists)
                    {
                        ModelState.AddModelError("Form.TankId", "An active low stock setting already exists for this tank.");
                    }
                }

                return;
            }

            if (stockType == WarehouseStockType || stockType == DisplayStockType)
            {
                if (!form.BranchId.HasValue || form.BranchId.Value <= 0)
                {
                    ModelState.AddModelError("Form.BranchId", "Branch is required.");
                    return;
                }

                if (!form.ProductId.HasValue || form.ProductId.Value <= 0)
                {
                    ModelState.AddModelError("Form.ProductId", "Product is required.");
                    return;
                }

                var branchExists = _db.Branches.Any(branch => branch.Id == form.BranchId && branch.Status == 1);
                if (!branchExists)
                {
                    ModelState.AddModelError("Form.BranchId", "Select a valid branch.");
                    return;
                }

                var productExists = _db.Products.Any(product => product.Id == form.ProductId && product.Status == 1 && product.IsActive);
                if (!productExists)
                {
                    ModelState.AddModelError("Form.ProductId", "Select a valid product.");
                    return;
                }

                var duplicateExists = _db.LowStockSettings.Any(setting => setting.Location == stockType
                    && setting.Status == 1
                    && setting.ProductId == form.ProductId
                    && setting.BranchId == form.BranchId
                    && setting.Id != form.Id);

                if (duplicateExists)
                {
                    ModelState.AddModelError("Form.ProductId", $"An active low stock setting already exists for this branch and product.");
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

        private async Task<LowStockSettingsPageViewModel> BuildLowStockSettingsPageAsync(string stockType, string? search, int? branchId = null, LowStockThresholdForm? form = null, int? editId = null, string activeModalId = "")
        {
            var searchText = (search ?? string.Empty).Trim();
            var rows = await BuildSettingRowsAsync(stockType);
            if ((stockType == WarehouseStockType || stockType == DisplayStockType || stockType == FuelStockType) && branchId.HasValue && branchId.Value > 0)
            {
                rows = rows.Where(row => row.BranchId == branchId.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                rows = rows
                    .Where(row => row.ProductName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                        || row.BranchName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
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
                BranchId = branchId,
                ActiveModalId = activeModalId,
                Form = form ?? await BuildLowStockFormAsync(stockType, editId),
                BranchOptions = await BuildBranchFilterOptionsAsync(),
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
                BranchId = setting.BranchId,
                BranchName = await BranchNameAsync(setting.BranchId),
                SelectedProductName = await ProductNameAsync(setting.ProductId),
                TankId = setting.TankId,
                SelectedTankName = await TankNameAsync(setting.TankId),
                SelectedFuelName = await TankFuelNameAsync(setting.TankId),
                Threshold = setting.MinimumQuantity,
                UnitLabel = setting.UnitLabel ?? UnitLabelFor(stockType)
            };
        }

        private async Task<List<LowStockSettingRow>> BuildSettingRowsAsync(string stockType)
        {
            var settings = await _db.LowStockSettings
                .AsNoTracking()
                .Include(setting => setting.Branch)
                .Include(setting => setting.Product)
                .Include(setting => setting.Tank)
                .ThenInclude(tank => tank!.Fuel)
                .Include(setting => setting.Tank)
                .ThenInclude(tank => tank!.Branch)
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
                    var difference = setting.MinimumQuantity - currentLiters;
                    return new LowStockSettingRow
                    {
                        Id = setting.Id,
                        BranchId = setting.BranchId ?? setting.Tank?.BranchId,
                        BranchName = setting.Branch?.Name ?? setting.Tank?.Branch?.Name ?? "Unassigned",
                        TankId = setting.TankId,
                        TankName = setting.Tank?.TankNo ?? "-",
                        FuelName = setting.Tank?.Fuel?.Name ?? "-",
                        UnitLabel = setting.UnitLabel ?? UnitLabelFor(stockType),
                        CurrentQuantity = currentLiters,
                        Threshold = setting.MinimumQuantity,
                        Difference = difference,
                        Status = StatusFor(currentLiters, setting.MinimumQuantity),
                        UpdatedAt = setting.UpdatedAt
                    };
                }).ToList();
            }

            if (stockType == WarehouseStockType)
            {
                return settings.Select(setting =>
                {
                    var currentQuantity = _db.WarehouseStocks.AsNoTracking()
                        .Where(stock => stock.ProductId == setting.ProductId && stock.BranchId == setting.BranchId)
                        .Sum(stock => (decimal?)stock.Quantity) ?? 0m;
                    var difference = setting.MinimumQuantity - currentQuantity;

                    return new LowStockSettingRow
                    {
                        Id = setting.Id,
                        ProductId = setting.ProductId,
                        BranchId = setting.BranchId,
                        BranchName = setting.Branch?.Name ?? "Unassigned",
                        ProductName = setting.Product?.Name ?? "-",
                        UnitLabel = setting.UnitLabel ?? UnitLabelFor(stockType),
                        CurrentQuantity = currentQuantity,
                        Threshold = setting.MinimumQuantity,
                        Difference = difference,
                        Status = StatusFor(currentQuantity, setting.MinimumQuantity),
                        UpdatedAt = setting.UpdatedAt
                    };
                }).ToList();
            }

            if (stockType == DisplayStockType)
            {
                return settings.Select(setting =>
                {
                    var currentQuantity = _db.DisplayStocks.AsNoTracking()
                        .Where(stock => stock.ProductId == setting.ProductId && stock.BranchId == setting.BranchId)
                        .Sum(stock => (decimal?)stock.Quantity) ?? 0m;
                    var difference = setting.MinimumQuantity - currentQuantity;

                    return new LowStockSettingRow
                    {
                        Id = setting.Id,
                        ProductId = setting.ProductId,
                        BranchId = setting.BranchId,
                        BranchName = setting.Branch?.Name ?? "Unassigned",
                        ProductName = setting.Product?.Name ?? "-",
                        UnitLabel = setting.UnitLabel ?? UnitLabelFor(stockType),
                        CurrentQuantity = currentQuantity,
                        Threshold = setting.MinimumQuantity,
                        Difference = difference,
                        Status = StatusFor(currentQuantity, setting.MinimumQuantity),
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
                var difference = setting.MinimumQuantity - currentQuantity;
                return new LowStockSettingRow
                {
                    Id = setting.Id,
                    ProductId = setting.ProductId,
                    ProductName = setting.Product?.Name ?? "-",
                    UnitLabel = setting.UnitLabel ?? UnitLabelFor(stockType),
                    CurrentQuantity = currentQuantity,
                    Threshold = setting.MinimumQuantity,
                    Difference = difference,
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

        private async Task<List<SelectListItem>> BuildBranchFilterOptionsAsync()
        {
            var options = await _db.Branches
                .AsNoTracking()
                .Where(branch => branch.Status == 1)
                .OrderBy(branch => branch.Name)
                .Select(branch => new SelectListItem { Value = branch.Id.ToString(), Text = branch.Name })
                .ToListAsync();
            options.Insert(0, new SelectListItem { Value = "", Text = "All Branches" });
            return options;
        }

        private async Task<string> BranchNameAsync(int? branchId)
        {
            if (!branchId.HasValue || branchId.Value <= 0)
            {
                return string.Empty;
            }

            return await _db.Branches
                .AsNoTracking()
                .Where(branch => branch.Id == branchId.Value)
                .Select(branch => branch.Name)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        private async Task<string> ProductNameAsync(int? productId)
        {
            if (!productId.HasValue || productId.Value <= 0)
            {
                return string.Empty;
            }

            return await _db.Products
                .AsNoTracking()
                .Where(product => product.Id == productId.Value)
                .Select(product => product.Name)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        private async Task<string> TankNameAsync(int? tankId)
        {
            if (!tankId.HasValue || tankId.Value <= 0)
            {
                return string.Empty;
            }

            return await _db.Tanks
                .AsNoTracking()
                .Where(tank => tank.Id == tankId.Value)
                .Select(tank => tank.TankNo)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        private async Task<string> TankFuelNameAsync(int? tankId)
        {
            if (!tankId.HasValue || tankId.Value <= 0)
            {
                return string.Empty;
            }

            return await _db.Tanks
                .AsNoTracking()
                .Include(tank => tank.Fuel)
                .Where(tank => tank.Id == tankId.Value)
                .Select(tank => tank.Fuel != null ? tank.Fuel.Name : string.Empty)
                .FirstOrDefaultAsync() ?? string.Empty;
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

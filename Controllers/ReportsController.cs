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
    public class ReportsController : Controller
    {
        private const string WarehouseStockType = "Warehouse";
        private const string DisplayStockType = "Display";
        private const string TankStockType = "Tank";
        private const string DailyStockModalId = "dailyStockModal";

        private readonly ApplicationDbContext _db;

        public ReportsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult DailySalesReport() => View();
        public IActionResult DailySales() => View();
        public IActionResult ShiftSalesReport() => View();
        public IActionResult ShiftReport() => View();
        public IActionResult FuelSalesReport() => View();
        public IActionResult FuelSales() => View();
        public IActionResult ProductSalesReport() => View();
        public IActionResult ProductSales() => View();
        public IActionResult CashierSalesReport() => View();
        public IActionResult CashReport() => View();
        public IActionResult PaymentMethodReport() => View();
        public IActionResult DiscountReport() => View();
        public IActionResult PointsReport() => View();
        public IActionResult CustomerReport() => View();
        public IActionResult InventoryReport() => View();
        public IActionResult PurchaseHistory() => View();
        public IActionResult FuelTankReport() => View();
        public IActionResult FuelBatchReport() => View();
        public IActionResult ItemBatchReport() => View();
        public IActionResult CashDifferenceReport() => View();
        public IActionResult VoidTransactionReport() => View();

        public async Task<IActionResult> WarehouseDailyStock(string? search, int? editId)
        {
            return View(await BuildDailyStockPageAsync(WarehouseStockType, search, editId: editId, activeModalId: editId.HasValue ? DailyStockModalId : string.Empty));
        }

        public async Task<IActionResult> DisplayDailyStock(string? search, int? editId)
        {
            return View(await BuildDailyStockPageAsync(DisplayStockType, search, editId: editId, activeModalId: editId.HasValue ? DailyStockModalId : string.Empty));
        }

        public async Task<IActionResult> TankDailyStock(string? search, int? editId)
        {
            return View(await BuildDailyStockPageAsync(TankStockType, search, editId: editId, activeModalId: editId.HasValue ? DailyStockModalId : string.Empty));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveWarehouseDailyStock([Bind(Prefix = "Form")] DailyStockForm form, string? search)
        {
            return await SaveDailyStockAsync(WarehouseStockType, form, search, nameof(WarehouseDailyStock));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDisplayDailyStock([Bind(Prefix = "Form")] DailyStockForm form, string? search)
        {
            return await SaveDailyStockAsync(DisplayStockType, form, search, nameof(DisplayDailyStock));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveTankDailyStock([Bind(Prefix = "Form")] DailyStockForm form, string? search)
        {
            return await SaveDailyStockAsync(TankStockType, form, search, nameof(TankDailyStock));
        }

        [HttpGet]
        public async Task<IActionResult> WarehouseDailyStockDefaults(int batchId, DateTime? stockDate)
        {
            return Json(await BuildProductDailyStockDefaultAsync(WarehouseStockType, batchId, stockDate ?? DateTime.Today));
        }

        [HttpGet]
        public async Task<IActionResult> DisplayDailyStockDefaults(int batchId, DateTime? stockDate)
        {
            return Json(await BuildProductDailyStockDefaultAsync(DisplayStockType, batchId, stockDate ?? DateTime.Today));
        }

        [HttpGet]
        public async Task<IActionResult> TankDailyStockDefaults(int tankId, DateTime? stockDate)
        {
            var date = (stockDate ?? DateTime.Today).Date;
            var tank = await _db.Tanks.AsNoTracking().Include(item => item.Fuel).FirstOrDefaultAsync(item => item.Id == tankId);
            var sold = await ComputeSoldAsync(TankStockType, date, null, tankId);
            var previousEnding = await _db.DailyStockRecords.AsNoTracking()
                .Where(record => record.StockType == TankStockType && record.TankId == tankId && record.StockDate < date)
                .OrderByDescending(record => record.StockDate)
                .ThenByDescending(record => record.Id)
                .Select(record => (decimal?)record.Ending)
                .FirstOrDefaultAsync();

            return Json(new
            {
                tankId,
                fuelName = tank?.Fuel?.Name ?? string.Empty,
                beginning = previousEnding ?? tank?.CurrentLiters ?? 0m,
                sold
            });
        }

        private async Task<IActionResult> SaveDailyStockAsync(string stockType, DailyStockForm form, string? search, string redirectAction)
        {
            form.StockType = stockType;
            NormalizeDailyStockForm(stockType, form);

            if (!ModelState.IsValid)
            {
                return View(ActionViewFor(stockType), await BuildDailyStockPageAsync(stockType, search, form, activeModalId: DailyStockModalId));
            }

            var now = DateTime.UtcNow;
            var sold = await ComputeSoldAsync(stockType, form.StockDate!.Value.Date, form.BatchId, form.TankId);
            var record = form.Id > 0
                ? await _db.DailyStockRecords.FirstOrDefaultAsync(item => item.Id == form.Id && item.StockType == stockType)
                : new DailyStockRecord { CreatedAt = now };

            if (record is null)
            {
                return NotFound();
            }

            record.StockType = stockType;
            record.StockDate = form.StockDate!.Value.Date;
            record.ProductId = stockType == TankStockType ? null : form.ProductId;
            record.BatchId = stockType == TankStockType ? null : form.BatchId;
            record.TankId = stockType == TankStockType ? form.TankId : null;
            record.FuelId = null;
            if (stockType == TankStockType && form.TankId.HasValue)
            {
                record.FuelId = await _db.Tanks.AsNoTracking()
                    .Where(tank => tank.Id == form.TankId.Value)
                    .Select(tank => (int?)tank.FuelId)
                    .FirstOrDefaultAsync();
            }
            record.Beginning = form.Beginning!.Value;
            record.Sold = sold;
            record.Actual = record.Beginning - record.Sold;
            record.Ending = form.Ending!.Value;
            record.Loss = record.Actual - record.Ending;
            record.Remarks = CleanOptional(form.Remarks);
            record.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.DailyStockRecords.Add(record);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(redirectAction, new { search });
        }

        private void NormalizeDailyStockForm(string stockType, DailyStockForm form)
        {
            ModelState.Remove("Form.Sold");
            ModelState.Remove("Form.Actual");
            ModelState.Remove("Form.Loss");

            form.Sold = 0m;
            form.Actual = form.Beginning;
            form.Loss = form.Actual.HasValue && form.Ending.HasValue ? form.Actual.Value - form.Ending.Value : null;

            if (stockType == TankStockType)
            {
                ModelState.Remove("Form.ProductId");
                ModelState.Remove("Form.BatchId");

                if (!form.TankId.HasValue || form.TankId.Value <= 0)
                {
                    ModelState.AddModelError("Form.TankId", "Tank is required.");
                }

                return;
            }

            ModelState.Remove("Form.TankId");

            if (!form.BatchId.HasValue || form.BatchId.Value <= 0)
            {
                ModelState.AddModelError("Form.BatchId", "Batch is required.");
                return;
            }

            var batch = _db.ProductBatches.AsNoTracking().FirstOrDefault(item => item.Id == form.BatchId.Value);
            if (batch is null)
            {
                ModelState.AddModelError("Form.BatchId", "Select a valid batch.");
                return;
            }

            form.ProductId = batch.ProductId;
        }

        private async Task<DailyStockPageViewModel> BuildDailyStockPageAsync(string stockType, string? search, DailyStockForm? form = null, int? editId = null, string activeModalId = "")
        {
            var searchText = (search ?? string.Empty).Trim();
            var recordsQuery = _db.DailyStockRecords
                .AsNoTracking()
                .Include(record => record.Product)
                .Include(record => record.Batch)
                .Include(record => record.Tank)
                .ThenInclude(tank => tank!.Fuel)
                .Where(record => record.StockType == stockType);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                recordsQuery = recordsQuery.Where(record =>
                    (record.Product != null && record.Product.Name.Contains(searchText))
                    || (record.Batch != null && record.Batch.BatchNo.Contains(searchText))
                    || (record.Tank != null && record.Tank.TankNo.Contains(searchText))
                    || (record.Tank != null && record.Tank.Fuel != null && record.Tank.Fuel.Name.Contains(searchText))
                    || (record.Remarks != null && record.Remarks.Contains(searchText)));
            }

            var stockOptions = stockType == TankStockType
                ? await BuildTankStockOptionsAsync()
                : await BuildProductStockOptionsAsync(stockType);

            return new DailyStockPageViewModel
            {
                StockType = stockType,
                PageTitle = $"{stockType} Daily Stock",
                Description = DescriptionFor(stockType),
                PageAction = PageActionFor(stockType),
                SaveAction = SaveActionFor(stockType),
                DefaultAction = DefaultActionFor(stockType),
                Search = searchText,
                ActiveModalId = activeModalId,
                Form = form ?? await BuildDailyStockFormAsync(stockType, editId),
                Records = await recordsQuery.OrderByDescending(record => record.StockDate).ThenByDescending(record => record.Id).ToListAsync(),
                ProductOptions = BuildProductSelectList(stockOptions),
                BatchOptions = BuildBatchSelectList(stockOptions),
                TankOptions = BuildTankSelectList(stockOptions),
                StockOptions = stockOptions
            };
        }

        private async Task<DailyStockForm> BuildDailyStockFormAsync(string stockType, int? editId)
        {
            if (!editId.HasValue)
            {
                return new DailyStockForm { StockType = stockType, StockDate = DateTime.Today, Sold = 0m, Ending = 0m };
            }

            var record = await _db.DailyStockRecords.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value && item.StockType == stockType);
            if (record is null)
            {
                return new DailyStockForm { StockType = stockType, StockDate = DateTime.Today, Sold = 0m, Ending = 0m };
            }

            return new DailyStockForm
            {
                Id = record.Id,
                StockType = stockType,
                StockDate = record.StockDate,
                ProductId = record.ProductId,
                BatchId = record.BatchId,
                TankId = record.TankId,
                Beginning = record.Beginning,
                Sold = record.Sold,
                Actual = record.Actual,
                Ending = record.Ending,
                Loss = record.Loss,
                Remarks = record.Remarks
            };
        }

        private async Task<object> BuildProductDailyStockDefaultAsync(string stockType, int batchId, DateTime stockDate)
        {
            var date = stockDate.Date;
            var batch = await _db.ProductBatches.AsNoTracking().Include(item => item.Product).FirstOrDefaultAsync(item => item.Id == batchId);
            var sold = await ComputeSoldAsync(stockType, date, batchId, null);
            var previousEnding = await _db.DailyStockRecords.AsNoTracking()
                .Where(record => record.StockType == stockType && record.BatchId == batchId && record.StockDate < date)
                .OrderByDescending(record => record.StockDate)
                .ThenByDescending(record => record.Id)
                .Select(record => (decimal?)record.Ending)
                .FirstOrDefaultAsync();

            var currentQuantity = stockType == WarehouseStockType
                ? await _db.WarehouseStocks.AsNoTracking().Where(stock => stock.BatchId == batchId).SumAsync(stock => (decimal?)stock.Quantity) ?? 0m
                : await _db.DisplayStocks.AsNoTracking().Where(stock => stock.BatchId == batchId).SumAsync(stock => (decimal?)stock.Quantity) ?? 0m;

            return new
            {
                batchId,
                productId = batch?.ProductId ?? 0,
                productName = batch?.Product?.Name ?? string.Empty,
                beginning = previousEnding ?? currentQuantity,
                sold
            };
        }

        private async Task<decimal> ComputeSoldAsync(string stockType, DateTime stockDate, int? batchId, int? tankId)
        {
            var start = stockDate.Date;
            var end = start.AddDays(1);

            if (stockType == DisplayStockType && batchId.HasValue)
            {
                return await _db.ProductSales
                    .AsNoTracking()
                    .Where(item => item.BatchId == batchId.Value
                        && item.Sale != null
                        && item.Status == "Completed"
                        && item.Sale.Status == "Completed"
                        && (item.Sale.CreatedAt ?? item.CreatedAt) >= start
                        && (item.Sale.CreatedAt ?? item.CreatedAt) < end)
                    .SumAsync(item => (decimal?)item.Quantity) ?? 0m;
            }

            if (stockType == TankStockType && tankId.HasValue)
            {
                return await _db.FuelSales
                    .AsNoTracking()
                    .Where(item => item.TankId == tankId.Value
                        && item.Sale != null
                        && item.Status == "Completed"
                        && item.Sale.Status == "Completed"
                        && (item.Sale.CreatedAt ?? item.CreatedAt) >= start
                        && (item.Sale.CreatedAt ?? item.CreatedAt) < end)
                    .SumAsync(item => (decimal?)item.Liters) ?? 0m;
            }

            if (stockType == WarehouseStockType && batchId.HasValue)
            {
                return await _db.StockMovements
                    .AsNoTracking()
                    .Where(item => item.ProductBatchId == batchId.Value
                        && item.SourceLocation == "Warehouse"
                        && item.DestinationLocation == "Display"
                        && item.MovementType == "Transfer"
                        && item.CreatedAt >= start
                        && item.CreatedAt < end)
                    .SumAsync(item => (decimal?)item.Quantity) ?? 0m;
            }

            return 0m;
        }

        private async Task<List<DailyStockOption>> BuildProductStockOptionsAsync(string stockType)
        {
            if (stockType == WarehouseStockType)
            {
                return await _db.WarehouseStocks.AsNoTracking()
                    .Include(stock => stock.Product)
                    .Include(stock => stock.Batch)
                    .GroupBy(stock => new { stock.BatchId, stock.ProductId, ProductName = stock.Product!.Name, stock.Batch!.BatchNo })
                    .Select(group => new DailyStockOption
                    {
                        Id = group.Key.BatchId,
                        ProductId = group.Key.ProductId,
                        ProductName = group.Key.ProductName,
                        BatchNo = group.Key.BatchNo,
                        CurrentQuantity = group.Sum(stock => stock.Quantity)
                    })
                    .OrderBy(option => option.ProductName)
                    .ThenBy(option => option.BatchNo)
                    .ToListAsync();
            }

            return await _db.DisplayStocks.AsNoTracking()
                .Include(stock => stock.Product)
                .Include(stock => stock.Batch)
                .GroupBy(stock => new { stock.BatchId, stock.ProductId, ProductName = stock.Product!.Name, stock.Batch!.BatchNo })
                .Select(group => new DailyStockOption
                {
                    Id = group.Key.BatchId,
                    ProductId = group.Key.ProductId,
                    ProductName = group.Key.ProductName,
                    BatchNo = group.Key.BatchNo,
                    CurrentQuantity = group.Sum(stock => stock.Quantity)
                })
                .OrderBy(option => option.ProductName)
                .ThenBy(option => option.BatchNo)
                .ToListAsync();
        }

        private async Task<List<DailyStockOption>> BuildTankStockOptionsAsync()
        {
            return await _db.Tanks.AsNoTracking()
                .Include(tank => tank.Fuel)
                .Where(tank => tank.Status == 1 && tank.IsActive)
                .OrderBy(tank => tank.TankNo)
                .Select(tank => new DailyStockOption
                {
                    Id = tank.Id,
                    TankNo = tank.TankNo,
                    FuelName = tank.Fuel != null ? tank.Fuel.Name : string.Empty,
                    CurrentQuantity = tank.CurrentLiters
                })
                .ToListAsync();
        }

        private static List<SelectListItem> BuildProductSelectList(List<DailyStockOption> stockOptions)
        {
            return stockOptions
                .Where(option => option.ProductId.HasValue)
                .GroupBy(option => new { option.ProductId, option.ProductName })
                .Select(group => new SelectListItem { Value = group.Key.ProductId!.Value.ToString(), Text = group.Key.ProductName })
                .ToList();
        }

        private static List<SelectListItem> BuildBatchSelectList(List<DailyStockOption> stockOptions)
        {
            return stockOptions
                .Where(option => option.ProductId.HasValue)
                .Select(option => new SelectListItem { Value = option.Id.ToString(), Text = $"{option.BatchNo} - {option.ProductName}" })
                .ToList();
        }

        private static List<SelectListItem> BuildTankSelectList(List<DailyStockOption> stockOptions)
        {
            return stockOptions
                .Where(option => !string.IsNullOrWhiteSpace(option.TankNo))
                .Select(option => new SelectListItem { Value = option.Id.ToString(), Text = string.IsNullOrWhiteSpace(option.FuelName) ? option.TankNo : $"{option.TankNo} - {option.FuelName}" })
                .ToList();
        }

        private static string? CleanOptional(string? value)
        {
            var trimmed = value?.Trim();
            return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
        }

        private static string DescriptionFor(string stockType) => stockType switch
        {
            WarehouseStockType => "Record warehouse daily stock counts by product batch.",
            DisplayStockType => "Record display daily stock counts by product batch.",
            TankStockType => "Record daily tank fuel liter counts.",
            _ => "Record daily stock counts."
        };

        private static string PageActionFor(string stockType) => stockType switch
        {
            WarehouseStockType => nameof(WarehouseDailyStock),
            DisplayStockType => nameof(DisplayDailyStock),
            TankStockType => nameof(TankDailyStock),
            _ => nameof(WarehouseDailyStock)
        };

        private static string SaveActionFor(string stockType) => stockType switch
        {
            WarehouseStockType => nameof(SaveWarehouseDailyStock),
            DisplayStockType => nameof(SaveDisplayDailyStock),
            TankStockType => nameof(SaveTankDailyStock),
            _ => nameof(SaveWarehouseDailyStock)
        };

        private static string DefaultActionFor(string stockType) => stockType switch
        {
            WarehouseStockType => nameof(WarehouseDailyStockDefaults),
            DisplayStockType => nameof(DisplayDailyStockDefaults),
            TankStockType => nameof(TankDailyStockDefaults),
            _ => nameof(WarehouseDailyStockDefaults)
        };

        private static string ActionViewFor(string stockType) => stockType switch
        {
            WarehouseStockType => "WarehouseDailyStock",
            DisplayStockType => "DisplayDailyStock",
            TankStockType => "TankDailyStock",
            _ => "WarehouseDailyStock"
        };
    }
}

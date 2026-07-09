using gpos.Data;
using gpos.Filters;
using gpos.Models;
using gpos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        public async Task<IActionResult> WarehouseDailyStock(string? search, int? editId, int? branchId, int? filterBranchId)
        {
            branchId = filterBranchId ?? branchId;
            return View(await BuildDailyStockPageAsync(WarehouseStockType, search, branchId, editId: editId, activeModalId: editId.HasValue ? DailyStockModalId : string.Empty));
        }

        public async Task<IActionResult> DisplayDailyStock(string? search, int? editId, int? branchId, int? filterBranchId)
        {
            branchId = filterBranchId ?? branchId;
            return View(await BuildDailyStockPageAsync(DisplayStockType, search, branchId, editId: editId, activeModalId: editId.HasValue ? DailyStockModalId : string.Empty));
        }

        public async Task<IActionResult> TankDailyStock(string? search, int? editId, int? branchId, int? filterBranchId)
        {
            branchId = filterBranchId ?? branchId;
            return View(await BuildDailyStockPageAsync(TankStockType, search, branchId, editId: editId, activeModalId: editId.HasValue ? DailyStockModalId : string.Empty));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveWarehouseDailyStock([Bind(Prefix = "Form")] DailyStockForm form, string? search, int? filterBranchId)
        {
            return await SaveDailyStockAsync(WarehouseStockType, form, search, filterBranchId, nameof(WarehouseDailyStock));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDisplayDailyStock([Bind(Prefix = "Form")] DailyStockForm form, string? search, int? filterBranchId)
        {
            return await SaveDailyStockAsync(DisplayStockType, form, search, filterBranchId, nameof(DisplayDailyStock));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveTankDailyStock([Bind(Prefix = "Form")] DailyStockForm form, string? search, int? filterBranchId)
        {
            return await SaveDailyStockAsync(TankStockType, form, search, filterBranchId, nameof(TankDailyStock));
        }

        [HttpGet]
        public async Task<IActionResult> WarehouseDailyStockDefaults(int batchId, DateTime? stockDate, int? branchId)
        {
            return Json(await BuildProductDailyStockDefaultAsync(WarehouseStockType, batchId, stockDate ?? DateTime.Today, branchId, null));
        }

        [HttpGet]
        public async Task<IActionResult> DisplayDailyStockDefaults(int batchId, DateTime? stockDate, int? branchId)
        {
            return Json(await BuildProductDailyStockDefaultAsync(DisplayStockType, batchId, stockDate ?? DateTime.Today, branchId, null));
        }

        [HttpGet]
        public async Task<IActionResult> TankDailyStockDefaults(int tankId, DateTime? stockDate, int? branchId)
        {
            return Json(await BuildTankDailyStockDefaultAsync(tankId, stockDate ?? DateTime.Today, branchId));
        }

        [HttpGet]
        public async Task<IActionResult> SearchWarehouseDailyStockItems(string? search, string? term, int? branchId)
        {
            return Json(await SearchProductDailyStockItemsAsync(WarehouseStockType, term ?? search, branchId));
        }

        [HttpGet]
        public async Task<IActionResult> SearchDisplayDailyStockItems(string? search, string? term, int? branchId)
        {
            return Json(await SearchProductDailyStockItemsAsync(DisplayStockType, term ?? search, branchId));
        }

        [HttpGet]
        public async Task<IActionResult> SearchTankDailyStockItems(string? search, string? term, int? branchId)
        {
            if (!branchId.HasValue || branchId.Value <= 0)
            {
                return Json(Array.Empty<object>());
            }

            var query = _db.Tanks.AsNoTracking()
                .Include(tank => tank.Branch)
                .Include(tank => tank.Fuel)
                .Where(tank => tank.Status == 1 && tank.IsActive && tank.BranchId == branchId.Value);

            var searchText = CleanOptional(term ?? search);
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(tank =>
                    tank.TankNo.Contains(searchText)
                    || (tank.Fuel != null && tank.Fuel.Name.Contains(searchText)));
            }

            var items = await query
                .OrderBy(tank => tank.Branch != null ? tank.Branch.Name : string.Empty)
                .ThenBy(tank => tank.TankNo)
                .Select(tank => new
                {
                    tankId = tank.Id,
                    branchId = tank.BranchId,
                    branchName = tank.Branch != null ? tank.Branch.Name : "Unassigned",
                    fuelId = tank.FuelId,
                    fuelName = tank.Fuel != null ? tank.Fuel.Name : string.Empty,
                    tankName = tank.TankNo,
                    currentLiters = tank.CurrentLiters,
                    capacity = tank.CapacityLiters
                })
                .Take(100)
                .ToListAsync();

            return Json(items);
        }

        [HttpGet]
        public async Task<IActionResult> GetDailyStockBeginning(string stockType, DateTime? recordDate, int? branchId, int? productId, int? batchId, int? tankId, int? warehouseStockId, int? displayStockId)
        {
            var result = stockType == TankStockType
                ? await BuildTankDailyStockDefaultAsync(tankId, recordDate ?? DateTime.Today, branchId)
                : await BuildProductDailyStockDefaultAsync(stockType, batchId ?? 0, recordDate ?? DateTime.Today, branchId, stockType == WarehouseStockType ? warehouseStockId : displayStockId);

            return Json(result);
        }

        private async Task<IActionResult> SaveDailyStockAsync(string stockType, DailyStockForm form, string? search, int? filterBranchId, string redirectAction)
        {
            form.StockType = stockType;
            NormalizeDailyStockForm(stockType, form);

            if (!ModelState.IsValid)
            {
                return View(ActionViewFor(stockType), await BuildDailyStockPageAsync(stockType, search, filterBranchId, form, activeModalId: DailyStockModalId));
            }

            var now = DateTime.UtcNow;
            if (!await BranchExistsAsync(form.BranchId))
            {
                ModelState.AddModelError("Form.BranchId", "Branch is required.");
                return View(ActionViewFor(stockType), await BuildDailyStockPageAsync(stockType, search, filterBranchId, form, activeModalId: DailyStockModalId));
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();
            var selectedStock = await ValidateAndLoadSelectedStockAsync(stockType, form);
            if (selectedStock is null)
            {
                return View(ActionViewFor(stockType), await BuildDailyStockPageAsync(stockType, search, filterBranchId, form, activeModalId: DailyStockModalId));
            }

            var sold = await ComputeSoldAsync(stockType, form.StockDate!.Value.Date, form.BatchId, form.TankId, form.BranchId);
            var record = form.Id > 0
                ? await _db.DailyStockRecords.FirstOrDefaultAsync(item => item.Id == form.Id && item.StockType == stockType)
                : new DailyStockRecord { CreatedAt = now, CreatedBy = CurrentUserId() };

            if (record is null)
            {
                return NotFound();
            }

            record.StockType = stockType;
            record.BranchId = form.BranchId;
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
            ApplyOfficialStockCorrection(stockType, selectedStock, record.Ending, now);
            AddDailyStockMovementIfSupported(stockType, record, selectedStock, now);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return RedirectToAction(redirectAction, new { search, filterBranchId });
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
                ModelState.Remove("Form.WarehouseStockId");
                ModelState.Remove("Form.DisplayStockId");
                ModelState.Remove("Form.ProductId");
                ModelState.Remove("Form.BatchId");

                if (!form.TankId.HasValue || form.TankId.Value <= 0)
                {
                    ModelState.AddModelError("Form.TankId", "Tank is required.");
                }

                if (form.BranchId <= 0)
                {
                    ModelState.AddModelError("Form.BranchId", "Branch is required.");
                }

                return;
            }

            ModelState.Remove("Form.TankId");

            if (stockType == WarehouseStockType)
            {
                ModelState.Remove("Form.DisplayStockId");
                if (!form.WarehouseStockId.HasValue || form.WarehouseStockId.Value <= 0)
                {
                    ModelState.AddModelError("Form.WarehouseStockId", "Warehouse stock selection is required.");
                }
            }
            else
            {
                ModelState.Remove("Form.WarehouseStockId");
                if (!form.DisplayStockId.HasValue || form.DisplayStockId.Value <= 0)
                {
                    ModelState.AddModelError("Form.DisplayStockId", "Display stock selection is required.");
                }
            }

            if (!form.BatchId.HasValue || form.BatchId.Value <= 0)
            {
                ModelState.AddModelError("Form.BatchId", "Batch is required.");
                return;
            }

            if (form.BranchId <= 0)
            {
                ModelState.AddModelError("Form.BranchId", "Branch is required.");
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

        private async Task<DailyStockPageViewModel> BuildDailyStockPageAsync(string stockType, string? search, int? branchId = null, DailyStockForm? form = null, int? editId = null, string activeModalId = "")
        {
            var searchText = (search ?? string.Empty).Trim();
            var recordsQuery = _db.DailyStockRecords
                .AsNoTracking()
                .Include(record => record.Branch)
                .Include(record => record.Product)
                .Include(record => record.Batch)
                .Include(record => record.Tank)
                .ThenInclude(tank => tank!.Fuel)
                .Where(record => record.StockType == stockType);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                recordsQuery = recordsQuery.Where(record =>
                    (record.Branch != null && record.Branch.Name.Contains(searchText))
                    || (record.Product != null && record.Product.Name.Contains(searchText))
                    || (record.Batch != null && record.Batch.BatchNo.Contains(searchText))
                    || (record.Tank != null && record.Tank.TankNo.Contains(searchText))
                    || (record.Tank != null && record.Tank.Fuel != null && record.Tank.Fuel.Name.Contains(searchText))
                    || (record.Remarks != null && record.Remarks.Contains(searchText)));
            }

            if (branchId.HasValue && branchId.Value > 0)
            {
                recordsQuery = recordsQuery.Where(record => record.BranchId == branchId.Value);
            }

            var dailyStockForm = form ?? await BuildDailyStockFormAsync(stockType, editId);
            if (dailyStockForm.Id == 0 && dailyStockForm.BranchId <= 0 && branchId.HasValue && branchId.Value > 0)
            {
                dailyStockForm.BranchId = branchId.Value;
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
                BranchId = branchId,
                FormBranchName = await BranchNameAsync(dailyStockForm.BranchId),
                ActiveModalId = activeModalId,
                Form = dailyStockForm,
                Records = await recordsQuery.OrderByDescending(record => record.StockDate).ThenByDescending(record => record.Id).ToListAsync(),
                ProductOptions = BuildProductSelectList(stockOptions),
                BatchOptions = BuildBatchSelectList(stockOptions),
                TankOptions = BuildTankSelectList(stockOptions),
                BranchOptions = await BuildBranchFilterOptionsAsync(),
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
                BranchId = record.BranchId ?? 0,
                ProductId = record.ProductId,
                BatchId = record.BatchId,
                WarehouseStockId = stockType == WarehouseStockType ? await ResolveWarehouseStockIdAsync(record.BranchId, record.ProductId, record.BatchId) : null,
                DisplayStockId = stockType == DisplayStockType ? await ResolveDisplayStockIdAsync(record.BranchId, record.ProductId, record.BatchId) : null,
                TankId = record.TankId,
                Beginning = record.Beginning,
                Sold = record.Sold,
                Actual = record.Actual,
                Ending = record.Ending,
                Loss = record.Loss,
                Remarks = record.Remarks
            };
        }

        private async Task<object> BuildProductDailyStockDefaultAsync(string stockType, int batchId, DateTime stockDate, int? branchId, int? stockId)
        {
            var date = stockDate.Date;
            var stockInfo = await GetProductDailyStockInfoAsync(stockType, stockId, branchId, batchId);
            batchId = stockInfo?.BatchId ?? batchId;
            branchId = stockInfo?.BranchId ?? branchId;
            var productId = stockInfo?.ProductId;
            var batch = await _db.ProductBatches.AsNoTracking().Include(item => item.Product).FirstOrDefaultAsync(item => item.Id == batchId);
            var sold = await ComputeSoldAsync(stockType, date, batchId, null, branchId);
            var previousEnding = await _db.DailyStockRecords.AsNoTracking()
                .Where(record => record.StockType == stockType
                    && record.BranchId == branchId
                    && record.ProductId == productId
                    && record.BatchId == batchId
                    && record.StockDate < date)
                .OrderByDescending(record => record.StockDate)
                .ThenByDescending(record => record.Id)
                .Select(record => (decimal?)record.Ending)
                .FirstOrDefaultAsync();

            var currentQuantity = stockInfo?.Quantity ?? 0m;

            return new
            {
                batchId,
                branchId,
                productId = productId ?? batch?.ProductId ?? 0,
                productName = batch?.Product?.Name ?? string.Empty,
                beginning = previousEnding ?? currentQuantity,
                sold,
                source = previousEnding.HasValue
                    ? "Previous Ending"
                    : stockType == WarehouseStockType ? "Current Warehouse Stock" : "Current Display Stock"
            };
        }

        private async Task<object> BuildTankDailyStockDefaultAsync(int? tankId, DateTime stockDate, int? branchId)
        {
            var date = stockDate.Date;
            var tank = await _db.Tanks.AsNoTracking().Include(item => item.Fuel).FirstOrDefaultAsync(item => item.Id == tankId);
            var effectiveBranchId = branchId ?? tank?.BranchId;
            var sold = await ComputeSoldAsync(TankStockType, date, null, tankId, effectiveBranchId);
            var previousEnding = await _db.DailyStockRecords.AsNoTracking()
                .Where(record => record.StockType == TankStockType
                    && record.TankId == tankId
                    && record.BranchId == effectiveBranchId
                    && record.StockDate < date)
                .OrderByDescending(record => record.StockDate)
                .ThenByDescending(record => record.Id)
                .Select(record => (decimal?)record.Ending)
                .FirstOrDefaultAsync();

            return new
            {
                tankId,
                branchId = effectiveBranchId,
                fuelId = tank?.FuelId ?? 0,
                fuelName = tank?.Fuel?.Name ?? string.Empty,
                beginning = previousEnding ?? tank?.CurrentLiters ?? 0m,
                sold,
                source = previousEnding.HasValue ? "Previous Ending" : "Current Tank Liters"
            };
        }

        private async Task<decimal> ComputeSoldAsync(string stockType, DateTime stockDate, int? batchId, int? tankId, int? branchId)
        {
            var start = stockDate.Date;
            var end = start.AddDays(1);

            if (stockType == DisplayStockType && batchId.HasValue)
            {
                return await _db.ProductSales
                    .AsNoTracking()
                    .Where(item => item.BatchId == batchId.Value
                        && item.Sale != null
                        && item.Sale.BranchId == branchId
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
                        && item.Sale.BranchId == branchId
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
                    .Where(item => !branchId.HasValue || _db.StockTransfers.Any(transfer => transfer.Id == item.ReferenceId && transfer.SourceBranchId == branchId.Value))
                    .SumAsync(item => (decimal?)item.Quantity) ?? 0m;
            }

            return 0m;
        }

        private async Task<object?> ValidateAndLoadSelectedStockAsync(string stockType, DailyStockForm form)
        {
            if (stockType == WarehouseStockType)
            {
                var stock = await _db.WarehouseStocks.FirstOrDefaultAsync(item => item.Id == form.WarehouseStockId);
                if (stock is null)
                {
                    ModelState.AddModelError("Form.WarehouseStockId", "Select a valid warehouse stock item.");
                    return null;
                }

                if (stock.BranchId != form.BranchId)
                {
                    ModelState.AddModelError("Form.BranchId", "Selected warehouse stock does not belong to this branch.");
                    return null;
                }

                form.ProductId = stock.ProductId;
                form.BatchId = stock.BatchId;
                return stock;
            }

            if (stockType == DisplayStockType)
            {
                var stock = await _db.DisplayStocks.FirstOrDefaultAsync(item => item.Id == form.DisplayStockId);
                if (stock is null)
                {
                    ModelState.AddModelError("Form.DisplayStockId", "Select a valid display stock item.");
                    return null;
                }

                if (stock.BranchId != form.BranchId)
                {
                    ModelState.AddModelError("Form.BranchId", "Selected display stock does not belong to this branch.");
                    return null;
                }

                form.ProductId = stock.ProductId;
                form.BatchId = stock.BatchId;
                return stock;
            }

            var tank = await _db.Tanks.FirstOrDefaultAsync(item => item.Id == form.TankId);
            if (tank is null)
            {
                ModelState.AddModelError("Form.TankId", "Select a valid tank.");
                return null;
            }

            if (tank.BranchId != form.BranchId)
            {
                ModelState.AddModelError("Form.BranchId", "Selected tank does not belong to this branch.");
                return null;
            }

            return tank;
        }

        private static void ApplyOfficialStockCorrection(string stockType, object selectedStock, decimal ending, DateTime now)
        {
            if (stockType == WarehouseStockType && selectedStock is WarehouseStock warehouseStock)
            {
                warehouseStock.Quantity = ending;
                warehouseStock.UpdatedAt = now;
                return;
            }

            if (stockType == DisplayStockType && selectedStock is DisplayStock displayStock)
            {
                displayStock.Quantity = ending;
                displayStock.UpdatedAt = now;
                return;
            }

            if (stockType == TankStockType && selectedStock is Tank tank)
            {
                tank.CurrentLiters = ending;
                tank.UpdatedAt = now;
            }
        }

        private void AddDailyStockMovementIfSupported(string stockType, DailyStockRecord record, object selectedStock, DateTime now)
        {
            if (stockType == TankStockType)
            {
                return;
            }

            var quantityDifference = record.Ending - record.Actual;
            if (quantityDifference == 0m)
            {
                return;
            }

            var productId = stockType == WarehouseStockType && selectedStock is WarehouseStock warehouseStock
                ? warehouseStock.ProductId
                : selectedStock is DisplayStock displayStock ? displayStock.ProductId : record.ProductId ?? 0;

            if (productId <= 0)
            {
                return;
            }

            _db.StockMovements.Add(new StockMovement
            {
                ProductId = productId,
                ProductBatchId = record.BatchId,
                SourceLocation = stockType,
                DestinationLocation = stockType,
                MovementType = "DailyStockCorrection",
                Quantity = Math.Abs(quantityDifference),
                ReferenceType = "DailyStockRecord",
                ReferenceId = record.Id,
                Remarks = record.Remarks,
                CreatedBy = CurrentUserId(),
                CreatedAt = now
            });
        }

        private async Task<ProductStockInfo?> GetProductDailyStockInfoAsync(string stockType, int? stockId, int? branchId, int? batchId)
        {
            if (stockType == WarehouseStockType)
            {
                var query = _db.WarehouseStocks.AsNoTracking();
                if (stockId.HasValue && stockId.Value > 0)
                {
                    query = query.Where(stock => stock.Id == stockId.Value);
                }
                else
                {
                    query = query.Where(stock => stock.BranchId == branchId && stock.BatchId == batchId);
                }

                return await query
                    .Select(stock => new ProductStockInfo(stock.Id, stock.BranchId, stock.ProductId, stock.BatchId, stock.Quantity))
                    .FirstOrDefaultAsync();
            }

            var displayQuery = _db.DisplayStocks.AsNoTracking();
            if (stockId.HasValue && stockId.Value > 0)
            {
                displayQuery = displayQuery.Where(stock => stock.Id == stockId.Value);
            }
            else
            {
                displayQuery = displayQuery.Where(stock => stock.BranchId == branchId && stock.BatchId == batchId);
            }

            return await displayQuery
                .Select(stock => new ProductStockInfo(stock.Id, stock.BranchId, stock.ProductId, stock.BatchId, stock.Quantity))
                .FirstOrDefaultAsync();
        }

        private async Task<List<object>> SearchProductDailyStockItemsAsync(string stockType, string? search, int? branchId)
        {
            if (!branchId.HasValue || branchId.Value <= 0)
            {
                return new List<object>();
            }

            var searchText = CleanOptional(search);
            if (stockType == WarehouseStockType)
            {
                var query = _db.WarehouseStocks.AsNoTracking()
                    .Include(stock => stock.Branch)
                    .Include(stock => stock.Product)
                    .Include(stock => stock.Batch)
                    .AsQueryable();

                query = query.Where(stock => stock.BranchId == branchId.Value);

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(stock =>
                    (stock.Product != null && stock.Product.Name.Contains(searchText))
                        || (stock.Batch != null && stock.Batch.BatchNo.Contains(searchText)));
                }

                var items = await query
                    .OrderBy(stock => stock.Branch != null ? stock.Branch.Name : string.Empty)
                    .ThenBy(stock => stock.Product != null ? stock.Product.Name : string.Empty)
                    .ThenBy(stock => stock.Batch != null ? stock.Batch.BatchNo : string.Empty)
                    .Select(stock => new
                    {
                        warehouseStockId = stock.Id,
                        branchId = stock.BranchId,
                        branchName = stock.Branch != null ? stock.Branch.Name : "Unassigned",
                        productId = stock.ProductId,
                        productName = stock.Product != null ? stock.Product.Name : string.Empty,
                        batchId = stock.BatchId,
                        batchNumber = stock.Batch != null ? stock.Batch.BatchNo : string.Empty,
                        currentQuantity = stock.Quantity
                    })
                    .Take(100)
                    .ToListAsync();

                return items.Cast<object>().ToList();
            }

            var displayQuery = _db.DisplayStocks.AsNoTracking()
                .Include(stock => stock.Branch)
                .Include(stock => stock.Product)
                .Include(stock => stock.Batch)
                .AsQueryable();

            displayQuery = displayQuery.Where(stock => stock.BranchId == branchId.Value);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                displayQuery = displayQuery.Where(stock =>
                    (stock.Product != null && stock.Product.Name.Contains(searchText))
                    || (stock.Batch != null && stock.Batch.BatchNo.Contains(searchText)));
            }

            var displayItems = await displayQuery
                .OrderBy(stock => stock.Branch != null ? stock.Branch.Name : string.Empty)
                .ThenBy(stock => stock.Product != null ? stock.Product.Name : string.Empty)
                .ThenBy(stock => stock.Batch != null ? stock.Batch.BatchNo : string.Empty)
                .Select(stock => new
                {
                    displayStockId = stock.Id,
                    branchId = stock.BranchId,
                    branchName = stock.Branch != null ? stock.Branch.Name : "Unassigned",
                    productId = stock.ProductId,
                    productName = stock.Product != null ? stock.Product.Name : string.Empty,
                    batchId = stock.BatchId,
                    batchNumber = stock.Batch != null ? stock.Batch.BatchNo : string.Empty,
                    currentQuantity = stock.Quantity
                })
                .Take(100)
                .ToListAsync();

            return displayItems.Cast<object>().ToList();
        }

        private async Task<List<DailyStockOption>> BuildProductStockOptionsAsync(string stockType)
        {
            if (stockType == WarehouseStockType)
            {
                return await _db.WarehouseStocks.AsNoTracking()
                    .Include(stock => stock.Product)
                    .Include(stock => stock.Batch)
                    .Include(stock => stock.Branch)
                    .OrderBy(stock => stock.Branch != null ? stock.Branch.Name : string.Empty)
                    .ThenBy(stock => stock.Product != null ? stock.Product.Name : string.Empty)
                    .ThenBy(stock => stock.Batch != null ? stock.Batch.BatchNo : string.Empty)
                    .Select(stock => new DailyStockOption
                    {
                        Id = stock.Id,
                        WarehouseStockId = stock.Id,
                        BranchId = stock.BranchId,
                        BranchName = stock.Branch != null ? stock.Branch.Name : "Unassigned",
                        ProductId = stock.ProductId,
                        ProductName = stock.Product != null ? stock.Product.Name : string.Empty,
                        BatchId = stock.BatchId,
                        BatchNo = stock.Batch != null ? stock.Batch.BatchNo : string.Empty,
                        CurrentQuantity = stock.Quantity
                    })
                    .ToListAsync();
            }

            return await _db.DisplayStocks.AsNoTracking()
                .Include(stock => stock.Product)
                .Include(stock => stock.Batch)
                .Include(stock => stock.Branch)
                .OrderBy(stock => stock.Branch != null ? stock.Branch.Name : string.Empty)
                .ThenBy(stock => stock.Product != null ? stock.Product.Name : string.Empty)
                .ThenBy(stock => stock.Batch != null ? stock.Batch.BatchNo : string.Empty)
                .Select(stock => new DailyStockOption
                {
                    Id = stock.Id,
                    DisplayStockId = stock.Id,
                    BranchId = stock.BranchId,
                    BranchName = stock.Branch != null ? stock.Branch.Name : "Unassigned",
                    ProductId = stock.ProductId,
                    ProductName = stock.Product != null ? stock.Product.Name : string.Empty,
                    BatchId = stock.BatchId,
                    BatchNo = stock.Batch != null ? stock.Batch.BatchNo : string.Empty,
                    CurrentQuantity = stock.Quantity
                })
                .ToListAsync();
        }

        private async Task<List<DailyStockOption>> BuildTankStockOptionsAsync()
        {
            return await _db.Tanks.AsNoTracking()
                .Include(tank => tank.Fuel)
                .Include(tank => tank.Branch)
                .Where(tank => tank.Status == 1 && tank.IsActive)
                .OrderBy(tank => tank.TankNo)
                .Select(tank => new DailyStockOption
                {
                    Id = tank.Id,
                    BranchId = tank.BranchId,
                    BranchName = tank.Branch != null ? tank.Branch.Name : "Unassigned",
                    TankNo = tank.TankNo,
                    FuelId = tank.FuelId,
                    FuelName = tank.Fuel != null ? tank.Fuel.Name : string.Empty,
                    CurrentQuantity = tank.CurrentLiters,
                    Capacity = tank.CapacityLiters
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

        private async Task<List<SelectListItem>> BuildBranchFilterOptionsAsync()
        {
            var options = await _db.Branches.AsNoTracking()
                .Where(branch => branch.Status == 1)
                .OrderBy(branch => branch.Name)
                .Select(branch => new SelectListItem { Value = branch.Id.ToString(), Text = branch.Name })
                .ToListAsync();
            options.Insert(0, new SelectListItem { Value = "", Text = "All Branches" });
            return options;
        }

        private async Task<string> BranchNameAsync(int branchId)
        {
            if (branchId <= 0)
            {
                return string.Empty;
            }

            return await _db.Branches.AsNoTracking()
                .Where(branch => branch.Id == branchId)
                .Select(branch => branch.Name)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        private async Task<bool> BranchExistsAsync(int branchId)
        {
            return branchId > 0 && await _db.Branches.AsNoTracking().AnyAsync(branch => branch.Id == branchId && branch.Status == 1);
        }

        private async Task<int?> ResolveWarehouseStockIdAsync(int? branchId, int? productId, int? batchId)
        {
            if (!branchId.HasValue || !productId.HasValue || !batchId.HasValue)
            {
                return null;
            }

            return await _db.WarehouseStocks.AsNoTracking()
                .Where(stock => stock.BranchId == branchId && stock.ProductId == productId && stock.BatchId == batchId)
                .Select(stock => (int?)stock.Id)
                .FirstOrDefaultAsync();
        }

        private async Task<int?> ResolveDisplayStockIdAsync(int? branchId, int? productId, int? batchId)
        {
            if (!branchId.HasValue || !productId.HasValue || !batchId.HasValue)
            {
                return null;
            }

            return await _db.DisplayStocks.AsNoTracking()
                .Where(stock => stock.BranchId == branchId && stock.ProductId == productId && stock.BatchId == batchId)
                .Select(stock => (int?)stock.Id)
                .FirstOrDefaultAsync();
        }

        private int? CurrentUserId()
        {
            var sessionUserId = HttpContext.Session.GetString("UserId");
            var claimUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(sessionUserId, out var userId) || int.TryParse(claimUserId, out userId))
            {
                return userId;
            }

            return null;
        }

        private static string? CleanOptional(string? value)
        {
            var trimmed = value?.Trim();
            return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
        }

        private sealed record ProductStockInfo(int Id, int? BranchId, int ProductId, int BatchId, decimal Quantity);

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

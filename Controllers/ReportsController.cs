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
        private readonly ILogger<ReportsController>? _logger;

        public ReportsController(ApplicationDbContext db, ILogger<ReportsController>? logger = null)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult DailySalesReport() => View();
        public async Task<IActionResult> DailySales(string? search, int? branchId, int? userId, string? status, DateTime? from, DateTime? to, int page = 1, int? detailsId = null)
        {
            const int pageSize = 20;
            var term = (search ?? string.Empty).Trim();
            status = (status ?? string.Empty).Trim();
            if (status is not ("" or "Completed" or "Voided")) status = string.Empty;
            if (from.HasValue && to.HasValue && from.Value.Date > to.Value.Date)
                ModelState.AddModelError(string.Empty, "From date cannot be later than To date.");

            var query = _db.Sales.AsNoTracking().Where(sale => sale.Status == "Completed" || sale.Status == "Voided");
            if (term.Length > 0)
                query = query.Where(sale => sale.ReceiptNo.Contains(term)
                    || (sale.Member != null && (sale.Member.MemberNo.Contains(term) || sale.Member.FullName.Contains(term)))
                    || sale.Payments.Any(payment => payment.ReferenceNo != null && payment.ReferenceNo.Contains(term)));
            if (branchId.HasValue) query = query.Where(sale => sale.BranchId == branchId.Value);
            if (userId.HasValue) query = query.Where(sale => sale.UserId == userId.Value);
            if (status.Length > 0) query = query.Where(sale => sale.Status == status);
            if (from.HasValue) query = query.Where(sale => (sale.BusinessDate ?? sale.CreatedAt) >= from.Value.Date);
            if (to.HasValue) { var exclusiveTo = to.Value.Date.AddDays(1); query = query.Where(sale => (sale.BusinessDate ?? sale.CreatedAt) < exclusiveTo); }

            var summary = await query.GroupBy(_ => 1).Select(group => new
            {
                Total = group.Count(), Completed = group.Count(sale => sale.Status == "Completed"), Voided = group.Count(sale => sale.Status == "Voided"),
                CompletedAmount = group.Where(sale => sale.Status == "Completed").Sum(sale => sale.NetTotal),
                VoidedAmount = group.Where(sale => sale.Status == "Voided").Sum(sale => sale.NetTotal)
            }).FirstOrDefaultAsync();
            var total = summary?.Total ?? 0;
            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            page = Math.Clamp(page, 1, totalPages);
            var rows = await query.OrderByDescending(sale => sale.BusinessDate ?? sale.CreatedAt).ThenByDescending(sale => sale.Id)
                .Skip((page - 1) * pageSize).Take(pageSize).Select(sale => new SalesReportRowViewModel
                {
                    Id = sale.Id, ReceiptNo = sale.ReceiptNo, SaleDate = sale.BusinessDate ?? sale.CreatedAt,
                    Branch = sale.Branch != null ? sale.Branch.Name : "Unassigned",
                    User = sale.User != null ? (sale.User.FullName ?? sale.User.Username) : "Unknown",
                    HasProducts = sale.ProductSales.Any(), HasFuel = sale.FuelSales.Any(),
                    PaymentMethod = sale.Payments.OrderBy(payment => payment.Id).Select(payment => payment.PaymentType).FirstOrDefault() ?? "Not recorded",
                    OriginalTotal = sale.NetTotal, Status = sale.Status,
                    VoidDate = sale.Voids.OrderByDescending(item => item.CompletedAt ?? item.CreatedAt).Select(item => (DateTime?)(item.CompletedAt ?? item.CreatedAt)).FirstOrDefault(),
                    VoidedBy = sale.Voids.OrderByDescending(item => item.CompletedAt ?? item.CreatedAt).Select(item => item.RequestedByUser != null ? (item.RequestedByUser.FullName ?? item.RequestedByUser.Username) : "Unknown").FirstOrDefault() ?? string.Empty
                }).ToListAsync();

            var model = new SalesReportPageViewModel
            {
                Search = term, BranchId = branchId, UserId = userId, Status = status, From = from, To = to,
                Page = page, PageSize = pageSize, TotalPages = totalPages, TotalRecords = total,
                CompletedCount = summary?.Completed ?? 0, VoidedCount = summary?.Voided ?? 0,
                CompletedAmount = summary?.CompletedAmount ?? 0, VoidedOriginalAmount = summary?.VoidedAmount ?? 0,
                Rows = rows, BranchOptions = await BuildBranchFilterOptionsAsync(),
                UserOptions = await _db.Users.AsNoTracking().OrderBy(user => user.FullName ?? user.Username).Select(user => new SelectListItem { Value = user.Id.ToString(), Text = user.FullName ?? user.Username }).ToListAsync()
            };
            model.UserOptions.Insert(0, new SelectListItem { Value = "", Text = "All Users" });
            foreach (var option in model.BranchOptions) option.Selected = option.Value == branchId?.ToString();
            foreach (var option in model.UserOptions) option.Selected = option.Value == userId?.ToString();
            if (detailsId.HasValue) model.Details = await BuildSalesReportDetailsAsync(detailsId.Value);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Return(string? receiptNo)
        {
            var model = new ReturnCreatePageViewModel { ReceiptNo = (receiptNo ?? string.Empty).Trim() };
            if (model.ReceiptNo.Length == 0) return View(model);
            var userId = CurrentUserId();
            if (!userId.HasValue) return Unauthorized();
            var userBranchId = await _db.Users.AsNoTracking().Where(x => x.Id == userId).Select(x => x.BranchId).FirstOrDefaultAsync();
            var sale = await _db.Sales.AsNoTracking().Where(x => x.ReceiptNo == model.ReceiptNo && x.Status == "Completed" && (!userBranchId.HasValue || x.BranchId == userBranchId))
                .Select(x => new ReturnSaleViewModel
                {
                    SaleId = x.Id, ReceiptNo = x.ReceiptNo,
                    Customer = x.Member == null ? "Walk-in" : x.Member.FullName + " (" + x.Member.MemberNo + ")",
                    Branch = x.Branch != null ? x.Branch.Name : "Unassigned", Cashier = x.User != null ? (x.User.FullName ?? x.User.Username) : "Unknown",
                    Products = x.ProductSales.Select(item => new ReturnableProductViewModel { ProductSaleId = item.Id, Product = item.Product != null ? item.Product.Name : "Unknown", Batch = item.Batch != null ? item.Batch.BatchNo : "Not recorded", Sold = item.Quantity, UnitPrice = item.UnitPrice > 0 ? item.UnitPrice : item.Price }).ToList(),
                    Fuels = x.FuelSales.Select(item => new ReturnableFuelViewModel { FuelSaleId = item.Id, Fuel = item.Fuel != null ? item.Fuel.Name : "Unknown", Source = (item.Tank != null ? "Tank " + item.Tank.TankNo : "") + (item.Pump != null ? " / Pump " + item.Pump.PumpNo : "") + (item.Nozzle != null ? " / Nozzle " + item.Nozzle.NozzleNo : ""), Sold = item.Liters, UnitPrice = item.PricePerLiter }).ToList()
                }).FirstOrDefaultAsync();
            if (sale is null) { ModelState.AddModelError(string.Empty, "No eligible completed Sale was found for this receipt and Branch."); return View(model); }
            var productIds = sale.Products.Select(x => x.ProductSaleId).ToList();
            var fuelIds = sale.Fuels.Select(x => x.FuelSaleId).ToList();
            var productReturned = await _db.CustomerProductReturnItems.AsNoTracking().Where(x => productIds.Contains(x.ProductSaleId) && x.CustomerReturn!.Status != "Rejected").GroupBy(x => x.ProductSaleId).Select(x => new { Id = x.Key, Quantity = x.Sum(y => y.Quantity) }).ToDictionaryAsync(x => x.Id, x => x.Quantity);
            var fuelReturned = await _db.CustomerFuelReturnItems.AsNoTracking().Where(x => fuelIds.Contains(x.FuelSaleId) && x.CustomerReturn!.Status != "Rejected").GroupBy(x => x.FuelSaleId).Select(x => new { Id = x.Key, Quantity = x.Sum(y => y.Liters) }).ToDictionaryAsync(x => x.Id, x => x.Quantity);
            sale.Products.ForEach(x => x.Returned = productReturned.GetValueOrDefault(x.ProductSaleId)); sale.Fuels.ForEach(x => x.Returned = fuelReturned.GetValueOrDefault(x.FuelSaleId));
            model.Sale = sale; return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReturn(int saleId, string reason, Dictionary<int, decimal>? productQuantities, Dictionary<int, decimal>? fuelQuantities)
        {
            var userId = CurrentUserId(); if (!userId.HasValue) return Unauthorized();
            reason = (reason ?? string.Empty).Trim(); if (reason.Length < 3 || reason.Length > 500) { TempData["ReturnError"] = "A reason between 3 and 500 characters is required."; return RedirectToAction(nameof(Return)); }
            productQuantities ??= new(); fuelQuantities ??= new();
            productQuantities = productQuantities.Where(x => x.Value > 0).ToDictionary(); fuelQuantities = fuelQuantities.Where(x => x.Value > 0).ToDictionary();
            if (productQuantities.Count == 0 && fuelQuantities.Count == 0) { TempData["ReturnError"] = "Select at least one item and enter a positive return quantity."; return RedirectToAction(nameof(Return)); }
            await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var userBranchId = await _db.Users.Where(x => x.Id == userId).Select(x => x.BranchId).FirstOrDefaultAsync();
                var sale = await _db.Sales.Include(x => x.ProductSales).Include(x => x.FuelSales).FirstOrDefaultAsync(x => x.Id == saleId && x.Status == "Completed" && x.BranchId.HasValue && (!userBranchId.HasValue || x.BranchId == userBranchId));
                if (sale is null) throw new InvalidOperationException("The Sale is not eligible for return in this Branch.");
                var productItems = new List<CustomerProductReturnItem>(); var fuelItems = new List<CustomerFuelReturnItem>();
                foreach (var pair in productQuantities)
                {
                    var original = sale.ProductSales.SingleOrDefault(x => x.Id == pair.Key) ?? throw new InvalidOperationException("A selected Product item is not part of the Sale.");
                    var prior = await _db.CustomerProductReturnItems.Where(x => x.ProductSaleId == original.Id && x.CustomerReturn!.Status != "Rejected").SumAsync(x => (decimal?)x.Quantity) ?? 0;
                    if (pair.Value > original.Quantity - prior) throw new InvalidOperationException("A Product return quantity exceeds the remaining returnable quantity.");
                    var price = original.UnitPrice > 0 ? original.UnitPrice : original.Price; productItems.Add(new CustomerProductReturnItem { ProductSaleId = original.Id, ProductId = original.ProductId, OriginalBatchId = original.BatchId, OriginalDisplayStockId = original.DisplayStockId, Quantity = pair.Value, OriginalUnitPrice = price, ReturnAmount = pair.Value * price });
                }
                foreach (var pair in fuelQuantities)
                {
                    var original = sale.FuelSales.SingleOrDefault(x => x.Id == pair.Key) ?? throw new InvalidOperationException("A selected Fuel item is not part of the Sale.");
                    var prior = await _db.CustomerFuelReturnItems.Where(x => x.FuelSaleId == original.Id && x.CustomerReturn!.Status != "Rejected").SumAsync(x => (decimal?)x.Liters) ?? 0;
                    if (pair.Value > original.Liters - prior) throw new InvalidOperationException("Returned Fuel liters exceed the remaining returnable liters.");
                    fuelItems.Add(new CustomerFuelReturnItem { FuelSaleId = original.Id, FuelId = original.FuelId, OriginalTankId = original.TankId, OriginalPumpId = original.PumpId, OriginalNozzleId = original.NozzleId, Liters = pair.Value, OriginalPricePerLiter = original.PricePerLiter, ReturnAmount = pair.Value * original.PricePerLiter });
                }
                var now = DateTime.UtcNow; var record = new CustomerReturn { ReturnNo = $"RET-{now:yyyyMMddHHmmssfff}", SaleId = sale.Id, OriginalReceiptNo = sale.ReceiptNo, BranchId = sale.BranchId!.Value, MemberId = sale.MemberId, ReturnType = productItems.Count > 0 && fuelItems.Count > 0 ? "Mixed" : productItems.Count > 0 ? "Product" : "Fuel", RefundAmount = productItems.Sum(x => x.ReturnAmount) + fuelItems.Sum(x => x.ReturnAmount), Reason = reason, Status = "Pending Inspection", CreatedByUserId = userId.Value, CreatedAt = now, ProductItems = productItems, FuelItems = fuelItems };
                _db.CustomerReturns.Add(record); await _db.SaveChangesAsync(); await transaction.CommitAsync(); TempData["ReturnMessage"] = $"Return {record.ReturnNo} was created for inspection."; return RedirectToAction(nameof(ReturnManagement), new { detailsId = record.Id });
            }
            catch (InvalidOperationException ex) { await transaction.RollbackAsync(); TempData["ReturnError"] = ex.Message; return RedirectToAction(nameof(Return)); }
        }

        public async Task<IActionResult> ReturnManagement(string? search, int? branchId, string? returnType, string? status, DateTime? from, DateTime? to, int? detailsId)
        {
            search = (search ?? "").Trim(); returnType = (returnType ?? "").Trim(); status = (status ?? "").Trim();
            if (returnType is not ("" or "Product" or "Fuel" or "Mixed")) returnType = string.Empty;
            if (status is not ("" or "Pending Inspection" or "Approved" or "Rejected" or "Completed")) status = string.Empty;
            if (from.HasValue && to.HasValue && from.Value.Date > to.Value.Date) ModelState.AddModelError(string.Empty, "From date cannot be later than To date.");
            var query = _db.CustomerReturns.AsNoTracking().AsQueryable();
            if (search.Length > 0) query = query.Where(x => x.ReturnNo.Contains(search) || x.OriginalReceiptNo.Contains(search) || (x.Member != null && (x.Member.MemberNo.Contains(search) || x.Member.FullName.Contains(search))));
            if (branchId.HasValue) query = query.Where(x => x.BranchId == branchId); if (returnType.Length > 0) query = query.Where(x => x.ReturnType == returnType); if (status.Length > 0) query = query.Where(x => x.Status == status);
            if (from.HasValue) query = query.Where(x => x.CreatedAt >= from.Value.Date); if (to.HasValue) { var until = to.Value.Date.AddDays(1); query = query.Where(x => x.CreatedAt < until); }
            var model = new ReturnManagementPageViewModel { Search = search, BranchId = branchId, ReturnType = returnType, Status = status, From = from, To = to, BranchOptions = await BuildBranchFilterOptionsAsync(), Rows = await query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).Select(x => new CustomerReturnRowViewModel { Id = x.Id, ReturnNo = x.ReturnNo, ReceiptNo = x.OriginalReceiptNo, ReturnDate = x.CreatedAt, Branch = x.Branch != null ? x.Branch.Name : "Unknown", Customer = x.Member == null ? "Walk-in" : x.Member.FullName + " (" + x.Member.MemberNo + ")", ReturnType = x.ReturnType, RefundAmount = x.RefundAmount, Status = x.Status }).ToListAsync() };
            foreach (var option in model.BranchOptions) option.Selected = option.Value == branchId?.ToString();
            if (detailsId.HasValue) model.Details = await BuildReturnDetailsAsync(detailsId.Value); return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InspectReturn(int id, string decision, string notes, Dictionary<int, string>? productResults, Dictionary<int, string>? fuelResults)
        {
            var userId = CurrentUserId(); if (!userId.HasValue) return Unauthorized(); notes = (notes ?? "").Trim();
            if (decision is not ("Approved" or "Rejected") || notes.Length < 3) { TempData["ReturnError"] = "Select Approved or Rejected and enter inspection notes."; return RedirectToAction(nameof(ReturnManagement), new { detailsId = id }); }
            var userBranchId = await _db.Users.AsNoTracking().Where(x => x.Id == userId).Select(x => x.BranchId).FirstOrDefaultAsync();
            var record = await _db.CustomerReturns.Include(x => x.ProductItems).Include(x => x.FuelItems).FirstOrDefaultAsync(x => x.Id == id && (!userBranchId.HasValue || x.BranchId == userBranchId));
            if (record is null) return NotFound(); if (record.Status != "Pending Inspection") { TempData["ReturnError"] = "Only pending Returns can be inspected."; return RedirectToAction(nameof(ReturnManagement), new { detailsId = id }); }
            productResults ??= new(); fuelResults ??= new();
            var productAllowed = new[] { "Restockable", "Damaged", "Expired", "Rejected", "Disposal" }; var fuelAllowed = new[] { "Approved for controlled handling", "Contaminated", "Wrong Fuel", "Water Suspected", "Rejected", "Disposal" };
            foreach (var item in record.ProductItems) { var result = productResults.GetValueOrDefault(item.Id); if (!productAllowed.Contains(result)) { TempData["ReturnError"] = "Choose an inspection result for every Product item."; return RedirectToAction(nameof(ReturnManagement), new { detailsId = id }); } item.InspectionResult = result; item.Disposition = result == "Restockable" && decision == "Approved" ? "Warehouse Stock" : result; }
            foreach (var item in record.FuelItems) { var result = fuelResults.GetValueOrDefault(item.Id); if (!fuelAllowed.Contains(result)) { TempData["ReturnError"] = "Choose an inspection result for every Fuel item."; return RedirectToAction(nameof(ReturnManagement), new { detailsId = id }); } item.InspectionResult = result; item.Disposition = result; }
            record.InspectedByUserId = userId; record.InspectedAt = DateTime.UtcNow; record.InspectionDecision = decision; record.InspectionNotes = notes; record.Status = decision;
            await _db.SaveChangesAsync(); TempData["ReturnMessage"] = $"Return {record.ReturnNo} was {decision.ToLowerInvariant()}."; return RedirectToAction(nameof(ReturnManagement), new { detailsId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessReturn(int id)
        {
            var userId = CurrentUserId(); if (!userId.HasValue) return Unauthorized();
            await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var userBranchId = await _db.Users.AsNoTracking().Where(x => x.Id == userId).Select(x => x.BranchId).FirstOrDefaultAsync();
                var record = await _db.CustomerReturns.Include(x => x.ProductItems).Include(x => x.FuelItems).FirstOrDefaultAsync(x => x.Id == id && (!userBranchId.HasValue || x.BranchId == userBranchId));
                if (record is null) return NotFound(); if (record.Status != "Approved" || record.CompletedAt.HasValue) throw new InvalidOperationException("Only an approved, unprocessed Return can be completed.");
                var now = DateTime.UtcNow;
                foreach (var item in record.ProductItems.Where(x => x.InspectionResult == "Restockable"))
                {
                    if (!item.OriginalBatchId.HasValue) throw new InvalidOperationException("A restockable Product has no original Batch reference.");
                    var stock = await _db.WarehouseStocks.FirstOrDefaultAsync(x => x.BranchId == record.BranchId && x.ProductId == item.ProductId && x.BatchId == item.OriginalBatchId);
                    if (stock is null) { stock = new WarehouseStock { BranchId = record.BranchId, ProductId = item.ProductId, BatchId = item.OriginalBatchId.Value, Quantity = 0, CreatedAt = now }; _db.WarehouseStocks.Add(stock); await _db.SaveChangesAsync(); }
                    var before = stock.Quantity; stock.Quantity += item.Quantity; stock.UpdatedAt = now;
                    var movement = new StockMovement { ProductId = item.ProductId, ProductBatchId = item.OriginalBatchId, BranchId = record.BranchId, BeforeQuantity = before, AfterQuantity = stock.Quantity, SourceLocation = "Customer Return", DestinationLocation = "Warehouse", MovementType = "CustomerReturn", Quantity = item.Quantity, ReferenceType = "CustomerReturn", ReferenceId = record.Id, Remarks = $"Approved restockable return {record.ReturnNo}", CreatedBy = userId, CreatedAt = now };
                    _db.StockMovements.Add(movement); await _db.SaveChangesAsync(); item.StockMovementId = movement.Id;
                }
                record.ProcessedByUserId = userId; record.CompletedAt = now; record.Status = "Completed"; await _db.SaveChangesAsync(); await transaction.CommitAsync(); TempData["ReturnMessage"] = $"Return {record.ReturnNo} was completed.";
            }
            catch (InvalidOperationException ex) { await transaction.RollbackAsync(); TempData["ReturnError"] = ex.Message; }
            return RedirectToAction(nameof(ReturnManagement), new { detailsId = id });
        }
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

        public async Task<IActionResult> WarehouseDailyStock(string? search, int? editId, int? detailsId, int? branchId, int? filterBranchId)
        {
            branchId = filterBranchId ?? branchId;
            return View(await BuildDailyStockPageAsync(WarehouseStockType, search, branchId, editId: editId, detailsId: detailsId, activeModalId: editId.HasValue ? DailyStockModalId : detailsId.HasValue ? "dailyStockDetailsModal" : string.Empty));
        }

        public async Task<IActionResult> DisplayDailyStock(string? search, int? editId, int? detailsId, int? branchId, int? filterBranchId)
        {
            branchId = filterBranchId ?? branchId;
            return View(await BuildDailyStockPageAsync(DisplayStockType, search, branchId, editId: editId, detailsId: detailsId, activeModalId: editId.HasValue ? DailyStockModalId : detailsId.HasValue ? "dailyStockDetailsModal" : string.Empty));
        }

        public async Task<IActionResult> TankDailyStock(string? search, int? editId, int? detailsId, int? branchId, int? filterBranchId)
        {
            branchId = filterBranchId ?? branchId;
            return View(await BuildDailyStockPageAsync(TankStockType, search, branchId, editId: editId, detailsId: detailsId, activeModalId: editId.HasValue ? DailyStockModalId : detailsId.HasValue ? "dailyStockDetailsModal" : string.Empty));
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
            var userId = CurrentUserId();
            if (!userId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Please sign in before recording Daily Stock.");
                return View(ActionViewFor(stockType), await BuildDailyStockPageAsync(stockType, search, filterBranchId, form, activeModalId: DailyStockModalId));
            }
            if (!await BranchExistsAsync(form.BranchId))
            {
                ModelState.AddModelError("Form.BranchId", "Branch is required.");
                return View(ActionViewFor(stockType), await BuildDailyStockPageAsync(stockType, search, filterBranchId, form, activeModalId: DailyStockModalId));
            }

            var selectedStock = await ValidateAndLoadSelectedStockAsync(stockType, form);
            if (selectedStock is null)
            {
                return View(ActionViewFor(stockType), await BuildDailyStockPageAsync(stockType, search, filterBranchId, form, activeModalId: DailyStockModalId));
            }

            var existing = form.Id > 0 ? await _db.DailyStockRecords.FirstOrDefaultAsync(item => item.Id == form.Id && item.StockType == stockType) : null;
            if (form.Id > 0 && existing is null) return NotFound();
            if (existing is not null && existing.Status != "Draft")
            {
                TempData["DailyStockError"] = "Confirmed Daily Stock records are read-only.";
                return RedirectToAction(redirectAction, new { search, filterBranchId });
            }

            var duplicate = await DailyStockScopeQuery(stockType, form.BranchId, form.ProductId, form.BatchId, form.TankId)
                .AnyAsync(item => item.StockDate == form.StockDate!.Value.Date && item.Id != form.Id && (item.Status == "Draft" || item.Status == "Confirmed" || item.Status == "Locked"));
            if (duplicate)
            {
                ModelState.AddModelError(string.Empty, "A Daily Stock record already exists for this date, branch, and stock item.");
                return View(ActionViewFor(stockType), await BuildDailyStockPageAsync(stockType, search, filterBranchId, form, activeModalId: DailyStockModalId));
            }

            var components = await BuildDailyStockComponentsAsync(stockType, form.StockDate!.Value.Date, form.BranchId, form.ProductId, form.BatchId, form.TankId, selectedStock);
            if (stockType == TankStockType && selectedStock is Tank draftTank && form.Ending > draftTank.CapacityLiters)
            {
                ModelState.AddModelError("Form.Ending", "Ending Liters cannot exceed Tank capacity.");
                return View(ActionViewFor(stockType), await BuildDailyStockPageAsync(stockType, search, filterBranchId, form, activeModalId: DailyStockModalId));
            }
            var record = form.Id > 0
                ? existing!
                : new DailyStockRecord { RecordNo = await GenerateDailyStockNoAsync(now), Status = "Draft", CreatedAt = now, CreatedBy = userId };

            record.StockType = stockType;
            record.BranchId = form.BranchId;
            record.StockDate = form.StockDate!.Value.Date;
            record.ProductId = stockType == TankStockType ? null : form.ProductId;
            record.BatchId = stockType == TankStockType ? null : form.BatchId;
            record.TankId = stockType == TankStockType ? form.TankId : null;
            record.FuelId = null;
            record.WarehouseStockId = stockType == WarehouseStockType ? form.WarehouseStockId : null;
            record.DisplayStockId = stockType == DisplayStockType ? form.DisplayStockId : null;
            if (stockType == TankStockType && form.TankId.HasValue)
            {
                record.FuelId = await _db.Tanks.AsNoTracking()
                    .Where(tank => tank.Id == form.TankId.Value)
                    .Select(tank => (int?)tank.FuelId)
                    .FirstOrDefaultAsync();
            }
            record.Beginning = components.Beginning;
            record.Received = components.Received;
            record.TransferIn = components.TransferIn;
            record.TransferOut = components.TransferOut;
            record.Sold = components.Sold;
            record.Adjustment = components.Adjustment;
            record.Expected = components.Expected;
            record.Actual = components.Expected;
            record.Ending = form.Ending!.Value;
            record.Variance = record.Ending - record.Expected;
            record.Loss = Math.Max(record.Expected - record.Ending, 0m);
            record.CurrentOfficialQuantity = components.CurrentOfficial;
            record.ReconciliationAdjustment = record.Ending - components.CurrentOfficial;
            record.NewOfficialQuantity = record.Ending;
            record.Remarks = CleanOptional(form.Remarks);
            record.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.DailyStockRecords.Add(record);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(redirectAction, new { search, filterBranchId });
        }

        private void NormalizeDailyStockForm(string stockType, DailyStockForm form)
        {
            ModelState.Remove("Form.Sold");
            ModelState.Remove("Form.Actual");
            ModelState.Remove("Form.Loss");
            ModelState.Remove("Form.Beginning");
            ModelState.Remove("Form.Received");
            ModelState.Remove("Form.TransferIn");
            ModelState.Remove("Form.TransferOut");
            ModelState.Remove("Form.Adjustment");
            ModelState.Remove("Form.Expected");
            ModelState.Remove("Form.Variance");
            ModelState.Remove("Form.CurrentOfficialQuantity");
            ModelState.Remove("Form.ReconciliationAdjustment");
            ModelState.Remove("Form.NewOfficialQuantity");

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

        private async Task<DailyStockPageViewModel> BuildDailyStockPageAsync(string stockType, string? search, int? branchId = null, DailyStockForm? form = null, int? editId = null, int? detailsId = null, string activeModalId = "")
        {
            var searchText = (search ?? string.Empty).Trim();
            var recordsQuery = _db.DailyStockRecords
                .AsNoTracking()
                .Include(record => record.Branch)
                .Include(record => record.Product)
                .Include(record => record.Batch)
                .Include(record => record.Tank)
                .ThenInclude(tank => tank!.Fuel)
                .Include(record => record.CreatedByUser)
                .Include(record => record.ConfirmedByUser)
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

            var details = detailsId.HasValue
                ? await recordsQuery.FirstOrDefaultAsync(record => record.Id == detailsId.Value)
                : null;
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
                Details = details,
                AdjustmentBreakdown = details is null ? new() : await BuildDailyStockAdjustmentBreakdownAsync(details),
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
                Received = record.Received,
                TransferIn = record.TransferIn,
                TransferOut = record.TransferOut,
                Sold = record.Sold,
                Adjustment = record.Adjustment,
                Expected = record.Expected,
                Actual = record.Actual,
                Ending = record.Ending,
                Loss = record.Loss,
                Variance = record.Variance,
                CurrentOfficialQuantity = record.CurrentOfficialQuantity,
                ReconciliationAdjustment = record.ReconciliationAdjustment,
                NewOfficialQuantity = record.NewOfficialQuantity,
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
            object? selectedStock = stockInfo is null ? null : stockType == WarehouseStockType
                ? await _db.WarehouseStocks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == stockInfo.Id)
                : await _db.DisplayStocks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == stockInfo.Id);
            var components = selectedStock is null
                ? new DailyStockComponents(previousEnding ?? currentQuantity, 0m, 0m, 0m, sold, 0m, previousEnding ?? currentQuantity, currentQuantity)
                : await BuildDailyStockComponentsAsync(stockType, date, branchId ?? 0, productId, batchId, null, selectedStock);

            return new
            {
                batchId,
                branchId,
                productId = productId ?? batch?.ProductId ?? 0,
                productName = batch?.Product?.Name ?? string.Empty,
                beginning = components.Beginning,
                received = components.Received,
                transferIn = components.TransferIn,
                transferOut = components.TransferOut,
                sold = components.Sold,
                adjustment = components.Adjustment,
                expected = components.Expected,
                currentOfficial = components.CurrentOfficial,
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
            var components = tank is null
                ? new DailyStockComponents(previousEnding ?? 0m, 0m, 0m, 0m, sold, 0m, previousEnding ?? 0m, 0m)
                : await BuildDailyStockComponentsAsync(TankStockType, date, effectiveBranchId ?? 0, null, null, tankId, tank);

            return new
            {
                tankId,
                branchId = effectiveBranchId,
                fuelId = tank?.FuelId ?? 0,
                fuelName = tank?.Fuel?.Name ?? string.Empty,
                beginning = components.Beginning,
                received = components.Received,
                transferIn = components.TransferIn,
                transferOut = components.TransferOut,
                sold = components.Sold,
                adjustment = components.Adjustment,
                expected = components.Expected,
                currentOfficial = components.CurrentOfficial,
                capacity = tank?.CapacityLiters ?? 0m,
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDailyStock(int id, string stockType, string? search, int? filterBranchId)
        {
            var redirectAction = ActionViewFor(stockType);
            var userId = CurrentUserId();
            if (!userId.HasValue) return Unauthorized();
            await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var record = await _db.DailyStockRecords.FirstOrDefaultAsync(x => x.Id == id && x.StockType == stockType)
                    ?? throw new InvalidOperationException("Daily Stock record was not found.");
                if (record.Status != "Draft") throw new InvalidOperationException("Only a Draft Daily Stock record can be confirmed.");
                if (await DailyStockScopeQuery(stockType, record.BranchId ?? 0, record.ProductId, record.BatchId, record.TankId)
                    .AnyAsync(x => x.Id != record.Id && (x.Status == "Confirmed" || x.Status == "Locked") && x.StockDate >= record.StockDate))
                    throw new InvalidOperationException("A newer or duplicate confirmed Daily Stock record exists. This Draft cannot overwrite current inventory.");

                var selectedStock = await LoadDailyStockTargetAsync(record) ?? throw new InvalidOperationException("The selected stock item no longer exists.");
                var components = await BuildDailyStockComponentsAsync(stockType, record.StockDate, record.BranchId ?? 0, record.ProductId, record.BatchId, record.TankId, selectedStock);
                if (components.Beginning != record.Beginning || components.Received != record.Received
                    || components.TransferIn != record.TransferIn || components.TransferOut != record.TransferOut
                    || components.Sold != record.Sold || components.Adjustment != record.Adjustment)
                    throw new InvalidOperationException("Inventory or source movements changed after this Draft was reviewed. Reopen and save the Draft to recalculate it.");
                if (stockType == TankStockType && selectedStock is Tank capacityTank && record.Ending > capacityTank.CapacityLiters)
                    throw new InvalidOperationException("Ending Liters cannot exceed Tank capacity.");

                record.Expected = components.Expected;
                record.Actual = components.Expected;
                record.Variance = record.Ending - record.Expected;
                record.Loss = Math.Max(record.Expected - record.Ending, 0m);
                record.CurrentOfficialQuantity = components.CurrentOfficial;
                record.ReconciliationAdjustment = record.Ending - components.CurrentOfficial;
                record.NewOfficialQuantity = record.Ending;
                if (record.ReconciliationAdjustment != 0m)
                {
                    if (stockType == TankStockType) await ApplyTankDailyStockReconciliationAsync(record, (Tank)selectedStock, userId.Value, DateTime.UtcNow);
                    else ApplyProductDailyStockReconciliation(record, selectedStock, userId.Value, DateTime.UtcNow);
                    AddDailyStockAdjustmentAudit(record, selectedStock, userId.Value, DateTime.UtcNow);
                }
                record.Status = "Confirmed";
                record.ConfirmedBy = userId;
                record.ConfirmedAt = DateTime.UtcNow;
                record.UpdatedAt = record.ConfirmedAt;
                var affectedRows = await _db.SaveChangesAsync();
                await VerifyDailyStockInventoryPersistedAsync(record);
                await transaction.CommitAsync();
                _logger?.LogInformation("Confirmed Daily Stock {RecordId} ({RecordNo}); stock type {StockType}, branch {BranchId}, target W:{WarehouseStockId} D:{DisplayStockId} T:{TankId}, current {CurrentOfficial}, ending {Ending}, reconciliation {Reconciliation}, SaveChanges entries {AffectedRows}.", record.Id, record.RecordNo, record.StockType, record.BranchId, record.WarehouseStockId, record.DisplayStockId, record.TankId, record.CurrentOfficialQuantity, record.Ending, record.ReconciliationAdjustment, affectedRows);
                if (Request.GetTypedHeaders().Accept?.Any(x => x.MediaType.HasValue && x.MediaType.Value.Equals("application/json", StringComparison.OrdinalIgnoreCase)) == true)
                    return Json(new { success = true, message = $"{record.RecordNo} was confirmed and inventory was reconciled.", affectedRows });
                TempData["DailyStockMessage"] = $"{record.RecordNo} was confirmed and inventory was reconciled.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger?.LogError(ex, "Daily Stock confirmation failed for record {RecordId}, requested stock type {StockType}, filter branch {FilterBranchId}.", id, stockType, filterBranchId);
                if (Request.GetTypedHeaders().Accept?.Any(x => x.MediaType.HasValue && x.MediaType.Value.Equals("application/json", StringComparison.OrdinalIgnoreCase)) == true)
                    return BadRequest(new { success = false, message = ex.Message });
                TempData["DailyStockError"] = ex.Message;
            }
            return RedirectToAction(redirectAction, new { search, filterBranchId });
        }

        private IQueryable<DailyStockRecord> DailyStockScopeQuery(string stockType, int branchId, int? productId, int? batchId, int? tankId)
            => _db.DailyStockRecords.Where(x => x.StockType == stockType && x.BranchId == branchId
                && (stockType == TankStockType ? x.TankId == tankId : x.ProductId == productId && x.BatchId == batchId));

        private async Task<object?> LoadDailyStockTargetAsync(DailyStockRecord record)
        {
            if (record.StockType == WarehouseStockType)
                return await _db.WarehouseStocks.AsTracking().FirstOrDefaultAsync(x => x.Id == record.WarehouseStockId && x.BranchId == record.BranchId && x.ProductId == record.ProductId && x.BatchId == record.BatchId);
            if (record.StockType == DisplayStockType)
                return await _db.DisplayStocks.AsTracking().FirstOrDefaultAsync(x => x.Id == record.DisplayStockId && x.BranchId == record.BranchId && x.ProductId == record.ProductId && x.BatchId == record.BatchId);
            return await _db.Tanks.AsTracking().FirstOrDefaultAsync(x => x.Id == record.TankId && x.BranchId == record.BranchId);
        }

        private async Task<DailyStockComponents> BuildDailyStockComponentsAsync(string stockType, DateTime date, int branchId, int? productId, int? batchId, int? tankId, object selectedStock)
        {
            date = date.Date;
            var end = date.AddDays(1);
            var current = selectedStock is WarehouseStock w ? w.Quantity : selectedStock is DisplayStock d ? d.Quantity : ((Tank)selectedStock).CurrentLiters;
            var previous = await DailyStockScopeQuery(stockType, branchId, productId, batchId, tankId)
                .AsNoTracking().Where(x => (x.Status == "Confirmed" || x.Status == "Locked" || x.Status == "Legacy") && x.StockDate < date)
                .OrderByDescending(x => x.StockDate).ThenByDescending(x => x.Id).Select(x => (decimal?)x.Ending).FirstOrDefaultAsync();
            decimal received = 0m, transferIn = 0m, transferOut = 0m, sold = 0m, adjustment = 0m;
            if (stockType == TankStockType)
            {
                received = await _db.FuelDeliveries.AsNoTracking().Where(x => x.TankId == tankId && x.BranchId == branchId && x.Status == 1 && x.DeliveryDate >= date && x.DeliveryDate < end).SumAsync(x => (decimal?)x.DeliveredLiters) ?? 0m;
                sold = await ComputeSoldAsync(stockType, date, null, tankId, branchId);
                adjustment = await _db.StockAdjustments.AsNoTracking().Where(x => x.Scope == "Fuel" && x.TankId == tankId && x.BranchId == branchId && x.Status == "Posted" && x.BusinessDate == date).SumAsync(x => (decimal?)x.SignedQuantity) ?? 0m;
            }
            else
            {
                received = stockType == WarehouseStockType
                    ? await _db.StockMovements.AsNoTracking().Where(x => x.ProductBatchId == batchId && x.MovementType == "Receiving" && x.CreatedAt >= date && x.CreatedAt < end && _db.StockReceivings.Any(r => r.Id == x.ReferenceId && r.BranchId == branchId && r.Status == 1)).SumAsync(x => (decimal?)x.Quantity) ?? 0m
                    : 0m;
                transferIn = await _db.StockTransfers.AsNoTracking().Where(x => x.Status == "Completed" && x.CompletedAt >= date && x.CompletedAt < end && x.DestinationBranchId == branchId && x.DestinationLocation == stockType)
                    .SelectMany(x => x.Items).Where(x => x.ProductId == productId && x.BatchId == batchId).SumAsync(x => x.Quantity) ?? 0m;
                transferOut = await _db.StockTransfers.AsNoTracking().Where(x => x.Status == "Completed" && x.CompletedAt >= date && x.CompletedAt < end && x.SourceBranchId == branchId && x.SourceLocation == stockType)
                    .SelectMany(x => x.Items).Where(x => x.ProductId == productId && x.BatchId == batchId).SumAsync(x => x.Quantity) ?? 0m;
                sold = stockType == DisplayStockType ? await ComputeSoldAsync(stockType, date, batchId, null, branchId) : 0m;
                adjustment = await _db.StockAdjustments.AsNoTracking().Where(x => x.Scope == stockType && x.BranchId == branchId && x.BatchId == batchId && x.ProductId == productId && x.Status == "Posted" && x.BusinessDate == date).SumAsync(x => (decimal?)x.SignedQuantity) ?? 0m;
                adjustment += await _db.StockMovements.AsNoTracking().Where(x => x.ProductBatchId == batchId && x.BranchId == branchId && x.CreatedAt >= date && x.CreatedAt < end && x.MovementType == "CustomerReturn" && x.DestinationLocation == stockType).SumAsync(x => (decimal?)x.Quantity) ?? 0m;
            }
            var beginning = previous ?? current;
            var expected = beginning + received + transferIn - transferOut - sold + adjustment;
            return new DailyStockComponents(beginning, received, transferIn, transferOut, sold, adjustment, expected, current);
        }

        private void ApplyProductDailyStockReconciliation(DailyStockRecord record, object target, int userId, DateTime now)
        {
            if (target is WarehouseStock warehouse)
            {
                warehouse.Quantity = record.Ending;
                warehouse.UpdatedAt = now;
                _db.Entry(warehouse).Property(x => x.Quantity).IsModified = true;
                _db.Entry(warehouse).Property(x => x.UpdatedAt).IsModified = true;
            }
            else
            {
                var display = (DisplayStock)target;
                display.Quantity = record.Ending;
                display.UpdatedAt = now;
                _db.Entry(display).Property(x => x.Quantity).IsModified = true;
                _db.Entry(display).Property(x => x.UpdatedAt).IsModified = true;
            }
            record.NewOfficialQuantity = record.Ending;
        }

        private async Task ApplyTankDailyStockReconciliationAsync(DailyStockRecord record, Tank tank, int userId, DateTime now)
        {
            var difference = record.ReconciliationAdjustment;
            var batches = await _db.FuelBatches.Where(x => x.TankId == tank.Id && x.BranchId == tank.BranchId && x.Status == 1 && x.IsActive)
                .OrderBy(x => x.ReceivedDate).ThenBy(x => x.Id).ToListAsync();
            if (difference > 0m)
            {
                var batch = batches.LastOrDefault() ?? throw new InvalidOperationException("Tank overage cannot be confirmed because no active Fuel cost layer exists.");
                var beforeBatch = batch.RemainingLiters; batch.RemainingLiters += difference; batch.UpdatedAt = now;
                _db.FuelStockMovements.Add(NewFuelDailyStockMovement(record, tank, batch, difference, 0m, beforeBatch, batch.RemainingLiters, tank.CurrentLiters, record.Ending, userId, now));
            }
            else
            {
                var remaining = -difference;
                var runningTank = tank.CurrentLiters;
                foreach (var batch in batches.Where(x => x.RemainingLiters > 0m))
                {
                    if (remaining <= 0m) break;
                    var used = Math.Min(remaining, batch.RemainingLiters); var beforeBatch = batch.RemainingLiters; batch.RemainingLiters -= used; batch.UpdatedAt = now;
                    var tankAfter = runningTank - used;
                    _db.FuelStockMovements.Add(NewFuelDailyStockMovement(record, tank, batch, 0m, used, beforeBatch, batch.RemainingLiters, runningTank, tankAfter, userId, now));
                    runningTank = tankAfter;
                    remaining -= used;
                }
                if (remaining > 0m) throw new InvalidOperationException("Active Fuel Batch layers are insufficient for this Tank shortage reconciliation.");
            }
            tank.CurrentLiters = record.Ending;
            tank.UpdatedAt = now;
            _db.Entry(tank).Property(x => x.CurrentLiters).IsModified = true;
            _db.Entry(tank).Property(x => x.UpdatedAt).IsModified = true;
        }

        private async Task VerifyDailyStockInventoryPersistedAsync(DailyStockRecord record)
        {
            decimal? persisted = record.StockType switch
            {
                WarehouseStockType => await _db.WarehouseStocks.AsNoTracking()
                    .Where(x => x.Id == record.WarehouseStockId).Select(x => (decimal?)x.Quantity).SingleOrDefaultAsync(),
                DisplayStockType => await _db.DisplayStocks.AsNoTracking()
                    .Where(x => x.Id == record.DisplayStockId).Select(x => (decimal?)x.Quantity).SingleOrDefaultAsync(),
                TankStockType => await _db.Tanks.AsNoTracking()
                    .Where(x => x.Id == record.TankId).Select(x => (decimal?)x.CurrentLiters).SingleOrDefaultAsync(),
                _ => null
            };
            if (!persisted.HasValue || persisted.Value != record.Ending)
                throw new InvalidOperationException($"Inventory reconciliation did not persist. Expected {record.Ending:N2}, but the inventory row contains {(persisted.HasValue ? persisted.Value.ToString("N2") : "no value")}.");
        }

        private static FuelStockMovement NewFuelDailyStockMovement(DailyStockRecord record, Tank tank, FuelBatch batch, decimal litersIn, decimal litersOut, decimal batchBefore, decimal batchAfter, decimal tankBefore, decimal tankAfter, int userId, DateTime now)
            => new() { TankId = tank.Id, FuelId = tank.FuelId, FuelBatchId = batch.Id, BranchId = record.BranchId!.Value, MovementType = litersIn > 0 ? "DailyStockIncrease" : "DailyStockDecrease", LitersIn = litersIn, LitersOut = litersOut, BatchLitersBefore = batchBefore, BatchLitersAfter = batchAfter, TankLitersBefore = tankBefore, TankLitersAfter = tankAfter, UnitCostSnapshot = batch.CostPricePerLiter, ReferenceType = "DailyStockRecord", ReferenceId = record.Id, Remarks = $"Daily Stock reconciliation {record.RecordNo}", CreatedByUserId = userId, CreatedAt = now };

        private void AddDailyStockAdjustmentAudit(DailyStockRecord record, object target, int userId, DateTime now)
        {
            _db.StockAdjustments.Add(new StockAdjustment { AdjustmentNo = $"{record.RecordNo}-ADJ", Scope = record.StockType == TankStockType ? "Fuel" : record.StockType, BusinessDate = record.StockDate, BranchId = record.BranchId!.Value, WarehouseStockId = record.WarehouseStockId, DisplayStockId = record.DisplayStockId, TankId = record.TankId, ProductId = record.ProductId, BatchId = record.BatchId, FuelId = record.FuelId, AdjustmentType = record.ReconciliationAdjustment > 0 ? "Increase" : "Decrease", BeforeQuantity = record.CurrentOfficialQuantity, AdjustmentQuantity = Math.Abs(record.ReconciliationAdjustment), SignedQuantity = record.ReconciliationAdjustment, AfterQuantity = record.Ending, Reason = $"Daily Stock reconciliation {record.RecordNo}", Remarks = record.Remarks, Status = "Posted", AdjustedBy = userId, CreatedAt = now, UpdatedAt = now, PostedBy = userId, PostedAt = now });
            if (record.StockType != TankStockType)
                _db.StockMovements.Add(new StockMovement { ProductId = record.ProductId!.Value, ProductBatchId = record.BatchId, BranchId = record.BranchId, BeforeQuantity = record.CurrentOfficialQuantity, AfterQuantity = record.Ending, SourceLocation = record.StockType, DestinationLocation = record.StockType, MovementType = record.ReconciliationAdjustment > 0 ? "DailyStockIncrease" : "DailyStockDecrease", Quantity = Math.Abs(record.ReconciliationAdjustment), ReferenceType = "DailyStockRecord", ReferenceId = record.Id, Remarks = $"Daily Stock reconciliation {record.RecordNo}", CreatedBy = userId, CreatedAt = now });
        }

        private async Task<string> GenerateDailyStockNoAsync(DateTime now)
        {
            var prefix = $"DS-{now:yyyyMMdd}-";
            var last = await _db.DailyStockRecords.Where(x => x.RecordNo.StartsWith(prefix)).OrderByDescending(x => x.Id).Select(x => x.RecordNo).FirstOrDefaultAsync();
            var next = last is not null && int.TryParse(last[(last.LastIndexOf('-') + 1)..], out var value) ? value + 1 : 1;
            return $"{prefix}{next:000000}";
        }

        private async Task<List<DailyStockAdjustmentBreakdownRow>> BuildDailyStockAdjustmentBreakdownAsync(DailyStockRecord record)
        {
            var rows = await _db.StockAdjustments.AsNoTracking().Where(x => x.Status == "Posted" && x.BusinessDate == record.StockDate && x.BranchId == record.BranchId
                && x.AdjustmentNo != record.RecordNo + "-ADJ"
                && (record.StockType == TankStockType ? x.TankId == record.TankId : x.Scope == record.StockType && x.ProductId == record.ProductId && x.BatchId == record.BatchId))
                .OrderBy(x => x.PostedAt).Select(x => new DailyStockAdjustmentBreakdownRow { Date = x.PostedAt ?? x.CreatedAt, Type = x.AdjustmentType, Direction = x.SignedQuantity >= 0 ? "In" : "Out", Quantity = Math.Abs(x.SignedQuantity), Reason = x.Reason, Reference = x.AdjustmentNo }).ToListAsync();
            if (record.StockType != TankStockType)
            {
                rows.AddRange(await _db.StockMovements.AsNoTracking().Where(x => x.ProductBatchId == record.BatchId && x.BranchId == record.BranchId && x.CreatedAt >= record.StockDate && x.CreatedAt < record.StockDate.AddDays(1) && x.MovementType == "CustomerReturn" && x.DestinationLocation == record.StockType)
                    .Select(x => new DailyStockAdjustmentBreakdownRow { Date = x.CreatedAt ?? record.StockDate, Type = "Approved Return", Direction = "In", Quantity = x.Quantity, Reason = x.Remarks ?? "Approved customer return", Reference = (x.ReferenceType ?? "CustomerReturn") + " #" + x.ReferenceId }).ToListAsync());
            }
            return rows.OrderBy(x => x.Date).ToList();
        }

        private sealed record DailyStockComponents(decimal Beginning, decimal Received, decimal TransferIn, decimal TransferOut, decimal Sold, decimal Adjustment, decimal Expected, decimal CurrentOfficial);

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

        private async Task<CustomerReturnDetailsViewModel?> BuildReturnDetailsAsync(int id)
        {
            return await _db.CustomerReturns.AsNoTracking().Where(x => x.Id == id).Select(x => new CustomerReturnDetailsViewModel
            {
                Id = x.Id, ReturnNo = x.ReturnNo, ReceiptNo = x.OriginalReceiptNo, ReturnDate = x.CreatedAt, Branch = x.Branch != null ? x.Branch.Name : "Unknown", Customer = x.Member == null ? "Walk-in" : x.Member.FullName + " (" + x.Member.MemberNo + ")", ReturnType = x.ReturnType, RefundAmount = x.RefundAmount, Status = x.Status, Reason = x.Reason,
                CreatedBy = x.CreatedByUser != null ? (x.CreatedByUser.FullName ?? x.CreatedByUser.Username) : "Unknown", Inspector = x.InspectedByUser != null ? (x.InspectedByUser.FullName ?? x.InspectedByUser.Username) : "", InspectedAt = x.InspectedAt, Decision = x.InspectionDecision ?? "", InspectionNotes = x.InspectionNotes ?? "", ProcessedBy = x.ProcessedByUser != null ? (x.ProcessedByUser.FullName ?? x.ProcessedByUser.Username) : "", CompletedAt = x.CompletedAt,
                Products = x.ProductItems.Select(item => new CustomerProductReturnDetailViewModel { Id = item.Id, Product = item.Product != null ? item.Product.Name : "Unknown", Batch = item.OriginalBatch != null ? item.OriginalBatch.BatchNo : "Not recorded", Quantity = item.Quantity, UnitPrice = item.OriginalUnitPrice, Amount = item.ReturnAmount, InspectionResult = item.InspectionResult ?? "Pending", Disposition = item.Disposition ?? "Pending" }).ToList(),
                Fuels = x.FuelItems.Select(item => new CustomerFuelReturnDetailViewModel { Id = item.Id, Fuel = item.Fuel != null ? item.Fuel.Name : "Unknown", Source = (item.OriginalTank != null ? "Tank " + item.OriginalTank.TankNo : "") + (item.OriginalPump != null ? " / Pump " + item.OriginalPump.PumpNo : "") + (item.OriginalNozzle != null ? " / Nozzle " + item.OriginalNozzle.NozzleNo : ""), Liters = item.Liters, UnitPrice = item.OriginalPricePerLiter, Amount = item.ReturnAmount, InspectionResult = item.InspectionResult ?? "Pending", Disposition = item.Disposition ?? "Pending" }).ToList()
            }).FirstOrDefaultAsync();
        }

        private async Task<SalesReportDetailsViewModel?> BuildSalesReportDetailsAsync(int saleId)
        {
            var details = await _db.Sales.AsNoTracking().Where(sale => sale.Id == saleId && (sale.Status == "Completed" || sale.Status == "Voided"))
                .Select(sale => new SalesReportDetailsViewModel
                {
                    Id = sale.Id, ReceiptNo = sale.ReceiptNo, SaleDate = sale.BusinessDate ?? sale.CreatedAt,
                    Branch = sale.Branch != null ? sale.Branch.Name : "Unassigned",
                    User = sale.User != null ? (sale.User.FullName ?? sale.User.Username) : "Unknown",
                    HasProducts = sale.ProductSales.Any(), HasFuel = sale.FuelSales.Any(),
                    PaymentMethod = sale.Payments.OrderBy(payment => payment.Id).Select(payment => payment.PaymentType).FirstOrDefault() ?? "Not recorded",
                    OriginalTotal = sale.NetTotal, Status = sale.Status,
                    SaleVoidId = sale.Voids.OrderByDescending(item => item.CompletedAt ?? item.CreatedAt).Select(item => (int?)item.Id).FirstOrDefault(),
                    VoidDate = sale.Voids.OrderByDescending(item => item.CompletedAt ?? item.CreatedAt).Select(item => (DateTime?)(item.CompletedAt ?? item.CreatedAt)).FirstOrDefault(),
                    VoidedBy = sale.Voids.OrderByDescending(item => item.CompletedAt ?? item.CreatedAt).Select(item => item.RequestedByUser != null ? (item.RequestedByUser.FullName ?? item.RequestedByUser.Username) : "Unknown").FirstOrDefault() ?? string.Empty,
                    VoidReason = sale.Voids.OrderByDescending(item => item.CompletedAt ?? item.CreatedAt).Select(item => item.Reason).FirstOrDefault() ?? string.Empty,
                    SaleVoidStatus = sale.Voids.OrderByDescending(item => item.CompletedAt ?? item.CreatedAt).Select(item => item.Status).FirstOrDefault() ?? string.Empty,
                    ProductItems = sale.ProductSales.OrderBy(item => item.Id).Select(item => new SalesReportProductItemViewModel
                    {
                        Product = item.Product != null ? item.Product.Name : "Unknown", BatchNo = item.Batch != null ? item.Batch.BatchNo : string.Empty,
                        Quantity = item.Quantity, SellingPrice = item.UnitPrice > 0 ? item.UnitPrice : item.Price, LineTotal = item.Subtotal
                    }).ToList(),
                    FuelItems = sale.FuelSales.OrderBy(item => item.Id).Select(item => new SalesReportFuelItemViewModel
                    {
                        Fuel = item.Fuel != null ? item.Fuel.Name : "Unknown", Tank = item.Tank != null ? item.Tank.TankNo : string.Empty,
                        Pump = item.Pump != null ? item.Pump.PumpNo : string.Empty, Liters = item.Liters, PricePerLiter = item.PricePerLiter, LineTotal = item.Subtotal
                    }).ToList()
                }).FirstOrDefaultAsync();
            return details;
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

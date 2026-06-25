using gpos.Filters;
using gpos.Data;
using gpos.Models;
using gpos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class TransactionController : Controller
    {
        private static readonly string[] SaleStatuses = ["Completed", "Voided", "Returned", "Cancelled"];
        private readonly ApplicationDbContext _db;

        public TransactionController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult POS() => View();

        [HttpGet]
        public async Task<IActionResult> MemberDiscount(string? membershipCardNo, decimal productTotal = 0m, decimal fuelTotal = 0m)
        {
            var member = await FindActiveMemberByCard(membershipCardNo);
            if (member is null)
            {
                return Ok(new { success = true, memberFound = false, discountAmount = 0m });
            }

            var now = DateTime.Now;
            var discountAmount = await CalculateMemberDiscount(member, Math.Max(0m, productTotal), Math.Max(0m, fuelTotal), now);

            return Ok(new { success = true, memberFound = true, discountAmount });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitSale([FromBody] PosSaleRequest? request)
        {
            if (request is null)
            {
                return BadRequest(new { success = false, message = "Sale payload is missing or invalid." });
            }

            var userId = CurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(new { success = false, message = "Please sign in before submitting a sale." });
            }

            if (request.Products.Count == 0 && request.Fuels.Count == 0)
            {
                return BadRequest(new { success = false, message = "Cart is empty. Add at least one product or fuel item." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Please check the cart quantities and amounts." });
            }

            await using var dbTransaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.Now;
                var productItems = await BuildValidatedProductItems(request.Products);
                var fuelItems = await BuildValidatedFuelItems(request.Fuels);
                var productTotal = productItems.Sum(item => item.Subtotal);
                var fuelTotal = fuelItems.Sum(item => item.Subtotal);
                var grossTotal = productTotal + fuelTotal;

                if (grossTotal <= 0)
                {
                    return BadRequest(new { success = false, message = "Sale total must be greater than zero." });
                }

                var member = await FindMember(request.MembershipCardNo);
                var discountAmount = await CalculateMemberDiscount(member, productTotal, fuelTotal, now);
                var rebate = await FindRebate(request.RebateRuleId);
                var rebateAmount = ValidateAndCalculateRebate(member, rebate, grossTotal, productItems.Count > 0, fuelItems.Count > 0);
                var netTotal = Math.Max(0m, grossTotal - discountAmount - rebateAmount);
                var cashAmount = Math.Max(0m, request.CashAmount);
                var pointsEarned = CalculateEarnedPoints(member, netTotal);

                if (cashAmount < netTotal)
                {
                    return BadRequest(new { success = false, message = "Cash amount is not enough to checkout." });
                }

                await DeductDisplayStock(productItems, userId.Value);
                await DeductTankFuel(fuelItems);

                var sale = new Sale
                {
                    ReceiptNo = await GenerateReceiptNo(now),
                    UserId = userId.Value,
                    MemberId = member?.Id,
                    GrossTotal = grossTotal,
                    DiscountAmount = discountAmount,
                    RebateAmount = rebateAmount,
                    NetTotal = netTotal,
                    CashAmount = cashAmount,
                    Status = "Completed",
                    CreatedAt = now,
                    UpdatedAt = now
                };

                _db.Sales.Add(sale);
                await _db.SaveChangesAsync();

                for (var i = 0; i < productItems.Count; i += 1)
                {
                    productItems[i].SaleId = sale.Id;
                    productItems[i].StockMovement.ReferenceId = sale.Id;
                    _db.ProductSales.Add(productItems[i].ProductSale);
                    _db.StockMovements.Add(productItems[i].StockMovement);
                }

                for (var i = 0; i < fuelItems.Count; i += 1)
                {
                    fuelItems[i].FuelSale.SaleId = sale.Id;
                    _db.FuelSales.Add(fuelItems[i].FuelSale);
                }

                _db.Payments.Add(new Payment
                {
                    SaleId = sale.Id,
                    PaymentType = PaymentType(cashAmount, rebateAmount, netTotal),
                    Amount = cashAmount,
                    Status = "Completed",
                    CreatedAt = now,
                    UpdatedAt = now
                });

                await ApplyPointsChanges(member, rebate, rebateAmount, pointsEarned, sale.Id, now);

                await _db.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return Ok(new
                {
                    success = true,
                    message = "Sale saved successfully.",
                    receiptNo = sale.ReceiptNo,
                    grossTotal,
                    discountAmount,
                    rebateAmount,
                    netTotal,
                    pointsEarned,
                    amountTendered = cashAmount,
                    change = Math.Max(0m, cashAmount - netTotal)
                });
            }
            catch (InvalidOperationException ex)
            {
                await dbTransaction.RollbackAsync();
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> ProductSales(string? search, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            var normalizedStatus = NormalizeStatus(status);
            var query = BuildProductSalesQuery(search, dateFrom, dateTo, normalizedStatus);
            var items = await query
                .OrderByDescending(item => item.Sale!.CreatedAt ?? item.CreatedAt)
                .ThenByDescending(item => item.Id)
                .Take(100)
                .ToListAsync();

            return View(new ProductSalesPageViewModel
            {
                Search = (search ?? string.Empty).Trim(),
                DateFrom = dateFrom,
                DateTo = dateTo,
                Status = normalizedStatus ?? string.Empty,
                StatusOptions = BuildStatusOptions(normalizedStatus),
                Sales = items.Select(ToProductSaleRow).ToList()
            });
        }

        public async Task<IActionResult> FuelSales(string? search, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            var normalizedStatus = NormalizeStatus(status);
            var query = BuildFuelSalesQuery(search, dateFrom, dateTo, normalizedStatus);
            var items = await query
                .OrderByDescending(item => item.Sale!.CreatedAt ?? item.CreatedAt)
                .ThenByDescending(item => item.Id)
                .Take(100)
                .ToListAsync();

            return View(new FuelSalesPageViewModel
            {
                Search = (search ?? string.Empty).Trim(),
                DateFrom = dateFrom,
                DateTo = dateTo,
                Status = normalizedStatus ?? string.Empty,
                StatusOptions = BuildStatusOptions(normalizedStatus),
                Sales = items.Select(ToFuelSaleRow).ToList()
            });
        }

        public IActionResult DailyCash() => View();
        public IActionResult CashIn() => View();
        public IActionResult CashOut() => View();
        public IActionResult ProductReceiving() => View();
        public IActionResult FuelReceiving() => View();
        public IActionResult WarehouseToDisplay() => View();
        public IActionResult DisplayToWarehouse() => View();
        public IActionResult BranchProductTransfer() => View();
        public IActionResult BranchFuelTransfer() => View();
        public IActionResult ProductReturn() => View();
        public IActionResult FuelReturn() => View();
        public IActionResult VoidSale() => View();
        public IActionResult VoidProduct() => View();
        public IActionResult VoidFuel() => View();
        public IActionResult DisplayStockAdjustment() => View();
        public IActionResult WarehouseStockAdjustment() => View();
        public IActionResult TankFuelAdjustment() => View();
        public IActionResult ProductPriceAdjustment() => View();
        public IActionResult FuelPriceAdjustment() => View();

        private IQueryable<ProductSale> BuildProductSalesQuery(string? search, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            var query = _db.ProductSales
                .AsNoTracking()
                .Include(item => item.Sale)
                    .ThenInclude(sale => sale!.User)
                .Include(item => item.Sale)
                    .ThenInclude(sale => sale!.Member)
                .Include(item => item.Sale)
                    .ThenInclude(sale => sale!.Payments)
                .Include(item => item.Product)
                .Include(item => item.Batch)
                .Where(item => item.Sale != null);

            if (dateFrom.HasValue)
            {
                var start = dateFrom.Value.Date;
                query = query.Where(item => (item.Sale!.CreatedAt ?? item.CreatedAt) >= start);
            }

            if (dateTo.HasValue)
            {
                var end = dateTo.Value.Date.AddDays(1);
                query = query.Where(item => (item.Sale!.CreatedAt ?? item.CreatedAt) < end);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(item => item.Status == status || item.Sale!.Status == status);
            }

            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(item => item.Sale!.ReceiptNo.Contains(searchText)
                    || (item.Product != null && item.Product.Name.Contains(searchText))
                    || (item.Batch != null && item.Batch.BatchNo.Contains(searchText))
                    || (item.Sale.User != null && ((item.Sale.User.FullName != null && item.Sale.User.FullName.Contains(searchText)) || item.Sale.User.Username.Contains(searchText)))
                    || (item.Sale.Member != null && item.Sale.Member.FullName.Contains(searchText)));
            }

            return query;
        }

        private IQueryable<FuelSale> BuildFuelSalesQuery(string? search, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            var query = _db.FuelSales
                .AsNoTracking()
                .Include(item => item.Sale)
                    .ThenInclude(sale => sale!.User)
                .Include(item => item.Sale)
                    .ThenInclude(sale => sale!.Member)
                .Include(item => item.Sale)
                    .ThenInclude(sale => sale!.Payments)
                .Include(item => item.Fuel)
                .Include(item => item.Tank)
                .Include(item => item.Nozzle)
                .Where(item => item.Sale != null);

            if (dateFrom.HasValue)
            {
                var start = dateFrom.Value.Date;
                query = query.Where(item => (item.Sale!.CreatedAt ?? item.CreatedAt) >= start);
            }

            if (dateTo.HasValue)
            {
                var end = dateTo.Value.Date.AddDays(1);
                query = query.Where(item => (item.Sale!.CreatedAt ?? item.CreatedAt) < end);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(item => item.Status == status || item.Sale!.Status == status);
            }

            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(item => item.Sale!.ReceiptNo.Contains(searchText)
                    || (item.Fuel != null && item.Fuel.Name.Contains(searchText))
                    || (item.Tank != null && item.Tank.TankNo.Contains(searchText))
                    || (item.Nozzle != null && item.Nozzle.NozzleNo.Contains(searchText))
                    || (item.Sale.User != null && ((item.Sale.User.FullName != null && item.Sale.User.FullName.Contains(searchText)) || item.Sale.User.Username.Contains(searchText)))
                    || (item.Sale.Member != null && item.Sale.Member.FullName.Contains(searchText)));
            }

            return query;
        }

        private static ProductSaleRowViewModel ToProductSaleRow(ProductSale item)
        {
            var sale = item.Sale;

            return new ProductSaleRowViewModel
            {
                SaleItemId = item.Id,
                ReceiptNo = sale?.ReceiptNo ?? "-",
                ProductName = item.Product?.Name ?? "-",
                BatchNo = item.Batch?.BatchNo ?? "-",
                Quantity = item.Quantity,
                Price = item.Price,
                Subtotal = item.Subtotal,
                CashierName = UserDisplayName(sale?.User),
                MemberName = sale?.Member?.FullName ?? "-",
                SaleDate = sale?.CreatedAt ?? item.CreatedAt,
                PaymentType = PaymentTypeFor(sale),
                GrossTotal = sale?.GrossTotal ?? 0m,
                RebateAmount = sale?.RebateAmount ?? 0m,
                NetTotal = sale?.NetTotal ?? 0m,
                Status = string.IsNullOrWhiteSpace(item.Status) ? sale?.Status ?? "-" : item.Status
            };
        }

        private static FuelSaleRowViewModel ToFuelSaleRow(FuelSale item)
        {
            var sale = item.Sale;

            return new FuelSaleRowViewModel
            {
                SaleItemId = item.Id,
                ReceiptNo = sale?.ReceiptNo ?? "-",
                FuelName = item.Fuel?.Name ?? "-",
                TankNo = item.Tank?.TankNo ?? "-",
                NozzleNo = item.Nozzle?.NozzleNo ?? "-",
                Liters = item.Liters,
                Price = item.PricePerLiter,
                Subtotal = item.Subtotal,
                CashierName = UserDisplayName(sale?.User),
                MemberName = sale?.Member?.FullName ?? "-",
                SaleDate = sale?.CreatedAt ?? item.CreatedAt,
                PaymentType = PaymentTypeFor(sale),
                GrossTotal = sale?.GrossTotal ?? 0m,
                RebateAmount = sale?.RebateAmount ?? 0m,
                NetTotal = sale?.NetTotal ?? 0m,
                Status = string.IsNullOrWhiteSpace(item.Status) ? sale?.Status ?? "-" : item.Status
            };
        }

        private static List<SelectListItem> BuildStatusOptions(string? selectedStatus)
        {
            var options = new List<SelectListItem> { new() { Value = "", Text = "All", Selected = string.IsNullOrWhiteSpace(selectedStatus) } };
            options.AddRange(SaleStatuses.Select(status => new SelectListItem { Value = status, Text = status, Selected = status == selectedStatus }));
            return options;
        }

        private static string? NormalizeStatus(string? status)
        {
            var value = (status ?? string.Empty).Trim();
            return SaleStatuses.Contains(value, StringComparer.OrdinalIgnoreCase)
                ? SaleStatuses.First(item => string.Equals(item, value, StringComparison.OrdinalIgnoreCase))
                : null;
        }

        private static string UserDisplayName(User? user)
        {
            if (user is null)
            {
                return "-";
            }

            return string.IsNullOrWhiteSpace(user.FullName) ? user.Username : user.FullName;
        }

        private static string PaymentTypeFor(Sale? sale)
        {
            return sale?.Payments.OrderBy(payment => payment.Id).Select(payment => payment.PaymentType).FirstOrDefault() ?? "-";
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

        private async Task<List<ValidatedProductSaleItem>> BuildValidatedProductItems(List<PosProductSaleRequestItem> products)
        {
            var items = new List<ValidatedProductSaleItem>();

            for (var i = 0; i < products.Count; i += 1)
            {
                var request = products[i];
                var displayStock = await _db.DisplayStocks
                    .AsNoTracking()
                    .Include(item => item.Product)
                    .Include(item => item.Batch)
                    .FirstOrDefaultAsync(item => item.Id == request.DisplayStockId && item.ProductId == request.ProductId && item.BatchId == request.BatchId);

                if (displayStock is null || displayStock.Product is null || displayStock.Batch is null)
                {
                    throw new InvalidOperationException("One or more selected products are no longer available in display inventory.");
                }

                if (displayStock.Product.Status != 1 || !displayStock.Product.IsActive || displayStock.Batch.Status != 1 || !displayStock.Batch.IsActive || displayStock.Batch.ProductId != displayStock.ProductId)
                {
                    throw new InvalidOperationException("One or more selected products are no longer active.");
                }

                if (displayStock.Quantity <= 0 || displayStock.Quantity < request.Quantity)
                {
                    throw new InvalidOperationException($"Display stock is not enough for {displayStock.Product.Name}.");
                }

                var price = displayStock.Batch.SellingPrice;
                var subtotal = request.Quantity * price;

                items.Add(new ValidatedProductSaleItem
                {
                    DisplayStockId = displayStock.Id,
                    ProductName = displayStock.Product.Name,
                    Quantity = request.Quantity,
                    Subtotal = subtotal,
                    ProductSale = new ProductSale
                    {
                        ProductId = displayStock.ProductId,
                        BatchId = displayStock.BatchId,
                        Quantity = request.Quantity,
                        Price = price,
                        Subtotal = subtotal,
                        Status = "Completed",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    StockMovement = new StockMovement
                    {
                        ProductId = displayStock.ProductId,
                        ProductBatchId = displayStock.BatchId,
                        SourceLocation = "Display",
                        MovementType = "Sale",
                        Quantity = request.Quantity,
                        ReferenceType = "POS",
                        Remarks = $"POS sale for {displayStock.Product.Name}",
                        CreatedAt = DateTime.Now
                    }
                });
            }

            return items;
        }

        private async Task<List<ValidatedFuelSaleItem>> BuildValidatedFuelItems(List<PosFuelSaleRequestItem> fuels)
        {
            var items = new List<ValidatedFuelSaleItem>();

            for (var i = 0; i < fuels.Count; i += 1)
            {
                var request = fuels[i];
                var tank = await _db.Tanks
                    .Include(item => item.Fuel)
                    .FirstOrDefaultAsync(item => item.Id == request.TankId && item.FuelId == request.FuelId && item.Status == 1 && item.IsActive);

                if (tank is null || tank.Fuel is null || tank.Fuel.Status != 1 || !tank.Fuel.IsActive)
                {
                    throw new InvalidOperationException("One or more selected fuel items are no longer available.");
                }

                if (request.NozzleId.HasValue && request.NozzleId.Value > 0)
                {
                    var nozzleExists = await _db.Nozzles
                        .Include(item => item.Pump)
                        .AnyAsync(item => item.Id == request.NozzleId.Value && item.Status == 1 && item.Pump != null && item.Pump.TankId == tank.Id);

                    if (!nozzleExists)
                    {
                        throw new InvalidOperationException($"No active nozzle is available for {tank.Fuel.Name}.");
                    }
                }

                var price = tank.Fuel.CurrentPricePerLiter;
                var subtotal = request.Liters * price;

                items.Add(new ValidatedFuelSaleItem
                {
                    FuelName = tank.Fuel.Name,
                    Tank = tank,
                    Liters = request.Liters,
                    Subtotal = subtotal,
                    FuelSale = new FuelSale
                    {
                        FuelId = tank.FuelId,
                        TankId = tank.Id,
                        NozzleId = request.NozzleId.HasValue && request.NozzleId.Value > 0 ? request.NozzleId : null,
                        Liters = request.Liters,
                        PricePerLiter = price,
                        Subtotal = subtotal,
                        Status = "Completed",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                });
            }

            return items;
        }

        private async Task DeductDisplayStock(List<ValidatedProductSaleItem> items, int userId)
        {
            for (var i = 0; i < items.Count; i += 1)
            {
                var saleItem = items[i].ProductSale;
                var displayStock = await _db.DisplayStocks
                    .FirstOrDefaultAsync(stock => stock.Id == items[i].DisplayStockId && stock.ProductId == saleItem.ProductId && stock.BatchId == saleItem.BatchId);

                if (displayStock is null || displayStock.Quantity < items[i].Quantity)
                {
                    throw new InvalidOperationException($"Display stock is not enough for {items[i].ProductName}.");
                }

                saleItem.DisplayStockBefore = displayStock.Quantity;
                displayStock.Quantity -= items[i].Quantity;
                saleItem.DisplayStockAfter = displayStock.Quantity;
                displayStock.UpdatedAt = DateTime.Now;
                items[i].StockMovement.CreatedBy = userId;
            }
        }

        private static Task DeductTankFuel(List<ValidatedFuelSaleItem> items)
        {
            for (var i = 0; i < items.Count; i += 1)
            {
                if (items[i].Tank.CurrentLiters < items[i].Liters)
                {
                    throw new InvalidOperationException($"Tank liters are not enough for {items[i].FuelName}.");
                }

                items[i].FuelSale.TankLitersBefore = items[i].Tank.CurrentLiters;
                items[i].Tank.CurrentLiters -= items[i].Liters;
                items[i].FuelSale.TankLitersAfter = items[i].Tank.CurrentLiters;
                items[i].Tank.UpdatedAt = DateTime.Now;
            }

            return Task.CompletedTask;
        }

        private async Task<Member?> FindMember(string? membershipCardNo)
        {
            if (string.IsNullOrWhiteSpace(membershipCardNo))
            {
                return null;
            }

            var member = await FindActiveMemberByCard(membershipCardNo);
            return member ?? throw new InvalidOperationException("Member card was not found or is inactive.");
        }

        private async Task<Member?> FindActiveMemberByCard(string? membershipCardNo)
        {
            var cardNo = (membershipCardNo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(cardNo))
            {
                return null;
            }

            return await _db.Members
                .Include(item => item.EarningRule)
                .FirstOrDefaultAsync(item => item.Status == 1 && (item.CardNo == cardNo || item.MemberNo == cardNo));
        }

        private async Task<RebateRule?> FindRebate(int? rebateRuleId)
        {
            if (!rebateRuleId.HasValue || rebateRuleId.Value <= 0)
            {
                return null;
            }

            var rebate = await _db.RebateRules.FirstOrDefaultAsync(item => item.Id == rebateRuleId.Value && item.Status == 1);
            return rebate ?? throw new InvalidOperationException("Selected rebate was not found or is inactive.");
        }

        private static decimal CalculateEarnedPoints(Member? member, decimal netTotal)
        {
            var earnRate = member?.EarningRule?.IsActive == true ? member.EarningRule.EarnRate : 0m;

            if (member is null || netTotal <= 0m || earnRate <= 0m)
            {
                return 0m;
            }

            return Math.Round(netTotal * (earnRate / 100m), 2, MidpointRounding.AwayFromZero);
        }

        private async Task ApplyPointsChanges(Member? member, RebateRule? rebate, decimal rebateAmount, decimal pointsEarned, int saleId, DateTime now)
        {
            if (member is null)
            {
                return;
            }

            if (rebate is not null && rebateAmount > 0m)
            {
                var oldPoints = member.Points;
                member.Points -= rebate.PointsRequired;
                member.UpdatedAt = now;

                _db.PointsLedger.Add(new PointsLedger
                {
                    MemberId = member.Id,
                    TransactionType = "Used",
                    Points = rebate.PointsRequired,
                    OldPoints = oldPoints,
                    NewPoints = member.Points,
                    ReferenceType = "POS",
                    ReferenceId = saleId,
                    SaleId = saleId,
                    Remarks = "Used as rebate in POS",
                    CreatedAt = now
                });
            }

            if (pointsEarned > 0m)
            {
                var alreadyAwarded = await _db.PointsLedger.AnyAsync(ledger =>
                    ledger.MemberId == member.Id
                    && ledger.SaleId == saleId
                    && ledger.TransactionType == "Earned");

                if (alreadyAwarded)
                {
                    return;
                }

                var oldPoints = member.Points;
                member.Points += pointsEarned;
                member.UpdatedAt = now;

                _db.PointsLedger.Add(new PointsLedger
                {
                    MemberId = member.Id,
                    TransactionType = "Earned",
                    Points = pointsEarned,
                    OldPoints = oldPoints,
                    NewPoints = member.Points,
                    ReferenceType = "POS",
                    ReferenceId = saleId,
                    SaleId = saleId,
                    Remarks = "Earned from POS sale",
                    CreatedAt = now
                });
            }
        }

        private async Task<decimal> CalculateMemberDiscount(Member? member, decimal productTotal, decimal fuelTotal, DateTime now)
        {
            if (member is null || !member.DiscountId.HasValue)
            {
                return 0m;
            }

            var rules = await _db.DiscountRules
                .Where(item => item.DiscountId == member.DiscountId.Value && item.Status == 1)
                .Where(item => !item.StartDate.HasValue || now.Date >= item.StartDate.Value.Date)
                .Where(item => !item.EndDate.HasValue || now.Date <= item.EndDate.Value.Date)
                .OrderBy(item => item.Id)
                .ToListAsync();

            return rules
                .Select(rule => ValidateAndCalculateDiscount(member, rule, productTotal, fuelTotal, now))
                .DefaultIfEmpty(0m)
                .Max();
        }

        private static decimal ValidateAndCalculateDiscount(Member? member, DiscountRule? discountRule, decimal productTotal, decimal fuelTotal, DateTime now)
        {
            if (discountRule is null)
            {
                return 0m;
            }

            if (discountRule.MemberRequired == 1)
            {
                if (member is null)
                {
                    throw new InvalidOperationException("Select a valid member before using this discount.");
                }

                if (member.DiscountId != discountRule.DiscountId)
                {
                    throw new InvalidOperationException("Selected member is not eligible for this discount.");
                }
            }

            if (discountRule.StartDate.HasValue && now.Date < discountRule.StartDate.Value.Date)
            {
                throw new InvalidOperationException("Selected discount is not active yet.");
            }

            if (discountRule.EndDate.HasValue && now.Date > discountRule.EndDate.Value.Date)
            {
                throw new InvalidOperationException("Selected discount has expired.");
            }

            var appliesTo = discountRule.AppliesTo.Trim();
            var discountBase = productTotal + fuelTotal;

            if (AppliesToProductOnly(appliesTo))
            {
                discountBase = productTotal;
            }
            else if (AppliesToFuelOnly(appliesTo))
            {
                discountBase = fuelTotal;
            }

            if (discountBase <= 0)
            {
                return 0m;
            }

            if (discountBase < discountRule.MinimumAmount)
            {
                return 0m;
            }

            var discountType = discountRule.DiscountType.Trim();
            var discountValue = Math.Max(0m, discountRule.DiscountValue);
            var discountAmount = IsPercentageDiscount(discountType)
                ? discountBase * (discountValue / 100m)
                : discountValue;

            return Math.Min(discountBase, discountAmount);
        }

        private static bool AppliesToProductOnly(string appliesTo)
        {
            var value = appliesTo.Trim().ToLowerInvariant();
            return value is "product" or "products";
        }

        private static bool AppliesToFuelOnly(string appliesTo)
        {
            var value = appliesTo.Trim().ToLowerInvariant();
            return value is "fuel" or "fuels" or "gas" or "gasoline";
        }

        private static bool IsPercentageDiscount(string discountType)
        {
            var value = discountType.Trim().ToLowerInvariant();
            return value is "percentage" or "percent" or "%";
        }

        private static decimal ValidateAndCalculateRebate(Member? member, RebateRule? rebate, decimal grossTotal, bool hasProducts, bool hasFuel)
        {
            if (rebate is null)
            {
                return 0m;
            }

            if (member is null)
            {
                throw new InvalidOperationException("Select a valid member before using a rebate.");
            }

            if (member.Points < rebate.PointsRequired)
            {
                throw new InvalidOperationException("Member does not have enough points for the selected rebate.");
            }

            var appliesTo = rebate.AppliesTo.Trim();
            var appliesToProduct = string.Equals(appliesTo, "Product", StringComparison.OrdinalIgnoreCase) || string.Equals(appliesTo, "Both", StringComparison.OrdinalIgnoreCase);
            var appliesToFuel = string.Equals(appliesTo, "Fuel", StringComparison.OrdinalIgnoreCase) || string.Equals(appliesTo, "Both", StringComparison.OrdinalIgnoreCase);

            if ((hasProducts && !appliesToProduct) || (hasFuel && !appliesToFuel))
            {
                throw new InvalidOperationException("Selected rebate does not apply to every item in this sale.");
            }

            if (grossTotal < rebate.MinimumPurchase)
            {
                throw new InvalidOperationException("Sale total does not meet the rebate minimum purchase.");
            }

            return Math.Min(grossTotal, rebate.RebateValue);
        }

        private async Task<string> GenerateReceiptNo(DateTime now)
        {
            var prefix = $"POS-{now:yyyyMMdd}-";
            var start = now.Date;
            var end = start.AddDays(1);
            var count = await _db.Sales.CountAsync(sale => sale.CreatedAt >= start && sale.CreatedAt < end);
            return $"{prefix}{count + 1:000000}";
        }

        private static string PaymentType(decimal cashAmount, decimal rebateAmount, decimal netTotal)
        {
            if (rebateAmount > 0 && cashAmount <= 0 && netTotal <= 0)
            {
                return "Points";
            }

            if (rebateAmount > 0)
            {
                return "Mixed";
            }

            return "Cash";
        }

        private sealed class ValidatedProductSaleItem
        {
            public int DisplayStockId { get; init; }
            public string ProductName { get; init; } = string.Empty;
            public decimal Quantity { get; init; }
            public decimal Subtotal { get; init; }
            public ProductSale ProductSale { get; init; } = new();
            public StockMovement StockMovement { get; init; } = new();
            public int SaleId
            {
                set => ProductSale.SaleId = value;
            }
        }

        private sealed class ValidatedFuelSaleItem
        {
            public string FuelName { get; init; } = string.Empty;
            public Tank Tank { get; init; } = new();
            public decimal Liters { get; init; }
            public decimal Subtotal { get; init; }
            public FuelSale FuelSale { get; init; } = new();
        }
    }
}

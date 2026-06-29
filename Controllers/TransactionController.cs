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

            return Ok(new { success = true, memberFound = true, discountAmount, points = member.Points });
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

                var paymentMethod = NormalizePaymentMethod(request.PaymentMethod);
                var isPointsPayment = paymentMethod == "Points";
                var voucherCode = NormalizeVoucherCode(request.VoucherCode);
                VoucherRedemptionResult? voucherRedemption = null;
                Member? member;
                decimal discountAmount;
                decimal memberDiscountAmount;
                decimal voucherDiscountAmount;
                decimal pointsRequired;
                decimal rebateAmount;

                if (isPointsPayment)
                {
                    if (!string.IsNullOrWhiteSpace(voucherCode))
                    {
                        throw new InvalidOperationException("Voucher redemption is not available with points payment.");
                    }

                    var pointsMember = await FindRequiredMember(request.MembershipCardNo);
                    var rebate = await FindLatestActiveRebate();
                    member = pointsMember;
                    memberDiscountAmount = 0m;
                    voucherDiscountAmount = 0m;
                    discountAmount = 0m;
                    pointsRequired = ValidateAndCalculatePointsPayment(pointsMember, rebate, grossTotal, productItems.Count > 0, fuelItems.Count > 0);
                    rebateAmount = grossTotal;
                }
                else
                {
                    member = await FindMember(request.MembershipCardNo);
                    memberDiscountAmount = await CalculateMemberDiscount(member, productTotal, fuelTotal, now);
                    voucherDiscountAmount = 0m;
                    if (!string.IsNullOrWhiteSpace(voucherCode))
                    {
                        voucherRedemption = await ValidateAndCalculateVoucherRedemption(voucherCode, member, productItems, fuelItems, productTotal, fuelTotal, now);
                        voucherDiscountAmount = voucherRedemption.DiscountAmount;
                    }

                    discountAmount = memberDiscountAmount + voucherDiscountAmount;
                    pointsRequired = 0m;
                    rebateAmount = 0m;
                }

                var netTotal = Math.Max(0m, grossTotal - discountAmount - rebateAmount);
                var cashAmount = isPointsPayment ? 0m : Math.Max(0m, request.CashAmount);
                var pointsEarned = isPointsPayment ? 0m : await CalculateMemberEarnings(member, productTotal, fuelTotal, netTotal, now);

                if (!isPointsPayment && cashAmount < netTotal)
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

                AddPaymentRecords(sale.Id, paymentMethod, cashAmount, rebateAmount, now);

                await ApplyPointsChanges(member, pointsRequired, pointsEarned, sale.Id, now);
                ApplyVoucherRedemption(voucherRedemption, sale.Id, now);

                await _db.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return Ok(new
                {
                    success = true,
                    message = "Sale saved successfully.",
                    receiptNo = sale.ReceiptNo,
                    grossTotal,
                    discountAmount,
                    memberDiscountAmount,
                    voucherDiscountAmount,
                    rebateAmount,
                    netTotal,
                    pointsEarned,
                    pointsRequired,
                    paymentMethod,
                    voucherCode = voucherRedemption?.Voucher.Code,
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
            var itemSummaries = await BuildItemSummaryLookup(items.Select(item => item.SaleId));

            return View(new ProductSalesPageViewModel
            {
                Search = (search ?? string.Empty).Trim(),
                DateFrom = dateFrom,
                DateTo = dateTo,
                Status = normalizedStatus ?? string.Empty,
                StatusOptions = BuildStatusOptions(normalizedStatus),
                Sales = items
                    .DistinctBy(item => item.SaleId)
                    .Select(item =>
                    {
                        var row = ToProductSaleRow(item);
                        ApplyItemSummary(row, itemSummaries);
                        return row;
                    })
                    .ToList()
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
            var itemSummaries = await BuildItemSummaryLookup(items.Select(item => item.SaleId));

            return View(new FuelSalesPageViewModel
            {
                Search = (search ?? string.Empty).Trim(),
                DateFrom = dateFrom,
                DateTo = dateTo,
                Status = normalizedStatus ?? string.Empty,
                StatusOptions = BuildStatusOptions(normalizedStatus),
                Sales = items
                    .DistinctBy(item => item.SaleId)
                    .Select(item =>
                    {
                        var row = ToFuelSaleRow(item);
                        ApplyItemSummary(row, itemSummaries);
                        return row;
                    })
                    .ToList()
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
                .Include(item => item.Sale)
                    .ThenInclude(sale => sale!.PointsLedger)
                .Include(item => item.Sale)
                    .ThenInclude(sale => sale!.VoucherRedemptions)
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
                .Include(item => item.Sale)
                    .ThenInclude(sale => sale!.PointsLedger)
                .Include(item => item.Sale)
                    .ThenInclude(sale => sale!.VoucherRedemptions)
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
            var voucherDiscount = VoucherDiscountFor(sale);
            var totalDiscount = sale?.DiscountAmount ?? 0m;
            var memberDiscount = Math.Max(0m, totalDiscount - voucherDiscount);

            return new ProductSaleRowViewModel
            {
                SaleItemId = item.Id,
                SaleId = item.SaleId,
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
                Cost = item.Batch is null ? 0m : item.Batch.CostPrice * item.Quantity,
                GrossTotal = sale?.GrossTotal ?? 0m,
                RebateAmount = sale?.RebateAmount ?? 0m,
                MemberDiscount = memberDiscount,
                VoucherDiscount = voucherDiscount,
                TotalDiscount = totalDiscount,
                NetTotal = sale?.NetTotal ?? 0m,
                NetSales = sale?.NetTotal ?? 0m,
                Loss = totalDiscount,
                CashAmount = sale?.CashAmount ?? 0m,
                Change = ChangeFor(sale),
                PointsPaid = PointsPaidFor(sale),
                Status = string.IsNullOrWhiteSpace(sale?.Status) ? item.Status : sale!.Status
            };
        }

        private static FuelSaleRowViewModel ToFuelSaleRow(FuelSale item)
        {
            var sale = item.Sale;
            var voucherDiscount = VoucherDiscountFor(sale);
            var totalDiscount = sale?.DiscountAmount ?? 0m;
            var memberDiscount = Math.Max(0m, totalDiscount - voucherDiscount);

            return new FuelSaleRowViewModel
            {
                SaleItemId = item.Id,
                SaleId = item.SaleId,
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
                Cost = sale?.GrossTotal ?? item.Subtotal,
                GrossTotal = sale?.GrossTotal ?? 0m,
                RebateAmount = sale?.RebateAmount ?? 0m,
                MemberDiscount = memberDiscount,
                VoucherDiscount = voucherDiscount,
                TotalDiscount = totalDiscount,
                NetTotal = sale?.NetTotal ?? 0m,
                NetSales = sale?.NetTotal ?? 0m,
                Loss = totalDiscount,
                CashAmount = sale?.CashAmount ?? 0m,
                Change = ChangeFor(sale),
                PointsPaid = PointsPaidFor(sale),
                Status = string.IsNullOrWhiteSpace(sale?.Status) ? item.Status : sale!.Status
            };
        }

        private static decimal VoucherDiscountFor(Sale? sale)
        {
            return sale?.VoucherRedemptions.Sum(redemption => redemption.DiscountAmount) ?? 0m;
        }

        private static decimal PointsPaidFor(Sale? sale)
        {
            return sale?.PointsLedger
                .Where(ledger => string.Equals(ledger.TransactionType, "Used", StringComparison.OrdinalIgnoreCase))
                .Sum(ledger => ledger.Points) ?? 0m;
        }

        private async Task<Dictionary<int, ItemSummary>> BuildItemSummaryLookup(IEnumerable<int> saleIds)
        {
            var ids = saleIds.Distinct().ToList();
            if (ids.Count == 0)
            {
                return new Dictionary<int, ItemSummary>();
            }

            var productItems = await _db.ProductSales
                .AsNoTracking()
                .Where(item => ids.Contains(item.SaleId))
                .OrderBy(item => item.Id)
                .Select(item => new
                {
                    item.SaleId,
                    item.Id,
                    Name = item.Product != null ? item.Product.Name : null,
                    item.Quantity,
                    item.Subtotal,
                    Cost = item.Batch == null ? 0m : item.Batch.CostPrice * item.Quantity
                })
                .ToListAsync();

            var fuelItems = await _db.FuelSales
                .AsNoTracking()
                .Where(item => ids.Contains(item.SaleId))
                .OrderBy(item => item.Id)
                .Select(item => new
                {
                    item.SaleId,
                    item.Id,
                    Name = item.Fuel != null ? item.Fuel.Name : null,
                    item.Liters,
                    item.Subtotal
                })
                .ToListAsync();

            return productItems
                .Select(item => new ItemSummaryLine(item.SaleId, item.Id, $"{(string.IsNullOrWhiteSpace(item.Name) ? "Product" : item.Name)} ({FormatQuantity(item.Quantity)})", item.Subtotal, item.Cost))
                .Concat(fuelItems.Select(item => new ItemSummaryLine(item.SaleId, item.Id, $"{(string.IsNullOrWhiteSpace(item.Name) ? "Fuel" : item.Name)} ({item.Liters:N2} L)", item.Subtotal, 0m)))
                .GroupBy(item => item.SaleId)
                .ToDictionary(group => group.Key, group => BuildItemSummary(group.OrderBy(item => item.Id).ToList()));
        }

        private static ItemSummary BuildItemSummary(List<ItemSummaryLine> lines)
        {
            var items = lines.Select(item => item.Text).ToList();
            if (items.Count == 0)
            {
                return new ItemSummary("-", "-", 0m, 0m);
            }

            const int visibleItems = 1;
            var summary = items.Take(visibleItems).ToList();
            if (items.Count > visibleItems)
            {
                summary.Add($"+{items.Count - visibleItems} more...");
            }

            return new ItemSummary(string.Join(" ", summary), string.Join(Environment.NewLine, items), lines.Sum(item => item.Subtotal), lines.Sum(item => item.Cost));
        }

        private static void ApplyItemSummary(ProductSaleRowViewModel row, IReadOnlyDictionary<int, ItemSummary> itemSummaries)
        {
            if (!itemSummaries.TryGetValue(row.SaleId, out var itemSummary))
            {
                return;
            }

            row.ItemSummary = itemSummary.Summary;
            row.ItemSummaryTitle = itemSummary.Title;
            row.Subtotal = itemSummary.Subtotal;
            row.Cost = itemSummary.Cost;
        }

        private static void ApplyItemSummary(FuelSaleRowViewModel row, IReadOnlyDictionary<int, ItemSummary> itemSummaries)
        {
            if (!itemSummaries.TryGetValue(row.SaleId, out var itemSummary))
            {
                return;
            }

            row.ItemSummary = itemSummary.Summary;
            row.ItemSummaryTitle = itemSummary.Title;
            row.Subtotal = itemSummary.Subtotal;
            row.Cost = itemSummary.Subtotal;
        }

        private static string FormatQuantity(decimal value)
        {
            return value == decimal.Truncate(value)
                ? value.ToString("N0")
                : value.ToString("N2");
        }

        private sealed record ItemSummary(string Summary, string Title, decimal Subtotal, decimal Cost);

        private sealed record ItemSummaryLine(int SaleId, int Id, string Text, decimal Subtotal, decimal Cost);

        private static decimal ChangeFor(Sale? sale)
        {
            if (!string.Equals(PaymentTypeFor(sale), "Cash", StringComparison.OrdinalIgnoreCase))
            {
                return 0m;
            }

            return Math.Max(0m, (sale?.CashAmount ?? 0m) - (sale?.NetTotal ?? 0m));
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
            if (sale is null || sale.Payments.Count == 0)
            {
                return "-";
            }

            return string.Join(" + ", sale.Payments
                .OrderBy(payment => payment.Id)
                .Select(payment => payment.PaymentType)
                .Where(paymentType => !string.IsNullOrWhiteSpace(paymentType))
                .Distinct());
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
                    ProductId = displayStock.ProductId,
                    CategoryId = displayStock.Product.CategoryId,
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

        private async Task<Member> FindRequiredMember(string? membershipCardNo)
        {
            var member = await FindMember(membershipCardNo);
            return member ?? throw new InvalidOperationException("Select a valid member before using points payment.");
        }

        private async Task<Member?> FindActiveMemberByCard(string? membershipCardNo)
        {
            var cardNo = (membershipCardNo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(cardNo))
            {
                return null;
            }

            return await _db.Members
                .FirstOrDefaultAsync(item => item.Status == 1 && (item.CardNo == cardNo || item.MemberNo == cardNo));
        }

        private async Task<RebateRule> FindLatestActiveRebate()
        {
            var rebate = await _db.RebateRules
                .Where(item => item.Status == 1)
                .OrderByDescending(item => item.CreatedAt)
                .ThenByDescending(item => item.Id)
                .FirstOrDefaultAsync();

            return rebate ?? throw new InvalidOperationException("No active rebate configuration is available for points payment.");
        }

        private async Task ApplyPointsChanges(Member? member, decimal pointsRedeemed, decimal pointsEarned, int saleId, DateTime now)
        {
            if (member is null)
            {
                return;
            }

            if (pointsRedeemed > 0m)
            {
                var oldPoints = member.Points;
                member.Points -= pointsRedeemed;
                member.UpdatedAt = now;

                _db.PointsLedger.Add(new PointsLedger
                {
                    MemberId = member.Id,
                    TransactionType = "Used",
                    Points = pointsRedeemed,
                    OldPoints = oldPoints,
                    NewPoints = member.Points,
                    ReferenceType = "POS",
                    ReferenceId = saleId,
                    SaleId = saleId,
                    Remarks = "Used as points payment in POS",
                    CreatedAt = now
                });
            }

            if (pointsEarned > 0m)
            {
                var alreadyAwarded = await _db.PointsLedger.AnyAsync(ledger =>
                    ledger.MemberId == member.Id
                    && (ledger.SaleId == saleId || ledger.ReferenceId == saleId)
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

        private async Task<decimal> CalculateMemberEarnings(Member? member, decimal productTotal, decimal fuelTotal, decimal netTotal, DateTime now)
        {
            if (member is null || !member.EarningsId.HasValue || netTotal <= 0m)
            {
                return 0m;
            }

            var rules = await _db.EarningRules
                .Where(item => item.EarningsId == member.EarningsId.Value && item.Status == 1)
                .Where(item => item.Earnings != null && item.Earnings.Status == 1)
                .Where(item => !item.StartDate.HasValue || now.Date >= item.StartDate.Value.Date)
                .Where(item => !item.EndDate.HasValue || now.Date <= item.EndDate.Value.Date)
                .OrderBy(item => item.Id)
                .ToListAsync();

            return rules
                .Select(rule => ValidateAndCalculateEarning(member, rule, productTotal, fuelTotal, netTotal, now))
                .DefaultIfEmpty(0m)
                .Max();
        }

        private static decimal ValidateAndCalculateEarning(Member? member, EarningRule? earningRule, decimal productTotal, decimal fuelTotal, decimal netTotal, DateTime now)
        {
            if (earningRule is null)
            {
                return 0m;
            }

            if (earningRule.MemberRequired == 1)
            {
                if (member is null)
                {
                    throw new InvalidOperationException("Select a valid member before using this earning rule.");
                }

                if (member.EarningsId != earningRule.EarningsId)
                {
                    throw new InvalidOperationException("Selected member is not eligible for this earning rule.");
                }
            }

            if (earningRule.StartDate.HasValue && now.Date < earningRule.StartDate.Value.Date)
            {
                throw new InvalidOperationException("Selected earning rule is not active yet.");
            }

            if (earningRule.EndDate.HasValue && now.Date > earningRule.EndDate.Value.Date)
            {
                throw new InvalidOperationException("Selected earning rule has expired.");
            }

            var appliesTo = earningRule.AppliesTo.Trim();
            var grossTotal = productTotal + fuelTotal;
            var eligibleGross = grossTotal;

            if (AppliesToProductOnly(appliesTo))
            {
                eligibleGross = productTotal;
            }
            else if (AppliesToFuelOnly(appliesTo))
            {
                eligibleGross = fuelTotal;
            }

            if (grossTotal <= 0m || eligibleGross <= 0m || netTotal <= 0m)
            {
                return 0m;
            }

            var earningBase = netTotal * (eligibleGross / grossTotal);

            if (earningBase < earningRule.MinimumAmount)
            {
                return 0m;
            }

            var earnType = earningRule.EarnType.Trim();
            var earnValue = Math.Max(0m, earningRule.EarnValue);
            var earnedPoints = IsPercentageEarning(earnType)
                ? earningBase * (earnValue / 100m)
                : earnValue;

            return Math.Round(Math.Max(0m, earnedPoints), 2, MidpointRounding.AwayFromZero);
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

        private async Task<VoucherRedemptionResult> ValidateAndCalculateVoucherRedemption(
            string voucherCode,
            Member? member,
            List<ValidatedProductSaleItem> productItems,
            List<ValidatedFuelSaleItem> fuelItems,
            decimal productTotal,
            decimal fuelTotal,
            DateTime now)
        {
            var voucher = await _db.Vouchers
                .Include(item => item.Redemptions)
                .FirstOrDefaultAsync(item => item.Code == voucherCode);

            if (voucher is null)
            {
                throw new InvalidOperationException("Voucher code was not found.");
            }

            if (!string.Equals(voucher.Status, "Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Voucher is not active.");
            }

            if (voucher.MemberId.HasValue && (member is null || voucher.MemberId.Value != member.Id))
            {
                throw new InvalidOperationException("Voucher does not belong to the selected member.");
            }

            var rule = await _db.VoucherRules
                .Include(rule => rule.Redemptions)
                .Where(rule => rule.VoucherId == voucher.Id && rule.Status == 1)
                .OrderByDescending(rule => rule.Priority)
                .ThenBy(rule => rule.Id)
                .FirstOrDefaultAsync();

            if (rule is null)
            {
                throw new InvalidOperationException("No active voucher rule is configured for this voucher.");
            }

            var discountAmount = ValidateAndCalculateVoucherDiscount(rule, voucher, member, productItems, fuelItems, productTotal, fuelTotal, now);
            return new VoucherRedemptionResult(voucher, rule, discountAmount);
        }

        private static decimal ValidateAndCalculateVoucherDiscount(
            VoucherRule rule,
            Voucher voucher,
            Member? member,
            List<ValidatedProductSaleItem> productItems,
            List<ValidatedFuelSaleItem> fuelItems,
            decimal productTotal,
            decimal fuelTotal,
            DateTime now)
        {
            if (rule.EffectiveDate.HasValue && now.Date < rule.EffectiveDate.Value.Date)
            {
                throw new InvalidOperationException("Voucher is not active yet.");
            }

            if (!rule.NoExpiration && rule.ExpirationDate.HasValue && now.Date > rule.ExpirationDate.Value.Date)
            {
                throw new InvalidOperationException("Voucher has expired.");
            }

            if (rule.MaxRedemptions.HasValue && rule.Redemptions.Count >= rule.MaxRedemptions.Value)
            {
                throw new InvalidOperationException("Voucher redemption limit has been reached.");
            }

            var voucherUseCount = voucher.Redemptions.Count;
            var usageLimitType = (rule.UsageLimitType ?? string.Empty).Trim();
            if (usageLimitType.Equals("Once Per Voucher", StringComparison.OrdinalIgnoreCase) && voucherUseCount >= 1)
            {
                throw new InvalidOperationException("Voucher has already been redeemed.");
            }

            if (usageLimitType.Equals("Limited Uses", StringComparison.OrdinalIgnoreCase)
                && rule.LimitedUseCount.HasValue
                && voucherUseCount >= rule.LimitedUseCount.Value)
            {
                throw new InvalidOperationException("Voucher use limit has been reached.");
            }

            var eligibleBase = EligibleVoucherBase(rule, productItems, fuelItems, productTotal, fuelTotal);
            if (eligibleBase <= 0m)
            {
                throw new InvalidOperationException("Voucher does not apply to the selected items.");
            }

            if (eligibleBase < rule.MinimumPurchaseAmount)
            {
                throw new InvalidOperationException("Sale total does not meet the voucher minimum amount.");
            }

            if (rule.MemberRequired == 1 && member is null)
            {
                throw new InvalidOperationException("Select a valid member before redeeming this voucher.");
            }

            var rewardValue = Math.Max(0m, rule.RewardValue);
            var discountAmount = IsPercentageDiscount(rule.RewardType)
                ? eligibleBase * (rewardValue / 100m)
                : rewardValue;

            if (IsPercentageDiscount(rule.RewardType) && rule.MaxDiscountAmount.HasValue)
            {
                discountAmount = Math.Min(discountAmount, Math.Max(0m, rule.MaxDiscountAmount.Value));
            }

            return Math.Round(Math.Min(eligibleBase, Math.Max(0m, discountAmount)), 2, MidpointRounding.AwayFromZero);
        }

        private static decimal EligibleVoucherBase(
            VoucherRule rule,
            List<ValidatedProductSaleItem> productItems,
            List<ValidatedFuelSaleItem> fuelItems,
            decimal productTotal,
            decimal fuelTotal)
        {
            var appliesTo = (rule.AppliesTo ?? string.Empty).Trim();
            var productBase = AppliesToFuelOnly(appliesTo) ? 0m : productTotal;
            var fuelBase = AppliesToProductOnly(appliesTo) ? 0m : fuelTotal;
            var productIds = ParseIdSet(rule.ApplicableProductIds);
            var categoryIds = ParseIdSet(rule.ApplicableCategoryIds);

            if (productBase > 0m && (productIds.Count > 0 || categoryIds.Count > 0))
            {
                productBase = productItems
                    .Where(item => productIds.Count == 0 || productIds.Contains(item.ProductId))
                    .Where(item => categoryIds.Count == 0 || item.CategoryId.HasValue && categoryIds.Contains(item.CategoryId.Value))
                    .Sum(item => item.Subtotal);
            }

            return productBase + fuelBase;
        }

        private static HashSet<int> ParseIdSet(string? ids)
        {
            return (ids ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(value => int.TryParse(value, out var id) ? id : 0)
                .Where(id => id > 0)
                .ToHashSet();
        }

        private void ApplyVoucherRedemption(VoucherRedemptionResult? redemption, int saleId, DateTime now)
        {
            if (redemption is null)
            {
                return;
            }

            _db.VoucherRedemptions.Add(new VoucherRedemption
            {
                VoucherId = redemption.Voucher.Id,
                VoucherRuleId = redemption.Rule.Id,
                SaleId = saleId,
                DiscountAmount = redemption.DiscountAmount,
                CreatedAt = now
            });

            redemption.Voucher.Status = VoucherStatusAfterRedemption(redemption.Voucher, redemption.Rule);
            redemption.Voucher.UpdatedAt = now;
        }

        private static string VoucherStatusAfterRedemption(Voucher voucher, VoucherRule rule)
        {
            var usageLimitType = (rule.UsageLimitType ?? string.Empty).Trim();
            var useCountAfterRedemption = voucher.Redemptions.Count + 1;

            if (usageLimitType.Equals("Unlimited", StringComparison.OrdinalIgnoreCase))
            {
                return "Active";
            }

            if (usageLimitType.Equals("Limited Uses", StringComparison.OrdinalIgnoreCase))
            {
                return rule.LimitedUseCount.HasValue && useCountAfterRedemption >= rule.LimitedUseCount.Value
                    ? "Redeemed"
                    : "Active";
            }

            return "Redeemed";
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

        private static bool IsPercentageEarning(string earnType)
        {
            var value = earnType.Trim().ToLowerInvariant();
            return value is "percentage" or "percent" or "%";
        }

        private static string NormalizeVoucherCode(string? voucherCode)
        {
            return (voucherCode ?? string.Empty).Trim().ToUpperInvariant();
        }

        private static decimal ValidateAndCalculatePointsPayment(Member member, RebateRule rebate, decimal grossTotal, bool hasProducts, bool hasFuel)
        {
            ValidateRebateEligibility(rebate, grossTotal, hasProducts, hasFuel);

            if (rebate.PointsRequired <= 0m || rebate.RebateValue <= 0m)
            {
                throw new InvalidOperationException("Active rebate configuration is invalid for points payment.");
            }

            var requiredPoints = Math.Ceiling((grossTotal / rebate.RebateValue) * rebate.PointsRequired);
            if (member.Points < requiredPoints)
            {
                throw new InvalidOperationException("Member does not have enough points for this purchase.");
            }

            return requiredPoints;
        }

        private static void ValidateRebateEligibility(RebateRule rebate, decimal grossTotal, bool hasProducts, bool hasFuel)
        {
            var appliesTo = rebate.AppliesTo.Trim();
            var appliesToProduct = string.Equals(appliesTo, "Product", StringComparison.OrdinalIgnoreCase) || string.Equals(appliesTo, "Both", StringComparison.OrdinalIgnoreCase);
            var appliesToFuel = string.Equals(appliesTo, "Fuel", StringComparison.OrdinalIgnoreCase) || string.Equals(appliesTo, "Both", StringComparison.OrdinalIgnoreCase);

            if ((hasProducts && !appliesToProduct) || (hasFuel && !appliesToFuel))
            {
                throw new InvalidOperationException("Active rebate does not apply to every item in this sale.");
            }

            if (grossTotal < rebate.MinimumPurchase)
            {
                throw new InvalidOperationException("Sale total does not meet the active rebate minimum purchase.");
            }
        }

        private async Task<string> GenerateReceiptNo(DateTime now)
        {
            var prefix = $"POS-{now:yyyyMMdd}-";
            var start = now.Date;
            var end = start.AddDays(1);
            var count = await _db.Sales.CountAsync(sale => sale.CreatedAt >= start && sale.CreatedAt < end);
            return $"{prefix}{count + 1:000000}";
        }

        private static string NormalizePaymentMethod(string? paymentMethod)
        {
            return string.Equals((paymentMethod ?? string.Empty).Trim(), "Points", StringComparison.OrdinalIgnoreCase)
                ? "Points"
                : "Cash";
        }

        private void AddPaymentRecords(int saleId, string paymentMethod, decimal cashAmount, decimal rebateAmount, DateTime now)
        {
            if (paymentMethod == "Cash")
            {
                _db.Payments.Add(new Payment
                {
                    SaleId = saleId,
                    PaymentType = "Cash",
                    Amount = cashAmount,
                    Status = "Completed",
                    CreatedAt = now,
                    UpdatedAt = now
                });
            }

            if (paymentMethod == "Points")
            {
                _db.Payments.Add(new Payment
                {
                    SaleId = saleId,
                    PaymentType = "Points",
                    Amount = rebateAmount,
                    Status = "Completed",
                    CreatedAt = now,
                    UpdatedAt = now
                });
            }
        }

        private sealed class ValidatedProductSaleItem
        {
            public int DisplayStockId { get; init; }
            public int ProductId { get; init; }
            public int? CategoryId { get; init; }
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

        private sealed record VoucherRedemptionResult(Voucher Voucher, VoucherRule Rule, decimal DiscountAmount);
    }
}

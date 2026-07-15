using gpos.Filters;
using gpos.Data;
using gpos.Models;
using gpos.Models.ViewModels;
using gpos.Services;
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
        private const int CashStatusInactive = 0;
        private const int CashStatusOpen = 1;
        private const int CashStatusRemitted = 4;
        private readonly ApplicationDbContext _db;
        private readonly FinancialMetricsService _financialMetrics;
        private readonly ProductBatchNumberService _batchNumberService;

        public TransactionController(ApplicationDbContext db, FinancialMetricsService financialMetrics, ProductBatchNumberService batchNumberService)
        {
            _db = db;
            _financialMetrics = financialMetrics;
            _batchNumberService = batchNumberService;
        }

        public async Task<IActionResult> CashRemittance(string? search, int? branchId, int? shiftId, int? userId, DateTime? dateFrom, DateTime? dateTo, string? status, int? editId)
        {
            return View(await BuildCashRemittancePageAsync(search, branchId, shiftId, userId, dateFrom, dateTo, status, editId: editId));
        }

        public async Task<IActionResult> POS()
        {
            return View(await BuildPosPageAsync());
        }

        [HttpGet]
        public async Task<IActionResult> POSRecentSales()
        {
            var context = await CurrentPosUserContextAsync();
            if (context is null)
            {
                return Unauthorized(new { success = false, message = "A valid user and active Branch are required." });
            }

            var sales = await _db.Sales.AsNoTracking()
                .Where(sale => sale.UserId == context.UserId
                    && sale.BranchId == context.BranchId
                    && sale.Status == "Completed")
                .OrderByDescending(sale => sale.CreatedAt)
                .ThenByDescending(sale => sale.Id)
                .Take(20)
                .Select(sale => new
                {
                    sale.Id,
                    sale.ReceiptNo,
                    sale.CreatedAt,
                    itemCount = sale.ProductSales.Count + sale.FuelSales.Count,
                    paymentMethod = sale.Payments.OrderBy(payment => payment.Id).Select(payment => payment.PaymentType).FirstOrDefault() ?? "Cash",
                    total = sale.NetTotal,
                    sale.Status
                })
                .ToListAsync();

            return Ok(new { success = true, sales });
        }

        [HttpGet]
        public async Task<IActionResult> POSSaleDetails(int id)
        {
            var context = await CurrentPosUserContextAsync();
            if (context is null)
            {
                return Unauthorized(new { success = false, message = "A valid user and active Branch are required." });
            }

            var sale = await _db.Sales.AsNoTracking()
                .Include(item => item.Branch)
                .Include(item => item.User)
                .Include(item => item.DailyCash).ThenInclude(item => item!.Shift)
                .Include(item => item.Member)
                .Include(item => item.ProductSales).ThenInclude(item => item.Product)
                .Include(item => item.ProductSales).ThenInclude(item => item.Batch)
                .Include(item => item.FuelSales).ThenInclude(item => item.Fuel)
                .Include(item => item.FuelSales).ThenInclude(item => item.Tank)
                .Include(item => item.FuelSales).ThenInclude(item => item.Nozzle)
                .Include(item => item.Payments)
                .Include(item => item.PointsLedger)
                .Include(item => item.VoucherRedemptions)
                .FirstOrDefaultAsync(item => item.Id == id
                    && item.UserId == context.UserId
                    && item.BranchId == context.BranchId
                    && item.Status == "Completed");
            if (sale is null)
            {
                return NotFound(new { success = false, message = "The completed sale was not found for the current user and Branch." });
            }

            var voucherDiscount = sale.VoucherRedemptions.Sum(item => item.DiscountAmount);
            var paymentTotal = sale.Payments.Where(item => item.Status == "Completed").Sum(item => item.Amount);
            return Ok(new
            {
                success = true,
                sale = new
                {
                    sale.Id,
                    sale.ReceiptNo,
                    businessDate = sale.DailyCash?.BusinessDate ?? (sale.CreatedAt ?? DateTime.Now).Date,
                    sale.CreatedAt,
                    branch = sale.Branch?.Name ?? context.BranchName,
                    cashier = UserDisplayName(sale.User),
                    dailyCash = sale.DailyCashId?.ToString() ?? "-",
                    shift = sale.DailyCash?.Shift?.Name ?? "-",
                    sale.Status,
                    paymentMethod = PaymentTypeFor(sale),
                    sale.CashAmount,
                    change = Math.Max(0m, sale.CashAmount - sale.NetTotal),
                    memberDiscount = Math.Max(0m, sale.DiscountAmount - voucherDiscount),
                    voucherDiscount,
                    totalDiscount = sale.DiscountAmount,
                    sale.RebateAmount,
                    sale.NetTotal,
                    sale.GrossTotal,
                    member = sale.Member?.FullName ?? "-",
                    products = sale.ProductSales.OrderBy(item => item.Id).Select(item => new
                    {
                        product = item.Product?.Name ?? "-",
                        batch = item.Batch?.BatchNo ?? "-",
                        item.Quantity,
                        sellingPrice = item.UnitPrice > 0 ? item.UnitPrice : item.Price,
                        item.UnitCost,
                        item.Subtotal,
                        item.GrossProfit
                    }),
                    fuels = sale.FuelSales.OrderBy(item => item.Id).Select(item => new
                    {
                        nozzle = item.Nozzle?.NozzleNo ?? "-",
                        fuel = item.Fuel?.Name ?? "-",
                        item.Liters,
                        item.PricePerLiter,
                        tank = item.Tank?.TankNo ?? "-",
                        item.Subtotal
                    }),
                    payments = sale.Payments.OrderBy(item => item.Id).Select(item => new
                    {
                        method = item.PaymentType,
                        item.Amount,
                        reference = item.ReferenceNo ?? "-",
                        item.Status
                    }),
                    paymentTotal
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> POSDisplayRefillOptions()
        {
            var context = await CurrentPosUserContextAsync();
            if (context is null)
            {
                return Unauthorized(new { success = false, message = "A valid user and active Branch are required." });
            }
            if (!await CanManageDisplayRefillAsync(context.UserId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { success = false, message = "You are not authorized to refill display stock." });
            }

            var stocks = await _db.WarehouseStocks.AsNoTracking()
                .Include(item => item.Product)
                .Include(item => item.Batch)
                .Where(item => item.BranchId == context.BranchId
                    && item.Quantity > 0
                    && item.Product != null && item.Product.Status == 1 && item.Product.IsActive
                    && item.Batch != null && item.Batch.Status == 1 && item.Batch.IsActive)
                .ToListAsync();
            var products = stocks.GroupBy(item => new { item.ProductId, item.Product!.Name })
                .OrderBy(group => group.Key.Name)
                .Select(group => new
                {
                    productId = group.Key.ProductId,
                    no = $"P-{group.Key.ProductId:000}",
                    product = group.Key.Name,
                    availableQuantity = group.Sum(item => item.Quantity),
                    oldestBatch = group.OrderBy(item => item.Batch!.CreatedAt ?? DateTime.MinValue).ThenBy(item => item.BatchId).Select(item => item.Batch!.BatchNo).FirstOrDefault() ?? "-",
                    activeBatchCount = group.Select(item => item.BatchId).Distinct().Count()
                })
                .ToList();
            return Ok(new { success = true, branch = context.BranchName, products });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> POSDisplayRefill([FromBody] PosDisplayRefillRequest? request)
        {
            var context = await CurrentPosUserContextAsync();
            if (context is null)
            {
                return Unauthorized(new { success = false, message = "A valid user and active Branch are required." });
            }
            if (!await CanManageDisplayRefillAsync(context.UserId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { success = false, message = "You are not authorized to refill display stock." });
            }
            if (request is null || !ModelState.IsValid || request.ProductId <= 0 || request.Quantity <= 0)
            {
                return BadRequest(new { success = false, message = "Select a product and enter a valid transfer quantity." });
            }

            var form = new StockTransferForm
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Remarks = request.Remarks?.Trim(),
                SourceBranchId = context.BranchId,
                DestinationBranchId = context.BranchId
            };
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(item => item.Id == request.ProductId && item.Status == 1 && item.IsActive)
                    ?? throw new InvalidOperationException("The selected product is no longer active.");
                var now = DateTime.Now;
                var transfer = new StockTransfer
                {
                    TransferNo = await GenerateTransferNoAsync(),
                    TransferType = "WarehouseToDisplayProduct",
                    SourceBranchId = context.BranchId,
                    DestinationBranchId = context.BranchId,
                    SourceLocation = "Warehouse",
                    DestinationLocation = "Display",
                    Status = "Completed",
                    Remarks = form.Remarks,
                    TransferredBy = context.UserId,
                    CompletedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                _db.StockTransfers.Add(transfer);
                await _db.SaveChangesAsync();
                await ApplyWarehouseSourceFifo(transfer, form, product.Name, context.UserId, now);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { success = true, message = "Display stock refilled successfully.", transferNo = transfer.TransferNo });
            }
            catch (InvalidOperationException ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Unable to complete the display refill." });
            }
        }

        private async Task<PosPageViewModel> BuildPosPageAsync()
        {
            var userId = CurrentUserId();
            var branchId = userId.HasValue ? await CurrentUserBranchIdAsync(userId.Value) : null;
            var displayStocks = await _db.DisplayStocks
                .AsNoTracking()
                .Include(stock => stock.Product)
                    .ThenInclude(product => product!.Category)
                .Include(stock => stock.Batch)
                .Where(stock => stock.Quantity > 0
                    && stock.Product != null
                    && stock.Product.Status == 1
                    && stock.Product.IsActive
                    && stock.Product.CategoryId.HasValue
                    && stock.Product.Category != null
                    && stock.Product.Category.Status == 1
                    && stock.Product.Category.IsActive
                    && stock.Batch != null
                    && stock.Batch.Status == 1
                    && stock.Batch.IsActive
                    && (!branchId.HasValue || stock.BranchId == branchId.Value))
                .ToListAsync();

            var productIds = displayStocks.Select(stock => stock.ProductId).Distinct().ToList();
            var latestBatches = await _db.ProductBatches
                .AsNoTracking()
                .Where(batch => productIds.Contains(batch.ProductId) && batch.Status == 1 && batch.IsActive)
                .OrderByDescending(batch => batch.CreatedAt ?? DateTime.MinValue)
                .ThenByDescending(batch => batch.Id)
                .ToListAsync();
            var latestBatchLookup = latestBatches
                .GroupBy(batch => batch.ProductId)
                .ToDictionary(group => group.Key, group => group.First());

            var nozzles = branchId.HasValue ? await _db.Nozzles
                .AsNoTracking()
                .Include(nozzle => nozzle.Pump).ThenInclude(pump => pump!.Dispenser)
                .Include(nozzle => nozzle.Pump).ThenInclude(pump => pump!.Tank).ThenInclude(tank => tank!.Fuel)
                .Where(nozzle => nozzle.Status == 1 && nozzle.Pump != null && nozzle.Pump.Status == 1
                    && nozzle.Pump.Dispenser != null && nozzle.Pump.Dispenser.Status == 1 && nozzle.Pump.Dispenser.BranchId == branchId.Value
                    && nozzle.Pump.Tank != null && nozzle.Pump.Tank.BranchId == branchId.Value && nozzle.Pump.Tank.Status == 1 && nozzle.Pump.Tank.IsActive && nozzle.Pump.Tank.CurrentLiters > 0
                    && nozzle.Pump.Tank.Fuel != null && nozzle.Pump.Tank.Fuel.Status == 1 && nozzle.Pump.Tank.Fuel.IsActive)
                .OrderBy(nozzle => nozzle.Pump!.Name).ThenBy(nozzle => nozzle.NozzleNo).ToListAsync() : new List<Nozzle>();
            var branchFuelPrices = branchId.HasValue
                ? await _db.BranchFuelPrices.AsNoTracking()
                    .Where(price => price.BranchId == branchId.Value && price.Status == 1)
                    .ToDictionaryAsync(price => price.FuelId, price => price.CurrentPricePerLiter)
                : new Dictionary<int, decimal>();
            var activeRebate = await _db.RebateRules
                .AsNoTracking()
                .Where(rebate => rebate.Status == 1)
                .OrderByDescending(rebate => rebate.CreatedAt)
                .ThenByDescending(rebate => rebate.Id)
                .Select(rebate => new PosRebateViewModel
                {
                    Name = rebate.Name,
                    PointsRequired = rebate.PointsRequired,
                    RebateValue = rebate.RebateValue
                })
                .FirstOrDefaultAsync();
            var activeVat = await _db.VatSettings
                .AsNoTracking()
                .Where(setting => setting.IsActive)
                .OrderByDescending(setting => setting.IsDefault)
                .ThenByDescending(setting => setting.CreatedAt)
                .ThenByDescending(setting => setting.Id)
                .Select(setting => new PosVatViewModel
                {
                    Name = setting.Name,
                    Rate = setting.Rate,
                    Type = setting.Type
                })
                .FirstOrDefaultAsync();

            return new PosPageViewModel
            {
                CurrentUserId = userId,
                CurrentBranchId = branchId,
                CurrentBranchName = branchId.HasValue ? await BranchNameAsync(branchId) : string.Empty,
                CanDisplayRefill = userId.HasValue && branchId.HasValue && await CanManageDisplayRefillAsync(userId.Value),
                ActiveRebate = activeRebate,
                ActiveVat = activeVat,
                Categories = displayStocks
                    .Where(stock => stock.Product?.Category != null)
                    .GroupBy(stock => stock.Product!.Category!.Id)
                    .OrderBy(group => group.First().Product!.Category!.Name)
                    .Select(group =>
                    {
                        var category = group.First().Product!.Category!;
                        return new PosCategoryCardViewModel
                        {
                            Id = category.Id,
                            Name = category.Name,
                            Code = category.Name.Length >= 3 ? category.Name[..3].ToUpper() : category.Name.ToUpper()
                        };
                    })
                    .ToList(),
                Products = displayStocks
                    .GroupBy(stock => stock.ProductId)
                    .OrderBy(group => group.First().Product!.Name)
                    .Select(group =>
                    {
                        var product = group.First().Product!;
                        var latestBatch = latestBatchLookup.TryGetValue(product.Id, out var batch)
                            ? batch
                            : group.Select(stock => stock.Batch!)
                                .OrderByDescending(item => item.CreatedAt ?? DateTime.MinValue)
                                .ThenByDescending(item => item.Id)
                                .First();
                        return new PosProductCardViewModel
                        {
                            ProductId = product.Id,
                            CategoryId = product.CategoryId,
                            ProductName = product.Name,
                            ProductCode = $"P-{product.Id:000}",
                            VisualText = product.Name.Length >= 2 ? product.Name[..2].ToUpper() : product.Name.ToUpper(),
                            AvailableQuantity = group.Sum(stock => stock.Quantity),
                            Price = latestBatch.SellingPrice,
                            UnitCost = latestBatch.CostPrice
                        };
                    })
                    .ToList(),
                Fuels = nozzles.Select(nozzle => new PosFuelOptionViewModel
                    {
                        NozzleId = nozzle.Id, NozzleName = nozzle.Name ?? string.Empty, NozzleNo = nozzle.NozzleNo,
                        PumpName = nozzle.Pump!.Name, PumpNo = nozzle.Pump.PumpNo,
                        DispenserName = nozzle.Pump.Dispenser!.Name,
                        FuelId = nozzle.Pump.Tank!.FuelId, TankId = nozzle.Pump.Tank.Id, TankNo = nozzle.Pump.Tank.TankNo,
                        Name = nozzle.Pump.Tank.Fuel!.Name, AvailableLiters = nozzle.Pump.Tank.CurrentLiters,
                        Price = branchFuelPrices.TryGetValue(nozzle.Pump.Tank.FuelId, out var price) ? price : nozzle.Pump.Tank.Fuel.CurrentPricePerLiter
                    })
                    .Select((option, index) =>
                    {
                        option.IsChecked = index == 0;
                        return option;
                    })
                    .ToList()
            };
        }

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

        [HttpGet]
        public async Task<IActionResult> VoucherDiscount(string? voucherCode, string? membershipCardNo, decimal productTotal = 0m, decimal fuelTotal = 0m)
        {
            var normalizedCode = NormalizeVoucherCode(voucherCode);
            if (string.IsNullOrWhiteSpace(normalizedCode))
            {
                return Ok(new { success = true, discountAmount = 0m });
            }

            try
            {
                var member = await FindMember(membershipCardNo);
                var redemption = await ValidateAndCalculateVoucherRedemption(
                    normalizedCode,
                    member,
                    Math.Max(0m, productTotal),
                    Math.Max(0m, fuelTotal),
                    DateTime.Now);

                return Ok(new
                {
                    success = true,
                    discountAmount = redemption.DiscountAmount,
                    voucherCode = redemption.Rule.Code ?? redemption.Voucher?.Code
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
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
                var saleBranchId = await CurrentUserBranchIdAsync(userId.Value);
                var productItems = await BuildValidatedProductItems(request.Products);
                var fuelItems = await BuildValidatedFuelItems(request.Fuels, saleBranchId);
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
                        voucherRedemption = await ValidateAndCalculateVoucherRedemption(voucherCode, member, productTotal, fuelTotal, now);
                        voucherDiscountAmount = voucherRedemption.DiscountAmount;
                    }

                    discountAmount = memberDiscountAmount + voucherDiscountAmount;
                    pointsRequired = 0m;
                    rebateAmount = 0m;
                }

                var saleDailyCashId = saleBranchId.HasValue
                    ? await FindOpenDailyCashForSaleAsync(saleBranchId.Value, userId.Value, now)
                    : null;
                var vatSetting = await FindCurrentVatSetting();
                var taxableAmount = Math.Max(0m, grossTotal - discountAmount - rebateAmount);
                var taxAmount = CalculateVatAmount(taxableAmount, vatSetting);
                var netTotal = Math.Max(0m, taxableAmount + ExclusiveVatAmount(taxAmount, vatSetting));
                var cashAmount = isPointsPayment || netTotal <= 0m ? 0m : Math.Max(0m, request.CashAmount);
                var pointsEarned = isPointsPayment ? 0m : await CalculateMemberEarnings(member, productTotal, fuelTotal, netTotal, now);

                if (!isPointsPayment && netTotal > 0m && cashAmount < netTotal)
                {
                    return BadRequest(new { success = false, message = "Cash amount is not enough to checkout." });
                }

                await DeductDisplayStock(productItems, userId.Value);
                await DeductTankFuel(fuelItems);

                var sale = new Sale
                {
                    ReceiptNo = await GenerateReceiptNo(now),
                    UserId = userId.Value,
                    BranchId = saleBranchId,
                    DailyCashId = saleDailyCashId,
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
                if (saleDailyCashId.HasValue)
                {
                    await RefreshDailyCashSnapshotAsync(saleDailyCashId.Value);
                    await _db.SaveChangesAsync();
                }
                await UpdateFinancialMetricsForSale(sale, productItems, fuelItems, discountAmount, rebateAmount, isPointsPayment);
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
                    taxAmount,
                    vatRate = vatSetting?.Rate ?? 0m,
                    vatType = vatSetting?.Type ?? "None",
                    netTotal,
                    pointsEarned,
                    pointsRequired,
                    paymentMethod,
                    voucherCode = voucherRedemption?.Rule.Code ?? voucherRedemption?.Voucher?.Code,
                    amountTendered = cashAmount,
                    change = Math.Max(0m, cashAmount - netTotal)
                });
            }
            catch (InvalidOperationException ex)
            {
                await dbTransaction.RollbackAsync();
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IActionResult> ProductSales(string? search, DateTime? dateFrom, DateTime? dateTo, string? status, int? branchId)
        {
            var normalizedStatus = NormalizeStatus(status);
            var query = BuildProductSalesQuery(search, dateFrom, dateTo, normalizedStatus, branchId);
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
                BranchId = branchId,
                Status = normalizedStatus ?? string.Empty,
                BranchOptions = await BuildBranchFilterOptionsAsync(),
                StatusOptions = BuildStatusOptions(normalizedStatus),
                Sales = items
                    .Select(item =>
                    {
                        var row = ToProductSaleRow(item);
                        ApplyItemSummary(row, itemSummaries);
                        return row;
                    })
                    .ToList()
            });
        }

        public async Task<IActionResult> FuelSales(string? search, DateTime? dateFrom, DateTime? dateTo, string? status, int? branchId)
        {
            var normalizedStatus = NormalizeStatus(status);
            var query = BuildFuelSalesQuery(search, dateFrom, dateTo, normalizedStatus, branchId);
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
                BranchId = branchId,
                Status = normalizedStatus ?? string.Empty,
                BranchOptions = await BuildBranchFilterOptionsAsync(),
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

        public async Task<IActionResult> DailyCash(string? search, int? branchId, int? shiftId, int? userId, DateTime? dateFrom, DateTime? dateTo, string? status, int? editId)
        {
            return View(await BuildDailyCashPageAsync(search, branchId, shiftId, userId, dateFrom, dateTo, status, editId: editId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDailyCash([Bind(Prefix = "Form")] DailyCashForm form, string? search, int? filterBranchId, int? filterShiftId, int? filterUserId, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            if (!ModelState.IsValid)
            {
                return View("DailyCash", await BuildDailyCashPageAsync(search, filterBranchId, filterShiftId, filterUserId, dateFrom, dateTo, status, form, "dailyCashFormModal"));
            }

            if (form.Id > 0 && !await _db.DailyCashRecords.AnyAsync(item => item.Id == form.Id && item.Status == CashStatusOpen))
            {
                ModelState.AddModelError(string.Empty, "A remitted Daily Cash session is readonly.");
                return View("DailyCash", await BuildDailyCashPageAsync(search, filterBranchId, filterShiftId, filterUserId, dateFrom, dateTo, status, form));
            }

            var validationError = await ValidateCashScopeAsync(form.BranchId, form.ShiftId, form.UserId);
            if (validationError is not null)
            {
                ModelState.AddModelError(string.Empty, validationError);
                return View("DailyCash", await BuildDailyCashPageAsync(search, filterBranchId, filterShiftId, filterUserId, dateFrom, dateTo, status, form, "dailyCashFormModal"));
            }

            var businessDate = form.BusinessDate!.Value.Date;
            var duplicateOpen = await _db.DailyCashRecords.AnyAsync(item => item.Id != form.Id
                && item.Status == CashStatusOpen
                && item.BranchId == form.BranchId
                && item.ShiftId == form.ShiftId
                && item.UserId == form.UserId
                && item.BusinessDate == businessDate);

            if (duplicateOpen)
            {
                ModelState.AddModelError(string.Empty, "An open Daily Cash session already exists for this branch, business date, shift, and cashier.");
                return View("DailyCash", await BuildDailyCashPageAsync(search, filterBranchId, filterShiftId, filterUserId, dateFrom, dateTo, status, form, "dailyCashFormModal"));
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;
                var dailyCash = form.Id > 0
                    ? await _db.DailyCashRecords.FindAsync(form.Id)
                    : new DailyCash { CreatedAt = now, CreatedByUserId = CurrentUserId() };

                if (dailyCash is null)
                {
                    return NotFound();
                }

                var totals = await CalculateDailyCashTotalsAsync(form.BranchId, form.ShiftId, form.UserId, businessDate, form.Id);

                dailyCash.BranchId = form.BranchId;
                dailyCash.ShiftId = form.ShiftId;
                dailyCash.UserId = form.UserId;
                dailyCash.BusinessDate = businessDate;
                dailyCash.OpeningCash = form.OpeningCash;
                dailyCash.CashSales = totals.CashSales;
                dailyCash.TotalCashIn = totals.TotalCashIn;
                dailyCash.TotalCashOut = totals.TotalCashOut;
                dailyCash.ExpectedCash = form.OpeningCash + totals.CashSales + totals.TotalCashIn - totals.TotalCashOut;
                dailyCash.ActualCash = form.ActualCash;
                dailyCash.Difference = form.ActualCash - dailyCash.ExpectedCash;
                dailyCash.Remarks = CleanOptional(form.Remarks);
                dailyCash.OpenedAt ??= now;
                dailyCash.Status = CashStatusOpen;
                dailyCash.UpdatedAt = now;

                if (form.Id == 0)
                {
                    _db.DailyCashRecords.Add(dailyCash);
                    await _db.SaveChangesAsync();
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return RedirectToAction(nameof(DailyCash), new { search, branchId = filterBranchId, shiftId = filterShiftId, userId = filterUserId, dateFrom, dateTo, status });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDailyCash(int id, string? search, int? branchId, int? shiftId, int? userId, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            var item = await _db.DailyCashRecords.FindAsync(id);
            if (item is not null)
            {
                item.Status = 0;
                item.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(DailyCash), new { search, branchId, shiftId, userId, dateFrom, dateTo, status });
        }

        public async Task<IActionResult> CashIn(string? search, int? branchId, int? shiftId, int? userId, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            return View(await BuildCashMovementPageAsync(true, search, branchId, shiftId, userId, dateFrom, dateTo, status));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCashIn([Bind(Prefix = "Form")] CashMovementForm form, string? search, int? filterBranchId, int? filterShiftId, int? filterUserId, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            return await SaveCashMovementAsync(true, form, search, filterBranchId, filterShiftId, filterUserId, dateFrom, dateTo, status);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCashIn(int id, string? search, int? branchId, int? shiftId, int? userId, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            var item = await _db.CashIns.FindAsync(id);
            if (item is not null)
            {
                item.Status = 0;
                item.UpdatedAt = DateTime.UtcNow;
                if (item.DailyCashId.HasValue)
                {
                    await RefreshDailyCashSnapshotAsync(item.DailyCashId.Value);
                }
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(CashIn), new { search, branchId, shiftId, userId, dateFrom, dateTo, status });
        }

        public async Task<IActionResult> CashOut(string? search, int? branchId, int? shiftId, int? userId, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            return View(await BuildCashMovementPageAsync(false, search, branchId, shiftId, userId, dateFrom, dateTo, status));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCashOut([Bind(Prefix = "Form")] CashMovementForm form, string? search, int? filterBranchId, int? filterShiftId, int? filterUserId, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            return await SaveCashMovementAsync(false, form, search, filterBranchId, filterShiftId, filterUserId, dateFrom, dateTo, status);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCashOut(int id, string? search, int? branchId, int? shiftId, int? userId, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            var item = await _db.CashOuts.FindAsync(id);
            if (item is not null)
            {
                item.Status = 0;
                item.UpdatedAt = DateTime.UtcNow;
                if (item.DailyCashId.HasValue)
                {
                    await RefreshDailyCashSnapshotAsync(item.DailyCashId.Value);
                }
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(CashOut), new { search, branchId, shiftId, userId, dateFrom, dateTo, status });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCashRemittance([Bind(Prefix = "Form")] CashRemittanceForm form, string? search, int? filterBranchId, int? filterShiftId, int? filterUserId, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            var currentUserId = CurrentUserId();
            if (!currentUserId.HasValue || !await _db.Users.AsNoTracking().AnyAsync(user => user.Id == currentUserId.Value && user.Status == 1))
            {
                ModelState.AddModelError(string.Empty, "Please sign in with an active account before creating a Cash Remittance.");
            }
            if (form.BranchId <= 0)
            {
                ModelState.AddModelError("Form.BranchId", "Branch is required.");
            }
            if (!ModelState.IsValid)
            {
                return View("CashRemittance", await BuildCashRemittancePageAsync(search, filterBranchId, filterShiftId, filterUserId, dateFrom, dateTo, status, form, "cashRemittanceModal"));
            }

            if (!await _db.Branches.AsNoTracking().AnyAsync(branch => branch.Id == form.BranchId && branch.Status == 1))
            {
                ModelState.AddModelError("Form.BranchId", "Select a valid active Branch.");
                return View("CashRemittance", await BuildCashRemittancePageAsync(search, filterBranchId, filterShiftId, filterUserId, dateFrom, dateTo, status, form, "cashRemittanceModal"));
            }

            await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var now = DateTime.UtcNow;
                var dailyCash = await _db.DailyCashRecords.FirstOrDefaultAsync(item => item.Id == form.DailyCashId && item.BranchId == form.BranchId && item.Status == CashStatusOpen);
                if (dailyCash is null) throw new InvalidOperationException("Select an open Daily Cash record from the selected Branch.");
                if (await _db.CashRemittances.AnyAsync(remittance => remittance.DailyCashId == dailyCash.Id && remittance.Status == 1))
                {
                    throw new InvalidOperationException("This Daily Cash has already been remitted.");
                }
                var remittanceDifference = form.RemittedAmount - dailyCash.ExpectedCash;
                if (remittanceDifference != 0m && string.IsNullOrWhiteSpace(form.Remarks))
                {
                    throw new InvalidOperationException("Remarks are required when the remittance has a shortage or excess.");
                }

                var item = new CashRemittance
                {
                    RemittanceNo = await GenerateRemittanceNoAsync(now),
                    BranchId = dailyCash.BranchId,
                    ShiftId = dailyCash.ShiftId,
                    UserId = dailyCash.UserId,
                    DailyCashId = dailyCash.Id,
                    ExpectedCash = dailyCash.ExpectedCash,
                    ActualCash = dailyCash.ActualCash,
                    RemittedAmount = form.RemittedAmount,
                    RemittanceDifference = remittanceDifference,
                    ReceivedByUserId = currentUserId!.Value,
                    ReceivedDateTime = form.ReceivedDateTime!.Value,
                    Remarks = CleanOptional(form.Remarks),
                    Status = 1,
                    CreatedByUserId = currentUserId.Value,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                _db.CashRemittances.Add(item);
                dailyCash.RemittedAmount = form.RemittedAmount;
                dailyCash.Status = CashStatusRemitted;
                dailyCash.UpdatedAt = now;
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (InvalidOperationException ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("CashRemittance", await BuildCashRemittancePageAsync(search, filterBranchId, filterShiftId, filterUserId, dateFrom, dateTo, status, form, "cashRemittanceModal"));
            }

            return RedirectToAction(nameof(CashRemittance), new { search, branchId = filterBranchId, shiftId = filterShiftId, userId = filterUserId, dateFrom, dateTo, status });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCashRemittance(int id, string? search, int? branchId, int? shiftId, int? userId, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            TempData["CashMessage"] = "Cash Remittance records are permanent financial audit records and cannot be deactivated.";
            return RedirectToAction(nameof(CashRemittance), new { search, branchId, shiftId, userId, dateFrom, dateTo, status });
        }

        [HttpGet]
        public async Task<IActionResult> ShiftOpeningCash(int shiftId)
        {
            var amount = await _db.ShiftSettings
                .AsNoTracking()
                .Where(shift => shift.Id == shiftId)
                .Select(shift => shift.OpeningCashAmount ?? 0m)
                .FirstOrDefaultAsync();

            return Json(new { openingCash = amount });
        }

        [HttpGet]
        public async Task<IActionResult> SearchCashUsers(string? search, int take = 20)
        {
            var searchText = (search ?? string.Empty).Trim();
            var resultLimit = Math.Clamp(take, 1, 50);
            var query = _db.Users.AsNoTracking().Where(user => user.Status == 1);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(user => user.Username.Contains(searchText)
                    || user.Email.Contains(searchText)
                    || (user.FullName != null && user.FullName.Contains(searchText)));
            }

            var users = await query
                .OrderBy(user => user.FullName ?? user.Username)
                .Take(resultLimit)
                .Select(user => new
                {
                    id = user.Id,
                    name = string.IsNullOrWhiteSpace(user.FullName) ? user.Username : user.FullName,
                    username = user.Username,
                    email = user.Email
                })
                .ToListAsync();

            return Json(users);
        }

        [HttpGet]
        public async Task<IActionResult> SearchDailyCashSessions(string? search, string? mode, int? branchId, int take = 20)
        {
            var searchText = (search ?? string.Empty).Trim();
            var resultLimit = Math.Clamp(take, 1, 50);
            var allowedStatuses = new[] { CashStatusOpen };

            var query = _db.DailyCashRecords.AsNoTracking()
                .Include(item => item.Branch)
                .Include(item => item.Shift)
                .Include(item => item.User)
                .Where(item => allowedStatuses.Contains(item.Status));

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(item => item.BranchId == branchId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(item => (item.Branch != null && item.Branch.Name.Contains(searchText))
                    || (item.Shift != null && item.Shift.Name.Contains(searchText))
                    || (item.User != null && ((item.User.FullName != null && item.User.FullName.Contains(searchText)) || item.User.Username.Contains(searchText)))
                    || item.BusinessDate.ToString().Contains(searchText));
            }

            var records = await query
                .OrderByDescending(item => item.BusinessDate)
                .ThenByDescending(item => item.Id)
                .Take(resultLimit)
                .ToListAsync();

            var rows = records
                .Select(item => new
                {
                    id = item.Id,
                    display = $"{item.BusinessDate:yyyy-MM-dd} - {item.Branch!.Name} - {item.Shift!.Name} - {(string.IsNullOrWhiteSpace(item.User!.FullName) ? item.User.Username : item.User.FullName)}",
                    businessDate = item.BusinessDate.ToString("yyyy-MM-dd"),
                    branch = item.Branch!.Name,
                    shift = item.Shift!.Name,
                    user = string.IsNullOrWhiteSpace(item.User!.FullName) ? item.User.Username : item.User.FullName,
                    expectedCash = item.ExpectedCash,
                    actualCash = item.ActualCash,
                    countDifference = item.ActualCash - item.ExpectedCash,
                    remittedAmount = item.RemittedAmount,
                    remainingAmount = item.ActualCash - item.RemittedAmount,
                    status = CashStatusName(item.Status)
                })
                .ToList();

            return Json(rows);
        }

        [HttpGet]
        public async Task<IActionResult> SearchDailyCashForCashIn(int branchId, string? term, DateTime? businessDate, int take = 20)
        {
            if (branchId <= 0)
            {
                return Json(Array.Empty<object>());
            }

            var searchText = (term ?? string.Empty).Trim();
            var resultLimit = Math.Clamp(take, 1, 50);

            var query = _db.DailyCashRecords.AsNoTracking()
                .Include(item => item.Branch)
                .Include(item => item.Shift)
                .Include(item => item.User)
                .Where(item => item.BranchId == branchId && item.Status == CashStatusOpen);

            if (businessDate.HasValue)
            {
                var date = businessDate.Value.Date;
                query = query.Where(item => item.BusinessDate == date);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(item => (item.Branch != null && item.Branch.Name.Contains(searchText))
                    || (item.Shift != null && item.Shift.Name.Contains(searchText))
                    || (item.User != null && ((item.User.FullName != null && item.User.FullName.Contains(searchText)) || item.User.Username.Contains(searchText)))
                    || item.BusinessDate.ToString().Contains(searchText)
                    || item.Id.ToString().Contains(searchText));
            }

            var records = await query
                .OrderByDescending(item => item.BusinessDate)
                .ThenByDescending(item => item.OpenedAt)
                .ThenByDescending(item => item.Id)
                .Take(resultLimit)
                .ToListAsync();

            var rows = records
                .Select(item => new
                {
                    dailyCashId = item.Id,
                    dailyCashNo = $"DC-{item.Id:000000}",
                    branchId = item.BranchId,
                    branchName = item.Branch != null ? item.Branch.Name : "-",
                    businessDate = item.BusinessDate.ToString("yyyy-MM-dd"),
                    shiftId = item.ShiftId,
                    shiftName = item.Shift != null ? item.Shift.Name : "-",
                    userId = item.UserId,
                    cashierName = item.User == null ? "-" : (string.IsNullOrWhiteSpace(item.User.FullName) ? item.User.Username : item.User.FullName),
                    openingCash = item.OpeningCash,
                    cashSales = item.CashSales,
                    cashIn = item.TotalCashIn,
                    cashOut = item.TotalCashOut,
                    expectedCash = item.ExpectedCash,
                    actualCash = item.ActualCash,
                    status = CashStatusName(item.Status)
                })
                .ToList();

            return Json(rows);
        }

        [HttpGet]
        public async Task<IActionResult> DailyCashSessionDetails(int id)
        {
            var item = await _db.DailyCashRecords.AsNoTracking()
                .Include(record => record.Branch)
                .Include(record => record.Shift)
                .Include(record => record.User)
                .FirstOrDefaultAsync(record => record.Id == id);

            if (item is null)
            {
                return NotFound();
            }

            return Json(new
            {
                id = item.Id,
                branchId = item.BranchId,
                branch = item.Branch?.Name ?? "-",
                shiftId = item.ShiftId,
                shift = item.Shift?.Name ?? "-",
                userId = item.UserId,
                user = UserDisplayName(item.User),
                businessDate = item.BusinessDate.ToString("yyyy-MM-dd"),
                expectedCash = item.ExpectedCash,
                actualCash = item.ActualCash,
                remittedAmount = item.RemittedAmount,
                remainingAmount = item.ActualCash - item.RemittedAmount,
                status = CashStatusName(item.Status)
            });
        }

        private async Task<IActionResult> SaveCashMovementAsync(bool isCashIn, CashMovementForm form, string? search, int? filterBranchId, int? filterShiftId, int? filterUserId, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            if (!ModelState.IsValid)
            {
                return View(isCashIn ? "CashIn" : "CashOut", await BuildCashMovementPageAsync(isCashIn, search, filterBranchId, filterShiftId, filterUserId, dateFrom, dateTo, status, form, isCashIn ? "cashInFormModal" : "cashOutFormModal"));
            }

            var dailyCash = await _db.DailyCashRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == form.DailyCashId
                    && item.BranchId == form.BranchId
                    && item.Status == CashStatusOpen);

            if (dailyCash is null)
            {
                ModelState.AddModelError("Form.DailyCashId", "Select an open Daily Cash session from the selected branch.");
                return View(isCashIn ? "CashIn" : "CashOut", await BuildCashMovementPageAsync(isCashIn, search, filterBranchId, filterShiftId, filterUserId, dateFrom, dateTo, status, form, isCashIn ? "cashInFormModal" : "cashOutFormModal"));
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;
                var transactionDateTime = form.TransactionDateTime!.Value;

                if (isCashIn)
                {
                    var item = new CashIn
                    {
                        BranchId = dailyCash.BranchId,
                        ShiftId = dailyCash.ShiftId,
                        UserId = dailyCash.UserId,
                        DailyCashId = dailyCash.Id,
                        TransactionDateTime = transactionDateTime,
                        Amount = form.Amount,
                        Reason = form.Reason.Trim(),
                        Remarks = CleanOptional(form.Remarks),
                        CreatedByUserId = CurrentUserId(),
                        Status = 1,
                        CreatedAt = now,
                        UpdatedAt = now
                    };
                    _db.CashIns.Add(item);
                }
                else
                {
                    var item = new CashOut
                    {
                        BranchId = dailyCash.BranchId,
                        ShiftId = dailyCash.ShiftId,
                        UserId = dailyCash.UserId,
                        DailyCashId = dailyCash.Id,
                        TransactionDateTime = transactionDateTime,
                        Amount = form.Amount,
                        Reason = form.Reason.Trim(),
                        Remarks = CleanOptional(form.Remarks),
                        CreatedByUserId = CurrentUserId(),
                        Status = 1,
                        CreatedAt = now,
                        UpdatedAt = now
                    };
                    _db.CashOuts.Add(item);
                }

                await _db.SaveChangesAsync();
                await RefreshDailyCashSnapshotAsync(dailyCash.Id);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return RedirectToAction(isCashIn ? nameof(CashIn) : nameof(CashOut), new { search, branchId = filterBranchId, shiftId = filterShiftId, userId = filterUserId, dateFrom, dateTo, status });
        }

        private async Task<DailyCashPageViewModel> BuildDailyCashPageAsync(string? search, int? branchId, int? shiftId, int? userId, DateTime? dateFrom, DateTime? dateTo, string? status, DailyCashForm? form = null, string activeModalId = "", int? editId = null)
        {
            var filter = BuildCashFilter(search, branchId, shiftId, userId, dateFrom, dateTo, status);
            var query = _db.DailyCashRecords.AsNoTracking()
                .Include(item => item.Branch)
                .Include(item => item.Shift)
                .Include(item => item.User)
                .Include(item => item.ReceivedByUser)
                .AsQueryable();

            ApplyDailyCashFilters(ref query, filter);

            return new DailyCashPageViewModel
            {
                Filter = filter,
                Form = form ?? await BuildDailyCashFormAsync(editId),
                Records = await query.OrderByDescending(item => item.BusinessDate).ThenByDescending(item => item.Id).Take(200).ToListAsync(),
                BranchOptions = await BuildCashBranchOptionsAsync("All Branches"),
                ShiftOptions = await BuildCashShiftOptionsAsync("All Shifts"),
                UserOptions = await BuildCashUserOptionsAsync("All Users"),
                ActiveModalId = activeModalId
            };
        }

        private async Task<CashMovementPageViewModel> BuildCashMovementPageAsync(bool isCashIn, string? search, int? branchId, int? shiftId, int? userId, DateTime? dateFrom, DateTime? dateTo, string? status, CashMovementForm? form = null, string activeModalId = "")
        {
            var filter = BuildCashFilter(search, branchId, shiftId, userId, dateFrom, dateTo, status);
            if (form is not null)
            {
                await PopulateCashMovementFormContextAsync(form);
            }
            var page = new CashMovementPageViewModel
            {
                Filter = filter,
                Form = form ?? new CashMovementForm(),
                BranchOptions = await BuildCashBranchOptionsAsync("All Branches"),
                ShiftOptions = await BuildCashShiftOptionsAsync("All Shifts"),
                UserOptions = await BuildCashUserOptionsAsync("All Users"),
                DailyCashOptions = await BuildDailyCashOptionsAsync("Optional Daily Cash"),
                ActiveModalId = activeModalId
            };

            if (isCashIn)
            {
                var query = _db.CashIns.AsNoTracking()
                    .Include(item => item.Branch)
                    .Include(item => item.Shift)
                    .Include(item => item.User)
                    .Include(item => item.CreatedByUser)
                    .Include(item => item.DailyCash)
                    .AsQueryable();
                ApplyCashInFilters(ref query, filter);
                page.CashIns = await query.OrderByDescending(item => item.TransactionDateTime).ThenByDescending(item => item.Id).Take(200).ToListAsync();
            }
            else
            {
                var query = _db.CashOuts.AsNoTracking()
                    .Include(item => item.Branch)
                    .Include(item => item.Shift)
                    .Include(item => item.User)
                    .Include(item => item.CreatedByUser)
                    .Include(item => item.DailyCash)
                    .AsQueryable();
                ApplyCashOutFilters(ref query, filter);
                page.CashOuts = await query.OrderByDescending(item => item.TransactionDateTime).ThenByDescending(item => item.Id).Take(200).ToListAsync();
            }

            return page;
        }

        private async Task<CashRemittancePageViewModel> BuildCashRemittancePageAsync(string? search, int? branchId, int? shiftId, int? userId, DateTime? dateFrom, DateTime? dateTo, string? status, CashRemittanceForm? form = null, string activeModalId = "", int? editId = null)
        {
            var filter = BuildCashFilter(search, branchId, shiftId, userId, dateFrom, dateTo, status);
            if (form is not null)
            {
                await PopulateCashRemittanceFormContextAsync(form);
            }
            var query = _db.CashRemittances.AsNoTracking()
                .Include(item => item.Branch)
                .Include(item => item.Shift)
                .Include(item => item.User)
                .Include(item => item.ReceivedByUser)
                .Include(item => item.CreatedByUser)
                .Include(item => item.DailyCash)
                .AsQueryable();

            ApplyCashRemittanceFilters(ref query, filter);

            return new CashRemittancePageViewModel
            {
                CurrentUserName = CurrentUserId() is int currentUserId
                    ? await _db.Users.AsNoTracking().Where(user => user.Id == currentUserId).Select(user => string.IsNullOrWhiteSpace(user.FullName) ? user.Username : user.FullName).FirstOrDefaultAsync() ?? string.Empty
                    : string.Empty,
                Filter = filter,
                Form = form ?? await BuildCashRemittanceFormAsync(editId),
                Records = await query.OrderByDescending(item => item.ReceivedDateTime).ThenByDescending(item => item.Id).Take(200).ToListAsync(),
                BranchOptions = await BuildCashBranchOptionsAsync("All Branches"),
                ShiftOptions = await BuildCashShiftOptionsAsync("All Shifts"),
                UserOptions = await BuildCashUserOptionsAsync("All Users"),
                DailyCashOptions = await BuildDailyCashOptionsAsync("Select Daily Cash"),
                ActiveModalId = activeModalId
            };
        }

        private async Task<DailyCashForm> BuildDailyCashFormAsync(int? editId)
        {
            var item = editId.HasValue
                ? await _db.DailyCashRecords.AsNoTracking()
                    .Include(record => record.Branch)
                    .Include(record => record.User)
                    .FirstOrDefaultAsync(record => record.Id == editId.Value && record.Status == CashStatusOpen)
                : null;
            if (item is null)
            {
                return new DailyCashForm();
            }

            return new DailyCashForm
            {
                Id = item.Id,
                BranchId = item.BranchId,
                BranchName = item.Branch?.Name ?? string.Empty,
                ShiftId = item.ShiftId,
                UserId = item.UserId,
                UserName = UserDisplayName(item.User),
                BusinessDate = item.BusinessDate,
                OpeningCash = item.OpeningCash,
                ActualCash = item.ActualCash,
                Remarks = item.Remarks,
                Status = item.Status
            };
        }

        private async Task<CashRemittanceForm> BuildCashRemittanceFormAsync(int? editId)
        {
            var item = editId.HasValue
                ? await _db.CashRemittances.AsNoTracking()
                    .Include(record => record.DailyCash)
                        .ThenInclude(dailyCash => dailyCash!.Branch)
                    .Include(record => record.DailyCash)
                        .ThenInclude(dailyCash => dailyCash!.Shift)
                    .Include(record => record.DailyCash)
                        .ThenInclude(dailyCash => dailyCash!.User)
                    .Include(record => record.ReceivedByUser)
                    .FirstOrDefaultAsync(record => record.Id == editId.Value)
                : null;
            if (item is null)
            {
                return new CashRemittanceForm();
            }

            var form = new CashRemittanceForm
            {
                Id = item.Id,
                DailyCashId = item.DailyCashId,
                RemittedAmount = item.RemittedAmount,
                ReceivedByUserId = item.ReceivedByUserId,
                ReceivedByName = UserDisplayName(item.ReceivedByUser),
                ReceivedDateTime = item.ReceivedDateTime,
                Remarks = item.Remarks
            };
            await PopulateCashRemittanceFormContextAsync(form);
            return form;
        }

        private static CashModuleFilter BuildCashFilter(string? search, int? branchId, int? shiftId, int? userId, DateTime? dateFrom, DateTime? dateTo, string? status)
        {
            var normalizedStatus = NormalizeCashStatus(status);
            return new CashModuleFilter
            {
                Search = (search ?? string.Empty).Trim(),
                BranchId = branchId > 0 ? branchId : null,
                ShiftId = shiftId > 0 ? shiftId : null,
                UserId = userId > 0 ? userId : null,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Status = normalizedStatus
            };
        }

        private static string NormalizeCashStatus(string? status)
        {
            var value = (status ?? string.Empty).Trim();
            return string.Equals(value, "Open", StringComparison.OrdinalIgnoreCase) ? "Open"
                : string.Equals(value, "Active", StringComparison.OrdinalIgnoreCase) ? "Active"
                : string.Equals(value, "Remitted", StringComparison.OrdinalIgnoreCase) ? "Remitted"
                : string.Equals(value, "Inactive", StringComparison.OrdinalIgnoreCase) ? "Inactive"
                : string.Empty;
        }

        public static string CashStatusName(int status)
        {
            return status switch
            {
                CashStatusOpen => "Open",
                _ => "Remitted"
            };
        }

        public static string CashStatusBadge(int status)
        {
            return status switch
            {
                CashStatusOpen => "bg-primary",
                _ => "bg-success"
            };
        }

        private static void ApplyDailyCashFilters(ref IQueryable<DailyCash> query, CashModuleFilter filter)
        {
            if (filter.BranchId.HasValue) query = query.Where(item => item.BranchId == filter.BranchId.Value);
            if (filter.ShiftId.HasValue) query = query.Where(item => item.ShiftId == filter.ShiftId.Value);
            if (filter.UserId.HasValue) query = query.Where(item => item.UserId == filter.UserId.Value);
            if (filter.DateFrom.HasValue) query = query.Where(item => item.BusinessDate >= filter.DateFrom.Value.Date);
            if (filter.DateTo.HasValue) query = query.Where(item => item.BusinessDate < filter.DateTo.Value.Date.AddDays(1));
            if (filter.Status == "Open") query = query.Where(item => item.Status == CashStatusOpen);
            if (filter.Status == "Remitted") query = query.Where(item => item.Status != CashStatusOpen);
            if (filter.Status == "Inactive") query = query.Where(item => item.Status == CashStatusInactive);
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(item => item.Branch != null && item.Branch.Name.Contains(filter.Search)
                    || item.Shift != null && item.Shift.Name.Contains(filter.Search)
                    || item.User != null && ((item.User.FullName != null && item.User.FullName.Contains(filter.Search)) || item.User.Username.Contains(filter.Search))
                    || item.Remarks != null && item.Remarks.Contains(filter.Search));
            }
        }

        private static void ApplyCashInFilters(ref IQueryable<CashIn> query, CashModuleFilter filter)
        {
            if (filter.BranchId.HasValue) query = query.Where(item => item.BranchId == filter.BranchId.Value);
            if (filter.ShiftId.HasValue) query = query.Where(item => item.ShiftId == filter.ShiftId.Value);
            if (filter.UserId.HasValue) query = query.Where(item => item.UserId == filter.UserId.Value);
            if (filter.DateFrom.HasValue) query = query.Where(item => item.TransactionDateTime >= filter.DateFrom.Value.Date);
            if (filter.DateTo.HasValue) query = query.Where(item => item.TransactionDateTime < filter.DateTo.Value.Date.AddDays(1));
            if (filter.Status == "Active") query = query.Where(item => item.Status == 1);
            if (filter.Status == "Inactive") query = query.Where(item => item.Status != 1);
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(item => item.Reason.Contains(filter.Search)
                    || item.Remarks != null && item.Remarks.Contains(filter.Search)
                    || item.Branch != null && item.Branch.Name.Contains(filter.Search)
                    || item.Shift != null && item.Shift.Name.Contains(filter.Search)
                    || item.User != null && ((item.User.FullName != null && item.User.FullName.Contains(filter.Search)) || item.User.Username.Contains(filter.Search)));
            }
        }

        private static void ApplyCashOutFilters(ref IQueryable<CashOut> query, CashModuleFilter filter)
        {
            if (filter.BranchId.HasValue) query = query.Where(item => item.BranchId == filter.BranchId.Value);
            if (filter.ShiftId.HasValue) query = query.Where(item => item.ShiftId == filter.ShiftId.Value);
            if (filter.UserId.HasValue) query = query.Where(item => item.UserId == filter.UserId.Value);
            if (filter.DateFrom.HasValue) query = query.Where(item => item.TransactionDateTime >= filter.DateFrom.Value.Date);
            if (filter.DateTo.HasValue) query = query.Where(item => item.TransactionDateTime < filter.DateTo.Value.Date.AddDays(1));
            if (filter.Status == "Active") query = query.Where(item => item.Status == 1);
            if (filter.Status == "Inactive") query = query.Where(item => item.Status != 1);
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(item => item.Reason.Contains(filter.Search)
                    || item.Remarks != null && item.Remarks.Contains(filter.Search)
                    || item.Branch != null && item.Branch.Name.Contains(filter.Search)
                    || item.Shift != null && item.Shift.Name.Contains(filter.Search)
                    || item.User != null && ((item.User.FullName != null && item.User.FullName.Contains(filter.Search)) || item.User.Username.Contains(filter.Search)));
            }
        }

        private static void ApplyCashRemittanceFilters(ref IQueryable<CashRemittance> query, CashModuleFilter filter)
        {
            if (filter.BranchId.HasValue) query = query.Where(item => item.BranchId == filter.BranchId.Value);
            if (filter.ShiftId.HasValue) query = query.Where(item => item.ShiftId == filter.ShiftId.Value);
            if (filter.UserId.HasValue) query = query.Where(item => item.UserId == filter.UserId.Value);
            if (filter.DateFrom.HasValue) query = query.Where(item => item.ReceivedDateTime >= filter.DateFrom.Value.Date);
            if (filter.DateTo.HasValue) query = query.Where(item => item.ReceivedDateTime < filter.DateTo.Value.Date.AddDays(1));
            if (filter.Status == "Active") query = query.Where(item => item.Status == 1);
            if (filter.Status == "Inactive") query = query.Where(item => item.Status != 1);
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(item => item.RemittanceNo.Contains(filter.Search)
                    || item.Remarks != null && item.Remarks.Contains(filter.Search)
                    || item.Branch != null && item.Branch.Name.Contains(filter.Search)
                    || item.Shift != null && item.Shift.Name.Contains(filter.Search)
                    || item.User != null && ((item.User.FullName != null && item.User.FullName.Contains(filter.Search)) || item.User.Username.Contains(filter.Search))
                    || item.ReceivedByUser != null && ((item.ReceivedByUser.FullName != null && item.ReceivedByUser.FullName.Contains(filter.Search)) || item.ReceivedByUser.Username.Contains(filter.Search)));
            }
        }

        private async Task<(decimal CashSales, decimal TotalCashIn, decimal TotalCashOut)> CalculateDailyCashTotalsAsync(int branchId, int shiftId, int userId, DateTime businessDate, int dailyCashId)
        {
            var nextDate = businessDate.AddDays(1);
            var salesQuery = _db.Sales.AsNoTracking()
                .Where(sale => sale.Status == "Completed");

            if (dailyCashId > 0)
            {
                salesQuery = salesQuery.Where(sale => sale.DailyCashId == dailyCashId
                    || (!sale.DailyCashId.HasValue
                        && sale.BranchId == branchId
                        && sale.UserId == userId
                        && (sale.CreatedAt ?? DateTime.MinValue) >= businessDate
                        && (sale.CreatedAt ?? DateTime.MinValue) < nextDate));
            }
            else
            {
                salesQuery = salesQuery.Where(sale => sale.BranchId == branchId
                    && sale.UserId == userId
                    && (sale.CreatedAt ?? DateTime.MinValue) >= businessDate
                    && (sale.CreatedAt ?? DateTime.MinValue) < nextDate);
            }

            var cashSales = await salesQuery.SumAsync(sale => sale.CashAmount);

            var cashInQuery = _db.CashIns.AsNoTracking()
                .Where(item => item.BranchId == branchId && item.ShiftId == shiftId && item.UserId == userId && item.Status == 1);
            var cashOutQuery = _db.CashOuts.AsNoTracking()
                .Where(item => item.BranchId == branchId && item.ShiftId == shiftId && item.UserId == userId && item.Status == 1);

            if (dailyCashId > 0)
            {
                cashInQuery = cashInQuery.Where(item => item.DailyCashId == dailyCashId || (!item.DailyCashId.HasValue && item.TransactionDateTime >= businessDate && item.TransactionDateTime < nextDate));
                cashOutQuery = cashOutQuery.Where(item => item.DailyCashId == dailyCashId || (!item.DailyCashId.HasValue && item.TransactionDateTime >= businessDate && item.TransactionDateTime < nextDate));
            }
            else
            {
                cashInQuery = cashInQuery.Where(item => item.TransactionDateTime >= businessDate && item.TransactionDateTime < nextDate);
                cashOutQuery = cashOutQuery.Where(item => item.TransactionDateTime >= businessDate && item.TransactionDateTime < nextDate);
            }

            return (cashSales, await cashInQuery.SumAsync(item => item.Amount), await cashOutQuery.SumAsync(item => item.Amount));
        }

        private async Task<int?> FindOpenDailyCashForSaleAsync(int branchId, int userId, DateTime saleDateTime)
        {
            return await _db.DailyCashRecords.AsNoTracking()
                .Where(item => item.Status == CashStatusOpen
                    && item.BranchId == branchId
                    && item.UserId == userId
                    && item.BusinessDate == saleDateTime.Date)
                .OrderByDescending(item => item.OpenedAt ?? item.CreatedAt ?? DateTime.MinValue)
                .ThenByDescending(item => item.Id)
                .Select(item => (int?)item.Id)
                .FirstOrDefaultAsync();
        }

        private async Task LinkCashMovementsToDailyCashAsync(DailyCash dailyCash)
        {
            var start = dailyCash.BusinessDate.Date;
            var end = start.AddDays(1);
            var cashIns = await _db.CashIns.Where(item => !item.DailyCashId.HasValue
                && item.BranchId == dailyCash.BranchId
                && item.ShiftId == dailyCash.ShiftId
                && item.UserId == dailyCash.UserId
                && item.TransactionDateTime >= start
                && item.TransactionDateTime < end)
                .ToListAsync();
            var cashOuts = await _db.CashOuts.Where(item => !item.DailyCashId.HasValue
                && item.BranchId == dailyCash.BranchId
                && item.ShiftId == dailyCash.ShiftId
                && item.UserId == dailyCash.UserId
                && item.TransactionDateTime >= start
                && item.TransactionDateTime < end)
                .ToListAsync();

            cashIns.ForEach(item => item.DailyCashId = dailyCash.Id);
            cashOuts.ForEach(item => item.DailyCashId = dailyCash.Id);
        }

        private async Task RefreshDailyCashSnapshotAsync(int dailyCashId)
        {
            var dailyCash = await _db.DailyCashRecords.FindAsync(dailyCashId);
            if (dailyCash is null)
            {
                return;
            }

            var totals = await CalculateDailyCashTotalsAsync(dailyCash.BranchId, dailyCash.ShiftId, dailyCash.UserId, dailyCash.BusinessDate.Date, dailyCash.Id);
            dailyCash.CashSales = totals.CashSales;
            dailyCash.TotalCashIn = totals.TotalCashIn;
            dailyCash.TotalCashOut = totals.TotalCashOut;
            dailyCash.ExpectedCash = dailyCash.OpeningCash + totals.CashSales + totals.TotalCashIn - totals.TotalCashOut;
            dailyCash.Difference = dailyCash.ActualCash - dailyCash.ExpectedCash;
            dailyCash.RemittedAmount = await _db.CashRemittances
                .Where(remittance => remittance.DailyCashId == dailyCash.Id && remittance.Status == 1)
                .SumAsync(remittance => remittance.RemittedAmount);
            dailyCash.UpdatedAt = DateTime.UtcNow;
        }

        private async Task<int?> FindMatchingDailyCashIdAsync(int branchId, int shiftId, int userId, DateTime businessDate)
        {
            return await _db.DailyCashRecords.AsNoTracking()
                .Where(item => item.Status == 1
                    && item.BranchId == branchId
                    && item.ShiftId == shiftId
                    && item.UserId == userId
                    && item.BusinessDate == businessDate.Date)
                .OrderByDescending(item => item.Id)
                .Select(item => (int?)item.Id)
                .FirstOrDefaultAsync();
        }

        private async Task<string?> ValidateCashScopeAsync(int branchId, int shiftId, int userId)
        {
            var branchExists = await _db.Branches.AnyAsync(item => item.Id == branchId && item.Status == 1);
            if (!branchExists) return "Select an active branch.";
            var shiftExists = await _db.ShiftSettings.AnyAsync(item => item.Id == shiftId && item.Status == 1);
            if (!shiftExists) return "Select an active shift.";
            var userExists = await _db.Users.AnyAsync(item => item.Id == userId && item.Status == 1);
            return userExists ? null : "Select an active user.";
        }

        private async Task<List<SelectListItem>> BuildCashBranchOptionsAsync(string label)
        {
            var options = await _db.Branches.AsNoTracking().Where(item => item.Status == 1).OrderBy(item => item.Name).Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Name }).ToListAsync();
            options.Insert(0, new SelectListItem { Value = "", Text = label });
            return options;
        }

        private async Task<List<SelectListItem>> BuildCashShiftOptionsAsync(string label)
        {
            var options = await _db.ShiftSettings.AsNoTracking().Where(item => item.Status == 1).OrderBy(item => item.Name).Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Name }).ToListAsync();
            options.Insert(0, new SelectListItem { Value = "", Text = label });
            return options;
        }

        private async Task<List<SelectListItem>> BuildCashUserOptionsAsync(string label)
        {
            var options = await _db.Users.AsNoTracking().Where(item => item.Status == 1).OrderBy(item => item.FullName ?? item.Username).Select(item => new SelectListItem { Value = item.Id.ToString(), Text = string.IsNullOrWhiteSpace(item.FullName) ? item.Username : item.FullName }).ToListAsync();
            options.Insert(0, new SelectListItem { Value = "", Text = label });
            return options;
        }

        private async Task<List<SelectListItem>> BuildDailyCashOptionsAsync(string label)
        {
            var records = await _db.DailyCashRecords.AsNoTracking()
                .Include(item => item.Branch)
                .Include(item => item.Shift)
                .Include(item => item.User)
                .Where(item => item.Status == 1)
                .OrderByDescending(item => item.BusinessDate)
                .ThenByDescending(item => item.Id)
                .Take(100)
                .ToListAsync();

            var options = records
                .Select(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = $"{item.BusinessDate:yyyy-MM-dd} - {item.Branch?.Name ?? "-"} - {item.Shift?.Name ?? "-"} - {UserDisplayName(item.User)}"
                })
                .ToList();
            options.Insert(0, new SelectListItem { Value = "", Text = label });
            return options;
        }

        private async Task PopulateCashMovementFormContextAsync(CashMovementForm form)
        {
            if (form.DailyCashId <= 0)
            {
                if (form.BranchId > 0 && string.IsNullOrWhiteSpace(form.BranchName))
                {
                    var branch = await _db.Branches.AsNoTracking().FirstOrDefaultAsync(item => item.Id == form.BranchId);
                    form.BranchName = branch?.Name ?? string.Empty;
                }

                return;
            }

            var dailyCash = await _db.DailyCashRecords.AsNoTracking()
                .Include(item => item.Branch)
                .Include(item => item.Shift)
                .Include(item => item.User)
                .FirstOrDefaultAsync(item => item.Id == form.DailyCashId);

            if (dailyCash is null)
            {
                return;
            }

            form.DailyCashDisplay = $"{dailyCash.BusinessDate:yyyy-MM-dd} - {dailyCash.Branch?.Name ?? "-"} - {dailyCash.Shift?.Name ?? "-"} - {UserDisplayName(dailyCash.User)}";
            form.BranchId = dailyCash.BranchId;
            form.BranchName = dailyCash.Branch?.Name ?? string.Empty;
            form.ShiftId = dailyCash.ShiftId;
            form.ShiftName = dailyCash.Shift?.Name ?? string.Empty;
            form.UserId = dailyCash.UserId;
            form.UserName = UserDisplayName(dailyCash.User);
            form.BusinessDate = dailyCash.BusinessDate;
        }

        private async Task PopulateCashRemittanceFormContextAsync(CashRemittanceForm form)
        {
            if (form.DailyCashId > 0)
            {
                var dailyCash = await _db.DailyCashRecords.AsNoTracking()
                    .Include(item => item.Branch)
                    .Include(item => item.Shift)
                    .Include(item => item.User)
                    .FirstOrDefaultAsync(item => item.Id == form.DailyCashId);

                if (dailyCash is not null)
                {
                    form.DailyCashDisplay = $"{dailyCash.BusinessDate:yyyy-MM-dd} - {dailyCash.Branch?.Name ?? "-"} - {dailyCash.Shift?.Name ?? "-"} - {UserDisplayName(dailyCash.User)}";
                    form.BranchId = dailyCash.BranchId;
                    form.BranchName = dailyCash.Branch?.Name ?? string.Empty;
                    form.ShiftId = dailyCash.ShiftId;
                    form.ShiftName = dailyCash.Shift?.Name ?? string.Empty;
                    form.UserId = dailyCash.UserId;
                    form.UserName = UserDisplayName(dailyCash.User);
                    form.BusinessDate = dailyCash.BusinessDate;
                    form.ExpectedCash = dailyCash.ExpectedCash;
                    form.ActualCash = dailyCash.ActualCash;
                    form.TotalRemitted = dailyCash.RemittedAmount;
                    form.RemainingAmount = dailyCash.ActualCash - dailyCash.RemittedAmount;
                }
            }

            if (form.ReceivedByUserId > 0 && string.IsNullOrWhiteSpace(form.ReceivedByName))
            {
                var receiver = await _db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == form.ReceivedByUserId);
                form.ReceivedByName = UserDisplayName(receiver);
            }
        }

        private async Task<string> GenerateRemittanceNoAsync(DateTime now)
        {
            var prefix = $"REMIT-{now:yyyyMMdd}-";
            var count = await _db.CashRemittances.CountAsync(item => item.RemittanceNo.StartsWith(prefix));
            return $"{prefix}{count + 1:0000}";
        }

        public async Task<IActionResult> ProductReceiving(string? search, int? branchId, int? filterBranchId)
        {
            branchId = filterBranchId ?? branchId;
            return View(await BuildProductReceivingPage(search, branchId));
        }

        public async Task<IActionResult> FuelReceiving(string? search, int? branchId, int? filterBranchId)
        {
            branchId = filterBranchId ?? branchId;
            return View(await BuildFuelReceivingPage(search, branchId));
        }

        [HttpGet]
        public async Task<IActionResult> TankOptionsByBranch(int branchId)
        {
            var tanks = await _db.Tanks
                .AsNoTracking()
                .Include(tank => tank.Fuel)
                .Where(tank => tank.BranchId == branchId && tank.Status == 1 && tank.IsActive)
                .OrderBy(tank => tank.TankNo)
                .Select(tank => new
                {
                    id = tank.Id,
                    text = tank.Fuel != null ? $"{tank.TankNo} ({tank.Fuel.Name})" : tank.TankNo,
                    fuelId = tank.FuelId
                })
                .ToListAsync();

            return Json(tanks);
        }

        [HttpGet]
        public async Task<IActionResult> SearchFuelReceivingTanks(string? search, int? branchId, int take = 20)
        {
            var searchText = (search ?? string.Empty).Trim();
            var resultLimit = Math.Clamp(take, 1, 50);

            if (!branchId.HasValue || branchId.Value <= 0)
            {
                return Json(Array.Empty<object>());
            }

            var query = _db.Tanks
                .AsNoTracking()
                .Include(tank => tank.Branch)
                .Include(tank => tank.Fuel)
                .Where(tank => tank.Status == 1 && tank.IsActive && tank.BranchId == branchId.Value)
                .AsQueryable();

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
                    branchId = tank.BranchId,
                    branchName = tank.Branch != null ? tank.Branch.Name : "Unassigned",
                    fuelId = tank.FuelId,
                    fuelName = tank.Fuel != null ? tank.Fuel.Name : "-",
                    currentLiters = tank.CurrentLiters,
                    capacity = tank.CapacityLiters
                })
                .ToListAsync();

            return Json(tanks);
        }

        [HttpGet]
        public async Task<IActionResult> SearchFuelReceivingSuppliers(string? search, int take = 20)
        {
            var searchText = (search ?? string.Empty).Trim();
            var resultLimit = Math.Clamp(take, 1, 50);
            var query = _db.Suppliers
                .AsNoTracking()
                .Where(supplier => supplier.Status == 1)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(supplier =>
                    supplier.Name.Contains(searchText)
                    || (supplier.ContactPerson != null && supplier.ContactPerson.Contains(searchText))
                    || (supplier.ContactNumber != null && supplier.ContactNumber.Contains(searchText))
                    || (supplier.Address != null && supplier.Address.Contains(searchText))
                    || (searchText == "Active" && supplier.Status == 1)
                    || (searchText == "Inactive" && supplier.Status == 0));
            }

            var suppliers = await query
                .OrderBy(supplier => supplier.Name)
                .Take(resultLimit)
                .Select(supplier => new
                {
                    supplierId = supplier.Id,
                    supplierName = supplier.Name,
                    contact = !string.IsNullOrWhiteSpace(supplier.ContactPerson) ? supplier.ContactPerson : supplier.ContactNumber,
                    address = supplier.Address,
                    status = supplier.Status == 1 ? "Active" : "Inactive"
                })
                .ToListAsync();

            return Json(suppliers);
        }

        [HttpGet]
        public async Task<IActionResult> SearchProductReceivingProducts(string? search, int? branchId, int take = 20)
        {
            var searchText = (search ?? string.Empty).Trim();
            var resultLimit = Math.Clamp(take, 1, 50);

            if (!branchId.HasValue || branchId.Value <= 0)
            {
                return Json(Array.Empty<object>());
            }

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
                    unitName = product.ProductUnit != null ? product.ProductUnit.Name : "-"
                })
                .ToListAsync();

            var productIds = products.Select(product => product.productId).ToList();
            var quantityLookup = await _db.WarehouseStocks
                .AsNoTracking()
                .Where(stock => stock.BranchId == branchId.Value && productIds.Contains(stock.ProductId))
                .GroupBy(stock => stock.ProductId)
                .Select(group => new { ProductId = group.Key, Quantity = group.Sum(stock => stock.Quantity) })
                .ToDictionaryAsync(item => item.ProductId, item => item.Quantity);

            return Json(products.Select(product => new
            {
                product.productId,
                product.productName,
                product.categoryName,
                product.unitName,
                currentWarehouseQuantity = quantityLookup.TryGetValue(product.productId, out var quantity) ? quantity : 0m
            }));
        }

        [HttpGet]
        public async Task<IActionResult> SearchProductReceivingSuppliers(string? search, int take = 20)
        {
            var searchText = (search ?? string.Empty).Trim();
            var resultLimit = Math.Clamp(take, 1, 50);
            var query = _db.Suppliers
                .AsNoTracking()
                .Where(supplier => supplier.Status == 1)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(supplier =>
                    supplier.Name.Contains(searchText)
                    || (supplier.ContactPerson != null && supplier.ContactPerson.Contains(searchText))
                    || (supplier.ContactNumber != null && supplier.ContactNumber.Contains(searchText))
                    || (supplier.Address != null && supplier.Address.Contains(searchText)));
            }

            var suppliers = await query
                .OrderBy(supplier => supplier.Name)
                .Take(resultLimit)
                .Select(supplier => new
                {
                    supplierId = supplier.Id,
                    supplierName = supplier.Name,
                    contact = !string.IsNullOrWhiteSpace(supplier.ContactPerson) ? supplier.ContactPerson : supplier.ContactNumber,
                    address = supplier.Address
                })
                .ToListAsync();

            return Json(suppliers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProductReceiving([Bind(Prefix = "Form")] ProductReceivingForm form, string? search, int? filterBranchId)
        {
            RemoveProductReceivingUiOnlyModelState(form);

            var userId = CurrentUserId();
            if (!userId.HasValue)
            {
                TempData["ReceivingMessage"] = "Failed to save product receiving: Please sign in before saving receiving.";
                return RedirectToAction(nameof(ProductReceiving), new { search, filterBranchId });
            }

            ValidateProductReceivingForm(form);
            await ValidateProductReceivingReferencesAsync(form);
            if (!ModelState.IsValid)
            {
                return await ProductReceivingValidationView(search, filterBranchId, form);
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.Now;
                var product = await ResolveReceivingProduct(form, now);
                form.ProductId = product.Id;
                var receiving = new StockReceiving
                {
                    ReceivingNo = await GenerateProductReceivingNoAsync(),
                    BranchId = form.BranchId,
                    SupplierId = form.SupplierId,
                    ReceivedDate = form.ReceivedDate!.Value,
                    TotalAmount = form.Quantity * form.CostPrice,
                    Remarks = form.Remarks,
                    Status = 1,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                _db.StockReceivings.Add(receiving);
                await _db.SaveChangesAsync();

                var batch = await CreateGeneratedProductBatch(form, now);
                var item = new StockReceivingItem
                {
                    StockReceivingId = receiving.Id,
                    ProductId = form.ProductId,
                    ProductBatchId = batch.Id,
                    Quantity = form.Quantity,
                    CostPrice = form.CostPrice,
                    SellingPrice = form.SellingPrice,
                    ExpiryDate = form.ExpiryDate,
                    Subtotal = form.Quantity * form.CostPrice,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                receiving.Items.Add(item);
                await _db.SaveChangesAsync();
                await CompleteProductReceiving(receiving, item, userId.Value, now);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["ReceivingMessage"] = "Product receiving saved successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ReceivingMessage"] = $"Failed to save product receiving: {ex.Message}";
            }

            return RedirectToAction(nameof(ProductReceiving), new { search, filterBranchId });
        }

        private async Task<IActionResult> ProductReceivingValidationView(string? search, int? filterBranchId, ProductReceivingForm form)
        {
            ViewData["OpenProductReceivingModal"] = true;
            return View("ProductReceiving", await BuildProductReceivingPage(search, filterBranchId, form));
        }

        private async Task<IActionResult> SaveProductReceivingOld([Bind(Prefix = "Form")] ProductReceivingForm form, string? search, int? filterBranchId)
        {
            if (form.SellingPrice < form.CostPrice)
            {
                TempData["ReceivingWarning"] = "Selling Price is lower than Cost Price. Receiving was saved with warning.";
            }

            if (!ModelState.IsValid)
            {
                return View("ProductReceiving", await BuildProductReceivingPage(search, filterBranchId, form));
            }

            var userId = CurrentUserId();
            if (!userId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Please sign in before saving receiving.");
                return View("ProductReceiving", await BuildProductReceivingPage(search, filterBranchId, form));
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.Now;
                var receiving = form.Id > 0
                    ? await _db.StockReceivings.Include(item => item.Items).FirstOrDefaultAsync(item => item.Id == form.Id)
                    : new StockReceiving { ReceivingNo = await GenerateProductReceivingNoAsync(), ReceivedDate = now, CreatedAt = now };

                if (receiving is null)
                {
                    ModelState.AddModelError(string.Empty, "Receiving record was not found.");
                    return View("ProductReceiving", await BuildProductReceivingPage(search, filterBranchId, form));
                }

                if (form.Id > 0 && receiving.Status == 1)
                {
                    TempData["ReceivingMessage"] = "Completed receiving records cannot be edited. Create an adjustment instead.";
                    return RedirectToAction(nameof(ProductReceiving), new { search, filterBranchId });
                }

                var product = await ResolveReceivingProduct(form, now);
                await ValidateBranchAsync(form.BranchId);
                form.ProductId = product.Id;
                var batch = await CreateGeneratedProductBatch(form, now);
                var item = receiving.Items.FirstOrDefault();
                receiving.BranchId = form.BranchId;
                receiving.SupplierId = form.SupplierId;
                receiving.ReceivedDate = receiving.ReceivedDate == default ? now : receiving.ReceivedDate;
                receiving.TotalAmount = form.Quantity * form.CostPrice;
                receiving.Remarks = form.Remarks;
                receiving.Status = form.Status;
                receiving.UpdatedAt = now;

                if (item is null)
                {
                    item = new StockReceivingItem { CreatedAt = now };
                    receiving.Items.Add(item);
                }

                item.ProductId = form.ProductId;
                item.ProductBatchId = batch.Id;
                item.Quantity = form.Quantity;
                item.CostPrice = form.CostPrice;
                item.SellingPrice = form.SellingPrice;
                item.ExpiryDate = form.ExpiryDate;
                item.Subtotal = form.Quantity * form.CostPrice;
                item.UpdatedAt = now;

                if (form.Id == 0)
                {
                    _db.StockReceivings.Add(receiving);
                }

                await _db.SaveChangesAsync();

                if (form.Status == 1)
                {
                    await CompleteProductReceiving(receiving, item, userId.Value, now);
                    await _db.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return RedirectToAction(nameof(ProductReceiving), new { search, filterBranchId });
            }
            catch (InvalidOperationException ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("ProductReceiving", await BuildProductReceivingPage(search, filterBranchId, form));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProductReceiving(int id, string? search, int? filterBranchId)
        {
            var userId = CurrentUserId();
            if (!userId.HasValue)
            {
                TempData["ReceivingMessage"] = "Please sign in before completing receiving.";
                return RedirectToAction(nameof(ProductReceiving), new { search, filterBranchId });
            }

            var receiving = await _db.StockReceivings.Include(item => item.Items).ThenInclude(item => item.ProductBatch).FirstOrDefaultAsync(item => item.Id == id);
            var item = receiving?.Items.FirstOrDefault();
            if (receiving is null || item is null)
            {
                TempData["ReceivingMessage"] = "Receiving record was not found.";
                return RedirectToAction(nameof(ProductReceiving), new { search, filterBranchId });
            }

            if (receiving.Status == 1)
            {
                TempData["ReceivingMessage"] = "Completed receiving records cannot be edited. Create an adjustment instead.";
                return RedirectToAction(nameof(ProductReceiving), new { search, filterBranchId });
            }

            if (receiving.Status == 2)
            {
                TempData["ReceivingMessage"] = "Cancelled receiving records cannot be completed.";
                return RedirectToAction(nameof(ProductReceiving), new { search, filterBranchId });
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();
            receiving.Status = 1;
            receiving.UpdatedAt = DateTime.Now;
            await CompleteProductReceiving(receiving, item, userId.Value, DateTime.Now);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return RedirectToAction(nameof(ProductReceiving), new { search, filterBranchId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelProductReceiving(int id, string? search, int? filterBranchId)
        {
            var receiving = await _db.StockReceivings.FindAsync(id);
            if (receiving is not null && receiving.Status == 0)
            {
                receiving.Status = 2;
                receiving.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();
            }
            else if (receiving?.Status == 1)
            {
                TempData["ReceivingMessage"] = "Completed receiving records cannot be edited. Create an adjustment instead.";
            }

            return RedirectToAction(nameof(ProductReceiving), new { search, filterBranchId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFuelReceiving([Bind(Prefix = "Form")] FuelReceivingForm form, string? search, int? filterBranchId)
        {
            RemoveFuelReceivingUiOnlyModelState();

            var costInputMode = string.Equals(form.CostInputMode, "TotalCost", StringComparison.Ordinal)
                ? "TotalCost"
                : string.Equals(form.CostInputMode, "UnitCost", StringComparison.Ordinal) ? "UnitCost" : string.Empty;
            if (costInputMode.Length == 0)
            {
                ModelState.AddModelError("Form.CostInputMode", "Cost input mode must be UnitCost or TotalCost.");
            }
            else
            {
                form.CostInputMode = costInputMode;
                ModelState.Remove(costInputMode == "UnitCost" ? "Form.TotalCost" : "Form.CostPerLiter");
            }

            var submittedFuelId = form.FuelId;
            var tank = await _db.Tanks.Include(item => item.Fuel).FirstOrDefaultAsync(item => item.Id == form.TankId && item.Status == 1 && item.IsActive);
            if (tank is not null)
            {
                form.FuelId = tank.FuelId;
                if (form.BranchId <= 0 && tank.BranchId.HasValue)
                {
                    form.BranchId = tank.BranchId.Value;
                }

                ModelState.Remove("Form.FuelId");
                ModelState.Remove("Form.BranchId");
            }

            if (form.Liters <= 0)
            {
                ModelState.AddModelError("Form.Liters", "Liters must be greater than 0.");
            }

            if (costInputMode == "UnitCost" && form.CostPerLiter < 0)
            {
                ModelState.AddModelError("Form.CostPerLiter", "Cost/Liter must be greater than or equal to 0.");
            }

            if (costInputMode == "TotalCost" && form.TotalCost < 0)
            {
                ModelState.AddModelError("Form.TotalCost", "Total Cost must be greater than or equal to 0.");
            }

            if (form.Liters > 0 && costInputMode == "UnitCost" && form.CostPerLiter >= 0)
            {
                form.CostPerLiter = decimal.Round(form.CostPerLiter, 4, MidpointRounding.AwayFromZero);
                form.TotalCost = decimal.Round(form.Liters * form.CostPerLiter, 2, MidpointRounding.AwayFromZero);
            }
            else if (form.Liters > 0 && costInputMode == "TotalCost" && form.TotalCost >= 0)
            {
                form.TotalCost = decimal.Round(form.TotalCost, 2, MidpointRounding.AwayFromZero);
                form.CostPerLiter = decimal.Round(form.TotalCost / form.Liters, 4, MidpointRounding.AwayFromZero);
            }

            if (!form.ReceivedDate.HasValue)
            {
                ModelState.AddModelError("Form.ReceivedDate", "Date is required.");
            }

            if (!ModelState.IsValid)
            {
                return await FuelReceivingValidationView(search, filterBranchId, form);
            }

            if (tank is null)
            {
                ModelState.AddModelError("Form.TankId", "Tank is required.");
                return await FuelReceivingValidationView(search, filterBranchId, form);
            }

            if (tank.Fuel is null || tank.FuelId <= 0)
            {
                ModelState.AddModelError("Form.TankId", "Selected tank must have a fuel.");
                return await FuelReceivingValidationView(search, filterBranchId, form);
            }

            if (submittedFuelId > 0 && submittedFuelId != tank.FuelId)
            {
                ModelState.AddModelError("Form.FuelId", "Fuel must match the selected tank.");
                return await FuelReceivingValidationView(search, filterBranchId, form);
            }

            if (!tank.BranchId.HasValue || tank.BranchId.Value <= 0)
            {
                ModelState.AddModelError("Form.BranchId", "Selected tank must belong to a branch.");
                return await FuelReceivingValidationView(search, filterBranchId, form);
            }

            if (form.BranchId != tank.BranchId.Value)
            {
                ModelState.AddModelError("Form.TankId", "Tank must belong to the selected branch.");
                return await FuelReceivingValidationView(search, filterBranchId, form);
            }

            if (!await _db.Branches.AsNoTracking().AnyAsync(branch => branch.Id == form.BranchId && branch.Status == 1))
            {
                ModelState.AddModelError("Form.BranchId", "Branch is required.");
                return await FuelReceivingValidationView(search, filterBranchId, form);
            }

            if (!await _db.Suppliers.AsNoTracking().AnyAsync(supplier => supplier.Id == form.SupplierId && supplier.Status == 1))
            {
                ModelState.AddModelError("Form.SupplierId", "Supplier is required.");
                return await FuelReceivingValidationView(search, filterBranchId, form);
            }

            var tankLitersAfter = tank.CurrentLiters + form.Liters;
            if (tank.CapacityLiters > 0 && tankLitersAfter > tank.CapacityLiters)
            {
                ModelState.AddModelError("Form.Liters", "Receiving liters would exceed the selected tank capacity.");
                return await FuelReceivingValidationView(search, filterBranchId, form);
            }

            var now = DateTime.Now;
            var delivery = form.Id > 0 ? await _db.FuelDeliveries.FindAsync(form.Id) : new FuelDelivery { DeliveryNo = await GenerateFuelReceivingNoAsync(), CreatedAt = now };
            if (delivery is null)
            {
                ModelState.AddModelError(string.Empty, "Fuel receiving record was not found.");
                return await FuelReceivingValidationView(search, filterBranchId, form);
            }

            if (form.Id > 0 && delivery.Status == 1)
            {
                TempData["ReceivingMessage"] = "Completed receiving records cannot be edited. Create an adjustment instead.";
                return RedirectToAction(nameof(FuelReceiving), new { search, filterBranchId });
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();
            delivery.BranchId = tank.BranchId;
            delivery.SupplierId = form.SupplierId;
            delivery.FuelId = tank.FuelId;
            delivery.TankId = tank.Id;
            delivery.DeliveredLiters = form.Liters;
            delivery.CostPerLiter = form.CostPerLiter;
            delivery.TotalCost = form.TotalCost;
            delivery.DeliveryDate = form.ReceivedDate!.Value;
            delivery.Remarks = form.Remarks;
            delivery.Status = 1;
            delivery.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.FuelDeliveries.Add(delivery);
            }

            tank.CurrentLiters = tankLitersAfter;
            tank.UpdatedAt = now;

            await _db.SaveChangesAsync();
            await CreateFuelBatchForDelivery(delivery, tank, now);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
            return RedirectToAction(nameof(FuelReceiving), new { search, filterBranchId });
        }

        private async Task<IActionResult> FuelReceivingValidationView(string? search, int? filterBranchId, FuelReceivingForm form)
        {
            ViewData["OpenFuelReceivingModal"] = true;
            return View("FuelReceiving", await BuildFuelReceivingPage(search, filterBranchId, form));
        }

        private void RemoveProductReceivingUiOnlyModelState(ProductReceivingForm form)
        {
            ModelState.Remove("Form.BatchNo");
            ModelState.Remove("Form.Status");

            if (IsNewProductReceivingMode(form))
            {
                ModelState.Remove("Form.ProductId");
            }
            else
            {
                ModelState.Remove("Form.NewProductName");
                ModelState.Remove("Form.Name");
            }
        }

        private void RemoveFuelReceivingUiOnlyModelState()
        {
            ModelState.Remove("Form.Status");
            ModelState.Remove("Form.SellingPricePerLiter");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteFuelReceiving(int id, string? search, int? filterBranchId)
        {
            var delivery = await _db.FuelDeliveries.Include(item => item.Tank).Include(item => item.Fuel).FirstOrDefaultAsync(item => item.Id == id);
            if (delivery is null)
            {
                TempData["ReceivingMessage"] = "Fuel receiving record was not found.";
                return RedirectToAction(nameof(FuelReceiving), new { search, filterBranchId });
            }

            if (delivery.Status == 1)
            {
                TempData["ReceivingMessage"] = "Completed receiving records cannot be edited. Create an adjustment instead.";
                return RedirectToAction(nameof(FuelReceiving), new { search, filterBranchId });
            }

            if (delivery.Status == 2)
            {
                TempData["ReceivingMessage"] = "Cancelled receiving records cannot be completed.";
                return RedirectToAction(nameof(FuelReceiving), new { search, filterBranchId });
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();
            var now = DateTime.Now;
            if (delivery.Tank is not null)
            {
                var tankLitersAfter = delivery.Tank.CurrentLiters + delivery.DeliveredLiters;
                if (delivery.Tank.CapacityLiters > 0 && tankLitersAfter > delivery.Tank.CapacityLiters)
                {
                    await transaction.RollbackAsync();
                    TempData["ReceivingMessage"] = "Fuel receiving cannot be completed because it would exceed the selected tank capacity.";
                    return RedirectToAction(nameof(FuelReceiving), new { search, filterBranchId });
                }

                delivery.Tank.CurrentLiters = tankLitersAfter;
                delivery.Tank.UpdatedAt = now;
            }

            delivery.Status = 1;
            delivery.UpdatedAt = now;
            await _db.SaveChangesAsync();
            if (delivery.Tank is not null)
            {
                await CreateFuelBatchForDelivery(delivery, delivery.Tank, now);
                await _db.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            return RedirectToAction(nameof(FuelReceiving), new { search, filterBranchId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelFuelReceiving(int id, string? search, int? filterBranchId)
        {
            var delivery = await _db.FuelDeliveries.FindAsync(id);
            if (delivery is not null && delivery.Status == 0)
            {
                delivery.Status = 2;
                delivery.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();
            }
            else if (delivery?.Status == 1)
            {
                TempData["ReceivingMessage"] = "Completed receiving records cannot be edited. Create an adjustment instead.";
            }

            return RedirectToAction(nameof(FuelReceiving), new { search, filterBranchId });
        }
        public async Task<IActionResult> WarehouseToDisplayProduct(string? search, int? filterBranchId) => View(await BuildStockTransferPage("WarehouseToDisplayProduct", "Warehouse to Display Product", nameof(SaveWarehouseToDisplayProduct), nameof(CompleteWarehouseToDisplayProduct), nameof(CancelWarehouseToDisplayProduct), search, filterBranchId));
        public async Task<IActionResult> DisplayToWarehouseProduct(string? search, int? filterBranchId) => View(await BuildStockTransferPage("DisplayToWarehouseProduct", "Display to Warehouse Product", nameof(SaveDisplayToWarehouseProduct), nameof(CompleteDisplayToWarehouseProduct), nameof(CancelDisplayToWarehouseProduct), search, filterBranchId));
        public async Task<IActionResult> BranchToBranchProduct(string? search, int? sourceBranchId, int? destinationBranchId) => View(await BuildStockTransferPage("BranchToBranchProduct", "Branch to Branch Product", nameof(SaveBranchToBranchProduct), nameof(CompleteBranchToBranchProduct), nameof(CancelBranchToBranchProduct), search, sourceBranchId, destinationBranchId));
        public async Task<IActionResult> BranchToBranchFuel(string? search) => View(await BuildStockTransferPage("BranchToBranchFuel", "Branch to Branch Fuel", nameof(SaveBranchToBranchFuel), nameof(CompleteBranchToBranchFuel), nameof(CancelBranchToBranchFuel), search));

        public IActionResult WarehouseToDisplay() => RedirectToAction(nameof(WarehouseToDisplayProduct));
        public IActionResult DisplayToWarehouse() => RedirectToAction(nameof(DisplayToWarehouseProduct));
        public IActionResult BranchProductTransfer() => RedirectToAction(nameof(BranchToBranchProduct));
        public IActionResult BranchFuelTransfer() => RedirectToAction(nameof(BranchToBranchFuel));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveWarehouseToDisplayProduct(StockTransferForm form, string? search) => await SaveStockTransfer("WarehouseToDisplayProduct", form, nameof(WarehouseToDisplayProduct), search);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDisplayToWarehouseProduct(StockTransferForm form, string? search) => await SaveProductTransferFifo("DisplayToWarehouseProduct", form, nameof(DisplayToWarehouseProduct), search);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveBranchToBranchProduct(StockTransferForm form, string? search) => await SaveProductTransferFifo("BranchToBranchProduct", form, nameof(BranchToBranchProduct), search);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveBranchToBranchFuel(StockTransferForm form, string? search) => await SaveStockTransfer("BranchToBranchFuel", form, nameof(BranchToBranchFuel), search);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteWarehouseToDisplayProduct(int id, string? search) => await CompleteStockTransfer(id, nameof(WarehouseToDisplayProduct), search);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteDisplayToWarehouseProduct(int id, string? search) => await CompleteStockTransfer(id, nameof(DisplayToWarehouseProduct), search);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteBranchToBranchProduct(int id, string? search) => await CompleteStockTransfer(id, nameof(BranchToBranchProduct), search);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteBranchToBranchFuel(int id, string? search) => await CompleteStockTransfer(id, nameof(BranchToBranchFuel), search);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelWarehouseToDisplayProduct(int id, string? search) => await CancelStockTransfer(id, nameof(WarehouseToDisplayProduct), search);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelDisplayToWarehouseProduct(int id, string? search) => await CancelStockTransfer(id, nameof(DisplayToWarehouseProduct), search);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBranchToBranchProduct(int id, string? search) => await CancelStockTransfer(id, nameof(BranchToBranchProduct), search);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBranchToBranchFuel(int id, string? search) => await CancelStockTransfer(id, nameof(BranchToBranchFuel), search);
        public IActionResult ProductReturn() => View();
        public IActionResult FuelReturn() => View();
        public IActionResult VoidSale() => View();
        public IActionResult VoidProduct() => View();
        public IActionResult VoidFuel() => View();
        public async Task<IActionResult> DisplayStockAdjustment(string? search, int? branchId, DateTime? dateFrom, DateTime? dateTo, string? status, int? editId, int? detailsId)
            => View(await BuildStockAdjustmentPageAsync("Display", search, branchId, dateFrom, dateTo, status, editId, detailsId));
        public async Task<IActionResult> WarehouseStockAdjustment(string? search, int? branchId, DateTime? dateFrom, DateTime? dateTo, string? status, int? editId, int? detailsId)
            => View(await BuildStockAdjustmentPageAsync("Warehouse", search, branchId, dateFrom, dateTo, status, editId, detailsId));
        public async Task<IActionResult> TankFuelAdjustment(string? search, int? branchId, DateTime? dateFrom, DateTime? dateTo, string? status, int? editId, int? detailsId)
            => View(await BuildStockAdjustmentPageAsync("Fuel", search, branchId, dateFrom, dateTo, status, editId, detailsId));

        [HttpGet]
        public async Task<IActionResult> SearchStockAdjustmentTargets(string scope, int branchId, string? search)
        {
            if (branchId <= 0) return BadRequest(new { message = "Please select a Branch first." });
            var term = (search ?? string.Empty).Trim();
            scope = NormalizeAdjustmentScope(scope);

            if (scope == "Warehouse")
            {
                return Json(await _db.WarehouseStocks.AsNoTracking()
                    .Where(x => x.BranchId == branchId && x.Batch != null && x.Batch.ProductId == x.ProductId
                        && (term == "" || (x.Product != null && x.Product.Name.Contains(term)) || x.Batch.BatchNo.Contains(term)))
                    .OrderBy(x => x.Product != null ? x.Product.Name : string.Empty).ThenBy(x => x.Batch!.BatchNo)
                    .Take(100)
                    .Select(x => new
                    {
                        id = x.Id, branchId = x.BranchId, productId = x.ProductId,
                        productName = x.Product != null ? x.Product.Name : string.Empty,
                        batchId = x.BatchId, batchNumber = x.Batch!.BatchNo, currentQuantity = x.Quantity,
                        price = x.Batch.CostPrice, expiryDate = x.Batch.ExpiryDate,
                        status = x.Batch.IsActive && x.Batch.Status == 1 ? "Active" : "Inactive"
                    }).ToListAsync());
            }

            if (scope == "Display")
            {
                return Json(await _db.DisplayStocks.AsNoTracking()
                    .Where(x => x.BranchId == branchId && x.Batch != null && x.Batch.ProductId == x.ProductId
                        && (term == "" || (x.Product != null && x.Product.Name.Contains(term)) || x.Batch.BatchNo.Contains(term)))
                    .OrderBy(x => x.Product != null ? x.Product.Name : string.Empty).ThenBy(x => x.Batch!.BatchNo)
                    .Take(100)
                    .Select(x => new
                    {
                        id = x.Id, branchId = x.BranchId, productId = x.ProductId,
                        productName = x.Product != null ? x.Product.Name : string.Empty,
                        batchId = x.BatchId, batchNumber = x.Batch!.BatchNo, currentQuantity = x.Quantity,
                        price = x.Batch.SellingPrice, expiryDate = x.Batch.ExpiryDate,
                        status = x.Batch.IsActive && x.Batch.Status == 1 ? "Active" : "Inactive"
                    }).ToListAsync());
            }

            return Json(await _db.Tanks.AsNoTracking()
                .Where(x => x.BranchId == branchId && x.IsActive && x.Status == 1
                    && (term == "" || x.TankNo.Contains(term) || (x.Fuel != null && x.Fuel.Name.Contains(term))))
                .OrderBy(x => x.TankNo).Take(100)
                .Select(x => new
                {
                    id = x.Id, branchId = x.BranchId, tankName = x.TankNo, fuelId = x.FuelId,
                    fuelName = x.Fuel != null ? x.Fuel.Name : string.Empty,
                    currentQuantity = x.CurrentLiters, capacity = x.CapacityLiters,
                    availableSpace = x.CapacityLiters - x.CurrentLiters, status = "Active"
                }).ToListAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveStockAdjustment([Bind(Prefix = "Form")] StockAdjustmentForm form)
        {
            var scope = NormalizeAdjustmentScope(form.Scope);
            ValidateAdjustmentTarget(form, scope);
            var userId = CurrentUserId();
            if (!userId.HasValue) ModelState.AddModelError(string.Empty, "Please sign in before saving an adjustment.");
            var target = await LoadAdjustmentTargetAsync(form, scope, false);
            if (target is null) ModelState.AddModelError(string.Empty, "Select a valid stock item for the selected branch.");
            if (!ModelState.IsValid) return View(AdjustmentViewName(scope), await BuildStockAdjustmentPageAsync(scope, null, form.BranchId, null, null, null, form.Id, null, form));

            var adjustment = form.Id > 0 ? await _db.StockAdjustments.FirstOrDefaultAsync(x => x.Id == form.Id && x.Scope == scope) : null;
            if (form.Id > 0 && (adjustment is null || adjustment.Status != "Draft")) return AdjustmentRedirect(scope, "Only draft adjustments can be edited.");
            var now = DateTime.Now;
            adjustment ??= new StockAdjustment { AdjustmentNo = await GenerateAdjustmentNoAsync(now), Scope = scope, AdjustedBy = userId!.Value, CreatedAt = now };
            PopulateAdjustment(adjustment, form, target!, scope, now);
            if (adjustment.Id == 0) _db.StockAdjustments.Add(adjustment);
            await _db.SaveChangesAsync();
            return AdjustmentRedirect(scope, "Adjustment draft saved.");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> PostStockAdjustment(int id)
        {
            var userId = CurrentUserId();
            var adjustment = await _db.StockAdjustments.FirstOrDefaultAsync(x => x.Id == id);
            if (!userId.HasValue || adjustment is null) return AdjustmentApplyResponse(adjustment?.Scope ?? "Warehouse", "Adjustment was not found.", false);
            if (adjustment.Status != "Draft") return AdjustmentApplyResponse(adjustment.Scope, "Only draft adjustments can be applied.", false);
            await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var target = await LoadAdjustmentTargetAsync(adjustment, true) ?? throw new InvalidOperationException("The stock item no longer exists.");
                ApplyPostedAdjustment(adjustment, target, userId.Value, DateTime.Now);
                if (adjustment.ReversalOfAdjustmentId.HasValue)
                {
                    var original = await _db.StockAdjustments.FirstOrDefaultAsync(x => x.Id == adjustment.ReversalOfAdjustmentId.Value && x.Status == "Posted");
                    if (original is null || original.ReversedByAdjustmentId.HasValue) throw new InvalidOperationException("The original adjustment cannot be reversed.");
                    original.Status = "Reversed"; original.ReversedByAdjustmentId = adjustment.Id; original.UpdatedAt = DateTime.Now;
                }
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return AdjustmentApplyResponse(adjustment.Scope, "Adjustment applied successfully.", true);
            }
            catch (Exception ex) { await transaction.RollbackAsync(); return AdjustmentApplyResponse(adjustment.Scope, $"Applying adjustment failed: {ex.Message}", false); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelStockAdjustment(int id)
        {
            var adjustment = await _db.StockAdjustments.FirstOrDefaultAsync(x => x.Id == id);
            var userId = CurrentUserId();
            if (adjustment is null || !userId.HasValue) return AdjustmentRedirect(adjustment?.Scope ?? "Warehouse", "Adjustment was not found.");
            if (adjustment.Status != "Draft") return AdjustmentRedirect(adjustment.Scope, "Only draft adjustments can be cancelled.");
            adjustment.Status = "Cancelled"; adjustment.CancelledBy = userId; adjustment.CancelledAt = DateTime.Now; adjustment.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return AdjustmentRedirect(adjustment.Scope, "Adjustment cancelled.");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ReverseStockAdjustment(int id)
        {
            var original = await _db.StockAdjustments.FirstOrDefaultAsync(x => x.Id == id);
            var userId = CurrentUserId();
            if (original is null || !userId.HasValue) return AdjustmentRedirect(original?.Scope ?? "Warehouse", "Adjustment was not found.");
            if (original.Status != "Posted" || original.ReversedByAdjustmentId.HasValue) return AdjustmentRedirect(original.Scope, "Only an unreversed applied adjustment can be reversed.");
            var now = DateTime.Now;
            var reversal = new StockAdjustment
            {
                AdjustmentNo = await GenerateAdjustmentNoAsync(now), Scope = original.Scope, BusinessDate = DateTime.Today,
                BranchId = original.BranchId, WarehouseStockId = original.WarehouseStockId, DisplayStockId = original.DisplayStockId, TankId = original.TankId,
                ProductId = original.ProductId, BatchId = original.BatchId, FuelId = original.FuelId,
                AdjustmentType = original.SignedQuantity < 0 ? "Increase" : "Decrease", AdjustmentQuantity = original.AdjustmentQuantity,
                Reason = $"Reversal of {original.AdjustmentNo}", Remarks = $"Reversal of {original.AdjustmentNo}. {original.Remarks}".Trim(),
                Status = "Draft", AdjustedBy = userId.Value, CreatedAt = now, ReversalOfAdjustmentId = original.Id
            };
            _db.StockAdjustments.Add(reversal);
            await _db.SaveChangesAsync();
            return AdjustmentRedirect(original.Scope, $"Reversal {reversal.AdjustmentNo} created as Draft. Review and apply it to update stock.");
        }
        public async Task<IActionResult> ProductPriceAdjustment(string? search, int? filterBranchId, DateTime? dateFrom, DateTime? dateTo)
        {
            return View(await BuildProductPriceAdjustmentPageAsync(search, filterBranchId, dateFrom, dateTo));
        }

        [HttpGet]
        public async Task<IActionResult> SearchPriceAdjustmentFuels(int branchId, string? search, int take = 20)
        {
            if (branchId <= 0) return BadRequest(new { message = "Please select a Branch first." });
            var term = (search ?? string.Empty).Trim();
            return Json(await _db.Fuels.AsNoTracking()
                .Where(item => item.Status == 1 && item.IsActive && (term == "" || item.Name.Contains(term)))
                .OrderBy(item => item.Name)
                .Take(Math.Clamp(take, 1, 50))
                .Select(item => new
                {
                    id = item.Id,
                    name = item.Name,
                    price = item.BranchPrices
                        .Where(price => price.BranchId == branchId && price.Status == 1)
                        .Select(price => (decimal?)price.CurrentPricePerLiter)
                        .FirstOrDefault() ?? item.CurrentPricePerLiter
                }).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> SearchProductPriceAdjustmentItems(int branchId, string? term, int take = 50)
        {
            if (branchId <= 0) return BadRequest(new { message = "Please select a Branch first." });
            var searchText = (term ?? string.Empty).Trim();
            var rows = await _db.DisplayStocks.AsNoTracking().Include(stock => stock.Product).Include(stock => stock.Batch)
                .Where(stock => stock.BranchId == branchId && stock.Product != null && stock.Batch != null && stock.Product.Status == 1 && stock.Product.IsActive && stock.Batch.Status == 1 && stock.Batch.IsActive
                    && (searchText == "" || stock.Product.Name.Contains(searchText) || stock.Batch.BatchNo.Contains(searchText)))
                .OrderBy(stock => stock.Product!.Name).ThenBy(stock => stock.Batch!.BatchNo).Take(Math.Clamp(take, 1, 50))
                .Select(stock => new
                {
                    displayStockId = stock.Id,
                    branchId = stock.BranchId,
                    productId = stock.ProductId,
                    productName = stock.Product!.Name,
                    batchId = stock.BatchId,
                    batchNumber = stock.Batch!.BatchNo,
                    currentSellingPrice = stock.Batch.SellingPrice,
                    availableQuantity = stock.Quantity,
                    status = "Active"
                }).ToListAsync();
            return Json(rows);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProductPriceAdjustment([Bind(Prefix = "ProductPriceHistoryForm")] ProductPriceHistoryForm form, string? search, int? filterBranchId, DateTime? dateFrom, DateTime? dateTo)
        {
            if (!ModelState.IsValid)
            {
                return View("ProductPriceAdjustment", await BuildProductPriceAdjustmentPageAsync(search, filterBranchId, dateFrom, dateTo, form, "productPriceAdjustmentModal"));
            }

            if (!await _db.Branches.AnyAsync(item => item.Id == form.BranchId && item.Status == 1))
            {
                ModelState.AddModelError("ProductPriceHistoryForm.BranchId", "Select an active branch.");
                return View("ProductPriceAdjustment", await BuildProductPriceAdjustmentPageAsync(search, filterBranchId, dateFrom, dateTo, form, "productPriceAdjustmentModal"));
            }

            var selectedStock = await _db.DisplayStocks.AsNoTracking()
                .Include(stock => stock.Batch)
                    .ThenInclude(batch => batch!.Product)
                .FirstOrDefaultAsync(stock => stock.BranchId == form.BranchId
                    && stock.BatchId == form.BatchId
                    && stock.Batch != null
                    && stock.Batch.Status == 1
                    && stock.Batch.IsActive
                    && stock.Batch.Product != null
                    && stock.Batch.Product.Status == 1
                    && stock.Batch.Product.IsActive);
            var selectedBatch = selectedStock?.Batch;
            if (selectedBatch is null)
            {
                ModelState.AddModelError("ProductPriceHistoryForm.BatchId", "The selected batch is not in Display Stock for this Branch.");
                return View("ProductPriceAdjustment", await BuildProductPriceAdjustmentPageAsync(search, filterBranchId, dateFrom, dateTo, form, "productPriceAdjustmentModal"));
            }
            if (form.ProductId != selectedBatch.ProductId)
            {
                ModelState.AddModelError("ProductPriceHistoryForm.ProductId", "The selected Product does not match the selected Batch.");
                return View("ProductPriceAdjustment", await BuildProductPriceAdjustmentPageAsync(search, filterBranchId, dateFrom, dateTo, form, "productPriceAdjustmentModal"));
            }

            var batch = await _db.ProductBatches.FirstOrDefaultAsync(item =>
                item.Id == selectedStock!.BatchId
                && item.ProductId == selectedStock.ProductId
                && item.ProductId == form.ProductId
                && item.Status == 1
                && item.IsActive);
            if (batch is null)
            {
                ModelState.AddModelError("ProductPriceHistoryForm.BatchId", "The selected active Product Batch could not be loaded.");
                return View("ProductPriceAdjustment", await BuildProductPriceAdjustmentPageAsync(search, filterBranchId, dateFrom, dateTo, form, "productPriceAdjustmentModal"));
            }

            var newPrice = form.NewPrice!.Value;
            if (newPrice == batch.SellingPrice)
            {
                ModelState.AddModelError("ProductPriceHistoryForm.NewPrice", "New Selling Price must be different from the current price.");
                form.CurrentPrice = batch.SellingPrice;
                return View("ProductPriceAdjustment", await BuildProductPriceAdjustmentPageAsync(search, filterBranchId, dateFrom, dateTo, form, "productPriceAdjustmentModal"));
            }

            var now = DateTime.UtcNow;
            var history = new ProductPriceHistory
            {
                ProductId = batch.ProductId,
                BranchId = form.BranchId,
                BatchId = batch.Id,
                OldPrice = batch.SellingPrice,
                NewPrice = newPrice,
                EffectiveDate = form.EffectiveDate!.Value,
                Reason = CleanOptional(form.Reason),
                Remarks = CleanOptional(form.Remarks),
                CreatedBy = CurrentUserId(),
                Status = 1,
                CreatedAt = now,
                UpdatedAt = now
            };

            batch.SellingPrice = newPrice;
            batch.UpdatedAt = now;
            _db.ProductPriceHistory.Add(history);
            await using var transaction = await _db.Database.BeginTransactionAsync();
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return RedirectToAction(nameof(ProductPriceAdjustment), new { search, filterBranchId, dateFrom, dateTo });
        }

        public async Task<IActionResult> FuelPriceAdjustment(string? search, int? filterBranchId, DateTime? dateFrom, DateTime? dateTo)
        {
            return View(await BuildFuelPriceAdjustmentPageAsync(search, filterBranchId, dateFrom, dateTo));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFuelPriceAdjustment([Bind(Prefix = "FuelPriceHistoryForm")] FuelPriceHistoryForm form, string? search, int? filterBranchId, DateTime? dateFrom, DateTime? dateTo)
        {
            if (!ModelState.IsValid)
            {
                return View("FuelPriceAdjustment", await BuildFuelPriceAdjustmentPageAsync(search, filterBranchId, dateFrom, dateTo, form, "fuelPriceAdjustmentModal"));
            }

            if (!await _db.Branches.AnyAsync(item => item.Id == form.BranchId && item.Status == 1))
            {
                ModelState.AddModelError("FuelPriceHistoryForm.BranchId", "Select an active branch.");
                return View("FuelPriceAdjustment", await BuildFuelPriceAdjustmentPageAsync(search, filterBranchId, dateFrom, dateTo, form, "fuelPriceAdjustmentModal"));
            }

            var fuel = await _db.Fuels.FindAsync(form.FuelId);
            if (fuel is null || fuel.Status != 1 || !fuel.IsActive)
            {
                ModelState.AddModelError("FuelPriceHistoryForm.FuelId", "Select a fuel.");
                return View("FuelPriceAdjustment", await BuildFuelPriceAdjustmentPageAsync(search, filterBranchId, dateFrom, dateTo, form, "fuelPriceAdjustmentModal"));
            }

            var now = DateTime.UtcNow;
            var branchPrice = await _db.BranchFuelPrices
                .FirstOrDefaultAsync(item => item.BranchId == form.BranchId && item.FuelId == form.FuelId);
            var oldPrice = branchPrice?.CurrentPricePerLiter ?? fuel.CurrentPricePerLiter;
            var history = new FuelPriceHistory
            {
                FuelId = form.FuelId,
                BranchId = form.BranchId,
                OldPrice = oldPrice,
                NewPrice = form.NewPrice!.Value,
                EffectiveAt = form.EffectiveAt!.Value,
                Reason = CleanOptional(form.Reason),
                Remarks = CleanOptional(form.Remarks),
                CreatedBy = CurrentUserId(),
                CreatedAt = now,
                UpdatedAt = now
            };

            if (branchPrice is null)
            {
                branchPrice = new BranchFuelPrice
                {
                    BranchId = form.BranchId,
                    FuelId = form.FuelId,
                    CurrentPricePerLiter = history.NewPrice,
                    EffectiveAt = history.EffectiveAt,
                    Status = 1,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                _db.BranchFuelPrices.Add(branchPrice);
            }
            else
            {
                branchPrice.CurrentPricePerLiter = history.NewPrice;
                branchPrice.EffectiveAt = history.EffectiveAt;
                branchPrice.Status = 1;
                branchPrice.UpdatedAt = now;
            }
            _db.FuelPriceHistory.Add(history);
            await using var transaction = await _db.Database.BeginTransactionAsync();
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return RedirectToAction(nameof(FuelPriceAdjustment), new { search, filterBranchId, dateFrom, dateTo });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFuelPriceAdjustment(int id, string? search)
        {
            var history = await _db.FuelPriceHistory.FindAsync(id);
            if (history is not null)
            {
                history.Status = 0;
                history.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(FuelPriceAdjustment), new { search });
        }

        private IQueryable<ProductSale> BuildProductSalesQuery(string? search, DateTime? dateFrom, DateTime? dateTo, string? status, int? branchId)
        {
            var query = _db.ProductSales
                .AsNoTracking()
                .Include(item => item.Sale)
                    .ThenInclude(sale => sale!.Branch)
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

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(item => item.Sale!.BranchId == branchId.Value);
            }

            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(item => item.Sale!.ReceiptNo.Contains(searchText)
                    || (item.Sale.Branch != null && item.Sale.Branch.Name.Contains(searchText))
                    || (item.Product != null && item.Product.Name.Contains(searchText))
                    || (item.Batch != null && item.Batch.BatchNo.Contains(searchText))
                    || (item.Sale.User != null && ((item.Sale.User.FullName != null && item.Sale.User.FullName.Contains(searchText)) || item.Sale.User.Username.Contains(searchText)))
                    || (item.Sale.Member != null && item.Sale.Member.FullName.Contains(searchText)));
            }

            return query;
        }

        private IQueryable<FuelSale> BuildFuelSalesQuery(string? search, DateTime? dateFrom, DateTime? dateTo, string? status, int? branchId)
        {
            var query = _db.FuelSales
                .AsNoTracking()
                .Include(item => item.Sale)
                    .ThenInclude(sale => sale!.Branch)
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

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(item => item.Sale!.BranchId == branchId.Value);
            }

            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(item => item.Sale!.ReceiptNo.Contains(searchText)
                    || (item.Sale.Branch != null && item.Sale.Branch.Name.Contains(searchText))
                    || (item.Fuel != null && item.Fuel.Name.Contains(searchText))
                    || (item.Tank != null && item.Tank.TankNo.Contains(searchText))
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
            var unitPrice = item.UnitPrice > 0m ? item.UnitPrice : item.Price;
            var unitCost = item.UnitCost > 0m ? item.UnitCost : item.Batch?.CostPrice ?? 0m;
            var grossProfit = item.GrossProfit != 0m ? item.GrossProfit : (unitPrice - unitCost) * item.Quantity;

            return new ProductSaleRowViewModel
            {
                SaleItemId = item.Id,
                SaleId = item.SaleId,
                ReceiptNo = sale?.ReceiptNo ?? "-",
                BranchName = sale?.Branch?.Name ?? "Unassigned",
                ProductName = item.Product?.Name ?? "-",
                BatchNo = item.Batch?.BatchNo ?? "-",
                Quantity = item.Quantity,
                Price = unitPrice,
                UnitCost = unitCost,
                UnitPrice = unitPrice,
                Subtotal = item.Subtotal,
                GrossProfit = grossProfit,
                CashierName = UserDisplayName(sale?.User),
                MemberName = sale?.Member?.FullName ?? "-",
                SaleDate = sale?.CreatedAt ?? item.CreatedAt,
                PaymentType = PaymentTypeFor(sale),
                Cost = unitCost * item.Quantity,
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
                BranchName = sale?.Branch?.Name ?? "Unassigned",
                FuelName = item.Fuel?.Name ?? "-",
                TankNo = item.Tank?.TankNo ?? "-",
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
                    item.UnitCost,
                    Cost = item.UnitCost > 0m ? item.UnitCost * item.Quantity : item.Batch == null ? 0m : item.Batch.CostPrice * item.Quantity
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

        private static string? CleanOptional(string? value)
        {
            var trimmed = (value ?? string.Empty).Trim();
            return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
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

        private async Task<ProductReceivingPageViewModel> BuildProductReceivingPage(string? search, int? branchId = null, ProductReceivingForm? form = null)
        {
            var searchText = (search ?? string.Empty).Trim();
            var query = _db.StockReceivings
                .AsNoTracking()
                .Include(item => item.Branch)
                .Include(item => item.Supplier)
                .Include(item => item.Items)
                    .ThenInclude(item => item.Product)
                .Include(item => item.Items)
                    .ThenInclude(item => item.ProductBatch)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(item =>
                    item.ReceivingNo.Contains(searchText)
                    || (item.Branch != null && item.Branch.Name.Contains(searchText))
                    || (item.Supplier != null && item.Supplier.Name.Contains(searchText))
                    || item.Items.Any(line => line.Product != null && line.Product.Name.Contains(searchText))
                    || item.Items.Any(line => line.ProductBatch != null && line.ProductBatch.BatchNo.Contains(searchText))
                    || (item.Remarks != null && item.Remarks.Contains(searchText)));
            }

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(item => item.BranchId == branchId.Value);
            }

            var receivings = await query.OrderByDescending(item => item.ReceivedDate).ThenByDescending(item => item.Id).Take(100).ToListAsync();
            var movementLookup = await _db.StockMovements
                .AsNoTracking()
                .Where(item => item.ReferenceType == "ProductReceiving" && item.ReferenceId.HasValue)
                .GroupBy(item => item.ReferenceId!.Value)
                .Select(group => new { ReceivingId = group.Key, UserId = group.OrderByDescending(item => item.Id).Select(item => item.CreatedBy).FirstOrDefault() })
                .ToDictionaryAsync(item => item.ReceivingId, item => item.UserId);
            var userIds = movementLookup.Values.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
            var userLookup = await _db.Users.AsNoTracking().Where(user => userIds.Contains(user.Id)).ToDictionaryAsync(user => user.Id, user => string.IsNullOrWhiteSpace(user.FullName) ? user.Username : user.FullName!);

            var receivingForm = form ?? new ProductReceivingForm();
            if (receivingForm.Id == 0 && receivingForm.BranchId <= 0 && branchId.HasValue)
            {
                receivingForm.BranchId = branchId.Value;
            }
            if (form is null && receivingForm.Id == 0 && !receivingForm.ReceivedDate.HasValue)
            {
                receivingForm.ReceivedDate = DateTime.Today;
            }

            return new ProductReceivingPageViewModel
            {
                Search = searchText,
                BranchId = branchId,
                FormBranchName = await BranchNameAsync(receivingForm.BranchId),
                FormSupplierName = await ProductReceivingSupplierNameAsync(receivingForm.SupplierId),
                FormProductName = await ProductReceivingProductNameAsync(receivingForm.ProductId),
                Form = receivingForm,
                BranchOptions = await BuildBranchFilterOptionsAsync(),
                SupplierOptions = await BuildSupplierOptionsAsync(),
                ProductOptions = await BuildProductOptionsAsync(),
                CategoryOptions = await BuildProductCategoryOptionsAsync(),
                UnitOptions = await BuildProductUnitOptionsAsync(),
                Receivings = receivings.Select(item =>
                {
                    var line = item.Items.FirstOrDefault();
                    var after = item.Status == 1 && line?.ProductBatchId is not null
                        ? _db.WarehouseStocks.AsNoTracking().Where(stock => stock.ProductId == line.ProductId && stock.BatchId == line.ProductBatchId.Value && stock.BranchId == item.BranchId).Select(stock => (decimal?)stock.Quantity).FirstOrDefault()
                        : null;
                    var before = after.HasValue && line is not null ? after.Value - line.Quantity : (decimal?)null;
                    var receivedBy = movementLookup.TryGetValue(item.Id, out var movementUserId) && movementUserId.HasValue && userLookup.TryGetValue(movementUserId.Value, out var userName) ? userName : "-";
                    return new ProductReceivingRowViewModel
                    {
                        Id = item.Id,
                        ReceivingNo = item.ReceivingNo,
                        BranchName = item.Branch?.Name ?? "Unassigned",
                        Supplier = item.Supplier?.Name ?? "-",
                        Product = line?.Product?.Name ?? "-",
                        Batch = line?.ProductBatch?.BatchNo ?? "-",
                        Quantity = line?.Quantity ?? 0m,
                        CostPrice = line?.CostPrice ?? 0m,
                        SellingPrice = line?.SellingPrice ?? 0m,
                        WarehouseStockBefore = before,
                        WarehouseStockAfter = after,
                        ReceivedBy = receivedBy,
                        Date = item.ReceivedDate,
                        Status = item.Status,
                        Remarks = item.Remarks ?? string.Empty
                    };
                }).ToList()
            };
        }

        private async Task<FuelReceivingPageViewModel> BuildFuelReceivingPage(string? search, int? branchId = null, FuelReceivingForm? form = null)
        {
            var searchText = (search ?? string.Empty).Trim();
            var query = _db.FuelDeliveries.AsNoTracking().Include(item => item.Branch).Include(item => item.Supplier).Include(item => item.Fuel).Include(item => item.Tank).ThenInclude(tank => tank!.Branch).AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(item =>
                    item.DeliveryNo.Contains(searchText)
                    || (item.Branch != null && item.Branch.Name.Contains(searchText))
                    || (item.Supplier != null && item.Supplier.Name.Contains(searchText))
                    || (item.Fuel != null && item.Fuel.Name.Contains(searchText))
                    || (item.Tank != null && item.Tank.TankNo.Contains(searchText))
                    || (item.Remarks != null && item.Remarks.Contains(searchText)));
            }

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(item => item.BranchId == branchId.Value);
            }

            var deliveries = await query.OrderByDescending(item => item.DeliveryDate).ThenByDescending(item => item.Id).Take(100).ToListAsync();
            var receivingForm = form ?? new FuelReceivingForm();
            if (receivingForm.Id == 0 && receivingForm.BranchId <= 0 && branchId.HasValue && branchId.Value > 0)
            {
                receivingForm.BranchId = branchId.Value;
            }
            if (form is null && receivingForm.Id == 0 && !receivingForm.ReceivedDate.HasValue)
            {
                receivingForm.ReceivedDate = DateTime.Today;
            }

            return new FuelReceivingPageViewModel
            {
                Search = searchText,
                BranchId = branchId,
                FormBranchName = await BranchNameAsync(receivingForm.BranchId),
                FormTankName = await FuelReceivingTankNameAsync(receivingForm.TankId),
                FormFuelName = await FuelReceivingTankFuelNameAsync(receivingForm.TankId),
                FormSupplierName = await ProductReceivingSupplierNameAsync(receivingForm.SupplierId),
                Form = receivingForm,
                BranchOptions = await BuildBranchFilterOptionsAsync(),
                SupplierOptions = await BuildSupplierOptionsAsync(),
                FuelOptions = await BuildFuelOptionsAsync(),
                TankOptions = await BuildTankOptionsAsync(receivingForm.BranchId),
                Receivings = deliveries.Select(item =>
                {
                    var after = item.Status == 1 && item.Tank is not null ? (decimal?)item.Tank.CurrentLiters : null;
                    var before = after.HasValue ? after.Value - item.DeliveredLiters : (decimal?)null;
                    return new FuelReceivingRowViewModel
                    {
                        Id = item.Id,
                        ReceivingNo = item.DeliveryNo,
                        BranchName = item.Branch?.Name ?? item.Tank?.Branch?.Name ?? "Unassigned",
                        Supplier = item.Supplier?.Name ?? "-",
                        Fuel = item.Fuel?.Name ?? "-",
                        Tank = item.Tank?.TankNo ?? "-",
                        Liters = item.DeliveredLiters,
                        CostPerLiter = item.CostPerLiter ?? 0m,
                        TotalCost = item.TotalCost ?? decimal.Round(item.DeliveredLiters * (item.CostPerLiter ?? 0m), 2, MidpointRounding.AwayFromZero),
                        CreatedAt = item.CreatedAt,
                        TankLitersBefore = before,
                        TankLitersAfter = after,
                        Date = item.DeliveryDate,
                        Status = item.Status,
                        Remarks = item.Remarks ?? string.Empty
                    };
                }).ToList()
            };
        }

        private async Task<SetupModulesPageViewModel> BuildProductPriceAdjustmentPageAsync(string? search, int? filterBranchId, DateTime? dateFrom, DateTime? dateTo, ProductPriceHistoryForm? form = null, string activeModalId = "")
        {
            var branchOptions = await BuildCashBranchOptionsAsync("All Branches");
            branchOptions.ForEach(option => option.Selected = option.Value == filterBranchId?.ToString());
            IQueryable<ProductPriceHistory> query = _db.ProductPriceHistory
                .AsNoTracking()
                .Include(history => history.Product)
                .Include(history => history.Batch)
                .Include(history => history.Branch);
            var searchText = (search ?? string.Empty).Trim();
            if (filterBranchId.HasValue) query = query.Where(history => history.BranchId == filterBranchId.Value);
            if (dateFrom.HasValue) query = query.Where(history => history.CreatedAt >= dateFrom.Value.Date);
            if (dateTo.HasValue) query = query.Where(history => history.CreatedAt < dateTo.Value.Date.AddDays(1));

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var createdByIds = await _db.Users
                    .AsNoTracking()
                    .Where(user => user.Username.Contains(searchText) || (user.FullName != null && user.FullName.Contains(searchText)))
                    .Select(user => user.Id)
                    .ToListAsync();
                var hasPriceSearch = decimal.TryParse(searchText, out var priceSearch);

                query = query.Where(history =>
                    (history.Product != null && history.Product.Name.Contains(searchText))
                    || (history.Batch != null && history.Batch.BatchNo.Contains(searchText))
                    || (history.Branch != null && history.Branch.Name.Contains(searchText))
                    || (history.Reason != null && history.Reason.Contains(searchText))
                    || (hasPriceSearch && (history.OldPrice == priceSearch || history.NewPrice == priceSearch))
                    || (history.CreatedBy.HasValue && createdByIds.Contains(history.CreatedBy.Value)));
            }

            var histories = await query
                .OrderByDescending(history => history.EffectiveDate)
                .ThenByDescending(history => history.CreatedAt)
                .ThenByDescending(history => history.Id)
                .ToListAsync();
            var createdByUserIds = histories
                .Where(history => history.CreatedBy.HasValue)
                .Select(history => history.CreatedBy!.Value)
                .Distinct()
                .ToList();
            var users = await _db.Users.AsNoTracking()
                .Where(user => createdByUserIds.Contains(user.Id))
                .ToDictionaryAsync(user => user.Id, user => string.IsNullOrWhiteSpace(user.FullName) ? user.Username : user.FullName!);
            var currentHistoryIds = await _db.ProductPriceHistory.AsNoTracking()
                .Where(history => history.Status == 1)
                .GroupBy(history => history.ProductId)
                .Select(group => group
                    .OrderByDescending(history => history.EffectiveDate)
                    .ThenByDescending(history => history.CreatedAt)
                    .ThenByDescending(history => history.Id)
                    .Select(history => history.Id)
                    .First())
                .ToListAsync();

            return new SetupModulesPageViewModel
            {
                Search = searchText,
                BranchId = filterBranchId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                ActiveModalId = activeModalId,
                ProductPriceHistoryForm = form ?? new ProductPriceHistoryForm(),
                ProductOptions = await BuildProductOptionsAsync(),
                ProductBatches = await BuildActiveProductPriceBatchesAsync(),
                BranchOptions = branchOptions,
                ProductPriceHistory = histories,
                ProductPriceHistoryCreatedBy = users,
                CurrentProductPriceHistoryIds = currentHistoryIds.ToHashSet()
            };
        }

        private async Task<ProductBatch?> ResolveProductPriceBatchAsync(int productId, int? batchId)
        {
            var query = _db.ProductBatches
                .Where(batch => batch.ProductId == productId && batch.Status == 1 && batch.IsActive);

            if (batchId.HasValue && batchId.Value > 0)
            {
                return await query.FirstOrDefaultAsync(batch => batch.Id == batchId.Value);
            }

            return await query
                .OrderByDescending(batch => batch.CreatedAt ?? DateTime.MinValue)
                .ThenByDescending(batch => batch.Id)
                .FirstOrDefaultAsync();
        }

        private async Task<List<ProductBatch>> BuildActiveProductPriceBatchesAsync()
        {
            return await _db.ProductBatches
                .AsNoTracking()
                .Include(batch => batch.Product)
                .Where(batch => batch.Status == 1 && batch.IsActive && batch.Product != null && batch.Product.Status == 1 && batch.Product.IsActive)
                .OrderBy(batch => batch.Product!.Name)
                .ThenByDescending(batch => batch.CreatedAt ?? DateTime.MinValue)
                .ThenByDescending(batch => batch.Id)
                .ToListAsync();
        }

        private async Task<SetupModulesPageViewModel> BuildFuelPriceAdjustmentPageAsync(string? search, int? filterBranchId, DateTime? dateFrom, DateTime? dateTo, FuelPriceHistoryForm? form = null, string activeModalId = "")
        {
            var branchOptions = await BuildCashBranchOptionsAsync("All Branches");
            branchOptions.ForEach(option => option.Selected = option.Value == filterBranchId?.ToString());
            IQueryable<FuelPriceHistory> query = _db.FuelPriceHistory.AsNoTracking().Include(history => history.Fuel).Include(history => history.Branch);
            var searchText = (search ?? string.Empty).Trim();
            if (filterBranchId.HasValue) query = query.Where(history => history.BranchId == filterBranchId.Value);
            if (dateFrom.HasValue) query = query.Where(history => history.CreatedAt >= dateFrom.Value.Date);
            if (dateTo.HasValue) query = query.Where(history => history.CreatedAt < dateTo.Value.Date.AddDays(1));

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var createdByIdsForSearch = await _db.Users.AsNoTracking().Where(user => user.Username.Contains(searchText) || (user.FullName != null && user.FullName.Contains(searchText))).Select(user => user.Id).ToListAsync();
                query = query.Where(history => (history.Fuel != null && history.Fuel.Name.Contains(searchText))
                    || (history.Branch != null && history.Branch.Name.Contains(searchText))
                    || (history.Reason != null && history.Reason.Contains(searchText))
                    || (history.CreatedBy.HasValue && createdByIdsForSearch.Contains(history.CreatedBy.Value)));
            }

            var histories = await query
                .OrderByDescending(history => history.EffectiveAt)
                .ThenByDescending(history => history.CreatedAt)
                .ThenByDescending(history => history.Id)
                .ToListAsync();
            var createdByIds = histories
                .Where(history => history.CreatedBy.HasValue)
                .Select(history => history.CreatedBy!.Value)
                .Distinct()
                .ToList();
            var users = await _db.Users.AsNoTracking()
                .Where(user => createdByIds.Contains(user.Id))
                .ToDictionaryAsync(user => user.Id, user => string.IsNullOrWhiteSpace(user.FullName) ? user.Username : user.FullName!);
            var currentHistoryIds = await _db.FuelPriceHistory.AsNoTracking()
                .GroupBy(history => history.FuelId)
                .Select(group => group
                    .OrderByDescending(history => history.EffectiveAt)
                    .ThenByDescending(history => history.CreatedAt)
                    .ThenByDescending(history => history.Id)
                    .Select(history => history.Id)
                    .First())
                .ToListAsync();

            return new SetupModulesPageViewModel
            {
                Search = searchText,
                BranchId = filterBranchId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                ActiveModalId = activeModalId,
                FuelPriceHistoryForm = form ?? new FuelPriceHistoryForm(),
                FuelOptions = await BuildFuelOptionsAsync(),
                BranchOptions = branchOptions,
                FuelPriceHistory = histories,
                FuelPriceHistoryCreatedBy = users,
                CurrentFuelPriceHistoryIds = currentHistoryIds.ToHashSet()
            };
        }

        private async Task<string> FuelReceivingTankNameAsync(int tankId)
        {
            if (tankId <= 0)
            {
                return string.Empty;
            }

            return await _db.Tanks.AsNoTracking()
                .Where(tank => tank.Id == tankId)
                .Select(tank => tank.TankNo)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        private async Task<string> FuelReceivingTankFuelNameAsync(int tankId)
        {
            if (tankId <= 0)
            {
                return string.Empty;
            }

            return await _db.Tanks.AsNoTracking()
                .Include(tank => tank.Fuel)
                .Where(tank => tank.Id == tankId)
                .Select(tank => tank.Fuel != null ? tank.Fuel.Name : string.Empty)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        private async Task<string> ProductReceivingSupplierNameAsync(int supplierId)
        {
            if (supplierId <= 0)
            {
                return string.Empty;
            }

            return await _db.Suppliers.AsNoTracking()
                .Where(supplier => supplier.Id == supplierId)
                .Select(supplier => supplier.Name)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        private async Task<string> ProductReceivingProductNameAsync(int productId)
        {
            if (productId <= 0)
            {
                return string.Empty;
            }

            return await _db.Products.AsNoTracking()
                .Where(product => product.Id == productId)
                .Select(product => product.Name)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        private async Task<ProductBatch> CreateGeneratedProductBatch(ProductReceivingForm form, DateTime now)
        {
            var batch = new ProductBatch
            {
                ProductId = form.ProductId,
                SupplierId = form.SupplierId,
                BatchNo = await _batchNumberService.GenerateNextBatchNoAsync(),
                CostPrice = form.CostPrice,
                SellingPrice = form.SellingPrice,
                ExpiryDate = form.ExpiryDate,
                Status = 1,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };
            _db.ProductBatches.Add(batch);
            await _db.SaveChangesAsync();
            return batch;
        }

        private async Task CreateFuelBatchForDelivery(FuelDelivery delivery, Tank tank, DateTime now)
        {
            if (delivery.Id > 0 && await _db.FuelBatches.AnyAsync(batch => batch.FuelDeliveryId == delivery.Id))
            {
                return;
            }

            _db.FuelBatches.Add(new FuelBatch
            {
                FuelId = delivery.FuelId,
                SupplierId = delivery.SupplierId,
                TankId = delivery.TankId,
                BranchId = tank.BranchId,
                FuelDeliveryId = delivery.Id,
                BatchNo = await GenerateNextFuelBatchNoAsync(),
                CostPricePerLiter = delivery.CostPerLiter ?? 0m,
                ReceivedLiters = delivery.DeliveredLiters,
                RemainingLiters = delivery.DeliveredLiters,
                ReceivedDate = delivery.DeliveryDate,
                Remarks = delivery.Remarks,
                Status = 1,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        private async Task<decimal> ResolveFuelPriceAsync(int? branchId, int fuelId, decimal legacyFallback)
        {
            if (!branchId.HasValue)
            {
                return legacyFallback;
            }

            return await _db.BranchFuelPrices
                .AsNoTracking()
                .Where(price => price.BranchId == branchId.Value && price.FuelId == fuelId && price.Status == 1)
                .Select(price => (decimal?)price.CurrentPricePerLiter)
                .FirstOrDefaultAsync() ?? legacyFallback;
        }

        private async Task<string> GenerateNextFuelBatchNoAsync()
        {
            var batchNumbers = await _db.FuelBatches
                .AsNoTracking()
                .Select(batch => batch.BatchNo)
                .ToListAsync();
            var usedNumbers = batchNumbers
                .Where(batchNo => !string.IsNullOrWhiteSpace(batchNo) && batchNo.All(char.IsDigit))
                .Select(batchNo => int.TryParse(batchNo, out var value) ? value : 0)
                .Where(value => value > 0)
                .ToHashSet();
            var next = usedNumbers.Count == 0 ? 1 : usedNumbers.Max() + 1;

            while (usedNumbers.Contains(next))
            {
                next += 1;
            }

            return next.ToString("D8");
        }

        private async Task<Product> ResolveReceivingProduct(ProductReceivingForm form, DateTime now)
        {
            if (!IsNewProductReceivingMode(form))
            {
                var existing = await _db.Products.FirstOrDefaultAsync(product => product.Id == form.ProductId && product.Status == 1 && product.IsActive);
                if (existing is null)
                {
                    throw new InvalidOperationException("Product is required.");
                }

                return existing;
            }

            var productName = (form.NewProductName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new InvalidOperationException("Product Name is required.");
            }

            var duplicateExists = await _db.Products
                .AsNoTracking()
                .AnyAsync(product => product.Status == 1 && product.IsActive && product.Name == productName);
            if (duplicateExists)
            {
                throw new InvalidOperationException("Product already exists. Select it from Existing Product instead.");
            }

            var product = new Product
            {
                Name = productName,
                CategoryId = form.NewProductCategoryId > 0 ? form.NewProductCategoryId : null,
                ProductUnitId = form.NewProductUnitId > 0 ? form.NewProductUnitId : null,
                Status = 1,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return product;
        }

        private void ValidateProductReceivingForm(ProductReceivingForm form)
        {
            if (form.BranchId <= 0)
            {
                ModelState.AddModelError("Form.BranchId", "Branch is required.");
            }

            if (form.SupplierId <= 0)
            {
                ModelState.AddModelError("Form.SupplierId", "Supplier is required.");
            }

            if (IsNewProductReceivingMode(form))
            {
                if (string.IsNullOrWhiteSpace(form.NewProductName))
                {
                    ModelState.AddModelError("Form.NewProductName", "Product Name is required.");
                }
            }
            else if (form.ProductId <= 0)
            {
                ModelState.AddModelError("Form.ProductId", "Product is required.");
            }

            if (form.Quantity <= 0)
            {
                ModelState.AddModelError("Form.Quantity", "Quantity must be greater than 0.");
            }

            if (form.CostPrice < 0)
            {
                ModelState.AddModelError("Form.CostPrice", "Cost Price must be greater than or equal to 0.");
            }

            if (form.SellingPrice < 0)
            {
                ModelState.AddModelError("Form.SellingPrice", "Selling Price must be greater than or equal to 0.");
            }

            if (!form.ReceivedDate.HasValue)
            {
                ModelState.AddModelError("Form.ReceivedDate", "Date is required.");
            }
        }

        private async Task ValidateProductReceivingReferencesAsync(ProductReceivingForm form)
        {
            if (form.BranchId > 0 && !await _db.Branches.AsNoTracking().AnyAsync(branch => branch.Id == form.BranchId && branch.Status == 1))
            {
                ModelState.AddModelError("Form.BranchId", "Branch is required.");
            }

            if (form.SupplierId > 0 && !await _db.Suppliers.AsNoTracking().AnyAsync(supplier => supplier.Id == form.SupplierId && supplier.Status == 1))
            {
                ModelState.AddModelError("Form.SupplierId", "Supplier is required.");
            }

            if (!IsNewProductReceivingMode(form)
                && form.ProductId > 0
                && !await _db.Products.AsNoTracking().AnyAsync(product => product.Id == form.ProductId && product.Status == 1 && product.IsActive))
            {
                ModelState.AddModelError("Form.ProductId", "Product is required.");
            }

            if (IsNewProductReceivingMode(form))
            {
                var productName = (form.NewProductName ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(productName)
                    && await _db.Products.AsNoTracking().AnyAsync(product => product.Status == 1 && product.IsActive && product.Name == productName))
                {
                    ModelState.AddModelError("Form.NewProductName", "Product already exists. Select it from Existing Product instead.");
                }
            }
        }

        private static bool IsNewProductReceivingMode(ProductReceivingForm form)
        {
            return string.Equals(form.ProductMode, "New", StringComparison.OrdinalIgnoreCase)
                || string.Equals(form.ProductMode, "NewProduct", StringComparison.OrdinalIgnoreCase);
        }

        private async Task CompleteProductReceiving(StockReceiving receiving, StockReceivingItem item, int userId, DateTime now)
        {
            if (!item.ProductBatchId.HasValue)
            {
                throw new InvalidOperationException("Receiving item must have a linked batch before completion.");
            }

            var alreadyCompleted = await _db.StockMovements.AnyAsync(movement => movement.ReferenceType == "ProductReceiving" && movement.ReferenceId == receiving.Id);
            if (alreadyCompleted)
            {
                return;
            }

            var stock = await _db.WarehouseStocks.FirstOrDefaultAsync(row => row.ProductId == item.ProductId && row.BatchId == item.ProductBatchId.Value && row.BranchId == receiving.BranchId);
            if (stock is null)
            {
                stock = new WarehouseStock
                {
                    BranchId = receiving.BranchId,
                    ProductId = item.ProductId,
                    BatchId = item.ProductBatchId.Value,
                    Quantity = 0m,
                    CreatedAt = now
                };
                _db.WarehouseStocks.Add(stock);
            }

            stock.Quantity += item.Quantity;
            stock.UpdatedAt = now;
            _db.StockMovements.Add(new StockMovement
            {
                ProductId = item.ProductId,
                ProductBatchId = item.ProductBatchId,
                SourceLocation = "Supplier",
                DestinationLocation = "Warehouse",
                MovementType = "Receiving",
                Quantity = item.Quantity,
                ReferenceType = "ProductReceiving",
                ReferenceId = receiving.Id,
                CreatedBy = userId,
                Remarks = receiving.Remarks,
                CreatedAt = now
            });
        }

        private async Task<List<SelectListItem>> BuildSupplierOptionsAsync()
        {
            var options = await _db.Suppliers.AsNoTracking().Where(item => item.Status == 1).OrderBy(item => item.Name).Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Name }).ToListAsync();
            options.Insert(0, new SelectListItem { Value = "0", Text = "Select supplier" });
            return options;
        }

        private async Task<List<SelectListItem>> BuildProductOptionsAsync()
        {
            var options = await _db.Products.AsNoTracking().Where(item => item.Status == 1 && item.IsActive).OrderBy(item => item.Name).Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Name }).ToListAsync();
            options.Insert(0, new SelectListItem { Value = "0", Text = "Select product" });
            return options;
        }

        private async Task<List<SelectListItem>> BuildProductCategoryOptionsAsync()
        {
            var options = await _db.ProductCategories.AsNoTracking().Where(item => item.Status == 1 && item.IsActive).OrderBy(item => item.Name).Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Name }).ToListAsync();
            options.Insert(0, new SelectListItem { Value = "", Text = "Select category" });
            return options;
        }

        private async Task<List<SelectListItem>> BuildProductUnitOptionsAsync()
        {
            var options = await _db.ProductUnits.AsNoTracking().Where(item => item.Status == 1).OrderBy(item => item.Name).Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Name }).ToListAsync();
            options.Insert(0, new SelectListItem { Value = "", Text = "Select unit" });
            return options;
        }

        private async Task<List<SelectListItem>> BuildFuelOptionsAsync()
        {
            var options = await _db.Fuels.AsNoTracking().Where(item => item.Status == 1 && item.IsActive).OrderBy(item => item.Name).Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Name }).ToListAsync();
            options.Insert(0, new SelectListItem { Value = "0", Text = "Select fuel" });
            return options;
        }

        private async Task<List<SelectListItem>> BuildTankOptionsAsync(int? branchId = null)
        {
            var query = _db.Tanks.AsNoTracking().Include(item => item.Fuel).Where(item => item.Status == 1 && item.IsActive);
            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(item => item.BranchId == branchId.Value);
            }

            var options = await query.OrderBy(item => item.TankNo).Select(item => new SelectListItem { Value = item.Id.ToString(), Text = $"{item.TankNo} ({item.Fuel!.Name})" }).ToListAsync();
            options.Insert(0, new SelectListItem { Value = "0", Text = "Select tank" });
            return options;
        }

        private async Task<string> GenerateProductReceivingNoAsync()
        {
            return $"PR-{DateTime.Now:yyyyMMdd}-{await _db.StockReceivings.CountAsync() + 1:0000}";
        }

        private async Task<string> GenerateFuelReceivingNoAsync()
        {
            return $"FR-{DateTime.Now:yyyyMMdd}-{await _db.FuelDeliveries.CountAsync() + 1:0000}";
        }

        private async Task<StockTransferPageViewModel> BuildStockTransferPage(string transferType, string title, string saveAction, string completeAction, string cancelAction, string? search, int? sourceBranchId = null, int? destinationBranchId = null)
        {
            var searchText = (search ?? string.Empty).Trim();
            var query = _db.StockTransfers
                .AsNoTracking()
                .Include(item => item.SourceBranch)
                .Include(item => item.DestinationBranch)
                .Include(item => item.User)
                .Include(item => item.Items)
                    .ThenInclude(item => item.Product)
                .Include(item => item.Items)
                    .ThenInclude(item => item.Batch)
                .Include(item => item.Items)
                    .ThenInclude(item => item.Fuel)
                .Include(item => item.Items)
                    .ThenInclude(item => item.SourceTank)
                .Include(item => item.Items)
                    .ThenInclude(item => item.DestinationTank)
                .Where(item => item.TransferType == transferType);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(item =>
                    item.TransferNo.Contains(searchText)
                    || item.Status.Contains(searchText)
                    || (item.SourceBranch != null && item.SourceBranch.Name.Contains(searchText))
                    || (item.DestinationBranch != null && item.DestinationBranch.Name.Contains(searchText))
                    || (item.User != null && ((item.User.FullName != null && item.User.FullName.Contains(searchText)) || item.User.Username.Contains(searchText)))
                    || item.Items.Any(line => line.Product != null && line.Product.Name.Contains(searchText))
                    || item.Items.Any(line => line.Batch != null && line.Batch.BatchNo.Contains(searchText))
                    || item.Items.Any(line => line.Fuel != null && line.Fuel.Name.Contains(searchText)));
            }

            if (sourceBranchId.HasValue && sourceBranchId.Value > 0)
            {
                query = query.Where(item => item.SourceBranchId == sourceBranchId.Value);
            }

            if (destinationBranchId.HasValue && destinationBranchId.Value > 0)
            {
                query = query.Where(item => item.DestinationBranchId == destinationBranchId.Value);
            }

            var transfers = await query.OrderByDescending(item => item.CreatedAt).ThenByDescending(item => item.Id).Take(100).ToListAsync();
            return new StockTransferPageViewModel
            {
                Search = searchText,
                SourceFilterBranchId = sourceBranchId,
                SourceFilterBranchName = await BranchNameAsync(sourceBranchId),
                DestinationFilterBranchId = destinationBranchId,
                DestinationFilterBranchName = await BranchNameAsync(destinationBranchId),
                TransferType = transferType,
                Title = title,
                SaveAction = saveAction,
                CompleteAction = completeAction,
                CancelAction = cancelAction,
                BranchOptions = await BuildBranchOptionsAsync(),
                ProductOptions = await BuildTransferProductOptionsAsync(transferType),
                ProductSelectorRows = await BuildTransferProductSelectorRowsAsync(transferType),
                BatchOptions = await BuildTransferBatchOptionsAsync(transferType),
                FuelOptions = await BuildFuelOptionsAsync(),
                TankOptions = await BuildTankOptionsAsync(),
                Transfers = transfers.Select(transfer =>
                {
                    var item = transfer.Items.FirstOrDefault();
                    return new StockTransferRowViewModel
                    {
                        Id = transfer.Id,
                        TransferNo = transfer.TransferNo,
                        SourceBranch = transfer.SourceBranch?.Name ?? "-",
                        DestinationBranch = transfer.DestinationBranch?.Name ?? "-",
                        Product = item?.Product?.Name ?? "-",
                        Batch = item?.Batch?.BatchNo ?? "-",
                        Fuel = item?.Fuel?.Name ?? "-",
                        SourceTank = item?.SourceTank?.TankNo ?? "-",
                        DestinationTank = item?.DestinationTank?.TankNo ?? "-",
                        Quantity = item?.Quantity ?? 0m,
                        Liters = item?.Liters ?? 0m,
                        SourceBefore = item?.SourceBefore,
                        SourceAfter = item?.SourceAfter,
                        DestinationBefore = item?.DestinationBefore,
                        DestinationAfter = item?.DestinationAfter,
                        TransferredBy = UserDisplayName(transfer.User),
                        Date = transfer.CreatedAt,
                        Status = transfer.Status,
                        Remarks = transfer.Remarks ?? string.Empty
                    };
                }).ToList()
            };
        }

        private async Task<IActionResult> SaveStockTransfer(string transferType, StockTransferForm form, string redirectAction, string? search)
        {
            var userId = CurrentUserId();
            if (!userId.HasValue)
            {
                TempData["TransferMessage"] = "Please sign in before saving transfer.";
                return RedirectToAction(redirectAction, new { search });
            }

            try
            {
                ValidateTransferForm(transferType, form);
                var now = DateTime.Now;
                await using var transaction = await _db.Database.BeginTransactionAsync();
                var transfer = new StockTransfer
                {
                    TransferNo = await GenerateTransferNoAsync(),
                    TransferType = transferType,
                    SourceBranchId = form.SourceBranchId,
                    DestinationBranchId = form.DestinationBranchId,
                    SourceLocation = TransferSourceLocation(transferType),
                    DestinationLocation = TransferDestinationLocation(transferType),
                    Status = NormalizeTransferStatus(form.Status),
                    Remarks = form.Remarks,
                    TransferredBy = userId,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                transfer.Items.Add(new StockTransferItem
                {
                    ProductId = form.ProductId,
                    BatchId = form.BatchId,
                    FuelId = form.FuelId,
                    SourceTankId = form.SourceTankId,
                    DestinationTankId = form.DestinationTankId,
                    Quantity = transferType == "BranchToBranchFuel" ? null : form.Quantity,
                    Liters = transferType == "BranchToBranchFuel" ? form.Liters : null,
                    CreatedAt = now,
                    UpdatedAt = now
                });
                _db.StockTransfers.Add(transfer);
                await _db.SaveChangesAsync();

                if (transfer.Status == "Completed")
                {
                    await ApplyStockTransfer(transfer, transfer.Items.First(), userId.Value, now);
                    transfer.CompletedAt = now;
                    await _db.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                TempData["TransferMessage"] = "Transfer saved.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["TransferMessage"] = ex.Message;
            }

            return RedirectToAction(redirectAction, new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveWarehouseToDisplayProductFifo(StockTransferForm form, string? search)
        {
            return await SaveProductTransferFifo("WarehouseToDisplayProduct", form, nameof(WarehouseToDisplayProduct), search);
        }

        private async Task<IActionResult> SaveProductTransferFifo(string transferType, StockTransferForm form, string redirectAction, string? search)
        {
            var userId = CurrentUserId();
            if (!userId.HasValue)
            {
                TempData["TransferMessage"] = "Please sign in before saving transfer.";
                return RedirectToAction(redirectAction, new { search });
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                if (!form.ProductId.HasValue || form.ProductId.Value <= 0)
                {
                    throw new InvalidOperationException("Product is required.");
                }

                if (form.Quantity <= 0)
                {
                    throw new InvalidOperationException("Quantity must be greater than 0.");
                }

                if (!form.SourceBranchId.HasValue || form.SourceBranchId.Value <= 0)
                {
                    throw new InvalidOperationException("Source branch is required.");
                }

                if (!await _db.Branches.AsNoTracking().AnyAsync(branch => branch.Id == form.SourceBranchId.Value && branch.Status == 1))
                {
                    throw new InvalidOperationException("Select a valid active branch.");
                }

                if (transferType is "WarehouseToDisplayProduct" or "DisplayToWarehouseProduct")
                {
                    if (form.DestinationBranchId.HasValue && form.DestinationBranchId != form.SourceBranchId)
                    {
                        throw new InvalidOperationException("Warehouse and display transfers must remain within the selected source branch.");
                    }

                    form.DestinationBranchId = form.SourceBranchId;
                }

                if (transferType == "BranchToBranchProduct" && (!form.SourceBranchId.HasValue || !form.DestinationBranchId.HasValue || form.SourceBranchId == form.DestinationBranchId))
                {
                    throw new InvalidOperationException("Select different source and destination branches.");
                }

                var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(item => item.Id == form.ProductId.Value);
                if (product is null)
                {
                    throw new InvalidOperationException("Product was not found.");
                }

                var now = DateTime.Now;
                var transfer = new StockTransfer
                {
                    TransferNo = await GenerateTransferNoAsync(),
                    TransferType = transferType,
                    SourceBranchId = form.SourceBranchId,
                    DestinationBranchId = form.DestinationBranchId,
                    SourceLocation = transferType == "DisplayToWarehouseProduct" ? "Display" : "Warehouse",
                    DestinationLocation = transferType == "WarehouseToDisplayProduct" ? "Display" : "Warehouse",
                    Status = "Completed",
                    Remarks = form.Remarks,
                    TransferredBy = userId.Value,
                    CompletedAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                _db.StockTransfers.Add(transfer);
                await _db.SaveChangesAsync();

                if (transferType == "DisplayToWarehouseProduct")
                {
                    await ApplyDisplayToWarehouseFifo(transfer, form, product.Name, userId.Value, now);
                }
                else
                {
                    await ApplyWarehouseSourceFifo(transfer, form, product.Name, userId.Value, now);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["TransferMessage"] = "Transfer saved successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["TransferMessage"] = $"Failed to save transfer: {ex.Message}";
            }

            return RedirectToAction(redirectAction, new { search });
        }

        private async Task<IActionResult> CompleteStockTransfer(int id, string redirectAction, string? search)
        {
            var userId = CurrentUserId();
            if (!userId.HasValue)
            {
                TempData["TransferMessage"] = "Please sign in before completing transfer.";
                return RedirectToAction(redirectAction, new { search });
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();
            var transfer = await _db.StockTransfers.Include(item => item.Items).FirstOrDefaultAsync(item => item.Id == id);
            var item = transfer?.Items.FirstOrDefault();
            if (transfer is null || item is null)
            {
                TempData["TransferMessage"] = "Transfer record was not found.";
                return RedirectToAction(redirectAction, new { search });
            }

            if (transfer.Status == "Completed")
            {
                TempData["TransferMessage"] = "Completed transfers cannot be edited. Create another transfer or adjustment instead.";
                return RedirectToAction(redirectAction, new { search });
            }

            if (transfer.Status == "Cancelled")
            {
                TempData["TransferMessage"] = "Cancelled transfers cannot be completed.";
                return RedirectToAction(redirectAction, new { search });
            }

            try
            {
                var now = DateTime.Now;
                await ApplyStockTransfer(transfer, item, userId.Value, now);
                transfer.Status = "Completed";
                transfer.CompletedAt = now;
                transfer.UpdatedAt = now;
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["TransferMessage"] = "Transfer completed.";
            }
            catch (InvalidOperationException ex)
            {
                await transaction.RollbackAsync();
                TempData["TransferMessage"] = ex.Message;
            }

            return RedirectToAction(redirectAction, new { search });
        }

        private async Task<IActionResult> CancelStockTransfer(int id, string redirectAction, string? search)
        {
            var transfer = await _db.StockTransfers.FindAsync(id);
            if (transfer is not null && transfer.Status == "Pending")
            {
                transfer.Status = "Cancelled";
                transfer.CancelledAt = DateTime.Now;
                transfer.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();
            }
            else if (transfer?.Status == "Completed")
            {
                TempData["TransferMessage"] = "Completed transfers cannot be edited. Create another transfer or adjustment instead.";
            }

            return RedirectToAction(redirectAction, new { search });
        }

        private async Task ApplyStockTransfer(StockTransfer transfer, StockTransferItem item, int userId, DateTime now)
        {
            switch (transfer.TransferType)
            {
                case "WarehouseToDisplayProduct":
                    await MoveProductWarehouseToDisplay(transfer, item, userId, now);
                    break;
                case "DisplayToWarehouseProduct":
                    await MoveProductDisplayToWarehouse(transfer, item, userId, now);
                    break;
                case "BranchToBranchProduct":
                    await MoveProductBranchToBranch(transfer, item, userId, now);
                    break;
                case "BranchToBranchFuel":
                    await MoveFuelBranchToBranch(transfer, item, now);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported transfer type.");
            }
        }

        private async Task MoveProductWarehouseToDisplay(StockTransfer transfer, StockTransferItem item, int userId, DateTime now)
        {
            var quantity = item.Quantity ?? 0m;
            var warehouse = await _db.WarehouseStocks.FirstOrDefaultAsync(stock => stock.ProductId == item.ProductId && stock.BatchId == item.BatchId && stock.BranchId == transfer.SourceBranchId);
            if (warehouse is null || warehouse.Quantity < quantity)
            {
                throw new InvalidOperationException("Not enough warehouse stock.");
            }

            var display = await _db.DisplayStocks.FirstOrDefaultAsync(stock => stock.ProductId == item.ProductId && stock.BatchId == item.BatchId && stock.BranchId == transfer.SourceBranchId);
            if (display is null)
            {
                display = new DisplayStock { ProductId = item.ProductId!.Value, BatchId = item.BatchId!.Value, BranchId = transfer.SourceBranchId, Quantity = 0m, CreatedAt = now };
                _db.DisplayStocks.Add(display);
            }

            item.SourceBefore = warehouse.Quantity;
            item.DestinationBefore = display.Quantity;
            warehouse.Quantity -= quantity;
            display.Quantity += quantity;
            warehouse.UpdatedAt = now;
            display.UpdatedAt = now;
            item.SourceAfter = warehouse.Quantity;
            item.DestinationAfter = display.Quantity;
            AddTransferMovement(item, "Transfer", "Warehouse", "Display", transfer, userId, now);
        }

        private async Task ApplyWarehouseSourceFifo(StockTransfer transfer, StockTransferForm form, string productName, int userId, DateTime now)
        {
            var fifoStocks = await _db.WarehouseStocks
                .Include(stock => stock.Batch)
                .Where(stock => stock.ProductId == form.ProductId!.Value
                    && stock.Quantity > 0
                    && stock.Batch != null
                    && stock.BranchId == form.SourceBranchId)
                .OrderBy(stock => stock.Batch!.CreatedAt ?? DateTime.MinValue)
                .ThenBy(stock => stock.Batch!.Id)
                .ThenBy(stock => stock.Id)
                .ToListAsync();
            if (fifoStocks.Sum(stock => stock.Quantity) < form.Quantity)
            {
                throw new InvalidOperationException($"Not enough warehouse stock for {productName}.");
            }

            var remainingQuantity = form.Quantity;
            for (var i = 0; i < fifoStocks.Count && remainingQuantity > 0; i += 1)
            {
                var source = fifoStocks[i];
                var allocatedQuantity = Math.Min(remainingQuantity, source.Quantity);
                if (transfer.TransferType == "BranchToBranchProduct")
                {
                    var destinationWarehouse = await _db.WarehouseStocks.FirstOrDefaultAsync(stock => stock.ProductId == source.ProductId && stock.BatchId == source.BatchId && stock.BranchId == form.DestinationBranchId);
                    if (destinationWarehouse is null)
                    {
                        destinationWarehouse = new WarehouseStock { ProductId = source.ProductId, BatchId = source.BatchId, BranchId = form.DestinationBranchId, Quantity = 0m, CreatedAt = now };
                        _db.WarehouseStocks.Add(destinationWarehouse);
                    }

                    AddFifoTransferItemAndMove(source, destinationWarehouse, transfer, allocatedQuantity, "Warehouse", "Warehouse", userId, now);
                }
                else
                {
                    var display = await _db.DisplayStocks.FirstOrDefaultAsync(stock => stock.ProductId == source.ProductId && stock.BatchId == source.BatchId && stock.BranchId == source.BranchId);
                    if (display is null)
                    {
                        display = new DisplayStock { ProductId = source.ProductId, BatchId = source.BatchId, BranchId = source.BranchId, Quantity = 0m, CreatedAt = now };
                        _db.DisplayStocks.Add(display);
                    }

                    AddFifoTransferItemAndMove(source, display, transfer, allocatedQuantity, "Warehouse", "Display", userId, now);
                }

                remainingQuantity -= allocatedQuantity;
            }
        }

        private async Task ApplyDisplayToWarehouseFifo(StockTransfer transfer, StockTransferForm form, string productName, int userId, DateTime now)
        {
            var fifoStocks = await _db.DisplayStocks
                .Include(stock => stock.Batch)
                .Where(stock => stock.ProductId == form.ProductId!.Value
                    && stock.BranchId == form.SourceBranchId
                    && stock.Quantity > 0
                    && stock.Batch != null)
                .OrderByDescending(stock => stock.Batch!.CreatedAt ?? DateTime.MinValue)
                .ThenByDescending(stock => stock.Batch!.Id)
                .ThenBy(stock => stock.Id)
                .ToListAsync();
            if (fifoStocks.Sum(stock => stock.Quantity) < form.Quantity)
            {
                throw new InvalidOperationException($"Not enough display stock for {productName}.");
            }

            var remainingQuantity = form.Quantity;
            for (var i = 0; i < fifoStocks.Count && remainingQuantity > 0; i += 1)
            {
                var source = fifoStocks[i];
                var allocatedQuantity = Math.Min(remainingQuantity, source.Quantity);
                var warehouse = await _db.WarehouseStocks.FirstOrDefaultAsync(stock => stock.ProductId == source.ProductId && stock.BatchId == source.BatchId && stock.BranchId == source.BranchId);
                if (warehouse is null)
                {
                    warehouse = new WarehouseStock { ProductId = source.ProductId, BatchId = source.BatchId, BranchId = source.BranchId, Quantity = 0m, CreatedAt = now };
                    _db.WarehouseStocks.Add(warehouse);
                }

                AddFifoTransferItemAndMove(source, warehouse, transfer, allocatedQuantity, "Display", "Warehouse", userId, now);
                remainingQuantity -= allocatedQuantity;
            }
        }

        private void AddFifoTransferItemAndMove(dynamic source, dynamic destination, StockTransfer transfer, decimal allocatedQuantity, string sourceLocation, string destinationLocation, int userId, DateTime now)
        {
            var sourceBefore = source.Quantity;
            var destinationBefore = destination.Quantity;
            source.Quantity -= allocatedQuantity;
            destination.Quantity += allocatedQuantity;
            source.UpdatedAt = now;
            destination.UpdatedAt = now;
            var item = new StockTransferItem
            {
                StockTransferId = transfer.Id,
                ProductId = source.ProductId,
                BatchId = source.BatchId,
                Quantity = allocatedQuantity,
                SourceBefore = sourceBefore,
                SourceAfter = source.Quantity,
                DestinationBefore = destinationBefore,
                DestinationAfter = destination.Quantity,
                CreatedAt = now,
                UpdatedAt = now
            };
            _db.StockTransferItems.Add(item);
            AddTransferMovement(item, "Transfer", sourceLocation, destinationLocation, transfer, userId, now);
        }

        private async Task MoveProductDisplayToWarehouse(StockTransfer transfer, StockTransferItem item, int userId, DateTime now)
        {
            var quantity = item.Quantity ?? 0m;
            var display = await _db.DisplayStocks.FirstOrDefaultAsync(stock => stock.ProductId == item.ProductId && stock.BatchId == item.BatchId && stock.BranchId == transfer.SourceBranchId);
            if (display is null || display.Quantity < quantity)
            {
                throw new InvalidOperationException("Not enough display stock.");
            }

            var warehouse = await _db.WarehouseStocks.FirstOrDefaultAsync(stock => stock.ProductId == item.ProductId && stock.BatchId == item.BatchId && stock.BranchId == transfer.SourceBranchId);
            if (warehouse is null)
            {
                warehouse = new WarehouseStock { ProductId = item.ProductId!.Value, BatchId = item.BatchId!.Value, BranchId = transfer.SourceBranchId, Quantity = 0m, CreatedAt = now };
                _db.WarehouseStocks.Add(warehouse);
            }

            item.SourceBefore = display.Quantity;
            item.DestinationBefore = warehouse.Quantity;
            display.Quantity -= quantity;
            warehouse.Quantity += quantity;
            display.UpdatedAt = now;
            warehouse.UpdatedAt = now;
            item.SourceAfter = display.Quantity;
            item.DestinationAfter = warehouse.Quantity;
            AddTransferMovement(item, "Transfer", "Display", "Warehouse", transfer, userId, now);
        }

        private async Task MoveProductBranchToBranch(StockTransfer transfer, StockTransferItem item, int userId, DateTime now)
        {
            var quantity = item.Quantity ?? 0m;
            var source = await _db.WarehouseStocks.FirstOrDefaultAsync(stock => stock.BranchId == transfer.SourceBranchId && stock.ProductId == item.ProductId && stock.BatchId == item.BatchId);
            if (source is null || source.Quantity < quantity)
            {
                throw new InvalidOperationException("Not enough source branch warehouse stock.");
            }

            var destination = await _db.WarehouseStocks.FirstOrDefaultAsync(stock => stock.BranchId == transfer.DestinationBranchId && stock.ProductId == item.ProductId && stock.BatchId == item.BatchId);
            if (destination is null)
            {
                destination = new WarehouseStock { BranchId = transfer.DestinationBranchId, ProductId = item.ProductId!.Value, BatchId = item.BatchId!.Value, Quantity = 0m, CreatedAt = now };
                _db.WarehouseStocks.Add(destination);
            }

            item.SourceBefore = source.Quantity;
            item.DestinationBefore = destination.Quantity;
            source.Quantity -= quantity;
            destination.Quantity += quantity;
            source.UpdatedAt = now;
            destination.UpdatedAt = now;
            item.SourceAfter = source.Quantity;
            item.DestinationAfter = destination.Quantity;
            AddTransferMovement(item, "Transfer", "Warehouse", "Warehouse", transfer, userId, now);
        }

        private async Task MoveFuelBranchToBranch(StockTransfer transfer, StockTransferItem item, DateTime now)
        {
            var liters = item.Liters ?? 0m;
            var source = await _db.Tanks.FirstOrDefaultAsync(tank => tank.Id == item.SourceTankId && tank.BranchId == transfer.SourceBranchId && tank.FuelId == item.FuelId);
            var destination = await _db.Tanks.FirstOrDefaultAsync(tank => tank.Id == item.DestinationTankId && tank.BranchId == transfer.DestinationBranchId && tank.FuelId == item.FuelId);
            if (source is null || destination is null || source.Id == destination.Id)
            {
                throw new InvalidOperationException("Select valid source and destination tanks for the same fuel.");
            }

            if (source.CurrentLiters < liters)
            {
                throw new InvalidOperationException("Not enough source tank liters.");
            }

            item.SourceBefore = source.CurrentLiters;
            item.DestinationBefore = destination.CurrentLiters;
            source.CurrentLiters -= liters;
            destination.CurrentLiters += liters;
            source.UpdatedAt = now;
            destination.UpdatedAt = now;
            item.SourceAfter = source.CurrentLiters;
            item.DestinationAfter = destination.CurrentLiters;
        }

        private void AddTransferMovement(StockTransferItem item, string movementType, string source, string destination, StockTransfer transfer, int userId, DateTime now)
        {
            _db.StockMovements.Add(new StockMovement
            {
                ProductId = item.ProductId!.Value,
                ProductBatchId = item.BatchId,
                SourceLocation = source,
                DestinationLocation = destination,
                MovementType = movementType,
                Quantity = item.Quantity ?? 0m,
                ReferenceType = transfer.TransferType,
                ReferenceId = transfer.Id,
                CreatedBy = userId,
                Remarks = transfer.Remarks,
                CreatedAt = now
            });
        }

        private static string NormalizeTransferStatus(string? status)
        {
            return string.Equals(status, "Completed", StringComparison.OrdinalIgnoreCase)
                ? "Completed"
                : string.Equals(status, "Cancelled", StringComparison.OrdinalIgnoreCase) ? "Cancelled" : "Pending";
        }

        private static string TransferSourceLocation(string transferType)
        {
            return transferType == "DisplayToWarehouseProduct" ? "Display" : transferType == "BranchToBranchFuel" ? "Tank" : "Warehouse";
        }

        private static string TransferDestinationLocation(string transferType)
        {
            return transferType == "WarehouseToDisplayProduct" ? "Display" : transferType == "BranchToBranchFuel" ? "Tank" : "Warehouse";
        }

        private void ValidateTransferForm(string transferType, StockTransferForm form)
        {
            if (transferType == "BranchToBranchProduct" || transferType == "BranchToBranchFuel")
            {
                if (!form.SourceBranchId.HasValue || !form.DestinationBranchId.HasValue || form.SourceBranchId == form.DestinationBranchId)
                {
                    throw new InvalidOperationException("Select different source and destination branches.");
                }
            }

            if (transferType == "BranchToBranchFuel")
            {
                if (!form.FuelId.HasValue || !form.SourceTankId.HasValue || !form.DestinationTankId.HasValue || form.Liters <= 0)
                {
                    throw new InvalidOperationException("Fuel, tanks, and liters are required.");
                }
                return;
            }

            if (!form.ProductId.HasValue || !form.BatchId.HasValue || form.Quantity <= 0)
            {
                throw new InvalidOperationException("Product, batch, and quantity are required.");
            }
        }

        private async Task<List<SelectListItem>> BuildBranchOptionsAsync()
        {
            var options = await _db.Branches.AsNoTracking().Where(item => item.Status == 1).OrderBy(item => item.Name).Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Name }).ToListAsync();
            options.Insert(0, new SelectListItem { Value = "", Text = "Select branch" });
            return options;
        }

        private async Task<List<SelectListItem>> BuildBranchFilterOptionsAsync()
        {
            var options = await _db.Branches.AsNoTracking()
                .Where(item => item.Status == 1)
                .OrderBy(item => item.Name)
                .Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Name })
                .ToListAsync();
            options.Insert(0, new SelectListItem { Value = "", Text = "All Branches" });
            return options;
        }

        private async Task<List<SelectListItem>> BuildTransferProductOptionsAsync(string transferType)
        {
            var query = transferType == "DisplayToWarehouseProduct"
                ? _db.DisplayStocks.AsNoTracking().Include(stock => stock.Product).Where(stock => stock.Quantity > 0 && stock.Product != null).Select(stock => stock.Product!)
                : _db.WarehouseStocks.AsNoTracking().Include(stock => stock.Product).Where(stock => stock.Quantity > 0 && stock.Product != null).Select(stock => stock.Product!);
            var options = await query.Distinct().OrderBy(product => product.Name).Select(product => new SelectListItem { Value = product.Id.ToString(), Text = product.Name }).ToListAsync();
            options.Insert(0, new SelectListItem { Value = "", Text = "Select product" });
            return options;
        }

        private async Task<List<StockTransferProductSelectorRowViewModel>> BuildTransferProductSelectorRowsAsync(string transferType)
        {
            if (transferType == "DisplayToWarehouseProduct")
            {
                return await _db.DisplayStocks
                    .AsNoTracking()
                    .Where(stock => stock.Quantity > 0 && stock.Product != null)
                    .GroupBy(stock => new
                    {
                        stock.BranchId,
                        stock.ProductId,
                        ProductName = stock.Product!.Name,
                        CategoryName = stock.Product.Category != null ? stock.Product.Category.Name : "-",
                        UnitName = stock.Product.ProductUnit != null ? stock.Product.ProductUnit.Name : "-"
                    })
                    .OrderBy(group => group.Key.ProductName)
                    .Select(group => new StockTransferProductSelectorRowViewModel
                    {
                        BranchId = group.Key.BranchId,
                        ProductId = group.Key.ProductId,
                        ProductName = group.Key.ProductName,
                        CategoryName = group.Key.CategoryName,
                        UnitName = group.Key.UnitName,
                        AvailableQuantity = group.Sum(stock => stock.Quantity)
                    })
                    .ToListAsync();
            }

            if (transferType == "BranchToBranchProduct")
            {
                return await _db.WarehouseStocks
                    .AsNoTracking()
                    .Where(stock => stock.Quantity > 0 && stock.Product != null)
                    .GroupBy(stock => new
                    {
                        stock.BranchId,
                        stock.ProductId,
                        ProductName = stock.Product!.Name,
                        CategoryName = stock.Product.Category != null ? stock.Product.Category.Name : "-",
                        UnitName = stock.Product.ProductUnit != null ? stock.Product.ProductUnit.Name : "-"
                    })
                    .OrderBy(group => group.Key.ProductName)
                    .Select(group => new StockTransferProductSelectorRowViewModel
                    {
                        BranchId = group.Key.BranchId,
                        ProductId = group.Key.ProductId,
                        ProductName = group.Key.ProductName,
                        CategoryName = group.Key.CategoryName,
                        UnitName = group.Key.UnitName,
                        AvailableQuantity = group.Sum(stock => stock.Quantity)
                    })
                    .ToListAsync();
            }

            return await _db.WarehouseStocks
                .AsNoTracking()
                .Where(stock => stock.Quantity > 0 && stock.Product != null)
                .GroupBy(stock => new
                {
                    stock.BranchId,
                    stock.ProductId,
                    ProductName = stock.Product!.Name,
                    CategoryName = stock.Product.Category != null ? stock.Product.Category.Name : "-",
                    UnitName = stock.Product.ProductUnit != null ? stock.Product.ProductUnit.Name : "-"
                })
                .OrderBy(group => group.Key.ProductName)
                .Select(group => new StockTransferProductSelectorRowViewModel
                {
                    BranchId = group.Key.BranchId,
                    ProductId = group.Key.ProductId,
                    ProductName = group.Key.ProductName,
                    CategoryName = group.Key.CategoryName,
                    UnitName = group.Key.UnitName,
                    AvailableQuantity = group.Sum(stock => stock.Quantity)
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> BuildTransferBatchOptionsAsync(string transferType)
        {
            var query = transferType == "DisplayToWarehouseProduct"
                ? _db.DisplayStocks.AsNoTracking().Include(stock => stock.Product).Include(stock => stock.Batch).Where(stock => stock.Quantity > 0 && stock.Batch != null).Select(stock => stock.Batch!)
                : _db.WarehouseStocks.AsNoTracking().Include(stock => stock.Product).Include(stock => stock.Batch).Where(stock => stock.Quantity > 0 && stock.Batch != null).Select(stock => stock.Batch!);
            var options = await query.Distinct().OrderBy(batch => batch.Product!.Name).ThenBy(batch => batch.CreatedAt ?? DateTime.MinValue).ThenBy(batch => batch.Id).Select(batch => new SelectListItem { Value = batch.Id.ToString(), Text = $"{batch.Product!.Name} - {batch.BatchNo}" }).ToListAsync();
            options.Insert(0, new SelectListItem { Value = "", Text = "Select batch" });
            return options;
        }

        private async Task<string> GenerateTransferNoAsync()
        {
            return $"TRF-{DateTime.Now:yyyyMMdd}-{await _db.StockTransfers.CountAsync() + 1:0000}";
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

        private async Task<PosUserContext?> CurrentPosUserContextAsync()
        {
            var userId = CurrentUserId();
            if (!userId.HasValue)
            {
                return null;
            }

            return await _db.Users.AsNoTracking()
                .Where(user => user.Id == userId.Value
                    && user.Status == 1
                    && user.BranchId.HasValue
                    && user.Branch != null
                    && user.Branch.Status == 1)
                .Select(user => new PosUserContext(user.Id, user.BranchId!.Value, user.Branch!.Name))
                .FirstOrDefaultAsync();
        }

        private async Task<bool> CanManageDisplayRefillAsync(int userId)
        {
            return await _db.UserRoles.AsNoTracking()
                .AnyAsync(userRole => userRole.UserId == userId
                    && userRole.Role != null
                    && userRole.Role.Status == 1
                    && ((userRole.Role.Code.ToLower() == "supperadmin"
                            || userRole.Role.Code.ToLower() == "superadmin"
                            || userRole.Role.Code.ToLower() == "admin"
                            || userRole.Role.Code.ToLower() == "manager")
                        || userRole.Role.RolePermissions.Any(rolePermission => rolePermission.Status == 1
                            && rolePermission.Permission != null
                            && rolePermission.Permission.Status == 1
                            && (rolePermission.Permission.Code.ToLower() == "tranwtd.create"
                                || rolePermission.Permission.Code.ToLower() == "warehouse.display.transfer.create"))));
        }

        private async Task<List<ValidatedProductSaleItem>> BuildValidatedProductItems(List<PosProductSaleRequestItem> products)
        {
            var items = new List<ValidatedProductSaleItem>();
            var requestedProducts = products
                .Where(item => item.ProductId > 0)
                .GroupBy(item => item.ProductId)
                .Select(group => new { ProductId = group.Key, Quantity = group.Sum(item => item.Quantity) })
                .ToList();

            for (var i = 0; i < requestedProducts.Count; i += 1)
            {
                var request = requestedProducts[i];
                if (request.Quantity <= 0)
                {
                    throw new InvalidOperationException("Product quantity must be greater than 0.");
                }

                var product = await _db.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(item => item.Id == request.ProductId && item.Status == 1 && item.IsActive);

                if (product is null)
                {
                    throw new InvalidOperationException("One or more selected products are no longer active.");
                }

                var currentSellingPrice = await _db.ProductBatches
                    .AsNoTracking()
                    .Where(batch => batch.ProductId == request.ProductId && batch.Status == 1 && batch.IsActive)
                    .OrderByDescending(batch => batch.CreatedAt ?? DateTime.MinValue)
                    .ThenByDescending(batch => batch.Id)
                    .Select(batch => (decimal?)batch.SellingPrice)
                    .FirstOrDefaultAsync();

                if (!currentSellingPrice.HasValue)
                {
                    throw new InvalidOperationException($"Not enough display stock for {product.Name}.");
                }

                var fifoStocks = await _db.DisplayStocks
                    .AsNoTracking()
                    .Include(stock => stock.Batch)
                    .Where(stock => stock.ProductId == request.ProductId
                        && stock.Quantity > 0
                        && stock.Batch != null
                        && stock.Batch.Status == 1
                        && stock.Batch.IsActive
                        && stock.Batch.ProductId == request.ProductId)
                    .OrderBy(stock => stock.Batch!.CreatedAt ?? DateTime.MinValue)
                    .ThenBy(stock => stock.Batch!.Id)
                    .ThenBy(stock => stock.Id)
                    .ToListAsync();

                var totalDisplayStock = fifoStocks.Sum(stock => stock.Quantity);
                if (totalDisplayStock < request.Quantity)
                {
                    throw new InvalidOperationException($"Not enough display stock for {product.Name}.");
                }

                var remainingQuantity = request.Quantity;
                for (var stockIndex = 0; stockIndex < fifoStocks.Count && remainingQuantity > 0; stockIndex += 1)
                {
                    var stock = fifoStocks[stockIndex];
                    var consumedQuantity = Math.Min(remainingQuantity, stock.Quantity);
                    var unitPrice = currentSellingPrice.Value;
                    var unitCost = stock.Batch!.CostPrice;
                    var subtotal = unitPrice * consumedQuantity;
                    var cost = unitCost * consumedQuantity;
                    var grossProfit = subtotal - cost;

                    items.Add(new ValidatedProductSaleItem
                    {
                        DisplayStockId = stock.Id,
                        ProductId = stock.ProductId,
                        CategoryId = product.CategoryId,
                        ProductName = product.Name,
                        Quantity = consumedQuantity,
                        Subtotal = subtotal,
                        Cost = cost,
                        ProductSale = new ProductSale
                        {
                            DisplayStockId = stock.Id,
                            ProductId = stock.ProductId,
                            BatchId = stock.BatchId,
                            Quantity = consumedQuantity,
                            Price = unitPrice,
                            UnitCost = unitCost,
                            UnitPrice = unitPrice,
                            Subtotal = subtotal,
                            GrossProfit = grossProfit,
                            Status = "Completed",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        StockMovement = new StockMovement
                        {
                            ProductId = stock.ProductId,
                            ProductBatchId = stock.BatchId,
                            SourceLocation = "Display",
                            MovementType = "Sale",
                            Quantity = consumedQuantity,
                            ReferenceType = "POS",
                            Remarks = $"POS FIFO sale for {product.Name}",
                            CreatedAt = DateTime.Now
                        }
                    });

                    remainingQuantity -= consumedQuantity;
                }
            }

            return items;
        }

        private async Task<List<ValidatedFuelSaleItem>> BuildValidatedFuelItems(List<PosFuelSaleRequestItem> fuels, int? branchId)
        {
            var items = new List<ValidatedFuelSaleItem>();

            if (fuels.Count > 0 && !branchId.HasValue)
            {
                throw new InvalidOperationException("The signed-in user must be assigned to a branch before selling fuel.");
            }

            for (var i = 0; i < fuels.Count; i += 1)
            {
                var request = fuels[i];
                var nozzle = await FuelEquipmentValidator.LoadPosNozzleAsync(_db, request.NozzleId ?? 0);
                var pump = nozzle?.Pump; var tank = pump?.Tank;

                if (nozzle is null || pump is null || pump.Status != 1 || pump.Dispenser is null || pump.Dispenser.Status != 1
                    || tank is null || tank.Fuel is null || tank.Status != 1 || !tank.IsActive || tank.Fuel.Status != 1 || !tank.Fuel.IsActive
                    || pump.Dispenser.BranchId != branchId || tank.BranchId != branchId || pump.DispenserId is null || pump.TankId is null
                    || await _db.Nozzles.CountAsync(item => item.PumpId == pump.Id && item.Status == 1) != 1)
                {
                    throw new InvalidOperationException("One or more selected fuel items are no longer available.");
                }

                var branchPrice = await _db.BranchFuelPrices
                    .AsNoTracking()
                    .Where(item => item.BranchId == branchId!.Value && item.FuelId == tank.FuelId && item.Status == 1)
                    .Select(item => (decimal?)item.CurrentPricePerLiter)
                    .FirstOrDefaultAsync();
                var price = branchPrice ?? tank.Fuel.CurrentPricePerLiter;
                var subtotal = request.Liters * price;
                var costPerLiter = await _db.FuelDeliveries
                    .AsNoTracking()
                    .Where(item => item.TankId == tank.Id
                        && item.FuelId == tank.FuelId
                        && item.Status == 1
                        && item.CostPerLiter.HasValue)
                    .OrderByDescending(item => item.DeliveryDate)
                    .ThenByDescending(item => item.Id)
                    .Select(item => item.CostPerLiter)
                    .FirstOrDefaultAsync();
                var cost = request.Liters * (costPerLiter ?? 0m);

                items.Add(new ValidatedFuelSaleItem
                {
                    FuelName = tank.Fuel.Name,
                    Tank = tank,
                    Liters = request.Liters,
                    Subtotal = subtotal,
                    Cost = cost,
                    FuelSale = new FuelSale
                    {
                        FuelId = tank.FuelId,
                        TankId = tank.Id,
                        NozzleId = nozzle.Id,
                        PumpId = pump.Id,
                        DispenserId = pump.Dispenser.Id,
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
                    throw new InvalidOperationException($"Not enough display stock for {items[i].ProductName}.");
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

        private async Task UpdateFinancialMetricsForSale(
            Sale sale,
            List<ValidatedProductSaleItem> productItems,
            List<ValidatedFuelSaleItem> fuelItems,
            decimal discountAmount,
            decimal rebateAmount,
            bool isPointsPayment)
        {
            var metricDate = (sale.CreatedAt ?? DateTime.Now).Date;
            var productSales = productItems.Sum(item => item.Subtotal);
            var fuelSales = fuelItems.Sum(item => item.Subtotal);
            var grossSales = FinancialMetricsService.ComputeGrossSales(fuelSales, productSales);
            var costOfGoodsSold = productItems.Sum(item => item.Cost) + fuelItems.Sum(item => item.Cost);
            var pointsRedeemedAmount = isPointsPayment ? rebateAmount : 0m;
            var rebateMetricAmount = isPointsPayment ? 0m : rebateAmount;
            var netSales = FinancialMetricsService.ComputeNetSales(grossSales, discountAmount, rebateMetricAmount, pointsRedeemedAmount, 0m);
            var grossProfit = FinancialMetricsService.ComputeGrossProfit(netSales, costOfGoodsSold);

            await AddMetricIfNonZero(metricDate, "gross_sales", grossSales);
            await AddMetricIfNonZero(metricDate, "net_sales", netSales);
            await AddMetricIfNonZero(metricDate, "gross_profit", grossProfit);
            await AddMetricIfNonZero(metricDate, "cost_of_goods_sold", costOfGoodsSold);
            await AddMetricIfNonZero(metricDate, "product_sales", productSales);
            await AddMetricIfNonZero(metricDate, "fuel_sales", fuelSales);
            await AddMetricIfNonZero(metricDate, "total_discount", discountAmount);
            await AddMetricIfNonZero(metricDate, "total_rebate", rebateMetricAmount);
            await AddMetricIfNonZero(metricDate, "total_points_redeemed", pointsRedeemedAmount);
        }

        private async Task AddMetricIfNonZero(DateTime metricDate, string metricCode, decimal amount)
        {
            if (amount == 0m)
            {
                return;
            }

            await _financialMetrics.AddToMetric(metricDate, metricCode, amount);
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

        private async Task<VatSetting?> FindCurrentVatSetting()
        {
            return await _db.VatSettings
                .AsNoTracking()
                .Where(setting => setting.IsActive)
                .OrderByDescending(setting => setting.IsDefault)
                .ThenByDescending(setting => setting.CreatedAt)
                .ThenByDescending(setting => setting.Id)
                .FirstOrDefaultAsync();
        }

        private static decimal CalculateVatAmount(decimal taxableAmount, VatSetting? vat)
        {
            if (vat is null || vat.Rate <= 0m || taxableAmount <= 0m || IsVatExempt(vat.Type))
            {
                return 0m;
            }

            return IsVatInclusive(vat.Type)
                ? Math.Round(taxableAmount * vat.Rate / (100m + vat.Rate), 2, MidpointRounding.AwayFromZero)
                : Math.Round(taxableAmount * vat.Rate / 100m, 2, MidpointRounding.AwayFromZero);
        }

        private static decimal ExclusiveVatAmount(decimal taxAmount, VatSetting? vat)
        {
            return vat is not null && IsVatExclusive(vat.Type) ? taxAmount : 0m;
        }

        private static bool IsVatInclusive(string? vatType)
        {
            return string.Equals(vatType, "Inclusive", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsVatExclusive(string? vatType)
        {
            return string.Equals(vatType, "Exclusive", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsVatExempt(string? vatType)
        {
            return string.Equals(vatType, "Exempt", StringComparison.OrdinalIgnoreCase)
                || string.Equals(vatType, "ZeroRated", StringComparison.OrdinalIgnoreCase);
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
            decimal productTotal,
            decimal fuelTotal,
            DateTime now)
        {
            var rule = await _db.VoucherRules
                .Include(rule => rule.Redemptions)
                .Include(rule => rule.Voucher)
                    .ThenInclude(voucher => voucher!.Redemptions)
                .Where(rule => rule.Code == voucherCode || (rule.Voucher != null && rule.Voucher.Code == voucherCode))
                .OrderBy(rule => rule.Id)
                .FirstOrDefaultAsync();

            if (rule is null)
            {
                throw new InvalidOperationException("Voucher not found.");
            }

            if (rule.Status != 1)
            {
                throw new InvalidOperationException("Voucher inactive.");
            }

            var voucher = rule.Voucher;
            if (voucher is not null
                && !string.Equals(voucher.Status, "Active", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(voucher.Status, "Redeemed", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Voucher inactive.");
            }

            if (voucher?.MemberId.HasValue == true && (member is null || voucher.MemberId.Value != member.Id))
            {
                throw new InvalidOperationException("Member required.");
            }

            var discountAmount = ValidateAndCalculateVoucherDiscount(rule, voucher, member, productTotal, fuelTotal, now);
            return new VoucherRedemptionResult(voucher, rule, discountAmount);
        }

        private static decimal ValidateAndCalculateVoucherDiscount(
            VoucherRule rule,
            Voucher? voucher,
            Member? member,
            decimal productTotal,
            decimal fuelTotal,
            DateTime now)
        {
            if (rule.EffectiveDate.HasValue && now.Date < rule.EffectiveDate.Value.Date)
            {
                throw new InvalidOperationException("Voucher inactive.");
            }

            if (!rule.NoExpiration && rule.ExpirationDate.HasValue && now.Date > rule.ExpirationDate.Value.Date)
            {
                throw new InvalidOperationException("Voucher has expired.");
            }

            var voucherUseCount = voucher?.Redemptions.Count ?? rule.Redemptions.Count;
            var usageLimit = Math.Max(1, rule.MaxRedemptions ?? rule.LimitedUseCount ?? 1);
            if (voucherUseCount >= usageLimit)
            {
                throw new InvalidOperationException("Voucher usage limit reached.");
            }

            var eligibleBase = EligibleVoucherBase(rule, productTotal, fuelTotal);
            if (eligibleBase <= 0m)
            {
                throw new InvalidOperationException("Voucher does not apply to the selected items.");
            }

            if (eligibleBase < rule.MinimumPurchaseAmount)
            {
                throw new InvalidOperationException("Minimum purchase not met.");
            }

            if (rule.MemberRequired == 1 && member is null)
            {
                throw new InvalidOperationException("Member required.");
            }

            var rewardValue = Math.Max(0m, rule.RewardValue);
            var discountAmount = IsPercentageDiscount(rule.RewardType)
                ? eligibleBase * (rewardValue / 100m)
                : rewardValue;

            return Math.Round(Math.Min(eligibleBase, Math.Max(0m, discountAmount)), 2, MidpointRounding.AwayFromZero);
        }

        private static decimal EligibleVoucherBase(
            VoucherRule rule,
            decimal productTotal,
            decimal fuelTotal)
        {
            var appliesTo = (rule.AppliesTo ?? string.Empty).Trim();
            var productBase = AppliesToFuelOnly(appliesTo) ? 0m : productTotal;
            var fuelBase = AppliesToProductOnly(appliesTo) ? 0m : fuelTotal;

            return productBase + fuelBase;
        }

        private void ApplyVoucherRedemption(VoucherRedemptionResult? redemption, int saleId, DateTime now)
        {
            if (redemption is null)
            {
                return;
            }

            _db.VoucherRedemptions.Add(new VoucherRedemption
            {
                VoucherId = redemption.Voucher?.Id,
                VoucherRuleId = redemption.Rule.Id,
                SaleId = saleId,
                DiscountAmount = redemption.DiscountAmount,
                CreatedAt = now
            });

            if (redemption.Voucher is not null)
            {
                redemption.Voucher.Status = VoucherStatusAfterRedemption(redemption.Voucher, redemption.Rule);
                redemption.Voucher.UpdatedAt = now;
            }
            else
            {
                redemption.Rule.Status = VoucherStatusAfterRedemption(redemption.Rule) == "Redeemed" ? 0 : 1;
                redemption.Rule.UpdatedAt = now;
            }
        }

        private static string VoucherStatusAfterRedemption(Voucher voucher, VoucherRule rule)
        {
            var useCountAfterRedemption = voucher.Redemptions.Count + 1;
            var usageLimit = Math.Max(1, rule.MaxRedemptions ?? rule.LimitedUseCount ?? 1);

            return useCountAfterRedemption >= usageLimit ? "Redeemed" : "Active";
        }

        private static string VoucherStatusAfterRedemption(VoucherRule rule)
        {
            var useCountAfterRedemption = rule.Redemptions.Count + 1;
            var usageLimit = Math.Max(1, rule.MaxRedemptions ?? rule.LimitedUseCount ?? 1);

            return useCountAfterRedemption >= usageLimit ? "Redeemed" : "Active";
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

        private async Task<StockAdjustmentPageViewModel> BuildStockAdjustmentPageAsync(string scope, string? search, int? branchId, DateTime? dateFrom, DateTime? dateTo, string? status, int? editId, int? detailsId, StockAdjustmentForm? suppliedForm = null)
        {
            scope = NormalizeAdjustmentScope(scope);
            var query = _db.StockAdjustments.AsNoTracking().Include(x => x.Branch).Include(x => x.Product).Include(x => x.Batch).Include(x => x.Fuel).Include(x => x.Tank).Include(x => x.AdjustedByUser).Include(x => x.PostedByUser).Where(x => x.Scope == scope);
            if (branchId > 0) query = query.Where(x => x.BranchId == branchId);
            if (dateFrom.HasValue) query = query.Where(x => x.BusinessDate >= dateFrom.Value.Date);
            if (dateTo.HasValue) query = query.Where(x => x.BusinessDate < dateTo.Value.Date.AddDays(1));
            if (!string.IsNullOrWhiteSpace(status)) query = query.Where(x => x.Status == status);
            if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.AdjustmentNo.Contains(search) || x.Reason.Contains(search) || (x.Remarks != null && x.Remarks.Contains(search)) || (x.Product != null && x.Product.Name.Contains(search)) || (x.Fuel != null && x.Fuel.Name.Contains(search)));
            var form = suppliedForm;
            if (form is null && editId.HasValue)
            {
                var x = await _db.StockAdjustments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == editId && a.Scope == scope && a.Status == "Draft");
                if (x != null) form = new StockAdjustmentForm { Id = x.Id, Scope = scope, BusinessDate = x.BusinessDate, BranchId = x.BranchId, WarehouseStockId = x.WarehouseStockId, DisplayStockId = x.DisplayStockId, TankId = x.TankId, AdjustmentType = x.AdjustmentType, AdjustmentQuantity = x.AdjustmentQuantity, Reason = x.Reason, Remarks = x.Remarks };
            }
            form ??= new StockAdjustmentForm { Scope = scope, BusinessDate = DateTime.Today, BranchId = branchId ?? 0 };
            return new StockAdjustmentPageViewModel { Scope = scope, Title = scope == "Fuel" ? "Fuel Stock Adjustment" : $"{scope} Stock Adjustment", PageAction = AdjustmentViewName(scope), Search = search ?? "", BranchId = branchId, DateFrom = dateFrom, DateTo = dateTo, Status = status, Form = form, Rows = await query.OrderByDescending(x => x.BusinessDate).ThenByDescending(x => x.Id).ToListAsync(), BranchOptions = await BuildBranchFilterOptionsAsync(), Targets = await BuildAdjustmentTargetsAsync(scope), DetailsId = detailsId };
        }

        private async Task<List<StockAdjustmentTargetOption>> BuildAdjustmentTargetsAsync(string scope)
        {
            if (scope == "Warehouse") return await _db.WarehouseStocks.AsNoTracking().Include(x => x.Product).Include(x => x.Batch).Where(x => x.BranchId.HasValue).Select(x => new StockAdjustmentTargetOption { Id = x.Id, BranchId = x.BranchId!.Value, Label = x.Product!.Name + " / " + x.Batch!.BatchNo, ProductOrFuel = x.Product.Name, BatchOrTank = x.Batch.BatchNo, CurrentQuantity = x.Quantity }).ToListAsync();
            if (scope == "Display") return await _db.DisplayStocks.AsNoTracking().Include(x => x.Product).Include(x => x.Batch).Where(x => x.BranchId.HasValue).Select(x => new StockAdjustmentTargetOption { Id = x.Id, BranchId = x.BranchId!.Value, Label = x.Product!.Name + " / " + x.Batch!.BatchNo, ProductOrFuel = x.Product.Name, BatchOrTank = x.Batch.BatchNo, CurrentQuantity = x.Quantity }).ToListAsync();
            return await _db.Tanks.AsNoTracking().Include(x => x.Fuel).Where(x => x.BranchId.HasValue && x.IsActive && x.Status == 1).Select(x => new StockAdjustmentTargetOption { Id = x.Id, BranchId = x.BranchId!.Value, Label = x.TankNo + " / " + x.Fuel!.Name, ProductOrFuel = x.Fuel.Name, BatchOrTank = x.TankNo, CurrentQuantity = x.CurrentLiters, Capacity = x.CapacityLiters }).ToListAsync();
        }

        private static string NormalizeAdjustmentScope(string? scope) => scope is "Display" or "Fuel" ? scope : "Warehouse";
        private static string AdjustmentViewName(string scope) => scope == "Display" ? nameof(DisplayStockAdjustment) : scope == "Fuel" ? nameof(TankFuelAdjustment) : nameof(WarehouseStockAdjustment);
        private IActionResult AdjustmentRedirect(string scope, string message) { TempData["AdjustmentMessage"] = message; return RedirectToAction(AdjustmentViewName(NormalizeAdjustmentScope(scope))); }
        private IActionResult AdjustmentApplyResponse(string scope, string message, bool success)
        {
            if (Request.Headers.Accept.Any(value => value?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true))
            {
                if (success) TempData["AdjustmentMessage"] = message;
                return Json(new { success, message });
            }
            return AdjustmentRedirect(scope, message);
        }
        private static void ValidateAdjustmentTarget(StockAdjustmentForm form, string scope)
        {
            if (scope == "Warehouse" && !form.WarehouseStockId.HasValue) form.WarehouseStockId = 0;
            if (scope == "Display" && !form.DisplayStockId.HasValue) form.DisplayStockId = 0;
            if (scope == "Fuel" && !form.TankId.HasValue) form.TankId = 0;
        }
        private async Task<object?> LoadAdjustmentTargetAsync(StockAdjustmentForm form, string scope, bool tracking)
        {
            if (scope == "Warehouse") return await _db.WarehouseStocks.FirstOrDefaultAsync(x => x.Id == form.WarehouseStockId && x.BranchId == form.BranchId && x.Batch != null && x.Batch.ProductId == x.ProductId);
            if (scope == "Display") return await _db.DisplayStocks.FirstOrDefaultAsync(x => x.Id == form.DisplayStockId && x.BranchId == form.BranchId && x.Batch != null && x.Batch.ProductId == x.ProductId);
            return await _db.Tanks.FirstOrDefaultAsync(x => x.Id == form.TankId && x.BranchId == form.BranchId && x.IsActive && x.Status == 1);
        }
        private async Task<object?> LoadAdjustmentTargetAsync(StockAdjustment x, bool tracking)
        {
            if (x.Scope == "Warehouse") return await _db.WarehouseStocks.FirstOrDefaultAsync(s => s.Id == x.WarehouseStockId && s.BranchId == x.BranchId && s.Batch != null && s.Batch.ProductId == s.ProductId);
            if (x.Scope == "Display") return await _db.DisplayStocks.FirstOrDefaultAsync(s => s.Id == x.DisplayStockId && s.BranchId == x.BranchId && s.Batch != null && s.Batch.ProductId == s.ProductId);
            return await _db.Tanks.FirstOrDefaultAsync(s => s.Id == x.TankId && s.BranchId == x.BranchId && s.IsActive && s.Status == 1);
        }
        private static decimal TargetQuantity(object target) => target is WarehouseStock w ? w.Quantity : target is DisplayStock d ? d.Quantity : ((Tank)target).CurrentLiters;
        private static void PopulateAdjustment(StockAdjustment x, StockAdjustmentForm f, object target, string scope, DateTime now)
        {
            x.BusinessDate = f.BusinessDate!.Value.Date; x.BranchId = f.BranchId; x.AdjustmentType = f.AdjustmentType == "Increase" ? "Increase" : "Decrease"; x.AdjustmentQuantity = f.AdjustmentQuantity; x.Reason = f.Reason.Trim(); x.Remarks = f.Remarks?.Trim(); x.UpdatedAt = now;
            x.WarehouseStockId = target is WarehouseStock w ? w.Id : null; x.DisplayStockId = target is DisplayStock d ? d.Id : null; x.TankId = target is Tank t ? t.Id : null;
            x.ProductId = target is WarehouseStock ww ? ww.ProductId : target is DisplayStock dd ? dd.ProductId : null; x.BatchId = target is WarehouseStock wb ? wb.BatchId : target is DisplayStock db ? db.BatchId : null; x.FuelId = target is Tank ft ? ft.FuelId : null;
        }
        private static void ApplyPostedAdjustment(StockAdjustment x, object target, int userId, DateTime now)
        {
            var before = TargetQuantity(target); var signed = x.AdjustmentType == "Increase" ? x.AdjustmentQuantity : -x.AdjustmentQuantity; var after = before + signed;
            if (x.AdjustmentQuantity <= 0 || after < 0) throw new InvalidOperationException("Adjustment would create an invalid negative balance.");
            if (target is Tank tank && tank.CapacityLiters > 0 && after > tank.CapacityLiters) throw new InvalidOperationException("Adjustment would exceed tank capacity.");
            if (target is WarehouseStock w) { w.Quantity = after; w.UpdatedAt = now; } else if (target is DisplayStock d) { d.Quantity = after; d.UpdatedAt = now; } else { var t = (Tank)target; t.CurrentLiters = after; t.UpdatedAt = now; }
            x.BeforeQuantity = before; x.SignedQuantity = signed; x.AfterQuantity = after; x.Status = "Posted"; x.PostedBy = userId; x.PostedAt = now; x.UpdatedAt = now;
        }
        private async Task<string> GenerateAdjustmentNoAsync(DateTime now)
        {
            var prefix = $"SA-{now:yyyyMMdd}-"; var last = await _db.StockAdjustments.Where(x => x.AdjustmentNo.StartsWith(prefix)).OrderByDescending(x => x.Id).Select(x => x.AdjustmentNo).FirstOrDefaultAsync();
            var sequence = last != null && int.TryParse(last[(last.LastIndexOf('-') + 1)..], out var n) ? n + 1 : 1; return $"{prefix}{sequence:000000}";
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

        private async Task ValidateBranchAsync(int branchId)
        {
            if (branchId <= 0 || !await _db.Branches.AsNoTracking().AnyAsync(branch => branch.Id == branchId && branch.Status == 1))
            {
                throw new InvalidOperationException("Branch is required.");
            }
        }

        private async Task<int?> CurrentUserBranchIdAsync(int userId)
        {
            return await _db.Users
                .AsNoTracking()
                .Where(user => user.Id == userId)
                .Select(user => user.BranchId)
                .FirstOrDefaultAsync();
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
            public decimal Cost { get; init; }
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
            public decimal Cost { get; init; }
            public FuelSale FuelSale { get; init; } = new();
        }

        private sealed record VoucherRedemptionResult(Voucher? Voucher, VoucherRule Rule, decimal DiscountAmount);
        private sealed record PosUserContext(int UserId, int BranchId, string BranchName);
    }
}

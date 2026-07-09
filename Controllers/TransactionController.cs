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
        private readonly ApplicationDbContext _db;
        private readonly FinancialMetricsService _financialMetrics;
        private readonly ProductBatchNumberService _batchNumberService;

        public TransactionController(ApplicationDbContext db, FinancialMetricsService financialMetrics, ProductBatchNumberService batchNumberService)
        {
            _db = db;
            _financialMetrics = financialMetrics;
            _batchNumberService = batchNumberService;
        }

        public async Task<IActionResult> POS()
        {
            return View(await BuildPosPageAsync());
        }

        private async Task<PosPageViewModel> BuildPosPageAsync()
        {
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
                    && stock.Batch.IsActive)
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

            var fuels = await _db.Fuels
                .AsNoTracking()
                .Where(fuel => fuel.Status == 1 && fuel.IsActive)
                .OrderBy(fuel => fuel.Name)
                .ToListAsync();
            var tanks = await _db.Tanks
                .AsNoTracking()
                .Where(tank => tank.Status == 1 && tank.IsActive && tank.CurrentLiters > 0)
                .OrderBy(tank => tank.Id)
                .ToListAsync();
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
                Fuels = fuels
                    .Select(fuel =>
                    {
                        var tank = tanks.FirstOrDefault(item => item.FuelId == fuel.Id);
                        return tank is null
                            ? null
                            : new PosFuelOptionViewModel
                            {
                                FuelId = fuel.Id,
                                TankId = tank.Id,
                                TankNo = tank.TankNo,
                                Name = fuel.Name,
                                AvailableLiters = tank.CurrentLiters,
                                Price = fuel.CurrentPricePerLiter
                            };
                    })
                    .Where(option => option is not null)
                    .Select((option, index) =>
                    {
                        option!.IsChecked = index == 0;
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
                        voucherRedemption = await ValidateAndCalculateVoucherRedemption(voucherCode, member, productTotal, fuelTotal, now);
                        voucherDiscountAmount = voucherRedemption.DiscountAmount;
                    }

                    discountAmount = memberDiscountAmount + voucherDiscountAmount;
                    pointsRequired = 0m;
                    rebateAmount = 0m;
                }

                var saleBranchId = await CurrentUserBranchIdAsync(userId.Value);
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

        public IActionResult DailyCash() => View();
        public IActionResult CashIn() => View();
        public IActionResult CashOut() => View();
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

            if (form.CostPerLiter < 0)
            {
                ModelState.AddModelError("Form.CostPerLiter", "Cost/Liter must be greater than or equal to 0.");
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
            delivery.BranchId = form.BranchId;
            delivery.SupplierId = form.SupplierId;
            delivery.FuelId = form.FuelId;
            delivery.TankId = form.TankId;
            delivery.DeliveredLiters = form.Liters;
            delivery.CostPerLiter = form.CostPerLiter;
            delivery.TotalCost = form.Liters * form.CostPerLiter;
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
            await CreateFuelBatchForDelivery(delivery, tank, tank.Fuel?.CurrentPricePerLiter ?? 0m, now);
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
                await CreateFuelBatchForDelivery(delivery, delivery.Tank, delivery.Fuel?.CurrentPricePerLiter ?? 0m, now);
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
        public async Task<IActionResult> WarehouseToDisplayProduct(string? search) => View(await BuildStockTransferPage("WarehouseToDisplayProduct", "Warehouse to Display Product", nameof(SaveWarehouseToDisplayProduct), nameof(CompleteWarehouseToDisplayProduct), nameof(CancelWarehouseToDisplayProduct), search));
        public async Task<IActionResult> DisplayToWarehouseProduct(string? search) => View(await BuildStockTransferPage("DisplayToWarehouseProduct", "Display to Warehouse Product", nameof(SaveDisplayToWarehouseProduct), nameof(CompleteDisplayToWarehouseProduct), nameof(CancelDisplayToWarehouseProduct), search));
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
        public IActionResult DisplayStockAdjustment() => View();
        public IActionResult WarehouseStockAdjustment() => View();
        public IActionResult TankFuelAdjustment() => View();
        public async Task<IActionResult> ProductPriceAdjustment(string? search)
        {
            return View(await BuildProductPriceAdjustmentPageAsync(search));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProductPriceAdjustment([Bind(Prefix = "ProductPriceHistoryForm")] ProductPriceHistoryForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("ProductPriceAdjustment", await BuildProductPriceAdjustmentPageAsync(search, form, "productPriceAdjustmentModal"));
            }

            var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(item => item.Id == form.ProductId && item.Status == 1 && item.IsActive);
            if (product is null)
            {
                ModelState.AddModelError("ProductPriceHistoryForm.ProductId", "Select a product.");
                return View("ProductPriceAdjustment", await BuildProductPriceAdjustmentPageAsync(search, form, "productPriceAdjustmentModal"));
            }

            var batch = await ResolveProductPriceBatchAsync(form.ProductId, form.BatchId);
            if (batch is null)
            {
                ModelState.AddModelError("ProductPriceHistoryForm.BatchId", "No active batch was found for this product.");
                return View("ProductPriceAdjustment", await BuildProductPriceAdjustmentPageAsync(search, form, "productPriceAdjustmentModal"));
            }

            var newPrice = form.NewPrice!.Value;
            if (newPrice == batch.SellingPrice)
            {
                ModelState.AddModelError("ProductPriceHistoryForm.NewPrice", "New Selling Price must be different from the current price.");
                form.CurrentPrice = batch.SellingPrice;
                return View("ProductPriceAdjustment", await BuildProductPriceAdjustmentPageAsync(search, form, "productPriceAdjustmentModal"));
            }

            var now = DateTime.UtcNow;
            var history = new ProductPriceHistory
            {
                ProductId = form.ProductId,
                BatchId = batch.Id,
                OldPrice = batch.SellingPrice,
                NewPrice = newPrice,
                EffectiveDate = form.EffectiveDate!.Value,
                Remarks = CleanOptional(form.Remarks),
                CreatedBy = CurrentUserId(),
                Status = 1,
                CreatedAt = now,
                UpdatedAt = now
            };

            batch.SellingPrice = newPrice;
            batch.UpdatedAt = now;
            _db.ProductPriceHistory.Add(history);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ProductPriceAdjustment), new { search });
        }

        public async Task<IActionResult> FuelPriceAdjustment(string? search)
        {
            return View(await BuildFuelPriceAdjustmentPageAsync(search));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFuelPriceAdjustment([Bind(Prefix = "FuelPriceHistoryForm")] FuelPriceHistoryForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("FuelPriceAdjustment", await BuildFuelPriceAdjustmentPageAsync(search, form, "fuelPriceAdjustmentModal"));
            }

            var fuel = await _db.Fuels.FindAsync(form.FuelId);
            if (fuel is null || fuel.Status != 1 || !fuel.IsActive)
            {
                ModelState.AddModelError("FuelPriceHistoryForm.FuelId", "Select a fuel.");
                return View("FuelPriceAdjustment", await BuildFuelPriceAdjustmentPageAsync(search, form, "fuelPriceAdjustmentModal"));
            }

            var now = DateTime.UtcNow;
            var history = new FuelPriceHistory
            {
                FuelId = form.FuelId,
                OldPrice = fuel.CurrentPricePerLiter,
                NewPrice = form.NewPrice!.Value,
                EffectiveAt = form.EffectiveAt!.Value,
                Remarks = CleanOptional(form.Remarks),
                CreatedBy = CurrentUserId(),
                CreatedAt = now,
                UpdatedAt = now
            };

            fuel.CurrentPricePerLiter = history.NewPrice;
            fuel.UpdatedAt = now;
            _db.FuelPriceHistory.Add(history);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(FuelPriceAdjustment), new { search });
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
                        TankLitersBefore = before,
                        TankLitersAfter = after,
                        Date = item.DeliveryDate,
                        Status = item.Status,
                        Remarks = item.Remarks ?? string.Empty
                    };
                }).ToList()
            };
        }

        private async Task<SetupModulesPageViewModel> BuildProductPriceAdjustmentPageAsync(string? search, ProductPriceHistoryForm? form = null, string activeModalId = "")
        {
            IQueryable<ProductPriceHistory> query = _db.ProductPriceHistory
                .AsNoTracking()
                .Include(history => history.Product)
                .Include(history => history.Batch);
            var searchText = (search ?? string.Empty).Trim();

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
                ActiveModalId = activeModalId,
                ProductPriceHistoryForm = form ?? new ProductPriceHistoryForm(),
                ProductOptions = await BuildProductOptionsAsync(),
                ProductBatches = await BuildActiveProductPriceBatchesAsync(),
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

        private async Task<SetupModulesPageViewModel> BuildFuelPriceAdjustmentPageAsync(string? search, FuelPriceHistoryForm? form = null, string activeModalId = "")
        {
            IQueryable<FuelPriceHistory> query = _db.FuelPriceHistory.AsNoTracking().Include(history => history.Fuel);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(history => history.Fuel != null && history.Fuel.Name.Contains(searchText));
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
                ActiveModalId = activeModalId,
                FuelPriceHistoryForm = form ?? new FuelPriceHistoryForm(),
                FuelOptions = await BuildFuelOptionsAsync(),
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

        private async Task CreateFuelBatchForDelivery(FuelDelivery delivery, Tank tank, decimal sellingPricePerLiter, DateTime now)
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
                SellingPricePerLiter = sellingPricePerLiter,
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
                    && (transfer.TransferType != "BranchToBranchProduct" || stock.BranchId == form.SourceBranchId))
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
                .Where(stock => stock.ProductId == form.ProductId!.Value && stock.Quantity > 0 && stock.Batch != null)
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
                        stock.ProductId,
                        ProductName = stock.Product!.Name,
                        CategoryName = stock.Product.Category != null ? stock.Product.Category.Name : "-",
                        UnitName = stock.Product.ProductUnit != null ? stock.Product.ProductUnit.Name : "-"
                    })
                    .OrderBy(group => group.Key.ProductName)
                    .Select(group => new StockTransferProductSelectorRowViewModel
                    {
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
                    stock.ProductId,
                    ProductName = stock.Product!.Name,
                    CategoryName = stock.Product.Category != null ? stock.Product.Category.Name : "-",
                    UnitName = stock.Product.ProductUnit != null ? stock.Product.ProductUnit.Name : "-"
                })
                .OrderBy(group => group.Key.ProductName)
                .Select(group => new StockTransferProductSelectorRowViewModel
                {
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

                var price = tank.Fuel.CurrentPricePerLiter;
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
                        NozzleId = null,
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
    }
}

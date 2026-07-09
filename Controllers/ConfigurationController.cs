using gpos.Data;
using gpos.Filters;
using gpos.Models;
using gpos.Models.ViewModels;
using gpos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public partial class ConfigurationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ProductBatchNumberService _batchNumberService;
        private static readonly string[] VatTypeValues = { "Inclusive", "Exclusive", "Exempt", "ZeroRated" };

        public ConfigurationController(ApplicationDbContext db, ProductBatchNumberService batchNumberService)
        {
            _db = db;
            _batchNumberService = batchNumberService;
        }

        public IActionResult Products() => View();

        public async Task<IActionResult> SearchBranches(string? search, int take = 20)
        {
            var searchText = (search ?? string.Empty).Trim();
            var resultLimit = Math.Clamp(take, 1, 50);
            var query = _db.Branches.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(branch => branch.Name.Contains(searchText)
                    || (branch.Address != null && branch.Address.Contains(searchText))
                    || branch.Id.ToString().Contains(searchText));
            }

            var branches = await query
                .OrderByDescending(branch => branch.Status == 1)
                .ThenBy(branch => branch.Name)
                .Take(resultLimit)
                .Select(branch => new
                {
                    id = branch.Id,
                    name = branch.Name,
                    code = branch.Id.ToString(),
                    address = branch.Address,
                    status = branch.Status == 1 ? "Active" : "Inactive",
                    isActive = branch.Status == 1
                })
                .ToListAsync();

            return Json(branches);
        }

        [HttpGet]
        public async Task<IActionResult> SearchStockProducts(string? search, int take = 20)
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
                query = query.Where(product => product.Name.Contains(searchText)
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

            return Json(products);
        }

        [HttpGet]
        public async Task<IActionResult> SearchStockBatches(string? search, int? productId, int take = 20)
        {
            var searchText = (search ?? string.Empty).Trim();
            var resultLimit = Math.Clamp(take, 1, 50);
            var query = _db.ProductBatches
                .AsNoTracking()
                .Include(batch => batch.Product)
                .Include(batch => batch.Supplier)
                .Where(batch => batch.Status == 1 && batch.IsActive)
                .AsQueryable();

            if (productId.HasValue && productId.Value > 0)
            {
                query = query.Where(batch => batch.ProductId == productId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(batch => batch.BatchNo.Contains(searchText)
                    || (batch.Product != null && batch.Product.Name.Contains(searchText))
                    || (batch.Supplier != null && batch.Supplier.Name.Contains(searchText)));
            }

            var batches = await query
                .OrderBy(batch => batch.BatchNo)
                .Take(resultLimit)
                .Select(batch => new
                {
                    batchId = batch.Id,
                    batchNumber = batch.BatchNo,
                    productId = batch.ProductId,
                    productName = batch.Product != null ? batch.Product.Name : "-",
                    supplierName = batch.Supplier != null ? batch.Supplier.Name : "-",
                    costPrice = batch.CostPrice,
                    sellingPrice = batch.SellingPrice,
                    expiryDate = batch.ExpiryDate.HasValue ? batch.ExpiryDate.Value.ToString("yyyy-MM-dd") : "-"
                })
                .ToListAsync();

            return Json(batches);
        }

        public async Task<IActionResult> DisplayProducts(string? search, int? branchId, int? filterBranchId, int? editId)
        {
            branchId = filterBranchId ?? branchId;
            return View(await BuildDisplayProductsPageAsync(search, branchId: branchId, editId: editId, activeModalId: editId.HasValue ? "displayProductModal" : string.Empty));
        }

        public async Task<IActionResult> WarehouseProducts(string? search, int? branchId, int? filterBranchId, int? editId)
        {
            branchId = filterBranchId ?? branchId;
            return View(await BuildWarehouseProductsPageAsync(search, branchId: branchId, editId: editId, activeModalId: editId.HasValue ? "warehouseProductModal" : string.Empty));
        }

        public async Task<IActionResult> WarehouseBatches(string? search, int? branchId, int? filterBranchId)
        {
            branchId = filterBranchId ?? branchId;
            return View(await BuildWarehouseBatchesPageAsync(search, branchId));
        }

        public async Task<IActionResult> DisplayBatches(string? search, int? branchId, int? filterBranchId)
        {
            branchId = filterBranchId ?? branchId;
            return View(await BuildDisplayBatchesPageAsync(search, branchId));
        }

        public async Task<IActionResult> ProductCategories(string? search)
        {
            return View(await BuildProductCategoriesPageAsync(search));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDisplayProduct([Bind(Prefix = "StockForm")] ProductStockForm form, string? search, int? filterBranchId)
        {
            await ValidateProductStockFormAsync(form, isEdit: form.Id.HasValue && form.Id.Value > 0, isDisplay: true);
            await PopulateProductStockFormDisplayNamesAsync(form);
            if (!ModelState.IsValid)
            {
                return View("DisplayProducts", await BuildDisplayProductsPageAsync(search, form, "displayProductModal", filterBranchId));
            }

            var now = DateTime.UtcNow;

            if (form.Id.HasValue && form.Id.Value > 0)
            {
                var displayStock = await _db.DisplayStocks
                    .Include(stock => stock.Product)
                    .Include(stock => stock.Batch)
                    .FirstOrDefaultAsync(stock => stock.Id == form.Id.Value);

                if (displayStock is null)
                {
                    ModelState.AddModelError(string.Empty, "Display product was not found.");
                    return View("DisplayProducts", await BuildDisplayProductsPageAsync(search, form, "displayProductModal", filterBranchId));
                }

                displayStock.BranchId = form.BranchId;
                displayStock.ProductId = form.ProductId!.Value;
                displayStock.BatchId = form.BatchId!.Value;
                displayStock.UpdatedAt = now;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(DisplayProducts), new { search, filterBranchId });
            }

            _db.DisplayStocks.Add(new DisplayStock
            {
                BranchId = form.BranchId,
                ProductId = form.ProductId!.Value,
                BatchId = form.BatchId!.Value,
                Quantity = form.Quantity!.Value,
                CreatedAt = now,
                UpdatedAt = now
            });

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(DisplayProducts), new { search, filterBranchId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveWarehouseProduct([Bind(Prefix = "StockForm")] ProductStockForm form, string? search, int? filterBranchId)
        {
            await ValidateProductStockFormAsync(form, isEdit: form.Id.HasValue && form.Id.Value > 0, isDisplay: false);
            await PopulateProductStockFormDisplayNamesAsync(form);
            if (!ModelState.IsValid)
            {
                return View("WarehouseProducts", await BuildWarehouseProductsPageAsync(search, form, "warehouseProductModal", filterBranchId));
            }

            var now = DateTime.UtcNow;

            if (form.Id.HasValue && form.Id.Value > 0)
            {
                var warehouseStock = await _db.WarehouseStocks.FirstOrDefaultAsync(stock => stock.Id == form.Id.Value);

                if (warehouseStock is null)
                {
                    ModelState.AddModelError(string.Empty, "Warehouse product was not found.");
                    return View("WarehouseProducts", await BuildWarehouseProductsPageAsync(search, form, "warehouseProductModal", filterBranchId));
                }

                warehouseStock.BranchId = form.BranchId;
                warehouseStock.ProductId = form.ProductId!.Value;
                warehouseStock.BatchId = form.BatchId!.Value;
                warehouseStock.UpdatedAt = now;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(WarehouseProducts), new { search, filterBranchId });
            }

            _db.WarehouseStocks.Add(new WarehouseStock
            {
                BranchId = form.BranchId,
                ProductId = form.ProductId!.Value,
                BatchId = form.BatchId!.Value,
                Quantity = form.Quantity!.Value,
                CreatedAt = now,
                UpdatedAt = now
            });

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(WarehouseProducts), new { search, filterBranchId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProductCategory([Bind(Prefix = "CategoryForm")] ProductCategoryForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("ProductCategories", await BuildProductCategoriesPageAsync(search, form, "productCategoryModal"));
            }

            var now = DateTime.UtcNow;
            _db.ProductCategories.Add(new ProductCategory
            {
                Name = form.Name.Trim(),
                Description = form.Description.Trim(),
                IsActive = form.IsActive,
                Status = form.IsActive ? 1 : 0,
                CreatedAt = now,
                UpdatedAt = now
            });

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product category saved successfully.";
            return RedirectToAction(nameof(ProductCategories), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProductCategory([Bind(Prefix = "CategoryForm")] ProductCategoryForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("ProductCategories", await BuildProductCategoriesPageAsync(search, form, "editCategoryModal"));
            }

            var category = await _db.ProductCategories.FindAsync(form.Id);

            if (category is null)
            {
                ModelState.AddModelError(string.Empty, "Product category was not found.");
                return View("ProductCategories", await BuildProductCategoriesPageAsync(search, form, "editCategoryModal"));
            }

            category.Name = form.Name.Trim();
            category.Description = form.Description.Trim();
            category.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product category updated successfully.";
            return RedirectToAction(nameof(ProductCategories), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductCategory(int id, string? search)
        {
            var category = await _db.ProductCategories.FindAsync(id);

            if (category is not null)
            {
                category.IsActive = false;
                category.Status = 0;
                category.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product category disabled successfully.";
            }

            return RedirectToAction(nameof(ProductCategories), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateProductCategory(int id, string? search)
        {
            var category = await _db.ProductCategories.FindAsync(id);
            if (category is not null)
            {
                category.IsActive = true;
                category.Status = 1;
                category.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product category activated successfully.";
            }
            return RedirectToAction(nameof(ProductCategories), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateProductFromStock(int id, string source, string? search)
        {
            var product = await _db.Products.FindAsync(id);
            if (product is not null)
            {
                product.IsActive = false;
                product.Status = 0;
                product.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            return RedirectToStockSource(source, search);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateProductFromStock(int id, string source, string? search)
        {
            var product = await _db.Products.FindAsync(id);
            if (product is not null)
            {
                product.IsActive = true;
                product.Status = 1;
                product.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            return RedirectToStockSource(source, search);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateWarehouseBatch(int id, string? search, int? filterBranchId)
        {
            await SetProductBatchActiveStateAsync(id, isActive: false);
            return RedirectToAction(nameof(WarehouseBatches), new { search, filterBranchId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateWarehouseBatch(int id, string? search, int? filterBranchId)
        {
            await SetProductBatchActiveStateAsync(id, isActive: true);
            return RedirectToAction(nameof(WarehouseBatches), new { search, filterBranchId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateDisplayBatch(int id, string? search, int? filterBranchId)
        {
            await SetProductBatchActiveStateAsync(id, isActive: false);
            return RedirectToAction(nameof(DisplayBatches), new { search, filterBranchId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateDisplayBatch(int id, string? search, int? filterBranchId)
        {
            await SetProductBatchActiveStateAsync(id, isActive: true);
            return RedirectToAction(nameof(DisplayBatches), new { search, filterBranchId });
        }

        private async Task SetProductBatchActiveStateAsync(int id, bool isActive)
        {
            var batch = await _db.ProductBatches.FindAsync(id);
            if (batch is null)
            {
                return;
            }

            batch.Status = isActive ? 1 : 0;
            batch.IsActive = isActive;
            batch.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        private IActionResult RedirectToStockSource(string source, string? search)
        {
            return string.Equals(source, "Warehouse", StringComparison.OrdinalIgnoreCase)
                ? RedirectToAction(nameof(WarehouseProducts), new { search })
                : RedirectToAction(nameof(DisplayProducts), new { search });
        }


        public IActionResult ItemUnits() => View();
        public IActionResult FuelTypes() => View();
        public IActionResult FuelPrices() => View();
        public IActionResult PumpUnits() => View();
        public async Task<IActionResult> Discounts(string? search, int? editId)
        {
            return View(await BuildDiscountsPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "discountModal" : ""));
        }

        public async Task<IActionResult> Earnings(string? search, int? editId)
        {
            return View(await BuildEarningsPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "earningsModal" : ""));
        }

        public async Task<IActionResult> Members(string? search, int? editId)
        {
            return View(await BuildMembersPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "memberModal" : ""));
        }

        public async Task<IActionResult> Vat(string? search, int? editId)
        {
            await EnsureDefaultVatSettingAsync();
            return View(await BuildVatPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "vatModal" : ""));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveVat([Bind(Prefix = "VatForm")] VatSettingForm form, string? search)
        {
            ValidateVatForm(form);

            if (!ModelState.IsValid)
            {
                return View("Vat", await BuildVatPageAsync(search, form, activeModalId: "vatModal"));
            }

            var now = DateTime.UtcNow;
            var vat = new VatSetting
            {
                Name = form.Name.Trim(),
                Rate = form.Rate,
                Type = form.Type.Trim(),
                IsDefault = form.IsDefault,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            _db.VatSettings.Add(vat);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "VAT setting saved successfully.";
            return RedirectToAction(nameof(Vat), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableVat(int id, string? search)
        {
            var vat = await _db.VatSettings.FirstOrDefaultAsync(setting => setting.Id == id);

            if (vat is null)
            {
                return NotFound();
            }

            _db.VatSettings.Add(CreateVatVersion(vat, isActive: false, isDefault: false, DateTime.UtcNow));
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "VAT setting disabled successfully.";
            return RedirectToAction(nameof(Vat), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateVat(int id, string? search)
        {
            var vat = await _db.VatSettings.FirstOrDefaultAsync(setting => setting.Id == id);

            if (vat is null)
            {
                return NotFound();
            }

            _db.VatSettings.Add(CreateVatVersion(vat, isActive: true, isDefault: vat.IsDefault, DateTime.UtcNow));
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "VAT setting activated successfully.";
            return RedirectToAction(nameof(Vat), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefaultVat(int id, string? search)
        {
            var vat = await _db.VatSettings.FirstOrDefaultAsync(setting => setting.Id == id);

            if (vat is null)
            {
                return NotFound();
            }

            if (!vat.IsActive)
            {
                TempData["ErrorMessage"] = "Only active VAT settings can be set as default.";
                return RedirectToAction(nameof(Vat), new { search });
            }

            _db.VatSettings.Add(CreateVatVersion(vat, isActive: true, isDefault: true, DateTime.UtcNow));
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Default VAT updated successfully.";
            return RedirectToAction(nameof(Vat), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDiscount([Bind(Prefix = "DiscountForm")] DiscountForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("Discounts", await BuildDiscountsPageAsync(search, form, activeModalId: "discountModal"));
            }

            var now = DateTime.UtcNow;
            var discount = form.Id > 0 ? await _db.Discounts.FindAsync(form.Id) : new Discount { CreatedAt = now };

            if (discount is null)
            {
                return NotFound();
            }

            discount.Name = form.Name.Trim();
            discount.Status = form.Status;
            discount.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.Discounts.Add(discount);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Discounts), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDiscount(int id, string? search)
        {
            var discount = await _db.Discounts.FindAsync(id);

            if (discount is not null)
            {
                discount.Status = 0;
                discount.UpdatedAt = DateTime.UtcNow;
                TempData["DiscountSetupFeedback"] = "Discount disabled.";
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Discounts), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateDiscount(int id, string? search)
        {
            var discount = await _db.Discounts.FindAsync(id);
            if (discount is not null)
            {
                discount.Status = 1;
                discount.UpdatedAt = DateTime.UtcNow;
                TempData["DiscountSetupFeedback"] = "Discount activated.";
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Discounts), new { search });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEarnings([Bind(Prefix = "EarningsForm")] EarningsForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("Earnings", await BuildEarningsPageAsync(search, form, activeModalId: "earningsModal"));
            }

            var name = form.Name.Trim();
            var hasDuplicateName = await _db.Earnings.AnyAsync(earnings => earnings.Id != form.Id && earnings.Name == name);

            if (hasDuplicateName)
            {
                ModelState.AddModelError("EarningsForm.Name", "Earnings already exists.");
                return View("Earnings", await BuildEarningsPageAsync(search, form, activeModalId: "earningsModal"));
            }

            var now = DateTime.UtcNow;
            var earnings = form.Id > 0 ? await _db.Earnings.FindAsync(form.Id) : new Earnings { CreatedAt = now };

            if (earnings is null)
            {
                return NotFound();
            }

            earnings.Name = name;
            earnings.Description = string.IsNullOrWhiteSpace(form.Description) ? null : form.Description.Trim();
            earnings.Status = form.Status;
            earnings.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.Earnings.Add(earnings);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Earnings), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEarnings(int id, string? search)
        {
            var earnings = await _db.Earnings.FindAsync(id);

            if (earnings is not null)
            {
                earnings.Status = 0;
                earnings.UpdatedAt = DateTime.UtcNow;
                TempData["DiscountSetupFeedback"] = "Earnings disabled.";
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Earnings), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateEarnings(int id, string? search)
        {
            var earnings = await _db.Earnings.FindAsync(id);
            if (earnings is not null)
            {
                earnings.Status = 1;
                earnings.UpdatedAt = DateTime.UtcNow;
                TempData["DiscountSetupFeedback"] = "Earnings activated.";
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Earnings), new { search });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveMember([Bind(Prefix = "MemberForm")] MemberForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("Members", await BuildMembersPageAsync(search, form, activeModalId: "memberModal"));
            }

            if (form.DiscountId.HasValue)
            {
                var discountExists = await _db.Discounts.AnyAsync(discount => discount.Id == form.DiscountId.Value && discount.Status == 1);

                if (!discountExists)
                {
                    ModelState.AddModelError("MemberForm.DiscountId", "Select an active discount.");
                    return View("Members", await BuildMembersPageAsync(search, form, activeModalId: "memberModal"));
                }
            }

            if (form.EarningsId.HasValue)
            {
                var earningsExists = await _db.Earnings.AnyAsync(earnings => earnings.Id == form.EarningsId.Value && earnings.Status == 1);

                if (!earningsExists)
                {
                    ModelState.AddModelError("MemberForm.EarningsId", "Select active earnings.");
                    return View("Members", await BuildMembersPageAsync(search, form, activeModalId: "memberModal"));
                }
            }

            var memberNo = form.MemberNo.Trim();
            var cardNo = string.IsNullOrWhiteSpace(form.CardNo) ? null : form.CardNo.Trim();
            var hasDuplicateMemberNo = await _db.Members.AnyAsync(member => member.Id != form.Id && member.MemberNo == memberNo);

            if (hasDuplicateMemberNo)
            {
                ModelState.AddModelError("MemberForm.MemberNo", "Member No already exists.");
                return View("Members", await BuildMembersPageAsync(search, form, activeModalId: "memberModal"));
            }

            if (!string.IsNullOrWhiteSpace(cardNo))
            {
                var hasDuplicateCardNo = await _db.Members.AnyAsync(member => member.Id != form.Id && member.CardNo == cardNo);

                if (hasDuplicateCardNo)
                {
                    ModelState.AddModelError("MemberForm.CardNo", "Card No already exists.");
                    return View("Members", await BuildMembersPageAsync(search, form, activeModalId: "memberModal"));
                }
            }

            var now = DateTime.UtcNow;
            var member = form.Id > 0 ? await _db.Members.FindAsync(form.Id) : new Member { CreatedAt = now };

            if (member is null)
            {
                return NotFound();
            }

            member.MemberNo = memberNo;
            member.CardNo = cardNo;
            member.FullName = form.FullName.Trim();
            member.ContactNumber = string.IsNullOrWhiteSpace(form.ContactNumber) ? null : form.ContactNumber.Trim();
            member.Email = string.IsNullOrWhiteSpace(form.Email) ? null : form.Email.Trim();
            member.Address = string.IsNullOrWhiteSpace(form.Address) ? null : form.Address.Trim();
            member.DiscountId = form.DiscountId;
            member.EarningsId = form.EarningsId;
            member.Points = form.Points;
            member.Status = form.Status;
            member.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.Members.Add(member);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Members), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMember(int id, string? search)
        {
            var member = await _db.Members.FindAsync(id);

            if (member is not null)
            {
                member.Status = 0;
                member.UpdatedAt = DateTime.UtcNow;
                TempData["DiscountSetupFeedback"] = "Member disabled.";
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Members), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateMember(int id, string? search)
        {
            var member = await _db.Members.FindAsync(id);
            if (member is not null)
            {
                member.Status = 1;
                member.UpdatedAt = DateTime.UtcNow;
                TempData["DiscountSetupFeedback"] = "Member activated.";
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Members), new { search });
        }


        [NonAction]
        public async Task<IActionResult> Position(string? search, int? editId)
        {
            return View(await BuildPositionsPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "positionModal" : ""));
        }

        public async Task<IActionResult> Branch(string? search, int? editId)
        {
            return View(await BuildBranchesPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "branchModal" : ""));
        }

        public async Task<IActionResult> Department(string? search, int? editId)
        {
            return View(await BuildDepartmentsPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "departmentModal" : ""));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveBranch([Bind(Prefix = "BranchForm")] BranchForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("Branch", await BuildBranchesPageAsync(search, form, activeModalId: "branchModal"));
            }

            var now = DateTime.UtcNow;
            var branch = form.Id > 0 ? await _db.Branches.FindAsync(form.Id) : new Branch { CreatedAt = now };

            if (branch is null)
            {
                return NotFound();
            }

            branch.Name = form.Name.Trim();
            branch.Address = CleanOptional(form.Address);
            branch.Status = form.Status;
            branch.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.Branches.Add(branch);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Branch), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBranch(int id, string? search)
        {
            var branch = await _db.Branches.FindAsync(id);

            if (branch is not null)
            {
                var isUsed = await _db.Departments.AnyAsync(department => department.BranchId == id);

                if (isUsed)
                {
                    branch.Status = 0;
                    branch.UpdatedAt = DateTime.UtcNow;
                    TempData["ConfigSetupFeedback"] = "Branch disabled because departments are assigned to it.";
                }
                else
                {
                    branch.Status = 0;
                    branch.UpdatedAt = DateTime.UtcNow;
                    TempData["ConfigSetupFeedback"] = "Branch disabled.";
                }

                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Branch), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateBranch(int id, string? search)
        {
            var branch = await _db.Branches.FindAsync(id);
            if (branch is not null)
            {
                branch.Status = 1;
                branch.UpdatedAt = DateTime.UtcNow;
                TempData["ConfigSetupFeedback"] = "Branch activated.";
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Branch), new { search });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDepartment([Bind(Prefix = "DepartmentForm")] DepartmentForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("Department", await BuildDepartmentsPageAsync(search, form, activeModalId: "departmentModal"));
            }

            var branchExists = await _db.Branches.AnyAsync(branch => branch.Id == form.BranchId && branch.Status == 1);

            if (!branchExists)
            {
                ModelState.AddModelError("DepartmentForm.BranchId", "Select an active branch.");
                return View("Department", await BuildDepartmentsPageAsync(search, form, activeModalId: "departmentModal"));
            }

            var now = DateTime.UtcNow;
            var department = form.Id > 0 ? await _db.Departments.FindAsync(form.Id) : new Department { CreatedAt = now };

            if (department is null)
            {
                return NotFound();
            }

            department.BranchId = form.BranchId;
            department.Name = form.Name.Trim();
            department.Description = CleanOptional(form.Description);
            department.Status = form.Status;
            department.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.Departments.Add(department);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Department), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDepartment(int id, string? search)
        {
            var department = await _db.Departments.FindAsync(id);

            if (department is not null)
            {
                var isUsed = await _db.Users.AnyAsync(user => user.DepartmentId == id);

                if (isUsed)
                {
                    department.Status = 0;
                    department.UpdatedAt = DateTime.UtcNow;
                    TempData["ConfigSetupFeedback"] = "Department disabled because users are assigned to it.";
                }
                else
                {
                    department.Status = 0;
                    department.UpdatedAt = DateTime.UtcNow;
                    TempData["ConfigSetupFeedback"] = "Department disabled.";
                }

                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Department), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateDepartment(int id, string? search)
        {
            var department = await _db.Departments.FindAsync(id);
            if (department is not null)
            {
                department.Status = 1;
                department.UpdatedAt = DateTime.UtcNow;
                TempData["ConfigSetupFeedback"] = "Department activated.";
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Department), new { search });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonAction]
        public async Task<IActionResult> SavePosition([Bind(Prefix = "PositionForm")] PositionForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("Position", await BuildPositionsPageAsync(search, form, activeModalId: "positionModal"));
            }

            var now = DateTime.UtcNow;
            var position = form.Id > 0 ? await _db.Positions.FindAsync(form.Id) : new gpos.Models.Position { CreatedAt = now };

            if (position is null)
            {
                return NotFound();
            }

            position.Name = form.Name.Trim();
            position.Description = CleanOptional(form.Description);
            position.Status = form.Status;
            position.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.Positions.Add(position);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Position), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonAction]
        public async Task<IActionResult> DeletePosition(int id, string? search)
        {
            var position = await _db.Positions.FindAsync(id);

            if (position is not null)
            {
                position.Status = 0;
                position.UpdatedAt = DateTime.UtcNow;
                TempData["ConfigSetupFeedback"] = "Position disabled.";

                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Position), new { search });
        }

        [HttpGet]
        public async Task<IActionResult> Fuels(string? search, int? editId)
        {
            return View(await BuildFuelsPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "fuelModal" : ""));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFuel([Bind(Prefix = "FuelForm")] FuelForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("Fuels", await BuildFuelsPageAsync(search, form, activeModalId: "fuelModal"));
            }

            if (form.SupplierId.HasValue)
            {
                var supplierExists = await _db.Suppliers.AnyAsync(supplier => supplier.Id == form.SupplierId.Value && supplier.Status == 1);

                if (!supplierExists)
                {
                    ModelState.AddModelError("FuelForm.SupplierId", "Select a supplier.");
                    return View("Fuels", await BuildFuelsPageAsync(search, form, activeModalId: "fuelModal"));
                }
            }

            var now = DateTime.UtcNow;
            var fuel = form.Id > 0 ? await _db.Fuels.FindAsync(form.Id) : new Fuel { CreatedAt = now };

            if (fuel is null)
            {
                return NotFound();
            }

            fuel.Name = form.Name.Trim();
            fuel.Code = form.Code.Trim();
            fuel.SupplierId = form.SupplierId;
            fuel.CurrentPricePerLiter = form.CurrentPricePerLiter!.Value;
            fuel.IsActive = form.IsActive;
            fuel.Status = form.IsActive ? 1 : 0;
            fuel.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.Fuels.Add(fuel);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Fuels), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFuel(int id, string? search)
        {
            var fuel = await _db.Fuels.FindAsync(id);

            if (fuel is null)
            {
                return RedirectToAction(nameof(Fuels), new { search });
            }

            fuel.IsActive = false;
            fuel.Status = 0;
            fuel.UpdatedAt = DateTime.UtcNow;
            TempData["FuelSetupFeedback"] = "Fuel disabled.";
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Fuels), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateFuel(int id, string? search)
        {
            var fuel = await _db.Fuels.FindAsync(id);
            if (fuel is not null)
            {
                fuel.IsActive = true;
                fuel.Status = 1;
                fuel.UpdatedAt = DateTime.UtcNow;
                TempData["FuelSetupFeedback"] = "Fuel activated.";
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Fuels), new { search });
        }


        public async Task<IActionResult> Tanks(string? search, int? editId, int? branchId, int? filterBranchId)
        {
            branchId = filterBranchId ?? branchId;
            return View(await BuildTanksPageAsync(search, branchId, editId: editId, activeModalId: editId.HasValue ? "tankModal" : ""));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveTank([Bind(Prefix = "TankForm")] TankForm form, string? search, int? filterBranchId)
        {
            if (!ModelState.IsValid)
            {
                return View("Tanks", await BuildTanksPageAsync(search, filterBranchId, form, activeModalId: "tankModal"));
            }

            var fuelExists = await _db.Fuels.AnyAsync(fuel => fuel.Id == form.FuelId && fuel.Status == 1 && fuel.IsActive);

            if (!fuelExists)
            {
                ModelState.AddModelError("TankForm.FuelId", "Tank fuel is required.");
                return View("Tanks", await BuildTanksPageAsync(search, filterBranchId, form, activeModalId: "tankModal"));
            }

            if (!await _db.Branches.AnyAsync(branch => branch.Id == form.BranchId && branch.Status == 1))
            {
                ModelState.AddModelError("TankForm.BranchId", "Branch is required.");
                return View("Tanks", await BuildTanksPageAsync(search, filterBranchId, form, activeModalId: "tankModal"));
            }

            var now = DateTime.UtcNow;
            var tank = form.Id > 0 ? await _db.Tanks.FindAsync(form.Id) : new Tank { CreatedAt = now };

            if (tank is null)
            {
                return NotFound();
            }

            tank.FuelId = form.FuelId;
            tank.BranchId = form.BranchId;
            tank.TankNo = form.TankNo.Trim();
            tank.CapacityLiters = form.CapacityLiters!.Value;
            tank.CurrentLiters = form.CurrentLiters!.Value;
            tank.IsActive = form.IsActive;
            tank.Status = form.IsActive ? 1 : 0;
            tank.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.Tanks.Add(tank);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Tanks), new { search, filterBranchId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTank(int id, string? search)
        {
            var tank = await _db.Tanks.FindAsync(id);

            if (tank is null)
            {
                return RedirectToAction(nameof(Tanks), new { search });
            }

            tank.IsActive = false;
            tank.Status = 0;
            tank.UpdatedAt = DateTime.UtcNow;
            TempData["FuelSetupFeedback"] = "Tank disabled.";
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Tanks), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateTank(int id, string? search)
        {
            var tank = await _db.Tanks.FindAsync(id);
            if (tank is not null)
            {
                tank.IsActive = true;
                tank.Status = 1;
                tank.UpdatedAt = DateTime.UtcNow;
                TempData["FuelSetupFeedback"] = "Tank activated.";
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Tanks), new { search });
        }


        public async Task<IActionResult> Pumps(string? search, int? editId, int? branchId, int? filterBranchId)
        {
            branchId = filterBranchId ?? branchId;
            return View(await BuildPumpsPageAsync(search, branchId, editId: editId, activeModalId: editId.HasValue ? "pumpModal" : ""));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePump([Bind(Prefix = "PumpForm")] PumpForm form, string? search, int? filterBranchId)
        {
            if (!ModelState.IsValid)
            {
                return View("Pumps", await BuildPumpsPageAsync(search, filterBranchId, form, activeModalId: "pumpModal"));
            }

            if (!await _db.Branches.AnyAsync(branch => branch.Id == form.BranchId && branch.Status == 1))
            {
                ModelState.AddModelError("PumpForm.BranchId", "Branch is required.");
                return View("Pumps", await BuildPumpsPageAsync(search, filterBranchId, form, activeModalId: "pumpModal"));
            }

            var now = DateTime.UtcNow;
            var pump = form.Id > 0 ? await _db.Pumps.FindAsync(form.Id) : new Pump { CreatedAt = now };

            if (pump is null)
            {
                return NotFound();
            }

            pump.BranchId = form.BranchId;
            pump.Name = form.Name.Trim();
            pump.PumpNo = form.Name.Trim();
            pump.Status = form.Status;
            pump.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.Pumps.Add(pump);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Pumps), new { search, filterBranchId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePump(int id, string? search)
        {
            var pump = await _db.Pumps.FindAsync(id);

            if (pump is not null)
            {
                pump.Status = 0;
                pump.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                TempData["FuelSetupFeedback"] = "Dispenser deactivated.";
            }

            return RedirectToAction(nameof(Pumps), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivatePump(int id, string? search)
        {
            var pump = await _db.Pumps.FindAsync(id);
            if (pump is not null)
            {
                pump.Status = 1;
                pump.UpdatedAt = DateTime.UtcNow;
                TempData["FuelSetupFeedback"] = "Dispenser activated.";
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Pumps), new { search });
        }


        private async Task<ProductStockPageViewModel> BuildDisplayProductsPageAsync(string? search, ProductStockForm? form = null, string activeModalId = "", int? branchId = null, int? editId = null)
        {
            IQueryable<DisplayStock> query = _db.DisplayStocks.AsNoTracking()
                .Include(stock => stock.Product)
                .ThenInclude(product => product!.Category)
                .Include(stock => stock.Product)
                .ThenInclude(product => product!.ProductUnit)
                .Include(stock => stock.Branch);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(stock => (stock.Product != null && stock.Product.Name.Contains(searchText))
                    || (stock.Product != null && stock.Product.Category != null && stock.Product.Category.Name.Contains(searchText))
                    || (stock.Product != null && stock.Product.ProductUnit != null && stock.Product.ProductUnit.Name.Contains(searchText))
                    || (stock.Branch != null && stock.Branch.Name.Contains(searchText)));
            }

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(stock => stock.BranchId == branchId.Value);
            }

            var stockForm = form ?? await BuildProductStockFormAsync(isDisplay: true, editId);
            if (!stockForm.Id.HasValue && branchId.HasValue && branchId.Value > 0 && !stockForm.BranchId.HasValue)
            {
                stockForm.BranchId = branchId;
                stockForm.BranchName = await BranchNameAsync(branchId);
            }

            return new ProductStockPageViewModel
            {
                Search = searchText,
                BranchId = branchId,
                BranchName = await BranchNameAsync(branchId),
                ActiveModalId = activeModalId,
                StockForm = stockForm,
                BranchOptions = await BuildBranchFilterOptionsAsync(),
                CategoryOptions = await BuildCategoryOptionsAsync(),
                DisplayStockSummaries = await query
                    .Where(stock => stock.Quantity > 0)
                    .GroupBy(stock => new
                    {
                        stock.BranchId,
                        BranchName = stock.Branch != null ? stock.Branch.Name : "-",
                        stock.ProductId,
                        ProductName = stock.Product != null ? stock.Product.Name : "-",
                        CategoryName = stock.Product != null && stock.Product.Category != null ? stock.Product.Category.Name : "-",
                        UnitName = stock.Product != null && stock.Product.ProductUnit != null ? stock.Product.ProductUnit.Name : "-",
                        ProductIsActive = stock.Product != null && stock.Product.IsActive && stock.Product.Status == 1
                    })
                    .Select(group => new ProductStockSummaryViewModel
                    {
                        BranchId = group.Key.BranchId,
                        BranchName = group.Key.BranchName,
                        ProductId = group.Key.ProductId,
                        ProductName = group.Key.ProductName,
                        CategoryName = group.Key.CategoryName,
                        UnitName = group.Key.UnitName,
                        TotalQuantity = group.Sum(stock => stock.Quantity),
                        IsActive = group.Key.ProductIsActive
                    })
                    .OrderBy(summary => summary.BranchName)
                    .ThenBy(summary => summary.ProductName)
                    .ToListAsync()
            };
        }

        private async Task<ProductStockPageViewModel> BuildWarehouseProductsPageAsync(string? search, ProductStockForm? form = null, string activeModalId = "", int? branchId = null, int? editId = null)
        {
            IQueryable<WarehouseStock> query = _db.WarehouseStocks.AsNoTracking()
                .Include(stock => stock.Product)
                .ThenInclude(product => product!.Category)
                .Include(stock => stock.Product)
                .ThenInclude(product => product!.ProductUnit)
                .Include(stock => stock.Batch)
                .Include(stock => stock.Branch);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(stock => (stock.Product != null && stock.Product.Name.Contains(searchText))
                    || (stock.Product != null && stock.Product.Category != null && stock.Product.Category.Name.Contains(searchText))
                    || (stock.Product != null && stock.Product.ProductUnit != null && stock.Product.ProductUnit.Name.Contains(searchText))
                    || (stock.Branch != null && stock.Branch.Name.Contains(searchText)));
            }

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(stock => stock.BranchId == branchId.Value);
            }

            var stockForm = form ?? await BuildProductStockFormAsync(isDisplay: false, editId);
            if (!stockForm.Id.HasValue && branchId.HasValue && branchId.Value > 0 && !stockForm.BranchId.HasValue)
            {
                stockForm.BranchId = branchId;
                stockForm.BranchName = await BranchNameAsync(branchId);
            }

            return new ProductStockPageViewModel
            {
                Search = searchText,
                BranchId = branchId,
                BranchName = await BranchNameAsync(branchId),
                ActiveModalId = activeModalId,
                StockForm = stockForm,
                BranchOptions = await BuildBranchFilterOptionsAsync(),
                CategoryOptions = await BuildCategoryOptionsAsync(),
                WarehouseStockSummaries = await query
                    .Where(stock => stock.Quantity > 0)
                    .GroupBy(stock => new
                    {
                        stock.BranchId,
                        BranchName = stock.Branch != null ? stock.Branch.Name : "-",
                        stock.ProductId,
                        ProductName = stock.Product != null ? stock.Product.Name : "-",
                        CategoryName = stock.Product != null && stock.Product.Category != null ? stock.Product.Category.Name : "-",
                        UnitName = stock.Product != null && stock.Product.ProductUnit != null ? stock.Product.ProductUnit.Name : "-",
                        ProductIsActive = stock.Product != null && stock.Product.IsActive && stock.Product.Status == 1
                    })
                    .Select(group => new ProductStockSummaryViewModel
                    {
                        BranchId = group.Key.BranchId,
                        BranchName = group.Key.BranchName,
                        ProductId = group.Key.ProductId,
                        ProductName = group.Key.ProductName,
                        CategoryName = group.Key.CategoryName,
                        UnitName = group.Key.UnitName,
                        TotalQuantity = group.Sum(stock => stock.Quantity),
                        TotalCostValue = group.Sum(stock => stock.Quantity * stock.Batch!.CostPrice),
                        IsActive = group.Key.ProductIsActive
                    })
                    .OrderBy(summary => summary.BranchName)
                    .ThenBy(summary => summary.ProductName)
                    .ToListAsync()
            };
        }

        private async Task<ProductStockPageViewModel> BuildWarehouseBatchesPageAsync(string? search, int? branchId = null)
        {
            IQueryable<WarehouseStock> query = _db.WarehouseStocks.AsNoTracking()
                .Include(stock => stock.Branch)
                .Include(stock => stock.Product)
                .ThenInclude(product => product!.Category)
                .Include(stock => stock.Batch)
                .ThenInclude(batch => batch!.Supplier);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(stock => (stock.Branch != null && stock.Branch.Name.Contains(searchText))
                    || (stock.Product != null && stock.Product.Name.Contains(searchText))
                    || (stock.Product != null && stock.Product.Category != null && stock.Product.Category.Name.Contains(searchText))
                    || (stock.Batch != null && stock.Batch.BatchNo.Contains(searchText))
                    || (stock.Batch != null && stock.Batch.Supplier != null && stock.Batch.Supplier.Name.Contains(searchText)));
            }

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(stock => stock.BranchId == branchId.Value);
            }

            return new ProductStockPageViewModel
            {
                Search = searchText,
                BranchId = branchId,
                BranchName = await BranchNameAsync(branchId),
                BranchOptions = await BuildBranchFilterOptionsAsync(),
                WarehouseStocks = await query
                    .OrderBy(stock => stock.Branch != null ? stock.Branch.Name : string.Empty)
                    .ThenBy(stock => stock.Product != null ? stock.Product.Name : string.Empty)
                    .ThenBy(stock => stock.Batch != null ? stock.Batch.BatchNo : string.Empty)
                    .ThenBy(stock => stock.Id)
                    .ToListAsync()
            };
        }

        private async Task<ProductStockPageViewModel> BuildDisplayBatchesPageAsync(string? search, int? branchId = null)
        {
            IQueryable<DisplayStock> query = _db.DisplayStocks.AsNoTracking()
                .Include(stock => stock.Branch)
                .Include(stock => stock.Product)
                .ThenInclude(product => product!.Category)
                .Include(stock => stock.Batch)
                .ThenInclude(batch => batch!.Supplier);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(stock => (stock.Branch != null && stock.Branch.Name.Contains(searchText))
                    || (stock.Product != null && stock.Product.Name.Contains(searchText))
                    || (stock.Product != null && stock.Product.Category != null && stock.Product.Category.Name.Contains(searchText))
                    || (stock.Batch != null && stock.Batch.BatchNo.Contains(searchText))
                    || (stock.Batch != null && stock.Batch.Supplier != null && stock.Batch.Supplier.Name.Contains(searchText)));
            }

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(stock => stock.BranchId == branchId.Value);
            }

            return new ProductStockPageViewModel
            {
                Search = searchText,
                BranchId = branchId,
                BranchName = await BranchNameAsync(branchId),
                BranchOptions = await BuildBranchFilterOptionsAsync(),
                DisplayStocks = await query
                    .OrderBy(stock => stock.Branch != null ? stock.Branch.Name : string.Empty)
                    .ThenBy(stock => stock.Product != null ? stock.Product.Name : string.Empty)
                    .ThenBy(stock => stock.Batch != null ? stock.Batch.BatchNo : string.Empty)
                    .ThenBy(stock => stock.Id)
                    .ToListAsync()
            };
        }

        private async Task<ProductStockPageViewModel> BuildProductCategoriesPageAsync(string? search, ProductCategoryForm? form = null, string activeModalId = "")
        {
            IQueryable<ProductCategory> query = _db.ProductCategories.AsNoTracking();
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(category => category.Name.Contains(searchText) || category.Description.Contains(searchText));
            }

            return new ProductStockPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                CategoryForm = form ?? new ProductCategoryForm(),
                Categories = await query.OrderBy(category => category.Id).ToListAsync()
            };
        }

        private async Task<ProductStockForm> BuildProductStockFormAsync(bool isDisplay, int? editId)
        {
            if (!editId.HasValue)
            {
                return new ProductStockForm();
            }

            if (isDisplay)
            {
                var stock = await _db.DisplayStocks
                    .AsNoTracking()
                    .Include(item => item.Branch)
                    .Include(item => item.Product)
                    .Include(item => item.Batch)
                    .FirstOrDefaultAsync(item => item.Id == editId.Value);

                return stock is null ? new ProductStockForm() : ToProductStockForm(stock.Id, stock.BranchId, stock.Branch?.Name, stock.ProductId, stock.Product?.Name, stock.BatchId, stock.Batch?.BatchNo, stock.Quantity);
            }

            var warehouseStock = await _db.WarehouseStocks
                .AsNoTracking()
                .Include(item => item.Branch)
                .Include(item => item.Product)
                .Include(item => item.Batch)
                .FirstOrDefaultAsync(item => item.Id == editId.Value);

            return warehouseStock is null ? new ProductStockForm() : ToProductStockForm(warehouseStock.Id, warehouseStock.BranchId, warehouseStock.Branch?.Name, warehouseStock.ProductId, warehouseStock.Product?.Name, warehouseStock.BatchId, warehouseStock.Batch?.BatchNo, warehouseStock.Quantity);
        }

        private static ProductStockForm ToProductStockForm(int id, int? branchId, string? branchName, int productId, string? productName, int batchId, string? batchNo, decimal quantity)
        {
            return new ProductStockForm
            {
                Id = id,
                BranchId = branchId,
                BranchName = branchName ?? string.Empty,
                ProductId = productId,
                ProductDisplayName = productName ?? string.Empty,
                BatchId = batchId,
                BatchDisplayName = string.IsNullOrWhiteSpace(batchNo) ? string.Empty : $"{batchNo} - {productName}",
                Quantity = quantity
            };
        }

        private async Task ValidateProductStockFormAsync(ProductStockForm form, bool isEdit, bool isDisplay)
        {
            if (!form.BranchId.HasValue || form.BranchId.Value <= 0)
            {
                ModelState.AddModelError("StockForm.BranchId", "Branch is required.");
            }

            if (!form.ProductId.HasValue || form.ProductId.Value <= 0)
            {
                ModelState.AddModelError("StockForm.ProductId", "Product is required.");
            }

            if (!form.BatchId.HasValue || form.BatchId.Value <= 0)
            {
                ModelState.AddModelError("StockForm.BatchId", "Batch is required.");
            }

            if (!isEdit && (!form.Quantity.HasValue || form.Quantity.Value < 0))
            {
                ModelState.AddModelError("StockForm.Quantity", "Initial quantity is required and cannot be negative.");
            }

            if (!ModelState.IsValid)
            {
                return;
            }

            var branchExists = await _db.Branches.AnyAsync(branch => branch.Id == form.BranchId && branch.Status == 1);
            if (!branchExists)
            {
                ModelState.AddModelError("StockForm.BranchId", "Select a valid branch.");
            }

            var productExists = await _db.Products.AnyAsync(product => product.Id == form.ProductId && product.Status == 1 && product.IsActive);
            if (!productExists)
            {
                ModelState.AddModelError("StockForm.ProductId", "Select a valid product.");
            }

            var batchMatchesProduct = await _db.ProductBatches.AnyAsync(batch => batch.Id == form.BatchId && batch.ProductId == form.ProductId && batch.Status == 1 && batch.IsActive);
            if (!batchMatchesProduct)
            {
                ModelState.AddModelError("StockForm.BatchId", "Select a valid batch for the selected product.");
            }

            var duplicateExists = isDisplay
                ? await _db.DisplayStocks.AnyAsync(stock => stock.BranchId == form.BranchId
                    && stock.ProductId == form.ProductId
                    && stock.BatchId == form.BatchId
                    && stock.Id != form.Id)
                : await _db.WarehouseStocks.AnyAsync(stock => stock.BranchId == form.BranchId
                    && stock.ProductId == form.ProductId
                    && stock.BatchId == form.BatchId
                    && stock.Id != form.Id);

            if (duplicateExists)
            {
                ModelState.AddModelError("StockForm.BatchId", "A stock record already exists for this branch, product, and batch.");
            }
        }

        private async Task PopulateProductStockFormDisplayNamesAsync(ProductStockForm form)
        {
            form.BranchName = await BranchNameAsync(form.BranchId);

            if (form.ProductId.HasValue && form.ProductId.Value > 0)
            {
                form.ProductDisplayName = await _db.Products
                    .AsNoTracking()
                    .Where(product => product.Id == form.ProductId.Value)
                    .Select(product => product.Name)
                    .FirstOrDefaultAsync() ?? string.Empty;
            }

            if (form.BatchId.HasValue && form.BatchId.Value > 0)
            {
                form.BatchDisplayName = await _db.ProductBatches
                    .AsNoTracking()
                    .Include(batch => batch.Product)
                    .Where(batch => batch.Id == form.BatchId.Value)
                    .Select(batch => batch.BatchNo + " - " + (batch.Product != null ? batch.Product.Name : string.Empty))
                    .FirstOrDefaultAsync() ?? string.Empty;
            }
        }

        private async Task<Product> CreateOrGetProductAsync(ProductStockForm form, DateTime now)
        {
            var productName = form.ProductName.Trim();
            var categoryId = form.CategoryId;
            var product = await _db.Products
                .FirstOrDefaultAsync(item => item.CategoryId == categoryId && item.Name == productName);

            if (product is not null)
            {
                product.IsActive = form.IsActive;
                product.Status = form.IsActive ? 1 : 0;
                product.UpdatedAt = now;
                return product;
            }

            product = new Product
            {
                CategoryId = categoryId,
                Name = productName,
                IsActive = form.IsActive,
                Status = form.IsActive ? 1 : 0,
                CreatedAt = now,
                UpdatedAt = now
            };

            _db.Products.Add(product);
            return product;
        }

        private async Task<ProductBatch> CreateProductBatchAsync(Product product, ProductStockForm form, DateTime now)
        {
            var batch = new ProductBatch
            {
                Product = product,
                BatchNo = await _batchNumberService.GenerateNextBatchNoAsync(),
                CostPrice = form.CostPrice!.Value,
                SellingPrice = form.SellingPrice ?? 0m,
                IsActive = form.IsActive,
                CreatedAt = now,
                UpdatedAt = now
            };

            _db.ProductBatches.Add(batch);
            return batch;
        }

        private async Task<List<SelectListItem>> BuildCategoryOptionsAsync()
        {
            return await _db.ProductCategories.AsNoTracking()
                .Where(category => category.IsActive && category.Status == 1)
                .OrderBy(category => category.Name)
                .Select(category => new SelectListItem { Value = category.Id.ToString(), Text = category.Name })
                .ToListAsync();
        }

        private async Task<bool> ProductCategoryExistsAsync(int categoryId)
        {
            return await _db.ProductCategories.AnyAsync(category => category.Id == categoryId && category.IsActive && category.Status == 1);
        }

        private async Task<FuelSetupPageViewModel> BuildFuelsPageAsync(string? search, FuelForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<Fuel> query = _db.Fuels.AsNoTracking().Include(fuel => fuel.Supplier);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(fuel => fuel.Name.Contains(searchText)
                    || fuel.Code.Contains(searchText)
                    || (fuel.Supplier != null && fuel.Supplier.Name.Contains(searchText)));
            }

            return new FuelSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                FuelForm = form ?? await BuildFuelFormAsync(editId),
                SupplierOptions = await BuildSupplierOptionsAsync(),
                Fuels = await query.OrderBy(fuel => fuel.Id).ToListAsync()
            };
        }

        private async Task<FuelSetupPageViewModel> BuildTanksPageAsync(string? search, int? branchId = null, TankForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<Tank> query = _db.Tanks.AsNoTracking().Include(tank => tank.Fuel).Include(tank => tank.Branch);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(tank => tank.TankNo.Contains(searchText)
                    || (tank.Fuel != null && tank.Fuel.Name.Contains(searchText))
                    || (tank.Branch != null && tank.Branch.Name.Contains(searchText)));
            }

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(tank => tank.BranchId == branchId.Value);
            }

            var tankForm = form ?? await BuildTankFormAsync(editId);
            if (tankForm.Id == 0 && tankForm.BranchId <= 0 && branchId.HasValue && branchId.Value > 0)
            {
                tankForm.BranchId = branchId.Value;
            }

            return new FuelSetupPageViewModel
            {
                Search = searchText,
                BranchId = branchId,
                BranchName = await BranchNameAsync(branchId),
                FormBranchName = await BranchNameAsync(tankForm.BranchId),
                ActiveModalId = activeModalId,
                TankForm = tankForm,
                BranchOptions = await BuildBranchFilterOptionsAsync(),
                FuelOptions = await BuildFuelOptionsAsync(),
                Tanks = await query.OrderBy(tank => tank.Id).ToListAsync()
            };
        }

        private async Task<FuelSetupPageViewModel> BuildPumpsPageAsync(string? search, int? branchId = null, PumpForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<Pump> query = _db.Pumps.AsNoTracking().Include(pump => pump.Branch);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(pump => pump.Name.Contains(searchText)
                    || (pump.Branch != null && pump.Branch.Name.Contains(searchText)));
            }

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(pump => pump.BranchId == branchId.Value);
            }

            var pumpForm = form ?? await BuildPumpFormAsync(editId);
            if (pumpForm.Id == 0 && pumpForm.BranchId <= 0 && branchId.HasValue && branchId.Value > 0)
            {
                pumpForm.BranchId = branchId.Value;
            }

            return new FuelSetupPageViewModel
            {
                Search = searchText,
                BranchId = branchId,
                BranchName = await BranchNameAsync(branchId),
                PumpFormBranchName = await BranchNameAsync(pumpForm.BranchId),
                ActiveModalId = activeModalId,
                PumpForm = pumpForm,
                BranchOptions = await BuildBranchFilterOptionsAsync(),
                Pumps = await query.OrderBy(pump => pump.Id).ToListAsync()
            };
        }

        private async Task<FuelForm> BuildFuelFormAsync(int? editId)
        {
            var fuel = editId.HasValue ? await _db.Fuels.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;

            return fuel is null ? new FuelForm() : new FuelForm
            {
                Id = fuel.Id,
                Name = fuel.Name,
                Code = fuel.Code,
                SupplierId = fuel.SupplierId,
                CurrentPricePerLiter = fuel.CurrentPricePerLiter,
                IsActive = fuel.IsActive
            };
        }

        private async Task<List<SelectListItem>> BuildSupplierOptionsAsync()
        {
            var options = await _db.Suppliers.AsNoTracking()
                .Where(supplier => supplier.Status == 1)
                .OrderBy(supplier => supplier.Name)
                .Select(supplier => new SelectListItem { Value = supplier.Id.ToString(), Text = supplier.Name })
                .ToListAsync();

            options.Insert(0, new SelectListItem { Value = "", Text = "No supplier" });
            return options;
        }

        private async Task<TankForm> BuildTankFormAsync(int? editId)
        {
            var tank = editId.HasValue ? await _db.Tanks.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;

            return tank is null ? new TankForm() : new TankForm
            {
                Id = tank.Id,
                FuelId = tank.FuelId,
                BranchId = tank.BranchId ?? 0,
                TankNo = tank.TankNo,
                CapacityLiters = tank.CapacityLiters,
                CurrentLiters = tank.CurrentLiters,
                IsActive = tank.IsActive
            };
        }

        private async Task<PumpForm> BuildPumpFormAsync(int? editId)
        {
            var pump = editId.HasValue ? await _db.Pumps.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;

            return pump is null ? new PumpForm() : new PumpForm
            {
                Id = pump.Id,
                BranchId = pump.BranchId ?? 0,
                Name = pump.Name,
                Status = pump.Status
            };
        }

        private async Task<List<SelectListItem>> BuildFuelOptionsAsync()
        {
            return await _db.Fuels.AsNoTracking()
                .Where(fuel => fuel.Status == 1 && fuel.IsActive)
                .OrderBy(fuel => fuel.Name)
                .Select(fuel => new SelectListItem { Value = fuel.Id.ToString(), Text = $"{fuel.Code} - {fuel.Name}" })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> BuildTankOptionsAsync()
        {
            var options = await _db.Tanks.AsNoTracking()
                .Include(tank => tank.Fuel)
                .Where(tank => tank.Status == 1 && tank.IsActive && tank.Fuel != null && tank.Fuel.Status == 1 && tank.Fuel.IsActive)
                .OrderBy(tank => tank.TankNo)
                .Select(tank => new SelectListItem
                {
                    Value = tank.Id.ToString(),
                    Text = tank.Fuel == null ? tank.TankNo : $"{tank.TankNo} - {tank.Fuel.Name}"
                })
                .ToListAsync();

            if (options.Count == 0)
            {
                options.Add(new SelectListItem { Value = "0", Text = "No tanks available", Disabled = true });
            }

            return options;
        }

        private async Task<List<Tank>> BuildActiveTankListAsync()
        {
            return await _db.Tanks.AsNoTracking()
                .Include(tank => tank.Fuel)
                .Where(tank => tank.Status == 1 && tank.IsActive && tank.Fuel != null && tank.Fuel.Status == 1 && tank.Fuel.IsActive)
                .OrderBy(tank => tank.TankNo)
                .ToListAsync();
        }

        private async Task<DiscountSetupPageViewModel> BuildDiscountsPageAsync(string? search, DiscountForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<Discount> query = _db.Discounts.AsNoTracking();
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(discount => discount.Name.Contains(searchText));
            }

            return new DiscountSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                DiscountForm = form ?? await BuildDiscountFormAsync(editId),
                Discounts = await query.OrderBy(discount => discount.Id).ToListAsync()
            };
        }

        private async Task<DiscountSetupPageViewModel> BuildEarningsPageAsync(string? search, EarningsForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<Earnings> query = _db.Earnings.AsNoTracking();
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(earnings => earnings.Name.Contains(searchText)
                    || (earnings.Description != null && earnings.Description.Contains(searchText)));
            }

            return new DiscountSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                EarningsForm = form ?? await BuildEarningsFormAsync(editId),
                Earnings = await query.OrderBy(earnings => earnings.Id).ToListAsync()
            };
        }

        private async Task<DiscountSetupPageViewModel> BuildMembersPageAsync(string? search, MemberForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<Member> query = _db.Members.AsNoTracking()
                .Include(member => member.Discount)
                .Include(member => member.Earnings);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(member => member.MemberNo.Contains(searchText)
                    || member.FullName.Contains(searchText)
                    || (member.CardNo != null && member.CardNo.Contains(searchText))
                    || (member.Discount != null && member.Discount.Name.Contains(searchText))
                    || (member.Earnings != null && member.Earnings.Name.Contains(searchText)));
            }

            return new DiscountSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                MemberForm = form ?? await BuildMemberFormAsync(editId),
                DiscountOptions = await BuildDiscountOptionsAsync(),
                EarningsOptions = await BuildEarningsOptionsAsync(),
                Members = await query.OrderBy(member => member.Id).ToListAsync()
            };
        }

        private async Task<VatSetupPageViewModel> BuildVatPageAsync(string? search, VatSettingForm? form = null, int? editId = null, string activeModalId = "")
        {
            var currentVat = await _db.VatSettings.AsNoTracking()
                .OrderByDescending(setting => setting.CreatedAt)
                .ThenByDescending(setting => setting.Id)
                .FirstOrDefaultAsync();

            IQueryable<VatSetting> query = _db.VatSettings.AsNoTracking();
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(setting => setting.Name.Contains(searchText)
                    || setting.Type.Contains(searchText)
                    || (searchText == "Active" && setting.IsActive)
                    || (searchText == "Inactive" && !setting.IsActive)
                    || (searchText == "Disabled" && !setting.IsActive));
            }

            var vatSettings = await query
                .OrderByDescending(setting => setting.CreatedAt)
                .ThenByDescending(setting => setting.Id)
                .ToListAsync();
            var history = currentVat is null
                ? vatSettings
                : vatSettings.Where(setting => setting.Id != currentVat.Id).ToList();

            return new VatSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                VatForm = form ?? await BuildVatFormAsync(editId),
                TypeOptions = BuildVatTypeOptions(),
                CurrentVatSetting = currentVat,
                VatHistory = history,
                VatSettings = vatSettings
            };
        }

        private async Task<VatSettingForm> BuildVatFormAsync(int? editId)
        {
            var vat = editId.HasValue ? await _db.VatSettings.AsNoTracking().FirstOrDefaultAsync(setting => setting.Id == editId.Value) : null;

            return vat is null ? new VatSettingForm() : new VatSettingForm
            {
                Id = vat.Id,
                Name = vat.Name,
                Rate = vat.Rate,
                Type = vat.Type,
                IsDefault = vat.IsDefault
            };
        }

        private static List<SelectListItem> BuildVatTypeOptions()
        {
            return VatTypeValues
                .Select(type => new SelectListItem { Value = type, Text = type })
                .ToList();
        }

        private void ValidateVatForm(VatSettingForm form)
        {
            if (!VatTypeValues.Contains(form.Type))
            {
                ModelState.AddModelError("VatForm.Type", "Select a valid VAT type.");
            }
        }

        private static VatSetting CreateVatVersion(VatSetting source, bool isActive, bool isDefault, DateTime now)
        {
            return new VatSetting
            {
                Name = source.Name,
                Rate = source.Rate,
                Type = source.Type,
                IsDefault = isDefault,
                IsActive = isActive,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        private async Task EnsureDefaultVatSettingAsync()
        {
            if (await _db.VatSettings.AnyAsync())
            {
                return;
            }

            var now = DateTime.UtcNow;
            _db.VatSettings.Add(new VatSetting
            {
                Name = "VAT 12%",
                Rate = 12.00m,
                Type = "Inclusive",
                IsDefault = true,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            });
            await _db.SaveChangesAsync();
        }

        private async Task<DiscountForm> BuildDiscountFormAsync(int? editId)
        {
            var discount = editId.HasValue ? await _db.Discounts.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;

            return discount is null ? new DiscountForm() : new DiscountForm
            {
                Id = discount.Id,
                Name = discount.Name,
                Status = discount.Status
            };
        }

        private async Task<EarningsForm> BuildEarningsFormAsync(int? editId)
        {
            var earnings = editId.HasValue ? await _db.Earnings.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;

            return earnings is null ? new EarningsForm() : new EarningsForm
            {
                Id = earnings.Id,
                Name = earnings.Name,
                Description = earnings.Description,
                Status = earnings.Status
            };
        }

        private async Task<MemberForm> BuildMemberFormAsync(int? editId)
        {
            var member = editId.HasValue ? await _db.Members.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;

            return member is null ? new MemberForm() : new MemberForm
            {
                Id = member.Id,
                MemberNo = member.MemberNo,
                CardNo = member.CardNo,
                FullName = member.FullName,
                ContactNumber = member.ContactNumber,
                Email = member.Email,
                Address = member.Address,
                DiscountId = member.DiscountId,
                EarningsId = member.EarningsId,
                Points = member.Points,
                Status = member.Status
            };
        }

        private async Task<List<SelectListItem>> BuildDiscountOptionsAsync()
        {
            var discounts = await _db.Discounts.AsNoTracking()
                .Where(discount => discount.Status == 1)
                .OrderBy(discount => discount.Name)
                .ToListAsync();

            var options = discounts
                .Select(discount => new SelectListItem { Value = discount.Id.ToString(), Text = discount.Name })
                .ToList();

            options.Insert(0, new SelectListItem { Value = "", Text = "No discount" });
            return options;
        }

        private async Task<List<SelectListItem>> BuildEarningsOptionsAsync()
        {
            var earnings = await _db.Earnings.AsNoTracking()
                .Where(item => item.Status == 1)
                .OrderBy(item => item.Name)
                .ToListAsync();

            var options = earnings
                .Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Name })
                .ToList();

            options.Insert(0, new SelectListItem { Value = "", Text = "No earnings" });
            return options;
        }

        private async Task<EmployeeSetupPageViewModel> BuildBranchesPageAsync(string? search, BranchForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<Branch> query = _db.Branches.AsNoTracking();
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(branch => branch.Name.Contains(searchText)
                    || (branch.Address != null && branch.Address.Contains(searchText)));
            }

            return new EmployeeSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                BranchForm = form ?? await BuildBranchFormAsync(editId),
                Branches = await query.OrderBy(branch => branch.Id).ToListAsync()
            };
        }

        private async Task<EmployeeSetupPageViewModel> BuildDepartmentsPageAsync(string? search, DepartmentForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<Department> query = _db.Departments.AsNoTracking().Include(department => department.Branch);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(department => department.Name.Contains(searchText)
                    || (department.Description != null && department.Description.Contains(searchText))
                    || (department.Branch != null && department.Branch.Name.Contains(searchText)));
            }

            return new EmployeeSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                DepartmentForm = form ?? await BuildDepartmentFormAsync(editId),
                BranchOptions = await BuildBranchOptionsAsync(),
                Departments = await query.OrderBy(department => department.Id).ToListAsync()
            };
        }

        private async Task<EmployeeSetupPageViewModel> BuildPositionsPageAsync(string? search, PositionForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<gpos.Models.Position> query = _db.Positions.AsNoTracking();
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(position => position.Name.Contains(searchText)
                    || (position.Description != null && position.Description.Contains(searchText)));
            }

            return new EmployeeSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                PositionForm = form ?? await BuildPositionFormAsync(editId),
                Positions = await query.OrderBy(position => position.Id).ToListAsync()
            };
        }

        private async Task<BranchForm> BuildBranchFormAsync(int? editId)
        {
            var branch = editId.HasValue ? await _db.Branches.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;

            return branch is null ? new BranchForm() : new BranchForm
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                Status = branch.Status
            };
        }

        private async Task<DepartmentForm> BuildDepartmentFormAsync(int? editId)
        {
            var department = editId.HasValue ? await _db.Departments.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;

            return department is null ? new DepartmentForm() : new DepartmentForm
            {
                Id = department.Id,
                BranchId = department.BranchId,
                Name = department.Name,
                Description = department.Description,
                Status = department.Status
            };
        }

        private async Task<PositionForm> BuildPositionFormAsync(int? editId)
        {
            var position = editId.HasValue ? await _db.Positions.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;

            return position is null ? new PositionForm() : new PositionForm
            {
                Id = position.Id,
                Name = position.Name,
                Description = position.Description,
                Status = position.Status
            };
        }

        private async Task<List<SelectListItem>> BuildBranchOptionsAsync()
        {
            return await _db.Branches.AsNoTracking()
                .Where(branch => branch.Status == 1)
                .OrderBy(branch => branch.Name)
                .Select(branch => new SelectListItem { Value = branch.Id.ToString(), Text = branch.Name })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> BuildBranchFilterOptionsAsync()
        {
            var options = await BuildBranchOptionsAsync();
            options.Insert(0, new SelectListItem { Value = "", Text = "All Branches" });
            return options;
        }

        private static string? CleanOptional(string? value)
        {
            var trimmed = (value ?? string.Empty).Trim();
            return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
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
    }
}

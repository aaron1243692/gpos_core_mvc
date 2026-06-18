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
    public class ConfigurationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ProductBatchNumberService _batchNumberService;

        public ConfigurationController(ApplicationDbContext db, ProductBatchNumberService batchNumberService)
        {
            _db = db;
            _batchNumberService = batchNumberService;
        }

        public IActionResult Products() => View();
        public async Task<IActionResult> DisplayProducts(string? search)
        {
            return View(await BuildDisplayProductsPageAsync(search));
        }

        public async Task<IActionResult> WarehouseProducts(string? search)
        {
            return View(await BuildWarehouseProductsPageAsync(search));
        }

        public async Task<IActionResult> ProductCategories(string? search)
        {
            return View(await BuildProductCategoriesPageAsync(search));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDisplayProduct([Bind(Prefix = "StockForm")] ProductStockForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("DisplayProducts", await BuildDisplayProductsPageAsync(search, form, "displayProductModal"));
            }

            if (form.CategoryId.HasValue && !await ProductCategoryExistsAsync(form.CategoryId.Value))
            {
                ModelState.AddModelError("StockForm.CategoryId", "Select an active category.");
                return View("DisplayProducts", await BuildDisplayProductsPageAsync(search, form, "displayProductModal"));
            }

            var now = DateTime.UtcNow;
            var product = await CreateOrGetProductAsync(form, now);
            var batch = await CreateProductBatchAsync(product, form, now);

            _db.DisplayStocks.Add(new DisplayStock
            {
                Product = product,
                Batch = batch,
                Quantity = form.Quantity!.Value,
                CreatedAt = now,
                UpdatedAt = now
            });

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(DisplayProducts), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveWarehouseProduct([Bind(Prefix = "StockForm")] ProductStockForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("WarehouseProducts", await BuildWarehouseProductsPageAsync(search, form, "warehouseProductModal"));
            }

            if (form.CategoryId.HasValue && !await ProductCategoryExistsAsync(form.CategoryId.Value))
            {
                ModelState.AddModelError("StockForm.CategoryId", "Select an active category.");
                return View("WarehouseProducts", await BuildWarehouseProductsPageAsync(search, form, "warehouseProductModal"));
            }

            var now = DateTime.UtcNow;
            var product = await CreateOrGetProductAsync(form, now);
            var batch = await CreateProductBatchAsync(product, form, now);

            _db.WarehouseStocks.Add(new WarehouseStock
            {
                Product = product,
                Batch = batch,
                Quantity = form.Quantity!.Value,
                CreatedAt = now,
                UpdatedAt = now
            });

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(WarehouseProducts), new { search });
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
                CreatedAt = now,
                UpdatedAt = now
            });

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ProductCategories), new { search });
        }

        public IActionResult ItemUnits() => View();
        public IActionResult FuelTypes() => View();
        public IActionResult FuelPrices() => View();
        public IActionResult PumpUnits() => View();
        public async Task<IActionResult> Discounts(string? search, int? editId)
        {
            return View(await BuildDiscountsPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "discountModal" : ""));
        }

        public async Task<IActionResult> Members(string? search, int? editId)
        {
            return View(await BuildMembersPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "memberModal" : ""));
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
            discount.EarnRate = form.EarnRate;
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

        public IActionResult Rebate() => View();
        public IActionResult Position() => View();
        public IActionResult Branch() => View();
        public IActionResult Department() => View();

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
                var supplierExists = await _db.Suppliers.AnyAsync(supplier => supplier.Id == form.SupplierId.Value);

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

            var isUsedByTank = await _db.Tanks.AnyAsync(tank => tank.FuelId == id);

            if (isUsedByTank)
            {
                TempData["FuelSetupFeedback"] = "Delete blocked. This fuel is used by a tank.";
                return RedirectToAction(nameof(Fuels), new { search });
            }

            fuel.IsActive = false;
            fuel.UpdatedAt = DateTime.UtcNow;
            TempData["FuelSetupFeedback"] = "Fuel deleted.";
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Fuels), new { search });
        }

        public async Task<IActionResult> Tanks(string? search, int? editId)
        {
            return View(await BuildTanksPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "tankModal" : ""));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveTank([Bind(Prefix = "TankForm")] TankForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("Tanks", await BuildTanksPageAsync(search, form, activeModalId: "tankModal"));
            }

            var fuelExists = await _db.Fuels.AnyAsync(fuel => fuel.Id == form.FuelId);

            if (!fuelExists)
            {
                ModelState.AddModelError("TankForm.FuelId", "Tank fuel is required.");
                return View("Tanks", await BuildTanksPageAsync(search, form, activeModalId: "tankModal"));
            }

            var now = DateTime.UtcNow;
            var tank = form.Id > 0 ? await _db.Tanks.FindAsync(form.Id) : new Tank { CreatedAt = now };

            if (tank is null)
            {
                return NotFound();
            }

            tank.FuelId = form.FuelId;
            tank.TankNo = form.TankNo.Trim();
            tank.IsActive = form.IsActive;
            tank.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.Tanks.Add(tank);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Tanks), new { search });
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

            var isUsedByPump = await _db.Pumps.AnyAsync(pump => pump.TankId == id);

            if (isUsedByPump)
            {
                TempData["FuelSetupFeedback"] = "Delete blocked. This tank is used by a pump.";
                return RedirectToAction(nameof(Tanks), new { search });
            }

            tank.IsActive = false;
            tank.UpdatedAt = DateTime.UtcNow;
            TempData["FuelSetupFeedback"] = "Tank deleted.";
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Tanks), new { search });
        }

        public async Task<IActionResult> Pumps(string? search, int? editId)
        {
            return View(await BuildPumpsPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "pumpModal" : ""));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePump([Bind(Prefix = "PumpForm")] PumpForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("Pumps", await BuildPumpsPageAsync(search, form, activeModalId: "pumpModal"));
            }

            var tankExists = await _db.Tanks.AnyAsync(tank => tank.Id == form.TankId);

            if (!tankExists)
            {
                ModelState.AddModelError("PumpForm.TankId", "Pump tank is required.");
                return View("Pumps", await BuildPumpsPageAsync(search, form, activeModalId: "pumpModal"));
            }

            var now = DateTime.UtcNow;
            var pump = form.Id > 0 ? await _db.Pumps.FindAsync(form.Id) : new Pump { CreatedAt = now };

            if (pump is null)
            {
                return NotFound();
            }

            pump.TankId = form.TankId;
            pump.Name = form.Name.Trim();
            pump.PumpNo = form.Name.Trim();
            pump.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.Pumps.Add(pump);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Pumps), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePump(int id, string? search)
        {
            var pump = await _db.Pumps.FindAsync(id);

            if (pump is not null)
            {
                _db.Pumps.Remove(pump);
                await _db.SaveChangesAsync();
                TempData["FuelSetupFeedback"] = "Pump deleted.";
            }

            return RedirectToAction(nameof(Pumps), new { search });
        }

        private async Task<ProductStockPageViewModel> BuildDisplayProductsPageAsync(string? search, ProductStockForm? form = null, string activeModalId = "")
        {
            IQueryable<DisplayStock> query = _db.DisplayStocks.AsNoTracking()
                .Include(stock => stock.Product)
                .ThenInclude(product => product!.Category)
                .Include(stock => stock.Batch);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(stock => (stock.Product != null && stock.Product.Name.Contains(searchText))
                    || (stock.Product != null && stock.Product.Category != null && stock.Product.Category.Name.Contains(searchText))
                    || (stock.Batch != null && stock.Batch.BatchNo.Contains(searchText)));
            }

            return new ProductStockPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                StockForm = form ?? new ProductStockForm(),
                CategoryOptions = await BuildCategoryOptionsAsync(),
                DisplayStocks = await query.OrderBy(stock => stock.Id).ToListAsync()
            };
        }

        private async Task<ProductStockPageViewModel> BuildWarehouseProductsPageAsync(string? search, ProductStockForm? form = null, string activeModalId = "")
        {
            IQueryable<WarehouseStock> query = _db.WarehouseStocks.AsNoTracking()
                .Include(stock => stock.Product)
                .ThenInclude(product => product!.Category)
                .Include(stock => stock.Batch);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(stock => (stock.Product != null && stock.Product.Name.Contains(searchText))
                    || (stock.Product != null && stock.Product.Category != null && stock.Product.Category.Name.Contains(searchText))
                    || (stock.Batch != null && stock.Batch.BatchNo.Contains(searchText)));
            }

            return new ProductStockPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                StockForm = form ?? new ProductStockForm(),
                CategoryOptions = await BuildCategoryOptionsAsync(),
                WarehouseStocks = await query.OrderBy(stock => stock.Id).ToListAsync()
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

        private async Task<Product> CreateOrGetProductAsync(ProductStockForm form, DateTime now)
        {
            var productName = form.ProductName.Trim();
            var categoryId = form.CategoryId;
            var product = await _db.Products
                .FirstOrDefaultAsync(item => item.CategoryId == categoryId && item.Name == productName);

            if (product is not null)
            {
                product.IsActive = form.IsActive;
                product.UpdatedAt = now;
                return product;
            }

            product = new Product
            {
                CategoryId = categoryId,
                Name = productName,
                IsActive = form.IsActive,
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
                .Where(category => category.IsActive)
                .OrderBy(category => category.Name)
                .Select(category => new SelectListItem { Value = category.Id.ToString(), Text = category.Name })
                .ToListAsync();
        }

        private async Task<bool> ProductCategoryExistsAsync(int categoryId)
        {
            return await _db.ProductCategories.AnyAsync(category => category.Id == categoryId && category.IsActive);
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

        private async Task<FuelSetupPageViewModel> BuildTanksPageAsync(string? search, TankForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<Tank> query = _db.Tanks.AsNoTracking().Include(tank => tank.Fuel);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(tank => tank.TankNo.Contains(searchText) || (tank.Fuel != null && tank.Fuel.Name.Contains(searchText)));
            }

            return new FuelSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                TankForm = form ?? await BuildTankFormAsync(editId),
                FuelOptions = await BuildFuelOptionsAsync(),
                Tanks = await query.OrderBy(tank => tank.Id).ToListAsync()
            };
        }

        private async Task<FuelSetupPageViewModel> BuildPumpsPageAsync(string? search, PumpForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<Pump> query = _db.Pumps.AsNoTracking().Include(pump => pump.Tank).ThenInclude(tank => tank!.Fuel);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(pump => pump.Name.Contains(searchText)
                    || (pump.Tank != null && pump.Tank.TankNo.Contains(searchText))
                    || (pump.Tank != null && pump.Tank.Fuel != null && pump.Tank.Fuel.Name.Contains(searchText)));
            }

            return new FuelSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                PumpForm = form ?? await BuildPumpFormAsync(editId),
                TankOptions = await BuildTankOptionsAsync(),
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
                TankNo = tank.TankNo,
                IsActive = tank.IsActive
            };
        }

        private async Task<PumpForm> BuildPumpFormAsync(int? editId)
        {
            var pump = editId.HasValue ? await _db.Pumps.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;

            return pump is null ? new PumpForm() : new PumpForm
            {
                Id = pump.Id,
                TankId = pump.TankId,
                Name = pump.Name
            };
        }

        private async Task<List<SelectListItem>> BuildFuelOptionsAsync()
        {
            return await _db.Fuels.AsNoTracking()
                .OrderBy(fuel => fuel.Name)
                .Select(fuel => new SelectListItem { Value = fuel.Id.ToString(), Text = $"{fuel.Code} - {fuel.Name}" })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> BuildTankOptionsAsync()
        {
            var options = await _db.Tanks.AsNoTracking()
                .Include(tank => tank.Fuel)
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

        private async Task<DiscountSetupPageViewModel> BuildMembersPageAsync(string? search, MemberForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<Member> query = _db.Members.AsNoTracking().Include(member => member.Discount);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(member => member.MemberNo.Contains(searchText)
                    || member.FullName.Contains(searchText)
                    || (member.CardNo != null && member.CardNo.Contains(searchText))
                    || (member.Discount != null && member.Discount.Name.Contains(searchText)));
            }

            return new DiscountSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                MemberForm = form ?? await BuildMemberFormAsync(editId),
                DiscountOptions = await BuildDiscountOptionsAsync(),
                Members = await query.OrderBy(member => member.Id).ToListAsync()
            };
        }

        private async Task<DiscountForm> BuildDiscountFormAsync(int? editId)
        {
            var discount = editId.HasValue ? await _db.Discounts.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;

            return discount is null ? new DiscountForm() : new DiscountForm
            {
                Id = discount.Id,
                Name = discount.Name,
                EarnRate = discount.EarnRate,
                Status = discount.Status
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
                Points = member.Points,
                Status = member.Status
            };
        }

        private async Task<List<SelectListItem>> BuildDiscountOptionsAsync()
        {
            var options = await _db.Discounts.AsNoTracking()
                .Where(discount => discount.Status == 1)
                .OrderBy(discount => discount.Name)
                .Select(discount => new SelectListItem { Value = discount.Id.ToString(), Text = discount.Name })
                .ToListAsync();

            options.Insert(0, new SelectListItem { Value = "", Text = "No discount" });
            return options;
        }

    }
}

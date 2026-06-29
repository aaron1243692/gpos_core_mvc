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
    public class VouchersController : Controller
    {
        private static readonly string[] VoucherStatuses = ["Active", "Inactive"];
        private readonly ApplicationDbContext _db;

        public VouchersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string? search, int? editId)
        {
            return View(await BuildVouchersPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "voucherModal" : ""));
        }

        public async Task<IActionResult> Rules(string? search, int? editId)
        {
            return View(await BuildRulesPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "voucherRuleModal" : ""));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveVoucher([Bind(Prefix = "VoucherForm")] VoucherForm form, string? search)
        {
            var name = NormalizeVoucherCode(form.Name);
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("VoucherForm.Name", "Name is required.");
            }

            if (!VoucherStatuses.Contains(form.Status, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("VoucherForm.Status", "Select a valid status.");
            }

            if (await _db.Vouchers.AnyAsync(voucher => voucher.Id != form.Id && voucher.Code == name))
            {
                ModelState.AddModelError("VoucherForm.Name", "Voucher name already exists.");
            }

            if (!ModelState.IsValid)
            {
                form.Name = name;
                return View("Index", await BuildVouchersPageAsync(search, form, activeModalId: "voucherModal"));
            }

            var now = DateTime.UtcNow;
            var voucher = form.Id > 0 ? await _db.Vouchers.FindAsync(form.Id) : new Voucher { CreatedAt = now };
            if (voucher is null) return NotFound();

            voucher.Code = name;
            voucher.Status = VoucherStatuses.First(status => string.Equals(status, form.Status, StringComparison.OrdinalIgnoreCase));
            voucher.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.Vouchers.Add(voucher);
            }

            await _db.SaveChangesAsync();
            TempData["VoucherSetupFeedback"] = form.Id > 0 ? "Voucher updated." : "Voucher added.";
            return RedirectToAction(nameof(Index), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVoucher(int id, string? search)
        {
            var voucher = await _db.Vouchers.FindAsync(id);
            if (voucher is not null)
            {
                voucher.Status = "Inactive";
                voucher.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                TempData["VoucherSetupFeedback"] = "Voucher disabled.";
            }

            return RedirectToAction(nameof(Index), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRule([Bind(Prefix = "VoucherRuleForm")] VoucherRuleForm form, string? search)
        {
            var discountType = NormalizeDiscountRuleType(form.DiscountType);
            var appliesTo = NormalizeDiscountRuleAppliesTo(form.AppliesTo);

            if (discountType is null)
            {
                ModelState.AddModelError("VoucherRuleForm.DiscountType", "Discount Type is required.");
            }

            if (appliesTo is null)
            {
                ModelState.AddModelError("VoucherRuleForm.AppliesTo", "Applies To is required.");
            }

            if (form.DiscountValue.HasValue && discountType == "Percentage" && (form.DiscountValue.Value <= 0 || form.DiscountValue.Value > 100))
            {
                ModelState.AddModelError("VoucherRuleForm.DiscountValue", "Percentage value must be greater than 0 and not greater than 100.");
            }

            if (form.DiscountValue.HasValue && discountType == "Fixed Amount" && form.DiscountValue.Value <= 0)
            {
                ModelState.AddModelError("VoucherRuleForm.DiscountValue", "Fixed Amount value must be greater than 0.");
            }

            if (form.StartDate.HasValue && form.EndDate.HasValue && form.EndDate.Value.Date < form.StartDate.Value.Date)
            {
                ModelState.AddModelError("VoucherRuleForm.EndDate", "End Date must not be earlier than Start Date.");
            }

            var voucher = form.VoucherId > 0
                ? await _db.Vouchers.AsNoTracking().FirstOrDefaultAsync(item => item.Id == form.VoucherId)
                : null;

            if (voucher is null)
            {
                ModelState.AddModelError("VoucherRuleForm.VoucherId", "Voucher is required.");
            }

            if (!form.UnlimitedUses && !form.MaxRedemptions.HasValue)
            {
                ModelState.AddModelError("VoucherRuleForm.MaxRedemptions", "Maximum redemptions is required unless unlimited uses is enabled.");
            }

            if (form.UsageLimitType == "Limited Uses" && !form.LimitedUseCount.HasValue)
            {
                ModelState.AddModelError("VoucherRuleForm.LimitedUseCount", "Limited use count is required.");
            }

            if (!ModelState.IsValid)
            {
                return View("Rules", await BuildRulesPageAsync(search, form, activeModalId: "voucherRuleModal"));
            }

            var now = DateTime.UtcNow;
            var rule = form.Id > 0 ? await _db.VoucherRules.FindAsync(form.Id) : new VoucherRule { CreatedAt = now };
            if (rule is null) return NotFound();

            rule.Name = voucher!.Code;
            rule.VoucherId = form.VoucherId;
            rule.AppliesTo = appliesTo!;
            rule.RewardType = discountType!;
            rule.RewardValue = form.DiscountValue!.Value;
            rule.MinimumPurchaseAmount = form.MinimumAmount!.Value;
            rule.MemberRequired = form.MemberRequired ? 1 : 0;
            rule.EffectiveDate = form.StartDate?.Date;
            rule.ExpirationDate = form.EndDate?.Date;
            rule.NoExpiration = !form.EndDate.HasValue;
            rule.MaxDiscountAmount = form.MaxDiscountAmount;
            rule.ApplicableProductIds = JoinIds(form.ProductIds);
            rule.ApplicableCategoryIds = JoinIds(form.CategoryIds);
            rule.UsageLimitType = form.UnlimitedUses ? "Unlimited" : form.UsageLimitType.Trim();
            rule.MaxRedemptions = form.UnlimitedUses ? null : form.MaxRedemptions;
            rule.LimitedUseCount = string.Equals(rule.UsageLimitType, "Limited Uses", StringComparison.OrdinalIgnoreCase) ? form.LimitedUseCount : null;
            rule.Priority = form.Priority;
            rule.Status = form.Status;
            rule.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.VoucherRules.Add(rule);
            }

            await _db.SaveChangesAsync();
            TempData["VoucherSetupFeedback"] = form.Id > 0 ? "Voucher rule updated." : "Voucher rule added.";
            return RedirectToAction(nameof(Rules), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRule(int id, string? search)
        {
            var rule = await _db.VoucherRules.FindAsync(id);
            if (rule is not null)
            {
                rule.Status = 0;
                rule.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                TempData["VoucherSetupFeedback"] = "Voucher rule disabled.";
            }

            return RedirectToAction(nameof(Rules), new { search });
        }

        private async Task<VoucherSetupPageViewModel> BuildVouchersPageAsync(string? search, VoucherForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<Voucher> query = _db.Vouchers.AsNoTracking();
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(voucher => voucher.Code.Contains(searchText)
                    || voucher.Status.Contains(searchText));
            }

            return new VoucherSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                VoucherForm = form ?? await BuildVoucherFormAsync(editId),
                Vouchers = await query.OrderBy(voucher => voucher.Id).ToListAsync()
            };
        }

        private async Task<VoucherSetupPageViewModel> BuildRulesPageAsync(string? search, VoucherRuleForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<VoucherRule> query = _db.VoucherRules.AsNoTracking().Include(rule => rule.Voucher);
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(rule => (rule.Voucher != null && rule.Voucher.Code.Contains(searchText))
                    || rule.Name.Contains(searchText)
                    || rule.AppliesTo.Contains(searchText)
                    || rule.RewardType.Contains(searchText)
                    || (searchText == "Active" && rule.Status == 1)
                    || (searchText == "Disabled" && rule.Status == 0)
                    || (searchText == "Inactive" && rule.Status == 0));
            }

            return new VoucherSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                VoucherRuleForm = form ?? await BuildVoucherRuleFormAsync(editId),
                VoucherOptions = await BuildRequiredVoucherOptionsAsync(),
                ProductOptions = await BuildProductOptionsAsync(),
                CategoryOptions = await BuildCategoryOptionsAsync(),
                VoucherRules = await query.OrderBy(rule => rule.Id).ToListAsync()
            };
        }

        private async Task<VoucherForm> BuildVoucherFormAsync(int? editId)
        {
            var voucher = editId.HasValue ? await _db.Vouchers.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;
            return voucher is null ? new VoucherForm() : new VoucherForm { Id = voucher.Id, Name = voucher.Code, Status = voucher.Status };
        }

        private async Task<VoucherRuleForm> BuildVoucherRuleFormAsync(int? editId)
        {
            var rule = editId.HasValue ? await _db.VoucherRules.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;
            return rule is null ? new VoucherRuleForm() : new VoucherRuleForm
            {
                Id = rule.Id,
                VoucherId = rule.VoucherId ?? 0,
                AppliesTo = rule.AppliesTo,
                DiscountType = rule.RewardType,
                DiscountValue = rule.RewardValue,
                MinimumAmount = rule.MinimumPurchaseAmount,
                MemberRequired = rule.MemberRequired == 1,
                StartDate = rule.EffectiveDate,
                EndDate = rule.ExpirationDate,
                MaxDiscountAmount = rule.MaxDiscountAmount,
                ProductIds = ParseIds(rule.ApplicableProductIds),
                CategoryIds = ParseIds(rule.ApplicableCategoryIds),
                NoExpiration = !rule.ExpirationDate.HasValue,
                UnlimitedUses = string.Equals(rule.UsageLimitType, "Unlimited", StringComparison.OrdinalIgnoreCase),
                MaxRedemptions = rule.MaxRedemptions,
                UsageLimitType = rule.UsageLimitType,
                LimitedUseCount = rule.LimitedUseCount,
                Priority = rule.Priority,
                Status = rule.Status
            };
        }

        private async Task<List<SelectListItem>> BuildRequiredVoucherOptionsAsync()
        {
            return await _db.Vouchers.AsNoTracking()
                .Where(item => item.Status == "Active")
                .OrderBy(item => item.Code)
                .Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Code })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> BuildProductOptionsAsync() => await _db.Products.AsNoTracking().Where(item => item.Status == 1 && item.IsActive).OrderBy(item => item.Name).Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Name }).ToListAsync();
        private async Task<List<SelectListItem>> BuildCategoryOptionsAsync() => await _db.ProductCategories.AsNoTracking().Where(item => item.Status == 1 && item.IsActive).OrderBy(item => item.Name).Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Name }).ToListAsync();

        private static string NormalizeVoucherCode(string? value)
        {
            return (value ?? string.Empty).Trim().ToUpperInvariant();
        }

        private static string? NormalizeDiscountRuleAppliesTo(string? appliesTo)
        {
            return appliesTo?.Trim() switch
            {
                "Fuel" => "Fuel",
                "Product" => "Product",
                "Both" => "Both",
                _ => null
            };
        }

        private static string? NormalizeDiscountRuleType(string? discountType)
        {
            return discountType?.Trim() switch
            {
                "Percentage" => "Percentage",
                "Fixed Amount" => "Fixed Amount",
                _ => null
            };
        }

        private static string? JoinIds(List<int> ids) => ids.Count == 0 ? null : string.Join(",", ids.Distinct().OrderBy(id => id));
        private static List<int> ParseIds(string? value) => (value ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(item => int.TryParse(item, out var id) ? id : 0).Where(id => id > 0).ToList();
    }
}

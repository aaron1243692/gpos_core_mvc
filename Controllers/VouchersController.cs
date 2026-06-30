using gpos.Data;
using gpos.Filters;
using gpos.Models;
using gpos.Models.ViewModels;
using gpos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class VouchersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly VoucherCodeService _voucherCodeService;

        public VouchersController(ApplicationDbContext db, VoucherCodeService voucherCodeService)
        {
            _db = db;
            _voucherCodeService = voucherCodeService;
        }

        public async Task<IActionResult> Index(string? search, int? editId)
        {
            return View(await BuildVouchersPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "voucherModal" : ""));
        }

        public async Task<IActionResult> Rules(string? search, int? editId)
        {
            return RedirectToAction(nameof(Index), new { search, editId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveVoucher([Bind(Prefix = "VoucherRuleForm")] VoucherRuleForm form, string? search)
        {
            var name = NormalizeVoucherName(form.Name);
            var discountType = NormalizeDiscountRuleType(form.DiscountType);
            var appliesTo = NormalizeDiscountRuleAppliesTo(form.AppliesTo);

            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("VoucherRuleForm.Name", "Voucher Name is required.");
            }

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

            if (await _db.VoucherRules.AnyAsync(rule => rule.Id != form.Id && rule.Name == name))
            {
                ModelState.AddModelError("VoucherRuleForm.Name", "Voucher name already exists.");
            }

            if (!form.UsageLimit.HasValue)
            {
                ModelState.AddModelError("VoucherRuleForm.UsageLimit", "Usage Limit is required.");
            }

            if (form.UsageLimit.HasValue && form.UsageLimit.Value < 1)
            {
                ModelState.AddModelError("VoucherRuleForm.UsageLimit", "Usage Limit must be at least 1.");
            }

            if (!ModelState.IsValid)
            {
                form.Name = name;
                return View("Index", await BuildVouchersPageAsync(search, form, activeModalId: "voucherModal"));
            }

            var now = DateTime.UtcNow;
            var rule = form.Id > 0 ? await _db.VoucherRules.FindAsync(form.Id) : new VoucherRule { CreatedAt = now };
            if (rule is null) return NotFound();

            if (string.IsNullOrWhiteSpace(rule.Code))
            {
                rule.Code = await ResolveVoucherRuleCodeAsync(form.Code, rule.Id);
            }

            rule.Name = name;
            rule.AppliesTo = appliesTo!;
            rule.RewardType = discountType!;
            rule.RewardValue = form.DiscountValue!.Value;
            rule.MinimumPurchaseAmount = form.MinimumAmount!.Value;
            rule.MemberRequired = form.MemberRequired ? 1 : 0;
            rule.EffectiveDate = form.StartDate?.Date;
            rule.ExpirationDate = form.EndDate?.Date;
            rule.NoExpiration = !form.EndDate.HasValue;
            rule.MaxRedemptions = form.UsageLimit!.Value;
            rule.Status = form.Status;
            rule.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.VoucherRules.Add(rule);
            }

            await _db.SaveChangesAsync();
            TempData["VoucherSetupFeedback"] = form.Id > 0 ? "Voucher updated." : "Voucher added.";
            return RedirectToAction(nameof(Index), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVoucher(int id, string? search)
        {
            var rule = await _db.VoucherRules.FindAsync(id);
            if (rule is not null)
            {
                rule.Status = 0;
                rule.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                TempData["VoucherSetupFeedback"] = "Voucher disabled.";
            }

            return RedirectToAction(nameof(Index), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateVoucher(int id, string? search)
        {
            var rule = await _db.VoucherRules.FindAsync(id);
            if (rule is not null)
            {
                rule.Status = 1;
                rule.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                TempData["VoucherSetupFeedback"] = "Voucher activated.";
            }
            return RedirectToAction(nameof(Index), new { search });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRule([Bind(Prefix = "VoucherRuleForm")] VoucherRuleForm form, string? search)
        {
            return await SaveVoucher(form, search);
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

            return RedirectToAction(nameof(Index), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateRule(int id, string? search)
        {
            var rule = await _db.VoucherRules.FindAsync(id);
            if (rule is not null)
            {
                rule.Status = 1;
                rule.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                TempData["VoucherSetupFeedback"] = "Voucher rule activated.";
            }
            return RedirectToAction(nameof(Index), new { search });
        }


        private async Task<VoucherSetupPageViewModel> BuildVouchersPageAsync(string? search, VoucherRuleForm? form = null, int? editId = null, string activeModalId = "")
        {
            return await BuildRulesPageAsync(search, form, editId, activeModalId);
        }

        private async Task<VoucherSetupPageViewModel> BuildRulesPageAsync(string? search, VoucherRuleForm? form = null, int? editId = null, string activeModalId = "")
        {
            await EnsureVoucherRulesHaveCodesAsync();

            IQueryable<VoucherRule> query = _db.VoucherRules.AsNoTracking().Include(rule => rule.Voucher);
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(rule => (rule.Code != null && rule.Code.Contains(searchText))
                    || (rule.Voucher != null && rule.Voucher.Code.Contains(searchText))
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
                VoucherRules = await query.OrderBy(rule => rule.Id).ToListAsync()
            };
        }

        private async Task<VoucherRuleForm> BuildVoucherRuleFormAsync(int? editId)
        {
            var rule = editId.HasValue ? await _db.VoucherRules.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;
            return rule is null ? new VoucherRuleForm { Code = await _voucherCodeService.GenerateUniqueVoucherRuleCodeAsync() } : new VoucherRuleForm
            {
                Id = rule.Id,
                Code = rule.Code ?? string.Empty,
                Name = rule.Name,
                AppliesTo = rule.AppliesTo,
                DiscountType = rule.RewardType,
                DiscountValue = rule.RewardValue,
                MinimumAmount = rule.MinimumPurchaseAmount,
                MemberRequired = rule.MemberRequired == 1,
                StartDate = rule.EffectiveDate,
                EndDate = rule.ExpirationDate,
                UsageLimit = ResolveUsageLimit(rule),
                Status = rule.Status
            };
        }

        private async Task EnsureVoucherRulesHaveCodesAsync()
        {
            var rulesWithoutCodes = await _db.VoucherRules
                .Where(rule => rule.Code == null || rule.Code == string.Empty)
                .OrderBy(rule => rule.Id)
                .ToListAsync();

            foreach (var rule in rulesWithoutCodes)
            {
                rule.Code = await _voucherCodeService.GenerateUniqueVoucherRuleCodeAsync();
                rule.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }

        private async Task<string> ResolveVoucherRuleCodeAsync(string? requestedCode, int ruleId)
        {
            var normalizedCode = NormalizeVoucherRuleCode(requestedCode);
            if (IsValidVoucherRuleCode(normalizedCode)
                && !await _db.VoucherRules.AnyAsync(rule => rule.Id != ruleId && rule.Code == normalizedCode))
            {
                return normalizedCode;
            }

            return await _voucherCodeService.GenerateUniqueVoucherRuleCodeAsync();
        }

        private static string NormalizeVoucherName(string? value)
        {
            return (value ?? string.Empty).Trim();
        }

        private static string NormalizeVoucherRuleCode(string? value)
        {
            return new string((value ?? string.Empty)
                .Trim()
                .ToUpperInvariant()
                .Where(character => (character >= 'A' && character <= 'Z') || (character >= '0' && character <= '9'))
                .Take(6)
                .ToArray());
        }

        private static bool IsValidVoucherRuleCode(string code)
        {
            return code.Length == 6 && code.All(character => (character >= 'A' && character <= 'Z') || (character >= '0' && character <= '9'));
        }

        private static int ResolveUsageLimit(VoucherRule rule)
        {
            if (rule.MaxRedemptions.HasValue && rule.MaxRedemptions.Value > 0)
            {
                return rule.MaxRedemptions.Value;
            }

            if (rule.LimitedUseCount.HasValue && rule.LimitedUseCount.Value > 0)
            {
                return rule.LimitedUseCount.Value;
            }

            return 1;
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

    }
}

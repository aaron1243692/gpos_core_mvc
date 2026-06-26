using System.Security.Cryptography;
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
        private const string VoucherAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly string[] VoucherStatuses = ["Active", "Redeemed", "Expired", "Disabled"];
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
            if (!VoucherStatuses.Contains(form.Status, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("VoucherForm.Status", "Select a valid status.");
            }

            if (!await _db.Members.AnyAsync(member => member.Id == form.MemberId && member.Status == 1))
            {
                ModelState.AddModelError("VoucherForm.MemberId", "Select an active customer.");
            }

            if (!ModelState.IsValid)
            {
                return View("Index", await BuildVouchersPageAsync(search, form, activeModalId: "voucherModal"));
            }

            var now = DateTime.UtcNow;
            var voucher = form.Id > 0 ? await _db.Vouchers.FindAsync(form.Id) : new Voucher { Code = await GenerateVoucherCodeAsync(), CreatedAt = now };
            if (voucher is null) return NotFound();

            voucher.MemberId = form.MemberId;
            voucher.Status = VoucherStatuses.First(status => string.Equals(status, form.Status, StringComparison.OrdinalIgnoreCase));
            voucher.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.Vouchers.Add(voucher);
            }

            await _db.SaveChangesAsync();
            TempData["VoucherSetupFeedback"] = form.Id > 0 ? "Voucher updated." : $"Voucher {voucher.Code} generated.";
            return RedirectToAction(nameof(Index), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVoucher(int id, string? search)
        {
            var voucher = await _db.Vouchers.FindAsync(id);
            if (voucher is not null)
            {
                voucher.Status = "Disabled";
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
            ValidateVoucherRule(form);
            if (!ModelState.IsValid)
            {
                return View("Rules", await BuildRulesPageAsync(search, form, activeModalId: "voucherRuleModal"));
            }

            var now = DateTime.UtcNow;
            var rule = form.Id > 0 ? await _db.VoucherRules.FindAsync(form.Id) : new VoucherRule { CreatedAt = now };
            if (rule is null) return NotFound();

            rule.Name = form.Name.Trim();
            rule.RewardType = form.RewardType.Trim();
            rule.RewardValue = form.RewardValue!.Value;
            rule.MaxDiscountAmount = form.MaxDiscountAmount;
            rule.MinimumPurchaseAmount = form.MinimumPurchaseAmount!.Value;
            rule.ApplicableProductIds = JoinIds(form.ProductIds);
            rule.ApplicableCategoryIds = JoinIds(form.CategoryIds);
            rule.AppliesTo = form.AppliesTo.Trim();
            rule.EffectiveDate = form.EffectiveDate?.Date;
            rule.ExpirationDate = form.NoExpiration ? null : form.ExpirationDate?.Date;
            rule.NoExpiration = form.NoExpiration;
            rule.MaxRedemptions = form.UnlimitedUses ? null : form.MaxRedemptions;
            rule.UsageLimitType = form.UnlimitedUses ? "Unlimited" : form.UsageLimitType.Trim();
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
            var query = _db.Vouchers.AsNoTracking().Include(voucher => voucher.Member).AsQueryable();
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(voucher => voucher.Code.Contains(searchText) || (voucher.Member != null && voucher.Member.FullName.Contains(searchText)));
            }

            return new VoucherSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                VoucherForm = form ?? await BuildVoucherFormAsync(editId),
                MemberOptions = await BuildMemberOptionsAsync(),
                Vouchers = await query.OrderByDescending(voucher => voucher.Id).ToListAsync()
            };
        }

        private async Task<VoucherSetupPageViewModel> BuildRulesPageAsync(string? search, VoucherRuleForm? form = null, int? editId = null, string activeModalId = "")
        {
            var query = _db.VoucherRules.AsNoTracking().AsQueryable();
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(rule => rule.Name.Contains(searchText) || rule.RewardType.Contains(searchText) || rule.AppliesTo.Contains(searchText));
            }

            return new VoucherSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                VoucherRuleForm = form ?? await BuildVoucherRuleFormAsync(editId),
                ProductOptions = await BuildProductOptionsAsync(),
                CategoryOptions = await BuildCategoryOptionsAsync(),
                VoucherRules = await query.OrderBy(rule => rule.Priority).ThenBy(rule => rule.Id).ToListAsync()
            };
        }

        private async Task<VoucherForm> BuildVoucherFormAsync(int? editId)
        {
            var voucher = editId.HasValue ? await _db.Vouchers.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;
            return voucher is null ? new VoucherForm() : new VoucherForm { Id = voucher.Id, Code = voucher.Code, MemberId = voucher.MemberId, Status = voucher.Status };
        }

        private async Task<VoucherRuleForm> BuildVoucherRuleFormAsync(int? editId)
        {
            var rule = editId.HasValue ? await _db.VoucherRules.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value) : null;
            return rule is null ? new VoucherRuleForm() : new VoucherRuleForm
            {
                Id = rule.Id,
                Name = rule.Name,
                RewardType = rule.RewardType,
                RewardValue = rule.RewardValue,
                MaxDiscountAmount = rule.MaxDiscountAmount,
                MinimumPurchaseAmount = rule.MinimumPurchaseAmount,
                ProductIds = ParseIds(rule.ApplicableProductIds),
                CategoryIds = ParseIds(rule.ApplicableCategoryIds),
                AppliesTo = rule.AppliesTo,
                EffectiveDate = rule.EffectiveDate,
                ExpirationDate = rule.ExpirationDate,
                NoExpiration = rule.NoExpiration,
                UnlimitedUses = string.Equals(rule.UsageLimitType, "Unlimited", StringComparison.OrdinalIgnoreCase),
                MaxRedemptions = rule.MaxRedemptions,
                UsageLimitType = rule.UsageLimitType,
                LimitedUseCount = rule.LimitedUseCount,
                Priority = rule.Priority,
                Status = rule.Status
            };
        }

        private async Task<string> GenerateVoucherCodeAsync()
        {
            for (var attempts = 0; attempts < 20; attempts += 1)
            {
                var code = new string(RandomNumberGenerator.GetItems(VoucherAlphabet.AsSpan(), 8));
                if (!await _db.Vouchers.AnyAsync(voucher => voucher.Code == code))
                {
                    return code;
                }
            }

            throw new InvalidOperationException("Unable to generate a unique voucher code. Please try again.");
        }

        private void ValidateVoucherRule(VoucherRuleForm form)
        {
            if (form.RewardType == "Percentage" && (!form.RewardValue.HasValue || form.RewardValue <= 0 || form.RewardValue > 100))
                ModelState.AddModelError("VoucherRuleForm.RewardValue", "Percentage value must be greater than 0 and not greater than 100.");
            if (form.RewardType == "Fixed Amount" && (!form.RewardValue.HasValue || form.RewardValue <= 0))
                ModelState.AddModelError("VoucherRuleForm.RewardValue", "Fixed amount must be greater than 0.");
            if (!form.NoExpiration && !form.ExpirationDate.HasValue)
                ModelState.AddModelError("VoucherRuleForm.ExpirationDate", "Expiration Date is required unless No Expiration is enabled.");
            if (!form.NoExpiration && form.EffectiveDate.HasValue && form.ExpirationDate.HasValue && form.ExpirationDate.Value.Date < form.EffectiveDate.Value.Date)
                ModelState.AddModelError("VoucherRuleForm.ExpirationDate", "Expiration Date must not be earlier than Effective Date.");
            if (!form.UnlimitedUses && !form.MaxRedemptions.HasValue)
                ModelState.AddModelError("VoucherRuleForm.MaxRedemptions", "Maximum redemptions is required unless unlimited uses is enabled.");
            if (form.UsageLimitType == "Limited Uses" && !form.LimitedUseCount.HasValue)
                ModelState.AddModelError("VoucherRuleForm.LimitedUseCount", "Limited use count is required.");
        }

        private async Task<List<SelectListItem>> BuildMemberOptionsAsync() => await _db.Members.AsNoTracking().Where(item => item.Status == 1).OrderBy(item => item.FullName).Select(item => new SelectListItem { Value = item.Id.ToString(), Text = $"{item.FullName} ({item.MemberNo})" }).ToListAsync();
        private async Task<List<SelectListItem>> BuildProductOptionsAsync() => await _db.Products.AsNoTracking().Where(item => item.Status == 1 && item.IsActive).OrderBy(item => item.Name).Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Name }).ToListAsync();
        private async Task<List<SelectListItem>> BuildCategoryOptionsAsync() => await _db.ProductCategories.AsNoTracking().Where(item => item.Status == 1 && item.IsActive).OrderBy(item => item.Name).Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.Name }).ToListAsync();

        private static string? JoinIds(List<int> ids) => ids.Count == 0 ? null : string.Join(",", ids.Distinct().OrderBy(id => id));
        private static List<int> ParseIds(string? value) => (value ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(item => int.TryParse(item, out var id) ? id : 0).Where(id => id > 0).ToList();
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class VoucherForm
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Customer is required.")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = "Active";
    }

    public class VoucherRuleForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reward Type is required.")]
        public string RewardType { get; set; } = "Fixed Amount";

        [Required(ErrorMessage = "Reward Value is required.")]
        public decimal? RewardValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Maximum discount cannot be negative.")]
        public decimal? MaxDiscountAmount { get; set; }

        [Required(ErrorMessage = "Minimum Purchase is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum Purchase cannot be negative.")]
        public decimal? MinimumPurchaseAmount { get; set; }

        public List<int> ProductIds { get; set; } = new();
        public List<int> CategoryIds { get; set; } = new();

        [Required(ErrorMessage = "Applies To is required.")]
        public string AppliesTo { get; set; } = "Both";

        [DataType(DataType.Date)]
        public DateTime? EffectiveDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }

        public bool NoExpiration { get; set; } = true;
        public bool UnlimitedUses { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Maximum redemptions must be at least 1.")]
        public int? MaxRedemptions { get; set; }

        [Required(ErrorMessage = "Usage Limit is required.")]
        public string UsageLimitType { get; set; } = "Once Per Voucher";

        [Range(1, int.MaxValue, ErrorMessage = "Limited use count must be at least 1.")]
        public int? LimitedUseCount { get; set; }

        public int Priority { get; set; }

        [Range(0, 1, ErrorMessage = "Status is required.")]
        public int Status { get; set; } = 1;
    }

    public class VoucherSetupPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public string ActiveModalId { get; set; } = string.Empty;
        public VoucherForm VoucherForm { get; set; } = new();
        public VoucherRuleForm VoucherRuleForm { get; set; } = new();
        public List<Voucher> Vouchers { get; set; } = new();
        public List<VoucherRule> VoucherRules { get; set; } = new();
        public List<SelectListItem> MemberOptions { get; set; } = new();
        public List<SelectListItem> ProductOptions { get; set; } = new();
        public List<SelectListItem> CategoryOptions { get; set; } = new();
    }
}

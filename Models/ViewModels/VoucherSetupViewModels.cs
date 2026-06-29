using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class VoucherForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(8, ErrorMessage = "Name must be 8 characters or fewer.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = "Active";
    }

    public class VoucherRuleForm
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Voucher is required.")]
        public int VoucherId { get; set; }

        [Required(ErrorMessage = "Applies To is required.")]
        public string AppliesTo { get; set; } = "Both";

        [Required(ErrorMessage = "Discount Type is required.")]
        public string DiscountType { get; set; } = "Percentage";

        [Required(ErrorMessage = "Discount Value is required.")]
        public decimal? DiscountValue { get; set; }

        [Required(ErrorMessage = "Minimum Amount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum Amount cannot be negative.")]
        public decimal? MinimumAmount { get; set; }

        public bool MemberRequired { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Maximum discount cannot be negative.")]
        public decimal? MaxDiscountAmount { get; set; }

        public List<int> ProductIds { get; set; } = new();
        public List<int> CategoryIds { get; set; } = new();

        public bool NoExpiration { get; set; }
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
        public List<SelectListItem> VoucherOptions { get; set; } = new();
        public List<SelectListItem> ProductOptions { get; set; } = new();
        public List<SelectListItem> CategoryOptions { get; set; } = new();
    }
}

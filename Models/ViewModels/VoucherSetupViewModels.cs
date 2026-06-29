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
        public string Code { get; set; } = string.Empty;

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

        [Required(ErrorMessage = "Usage Limit is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Usage Limit must be at least 1.")]
        public int? UsageLimit { get; set; } = 1;

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
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class DiscountForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Range(0, 1, ErrorMessage = "Status is required.")]
        public int Status { get; set; } = 1;
    }

    public class EarningRuleForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Earn Rate is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Earn Rate cannot be negative.")]
        public decimal? EarnRate { get; set; }

        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class MemberForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Member No is required.")]
        public string MemberNo { get; set; } = string.Empty;

        public string? CardNo { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; } = string.Empty;

        public string? ContactNumber { get; set; }
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int? DiscountId { get; set; }
        public int? EarningRuleId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Points cannot be negative.")]
        public decimal Points { get; set; }

        [Range(0, 1, ErrorMessage = "Status is required.")]
        public int Status { get; set; } = 1;
    }

    public class DiscountSetupPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public string ActiveModalId { get; set; } = string.Empty;
        public DiscountForm DiscountForm { get; set; } = new();
        public EarningRuleForm EarningRuleForm { get; set; } = new();
        public MemberForm MemberForm { get; set; } = new();
        public List<Discount> Discounts { get; set; } = new();
        public List<EarningRule> EarningRules { get; set; } = new();
        public List<Member> Members { get; set; } = new();
        public List<SelectListItem> DiscountOptions { get; set; } = new();
        public List<SelectListItem> EarningRuleOptions { get; set; } = new();
    }
}

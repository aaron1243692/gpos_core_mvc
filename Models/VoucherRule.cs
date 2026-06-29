namespace gpos.Models
{
    public class VoucherRule
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? VoucherId { get; set; }
        public string RewardType { get; set; } = "Fixed Amount";
        public decimal RewardValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal MinimumPurchaseAmount { get; set; }
        public int MemberRequired { get; set; }
        public string? ApplicableProductIds { get; set; }
        public string? ApplicableCategoryIds { get; set; }
        public string AppliesTo { get; set; } = "Both";
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool NoExpiration { get; set; }
        public int? MaxRedemptions { get; set; }
        public string UsageLimitType { get; set; } = "Once Per Voucher";
        public int? LimitedUseCount { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Voucher? Voucher { get; set; }
        public ICollection<VoucherRedemption> Redemptions { get; set; } = new List<VoucherRedemption>();

        public string DiscountType
        {
            get => RewardType;
            set => RewardType = value;
        }

        public decimal DiscountValue
        {
            get => RewardValue;
            set => RewardValue = value;
        }

        public decimal MinimumAmount
        {
            get => MinimumPurchaseAmount;
            set => MinimumPurchaseAmount = value;
        }

        public DateTime? StartDate
        {
            get => EffectiveDate;
            set => EffectiveDate = value;
        }

        public DateTime? EndDate
        {
            get => ExpirationDate;
            set => ExpirationDate = value;
        }
    }
}

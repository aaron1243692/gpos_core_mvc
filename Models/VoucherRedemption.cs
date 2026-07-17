namespace gpos.Models
{
    public class VoucherRedemption
    {
        public int Id { get; set; }
        public int? VoucherId { get; set; }
        public int VoucherRuleId { get; set; }
        public int SaleId { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? VoucherCodeSnapshot { get; set; }
        public string? RuleNameSnapshot { get; set; }
        public string? RewardTypeSnapshot { get; set; }
        public decimal? RewardValueSnapshot { get; set; }
        public decimal? EligibleBaseSnapshot { get; set; }
        public DateTime? CreatedAt { get; set; }

        public Voucher? Voucher { get; set; }
        public VoucherRule? VoucherRule { get; set; }
        public Sale? Sale { get; set; }
    }
}

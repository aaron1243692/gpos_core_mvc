namespace gpos.Models
{
    public class VoucherRedemptionReversal
    {
        public int Id { get; set; }
        public int SaleVoidId { get; set; }
        public int VoucherRedemptionId { get; set; }
        public int? VoucherId { get; set; }
        public int? MemberId { get; set; }
        public string? VoucherCodeSnapshot { get; set; }
        public decimal AppliedDiscountAmount { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}

namespace gpos.Models
{
    public class SaleVoidPayment
    {
        public int Id { get; set; }
        public int SaleVoidId { get; set; }
        public int PaymentId { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public decimal OriginalAppliedAmount { get; set; }
        public decimal ReversedAmount { get; set; }
        public decimal? TenderedAmountSnapshot { get; set; }
        public decimal? ChangeAmountSnapshot { get; set; }
        public string? ReferenceNoSnapshot { get; set; }
        public string ExternalRefundStatus { get; set; } = "Not Applicable";
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public SaleVoid? SaleVoid { get; set; }
        public Payment? Payment { get; set; }
        public User? CreatedByUser { get; set; }
    }
}

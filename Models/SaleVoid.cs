namespace gpos.Models
{
    public class SaleVoid
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int BranchId { get; set; }
        public int? OriginalDailyCashId { get; set; }
        public int DailyCashId { get; set; }
        public int RequestedByUserId { get; set; }
        public string? ReasonCode { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string OriginalReceiptNo { get; set; } = string.Empty;
        public DateTime? OriginalBusinessDate { get; set; }
        public DateTime VoidBusinessDate { get; set; }
        public decimal OriginalGrossTotal { get; set; }
        public decimal OriginalDiscountAmount { get; set; }
        public decimal OriginalRebateAmount { get; set; }
        public string? OriginalVatType { get; set; }
        public decimal? OriginalVatRate { get; set; }
        public decimal OriginalTaxableAmount { get; set; }
        public decimal OriginalVatAmount { get; set; }
        public decimal ReversedVatAmount { get; set; }
        public decimal OriginalNetTotal { get; set; }
        public decimal OriginalAppliedPaymentAmount { get; set; }
        public string Status { get; set; } = "Completed";
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Sale? Sale { get; set; }
        public Branch? Branch { get; set; }
        public DailyCash? OriginalDailyCash { get; set; }
        public DailyCash? DailyCash { get; set; }
        public User? RequestedByUser { get; set; }
        public ICollection<SaleVoidProductItem> ProductItems { get; set; } = new List<SaleVoidProductItem>();
        public ICollection<SaleVoidPayment> Payments { get; set; } = new List<SaleVoidPayment>();
    }
}

namespace gpos.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int? BranchId { get; set; }
        public int? DailyCashId { get; set; }
        public int? ShiftId { get; set; }
        public DateTime? BusinessDate { get; set; }
        public int? VatSettingId { get; set; }
        public string? VatNameSnapshot { get; set; }
        public string? VatTypeSnapshot { get; set; }
        public decimal? VatRate { get; set; }
        public decimal? TaxableAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? VatExemptAmount { get; set; }
        public int? MemberId { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal RebateAmount { get; set; }
        public decimal NetTotal { get; set; }
        public decimal CashAmount { get; set; }
        public decimal? ChangeAmount { get; set; }
        public string Status { get; set; } = "Completed";
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public User? User { get; set; }
        public Branch? Branch { get; set; }
        public DailyCash? DailyCash { get; set; }
        public ShiftSetting? Shift { get; set; }
        public VatSetting? VatSetting { get; set; }
        public Member? Member { get; set; }
        public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
        public ICollection<ProductSale> ProductSales { get; set; } = new List<ProductSale>();
        public ICollection<FuelSale> FuelSales { get; set; } = new List<FuelSale>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<PointsLedger> PointsLedger { get; set; } = new List<PointsLedger>();
        public ICollection<VoucherRedemption> VoucherRedemptions { get; set; } = new List<VoucherRedemption>();
        public ICollection<SaleDiscountApplication> DiscountApplications { get; set; } = new List<SaleDiscountApplication>();
    }
}

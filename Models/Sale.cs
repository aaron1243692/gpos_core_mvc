namespace gpos.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int? MemberId { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal RebateAmount { get; set; }
        public decimal NetTotal { get; set; }
        public decimal CashAmount { get; set; }
        public string Status { get; set; } = "Completed";
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public User? User { get; set; }
        public Member? Member { get; set; }
        public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
        public ICollection<ProductSale> ProductSales { get; set; } = new List<ProductSale>();
        public ICollection<FuelSale> FuelSales { get; set; } = new List<FuelSale>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<PointsLedger> PointsLedger { get; set; } = new List<PointsLedger>();
    }
}

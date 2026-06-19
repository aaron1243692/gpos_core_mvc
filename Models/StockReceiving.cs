namespace gpos.Models
{
    public class StockReceiving
    {
        public int Id { get; set; }
        public string ReceivingNo { get; set; } = string.Empty;
        public int? SupplierId { get; set; }
        public DateTime ReceivedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Remarks { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Supplier? Supplier { get; set; }
        public ICollection<StockReceivingItem> Items { get; set; } = new List<StockReceivingItem>();
    }
}

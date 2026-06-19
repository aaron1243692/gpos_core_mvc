namespace gpos.Models
{
    public class StockReceivingItem
    {
        public int Id { get; set; }
        public int StockReceivingId { get; set; }
        public int ProductId { get; set; }
        public int? ProductBatchId { get; set; }
        public decimal Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal Subtotal { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public StockReceiving? StockReceiving { get; set; }
        public Product? Product { get; set; }
        public ProductBatch? ProductBatch { get; set; }
    }
}

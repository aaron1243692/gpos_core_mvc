namespace gpos.Models
{
    public class ProductSale
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int? BatchId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
        public decimal? DisplayStockBefore { get; set; }
        public decimal? DisplayStockAfter { get; set; }
        public string Status { get; set; } = "Completed";
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Sale? Sale { get; set; }
        public Product? Product { get; set; }
        public ProductBatch? Batch { get; set; }
    }
}

namespace gpos.Models
{
    public class WarehouseStock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int BatchId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Product? Product { get; set; }
        public ProductBatch? Batch { get; set; }
    }
}

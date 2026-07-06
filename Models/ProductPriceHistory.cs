namespace gpos.Models
{
    public class ProductPriceHistory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? BatchId { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string? Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Product? Product { get; set; }
        public ProductBatch? Batch { get; set; }
    }
}

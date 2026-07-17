namespace gpos.Models
{
    public class StockMovement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? ProductBatchId { get; set; }
        public int? BranchId { get; set; }
        public int? DisplayStockId { get; set; }
        public decimal? BeforeQuantity { get; set; }
        public decimal? AfterQuantity { get; set; }
        public string? SourceLocation { get; set; }
        public string? DestinationLocation { get; set; }
        public string MovementType { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public string? Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }

        public Product? Product { get; set; }
        public ProductBatch? ProductBatch { get; set; }
        public Branch? Branch { get; set; }
        public DisplayStock? DisplayStock { get; set; }
    }
}

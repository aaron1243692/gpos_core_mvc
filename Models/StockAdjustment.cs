namespace gpos.Models
{
    public class StockAdjustment
    {
        public int Id { get; set; }
        public string AdjustmentNo { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public DateTime BusinessDate { get; set; }
        public int BranchId { get; set; }
        public int? WarehouseStockId { get; set; }
        public int? DisplayStockId { get; set; }
        public int? TankId { get; set; }
        public int? ProductId { get; set; }
        public int? BatchId { get; set; }
        public int? FuelId { get; set; }
        public string AdjustmentType { get; set; } = string.Empty;
        public decimal BeforeQuantity { get; set; }
        public decimal AdjustmentQuantity { get; set; }
        public decimal SignedQuantity { get; set; }
        public decimal AfterQuantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public string Status { get; set; } = "Draft";
        public int AdjustedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? PostedBy { get; set; }
        public DateTime? PostedAt { get; set; }
        public int? CancelledBy { get; set; }
        public DateTime? CancelledAt { get; set; }
        public int? ReversalOfAdjustmentId { get; set; }
        public int? ReversedByAdjustmentId { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? TotalCost { get; set; }
        public string? CostInputMode { get; set; }
        public string? EvidenceReference { get; set; }
        public int? CreatedFuelBatchId { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public Branch? Branch { get; set; }
        public WarehouseStock? WarehouseStock { get; set; }
        public DisplayStock? DisplayStock { get; set; }
        public Tank? Tank { get; set; }
        public Product? Product { get; set; }
        public ProductBatch? Batch { get; set; }
        public Fuel? Fuel { get; set; }
        public User? AdjustedByUser { get; set; }
        public User? PostedByUser { get; set; }
        public StockAdjustment? ReversalOfAdjustment { get; set; }
        public FuelBatch? CreatedFuelBatch { get; set; }
        public User? ApprovedByUser { get; set; }
    }
}

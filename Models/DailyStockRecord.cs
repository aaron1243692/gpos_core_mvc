namespace gpos.Models
{
    public class DailyStockRecord
    {
        public int Id { get; set; }
        public string RecordNo { get; set; } = string.Empty;
        public string StockType { get; set; } = string.Empty;
        public string Status { get; set; } = "Draft";
        public int? BranchId { get; set; }
        public DateTime StockDate { get; set; }
        public int? ProductId { get; set; }
        public int? BatchId { get; set; }
        public int? TankId { get; set; }
        public int? FuelId { get; set; }
        public int? WarehouseStockId { get; set; }
        public int? DisplayStockId { get; set; }
        public decimal Beginning { get; set; }
        public decimal Received { get; set; }
        public decimal TransferIn { get; set; }
        public decimal TransferOut { get; set; }
        public decimal Sold { get; set; }
        public decimal Adjustment { get; set; }
        public decimal Expected { get; set; }
        public decimal Actual { get; set; }
        public decimal Ending { get; set; }
        public decimal Variance { get; set; }
        public decimal Loss { get; set; }
        public decimal CurrentOfficialQuantity { get; set; }
        public decimal ReconciliationAdjustment { get; set; }
        public decimal NewOfficialQuantity { get; set; }
        public string? Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? ConfirmedBy { get; set; }
        public DateTime? ConfirmedAt { get; set; }

        public Branch? Branch { get; set; }
        public Product? Product { get; set; }
        public ProductBatch? Batch { get; set; }
        public Tank? Tank { get; set; }
        public Fuel? Fuel { get; set; }
        public User? CreatedByUser { get; set; }
        public User? ConfirmedByUser { get; set; }
    }
}

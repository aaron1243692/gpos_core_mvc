namespace gpos.Models
{
    public class DailyStockRecord
    {
        public int Id { get; set; }
        public string StockType { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public DateTime StockDate { get; set; }
        public int? ProductId { get; set; }
        public int? BatchId { get; set; }
        public int? TankId { get; set; }
        public int? FuelId { get; set; }
        public decimal Beginning { get; set; }
        public decimal Sold { get; set; }
        public decimal Actual { get; set; }
        public decimal Ending { get; set; }
        public decimal Loss { get; set; }
        public string? Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Branch? Branch { get; set; }
        public Product? Product { get; set; }
        public ProductBatch? Batch { get; set; }
        public Tank? Tank { get; set; }
        public Fuel? Fuel { get; set; }
    }
}

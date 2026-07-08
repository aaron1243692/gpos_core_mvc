namespace gpos.Models
{
    public class LowStockSetting
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? ProductBatchId { get; set; }
        public int? BranchId { get; set; }
        public int? TankId { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal MinimumQuantity { get; set; }
        public string? UnitLabel { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Product? Product { get; set; }
        public ProductBatch? ProductBatch { get; set; }
        public Branch? Branch { get; set; }
        public Tank? Tank { get; set; }
    }
}

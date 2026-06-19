namespace gpos.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public int? ProductUnitId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ProductCategory? Category { get; set; }
        public ProductUnit? ProductUnit { get; set; }
        public ICollection<ProductBatch> ProductBatches { get; set; } = new List<ProductBatch>();
        public ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
        public ICollection<DisplayStock> DisplayStocks { get; set; } = new List<DisplayStock>();
        public ICollection<StockReceivingItem> StockReceivingItems { get; set; } = new List<StockReceivingItem>();
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
        public ICollection<LowStockSetting> LowStockSettings { get; set; } = new List<LowStockSetting>();
    }
}

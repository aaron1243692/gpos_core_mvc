namespace gpos.Models
{
    public class ProductBatch
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? SupplierId { get; set; }
        public string BatchNo { get; set; } = string.Empty;
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int Status { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Product? Product { get; set; }
        public Supplier? Supplier { get; set; }
        public ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
        public ICollection<DisplayStock> DisplayStocks { get; set; } = new List<DisplayStock>();
        public ICollection<StockReceivingItem> StockReceivingItems { get; set; } = new List<StockReceivingItem>();
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
        public ICollection<LowStockSetting> LowStockSettings { get; set; } = new List<LowStockSetting>();
    }
}

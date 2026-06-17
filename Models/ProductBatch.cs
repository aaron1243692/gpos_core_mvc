namespace gpos.Models
{
    public class ProductBatch
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string BatchNo { get; set; } = string.Empty;
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Product? Product { get; set; }
        public ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
        public ICollection<DisplayStock> DisplayStocks { get; set; } = new List<DisplayStock>();
    }
}

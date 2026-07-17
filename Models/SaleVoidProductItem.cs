namespace gpos.Models
{
    public class SaleVoidProductItem
    {
        public int Id { get; set; }
        public int SaleVoidId { get; set; }
        public int ProductSaleId { get; set; }
        public int ProductId { get; set; }
        public int DisplayStockId { get; set; }
        public int BatchId { get; set; }
        public decimal QuantityRestored { get; set; }
        public decimal UnitCostSnapshot { get; set; }
        public decimal UnitPriceSnapshot { get; set; }
        public decimal RestoredValue { get; set; }
        public decimal BeforeQuantity { get; set; }
        public decimal AfterQuantity { get; set; }
        public int? StockMovementId { get; set; }
        public DateTime CreatedAt { get; set; }
        public SaleVoid? SaleVoid { get; set; }
        public ProductSale? ProductSale { get; set; }
        public Product? Product { get; set; }
        public DisplayStock? DisplayStock { get; set; }
        public ProductBatch? Batch { get; set; }
        public StockMovement? StockMovement { get; set; }
    }
}

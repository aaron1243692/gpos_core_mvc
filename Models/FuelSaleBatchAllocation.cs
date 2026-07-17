namespace gpos.Models
{
    public class FuelSaleBatchAllocation
    {
        public int Id { get; set; }
        public int FuelSaleId { get; set; }
        public int FuelBatchId { get; set; }
        public int TankId { get; set; }
        public int FuelId { get; set; }
        public int BranchId { get; set; }
        public decimal LitersAllocated { get; set; }
        public decimal BatchLitersBefore { get; set; }
        public decimal BatchLitersAfter { get; set; }
        public decimal UnitCostSnapshot { get; set; }
        public decimal TotalCostSnapshot { get; set; }
        public decimal UnitPriceSnapshot { get; set; }
        public decimal RevenueSnapshot { get; set; }
        public decimal GrossProfitSnapshot { get; set; }
        public int? FuelStockMovementId { get; set; }
        public DateTime CreatedAt { get; set; }
        public FuelSale? FuelSale { get; set; }
        public FuelBatch? FuelBatch { get; set; }
        public Tank? Tank { get; set; }
        public Fuel? Fuel { get; set; }
        public Branch? Branch { get; set; }
        public FuelStockMovement? FuelStockMovement { get; set; }
    }
}

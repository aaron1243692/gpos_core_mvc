namespace gpos.Models
{
    public class FuelBatch
    {
        public int Id { get; set; }
        public int FuelId { get; set; }
        public int? SupplierId { get; set; }
        public int? TankId { get; set; }
        public int? BranchId { get; set; }
        public int? FuelDeliveryId { get; set; }
        public string BatchNo { get; set; } = string.Empty;
        public decimal CostPricePerLiter { get; set; }
        public decimal SellingPricePerLiter { get; set; }
        public decimal ReceivedLiters { get; set; }
        public decimal RemainingLiters { get; set; }
        public DateTime ReceivedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Remarks { get; set; }
        public int Status { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Fuel? Fuel { get; set; }
        public Supplier? Supplier { get; set; }
        public Tank? Tank { get; set; }
        public Branch? Branch { get; set; }
        public FuelDelivery? FuelDelivery { get; set; }
        public ICollection<FuelSaleBatchAllocation> SaleAllocations { get; set; } = new List<FuelSaleBatchAllocation>();
    }
}

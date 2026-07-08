namespace gpos.Models
{
    public class FuelDelivery
    {
        public int Id { get; set; }
        public string DeliveryNo { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public int? SupplierId { get; set; }
        public int FuelId { get; set; }
        public int TankId { get; set; }
        public decimal DeliveredLiters { get; set; }
        public decimal? CostPerLiter { get; set; }
        public decimal? TotalCost { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? Remarks { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Branch? Branch { get; set; }
        public Supplier? Supplier { get; set; }
        public Fuel? Fuel { get; set; }
        public Tank? Tank { get; set; }
        public ICollection<FuelBatch> FuelBatches { get; set; } = new List<FuelBatch>();
    }
}

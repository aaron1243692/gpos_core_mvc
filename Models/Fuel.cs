namespace gpos.Models
{
    public class Fuel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int? SupplierId { get; set; }
        public decimal CurrentPricePerLiter { get; set; }
        public bool IsActive { get; set; } = true;
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Supplier? Supplier { get; set; }
        public ICollection<Tank> Tanks { get; set; } = new List<Tank>();
        public ICollection<FuelDelivery> FuelDeliveries { get; set; } = new List<FuelDelivery>();
        public ICollection<FuelPriceHistory> FuelPriceHistory { get; set; } = new List<FuelPriceHistory>();
    }
}

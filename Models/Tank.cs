namespace gpos.Models
{
    public class Tank
    {
        public int Id { get; set; }
        public int FuelId { get; set; }
        public string TankNo { get; set; } = string.Empty;
        public decimal CapacityLiters { get; set; }
        public decimal CurrentLiters { get; set; }
        public bool IsActive { get; set; } = true;
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Fuel? Fuel { get; set; }
        public ICollection<Pump> Pumps { get; set; } = new List<Pump>();
        public ICollection<FuelDelivery> FuelDeliveries { get; set; } = new List<FuelDelivery>();
    }
}

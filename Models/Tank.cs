namespace gpos.Models
{
    public class Tank
    {
        public int Id { get; set; }
        public int FuelId { get; set; }
        public string TankNo { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Fuel? Fuel { get; set; }
        public ICollection<Pump> Pumps { get; set; } = new List<Pump>();
    }
}

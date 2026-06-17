namespace gpos.Models
{
    public class Pump
    {
        public int Id { get; set; }
        public int TankId { get; set; }
        public string PumpNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Tank? Tank { get; set; }
    }
}

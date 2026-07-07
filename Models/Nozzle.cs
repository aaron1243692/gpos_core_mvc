namespace gpos.Models
{
    public class Nozzle
    {
        public int Id { get; set; }
        public int PumpId { get; set; }
        public int? TankId { get; set; }
        public string NozzleNo { get; set; } = string.Empty;
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Pump? Pump { get; set; }
        public Tank? Tank { get; set; }
        public ICollection<PumpMeterReading> PumpMeterReadings { get; set; } = new List<PumpMeterReading>();
    }
}

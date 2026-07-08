namespace gpos.Models
{
    public class Pump
    {
        public int Id { get; set; }
        public int? TankId { get; set; }
        public int? BranchId { get; set; }
        public string PumpNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Tank? Tank { get; set; }
        public Branch? Branch { get; set; }
        public ICollection<Nozzle> Nozzles { get; set; } = new List<Nozzle>();
        public ICollection<PumpMeterReading> PumpMeterReadings { get; set; } = new List<PumpMeterReading>();
    }
}

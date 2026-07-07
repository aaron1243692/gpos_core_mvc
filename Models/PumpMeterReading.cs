namespace gpos.Models
{
    public class PumpMeterReading
    {
        public int Id { get; set; }
        public int? PumpId { get; set; }
        public int? NozzleId { get; set; }
        public int? ShiftId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal OpeningMeter { get; set; }
        public decimal? ClosingMeter { get; set; }
        public decimal? LitersSold { get; set; }
        public string? Remarks { get; set; }
        public DateTime ReadingDate { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Pump? Pump { get; set; }
        public Nozzle? Nozzle { get; set; }
    }
}

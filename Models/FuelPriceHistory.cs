namespace gpos.Models
{
    public class FuelPriceHistory
    {
        public int Id { get; set; }
        public int FuelId { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime EffectiveAt { get; set; }
        public string? Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Fuel? Fuel { get; set; }
    }
}

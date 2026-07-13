namespace gpos.Models
{
    public class BranchFuelPrice
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int FuelId { get; set; }
        public decimal CurrentPricePerLiter { get; set; }
        public DateTime? EffectiveAt { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Branch? Branch { get; set; }
        public Fuel? Fuel { get; set; }
    }
}

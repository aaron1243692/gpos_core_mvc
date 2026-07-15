namespace gpos.Models
{
    public class Dispenser
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string DispenserCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Remarks { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Branch? Branch { get; set; }
        public ICollection<Pump> Pumps { get; set; } = new List<Pump>();
    }
}

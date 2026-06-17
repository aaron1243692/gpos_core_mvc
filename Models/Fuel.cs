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
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Supplier? Supplier { get; set; }
        public ICollection<Tank> Tanks { get; set; } = new List<Tank>();
    }
}

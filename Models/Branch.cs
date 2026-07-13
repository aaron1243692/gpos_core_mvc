namespace gpos.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Department> Departments { get; set; } = new List<Department>();
        public ICollection<BranchFuelPrice> FuelPrices { get; set; } = new List<BranchFuelPrice>();
    }
}

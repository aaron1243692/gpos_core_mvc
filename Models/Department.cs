namespace gpos.Models
{
    public class Department
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Branch? Branch { get; set; }
        public ICollection<EmployeeAccount> EmployeeAccounts { get; set; } = new List<EmployeeAccount>();
    }
}

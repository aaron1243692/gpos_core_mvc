namespace gpos.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public int DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Department? Department { get; set; }
        public Position? Position { get; set; }
        public EmployeeAccount? Account { get; set; }
    }
}

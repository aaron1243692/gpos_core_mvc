namespace gpos.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ScheduleDetail> Details { get; set; } = new List<ScheduleDetail>();
        public ICollection<EmployeeAccount> EmployeeAccounts { get; set; } = new List<EmployeeAccount>();
    }
}

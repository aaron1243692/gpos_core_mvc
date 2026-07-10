namespace gpos.Models
{
    public class ShiftSetting
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public decimal? OpeningCashAmount { get; set; }
        public int RequireOpeningCash { get; set; } = 1;
        public int AllowCashIn { get; set; } = 1;
        public int AllowCashOut { get; set; } = 1;
        public int RequireClosingApproval { get; set; }
        public int Status { get; set; } = 1;
        public string? Remarks { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<EmployeeShiftSchedule> EmployeeShiftSchedules { get; set; } = new List<EmployeeShiftSchedule>();
    }
}

namespace gpos.Models
{
    public class EmployeeShiftSchedule
    {
        public int Id { get; set; }
        public int EmployeeAccountId { get; set; }
        public int? ShiftSettingId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string DayName => DayOfWeek switch
        {
            1 => "Monday",
            2 => "Tuesday",
            3 => "Wednesday",
            4 => "Thursday",
            5 => "Friday",
            6 => "Saturday",
            7 => "Sunday",
            _ => string.Empty
        };

        public EmployeeAccount? EmployeeAccount { get; set; }
        public ShiftSetting? ShiftSetting { get; set; }
    }
}

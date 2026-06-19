namespace gpos.Models
{
    public class ScheduleDetail
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan? AmIn { get; set; }
        public TimeSpan? AmOut { get; set; }
        public TimeSpan? PmIn { get; set; }
        public TimeSpan? PmOut { get; set; }
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

        public Schedule? Schedule { get; set; }
    }
}

namespace gpos.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? Username { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Module { get; set; }
        public string? Description { get; set; }
        public string? IpAddress { get; set; }
        public DateTime? CreatedAt { get; set; }

        public User? User { get; set; }
    }
}

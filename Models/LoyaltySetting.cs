namespace gpos.Models
{
    public class LoyaltySetting
    {
        public int Id { get; set; }
        public string SettingKey { get; set; } = string.Empty;
        public decimal DecimalValue { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

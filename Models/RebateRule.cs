namespace gpos.Models
{
    public class RebateRule
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal PointsRequired { get; set; }
        public decimal RebateValue { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

namespace gpos.Models
{
    public class FinancialMetric
    {
        public int Id { get; set; }
        public string MetricCode { get; set; } = string.Empty;
        public decimal OldAmount { get; set; }
        public decimal NewAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime MetricDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

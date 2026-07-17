namespace gpos.Models
{
    public class SaleDiscountApplication
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public string SourceType { get; set; } = string.Empty;
        public int? RuleId { get; set; }
        public string? RuleNameSnapshot { get; set; }
        public string? CalculationTypeSnapshot { get; set; }
        public decimal? RateOrValueSnapshot { get; set; }
        public decimal EligibleBaseSnapshot { get; set; }
        public decimal AppliedAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        public Sale? Sale { get; set; }
    }
}

namespace gpos.Models
{
    public class DiscountRule
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DiscountId { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public string AppliesTo { get; set; } = string.Empty;
        public decimal MinimumAmount { get; set; }
        public int MemberRequired { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Discount? Discount { get; set; }
    }
}

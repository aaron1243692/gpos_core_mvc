namespace gpos.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string MemberNo { get; set; } = string.Empty;
        public string? CardNo { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int? DiscountId { get; set; }
        public int? EarningsId { get; set; }
        public decimal Points { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Discount? Discount { get; set; }
        public Earnings? Earnings { get; set; }
        public ICollection<PointsLedger> PointsLedger { get; set; } = new List<PointsLedger>();
    }
}

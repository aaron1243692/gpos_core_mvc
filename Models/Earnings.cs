namespace gpos.Models
{
    public class Earnings
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Member> Members { get; set; } = new List<Member>();
        public ICollection<EarningRule> EarningRules { get; set; } = new List<EarningRule>();
    }
}

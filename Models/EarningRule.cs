namespace gpos.Models
{
    public class EarningRule
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal EarnRate { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Member> Members { get; set; } = new List<Member>();
    }
}

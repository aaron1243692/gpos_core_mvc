namespace gpos.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public int MemberId { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Member? Member { get; set; }
        public ICollection<VoucherRedemption> Redemptions { get; set; } = new List<VoucherRedemption>();
    }
}

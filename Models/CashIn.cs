namespace gpos.Models
{
    public class CashIn
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ShiftId { get; set; }
        public int UserId { get; set; }
        public int? DailyCashId { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public int? CreatedByUserId { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Branch? Branch { get; set; }
        public ShiftSetting? Shift { get; set; }
        public User? User { get; set; }
        public DailyCash? DailyCash { get; set; }
        public User? CreatedByUser { get; set; }
    }
}

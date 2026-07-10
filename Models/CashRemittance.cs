namespace gpos.Models
{
    public class CashRemittance
    {
        public int Id { get; set; }
        public string RemittanceNo { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public int ShiftId { get; set; }
        public int UserId { get; set; }
        public int DailyCashId { get; set; }
        public decimal ExpectedCash { get; set; }
        public decimal ActualCash { get; set; }
        public decimal RemittedAmount { get; set; }
        public decimal RemittanceDifference { get; set; }
        public int ReceivedByUserId { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public string? Remarks { get; set; }
        public int Status { get; set; } = 1;
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Branch? Branch { get; set; }
        public ShiftSetting? Shift { get; set; }
        public User? User { get; set; }
        public DailyCash? DailyCash { get; set; }
        public User? ReceivedByUser { get; set; }
        public User? CreatedByUser { get; set; }
    }
}

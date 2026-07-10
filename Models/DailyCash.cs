namespace gpos.Models
{
    public class DailyCash
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ShiftId { get; set; }
        public int UserId { get; set; }
        public DateTime BusinessDate { get; set; }
        public decimal OpeningCash { get; set; }
        public decimal CashSales { get; set; }
        public decimal TotalCashIn { get; set; }
        public decimal TotalCashOut { get; set; }
        public decimal ExpectedCash { get; set; }
        public decimal ActualCash { get; set; }
        public decimal Difference { get; set; }
        public decimal RemittedAmount { get; set; }
        public int? ReceivedByUserId { get; set; }
        public string? Remarks { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? OpenedAt { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Branch? Branch { get; set; }
        public ShiftSetting? Shift { get; set; }
        public User? User { get; set; }
        public User? ReceivedByUser { get; set; }
        public User? CreatedByUser { get; set; }
        public ICollection<CashIn> CashIns { get; set; } = new List<CashIn>();
        public ICollection<CashOut> CashOuts { get; set; } = new List<CashOut>();
        public ICollection<CashRemittance> CashRemittances { get; set; } = new List<CashRemittance>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}

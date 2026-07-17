namespace gpos.Models
{
    public class SaleVoidCashAdjustment
    {
        public int Id { get; set; }
        public int SaleVoidId { get; set; }
        public int SaleId { get; set; }
        public int DailyCashId { get; set; }
        public int BranchId { get; set; }
        public int UserId { get; set; }
        public DateTime BusinessDate { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

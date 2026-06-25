namespace gpos.Models
{
    public class PointsLedger
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Points { get; set; }
        public decimal OldPoints { get; set; }
        public decimal NewPoints { get; set; }
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public int? SaleId { get; set; }
        public string? Remarks { get; set; }
        public DateTime? CreatedAt { get; set; }

        public Member? Member { get; set; }
        public Sale? Sale { get; set; }
    }
}

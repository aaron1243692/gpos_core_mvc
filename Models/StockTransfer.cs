namespace gpos.Models
{
    public class StockTransfer
    {
        public int Id { get; set; }
        public string TransferNo { get; set; } = string.Empty;
        public string TransferType { get; set; } = string.Empty;
        public int? SourceBranchId { get; set; }
        public int? DestinationBranchId { get; set; }
        public string? SourceLocation { get; set; }
        public string? DestinationLocation { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Remarks { get; set; }
        public int? TransferredBy { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Branch? SourceBranch { get; set; }
        public Branch? DestinationBranch { get; set; }
        public User? User { get; set; }
        public ICollection<StockTransferItem> Items { get; set; } = new List<StockTransferItem>();
    }
}

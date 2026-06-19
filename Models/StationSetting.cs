namespace gpos.Models
{
    public class StationSetting
    {
        public int Id { get; set; }
        public string StationName { get; set; } = string.Empty;
        public string? BusinessName { get; set; }
        public string? Address { get; set; }
        public string? Tin { get; set; }
        public string? ReceiptHeader { get; set; }
        public string? ReceiptFooter { get; set; }
        public int? DefaultBranchId { get; set; }
        public string Currency { get; set; } = "PHP";
        public int TaxEnabled { get; set; }
        public decimal TaxRate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Branch? DefaultBranch { get; set; }
    }
}

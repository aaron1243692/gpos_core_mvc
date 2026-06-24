namespace gpos.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int? PaymentMethodId { get; set; }
        public string PaymentType { get; set; } = "Cash";
        public decimal Amount { get; set; }
        public string? ReferenceNo { get; set; }
        public string Status { get; set; } = "Completed";
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Sale? Sale { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
    }
}

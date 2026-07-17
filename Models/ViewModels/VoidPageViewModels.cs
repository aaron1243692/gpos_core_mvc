namespace gpos.Models.ViewModels
{
    public class VoidPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool CanCreate { get; set; }
        public List<VoidSaleRowViewModel> Sales { get; set; } = new();
    }

    public class VoidSaleRowViewModel
    {
        public int SaleId { get; set; }
        public int? SaleVoidId { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public DateTime? BusinessDate { get; set; }
        public string Cashier { get; set; } = string.Empty;
        public string SaleType { get; set; } = string.Empty;
        public decimal Gross { get; set; }
        public decimal Discount { get; set; }
        public decimal Vat { get; set; }
        public decimal Net { get; set; }
        public string Payment { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool Eligible { get; set; }
        public string EligibilityMessage { get; set; } = string.Empty;
    }
}

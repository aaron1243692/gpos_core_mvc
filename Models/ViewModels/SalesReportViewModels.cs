using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels;

public class SalesReportPageViewModel
{
    public string Search { get; set; } = string.Empty;
    public int? BranchId { get; set; }
    public int? UserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public int CompletedCount { get; set; }
    public int VoidedCount { get; set; }
    public decimal CompletedAmount { get; set; }
    public decimal VoidedOriginalAmount { get; set; }
    public List<SalesReportRowViewModel> Rows { get; set; } = new();
    public List<SelectListItem> BranchOptions { get; set; } = new();
    public List<SelectListItem> UserOptions { get; set; } = new();
    public SalesReportDetailsViewModel? Details { get; set; }
}

public class SalesReportRowViewModel
{
    public int Id { get; set; }
    public string ReceiptNo { get; set; } = string.Empty;
    public DateTime? SaleDate { get; set; }
    public string Branch { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public bool HasProducts { get; set; }
    public bool HasFuel { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal OriginalTotal { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? VoidDate { get; set; }
    public string VoidedBy { get; set; } = string.Empty;
    public string SaleType => HasProducts && HasFuel ? "Mixed" : HasFuel ? "Fuel" : HasProducts ? "Product" : "Unknown";
}

public class SalesReportDetailsViewModel : SalesReportRowViewModel
{
    public string VoidReason { get; set; } = string.Empty;
    public int? SaleVoidId { get; set; }
    public string SaleVoidStatus { get; set; } = string.Empty;
    public List<SalesReportProductItemViewModel> ProductItems { get; set; } = new();
    public List<SalesReportFuelItemViewModel> FuelItems { get; set; } = new();
}

public class SalesReportProductItemViewModel
{
    public string Product { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class SalesReportFuelItemViewModel
{
    public string Fuel { get; set; } = string.Empty;
    public string Tank { get; set; } = string.Empty;
    public string Pump { get; set; } = string.Empty;
    public decimal Liters { get; set; }
    public decimal PricePerLiter { get; set; }
    public decimal LineTotal { get; set; }
}

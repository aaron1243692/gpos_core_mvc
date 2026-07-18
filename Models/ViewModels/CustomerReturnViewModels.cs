using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels;

public class ReturnCreatePageViewModel
{
    public string ReceiptNo { get; set; } = string.Empty;
    public ReturnSaleViewModel? Sale { get; set; }
}

public class ReturnSaleViewModel
{
    public int SaleId { get; set; }
    public string ReceiptNo { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string Cashier { get; set; } = string.Empty;
    public List<ReturnableProductViewModel> Products { get; set; } = new();
    public List<ReturnableFuelViewModel> Fuels { get; set; } = new();
}

public class ReturnableProductViewModel
{
    public int ProductSaleId { get; set; }
    public string Product { get; set; } = string.Empty;
    public string Batch { get; set; } = string.Empty;
    public decimal Sold { get; set; }
    public decimal Returned { get; set; }
    public decimal Remaining => Sold - Returned;
    public decimal UnitPrice { get; set; }
}

public class ReturnableFuelViewModel
{
    public int FuelSaleId { get; set; }
    public string Fuel { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public decimal Sold { get; set; }
    public decimal Returned { get; set; }
    public decimal Remaining => Sold - Returned;
    public decimal UnitPrice { get; set; }
}

public class ReturnManagementPageViewModel
{
    public string Search { get; set; } = string.Empty;
    public int? BranchId { get; set; }
    public string ReturnType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public List<SelectListItem> BranchOptions { get; set; } = new();
    public List<CustomerReturnRowViewModel> Rows { get; set; } = new();
    public CustomerReturnDetailsViewModel? Details { get; set; }
}

public class CustomerReturnRowViewModel
{
    public int Id { get; set; }
    public string ReturnNo { get; set; } = string.Empty;
    public string ReceiptNo { get; set; } = string.Empty;
    public DateTime ReturnDate { get; set; }
    public string Branch { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string ReturnType { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CustomerReturnDetailsViewModel : CustomerReturnRowViewModel
{
    public string Reason { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string Inspector { get; set; } = string.Empty;
    public DateTime? InspectedAt { get; set; }
    public string Decision { get; set; } = string.Empty;
    public string InspectionNotes { get; set; } = string.Empty;
    public string ProcessedBy { get; set; } = string.Empty;
    public DateTime? CompletedAt { get; set; }
    public List<CustomerProductReturnDetailViewModel> Products { get; set; } = new();
    public List<CustomerFuelReturnDetailViewModel> Fuels { get; set; } = new();
}

public class CustomerProductReturnDetailViewModel
{
    public int Id { get; set; }
    public string Product { get; set; } = string.Empty;
    public string Batch { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public string InspectionResult { get; set; } = string.Empty;
    public string Disposition { get; set; } = string.Empty;
}

public class CustomerFuelReturnDetailViewModel
{
    public int Id { get; set; }
    public string Fuel { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public decimal Liters { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public string InspectionResult { get; set; } = string.Empty;
    public string Disposition { get; set; } = string.Empty;
}

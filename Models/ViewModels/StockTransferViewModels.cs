using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace gpos.Models.ViewModels
{
    public class StockTransferPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public int? SourceFilterBranchId { get; set; }
        public string SourceFilterBranchName { get; set; } = string.Empty;
        public int? DestinationFilterBranchId { get; set; }
        public string DestinationFilterBranchName { get; set; } = string.Empty;
        public string TransferType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string SaveAction { get; set; } = string.Empty;
        public string CompleteAction { get; set; } = string.Empty;
        public string CancelAction { get; set; } = string.Empty;
        public StockTransferForm Form { get; set; } = new();
        public List<StockTransferRowViewModel> Transfers { get; set; } = new();
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<SelectListItem> ProductOptions { get; set; } = new();
        public List<StockTransferProductSelectorRowViewModel> ProductSelectorRows { get; set; } = new();
        public List<SelectListItem> BatchOptions { get; set; } = new();
        public List<SelectListItem> FuelOptions { get; set; } = new();
        public List<SelectListItem> TankOptions { get; set; } = new();
    }

    public class StockTransferProductSelectorRowViewModel
    {
        public int ProductId { get; set; }
        public int? BranchId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = "-";
        public string UnitName { get; set; } = "-";
        public decimal AvailableQuantity { get; set; }
    }

    public class StockTransferForm
    {
        public int Id { get; set; }
        public int? SourceBranchId { get; set; }
        public int? DestinationBranchId { get; set; }
        public int? ProductId { get; set; }
        public int? BatchId { get; set; }
        public int? FuelId { get; set; }
        public int? SourceTankId { get; set; }
        public int? DestinationTankId { get; set; }

        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Quantity must be greater than 0.")]
        public decimal Quantity { get; set; }

        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Liters must be greater than 0.")]
        public decimal Liters { get; set; }

        public string? Remarks { get; set; }
        public string Status { get; set; } = "Pending";
    }

    public class StockTransferRowViewModel
    {
        public int Id { get; set; }
        public string TransferNo { get; set; } = "-";
        public string SourceBranch { get; set; } = "-";
        public string DestinationBranch { get; set; } = "-";
        public string Product { get; set; } = "-";
        public string Batch { get; set; } = "-";
        public string Fuel { get; set; } = "-";
        public string SourceTank { get; set; } = "-";
        public string DestinationTank { get; set; } = "-";
        public decimal Quantity { get; set; }
        public decimal Liters { get; set; }
        public decimal? SourceBefore { get; set; }
        public decimal? SourceAfter { get; set; }
        public decimal? DestinationBefore { get; set; }
        public decimal? DestinationAfter { get; set; }
        public string TransferredBy { get; set; } = "-";
        public DateTime? Date { get; set; }
        public string Status { get; set; } = "Pending";
        public string Remarks { get; set; } = string.Empty;
    }
}

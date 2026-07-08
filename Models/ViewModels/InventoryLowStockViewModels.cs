using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class LowStockThresholdForm
    {
        public int Id { get; set; }
        public string StockType { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Product is required.")]
        public int? ProductId { get; set; }

        public int? BranchId { get; set; }

        public string BranchName { get; set; } = string.Empty;

        public string SelectedProductName { get; set; } = string.Empty;

        public string SelectedTankName { get; set; } = string.Empty;

        public string SelectedFuelName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Tank is required.")]
        public int? TankId { get; set; }

        [Required(ErrorMessage = "Threshold is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Threshold must be 0 or greater.")]
        public decimal? Threshold { get; set; }

        public string? UnitLabel { get; set; }
    }

    public class LowStockSettingRow
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? BranchId { get; set; }
        public int? TankId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string TankName { get; set; } = string.Empty;
        public string FuelName { get; set; } = string.Empty;
        public string UnitLabel { get; set; } = string.Empty;
        public decimal CurrentQuantity { get; set; }
        public decimal Threshold { get; set; }
        public decimal Difference { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
    }

    public class LowStockSettingsPageViewModel
    {
        public string StockType { get; set; } = string.Empty;
        public string PageTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UnitLabel { get; set; } = string.Empty;
        public string QuantityLabel { get; set; } = string.Empty;
        public string SaveAction { get; set; } = string.Empty;
        public string PageAction { get; set; } = string.Empty;
        public string EmptyMessage { get; set; } = string.Empty;
        public string Search { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public string ActiveModalId { get; set; } = string.Empty;
        public LowStockThresholdForm Form { get; set; } = new();
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<SelectListItem> ProductOptions { get; set; } = new();
        public List<SelectListItem> TankOptions { get; set; } = new();
        public List<LowStockSettingRow> Settings { get; set; } = new();
    }
}

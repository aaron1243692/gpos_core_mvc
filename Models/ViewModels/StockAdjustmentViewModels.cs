using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace gpos.Models.ViewModels
{
    public class StockAdjustmentPageViewModel
    {
        public string Scope { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string PageAction { get; set; } = string.Empty;
        public string Search { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? Status { get; set; }
        public StockAdjustmentForm Form { get; set; } = new();
        public List<StockAdjustment> Rows { get; set; } = new();
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<StockAdjustmentTargetOption> Targets { get; set; } = new();
        public int? DetailsId { get; set; }
    }

    public class StockAdjustmentForm
    {
        public int Id { get; set; }
        public string Scope { get; set; } = string.Empty;
        [Required] public DateTime? BusinessDate { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Branch is required.")] public int BranchId { get; set; }
        public int? WarehouseStockId { get; set; }
        public int? DisplayStockId { get; set; }
        public int? TankId { get; set; }
        [Required] public string AdjustmentType { get; set; } = "Decrease";
        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Adjustment quantity must be greater than 0.")]
        public decimal AdjustmentQuantity { get; set; }
        [Required, StringLength(100)] public string Reason { get; set; } = string.Empty;
        [StringLength(1000)] public string? Remarks { get; set; }
    }

    public class StockAdjustmentTargetOption
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string Label { get; set; } = string.Empty;
        public string ProductOrFuel { get; set; } = string.Empty;
        public string BatchOrTank { get; set; } = string.Empty;
        public decimal CurrentQuantity { get; set; }
        public decimal? Capacity { get; set; }
    }
}

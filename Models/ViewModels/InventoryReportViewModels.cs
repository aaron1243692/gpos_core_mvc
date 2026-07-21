using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class InventoryReportPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<InventoryReportRowViewModel> Rows { get; set; } = new();
    }

    public class InventoryReportRowViewModel
    {
        public int? BranchId { get; set; }
        public int? ProductId { get; set; }
        public int? TankId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ProductOrTankName { get; set; } = string.Empty;
        public string FuelName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public decimal? Threshold { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

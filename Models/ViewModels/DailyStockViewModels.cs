using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class DailyStockForm
    {
        public int Id { get; set; }
        public string StockType { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Branch is required.")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime? StockDate { get; set; } = DateTime.Today;

        [Range(1, int.MaxValue, ErrorMessage = "Product is required.")]
        public int? ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Batch is required.")]
        public int? BatchId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Tank is required.")]
        public int? TankId { get; set; }

        [Required(ErrorMessage = "Beginning is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Beginning must be 0 or greater.")]
        public decimal? Beginning { get; set; }

        public decimal? Sold { get; set; }

        public decimal? Actual { get; set; }

        [Required(ErrorMessage = "Ending is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Ending must be 0 or greater.")]
        public decimal? Ending { get; set; }

        public decimal? Loss { get; set; }
        public string? Remarks { get; set; }
    }

    public class DailyStockOption
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string BatchNo { get; set; } = string.Empty;
        public string TankNo { get; set; } = string.Empty;
        public string FuelName { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public decimal CurrentQuantity { get; set; }
    }

    public class DailyStockPageViewModel
    {
        public string StockType { get; set; } = string.Empty;
        public string PageTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PageAction { get; set; } = string.Empty;
        public string SaveAction { get; set; } = string.Empty;
        public string DefaultAction { get; set; } = string.Empty;
        public string Search { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public string FormBranchName { get; set; } = string.Empty;
        public string ActiveModalId { get; set; } = string.Empty;
        public DailyStockForm Form { get; set; } = new();
        public List<DailyStockRecord> Records { get; set; } = new();
        public List<SelectListItem> ProductOptions { get; set; } = new();
        public List<SelectListItem> BatchOptions { get; set; } = new();
        public List<SelectListItem> TankOptions { get; set; } = new();
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<DailyStockOption> StockOptions { get; set; } = new();
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class ProductStockForm
    {
        public int? Id { get; set; }

        public int? CategoryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Branch is required.")]
        public int? BranchId { get; set; }

        public string BranchName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Product is required.")]
        public int? ProductId { get; set; }

        public string ProductDisplayName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Batch is required.")]
        public int? BatchId { get; set; }

        public string BatchDisplayName { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Cost Price cannot be negative.")]
        public decimal? CostPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Selling Price cannot be negative.")]
        public decimal? SellingPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public decimal? Quantity { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ProductCategoryForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class ProductStockPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string ActiveModalId { get; set; } = string.Empty;
        public ProductStockForm StockForm { get; set; } = new();
        public ProductCategoryForm CategoryForm { get; set; } = new();
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<SelectListItem> CategoryOptions { get; set; } = new();
        public List<DisplayStock> DisplayStocks { get; set; } = new();
        public List<WarehouseStock> WarehouseStocks { get; set; } = new();
        public List<ProductStockSummaryViewModel> DisplayStockSummaries { get; set; } = new();
        public List<ProductStockSummaryViewModel> WarehouseStockSummaries { get; set; } = new();
        public List<ProductCategory> Categories { get; set; } = new();
    }

    public class ProductStockSummaryViewModel
    {
        public int ProductId { get; set; }
        public int? BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public decimal TotalQuantity { get; set; }
        public decimal? TotalCostValue { get; set; }
        public bool IsActive { get; set; }
        public string StatusText => IsActive ? "Active" : "Inactive";
    }
}

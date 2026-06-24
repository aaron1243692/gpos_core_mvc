using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class ProductStockForm
    {
        public int? Id { get; set; }

        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cost Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Cost Price cannot be negative.")]
        public decimal? CostPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Selling Price cannot be negative.")]
        public decimal? SellingPrice { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public decimal? Quantity { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ProductCategoryForm
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class ProductStockPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public string ActiveModalId { get; set; } = string.Empty;
        public ProductStockForm StockForm { get; set; } = new();
        public ProductCategoryForm CategoryForm { get; set; } = new();
        public List<SelectListItem> CategoryOptions { get; set; } = new();
        public List<DisplayStock> DisplayStocks { get; set; } = new();
        public List<WarehouseStock> WarehouseStocks { get; set; } = new();
        public List<ProductCategory> Categories { get; set; } = new();
    }
}

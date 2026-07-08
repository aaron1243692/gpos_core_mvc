using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace gpos.Models.ViewModels
{
    public class ProductReceivingPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public string FormBranchName { get; set; } = string.Empty;
        public ProductReceivingForm Form { get; set; } = new();
        public List<ProductReceivingRowViewModel> Receivings { get; set; } = new();
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<SelectListItem> SupplierOptions { get; set; } = new();
        public List<SelectListItem> ProductOptions { get; set; } = new();
        public List<SelectListItem> CategoryOptions { get; set; } = new();
        public List<SelectListItem> UnitOptions { get; set; } = new();
    }

    public class ProductReceivingForm
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Branch is required.")]
        public int BranchId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Supplier is required.")]
        public int SupplierId { get; set; }

        public int ProductId { get; set; }

        public string ProductMode { get; set; } = "Existing";

        public string NewProductName { get; set; } = string.Empty;

        public int? NewProductCategoryId { get; set; }

        public int? NewProductUnitId { get; set; }

        public string? NewProductDescription { get; set; }

        public string BatchNo { get; set; } = string.Empty;

        public DateTime? ExpiryDate { get; set; }

        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Quantity must be greater than 0.")]
        public decimal Quantity { get; set; }

        [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Cost Price must be greater than or equal to 0.")]
        public decimal CostPrice { get; set; }

        [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Selling Price must be greater than or equal to 0.")]
        public decimal SellingPrice { get; set; }

        public string? Remarks { get; set; }

        [Range(0, 2)]
        public int Status { get; set; } = 1;
    }

    public class ProductReceivingRowViewModel
    {
        public int Id { get; set; }
        public string ReceivingNo { get; set; } = "-";
        public string BranchName { get; set; } = "-";
        public string Supplier { get; set; } = "-";
        public string Product { get; set; } = "-";
        public string Batch { get; set; } = "-";
        public decimal Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal? WarehouseStockBefore { get; set; }
        public decimal? WarehouseStockAfter { get; set; }
        public string ReceivedBy { get; set; } = "-";
        public DateTime Date { get; set; }
        public int Status { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }

    public class FuelReceivingPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public string FormBranchName { get; set; } = string.Empty;
        public FuelReceivingForm Form { get; set; } = new();
        public List<FuelReceivingRowViewModel> Receivings { get; set; } = new();
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<SelectListItem> SupplierOptions { get; set; } = new();
        public List<SelectListItem> FuelOptions { get; set; } = new();
        public List<SelectListItem> TankOptions { get; set; } = new();
    }

    public class FuelReceivingForm
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Branch is required.")]
        public int BranchId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Supplier is required.")]
        public int SupplierId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Fuel is required.")]
        public int FuelId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Tank is required.")]
        public int TankId { get; set; }

        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Liters must be greater than 0.")]
        public decimal Liters { get; set; }

        [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Cost/Liter must be greater than or equal to 0.")]
        public decimal CostPerLiter { get; set; }

        [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Selling Price/Liter must be greater than or equal to 0.")]
        public decimal SellingPricePerLiter { get; set; }

        public string? Remarks { get; set; }

        [Range(0, 2)]
        public int Status { get; set; } = 0;
    }

    public class FuelReceivingRowViewModel
    {
        public int Id { get; set; }
        public string ReceivingNo { get; set; } = "-";
        public string BranchName { get; set; } = "-";
        public string Supplier { get; set; } = "-";
        public string Fuel { get; set; } = "-";
        public string Tank { get; set; } = "-";
        public decimal Liters { get; set; }
        public decimal CostPerLiter { get; set; }
        public decimal SellingPricePerLiter { get; set; }
        public decimal? TankLitersBefore { get; set; }
        public decimal? TankLitersAfter { get; set; }
        public string ReceivedBy { get; set; } = "-";
        public DateTime Date { get; set; }
        public int Status { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }
}

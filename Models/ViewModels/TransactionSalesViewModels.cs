using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace gpos.Models.ViewModels
{
    public class TransactionSalesFilterViewModel
    {
        public string Search { get; set; } = string.Empty;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? BranchId { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<SelectListItem> StatusOptions { get; set; } = new();
    }

    public class ProductSalesPageViewModel : TransactionSalesFilterViewModel
    {
        public List<ProductSaleRowViewModel> Sales { get; set; } = new();
    }

    public class FuelSalesPageViewModel : TransactionSalesFilterViewModel
    {
        public List<FuelSaleRowViewModel> Sales { get; set; } = new();
    }

    public class PosPageViewModel
    {
        public int? CurrentUserId { get; set; }
        public int? CurrentBranchId { get; set; }
        public string CurrentBranchName { get; set; } = string.Empty;
        public bool CanDisplayRefill { get; set; }
        public List<PosCategoryCardViewModel> Categories { get; set; } = new();
        public List<PosProductCardViewModel> Products { get; set; } = new();
        public List<PosFuelOptionViewModel> Fuels { get; set; } = new();
        public PosRebateViewModel? ActiveRebate { get; set; }
        public PosVatViewModel? ActiveVat { get; set; }
    }

    public class PosDisplayRefillRequest
    {
        public int ProductId { get; set; }

        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Quantity must be greater than 0.")]
        public decimal Quantity { get; set; }

        [StringLength(255)]
        public string? Remarks { get; set; }
    }

    public class PosCategoryCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class PosProductCardViewModel
    {
        public int ProductId { get; set; }
        public int? CategoryId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string VisualText { get; set; } = string.Empty;
        public decimal AvailableQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal UnitCost { get; set; }
    }

    public class PosFuelOptionViewModel
    {
        public int FuelId { get; set; }
        public int TankId { get; set; }
        public string TankNo { get; set; } = "-";
        public string Name { get; set; } = string.Empty;
        public decimal AvailableLiters { get; set; }
        public decimal Price { get; set; }
        public bool IsChecked { get; set; }
        public int NozzleId { get; set; }
        public string NozzleName { get; set; } = string.Empty;
        public string NozzleNo { get; set; } = string.Empty;
        public string PumpName { get; set; } = string.Empty;
        public string PumpNo { get; set; } = string.Empty;
        public string DispenserName { get; set; } = string.Empty;
    }

    public class PosRebateViewModel
    {
        public string Name { get; set; } = string.Empty;
        public decimal PointsRequired { get; set; }
        public decimal RebateValue { get; set; }
    }

    public class PosVatViewModel
    {
        public string Name { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public string Type { get; set; } = "Inclusive";
    }

    public class ProductSaleRowViewModel
    {
        public int SaleItemId { get; set; }
        public int SaleId { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public string BranchName { get; set; } = "-";
        public string ProductName { get; set; } = "-";
        public string BatchNo { get; set; } = "-";
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal UnitCost { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public decimal GrossProfit { get; set; }
        public bool HasCostSnapshot { get; set; }
        public string CashierName { get; set; } = "-";
        public string MemberName { get; set; } = "-";
        public DateTime? SaleDate { get; set; }
        public string PaymentType { get; set; } = "-";
        public decimal Cost { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal RebateAmount { get; set; }
        public decimal MemberDiscount { get; set; }
        public decimal VoucherDiscount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal NetTotal { get; set; }
        public decimal NetSales { get; set; }
        public decimal NetProfit { get; set; }
        public decimal Loss { get; set; }
        public decimal CashAmount { get; set; }
        public decimal Change { get; set; }
        public decimal PointsPaid { get; set; }
        public decimal PointConversionUsed { get; set; }
        public decimal PointsMonetaryValue { get; set; }
        public string ItemSummary { get; set; } = "-";
        public string ItemSummaryTitle { get; set; } = "-";
        public string Status { get; set; } = "Completed";
    }

    public class FuelSaleRowViewModel
    {
        public int SaleItemId { get; set; }
        public int SaleId { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public string BranchName { get; set; } = "-";
        public string FuelName { get; set; } = "-";
        public string TankNo { get; set; } = "-";
        public string NozzleNo { get; set; } = "-";
        public decimal Liters { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
        public string CashierName { get; set; } = "-";
        public string MemberName { get; set; } = "-";
        public DateTime? SaleDate { get; set; }
        public string PaymentType { get; set; } = "-";
        public decimal Cost { get; set; }
        public bool HasCostSnapshot { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal RebateAmount { get; set; }
        public decimal MemberDiscount { get; set; }
        public decimal VoucherDiscount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal NetTotal { get; set; }
        public decimal NetSales { get; set; }
        public decimal NetProfit { get; set; }
        public decimal Loss { get; set; }
        public decimal CashAmount { get; set; }
        public decimal Change { get; set; }
        public decimal PointsPaid { get; set; }
        public decimal PointConversionUsed { get; set; }
        public decimal PointsMonetaryValue { get; set; }
        public string ItemSummary { get; set; } = "-";
        public string ItemSummaryTitle { get; set; } = "-";
        public string Status { get; set; } = "Completed";
    }

    public class PosSaleRequest
    {
        public List<PosProductSaleRequestItem> Products { get; set; } = new();
        public List<PosFuelSaleRequestItem> Fuels { get; set; } = new();
        public decimal CashAmount { get; set; }
        public string? MembershipCardNo { get; set; }
        public string? VoucherCode { get; set; }
        public string? PaymentMethod { get; set; }
    }

    public class PosProductSaleRequestItem
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal Quantity { get; set; }
    }

    public class PosFuelSaleRequestItem
    {
        public int FuelId { get; set; }
        public int TankId { get; set; }
        [Range(1, int.MaxValue)] public int? NozzleId { get; set; }

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal Liters { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal Price { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal Subtotal { get; set; }
    }
}

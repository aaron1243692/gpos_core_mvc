using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace gpos.Models.ViewModels
{
    public class TransactionSalesFilterViewModel
    {
        public string Search { get; set; } = string.Empty;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Status { get; set; } = string.Empty;
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

    public class ProductSaleRowViewModel
    {
        public int SaleItemId { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public string ProductName { get; set; } = "-";
        public string BatchNo { get; set; } = "-";
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
        public string CashierName { get; set; } = "-";
        public string MemberName { get; set; } = "-";
        public DateTime? SaleDate { get; set; }
        public string PaymentType { get; set; } = "-";
        public decimal GrossTotal { get; set; }
        public decimal RebateAmount { get; set; }
        public decimal NetTotal { get; set; }
        public string Status { get; set; } = "Completed";
    }

    public class FuelSaleRowViewModel
    {
        public int SaleItemId { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
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
        public decimal GrossTotal { get; set; }
        public decimal RebateAmount { get; set; }
        public decimal NetTotal { get; set; }
        public string Status { get; set; } = "Completed";
    }

    public class PosSaleRequest
    {
        public List<PosProductSaleRequestItem> Products { get; set; } = new();
        public List<PosFuelSaleRequestItem> Fuels { get; set; } = new();
        public decimal CashAmount { get; set; }
        public string? MembershipCardNo { get; set; }
        public string? PaymentMethod { get; set; }
    }

    public class PosProductSaleRequestItem
    {
        [Range(1, int.MaxValue)]
        public int DisplayStockId { get; set; }

        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int BatchId { get; set; }

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal Quantity { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal Price { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal Subtotal { get; set; }
    }

    public class PosFuelSaleRequestItem
    {
        [Range(1, int.MaxValue)]
        public int FuelId { get; set; }

        [Range(1, int.MaxValue)]
        public int TankId { get; set; }

        public int? NozzleId { get; set; }

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal Liters { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal Price { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal Subtotal { get; set; }
    }
}

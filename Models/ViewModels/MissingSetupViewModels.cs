using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class ProductUnitForm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;
        public string? Abbreviation { get; set; }
        public int Status { get; set; } = 1;
    }

    public class ProductBatchForm
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }
        public int? SupplierId { get; set; }
        [Required(ErrorMessage = "Batch No is required.")]
        public string BatchNo { get; set; } = string.Empty;
        [Required(ErrorMessage = "Cost Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Cost Price cannot be negative.")]
        public decimal? CostPrice { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Selling Price cannot be negative.")]
        public decimal? SellingPrice { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int Status { get; set; } = 1;
    }

    public class StockReceivingForm
    {
        public int Id { get; set; }
        public int? SupplierId { get; set; }
        [Required(ErrorMessage = "Received Date is required.")]
        public DateTime? ReceivedDate { get; set; } = DateTime.Today;
        public string? Remarks { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public decimal? Quantity { get; set; }
        [Required(ErrorMessage = "Cost Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Cost Price cannot be negative.")]
        public decimal? CostPrice { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Selling Price cannot be negative.")]
        public decimal? SellingPrice { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int Status { get; set; } = 1;
    }

    public class LowStockSettingForm
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }
        public int? ProductBatchId { get; set; }
        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; } = string.Empty;
        [Range(0, double.MaxValue, ErrorMessage = "Minimum Quantity cannot be negative.")]
        public decimal MinimumQuantity { get; set; }
        public int Status { get; set; } = 1;
    }

    public class NozzleForm
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Pump is required.")]
        public int PumpId { get; set; }
        [Required(ErrorMessage = "Nozzle No is required.")]
        public string NozzleNo { get; set; } = string.Empty;
        public int Status { get; set; } = 1;
    }

    public class FuelDeliveryForm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Delivery No is required.")]
        public string DeliveryNo { get; set; } = string.Empty;
        public int? SupplierId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Fuel is required.")]
        public int FuelId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Tank is required.")]
        public int TankId { get; set; }
        [Required(ErrorMessage = "Delivered Liters is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Delivered Liters must be greater than 0.")]
        public decimal? DeliveredLiters { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Cost Per Liter cannot be negative.")]
        public decimal? CostPerLiter { get; set; }
        [Required(ErrorMessage = "Delivery Date is required.")]
        public DateTime? DeliveryDate { get; set; } = DateTime.Today;
        public string? Remarks { get; set; }
        public int Status { get; set; } = 1;
    }

    public class FuelPriceHistoryForm
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Fuel is required.")]
        public int FuelId { get; set; }
        [Required(ErrorMessage = "New Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "New Price cannot be negative.")]
        public decimal? NewPrice { get; set; }
        [Required(ErrorMessage = "Effective At is required.")]
        public DateTime? EffectiveAt { get; set; } = DateTime.Today;
        public string? Remarks { get; set; }
    }

    public class PumpMeterReadingForm : IValidatableObject
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Nozzle is required.")]
        public int NozzleId { get; set; }
        [Required(ErrorMessage = "Opening Meter is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Opening Meter cannot be negative.")]
        public decimal? OpeningMeter { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Closing Meter cannot be negative.")]
        public decimal? ClosingMeter { get; set; }
        [Required(ErrorMessage = "Reading Date is required.")]
        public DateTime? ReadingDate { get; set; } = DateTime.Today;
        public int Status { get; set; } = 1;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OpeningMeter.HasValue && ClosingMeter.HasValue && ClosingMeter.Value < OpeningMeter.Value)
            {
                yield return new ValidationResult("Closing Meter must be greater than or equal to Opening Meter.", new[] { nameof(ClosingMeter) });
            }
        }
    }

    public class RebateRuleForm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;
        [Range(0.01, double.MaxValue, ErrorMessage = "Points Required must be greater than 0.")]
        public decimal PointsRequired { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Rebate Value must be greater than 0.")]
        public decimal RebateValue { get; set; }
        public int Status { get; set; } = 1;
    }

    public class PointsLedgerForm
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Member is required.")]
        public int MemberId { get; set; }
        [Required(ErrorMessage = "Transaction Type is required.")]
        public string TransactionType { get; set; } = "Earned";
        [Required(ErrorMessage = "Points is required.")]
        public decimal? Points { get; set; }
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public string? Remarks { get; set; }
    }

    public class DiscountRuleForm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Type is required.")]
        public string DiscountType { get; set; } = "Percentage";
        [Range(0, double.MaxValue, ErrorMessage = "Value cannot be negative.")]
        public decimal DiscountValue { get; set; }
        [Required(ErrorMessage = "Applies To is required.")]
        public string AppliesTo { get; set; } = "All";
        public int Status { get; set; } = 1;
    }

    public class PaymentMethodForm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Code is required.")]
        public string Code { get; set; } = string.Empty;
        public int Status { get; set; } = 1;
    }

    public class ShiftSettingForm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool RequireOpeningCash { get; set; } = true;
        public bool AllowCashIn { get; set; } = true;
        public bool AllowCashOut { get; set; } = true;
        public bool RequireClosingApproval { get; set; }
        public int Status { get; set; } = 1;
    }

    public class EmployeeShiftScheduleForm
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Employee is required.")]
        public int EmployeeAccountId { get; set; }
        [Range(1, 7, ErrorMessage = "Day is required.")]
        public int DayOfWeek { get; set; }
        public int? ShiftSettingId { get; set; }
        [Required(ErrorMessage = "Start Time is required.")]
        public TimeSpan? StartTime { get; set; }
        [Required(ErrorMessage = "End Time is required.")]
        public TimeSpan? EndTime { get; set; }
        public int Status { get; set; } = 1;
    }

    public class RolePermissionsForm
    {
        [Range(1, int.MaxValue, ErrorMessage = "Role is required.")]
        public int RoleId { get; set; }
        public List<int> PermissionIds { get; set; } = new();
    }

    public class ProductSearchOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string BatchNo { get; set; } = string.Empty;
        public decimal? CostPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public string SearchText { get; set; } = string.Empty;
        public string DisplayText { get; set; } = string.Empty;
    }

    public class ShiftScheduleShiftOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string StartTimeValue => StartTime?.ToString(@"hh\:mm") ?? string.Empty;
        public string EndTimeValue => EndTime?.ToString(@"hh\:mm") ?? string.Empty;
    }

    public class SetupModulesPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public string ActiveModalId { get; set; } = string.Empty;
        public ProductUnitForm ProductUnitForm { get; set; } = new();
        public ProductBatchForm ProductBatchForm { get; set; } = new();
        public StockReceivingForm StockReceivingForm { get; set; } = new();
        public LowStockSettingForm LowStockSettingForm { get; set; } = new();
        public NozzleForm NozzleForm { get; set; } = new();
        public FuelDeliveryForm FuelDeliveryForm { get; set; } = new();
        public FuelPriceHistoryForm FuelPriceHistoryForm { get; set; } = new();
        public PumpMeterReadingForm PumpMeterReadingForm { get; set; } = new();
        public RebateRuleForm RebateRuleForm { get; set; } = new();
        public PointsLedgerForm PointsLedgerForm { get; set; } = new();
        public DiscountRuleForm DiscountRuleForm { get; set; } = new();
        public PaymentMethodForm PaymentMethodForm { get; set; } = new();
        public ShiftSettingForm ShiftSettingForm { get; set; } = new();
        public EmployeeShiftScheduleForm EmployeeShiftScheduleForm { get; set; } = new();
        public RolePermissionsForm RolePermissionsForm { get; set; } = new();
        public List<SelectListItem> ProductOptions { get; set; } = new();
        public List<ProductSearchOption> ProductSearchOptions { get; set; } = new();
        public List<SelectListItem> ProductBatchOptions { get; set; } = new();
        public List<SelectListItem> SupplierOptions { get; set; } = new();
        public List<SelectListItem> FuelOptions { get; set; } = new();
        public List<SelectListItem> TankOptions { get; set; } = new();
        public List<SelectListItem> PumpOptions { get; set; } = new();
        public List<SelectListItem> NozzleOptions { get; set; } = new();
        public List<SelectListItem> MemberOptions { get; set; } = new();
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<SelectListItem> RoleOptions { get; set; } = new();
        public List<SelectListItem> EmployeeAccountOptions { get; set; } = new();
        public List<SelectListItem> DayOptions { get; set; } = new();
        public List<ShiftScheduleShiftOption> ShiftScheduleShiftOptions { get; set; } = new();
        public List<ProductUnit> ProductUnits { get; set; } = new();
        public List<ProductBatch> ProductBatches { get; set; } = new();
        public List<StockReceiving> StockReceivings { get; set; } = new();
        public List<LowStockSetting> LowStockSettings { get; set; } = new();
        public List<Nozzle> Nozzles { get; set; } = new();
        public List<FuelDelivery> FuelDeliveries { get; set; } = new();
        public List<FuelPriceHistory> FuelPriceHistory { get; set; } = new();
        public List<PumpMeterReading> PumpMeterReadings { get; set; } = new();
        public List<RebateRule> RebateRules { get; set; } = new();
        public List<PointsLedger> PointsLedger { get; set; } = new();
        public List<DiscountRule> DiscountRules { get; set; } = new();
        public List<PaymentMethod> PaymentMethods { get; set; } = new();
        public List<ShiftSetting> ShiftSettings { get; set; } = new();
        public List<EmployeeShiftSchedule> EmployeeShiftSchedules { get; set; } = new();
        public List<ActivityLog> ActivityLogs { get; set; } = new();
        public List<Role> Roles { get; set; } = new();
        public List<Permission> Permissions { get; set; } = new();
        public List<int> AssignedPermissionIds { get; set; } = new();
    }
}

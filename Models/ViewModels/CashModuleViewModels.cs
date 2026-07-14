using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class CashModuleFilter
    {
        public string Search { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public int? ShiftId { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class DailyCashForm
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Branch is required.")]
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "Shift is required.")]
        public int ShiftId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "User is required.")]
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Business Date is required.")]
        [DataType(DataType.Date)]
        public DateTime? BusinessDate { get; set; } = DateTime.Today;
        [Range(0, double.MaxValue, ErrorMessage = "Opening Cash must be 0 or greater.")]
        public decimal OpeningCash { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Actual Cash must be 0 or greater.")]
        public decimal ActualCash { get; set; }
        public string? Remarks { get; set; }
        public int Status { get; set; } = 1;
    }

    public class CashMovementForm
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Daily Cash is required.")]
        public int DailyCashId { get; set; }
        public string DailyCashDisplay { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "Branch is required.")]
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime? BusinessDate { get; set; }
        [Required(ErrorMessage = "Date/Time is required.")]
        public DateTime? TransactionDateTime { get; set; } = DateTime.Now;
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Reason is required.")]
        public string Reason { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }

    public class CashRemittanceForm
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Daily Cash is required.")]
        public int DailyCashId { get; set; }
        public string DailyCashDisplay { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "Branch is required.")]
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime? BusinessDate { get; set; }
        public decimal ExpectedCash { get; set; }
        public decimal ActualCash { get; set; }
        public decimal TotalRemitted { get; set; }
        public decimal RemainingAmount { get; set; }
        [Range(typeof(decimal), "0", "9999999999999999.99", ErrorMessage = "Remitted Amount must be 0 or greater.")]
        public decimal RemittedAmount { get; set; }
        public int ReceivedByUserId { get; set; }
        public string ReceivedByName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Received Date/Time is required.")]
        public DateTime? ReceivedDateTime { get; set; } = DateTime.Now;
        public string? Remarks { get; set; }
    }

    public class DailyCashPageViewModel
    {
        public CashModuleFilter Filter { get; set; } = new();
        public DailyCashForm Form { get; set; } = new();
        public List<DailyCash> Records { get; set; } = new();
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<SelectListItem> ShiftOptions { get; set; } = new();
        public List<SelectListItem> UserOptions { get; set; } = new();
        public string ActiveModalId { get; set; } = string.Empty;
    }

    public class CashMovementPageViewModel
    {
        public CashModuleFilter Filter { get; set; } = new();
        public CashMovementForm Form { get; set; } = new();
        public List<CashIn> CashIns { get; set; } = new();
        public List<CashOut> CashOuts { get; set; } = new();
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<SelectListItem> ShiftOptions { get; set; } = new();
        public List<SelectListItem> UserOptions { get; set; } = new();
        public List<SelectListItem> DailyCashOptions { get; set; } = new();
        public string ActiveModalId { get; set; } = string.Empty;
    }

    public class CashRemittancePageViewModel
    {
        public string CurrentUserName { get; set; } = string.Empty;
        public CashModuleFilter Filter { get; set; } = new();
        public CashRemittanceForm Form { get; set; } = new();
        public List<CashRemittance> Records { get; set; } = new();
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<SelectListItem> ShiftOptions { get; set; } = new();
        public List<SelectListItem> UserOptions { get; set; } = new();
        public List<SelectListItem> DailyCashOptions { get; set; } = new();
        public string ActiveModalId { get; set; } = string.Empty;
    }
}

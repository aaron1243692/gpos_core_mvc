using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class ShiftReportPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public int? ShiftId { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<SelectListItem> ShiftOptions { get; set; } = new();
        public List<SelectListItem> UserOptions { get; set; } = new();
        public List<ShiftReportRowViewModel> Rows { get; set; } = new();
    }

    public class ShiftReportRowViewModel
    {
        public int DailyCashId { get; set; }
        public DateTime BusinessDate { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string ShiftName { get; set; } = string.Empty;
        public string CashierName { get; set; } = string.Empty;
        public DateTime? OpeningTime { get; set; }
        public decimal OpeningCash { get; set; }
        public decimal GrossSales { get; set; }
        public decimal CashSales { get; set; }
        public decimal NonCashSales { get; set; }
        public decimal CashIn { get; set; }
        public decimal CashOut { get; set; }
        public decimal ExpectedCash { get; set; }
        public decimal ActualCash { get; set; }
        public decimal DrawerDifference { get; set; }
        public decimal RemittedAmount { get; set; }
        public decimal RemittanceDifference { get; set; }
        public string SessionStatus { get; set; } = string.Empty;
    }
}

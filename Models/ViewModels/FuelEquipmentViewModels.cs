using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class DispenserForm
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue)] public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        [Required] public string DispenserCode { get; set; } = string.Empty;
        [Required] public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Remarks { get; set; }
        public int Status { get; set; } = 1;
    }

    public class FuelPumpForm
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue)] public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        [Range(1, int.MaxValue)] public int DispenserId { get; set; }
        public string DispenserName { get; set; } = string.Empty;
        [Range(1, int.MaxValue)] public int TankId { get; set; }
        public string TankName { get; set; } = string.Empty;
        public string FuelName { get; set; } = string.Empty;
        [Required] public string PumpNo { get; set; } = string.Empty;
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string NozzleNo { get; set; } = string.Empty;
        public string? NozzleName { get; set; }
        public string? Remarks { get; set; }
        public int Status { get; set; } = 1;
    }

    public class FuelEquipmentPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ActiveModalId { get; set; } = string.Empty;
        public DispenserForm DispenserForm { get; set; } = new();
        public FuelPumpForm PumpForm { get; set; } = new();
        public List<Dispenser> Dispensers { get; set; } = new();
        public List<Pump> Pumps { get; set; } = new();
        public List<SelectListItem> BranchOptions { get; set; } = new();
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class FuelForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Fuel name is required.")]
        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public int? SupplierId { get; set; }

        [Required(ErrorMessage = "Current Price Per Liter is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
        public decimal? CurrentPricePerLiter { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class TankForm : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Branch is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch is required.")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Tank fuel is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Tank fuel is required.")]
        public int FuelId { get; set; }

        [Required(ErrorMessage = "Tank no is required.")]
        public string TankNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity Liters is required.")]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Capacity Liters must be greater than 0.")]
        public decimal? CapacityLiters { get; set; }

        public decimal? CurrentLiters { get; set; }

        public bool IsActive { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

    public class PumpForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Branch is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch is required.")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Dispenser name is required.")]
        public string Name { get; set; } = string.Empty;

        public int Status { get; set; } = 1;
    }

    public class FuelSetupPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string FormBranchName { get; set; } = string.Empty;
        public string PumpFormBranchName { get; set; } = string.Empty;
        public string ActiveModalId { get; set; } = string.Empty;
        public FuelForm FuelForm { get; set; } = new();
        public TankForm TankForm { get; set; } = new();
        public PumpForm PumpForm { get; set; } = new();
        public List<Fuel> Fuels { get; set; } = new();
        public List<Tank> Tanks { get; set; } = new();
        public List<Pump> Pumps { get; set; } = new();
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<SelectListItem> SupplierOptions { get; set; } = new();
        public List<SelectListItem> FuelOptions { get; set; } = new();
        public List<SelectListItem> TankOptions { get; set; } = new();
    }
}

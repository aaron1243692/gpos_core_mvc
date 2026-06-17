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

        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
        public decimal CurrentPricePerLiter { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class TankForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tank fuel is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Tank fuel is required.")]
        public int FuelId { get; set; }

        [Required(ErrorMessage = "Tank no is required.")]
        public string TankNo { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    public class PumpForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pump tank is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Pump tank is required.")]
        public int TankId { get; set; }

        [Required(ErrorMessage = "Pump name is required.")]
        public string Name { get; set; } = string.Empty;
    }

    public class FuelSetupPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public string ActiveModalId { get; set; } = string.Empty;
        public FuelForm FuelForm { get; set; } = new();
        public TankForm TankForm { get; set; } = new();
        public PumpForm PumpForm { get; set; } = new();
        public List<Fuel> Fuels { get; set; } = new();
        public List<Tank> Tanks { get; set; } = new();
        public List<Pump> Pumps { get; set; } = new();
        public List<SelectListItem> SupplierOptions { get; set; } = new();
        public List<SelectListItem> FuelOptions { get; set; } = new();
        public List<SelectListItem> TankOptions { get; set; } = new();
    }
}

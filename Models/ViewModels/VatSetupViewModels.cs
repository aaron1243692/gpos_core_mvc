using gpos.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class VatSetupPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public string ActiveModalId { get; set; } = string.Empty;
        public VatSettingForm VatForm { get; set; } = new();
        public VatSetting? CurrentVatSetting { get; set; }
        public List<VatSetting> VatHistory { get; set; } = new();
        public List<VatSetting> VatSettings { get; set; } = new();
        public List<SelectListItem> TypeOptions { get; set; } = new();
    }

    public class VatSettingForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "VAT name is required.")]
        public string Name { get; set; } = string.Empty;

        [Range(0, 9999999999, ErrorMessage = "Rate must be greater than or equal to 0.")]
        public decimal Rate { get; set; }

        [Required(ErrorMessage = "VAT type is required.")]
        public string Type { get; set; } = "Inclusive";

        public bool IsDefault { get; set; }
    }
}

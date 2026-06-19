using System.ComponentModel.DataAnnotations;

namespace gpos.Models.ViewModels
{
    public class SupplierForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public int Status { get; set; } = 1;
    }

    public class SupplierPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public string ActiveModalId { get; set; } = string.Empty;
        public SupplierForm SupplierForm { get; set; } = new();
        public List<Supplier> Suppliers { get; set; } = new();
    }
}

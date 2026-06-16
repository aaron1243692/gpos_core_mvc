using System.ComponentModel.DataAnnotations;

namespace gpos.Models
{
    public class SalesmanLoginViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}

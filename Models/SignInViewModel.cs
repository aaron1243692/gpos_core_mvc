using System.ComponentModel.DataAnnotations;

namespace gpos.Models
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "Email or username is required")]
        public string Login { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}

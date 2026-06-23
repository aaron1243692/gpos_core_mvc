using System.ComponentModel.DataAnnotations;
using gpos.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class RoleForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Role name is required.")]
        public string Name { get; set; } = string.Empty;
    }

    public class RolesPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public string ActiveModalId { get; set; } = string.Empty;
        public RoleForm RoleForm { get; set; } = new();
        public List<Role> Roles { get; set; } = new();
    }

    public class UserForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }

        public string? FullName { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Branch is required.")]
        public int BranchId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Role is required.")]
        public int RoleId { get; set; }

        public int Status { get; set; } = 1;
    }

    public class ResetUserPasswordForm
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class UsersPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public string ActiveModalId { get; set; } = string.Empty;
        public UserForm UserForm { get; set; } = new();
        public ResetUserPasswordForm ResetPasswordForm { get; set; } = new();
        public List<User> Users { get; set; } = new();
        public List<SelectListItem> RoleOptions { get; set; } = new();
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<SelectListItem> DepartmentOptions { get; set; } = new();
    }
}

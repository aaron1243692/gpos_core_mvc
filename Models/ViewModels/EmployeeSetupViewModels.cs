using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpos.Models.ViewModels
{
    public class BranchForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Branch name is required.")]
        public string Name { get; set; } = string.Empty;

        public string? Address { get; set; }

        [Range(0, 1, ErrorMessage = "Status is required.")]
        public int Status { get; set; } = 1;

    }

    public class DepartmentForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Branch is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch is required.")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Department name is required.")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0, 1, ErrorMessage = "Status is required.")]
        public int Status { get; set; } = 1;

    }

    public class PositionForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Position name is required.")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0, 1, ErrorMessage = "Status is required.")]
        public int Status { get; set; } = 1;
    }

    public class EmployeeForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? Email { get; set; }

        public string? ContactNumber { get; set; }
        public string? Address { get; set; }

        [Required(ErrorMessage = "Branch is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch is required.")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        public int? PositionId { get; set; }

        [Range(0, 1, ErrorMessage = "Status is required.")]
        public int Status { get; set; } = 1;

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        public string? Password { get; set; }
    }

    public class ResetPasswordForm
    {
        public int Id { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        [Required(ErrorMessage = "New Password is required.")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare(nameof(NewPassword), ErrorMessage = "New Password and Confirm Password must match.")]
        public string? ConfirmPassword { get; set; }
    }

    public class EmployeeSetupPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public string ActiveModalId { get; set; } = string.Empty;
        public BranchForm BranchForm { get; set; } = new();
        public DepartmentForm DepartmentForm { get; set; } = new();
        public PositionForm PositionForm { get; set; } = new();
        public EmployeeForm EmployeeForm { get; set; } = new();
        public ResetPasswordForm ResetPasswordForm { get; set; } = new();
        public List<Branch> Branches { get; set; } = new();
        public List<Department> Departments { get; set; } = new();
        public List<Position> Positions { get; set; } = new();
        public List<EmployeeAccount> EmployeeAccounts { get; set; } = new();
        public List<SelectListItem> BranchOptions { get; set; } = new();
        public List<SelectListItem> DepartmentOptions { get; set; } = new();
        public List<SelectListItem> PositionOptions { get; set; } = new();
    }
}

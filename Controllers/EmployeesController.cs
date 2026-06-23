using gpos.Filters;
using gpos.Data;
using gpos.Models;
using gpos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace gpos.Controllers
{
    [NonController]
    [BlockSalesmanSession]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public EmployeesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string? search, int? editId, int? resetPasswordId)
        {
            var activeModalId = editId.HasValue ? "employeeModal" : resetPasswordId.HasValue ? "resetPasswordModal" : string.Empty;
            return View(await BuildEmployeesPageAsync(search, editId: editId, resetPasswordId: resetPasswordId, activeModalId: activeModalId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([Bind(Prefix = "EmployeeForm")] EmployeeForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", await BuildEmployeesPageAsync(search, form, activeModalId: "employeeModal"));
            }

            var department = await _db.Departments.AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == form.DepartmentId && item.Status == 1);

            if (department is null || department.BranchId != form.BranchId)
            {
                ModelState.AddModelError("EmployeeForm.DepartmentId", "Selected department does not belong to the selected branch.");
                return View("Index", await BuildEmployeesPageAsync(search, form, activeModalId: "employeeModal"));
            }

            if (form.PositionId.HasValue && form.PositionId.Value > 0)
            {
                var positionExists = await _db.Positions.AnyAsync(position => position.Id == form.PositionId && position.Status == 1);

                if (!positionExists)
                {
                    ModelState.AddModelError("EmployeeForm.PositionId", "Select an active position.");
                    return View("Index", await BuildEmployeesPageAsync(search, form, activeModalId: "employeeModal"));
                }
            }

            var username = form.Username.Trim();
            var existingAccount = form.Id > 0 ? await _db.EmployeeAccounts.FindAsync(form.Id) : null;
            var accountId = existingAccount?.Id ?? 0;
            var usernameExists = await _db.EmployeeAccounts.AnyAsync(account => account.Id != accountId && account.Username == username);

            if (usernameExists)
            {
                ModelState.AddModelError("EmployeeForm.Username", "Username already exists.");
                return View("Index", await BuildEmployeesPageAsync(search, form, activeModalId: "employeeModal"));
            }

            if (existingAccount is null && string.IsNullOrWhiteSpace(form.Password))
            {
                ModelState.AddModelError("EmployeeForm.Password", "Password is required.");
                return View("Index", await BuildEmployeesPageAsync(search, form, activeModalId: "employeeModal"));
            }

            if (form.Id > 0 && existingAccount is null)
            {
                return NotFound();
            }

            var now = DateTime.UtcNow;
            var account = existingAccount ?? new EmployeeAccount { CreatedAt = now };

            account.Username = username;
            account.FullName = form.FullName.Trim();
            account.Email = CleanOptional(form.Email);
            account.ContactNumber = CleanOptional(form.ContactNumber);
            account.Address = CleanOptional(form.Address);
            account.DepartmentId = form.DepartmentId;
            account.PositionId = form.PositionId.HasValue && form.PositionId.Value > 0 ? form.PositionId.Value : null;
            account.Status = form.Status;
            account.UpdatedAt = now;

            if (existingAccount is null)
            {
                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(form.Password!);
            }

            if (existingAccount is null)
            {
                _db.EmployeeAccounts.Add(account);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword([Bind(Prefix = "ResetPasswordForm")] ResetPasswordForm form, string? search)
        {
            var account = await _db.EmployeeAccounts.FindAsync(form.Id);

            if (account is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await PopulateResetPasswordDisplayNameAsync(form);
                return View("Index", await BuildEmployeesPageAsync(search, resetPasswordForm: form, activeModalId: "resetPasswordModal"));
            }

            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(form.NewPassword!);
            account.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            TempData["EmployeeSetupFeedback"] = "Employee password reset.";

            return RedirectToAction(nameof(Index), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? search)
        {
            var account = await _db.EmployeeAccounts.FindAsync(id);

            if (account is not null)
            {
                account.Status = 0;
                account.UpdatedAt = DateTime.UtcNow;
                TempData["EmployeeSetupFeedback"] = "Employee disabled.";
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { search });
        }

        private async Task<EmployeeSetupPageViewModel> BuildEmployeesPageAsync(
            string? search,
            EmployeeForm? form = null,
            ResetPasswordForm? resetPasswordForm = null,
            int? editId = null,
            int? resetPasswordId = null,
            string activeModalId = "")
        {
            IQueryable<EmployeeAccount> query = _db.EmployeeAccounts.AsNoTracking()
                .Include(account => account.Department)
                .ThenInclude(department => department!.Branch)
                .Include(account => account.Position);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(account => account.Username.Contains(searchText)
                    || account.FullName.Contains(searchText)
                    || (account.Department != null && account.Department.Name.Contains(searchText))
                    || (account.Department != null && account.Department.Branch != null && account.Department.Branch.Name.Contains(searchText))
                    || (account.Position != null && account.Position.Name.Contains(searchText)));
            }

            return new EmployeeSetupPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                EmployeeForm = form ?? await BuildEmployeeFormAsync(editId),
                ResetPasswordForm = resetPasswordForm ?? await BuildResetPasswordFormAsync(resetPasswordId),
                BranchOptions = await BuildBranchOptionsAsync(),
                DepartmentOptions = await BuildDepartmentOptionsAsync(),
                PositionOptions = await BuildPositionOptionsAsync(),
                Departments = await BuildActiveDepartmentsAsync(),
                EmployeeAccounts = await query.OrderBy(account => account.Id).ToListAsync()
            };
        }

        private async Task<EmployeeForm> BuildEmployeeFormAsync(int? editId)
        {
            var account = editId.HasValue
                ? await _db.EmployeeAccounts.AsNoTracking()
                    .Include(item => item.Department)
                    .FirstOrDefaultAsync(item => item.Id == editId.Value)
                : null;

            return account is null ? new EmployeeForm() : new EmployeeForm
            {
                Id = account.Id,
                Username = account.Username,
                FullName = account.FullName,
                Email = account.Email,
                ContactNumber = account.ContactNumber,
                Address = account.Address,
                BranchId = account.Department?.BranchId ?? 0,
                DepartmentId = account.DepartmentId,
                PositionId = account.PositionId ?? 0,
                Status = account.Status
            };
        }

        private async Task<ResetPasswordForm> BuildResetPasswordFormAsync(int? resetPasswordId)
        {
            var account = resetPasswordId.HasValue
                ? await _db.EmployeeAccounts.AsNoTracking()
                    .FirstOrDefaultAsync(item => item.Id == resetPasswordId.Value)
                : null;

            return account is null ? new ResetPasswordForm() : new ResetPasswordForm
            {
                Id = account.Id,
                DisplayName = BuildAccountDisplayName(account)
            };
        }

        private async Task PopulateResetPasswordDisplayNameAsync(ResetPasswordForm form)
        {
            var account = await _db.EmployeeAccounts.AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == form.Id);

            form.DisplayName = account is null ? form.DisplayName : BuildAccountDisplayName(account);
        }

        private static string BuildAccountDisplayName(EmployeeAccount account)
        {
            return string.IsNullOrWhiteSpace(account.FullName)
                ? account.Username
                : $"{account.FullName} ({account.Username})";
        }

        private async Task<List<SelectListItem>> BuildBranchOptionsAsync()
        {
            return await _db.Branches.AsNoTracking()
                .Where(branch => branch.Status == 1)
                .OrderBy(branch => branch.Name)
                .Select(branch => new SelectListItem { Value = branch.Id.ToString(), Text = branch.Name })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> BuildDepartmentOptionsAsync()
        {
            return await _db.Departments.AsNoTracking()
                .Include(department => department.Branch)
                .Where(department => department.Status == 1 && department.Branch != null && department.Branch.Status == 1)
                .OrderBy(department => department.Branch!.Name)
                .ThenBy(department => department.Name)
                .Select(department => new SelectListItem
                {
                    Value = department.Id.ToString(),
                    Text = department.Name,
                    Group = new SelectListGroup { Name = department.Branch!.Name }
                })
                .ToListAsync();
        }

        private async Task<List<Department>> BuildActiveDepartmentsAsync()
        {
            return await _db.Departments.AsNoTracking()
                .Include(department => department.Branch)
                .Where(department => department.Status == 1 && department.Branch != null && department.Branch.Status == 1)
                .OrderBy(department => department.Branch!.Name)
                .ThenBy(department => department.Name)
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> BuildPositionOptionsAsync()
        {
            return await _db.Positions.AsNoTracking()
                .Where(position => position.Status == 1)
                .OrderBy(position => position.Name)
                .Select(position => new SelectListItem { Value = position.Id.ToString(), Text = position.Name })
                .ToListAsync();
        }

        private static string? CleanOptional(string? value)
        {
            var trimmed = (value ?? string.Empty).Trim();
            return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
        }
    }
}

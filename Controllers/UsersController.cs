using gpos.Filters;
using gpos.Data;
using gpos.Models;
using gpos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string? search, int? editId, int? branchId)
        {
            return View(await BuildUsersPageAsync(search, branchId, editId: editId, activeModalId: editId.HasValue ? "userModal" : ""));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([Bind(Prefix = "UserForm")] UserForm form, string? search)
        {
            NormalizeUserForm(form);
            await ValidateUserFormAsync(form);

            if (!ModelState.IsValid)
            {
                return View("Index", await BuildUsersPageAsync(search, form: form, activeModalId: "userModal"));
            }

            var now = DateTime.UtcNow;
            var user = form.Id > 0 ? await _db.Users.FindAsync(form.Id) : new User { CreatedAt = now };

            if (user is null)
            {
                return NotFound();
            }

            user.Username = form.Username;
            user.Email = form.Email;
            user.FullName = form.FullName;
            user.ContactNumber = form.ContactNumber;
            user.Address = form.Address;
            user.BranchId = form.BranchId;
            user.Status = form.Status;
            user.UpdatedAt = now;

            if (form.Id == 0)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(form.Password);
                _db.Users.Add(user);
            }

            await _db.SaveChangesAsync();
            await SaveSingleUserRoleAsync(user.Id, form.RoleId, now);
            TempData["UsersFeedback"] = form.Id == 0 ? "User created." : "User updated.";
            return RedirectToAction(nameof(Index), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword([Bind(Prefix = "ResetPasswordForm")] ResetUserPasswordForm form, string? search)
        {
            if (form.Password != form.ConfirmPassword)
            {
                ModelState.AddModelError("ResetPasswordForm.ConfirmPassword", "Passwords do not match.");
            }

            if (!ModelState.IsValid)
            {
                return View("Index", await BuildUsersPageAsync(search, resetForm: form, activeModalId: "passwordModal"));
            }

            var user = await _db.Users.FirstOrDefaultAsync(item => item.Id == form.UserId && item.Status == 1);

            if (user is null)
            {
                return NotFound();
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(form.Password);
            user.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            TempData["UsersFeedback"] = "Password reset.";
            return RedirectToAction(nameof(Index), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? search)
        {
            var user = await _db.Users.FindAsync(id);

            if (user is null)
            {
                return RedirectToAction(nameof(Index), new { search });
            }

            user.Status = 0;
            user.UpdatedAt = DateTime.UtcNow;
            TempData["UsersFeedback"] = "User disabled.";
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id, string? search)
        {
            var user = await _db.Users.FindAsync(id);
            if (user is not null)
            {
                user.Status = 1;
                user.UpdatedAt = DateTime.UtcNow;
                TempData["UsersFeedback"] = "User activated.";
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { search });
        }


        private async Task<UsersPageViewModel> BuildUsersPageAsync(
            string? search,
            int? branchId = null,
            UserForm? form = null,
            ResetUserPasswordForm? resetForm = null,
            int? editId = null,
            string activeModalId = "")
        {
            IQueryable<User> query = _db.Users
                .AsNoTracking()
                .Include(user => user.Branch)
                .Include(user => user.UserRoles)
                    .ThenInclude(userRole => userRole.Role)
                .Where(user => user.Status == 1 || user.Status == 0);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(user => user.Username.Contains(searchText)
                    || user.Email.Contains(searchText)
                    || (user.FullName != null && user.FullName.Contains(searchText))
                    || (user.Branch != null && user.Branch.Name.Contains(searchText))
                    || user.UserRoles.Any(userRole => userRole.Role != null && userRole.Role.Name.Contains(searchText)));
            }

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(user => user.BranchId == branchId.Value);
            }

            var userForm = form ?? await BuildUserFormAsync(editId);

            return new UsersPageViewModel
            {
                Search = searchText,
                BranchId = branchId,
                BranchName = await BranchNameAsync(branchId),
                ActiveModalId = activeModalId,
                UserForm = userForm,
                ResetPasswordForm = resetForm ?? new ResetUserPasswordForm(),
                Users = await query.OrderBy(user => user.Id).ToListAsync(),
                RoleOptions = await BuildRoleOptionsAsync(),
                BranchOptions = await BuildBranchOptionsAsync()
            };
        }

        private async Task<UserForm> BuildUserFormAsync(int? editId)
        {
            var user = editId.HasValue
                ? await _db.Users
                    .AsNoTracking()
                    .Include(item => item.UserRoles)
                    .FirstOrDefaultAsync(item => item.Id == editId.Value)
                : null;

            return user is null ? new UserForm() : new UserForm
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                ContactNumber = user.ContactNumber,
                Address = user.Address,
                BranchId = user.BranchId ?? 0,
                RoleId = user.UserRoles.FirstOrDefault()?.RoleId ?? 0,
                Status = user.Status
            };
        }

        private async Task<List<SelectListItem>> BuildBranchOptionsAsync()
        {
            return await _db.Branches
                .AsNoTracking()
                .Where(branch => branch.Status == 1)
                .OrderBy(branch => branch.Name)
                .Select(branch => new SelectListItem
                {
                    Value = branch.Id.ToString(),
                    Text = branch.Name
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> BuildRoleOptionsAsync()
        {
            return await _db.Roles
                .AsNoTracking()
                .Where(role => role.Status == 1)
                .OrderBy(role => role.Name)
                .Select(role => new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name
                })
                .ToListAsync();
        }

        private async Task ValidateUserFormAsync(UserForm form)
        {
            if (form.Id == 0)
            {
                if (string.IsNullOrWhiteSpace(form.Password))
                {
                    ModelState.AddModelError("UserForm.Password", "Password is required.");
                }

                if (string.IsNullOrWhiteSpace(form.ConfirmPassword))
                {
                    ModelState.AddModelError("UserForm.ConfirmPassword", "Confirm password is required.");
                }
            }

            if (!string.Equals(form.Password, form.ConfirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("UserForm.ConfirmPassword", "Passwords do not match.");
            }

            if (await _db.Users.AnyAsync(user => user.Id != form.Id && user.Username == form.Username))
            {
                ModelState.AddModelError("UserForm.Username", "Username already exists.");
            }

            if (await _db.Users.AnyAsync(user => user.Id != form.Id && user.Email == form.Email))
            {
                ModelState.AddModelError("UserForm.Email", "Email already exists.");
            }

            if (!await _db.Roles.AnyAsync(role => role.Id == form.RoleId && role.Status == 1))
            {
                ModelState.AddModelError("UserForm.RoleId", "Role is required.");
            }

            if (form.Status is not 0 and not 1)
            {
                ModelState.AddModelError("UserForm.Status", "Status is required.");
            }

            if (!await _db.Branches.AnyAsync(branch => branch.Id == form.BranchId && branch.Status == 1))
            {
                ModelState.AddModelError("UserForm.BranchId", "Branch is required.");
            }
        }

        private async Task<string> BranchNameAsync(int? branchId)
        {
            if (!branchId.HasValue || branchId.Value <= 0)
            {
                return string.Empty;
            }

            return await _db.Branches
                .AsNoTracking()
                .Where(branch => branch.Id == branchId.Value)
                .Select(branch => branch.Name)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        private async Task SaveSingleUserRoleAsync(int userId, int roleId, DateTime now)
        {
            var existingRoles = await _db.UserRoles
                .Where(userRole => userRole.UserId == userId)
                .ToListAsync();

            _db.UserRoles.RemoveRange(existingRoles);
            _db.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                CreatedAt = now,
                UpdatedAt = now
            });

            await _db.SaveChangesAsync();
        }

        private static void NormalizeUserForm(UserForm form)
        {
            form.Username = form.Username.Trim();
            form.Email = form.Email.Trim();
            form.Password = form.Password?.Trim();
            form.ConfirmPassword = form.ConfirmPassword?.Trim();
            form.FullName = string.IsNullOrWhiteSpace(form.FullName) ? null : form.FullName.Trim();
            form.ContactNumber = string.IsNullOrWhiteSpace(form.ContactNumber) ? null : form.ContactNumber.Trim();
            form.Address = string.IsNullOrWhiteSpace(form.Address) ? null : form.Address.Trim();
        }
    }
}

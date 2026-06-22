using gpos.Filters;
using gpos.Data;
using gpos.Models;
using gpos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RolesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string? search, int? editId)
        {
            return View(await BuildRolesPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "roleModal" : ""));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([Bind(Prefix = "RoleForm")] RoleForm form, string? search)
        {
            var name = form.Name.Trim();
            var duplicateExists = await _db.Roles
                .AnyAsync(role => role.Id != form.Id && role.Name == name);

            if (duplicateExists)
            {
                ModelState.AddModelError("RoleForm.Name", "Role name already exists.");
            }

            if (!ModelState.IsValid)
            {
                form.Name = name;
                return View("Index", await BuildRolesPageAsync(search, form, activeModalId: "roleModal"));
            }

            var now = DateTime.UtcNow;
            var role = form.Id > 0 ? await _db.Roles.FindAsync(form.Id) : new Role { CreatedAt = now };

            if (role is null)
            {
                return NotFound();
            }

            role.Name = name;
            role.Code = await BuildUniqueRoleCodeAsync(name, role.Id);
            role.Status = 1;
            role.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.Roles.Add(role);
            }

            await _db.SaveChangesAsync();
            TempData["RolesFeedback"] = form.Id == 0 ? "Role created." : "Role updated.";
            return RedirectToAction(nameof(Index), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? search)
        {
            var role = await _db.Roles.FindAsync(id);

            if (role is null)
            {
                return RedirectToAction(nameof(Index), new { search });
            }

            role.Status = 0;
            role.UpdatedAt = DateTime.UtcNow;
            TempData["RolesFeedback"] = "Role disabled.";
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { search });
        }

        private async Task<RolesPageViewModel> BuildRolesPageAsync(string? search, RoleForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<Role> query = _db.Roles
                .AsNoTracking()
                .Where(role => role.Status == 1);
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(role => role.Name.Contains(searchText));
            }

            return new RolesPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                RoleForm = form ?? await BuildRoleFormAsync(editId),
                Roles = await query.OrderBy(role => role.Id).ToListAsync()
            };
        }

        private async Task<RoleForm> BuildRoleFormAsync(int? editId)
        {
            var role = editId.HasValue
                ? await _db.Roles.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value && item.Status == 1)
                : null;

            return role is null ? new RoleForm() : new RoleForm
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        private async Task<string> BuildUniqueRoleCodeAsync(string name, int roleId)
        {
            var baseCode = SlugifyCode(name);
            var code = baseCode;
            var suffix = 2;

            while (await _db.Roles.AnyAsync(role => role.Id != roleId && role.Code == code))
            {
                code = $"{baseCode}_{suffix}";
                suffix++;
            }

            return code;
        }

        private static string SlugifyCode(string value)
        {
            var builder = new StringBuilder();
            var previousUnderscore = false;

            foreach (var character in value.Trim().ToLowerInvariant())
            {
                if (char.IsLetterOrDigit(character))
                {
                    builder.Append(character);
                    previousUnderscore = false;
                    continue;
                }

                if (!previousUnderscore && builder.Length > 0)
                {
                    builder.Append('_');
                    previousUnderscore = true;
                }
            }

            var code = builder.ToString().Trim('_');
            return string.IsNullOrWhiteSpace(code) ? "role" : code;
        }
    }
}

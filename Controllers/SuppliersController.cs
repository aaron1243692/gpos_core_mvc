using gpos.Filters;
using gpos.Data;
using gpos.Models;
using gpos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gpos.Controllers
{
    [BlockSalesmanSession]
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SuppliersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string? search, int? editId)
        {
            return View(await BuildSuppliersPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "supplierModal" : ""));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([Bind(Prefix = "SupplierForm")] SupplierForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", await BuildSuppliersPageAsync(search, form, activeModalId: "supplierModal"));
            }

            var now = DateTime.UtcNow;
            var supplier = form.Id > 0 ? await _db.Suppliers.FindAsync(form.Id) : new Supplier { CreatedAt = now };

            if (supplier is null)
            {
                return NotFound();
            }

            supplier.Name = form.Name.Trim();
            supplier.Email = CleanOptional(form.Email);
            supplier.ContactPerson = CleanOptional(form.ContactPerson);
            supplier.ContactNumber = CleanOptional(form.ContactNumber);
            supplier.Address = CleanOptional(form.Address);
            supplier.Status = form.Status;
            supplier.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.Suppliers.Add(supplier);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? search)
        {
            var supplier = await _db.Suppliers.FindAsync(id);

            if (supplier is null)
            {
                return RedirectToAction(nameof(Index), new { search });
            }

            supplier.Status = 0;
            supplier.UpdatedAt = DateTime.UtcNow;
            TempData["SupplierFeedback"] = "Supplier disabled.";
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { search });
        }

        private async Task<SupplierPageViewModel> BuildSuppliersPageAsync(string? search, SupplierForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<Supplier> query = _db.Suppliers.AsNoTracking();
            var searchText = (search ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(supplier => supplier.Name.Contains(searchText)
                    || (supplier.Email != null && supplier.Email.Contains(searchText))
                    || (supplier.ContactPerson != null && supplier.ContactPerson.Contains(searchText))
                    || (supplier.ContactNumber != null && supplier.ContactNumber.Contains(searchText))
                    || (supplier.Address != null && supplier.Address.Contains(searchText)));
            }

            return new SupplierPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                SupplierForm = form ?? await BuildSupplierFormAsync(editId),
                Suppliers = await query.OrderBy(supplier => supplier.Id).ToListAsync()
            };
        }

        private async Task<SupplierForm> BuildSupplierFormAsync(int? editId)
        {
            var supplier = editId.HasValue
                ? await _db.Suppliers.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value)
                : null;

            return supplier is null ? new SupplierForm() : new SupplierForm
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Email = supplier.Email,
                ContactPerson = supplier.ContactPerson,
                ContactNumber = supplier.ContactNumber,
                Address = supplier.Address,
                Status = supplier.Status
            };
        }

        private static string? CleanOptional(string? value)
        {
            var trimmed = (value ?? string.Empty).Trim();
            return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
        }
    }
}

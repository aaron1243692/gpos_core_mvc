using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gpos.Controllers
{
    public partial class ConfigurationController
    {
        public async Task<IActionResult> Shift(string? search, string? status, int? editId)
        {
            return View(await BuildShiftPageAsync(search, status, editId: editId, activeModalId: editId.HasValue ? "shiftFormModal" : ""));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveShift([Bind(Prefix = "ShiftForm")] Models.ViewModels.ShiftForm form, string? search, string? status)
        {
            if (!ModelState.IsValid)
            {
                return View("Shift", await BuildShiftPageAsync(search, status, form, activeModalId: "shiftFormModal"));
            }

            var now = DateTime.UtcNow;
            var shift = form.Id > 0 ? await _db.ShiftSettings.FindAsync(form.Id) : new Models.ShiftSetting { CreatedAt = now };

            if (shift is null)
            {
                return NotFound();
            }

            shift.Name = form.Name.Trim();
            shift.StartTime = form.StartTime;
            shift.EndTime = form.EndTime;
            shift.OpeningCashAmount = form.OpeningCashAmount;
            shift.Remarks = CleanOptional(form.Remarks);
            shift.Status = form.Id > 0 ? form.Status : 1;
            shift.UpdatedAt = now;

            if (form.Id == 0)
            {
                _db.ShiftSettings.Add(shift);
            }

            await _db.SaveChangesAsync();
            TempData["ConfigSetupFeedback"] = form.Id > 0 ? "Shift updated." : "Shift created.";

            return RedirectToAction(nameof(Shift), new { search, status });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteShift(int id, string? search, string? status)
        {
            var shift = await _db.ShiftSettings.FindAsync(id);

            if (shift is not null)
            {
                shift.Status = 0;
                shift.UpdatedAt = DateTime.UtcNow;
                TempData["ConfigSetupFeedback"] = "Shift disabled.";
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Shift), new { search, status });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateShift(int id, string? search, string? status)
        {
            var shift = await _db.ShiftSettings.FindAsync(id);

            if (shift is not null)
            {
                shift.Status = 1;
                shift.UpdatedAt = DateTime.UtcNow;
                TempData["ConfigSetupFeedback"] = "Shift activated.";
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Shift), new { search, status });
        }

        private async Task<Models.ViewModels.ShiftPageViewModel> BuildShiftPageAsync(
            string? search,
            string? status,
            Models.ViewModels.ShiftForm? form = null,
            int? editId = null,
            string activeModalId = "")
        {
            var searchText = (search ?? string.Empty).Trim();
            var statusFilter = (status ?? string.Empty).Trim();
            var query = _db.ShiftSettings.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(shift => shift.Name.Contains(searchText)
                    || (shift.Remarks != null && shift.Remarks.Contains(searchText)));
            }

            if (string.Equals(statusFilter, "Active", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(shift => shift.Status == 1);
            }
            else if (string.Equals(statusFilter, "Inactive", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(shift => shift.Status != 1);
            }
            else
            {
                statusFilter = string.Empty;
            }

            return new Models.ViewModels.ShiftPageViewModel
            {
                Search = searchText,
                Status = statusFilter,
                ActiveModalId = activeModalId,
                ShiftForm = form ?? await BuildShiftFormAsync(editId),
                Shifts = await query.OrderBy(shift => shift.Id).ToListAsync()
            };
        }

        private async Task<Models.ViewModels.ShiftForm> BuildShiftFormAsync(int? editId)
        {
            var shift = editId.HasValue
                ? await _db.ShiftSettings.AsNoTracking().FirstOrDefaultAsync(item => item.Id == editId.Value)
                : null;

            return shift is null
                ? new Models.ViewModels.ShiftForm()
                : new Models.ViewModels.ShiftForm
                {
                    Id = shift.Id,
                    Name = shift.Name,
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime,
                    OpeningCashAmount = shift.OpeningCashAmount,
                    Remarks = shift.Remarks,
                    Status = shift.Status
                };
        }
    }
}

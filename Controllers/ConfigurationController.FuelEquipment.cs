using gpos.Models;
using gpos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gpos.Services;

namespace gpos.Controllers
{
    public partial class ConfigurationController
    {
        public async Task<IActionResult> Dispensers(string? search, int? filterBranchId, string? status, int? editId)
            => View(await BuildDispensersPage(search, filterBranchId, status, editId));

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDispenser([Bind(Prefix = "DispenserForm")] DispenserForm form, string? search, int? filterBranchId, string? status)
        {
            var dispenserCode = form.DispenserCode?.Trim() ?? string.Empty;
            var existing = form.Id > 0 ? await _db.Dispensers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == form.Id) : null;
            if (!await _db.Branches.AnyAsync(x => x.Id == form.BranchId && x.Status == 1)) ModelState.AddModelError("DispenserForm.BranchId", "Select an active branch.");
            if (form.Id > 0 && existing is null) return NotFound();
            if (existing is not null && existing.BranchId != form.BranchId && await _db.Pumps.AnyAsync(x => x.DispenserId == form.Id && (x.BranchId != form.BranchId || x.Tank == null || x.Tank.BranchId != form.BranchId)))
                ModelState.AddModelError("DispenserForm.BranchId", "Branch cannot be changed because one or more attached Pumps or Tanks belong to another Branch.");
            if (dispenserCode.Length > 0 && await _db.Dispensers.AnyAsync(x => x.Id != form.Id && x.BranchId == form.BranchId && x.DispenserCode == dispenserCode)) ModelState.AddModelError("DispenserForm.DispenserCode", "Dispenser code already exists in this branch.");
            if (!ModelState.IsValid) { await PopulateDispenserBranchName(form); return View("Dispensers", await BuildDispensersPage(search, filterBranchId, status, null, form, "dispenserModal")); }
            var now = DateTime.UtcNow;
            var item = form.Id > 0 ? await _db.Dispensers.FindAsync(form.Id) : new Dispenser { CreatedAt = now };
            if (item is null) return NotFound();
            item.BranchId = form.BranchId; item.DispenserCode = dispenserCode; item.Name = form.Name.Trim(); item.Location = Clean(form.Location); item.Remarks = Clean(form.Remarks); item.Status = form.Status; item.UpdatedAt = now;
            if (form.Id == 0) _db.Dispensers.Add(item);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Dispensers), new { search, filterBranchId, status });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDispenserStatus(int id, int status, string? search, int? filterBranchId)
        {
            var item = await _db.Dispensers.FindAsync(id); if (item is null) return NotFound();
            item.Status = status == 1 ? 1 : 0; item.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Dispensers), new { search, filterBranchId });
        }

        public async Task<IActionResult> FuelPumps(string? search, int? filterBranchId, string? status, int? editId)
            => View(await BuildFuelPumpsPage(search, filterBranchId, status, editId));

        [HttpGet]
        public async Task<IActionResult> SearchEquipmentDispensers(int branchId, string? search, int take = 20) => Json(await _db.Dispensers.AsNoTracking().Include(x => x.Branch).Where(x => x.BranchId == branchId && x.Status == 1 && (string.IsNullOrEmpty(search) || x.Name.Contains(search) || x.DispenserCode.Contains(search))).OrderBy(x => x.Name).Take(Math.Clamp(take, 1, 50)).Select(x => new { id = x.Id, code = x.DispenserCode, name = x.Name, branch = x.Branch != null ? x.Branch.Name : "-", status = "Active" }).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> SearchEquipmentTanks(int branchId, string? search, int take = 20) => Json(await _db.Tanks.AsNoTracking().Where(x => x.BranchId == branchId && x.Status == 1 && x.IsActive && x.Fuel != null && x.Fuel.Status == 1 && x.Fuel.IsActive && (string.IsNullOrEmpty(search) || x.TankNo.Contains(search) || x.Fuel.Name.Contains(search))).OrderBy(x => x.TankNo).Take(Math.Clamp(take, 1, 50)).Select(x => new { id = x.Id, name = x.TankNo, fuel = x.Fuel!.Name, capacity = x.CapacityLiters, currentLiters = x.CurrentLiters, status = "Active" }).ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFuelPump([Bind(Prefix = "PumpForm")] FuelPumpForm form, string? search, int? filterBranchId, string? status)
        {
            var pumpNo = form.PumpNo?.Trim() ?? string.Empty;
            if (!await _db.Branches.AnyAsync(x => x.Id == form.BranchId && x.Status == 1)) ModelState.AddModelError("PumpForm.BranchId", "Select an active branch.");
            var equipment = await FuelEquipmentValidator.ValidatePumpScopeAsync(_db, form.BranchId, form.DispenserId, form.TankId);
            var dispenser = equipment.Dispenser; var tank = equipment.Tank;
            if (equipment.Error is not null) ModelState.AddModelError(equipment.Error.Contains("dispenser", StringComparison.OrdinalIgnoreCase) ? "PumpForm.DispenserId" : "PumpForm.TankId", equipment.Error);
            if (pumpNo.Length > 0 && await _db.Pumps.AnyAsync(x => x.Id != form.Id && x.DispenserId == form.DispenserId && x.PumpNo == pumpNo)) ModelState.AddModelError("PumpForm.PumpNo", "Pump code already exists for this dispenser.");
            if (!ModelState.IsValid) { await PopulateFuelPumpDisplayNames(form); return View("FuelPumps", await BuildFuelPumpsPage(search, filterBranchId, status, null, form, "pumpModal")); }
            var now = DateTime.UtcNow; var pump = form.Id > 0 ? await _db.Pumps.Include(x => x.Nozzles).FirstOrDefaultAsync(x => x.Id == form.Id) : new Pump { CreatedAt = now };
            if (pump is null) return NotFound();
            pump.DispenserId = dispenser!.Id; pump.BranchId = dispenser.BranchId; pump.TankId = tank!.Id; pump.PumpNo = pumpNo; pump.Name = form.Name.Trim(); pump.Remarks = Clean(form.Remarks); pump.Status = form.Status; pump.UpdatedAt = now;
            if (form.Id == 0) _db.Pumps.Add(pump);
            var nozzle = pump.Nozzles.SingleOrDefault();
            if (pump.Nozzles.Count > 1) { ModelState.AddModelError("", "This pump has multiple legacy nozzles and must be reconciled before editing."); await PopulateFuelPumpDisplayNames(form); return View("FuelPumps", await BuildFuelPumpsPage(search, filterBranchId, status, null, form, "pumpModal")); }
            nozzle ??= new Nozzle { Pump = pump, CreatedAt = now };
            nozzle.NozzleNo = form.NozzleNo.Trim(); nozzle.Name = Clean(form.NozzleName); nozzle.TankId = tank.Id; nozzle.Status = form.Status; nozzle.UpdatedAt = now;
            if (nozzle.Id == 0) _db.Nozzles.Add(nozzle);
            await _db.SaveChangesAsync(); return RedirectToAction(nameof(FuelPumps), new { search, filterBranchId, status });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SetFuelPumpStatus(int id, int status, string? search, int? filterBranchId)
        { var pump = await _db.Pumps.Include(x => x.Nozzles).FirstOrDefaultAsync(x => x.Id == id); if (pump is null) return NotFound(); pump.Status = status == 1 ? 1 : 0; foreach (var n in pump.Nozzles) n.Status = pump.Status; pump.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(); return RedirectToAction(nameof(FuelPumps), new { search, filterBranchId }); }

        private async Task<FuelEquipmentPageViewModel> BuildDispensersPage(string? search, int? branchId, string? status, int? editId, DispenserForm? form = null, string modal = "")
        { var q = _db.Dispensers.AsNoTracking().Include(x => x.Branch).AsQueryable(); if (branchId > 0) q = q.Where(x => x.BranchId == branchId); if (status == "Active") q = q.Where(x => x.Status == 1); if (status == "Inactive") q = q.Where(x => x.Status != 1); if (!string.IsNullOrWhiteSpace(search)) q = q.Where(x => x.Name.Contains(search) || x.DispenserCode.Contains(search)); var edit = editId > 0 ? await _db.Dispensers.AsNoTracking().Include(x => x.Branch).FirstOrDefaultAsync(x => x.Id == editId) : null; return new() { Search = search ?? "", BranchId = branchId, Status = status ?? "", ActiveModalId = modal.Length > 0 ? modal : edit is null ? "" : "dispenserModal", Dispensers = await q.OrderBy(x => x.Name).ToListAsync(), DispenserForm = form ?? (edit is null ? new() : new() { Id=edit.Id,BranchId=edit.BranchId,BranchName=edit.Branch?.Name??"",DispenserCode=edit.DispenserCode,Name=edit.Name,Location=edit.Location,Remarks=edit.Remarks,Status=edit.Status }), BranchOptions = await EquipmentBranches() }; }
        private async Task PopulateDispenserBranchName(DispenserForm form)
            => form.BranchName = await _db.Branches.AsNoTracking().Where(x => x.Id == form.BranchId).Select(x => x.Name).FirstOrDefaultAsync() ?? string.Empty;
        private async Task<FuelEquipmentPageViewModel> BuildFuelPumpsPage(string? search, int? branchId, string? status, int? editId, FuelPumpForm? form = null, string modal = "")
        { var q = _db.Pumps.AsNoTracking().Include(x=>x.Branch).Include(x=>x.Dispenser).Include(x=>x.Tank).ThenInclude(x=>x!.Fuel).Include(x=>x.Nozzles).AsQueryable(); if(branchId>0)q=q.Where(x=>x.BranchId==branchId); if(status=="Active")q=q.Where(x=>x.Status==1);if(status=="Inactive")q=q.Where(x=>x.Status!=1);if(!string.IsNullOrWhiteSpace(search))q=q.Where(x=>x.Name.Contains(search)||x.PumpNo.Contains(search)); var edit=editId>0?await _db.Pumps.AsNoTracking().Include(x=>x.Branch).Include(x=>x.Dispenser).Include(x=>x.Tank).ThenInclude(x=>x!.Fuel).Include(x=>x.Nozzles).FirstOrDefaultAsync(x=>x.Id==editId):null; var nozzle=edit?.Nozzles.SingleOrDefault(); return new(){Search=search??"",BranchId=branchId,Status=status??"",ActiveModalId=modal.Length>0?modal:edit is null?"":"pumpModal",Pumps=await q.OrderBy(x=>x.Name).ToListAsync(),PumpForm=form??(edit is null?new():new(){Id=edit.Id,BranchId=edit.BranchId??0,BranchName=edit.Branch?.Name??"",DispenserId=edit.DispenserId??0,DispenserName=edit.Dispenser?.Name??"",TankId=edit.TankId??0,TankName=edit.Tank?.TankNo??"",FuelName=edit.Tank?.Fuel?.Name??"",PumpNo=edit.PumpNo,Name=edit.Name,Remarks=edit.Remarks,Status=edit.Status,NozzleNo=nozzle?.NozzleNo??"",NozzleName=nozzle?.Name}),BranchOptions=await EquipmentBranches()}; }
        private async Task PopulateFuelPumpDisplayNames(FuelPumpForm form)
        {
            form.BranchName = await _db.Branches.Where(x => x.Id == form.BranchId).Select(x => x.Name).FirstOrDefaultAsync() ?? string.Empty;
            form.DispenserName = await _db.Dispensers.Where(x => x.Id == form.DispenserId).Select(x => x.Name).FirstOrDefaultAsync() ?? string.Empty;
            var tank = await _db.Tanks.Where(x => x.Id == form.TankId).Select(x => new { x.TankNo, FuelName = x.Fuel != null ? x.Fuel.Name : "" }).FirstOrDefaultAsync();
            form.TankName = tank?.TankNo ?? string.Empty; form.FuelName = tank?.FuelName ?? string.Empty;
        }
        private async Task<List<SelectListItem>> EquipmentBranches() { var list = await _db.Branches.AsNoTracking().Where(x=>x.Status==1).OrderBy(x=>x.Name).Select(x=>new SelectListItem(x.Name,x.Id.ToString())).ToListAsync(); list.Insert(0,new SelectListItem("All branches","")); return list; }
        private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

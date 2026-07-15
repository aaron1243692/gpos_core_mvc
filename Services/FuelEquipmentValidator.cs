using gpos.Data;
using gpos.Models;
using Microsoft.EntityFrameworkCore;

namespace gpos.Services
{
    public static class FuelEquipmentValidator
    {
        public static async Task<(Dispenser? Dispenser, Tank? Tank, string? Error)> ValidatePumpScopeAsync(ApplicationDbContext db, int branchId, int dispenserId, int tankId)
        {
            var dispenser = await db.Dispensers.FirstOrDefaultAsync(x => x.Id == dispenserId && x.Status == 1);
            var tank = await db.Tanks.Include(x => x.Fuel).FirstOrDefaultAsync(x => x.Id == tankId && x.Status == 1 && x.IsActive);
            if (dispenser is null) return (null, tank, "Select an active dispenser.");
            if (tank?.Fuel is null || tank.Fuel.Status != 1 || !tank.Fuel.IsActive) return (dispenser, null, "Select an active tank with active fuel.");
            if (dispenser.BranchId != branchId || tank.BranchId != branchId) return (dispenser, tank, "Dispenser and tank must belong to the selected branch.");
            return (dispenser, tank, null);
        }

        public static async Task<Nozzle?> LoadPosNozzleAsync(ApplicationDbContext db, int nozzleId)
            => await db.Nozzles.Include(x => x.Pump).ThenInclude(x => x!.Dispenser)
                .Include(x => x.Pump).ThenInclude(x => x!.Tank).ThenInclude(x => x!.Fuel)
                .FirstOrDefaultAsync(x => x.Id == nozzleId && x.Status == 1);
    }
}

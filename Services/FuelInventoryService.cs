using gpos.Data;
using gpos.Models;
using gpos.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace gpos.Services
{
    public sealed class FuelInventoryService
    {
        public const string ReconciliationType = "Reconciliation";
        public const string PreparePermission = "tranfueladjust.reconcile";
        public const string ApprovePermission = "tranfueladjust.approve";
        private readonly ApplicationDbContext _db;

        public FuelInventoryService(ApplicationDbContext db) => _db = db;

        public async Task<TankFuelInvariantResult> GetInvariantAsync(int tankId, bool tracking = false)
        {
            var tankQuery = tracking ? _db.Tanks.AsQueryable() : _db.Tanks.AsNoTracking();
            var tank = await tankQuery.FirstOrDefaultAsync(x => x.Id == tankId)
                ?? throw new InvalidOperationException("Tank was not found.");
            var batchQuery = tracking ? _db.FuelBatches.AsQueryable() : _db.FuelBatches.AsNoTracking();
            var active = batchQuery.Where(x => x.TankId == tankId && x.Status == 1 && x.IsActive && x.RemainingLiters > 0m);
            var liters = await active.SumAsync(x => (decimal?)x.RemainingLiters) ?? 0m;
            var count = await active.CountAsync();
            return Build(tank.Id, tank.CurrentLiters, liters, count);
        }

        public async Task<TankFuelInvariantResult> GetLockedInvariantAsync(int tankId)
        {
            var tank = await _db.Tanks.FromSqlInterpolated($"SELECT * FROM tanks WHERE id = {tankId} FOR UPDATE").SingleOrDefaultAsync()
                ?? throw new InvalidOperationException("Tank was not found.");
            var active = await _db.FuelBatches
                .FromSqlInterpolated($"SELECT * FROM fuel_batches WHERE tank_id = {tankId} AND status = 1 AND is_active = 1 AND remaining_liters > 0 FOR UPDATE")
                .ToListAsync();
            return Build(tank.Id, tank.CurrentLiters, active.Sum(x => x.RemainingLiters), active.Count);
        }

        public async Task ConsumeSaleFifoAsync(IReadOnlyCollection<FuelSale> fuelSales, int branchId, int userId, DateTime now)
        {
            foreach (var tankGroup in fuelSales.GroupBy(x => x.TankId).OrderBy(x => x.Key))
            {
                var tank = await _db.Tanks
                    .FromSqlInterpolated($"SELECT * FROM tanks WHERE id = {tankGroup.Key} FOR UPDATE")
                    .SingleOrDefaultAsync() ?? throw new InvalidOperationException("Tank was not found during Fuel FIFO allocation.");
                if (tank.BranchId != branchId) throw new InvalidOperationException("Tank does not belong to the current Branch.");

                var batches = await _db.FuelBatches.FromSqlInterpolated($@"SELECT * FROM fuel_batches
                    WHERE tank_id = {tank.Id} AND status = 1 AND is_active = 1 AND remaining_liters > 0
                    ORDER BY received_date, id FOR UPDATE").ToListAsync();
                if (batches.Any(x => x.BranchId != branchId || x.FuelId != tank.FuelId))
                    throw new InvalidOperationException("Fuel Batch ownership does not match its Tank, Branch, and Fuel.");

                var batchTotal = RoundLiters(batches.Sum(x => x.RemainingLiters));
                if (RoundLiters(tank.CurrentLiters) != batchTotal)
                    throw new InvalidOperationException("Tank inventory is not balanced with its Fuel Batch layers. Complete an approved Opening Balance / Reconciliation before selling Fuel.");

                foreach (var fuelSale in tankGroup.OrderBy(x => x.Id))
                {
                    if (fuelSale.FuelId != tank.FuelId || fuelSale.Liters <= 0m)
                        throw new InvalidOperationException("Fuel Sale does not match the locked Tank inventory.");
                    if (RoundLiters(batches.Sum(x => x.RemainingLiters)) < RoundLiters(fuelSale.Liters))
                        throw new InvalidOperationException("Authoritative Fuel Batch liters are not enough for this Sale.");

                    var tankBefore = RoundLiters(tank.CurrentLiters);
                    var remaining = RoundLiters(fuelSale.Liters);
                    var totalCost = 0m;
                    foreach (var batch in batches.Where(x => x.RemainingLiters > 0m))
                    {
                        if (remaining <= 0m) break;
                        var allocated = Math.Min(remaining, RoundLiters(batch.RemainingLiters));
                        var batchBefore = RoundLiters(batch.RemainingLiters);
                        var batchAfter = RoundLiters(batchBefore - allocated);
                        var cost = RoundMoney(allocated * batch.CostPricePerLiter);
                        var revenue = RoundMoney(allocated * fuelSale.PricePerLiter);
                        tank.CurrentLiters = RoundLiters(tank.CurrentLiters - allocated);
                        batch.RemainingLiters = batchAfter;
                        batch.UpdatedAt = now;
                        totalCost += cost;

                        var movement = new FuelStockMovement
                        {
                            TankId = tank.Id, FuelId = tank.FuelId, FuelBatchId = batch.Id, BranchId = branchId,
                            MovementType = "Sale", LitersIn = 0m, LitersOut = allocated,
                            BatchLitersBefore = batchBefore, BatchLitersAfter = batchAfter,
                            TankLitersBefore = RoundLiters(tank.CurrentLiters + allocated), TankLitersAfter = tank.CurrentLiters,
                            UnitCostSnapshot = batch.CostPricePerLiter, ReferenceType = "FuelSale", ReferenceId = fuelSale.Id,
                            Remarks = $"FIFO Fuel Sale allocation for FuelSale {fuelSale.Id}", CreatedByUserId = userId, CreatedAt = now
                        };
                        _db.FuelStockMovements.Add(movement);
                        await _db.SaveChangesAsync();
                        _db.FuelSaleBatchAllocations.Add(new FuelSaleBatchAllocation
                        {
                            FuelSaleId = fuelSale.Id, FuelBatchId = batch.Id, TankId = tank.Id, FuelId = tank.FuelId, BranchId = branchId,
                            LitersAllocated = allocated, BatchLitersBefore = batchBefore, BatchLitersAfter = batchAfter,
                            UnitCostSnapshot = batch.CostPricePerLiter, TotalCostSnapshot = cost,
                            UnitPriceSnapshot = fuelSale.PricePerLiter, RevenueSnapshot = revenue, GrossProfitSnapshot = revenue - cost,
                            FuelStockMovementId = movement.Id, CreatedAt = now
                        });
                        remaining = RoundLiters(remaining - allocated);
                    }
                    if (remaining != 0m) throw new InvalidOperationException("Fuel FIFO allocation did not fully satisfy the Fuel Sale.");
                    fuelSale.TankLitersBefore = tankBefore;
                    fuelSale.TankLitersAfter = RoundLiters(tank.CurrentLiters);
                    fuelSale.TotalCostSnapshot = RoundMoney(totalCost);
                    fuelSale.UnitCostSnapshot = fuelSale.Liters == 0m ? 0m : decimal.Round(totalCost / fuelSale.Liters, 4, MidpointRounding.AwayFromZero);
                    fuelSale.GrossProfitSnapshot = RoundMoney(fuelSale.Subtotal - fuelSale.TotalCostSnapshot);
                }
                tank.UpdatedAt = now;
                var afterTotal = RoundLiters(batches.Sum(x => x.RemainingLiters));
                if (RoundLiters(tank.CurrentLiters) != afterTotal)
                    throw new InvalidOperationException("Fuel FIFO invariant failed after allocation.");
            }
            using (_db.BeginFuelFifoSnapshotCompletion()) await _db.SaveChangesAsync();
        }

        public async Task<FuelBatch> CreatePositiveReconciliationLayerAsync(StockAdjustment adjustment, TankFuelInvariantResult current, DateTime now)
        {
            if (adjustment.AdjustmentType != ReconciliationType || adjustment.Status != "Draft") throw new InvalidOperationException("Only a Draft reconciliation can be applied.");
            if (current.Difference <= 0m) throw new InvalidOperationException(current.Difference == 0m ? "This Tank is already balanced." : "Negative reconciliation is blocked until audited FIFO layer reduction is implemented.");
            if (decimal.Round(current.Difference, 2, MidpointRounding.AwayFromZero) != decimal.Round(adjustment.AdjustmentQuantity, 2, MidpointRounding.AwayFromZero)) throw new InvalidOperationException("The reconciliation balance changed. Refresh the Draft and obtain approval again.");
            if (!adjustment.UnitCost.HasValue || adjustment.UnitCost <= 0m) throw new InvalidOperationException("An approved Cost per Liter greater than zero is required.");

            var unitCost = decimal.Round(adjustment.UnitCost.Value, 2, MidpointRounding.AwayFromZero);
            var totalCost = adjustment.CostInputMode == "TotalCost" && adjustment.TotalCost.HasValue
                ? decimal.Round(adjustment.TotalCost.Value, 2, MidpointRounding.AwayFromZero)
                : decimal.Round(current.Difference * unitCost, 2, MidpointRounding.AwayFromZero);
            var tank = await _db.Tanks.Include(x => x.Fuel).SingleAsync(x => x.Id == adjustment.TankId && x.BranchId == adjustment.BranchId && x.FuelId == adjustment.FuelId);
            var batch = new FuelBatch
            {
                FuelId = tank.FuelId, TankId = tank.Id, BranchId = tank.BranchId, FuelDeliveryId = null,
                BatchNo = await GenerateBatchNoAsync(), CostPricePerLiter = unitCost,
                ReceivedLiters = current.Difference, RemainingLiters = current.Difference,
                ReceivedDate = adjustment.BusinessDate, Remarks = $"Opening Balance / Reconciliation: {adjustment.Reason}. {adjustment.EvidenceReference}".Trim(),
                Status = 1, IsActive = true, CreatedAt = now, UpdatedAt = now
            };
            _db.FuelBatches.Add(batch);
            await _db.SaveChangesAsync();
            adjustment.BeforeQuantity = current.ActiveBatchLiters;
            adjustment.SignedQuantity = current.Difference;
            adjustment.AfterQuantity = current.TankLiters;
            adjustment.UnitCost = unitCost;
            adjustment.TotalCost = totalCost;
            adjustment.CreatedFuelBatchId = batch.Id;
            return batch;
        }

        private async Task<string> GenerateBatchNoAsync()
        {
            var numbers = await _db.FuelBatches.AsNoTracking().Select(x => x.BatchNo).ToListAsync();
            var max = numbers.Select(x => int.TryParse(x, out var n) ? n : 0).DefaultIfEmpty().Max();
            return (max + 1).ToString("D8");
        }

        private static TankFuelInvariantResult Build(int tankId, decimal tankLiters, decimal batchLiters, int count)
        {
            tankLiters = decimal.Round(tankLiters, 2, MidpointRounding.AwayFromZero);
            batchLiters = decimal.Round(batchLiters, 2, MidpointRounding.AwayFromZero);
            var difference = tankLiters - batchLiters;
            var state = difference == 0m ? "Balanced" : difference > 0m ? "Needs Opening Balance" : "Batch Layers Exceed Tank";
            return new TankFuelInvariantResult { TankId = tankId, TankLiters = tankLiters, ActiveBatchLiters = batchLiters, Difference = difference, ActiveBatchCount = count, InventoryState = state, Message = difference == 0m ? "Physical inventory and active cost layers are balanced." : "Physical inventory and active cost layers require reconciliation." };
        }

        private static decimal RoundLiters(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
        private static decimal RoundMoney(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}

using gpos.Models;
using gpos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace gpos.Controllers
{
    public partial class ConfigurationController
    {
        public async Task<IActionResult> Categories(string? search)
        {
            return View("ProductCategories", await BuildProductCategoriesPageAsync(search));
        }

        public async Task<IActionResult> ProductUnits(string? search, int? editId)
        {
            return View(await BuildProductUnitsPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "productUnitModal" : ""));
        }

        public async Task<IActionResult> ProductBatches(string? search, int? editId)
        {
            return View(await BuildProductBatchesPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "productBatchModal" : ""));
        }

        public async Task<IActionResult> StockReceiving(string? search, int? editId)
        {
            return View(await BuildStockReceivingPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "stockReceivingModal" : ""));
        }

        public async Task<IActionResult> LowStockSettings(string? search, int? editId)
        {
            return View(await BuildLowStockSettingsPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "lowStockSettingModal" : ""));
        }

        public async Task<IActionResult> Nozzles(string? search, int? editId)
        {
            return View(await BuildNozzlesPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "nozzleModal" : ""));
        }

        public async Task<IActionResult> FuelDeliveries(string? search, int? editId)
        {
            return View(await BuildFuelDeliveriesPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "fuelDeliveryModal" : ""));
        }

        public async Task<IActionResult> FuelPriceHistory(string? search, int? editId)
        {
            return View(await BuildFuelPriceHistoryPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "fuelPriceHistoryModal" : ""));
        }

        public async Task<IActionResult> PumpMeterReadings(string? search, int? editId)
        {
            return View(await BuildPumpMeterReadingsPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "pumpMeterReadingModal" : ""));
        }

        public async Task<IActionResult> Rebate(string? search)
        {
            return View("~/Views/Configuration/Rebate.cshtml", await BuildRebatePageAsync(search));
        }

        public async Task<IActionResult> PointsLedger(string? search)
        {
            return View(await BuildPointsLedgerPageAsync(search));
        }

        [HttpGet("Configuration/PointsLedger/Members")]
        public async Task<IActionResult> PointsLedgerMembers(string? search)
        {
            var searchText = (search ?? string.Empty).Trim();
            var query = _db.Members
                .AsNoTracking()
                .Where(member => member.Status == 1);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(member => (member.CardNo != null && member.CardNo.Contains(searchText))
                    || member.FullName.Contains(searchText)
                    || (member.ContactNumber != null && member.ContactNumber.Contains(searchText)));
            }

            var members = await query
                .OrderBy(member => member.FullName)
                .Take(50)
                .Select(member => new
                {
                    id = member.Id,
                    cardNo = member.CardNo ?? string.Empty,
                    name = member.FullName,
                    contact = member.ContactNumber ?? string.Empty,
                    points = member.Points
                })
                .ToListAsync();

            return Json(members);
        }

        public async Task<IActionResult> DiscountRules(string? search, int? editId)
        {
            return View(await BuildDiscountRulesPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "discountRuleModal" : ""));
        }

        public async Task<IActionResult> EarningRules(string? search, int? editId)
        {
            return View(await BuildEarningRulesPageAsync(search, editId: editId, activeModalId: editId.HasValue ? "earningRuleModal" : ""));
        }

        [NonAction]
        public async Task<IActionResult> ShiftSettings(string? search, int? editId)
        {
            return RedirectToAction(nameof(Schedules), new { search });
        }

        [NonAction]
        public async Task<IActionResult> ShiftSchedule(string? search, int? editId)
        {
            return RedirectToAction(nameof(EmployeeSchedules), new { search });
        }

        [NonAction]
        public async Task<IActionResult> Schedules(string? search, int? editId, int? detailsId)
        {
            var activeModalId = editId.HasValue ? "scheduleModal" : detailsId.HasValue ? "scheduleDetailsModal" : "";
            return View(await BuildSchedulesPageAsync(search, editId: editId, detailsId: detailsId, activeModalId: activeModalId));
        }

        [NonAction]
        public async Task<IActionResult> EmployeeSchedules(string? search)
        {
            return View(await BuildEmployeeSchedulesPageAsync(search));
        }

        public IActionResult Users() => RedirectToAction("Index", "Users");
        public IActionResult Roles() => RedirectToAction("Index", "Roles");
        public IActionResult Permissions() => RedirectToAction("Index", "Permissions");

        public async Task<IActionResult> RolePermissions(int? roleId)
        {
            return View(await BuildRolePermissionsPageAsync(roleId));
        }

        public async Task<IActionResult> ActivityLogs(string? search)
        {
            return View(await BuildActivityLogsPageAsync(search));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProductUnit([Bind(Prefix = "ProductUnitForm")] ProductUnitForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("ProductUnits", await BuildProductUnitsPageAsync(search, form, activeModalId: "productUnitModal"));
            }

            var now = DateTime.UtcNow;
            var unit = form.Id > 0 ? await _db.ProductUnits.FindAsync(form.Id) : new ProductUnit { CreatedAt = now };
            if (unit is null) return NotFound();

            unit.Name = form.Name.Trim();
            unit.Abbreviation = CleanOptional(form.Abbreviation);
            unit.Status = form.Status;
            unit.UpdatedAt = now;
            if (form.Id == 0) _db.ProductUnits.Add(unit);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ProductUnits), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductUnit(int id, string? search)
        {
            var unit = await _db.ProductUnits.FindAsync(id);
            if (unit is not null)
            {
                unit.Status = 0;
                unit.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ProductUnits), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProductBatch([Bind(Prefix = "ProductBatchForm")] ProductBatchForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("ProductBatches", await BuildProductBatchesPageAsync(search, form, activeModalId: "productBatchModal"));
            }

            var productExists = await _db.Products.AnyAsync(product => product.Id == form.ProductId && product.Status == 1 && product.IsActive);
            if (!productExists)
            {
                ModelState.AddModelError("ProductBatchForm.ProductId", "Select a product.");
                return View("ProductBatches", await BuildProductBatchesPageAsync(search, form, activeModalId: "productBatchModal"));
            }

            var batchNo = form.BatchNo.Trim();
            var duplicate = await _db.ProductBatches.AnyAsync(batch => batch.Id != form.Id && batch.BatchNo == batchNo);
            if (duplicate)
            {
                ModelState.AddModelError("ProductBatchForm.BatchNo", "Batch No already exists.");
                return View("ProductBatches", await BuildProductBatchesPageAsync(search, form, activeModalId: "productBatchModal"));
            }

            var now = DateTime.UtcNow;
            var batch = form.Id > 0 ? await _db.ProductBatches.FindAsync(form.Id) : new ProductBatch { CreatedAt = now };
            if (batch is null) return NotFound();

            batch.ProductId = form.ProductId;
            batch.SupplierId = form.SupplierId;
            batch.BatchNo = batchNo;
            batch.CostPrice = form.CostPrice!.Value;
            batch.SellingPrice = form.SellingPrice ?? 0m;
            batch.ExpiryDate = form.ExpiryDate;
            batch.Status = form.Status;
            batch.IsActive = form.Status == 1;
            batch.UpdatedAt = now;
            if (form.Id == 0) _db.ProductBatches.Add(batch);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ProductBatches), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductBatch(int id, string? search)
        {
            var batch = await _db.ProductBatches.FindAsync(id);
            if (batch is not null)
            {
                batch.Status = 0;
                batch.IsActive = false;
                batch.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ProductBatches), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveStockReceiving([Bind(Prefix = "StockReceivingForm")] StockReceivingForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("StockReceiving", await BuildStockReceivingPageAsync(search, form, activeModalId: "stockReceivingModal"));
            }

            var now = DateTime.UtcNow;
            var receiving = form.Id > 0 ? await _db.StockReceivings.Include(item => item.Items).FirstOrDefaultAsync(item => item.Id == form.Id) : new StockReceiving { CreatedAt = now };
            if (receiving is null) return NotFound();

            var subtotal = form.Quantity!.Value * form.CostPrice!.Value;
            receiving.SupplierId = form.SupplierId;
            receiving.ReceivedDate = form.ReceivedDate!.Value;
            receiving.Remarks = CleanOptional(form.Remarks);
            receiving.TotalAmount = subtotal;
            receiving.Status = form.Status;
            receiving.UpdatedAt = now;

            if (form.Id == 0)
            {
                receiving.ReceivingNo = await GenerateReceivingNoAsync();
                receiving.Items.Add(new StockReceivingItem
                {
                    ProductId = form.ProductId,
                    Quantity = form.Quantity.Value,
                    CostPrice = form.CostPrice.Value,
                    SellingPrice = form.SellingPrice,
                    ExpiryDate = form.ExpiryDate,
                    Subtotal = subtotal,
                    CreatedAt = now,
                    UpdatedAt = now
                });
                _db.StockReceivings.Add(receiving);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(StockReceiving), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStockReceiving(int id, string? search)
        {
            var receiving = await _db.StockReceivings.FindAsync(id);
            if (receiving is not null)
            {
                receiving.Status = 0;
                receiving.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(StockReceiving), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLowStockSetting([Bind(Prefix = "LowStockSettingForm")] LowStockSettingForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("LowStockSettings", await BuildLowStockSettingsPageAsync(search, form, activeModalId: "lowStockSettingModal"));
            }

            var now = DateTime.UtcNow;
            var setting = form.Id > 0 ? await _db.LowStockSettings.FindAsync(form.Id) : new LowStockSetting { CreatedAt = now };
            if (setting is null) return NotFound();
            setting.ProductId = form.ProductId;
            setting.ProductBatchId = form.ProductBatchId;
            setting.Location = form.Location.Trim();
            setting.MinimumQuantity = form.MinimumQuantity;
            setting.Status = form.Status;
            setting.UpdatedAt = now;
            if (form.Id == 0) _db.LowStockSettings.Add(setting);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(LowStockSettings), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLowStockSetting(int id, string? search)
        {
            var setting = await _db.LowStockSettings.FindAsync(id);
            if (setting is not null)
            {
                setting.Status = 0;
                setting.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(LowStockSettings), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveNozzle([Bind(Prefix = "NozzleForm")] NozzleForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("Nozzles", await BuildNozzlesPageAsync(search, form, activeModalId: "nozzleModal"));
            }

            var now = DateTime.UtcNow;
            var nozzle = form.Id > 0 ? await _db.Nozzles.FindAsync(form.Id) : new Nozzle { CreatedAt = now };
            if (nozzle is null) return NotFound();
            nozzle.PumpId = form.PumpId;
            nozzle.NozzleNo = form.NozzleNo.Trim();
            nozzle.Status = form.Status;
            nozzle.UpdatedAt = now;
            if (form.Id == 0) _db.Nozzles.Add(nozzle);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Nozzles), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNozzle(int id, string? search)
        {
            var nozzle = await _db.Nozzles.FindAsync(id);
            if (nozzle is not null)
            {
                nozzle.Status = 0;
                nozzle.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Nozzles), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFuelDelivery([Bind(Prefix = "FuelDeliveryForm")] FuelDeliveryForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("FuelDeliveries", await BuildFuelDeliveriesPageAsync(search, form, activeModalId: "fuelDeliveryModal"));
            }

            var tank = await _db.Tanks.FindAsync(form.TankId);
            if (tank is null || tank.FuelId != form.FuelId || tank.Status != 1 || !tank.IsActive)
            {
                ModelState.AddModelError("FuelDeliveryForm.TankId", "Select a tank for the selected fuel.");
                return View("FuelDeliveries", await BuildFuelDeliveriesPageAsync(search, form, activeModalId: "fuelDeliveryModal"));
            }

            var delivered = form.DeliveredLiters!.Value;
            if (form.Id == 0 && tank.CurrentLiters + delivered > tank.CapacityLiters)
            {
                ModelState.AddModelError("FuelDeliveryForm.DeliveredLiters", "Delivery would exceed tank capacity.");
                return View("FuelDeliveries", await BuildFuelDeliveriesPageAsync(search, form, activeModalId: "fuelDeliveryModal"));
            }

            var now = DateTime.UtcNow;
            var delivery = form.Id > 0 ? await _db.FuelDeliveries.FindAsync(form.Id) : new FuelDelivery { CreatedAt = now };
            if (delivery is null) return NotFound();
            delivery.DeliveryNo = form.DeliveryNo.Trim();
            delivery.SupplierId = form.SupplierId;
            delivery.FuelId = form.FuelId;
            delivery.TankId = form.TankId;
            delivery.DeliveredLiters = delivered;
            delivery.CostPerLiter = form.CostPerLiter;
            delivery.TotalCost = form.CostPerLiter.HasValue ? form.CostPerLiter.Value * delivered : null;
            delivery.DeliveryDate = form.DeliveryDate!.Value;
            delivery.Remarks = CleanOptional(form.Remarks);
            delivery.Status = form.Status;
            delivery.UpdatedAt = now;
            if (form.Id == 0)
            {
                tank.CurrentLiters += delivered;
                tank.UpdatedAt = now;
                _db.FuelDeliveries.Add(delivery);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(FuelDeliveries), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFuelDelivery(int id, string? search)
        {
            var delivery = await _db.FuelDeliveries.FindAsync(id);
            if (delivery is not null)
            {
                delivery.Status = 0;
                delivery.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(FuelDeliveries), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFuelPriceHistory([Bind(Prefix = "FuelPriceHistoryForm")] FuelPriceHistoryForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("FuelPriceHistory", await BuildFuelPriceHistoryPageAsync(search, form, activeModalId: "fuelPriceHistoryModal"));
            }

            var fuel = await _db.Fuels.FindAsync(form.FuelId);
            if (fuel is null || fuel.Status != 1 || !fuel.IsActive)
            {
                ModelState.AddModelError("FuelPriceHistoryForm.FuelId", "Select a fuel.");
                return View("FuelPriceHistory", await BuildFuelPriceHistoryPageAsync(search, form, activeModalId: "fuelPriceHistoryModal"));
            }

            var history = new FuelPriceHistory
            {
                FuelId = form.FuelId,
                OldPrice = fuel.CurrentPricePerLiter,
                NewPrice = form.NewPrice!.Value,
                EffectiveAt = form.EffectiveAt!.Value,
                Remarks = CleanOptional(form.Remarks),
                CreatedAt = DateTime.UtcNow
            };
            fuel.CurrentPricePerLiter = history.NewPrice;
            fuel.UpdatedAt = DateTime.UtcNow;
            _db.FuelPriceHistory.Add(history);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(FuelPriceHistory), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFuelPriceHistory(int id, string? search)
        {
            var history = await _db.FuelPriceHistory.FindAsync(id);
            if (history is not null)
            {
                history.Status = 0;
                history.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(FuelPriceHistory), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePumpMeterReading([Bind(Prefix = "PumpMeterReadingForm")] PumpMeterReadingForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("PumpMeterReadings", await BuildPumpMeterReadingsPageAsync(search, form, activeModalId: "pumpMeterReadingModal"));
            }

            var nozzle = await _db.Nozzles.AsNoTracking()
                .Include(item => item.Pump)
                .ThenInclude(pump => pump!.Tank)
                .ThenInclude(tank => tank!.Fuel)
                .FirstOrDefaultAsync(item => item.Id == form.NozzleId && item.Status == 1);

            if (nozzle is null || nozzle.Pump is null || nozzle.Pump.Status != 1 || nozzle.Pump.Tank is null || nozzle.Pump.Tank.Status != 1 || !nozzle.Pump.Tank.IsActive || nozzle.Pump.Tank.Fuel is null || nozzle.Pump.Tank.Fuel.Status != 1 || !nozzle.Pump.Tank.Fuel.IsActive)
            {
                ModelState.AddModelError("PumpMeterReadingForm.NozzleId", "Select an active nozzle.");
                return View("PumpMeterReadings", await BuildPumpMeterReadingsPageAsync(search, form, activeModalId: "pumpMeterReadingModal"));
            }

            var now = DateTime.UtcNow;
            var reading = form.Id > 0 ? await _db.PumpMeterReadings.FindAsync(form.Id) : new PumpMeterReading { CreatedAt = now };
            if (reading is null) return NotFound();
            reading.PumpId = nozzle.PumpId;
            reading.NozzleId = form.NozzleId;
            reading.OpeningMeter = form.OpeningMeter!.Value;
            reading.ClosingMeter = form.ClosingMeter;
            reading.LitersSold = form.ClosingMeter.HasValue ? form.ClosingMeter.Value - form.OpeningMeter.Value : null;
            reading.ReadingDate = form.ReadingDate!.Value;
            reading.Status = form.Status;
            reading.UpdatedAt = now;
            if (form.Id == 0) _db.PumpMeterReadings.Add(reading);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(PumpMeterReadings), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePumpMeterReading(int id, string? search)
        {
            var reading = await _db.PumpMeterReadings.FindAsync(id);
            if (reading is not null)
            {
                reading.Status = 0;
                reading.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(PumpMeterReadings), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRebateRule([Bind(Prefix = "RebateRuleForm")] RebateRuleForm form, string? search)
        {
            var appliesTo = NormalizeRebateAppliesTo(form.AppliesTo);

            if (appliesTo is null)
            {
                ModelState.AddModelError("RebateRuleForm.AppliesTo", "Applies To is required.");
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Configuration/Rebate.cshtml", await BuildRebatePageAsync(search, form, activeModalId: "rebateRuleModal"));
            }

            var now = DateTime.Now;
            _db.RebateRules.Add(new RebateRule
            {
                Name = form.Name.Trim(),
                AppliesTo = appliesTo!,
                PointsRequired = form.PointsRequired!.Value,
                RebateValue = form.RebateValue!.Value,
                MinimumPurchase = form.MinimumPurchase!.Value,
                Status = 1,
                CreatedAt = now,
                UpdatedAt = now
            });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Rebate), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePointsLedger([Bind(Prefix = "PointsLedgerForm")] PointsLedgerForm form, string? search)
        {
            var normalizedType = NormalizePointsLedgerType(form.Type);

            if (normalizedType is null)
            {
                ModelState.AddModelError("PointsLedgerForm.Type", "Type is required.");
            }

            if (normalizedType == "set")
            {
                if (form.Points < 0)
                {
                    ModelState.AddModelError("PointsLedgerForm.Points", "Points must be greater than or equal to 0 for Set.");
                }
            }
            else if (form.Points <= 0)
            {
                ModelState.AddModelError("PointsLedgerForm.Points", "Points must be greater than 0.");
            }

            if (!ModelState.IsValid)
            {
                return View("PointsLedger", await BuildPointsLedgerPageAsync(search, form, "pointsLedgerModal"));
            }

            var member = await _db.Members.FirstOrDefaultAsync(item => item.Id == form.MemberId && item.Status == 1);

            if (member is null)
            {
                ModelState.AddModelError("PointsLedgerForm.MemberId", "Please select a member.");
                return View("PointsLedger", await BuildPointsLedgerPageAsync(search, form, "pointsLedgerModal"));
            }

            var oldPoints = member.Points;
            var inputPoints = form.Points!.Value;
            var ledgerPoints = CalculatePointsLedgerDelta(normalizedType!, inputPoints);
            var newPoints = CalculatePointsLedgerNewPoints(normalizedType!, oldPoints, inputPoints);

            if (newPoints < 0)
            {
                ModelState.AddModelError("PointsLedgerForm.Points", "Insufficient member points.");
                return View("PointsLedger", await BuildPointsLedgerPageAsync(search, form, "pointsLedgerModal"));
            }

            var ledger = new PointsLedger
            {
                MemberId = form.MemberId,
                TransactionType = normalizedType!,
                Points = ledgerPoints,
                OldPoints = oldPoints,
                NewPoints = newPoints,
                Remarks = CleanOptional(form.Remarks),
                CreatedAt = DateTime.UtcNow
            };
            member.Points = newPoints;
            member.UpdatedAt = DateTime.UtcNow;
            _db.PointsLedger.Add(ledger);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(PointsLedger), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDiscountRule([Bind(Prefix = "DiscountRuleForm")] DiscountRuleForm form, string? search)
        {
            var discountType = NormalizeDiscountRuleType(form.DiscountType);
            var appliesTo = NormalizeDiscountRuleAppliesTo(form.AppliesTo);

            if (discountType is null)
            {
                ModelState.AddModelError("DiscountRuleForm.DiscountType", "Discount Type is required.");
            }

            if (appliesTo is null)
            {
                ModelState.AddModelError("DiscountRuleForm.AppliesTo", "Applies To is required.");
            }

            if (form.DiscountValue.HasValue && discountType == "Percentage" && (form.DiscountValue.Value <= 0 || form.DiscountValue.Value > 100))
            {
                ModelState.AddModelError("DiscountRuleForm.DiscountValue", "Percentage value must be greater than 0 and not greater than 100.");
            }

            if (form.DiscountValue.HasValue && discountType == "Fixed Amount" && form.DiscountValue.Value <= 0)
            {
                ModelState.AddModelError("DiscountRuleForm.DiscountValue", "Fixed Amount value must be greater than 0.");
            }

            if (form.StartDate.HasValue && form.EndDate.HasValue && form.EndDate.Value.Date < form.StartDate.Value.Date)
            {
                ModelState.AddModelError("DiscountRuleForm.EndDate", "End Date must not be earlier than Start Date.");
            }

            var discount = form.DiscountId > 0
                ? await _db.Discounts.AsNoTracking().FirstOrDefaultAsync(item => item.Id == form.DiscountId)
                : null;

            if (discount is null)
            {
                ModelState.AddModelError("DiscountRuleForm.DiscountId", "Discount is required.");
            }

            if (!ModelState.IsValid)
            {
                return View("DiscountRules", await BuildDiscountRulesPageAsync(search, form, activeModalId: "discountRuleModal"));
            }

            var now = DateTime.UtcNow;
            var rule = form.Id > 0 ? await _db.DiscountRules.FindAsync(form.Id) : new DiscountRule { CreatedAt = now };
            if (rule is null) return NotFound();
            rule.Name = discount!.Name;
            rule.DiscountId = form.DiscountId;
            rule.AppliesTo = appliesTo!;
            rule.DiscountType = discountType!;
            rule.DiscountValue = form.DiscountValue!.Value;
            rule.MinimumAmount = form.MinimumAmount!.Value;
            rule.MemberRequired = form.MemberRequired ? 1 : 0;
            rule.StartDate = form.StartDate?.Date;
            rule.EndDate = form.EndDate?.Date;
            rule.Status = form.Status;
            rule.UpdatedAt = now;
            if (form.Id == 0) _db.DiscountRules.Add(rule);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(DiscountRules), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDiscountRule(int id, string? search)
        {
            var rule = await _db.DiscountRules.FindAsync(id);
            if (rule is not null)
            {
                rule.Status = 0;
                rule.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(DiscountRules), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEarningRule([Bind(Prefix = "EarningRuleForm")] EarningRuleForm form, string? search)
        {
            var earnType = NormalizeEarningRuleType(form.EarnType);
            var appliesTo = NormalizeEarningRuleAppliesTo(form.AppliesTo);

            if (earnType is null)
            {
                ModelState.AddModelError("EarningRuleForm.EarnType", "Earn Type is required.");
            }

            if (appliesTo is null)
            {
                ModelState.AddModelError("EarningRuleForm.AppliesTo", "Applies To is required.");
            }

            if (form.EarnValue.HasValue && earnType == "Percentage" && (form.EarnValue.Value <= 0 || form.EarnValue.Value > 100))
            {
                ModelState.AddModelError("EarningRuleForm.EarnValue", "Percentage value must be greater than 0 and not greater than 100.");
            }

            if (form.EarnValue.HasValue && earnType == "Fixed Points" && form.EarnValue.Value <= 0)
            {
                ModelState.AddModelError("EarningRuleForm.EarnValue", "Fixed Points value must be greater than 0.");
            }

            if (form.StartDate.HasValue && form.EndDate.HasValue && form.EndDate.Value.Date < form.StartDate.Value.Date)
            {
                ModelState.AddModelError("EarningRuleForm.EndDate", "End Date must not be earlier than Start Date.");
            }

            var earnings = form.EarningsId > 0
                ? await _db.Earnings.AsNoTracking().FirstOrDefaultAsync(item => item.Id == form.EarningsId)
                : null;

            if (earnings is null)
            {
                ModelState.AddModelError("EarningRuleForm.EarningsId", "Earnings is required.");
            }

            if (!ModelState.IsValid)
            {
                return View("EarningRules", await BuildEarningRulesPageAsync(search, form, activeModalId: "earningRuleModal"));
            }

            var now = DateTime.UtcNow;
            var rule = form.Id > 0 ? await _db.EarningRules.FindAsync(form.Id) : new EarningRule { CreatedAt = now };
            if (rule is null) return NotFound();

            rule.EarningsId = form.EarningsId;
            rule.Name = form.Name.Trim();
            rule.EarnType = earnType!;
            rule.EarnValue = form.EarnValue!.Value;
            rule.AppliesTo = appliesTo!;
            rule.MinimumAmount = form.MinimumAmount!.Value;
            rule.MemberRequired = form.MemberRequired ? 1 : 0;
            rule.StartDate = form.StartDate?.Date;
            rule.EndDate = form.EndDate?.Date;
            rule.Status = form.Status;
            rule.UpdatedAt = now;

            if (form.Id == 0) _db.EarningRules.Add(rule);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(EarningRules), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEarningRule(int id, string? search)
        {
            var rule = await _db.EarningRules.FindAsync(id);
            if (rule is not null)
            {
                rule.Status = 0;
                rule.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(EarningRules), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonAction]
        public async Task<IActionResult> SaveSchedule([Bind(Prefix = "ScheduleForm")] ScheduleForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("Schedules", await BuildSchedulesPageAsync(search, form, activeModalId: "scheduleModal"));
            }

            var name = form.Name.Trim();
            var duplicate = await _db.Schedules.AnyAsync(schedule => schedule.Id != form.Id && schedule.Name == name);
            if (duplicate)
            {
                ModelState.AddModelError("ScheduleForm.Name", "Schedule name already exists.");
                return View("Schedules", await BuildSchedulesPageAsync(search, form, activeModalId: "scheduleModal"));
            }

            var now = DateTime.UtcNow;
            var scheduleItem = form.Id > 0 ? await _db.Schedules.FindAsync(form.Id) : new Schedule { CreatedAt = now };
            if (scheduleItem is null) return NotFound();

            scheduleItem.Name = name;
            scheduleItem.Status = form.Status;
            scheduleItem.UpdatedAt = now;
            if (form.Id == 0) _db.Schedules.Add(scheduleItem);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Schedules), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonAction]
        public async Task<IActionResult> SaveScheduleDetails([Bind(Prefix = "ScheduleDetailsForm")] ScheduleDetailsForm form, string? search)
        {
            var scheduleExists = await _db.Schedules.AnyAsync(schedule => schedule.Id == form.ScheduleId && schedule.Status == 1);
            if (!scheduleExists) return NotFound();

            var now = DateTime.UtcNow;
            var existing = await _db.ScheduleDetails.Where(detail => detail.ScheduleId == form.ScheduleId).ToListAsync();
            for (var i = 0; i < form.Details.Count; i++)
            {
                var line = form.Details[i];
                var detail = existing.FirstOrDefault(item => item.DayOfWeek == line.DayOfWeek);
                if (detail is null)
                {
                    detail = new ScheduleDetail { ScheduleId = form.ScheduleId, DayOfWeek = line.DayOfWeek, CreatedAt = now };
                    _db.ScheduleDetails.Add(detail);
                }

                detail.AmIn = line.AmIn;
                detail.AmOut = line.AmOut;
                detail.PmIn = line.PmIn;
                detail.PmOut = line.PmOut;
                detail.UpdatedAt = now;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Schedules), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonAction]
        public async Task<IActionResult> DeleteSchedule(int id, string? search)
        {
            var schedule = await _db.Schedules.FindAsync(id);
            if (schedule is not null)
            {
                schedule.Status = 0;
                schedule.UpdatedAt = DateTime.UtcNow;
                var assignedAccounts = await _db.EmployeeAccounts.Where(account => account.ScheduleId == id).ToListAsync();
                for (var i = 0; i < assignedAccounts.Count; i++)
                {
                    assignedAccounts[i].ScheduleId = null;
                    assignedAccounts[i].UpdatedAt = DateTime.UtcNow;
                }
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Schedules), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonAction]
        public async Task<IActionResult> AssignEmployeeSchedule([Bind(Prefix = "EmployeeScheduleForm")] EmployeeScheduleForm form, string? search)
        {
            var account = await _db.EmployeeAccounts.FindAsync(form.EmployeeAccountId);
            if (account is null) return NotFound();

            if (form.ScheduleId.HasValue)
            {
                var scheduleExists = await _db.Schedules.AnyAsync(schedule => schedule.Id == form.ScheduleId.Value && schedule.Status == 1);
                if (!scheduleExists) form.ScheduleId = null;
            }

            account.ScheduleId = form.ScheduleId;
            account.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(EmployeeSchedules), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonAction]
        public async Task<IActionResult> SaveShiftSetting([Bind(Prefix = "ShiftSettingForm")] ShiftSettingForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("ShiftSettings", await BuildShiftSettingsPageAsync(search, form, activeModalId: "shiftSettingModal"));
            }

            var now = DateTime.UtcNow;
            var setting = form.Id > 0 ? await _db.ShiftSettings.FindAsync(form.Id) : new ShiftSetting { CreatedAt = now };
            if (setting is null) return NotFound();
            setting.Name = form.Name.Trim();
            setting.StartTime = form.StartTime;
            setting.EndTime = form.EndTime;
            setting.RequireOpeningCash = form.RequireOpeningCash ? 1 : 0;
            setting.AllowCashIn = form.AllowCashIn ? 1 : 0;
            setting.AllowCashOut = form.AllowCashOut ? 1 : 0;
            setting.RequireClosingApproval = form.RequireClosingApproval ? 1 : 0;
            setting.Status = form.Status;
            setting.UpdatedAt = now;
            if (form.Id == 0) _db.ShiftSettings.Add(setting);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ShiftSettings), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonAction]
        public async Task<IActionResult> DeleteShiftSetting(int id, string? search)
        {
            var setting = await _db.ShiftSettings.FindAsync(id);
            if (setting is not null)
            {
                setting.Status = 0;
                setting.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ShiftSettings), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonAction]
        public async Task<IActionResult> SaveShiftSchedule([Bind(Prefix = "EmployeeShiftScheduleForm")] EmployeeShiftScheduleForm form, string? search)
        {
            if (!ModelState.IsValid)
            {
                return View("ShiftSchedule", await BuildShiftSchedulePageAsync(search, form, activeModalId: "shiftScheduleModal"));
            }

            if (!form.StartTime.HasValue || !form.EndTime.HasValue)
            {
                return View("ShiftSchedule", await BuildShiftSchedulePageAsync(search, form, activeModalId: "shiftScheduleModal"));
            }

            var startTime = form.StartTime.Value;
            var endTime = form.EndTime.Value;

            var employeeExists = await _db.EmployeeAccounts.AnyAsync(account => account.Id == form.EmployeeAccountId && account.Status == 1);
            if (!employeeExists)
            {
                ModelState.AddModelError("EmployeeShiftScheduleForm.EmployeeAccountId", "Select an active employee.");
                return View("ShiftSchedule", await BuildShiftSchedulePageAsync(search, form, activeModalId: "shiftScheduleModal"));
            }

            if (form.ShiftSettingId.HasValue)
            {
                var shiftExists = await _db.ShiftSettings.AnyAsync(setting => setting.Id == form.ShiftSettingId.Value && setting.Status == 1);
                if (!shiftExists)
                {
                    ModelState.AddModelError("EmployeeShiftScheduleForm.ShiftSettingId", "Select an active shift.");
                    return View("ShiftSchedule", await BuildShiftSchedulePageAsync(search, form, activeModalId: "shiftScheduleModal"));
                }
            }

            var duplicate = await _db.EmployeeShiftSchedules.AnyAsync(schedule => schedule.Id != form.Id
                && schedule.Status == 1
                && schedule.EmployeeAccountId == form.EmployeeAccountId
                && schedule.DayOfWeek == form.DayOfWeek
                && schedule.StartTime == startTime
                && schedule.EndTime == endTime);

            if (duplicate)
            {
                ModelState.AddModelError("EmployeeShiftScheduleForm.StartTime", "This active schedule already exists.");
                return View("ShiftSchedule", await BuildShiftSchedulePageAsync(search, form, activeModalId: "shiftScheduleModal"));
            }

            var activeSchedules = await _db.EmployeeShiftSchedules.AsNoTracking()
                .Where(schedule => schedule.Id != form.Id
                    && schedule.Status == 1
                    && schedule.EmployeeAccountId == form.EmployeeAccountId
                    && schedule.DayOfWeek == form.DayOfWeek)
                .ToListAsync();

            if (activeSchedules.Any(schedule => TimeRangesOverlap(schedule.StartTime, schedule.EndTime, startTime, endTime)))
            {
                ModelState.AddModelError("EmployeeShiftScheduleForm.StartTime", "This schedule overlaps another active schedule for the same employee and day.");
                return View("ShiftSchedule", await BuildShiftSchedulePageAsync(search, form, activeModalId: "shiftScheduleModal"));
            }

            var now = DateTime.UtcNow;
            var scheduleItem = form.Id > 0 ? await _db.EmployeeShiftSchedules.FindAsync(form.Id) : new EmployeeShiftSchedule { CreatedAt = now };
            if (scheduleItem is null) return NotFound();

            scheduleItem.EmployeeAccountId = form.EmployeeAccountId;
            scheduleItem.DayOfWeek = form.DayOfWeek;
            scheduleItem.ShiftSettingId = form.ShiftSettingId;
            scheduleItem.StartTime = startTime;
            scheduleItem.EndTime = endTime;
            scheduleItem.Status = form.Status;
            scheduleItem.UpdatedAt = now;
            if (form.Id == 0) _db.EmployeeShiftSchedules.Add(scheduleItem);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ShiftSchedule), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonAction]
        public async Task<IActionResult> DeleteShiftSchedule(int id, string? search)
        {
            var schedule = await _db.EmployeeShiftSchedules.FindAsync(id);
            if (schedule is not null)
            {
                schedule.Status = 0;
                schedule.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ShiftSchedule), new { search });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRolePermissions([Bind(Prefix = "RolePermissionsForm")] RolePermissionsForm form)
        {
            if (!ModelState.IsValid)
            {
                return View("RolePermissions", await BuildRolePermissionsPageAsync(form.RoleId, form));
            }

            var now = DateTime.UtcNow;
            var existing = await _db.RolePermissions.Where(item => item.RoleId == form.RoleId).ToListAsync();
            for (var i = 0; i < existing.Count; i++)
            {
                existing[i].Status = 0;
                existing[i].UpdatedAt = now;
            }

            for (var i = 0; i < form.PermissionIds.Count; i++)
            {
                var permissionId = form.PermissionIds[i];
                var assignment = existing.FirstOrDefault(item => item.PermissionId == permissionId);

                if (assignment is not null)
                {
                    assignment.Status = 1;
                    assignment.UpdatedAt = now;
                    continue;
                }

                _db.RolePermissions.Add(new RolePermission
                {
                    RoleId = form.RoleId,
                    PermissionId = permissionId,
                    Status = 1,
                    CreatedAt = now,
                    UpdatedAt = now
                });
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(RolePermissions), new { roleId = form.RoleId });
        }

        private async Task<SetupModulesPageViewModel> BuildProductUnitsPageAsync(string? search, ProductUnitForm? form = null, int? editId = null, string activeModalId = "")
        {
            var query = _db.ProductUnits.AsNoTracking();
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText)) query = query.Where(unit => unit.Name.Contains(searchText) || (unit.Abbreviation != null && unit.Abbreviation.Contains(searchText)));
            return new SetupModulesPageViewModel { Search = searchText, ActiveModalId = activeModalId, ProductUnitForm = form ?? await BuildProductUnitFormAsync(editId), ProductUnits = await query.OrderBy(unit => unit.Id).ToListAsync() };
        }

        private async Task<SetupModulesPageViewModel> BuildProductBatchesPageAsync(string? search, ProductBatchForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<ProductBatch> query = _db.ProductBatches.AsNoTracking().Include(batch => batch.Product).Include(batch => batch.Supplier);
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText)) query = query.Where(batch => batch.BatchNo.Contains(searchText) || (batch.Product != null && batch.Product.Name.Contains(searchText)) || (batch.Supplier != null && batch.Supplier.Name.Contains(searchText)));
            return new SetupModulesPageViewModel { Search = searchText, ActiveModalId = activeModalId, ProductBatchForm = form ?? await BuildProductBatchFormAsync(editId), ProductOptions = await BuildProductOptionsAsync(), SupplierOptions = await BuildSupplierOptionsAsync(), ProductBatches = await query.OrderBy(batch => batch.Id).ToListAsync() };
        }

        private async Task<SetupModulesPageViewModel> BuildStockReceivingPageAsync(string? search, StockReceivingForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<StockReceiving> query = _db.StockReceivings.AsNoTracking().Include(receiving => receiving.Supplier);
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText)) query = query.Where(receiving => receiving.ReceivingNo.Contains(searchText) || (receiving.Supplier != null && receiving.Supplier.Name.Contains(searchText)));
            return new SetupModulesPageViewModel { Search = searchText, ActiveModalId = activeModalId, StockReceivingForm = form ?? await BuildStockReceivingFormAsync(editId), SupplierOptions = await BuildSupplierOptionsAsync(), ProductOptions = await BuildProductOptionsAsync(), ProductSearchOptions = await BuildProductSearchOptionsAsync(), StockReceivings = await query.OrderBy(item => item.Id).ToListAsync() };
        }

        private async Task<SetupModulesPageViewModel> BuildLowStockSettingsPageAsync(string? search, LowStockSettingForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<LowStockSetting> query = _db.LowStockSettings.AsNoTracking().Include(setting => setting.Product).Include(setting => setting.ProductBatch);
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText)) query = query.Where(setting => setting.Location.Contains(searchText) || (setting.Product != null && setting.Product.Name.Contains(searchText)) || (setting.ProductBatch != null && setting.ProductBatch.BatchNo.Contains(searchText)));
            return new SetupModulesPageViewModel { Search = searchText, ActiveModalId = activeModalId, LowStockSettingForm = form ?? await BuildLowStockSettingFormAsync(editId), ProductOptions = await BuildProductOptionsAsync(), ProductBatchOptions = await BuildProductBatchOptionsAsync(), LowStockSettings = await query.OrderBy(item => item.Id).ToListAsync() };
        }

        private async Task<SetupModulesPageViewModel> BuildNozzlesPageAsync(string? search, NozzleForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<Nozzle> query = _db.Nozzles.AsNoTracking().Include(nozzle => nozzle.Pump).ThenInclude(pump => pump!.Tank).ThenInclude(tank => tank!.Fuel);
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText)) query = query.Where(nozzle => nozzle.NozzleNo.Contains(searchText) || (nozzle.Pump != null && nozzle.Pump.Name.Contains(searchText)));
            return new SetupModulesPageViewModel { Search = searchText, ActiveModalId = activeModalId, NozzleForm = form ?? await BuildNozzleFormAsync(editId), PumpOptions = await BuildPumpOptionsAsync(), Nozzles = await query.OrderBy(item => item.Id).ToListAsync() };
        }

        private async Task<SetupModulesPageViewModel> BuildFuelDeliveriesPageAsync(string? search, FuelDeliveryForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<FuelDelivery> query = _db.FuelDeliveries.AsNoTracking().Include(delivery => delivery.Supplier).Include(delivery => delivery.Fuel).Include(delivery => delivery.Tank);
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText)) query = query.Where(delivery => delivery.DeliveryNo.Contains(searchText) || (delivery.Supplier != null && delivery.Supplier.Name.Contains(searchText)) || (delivery.Fuel != null && delivery.Fuel.Name.Contains(searchText)) || (delivery.Tank != null && delivery.Tank.TankNo.Contains(searchText)));
            return new SetupModulesPageViewModel { Search = searchText, ActiveModalId = activeModalId, FuelDeliveryForm = form ?? await BuildFuelDeliveryFormAsync(editId), SupplierOptions = await BuildSupplierOptionsAsync(), FuelOptions = await BuildFuelOptionsAsync(), TankOptions = await BuildTankOptionsAsync(), FuelDeliveries = await query.OrderBy(item => item.Id).ToListAsync() };
        }

        private async Task<SetupModulesPageViewModel> BuildFuelPriceHistoryPageAsync(string? search, FuelPriceHistoryForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<FuelPriceHistory> query = _db.FuelPriceHistory.AsNoTracking().Include(history => history.Fuel);
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText)) query = query.Where(history => history.Fuel != null && history.Fuel.Name.Contains(searchText));
            return new SetupModulesPageViewModel { Search = searchText, ActiveModalId = activeModalId, FuelPriceHistoryForm = form ?? new FuelPriceHistoryForm(), FuelOptions = await BuildFuelOptionsAsync(), FuelPriceHistory = await query.OrderByDescending(item => item.EffectiveAt).ToListAsync() };
        }

        private async Task<SetupModulesPageViewModel> BuildPumpMeterReadingsPageAsync(string? search, PumpMeterReadingForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<PumpMeterReading> query = _db.PumpMeterReadings.AsNoTracking()
                .Include(reading => reading.Nozzle)
                .ThenInclude(nozzle => nozzle!.Pump)
                .ThenInclude(pump => pump!.Tank)
                .ThenInclude(tank => tank!.Fuel)
                .Include(reading => reading.Pump)
                .ThenInclude(pump => pump!.Tank)
                .ThenInclude(tank => tank!.Fuel);
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText)) query = query.Where(reading => (reading.Nozzle != null && reading.Nozzle.NozzleNo.Contains(searchText)) || (reading.Nozzle != null && reading.Nozzle.Pump != null && reading.Nozzle.Pump.Name.Contains(searchText)) || (reading.Nozzle != null && reading.Nozzle.Pump != null && reading.Nozzle.Pump.Tank != null && reading.Nozzle.Pump.Tank.TankNo.Contains(searchText)) || (reading.Nozzle != null && reading.Nozzle.Pump != null && reading.Nozzle.Pump.Tank != null && reading.Nozzle.Pump.Tank.Fuel != null && reading.Nozzle.Pump.Tank.Fuel.Name.Contains(searchText)));
            return new SetupModulesPageViewModel { Search = searchText, ActiveModalId = activeModalId, PumpMeterReadingForm = form ?? await BuildPumpMeterReadingFormAsync(editId), Nozzles = await BuildActiveNozzlesAsync(), PumpMeterReadings = await query.OrderByDescending(item => item.ReadingDate).ToListAsync() };
        }

        private async Task<SetupModulesPageViewModel> BuildRebatePageAsync(string? search, RebateRuleForm? form = null, string activeModalId = "")
        {
            var query = _db.RebateRules.AsNoTracking();
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(rule => rule.Name.Contains(searchText)
                    || rule.AppliesTo.Contains(searchText)
                    || (searchText == "Active" && rule.Status == 1)
                    || (searchText == "Disabled" && rule.Status == 0));
            }

            return new SetupModulesPageViewModel { Search = searchText, ActiveModalId = activeModalId, RebateRuleForm = form ?? new RebateRuleForm(), RebateRules = await query.OrderByDescending(rule => rule.CreatedAt).ThenByDescending(rule => rule.Id).ToListAsync() };
        }

        private static string? NormalizeRebateAppliesTo(string? appliesTo)
        {
            return appliesTo?.Trim() switch
            {
                "Fuel" => "Fuel",
                "Product" => "Product",
                "Both" => "Both",
                _ => null
            };
        }

        private async Task<SetupModulesPageViewModel> BuildPointsLedgerPageAsync(string? search, PointsLedgerForm? form = null, string activeModalId = "")
        {
            IQueryable<PointsLedger> query = _db.PointsLedger.AsNoTracking().Include(ledger => ledger.Member);
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText)) query = query.Where(ledger => ledger.TransactionType.Contains(searchText) || (ledger.Member != null && ledger.Member.FullName.Contains(searchText)) || (ledger.Remarks != null && ledger.Remarks.Contains(searchText)));
            var ledgerForm = form ?? new PointsLedgerForm();

            if (ledgerForm.MemberId > 0 && string.IsNullOrWhiteSpace(ledgerForm.MemberName))
            {
                ledgerForm.MemberName = await _db.Members
                    .AsNoTracking()
                    .Where(member => member.Id == ledgerForm.MemberId)
                    .Select(member => member.FullName)
                    .FirstOrDefaultAsync();
            }

            return new SetupModulesPageViewModel { Search = searchText, ActiveModalId = activeModalId, PointsLedgerForm = ledgerForm, MemberOptions = await BuildMemberOptionsAsync(), PointsLedger = await query.OrderByDescending(ledger => ledger.CreatedAt).ToListAsync() };
        }

        private static string? NormalizePointsLedgerType(string? type)
        {
            return type?.Trim().ToLowerInvariant() switch
            {
                "earned" => "earned",
                "used" => "used",
                "deduct" => "deduct",
                "set" => "set",
                _ => null
            };
        }

        private static decimal CalculatePointsLedgerDelta(string type, decimal points)
        {
            return type switch
            {
                "earned" => Math.Abs(points),
                "used" => -Math.Abs(points),
                "deduct" => -Math.Abs(points),
                "set" => points,
                _ => points
            };
        }

        private static decimal CalculatePointsLedgerNewPoints(string type, decimal oldPoints, decimal points)
        {
            return type switch
            {
                "earned" => oldPoints + Math.Abs(points),
                "used" => oldPoints - Math.Abs(points),
                "deduct" => oldPoints - Math.Abs(points),
                "set" => points,
                _ => oldPoints + points
            };
        }

        private async Task<SetupModulesPageViewModel> BuildDiscountRulesPageAsync(string? search, DiscountRuleForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<DiscountRule> query = _db.DiscountRules.AsNoTracking().Include(rule => rule.Discount);
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(rule => (rule.Discount != null && rule.Discount.Name.Contains(searchText))
                    || rule.AppliesTo.Contains(searchText)
                    || rule.DiscountType.Contains(searchText)
                    || (searchText == "Active" && rule.Status == 1)
                    || (searchText == "Disabled" && rule.Status == 0));
            }

            return new SetupModulesPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                DiscountRuleForm = form ?? await BuildDiscountRuleFormAsync(editId),
                DiscountOptions = await BuildRequiredDiscountOptionsAsync(),
                DiscountRules = await query.OrderBy(rule => rule.Id).ToListAsync()
            };
        }

        private async Task<SetupModulesPageViewModel> BuildEarningRulesPageAsync(string? search, EarningRuleForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<EarningRule> query = _db.EarningRules.AsNoTracking().Include(rule => rule.Earnings);
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(rule => (rule.Earnings != null && rule.Earnings.Name.Contains(searchText))
                    || rule.Name.Contains(searchText)
                    || rule.AppliesTo.Contains(searchText)
                    || rule.EarnType.Contains(searchText)
                    || (searchText == "Active" && rule.Status == 1)
                    || (searchText == "Disabled" && rule.Status == 0));
            }

            return new SetupModulesPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                EarningRuleForm = form ?? await BuildEarningRuleFormAsync(editId),
                EarningsOptions = await BuildRequiredEarningsOptionsAsync(),
                EarningRules = await query.OrderBy(rule => rule.Id).ToListAsync()
            };
        }

        private static string? NormalizeDiscountRuleAppliesTo(string? appliesTo)
        {
            return appliesTo?.Trim() switch
            {
                "Fuel" => "Fuel",
                "Product" => "Product",
                "Both" => "Both",
                _ => null
            };
        }

        private static string? NormalizeDiscountRuleType(string? discountType)
        {
            return discountType?.Trim() switch
            {
                "Percentage" => "Percentage",
                "Fixed Amount" => "Fixed Amount",
                _ => null
            };
        }

        private static string? NormalizeEarningRuleAppliesTo(string? appliesTo)
        {
            return appliesTo?.Trim() switch
            {
                "Fuel" => "Fuel",
                "Product" => "Products",
                "Products" => "Products",
                "Both" => "Both",
                _ => null
            };
        }

        private static string? NormalizeEarningRuleType(string? earnType)
        {
            return earnType?.Trim() switch
            {
                "Percentage" => "Percentage",
                "Fixed Points" => "Fixed Points",
                _ => null
            };
        }

        private async Task<List<SelectListItem>> BuildRequiredDiscountOptionsAsync()
        {
            var options = await _db.Discounts.AsNoTracking()
                .Where(discount => discount.Status == 1)
                .OrderBy(discount => discount.Name)
                .Select(discount => new SelectListItem { Value = discount.Id.ToString(), Text = discount.Name })
                .ToListAsync();

            options.Insert(0, new SelectListItem { Value = "0", Text = "Select discount" });
            return options;
        }

        private async Task<List<SelectListItem>> BuildRequiredEarningsOptionsAsync()
        {
            var options = await _db.Earnings.AsNoTracking()
                .Where(earnings => earnings.Status == 1)
                .OrderBy(earnings => earnings.Name)
                .Select(earnings => new SelectListItem { Value = earnings.Id.ToString(), Text = earnings.Name })
                .ToListAsync();

            options.Insert(0, new SelectListItem { Value = "0", Text = "Select earnings" });
            return options;
        }

        private async Task<SetupModulesPageViewModel> BuildShiftSettingsPageAsync(string? search, ShiftSettingForm? form = null, int? editId = null, string activeModalId = "")
        {
            var query = _db.ShiftSettings.AsNoTracking();
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText)) query = query.Where(setting => setting.Name.Contains(searchText));
            return new SetupModulesPageViewModel { Search = searchText, ActiveModalId = activeModalId, ShiftSettingForm = form ?? await BuildShiftSettingFormAsync(editId), ShiftSettings = await query.OrderBy(setting => setting.Id).ToListAsync() };
        }

        private async Task<SetupModulesPageViewModel> BuildShiftSchedulePageAsync(string? search, EmployeeShiftScheduleForm? form = null, int? editId = null, string activeModalId = "")
        {
            IQueryable<EmployeeShiftSchedule> query = _db.EmployeeShiftSchedules.AsNoTracking()
                .Include(schedule => schedule.EmployeeAccount)
                .Include(schedule => schedule.ShiftSetting);
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(schedule => (schedule.EmployeeAccount != null && schedule.EmployeeAccount.FullName.Contains(searchText))
                    || (schedule.EmployeeAccount != null && schedule.EmployeeAccount.Username.Contains(searchText))
                    || (schedule.ShiftSetting != null && schedule.ShiftSetting.Name.Contains(searchText)));
            }

            var schedules = await query.OrderBy(schedule => schedule.EmployeeAccount!.FullName).ThenBy(schedule => schedule.DayOfWeek).ThenBy(schedule => schedule.StartTime).ToListAsync();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                schedules = schedules.Where(schedule => DayName(schedule.DayOfWeek).Contains(searchText, StringComparison.OrdinalIgnoreCase)
                    || (schedule.EmployeeAccount?.FullName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (schedule.EmployeeAccount?.Username.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (schedule.ShiftSetting?.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
            }

            return new SetupModulesPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                EmployeeShiftScheduleForm = form ?? await BuildShiftScheduleFormAsync(editId),
                EmployeeAccountOptions = await BuildEmployeeAccountOptionsAsync(),
                DayOptions = BuildDayOptions(),
                ShiftScheduleShiftOptions = await BuildShiftScheduleShiftOptionsAsync(),
                EmployeeShiftSchedules = schedules
            };
        }

        private async Task<SetupModulesPageViewModel> BuildSchedulesPageAsync(string? search, ScheduleForm? form = null, int? editId = null, int? detailsId = null, string activeModalId = "")
        {
            var query = _db.Schedules.AsNoTracking();
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText)) query = query.Where(schedule => schedule.Name.Contains(searchText));

            var selectedDetailsId = detailsId ?? 0;
            return new SetupModulesPageViewModel
            {
                Search = searchText,
                ActiveModalId = activeModalId,
                ScheduleForm = form ?? await BuildScheduleFormAsync(editId),
                ScheduleDetailsForm = await BuildScheduleDetailsFormAsync(selectedDetailsId),
                Schedules = await query.OrderBy(schedule => schedule.Id).ToListAsync()
            };
        }

        private async Task<SetupModulesPageViewModel> BuildEmployeeSchedulesPageAsync(string? search)
        {
            IQueryable<EmployeeAccount> query = _db.EmployeeAccounts.AsNoTracking().Include(account => account.Schedule);
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(account => account.FullName.Contains(searchText)
                    || account.Username.Contains(searchText)
                    || (account.Schedule != null && account.Schedule.Name.Contains(searchText)));
            }

            return new SetupModulesPageViewModel
            {
                Search = searchText,
                ScheduleOptions = await BuildScheduleOptionsAsync(),
                EmployeeScheduleAccounts = await query.OrderBy(account => account.FullName).ToListAsync()
            };
        }

        private async Task<SetupModulesPageViewModel> BuildRolePermissionsPageAsync(int? roleId, RolePermissionsForm? form = null)
        {
            var selectedRoleId = form?.RoleId ?? roleId ?? 0;
            var assigned = selectedRoleId > 0 ? await _db.RolePermissions.AsNoTracking().Where(item => item.RoleId == selectedRoleId && item.Status == 1).Select(item => item.PermissionId).ToListAsync() : new List<int>();
            return new SetupModulesPageViewModel
            {
                RolePermissionsForm = form ?? new RolePermissionsForm { RoleId = selectedRoleId, PermissionIds = assigned },
                RoleOptions = await BuildRoleOptionsAsync(),
                Roles = await _db.Roles.AsNoTracking().Where(role => role.Status == 1).OrderBy(role => role.Name).ToListAsync(),
                Permissions = await _db.Permissions.AsNoTracking().Where(permission => permission.Status == 1).OrderBy(permission => permission.ParentId).ThenBy(permission => permission.Name).ToListAsync(),
                AssignedPermissionIds = assigned
            };
        }

        private async Task<SetupModulesPageViewModel> BuildActivityLogsPageAsync(string? search)
        {
            var query = _db.ActivityLogs.AsNoTracking();
            var searchText = (search ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(searchText)) query = query.Where(log => (log.Username != null && log.Username.Contains(searchText)) || log.Action.Contains(searchText) || (log.Module != null && log.Module.Contains(searchText)));
            return new SetupModulesPageViewModel { Search = searchText, ActivityLogs = await query.OrderByDescending(log => log.CreatedAt).ToListAsync() };
        }

        private async Task<ProductUnitForm> BuildProductUnitFormAsync(int? editId)
        {
            var item = editId.HasValue ? await _db.ProductUnits.AsNoTracking().FirstOrDefaultAsync(unit => unit.Id == editId.Value) : null;
            return item is null ? new ProductUnitForm() : new ProductUnitForm { Id = item.Id, Name = item.Name, Abbreviation = item.Abbreviation, Status = item.Status };
        }

        private async Task<ProductBatchForm> BuildProductBatchFormAsync(int? editId)
        {
            var item = editId.HasValue ? await _db.ProductBatches.AsNoTracking().FirstOrDefaultAsync(batch => batch.Id == editId.Value) : null;
            return item is null ? new ProductBatchForm() : new ProductBatchForm { Id = item.Id, ProductId = item.ProductId, SupplierId = item.SupplierId, BatchNo = item.BatchNo, CostPrice = item.CostPrice, SellingPrice = item.SellingPrice, ExpiryDate = item.ExpiryDate, Status = item.Status };
        }

        private async Task<StockReceivingForm> BuildStockReceivingFormAsync(int? editId)
        {
            var item = editId.HasValue ? await _db.StockReceivings.AsNoTracking().Include(receiving => receiving.Items).FirstOrDefaultAsync(receiving => receiving.Id == editId.Value) : null;
            var firstItem = item?.Items.FirstOrDefault();
            return item is null ? new StockReceivingForm() : new StockReceivingForm { Id = item.Id, SupplierId = item.SupplierId, ReceivedDate = item.ReceivedDate, Remarks = item.Remarks, ProductId = firstItem?.ProductId ?? 0, Quantity = firstItem?.Quantity, CostPrice = firstItem?.CostPrice, SellingPrice = firstItem?.SellingPrice, ExpiryDate = firstItem?.ExpiryDate, Status = item.Status };
        }

        private async Task<LowStockSettingForm> BuildLowStockSettingFormAsync(int? editId)
        {
            var item = editId.HasValue ? await _db.LowStockSettings.AsNoTracking().FirstOrDefaultAsync(setting => setting.Id == editId.Value) : null;
            return item is null ? new LowStockSettingForm() : new LowStockSettingForm { Id = item.Id, ProductId = item.ProductId, ProductBatchId = item.ProductBatchId, Location = item.Location, MinimumQuantity = item.MinimumQuantity, Status = item.Status };
        }

        private async Task<NozzleForm> BuildNozzleFormAsync(int? editId)
        {
            var item = editId.HasValue ? await _db.Nozzles.AsNoTracking().FirstOrDefaultAsync(nozzle => nozzle.Id == editId.Value) : null;
            return item is null ? new NozzleForm() : new NozzleForm { Id = item.Id, PumpId = item.PumpId, NozzleNo = item.NozzleNo, Status = item.Status };
        }

        private async Task<FuelDeliveryForm> BuildFuelDeliveryFormAsync(int? editId)
        {
            var item = editId.HasValue ? await _db.FuelDeliveries.AsNoTracking().FirstOrDefaultAsync(delivery => delivery.Id == editId.Value) : null;
            return item is null ? new FuelDeliveryForm() : new FuelDeliveryForm { Id = item.Id, DeliveryNo = item.DeliveryNo, SupplierId = item.SupplierId, FuelId = item.FuelId, TankId = item.TankId, DeliveredLiters = item.DeliveredLiters, CostPerLiter = item.CostPerLiter, DeliveryDate = item.DeliveryDate, Remarks = item.Remarks, Status = item.Status };
        }

        private async Task<PumpMeterReadingForm> BuildPumpMeterReadingFormAsync(int? editId)
        {
            var item = editId.HasValue ? await _db.PumpMeterReadings.AsNoTracking().FirstOrDefaultAsync(reading => reading.Id == editId.Value) : null;
            return item is null ? new PumpMeterReadingForm() : new PumpMeterReadingForm { Id = item.Id, NozzleId = item.NozzleId ?? 0, OpeningMeter = item.OpeningMeter, ClosingMeter = item.ClosingMeter, ReadingDate = item.ReadingDate, Status = item.Status };
        }

        private async Task<DiscountRuleForm> BuildDiscountRuleFormAsync(int? editId)
        {
            var item = editId.HasValue ? await _db.DiscountRules.AsNoTracking().FirstOrDefaultAsync(rule => rule.Id == editId.Value) : null;
            return item is null ? new DiscountRuleForm() : new DiscountRuleForm { Id = item.Id, DiscountId = item.DiscountId, AppliesTo = item.AppliesTo, DiscountType = item.DiscountType, DiscountValue = item.DiscountValue, MinimumAmount = item.MinimumAmount, MemberRequired = item.MemberRequired == 1, StartDate = item.StartDate, EndDate = item.EndDate, Status = item.Status };
        }

        private async Task<EarningRuleForm> BuildEarningRuleFormAsync(int? editId)
        {
            var item = editId.HasValue ? await _db.EarningRules.AsNoTracking().FirstOrDefaultAsync(rule => rule.Id == editId.Value) : null;
            return item is null ? new EarningRuleForm() : new EarningRuleForm { Id = item.Id, EarningsId = item.EarningsId, Name = item.Name, AppliesTo = item.AppliesTo, EarnType = item.EarnType, EarnValue = item.EarnValue, MinimumAmount = item.MinimumAmount, MemberRequired = item.MemberRequired == 1, StartDate = item.StartDate, EndDate = item.EndDate, Status = item.Status };
        }

        private async Task<ShiftSettingForm> BuildShiftSettingFormAsync(int? editId)
        {
            var item = editId.HasValue ? await _db.ShiftSettings.AsNoTracking().FirstOrDefaultAsync(setting => setting.Id == editId.Value) : null;
            return item is null ? new ShiftSettingForm() : new ShiftSettingForm { Id = item.Id, Name = item.Name, StartTime = item.StartTime, EndTime = item.EndTime, RequireOpeningCash = item.RequireOpeningCash == 1, AllowCashIn = item.AllowCashIn == 1, AllowCashOut = item.AllowCashOut == 1, RequireClosingApproval = item.RequireClosingApproval == 1, Status = item.Status };
        }

        private async Task<EmployeeShiftScheduleForm> BuildShiftScheduleFormAsync(int? editId)
        {
            var item = editId.HasValue ? await _db.EmployeeShiftSchedules.AsNoTracking().FirstOrDefaultAsync(schedule => schedule.Id == editId.Value) : null;
            return item is null ? new EmployeeShiftScheduleForm() : new EmployeeShiftScheduleForm { Id = item.Id, EmployeeAccountId = item.EmployeeAccountId, DayOfWeek = item.DayOfWeek, ShiftSettingId = item.ShiftSettingId, StartTime = item.StartTime, EndTime = item.EndTime, Status = item.Status };
        }

        private async Task<ScheduleForm> BuildScheduleFormAsync(int? editId)
        {
            var item = editId.HasValue ? await _db.Schedules.AsNoTracking().FirstOrDefaultAsync(schedule => schedule.Id == editId.Value) : null;
            return item is null ? new ScheduleForm() : new ScheduleForm { Id = item.Id, Name = item.Name, Status = item.Status };
        }

        private async Task<ScheduleDetailsForm> BuildScheduleDetailsFormAsync(int scheduleId)
        {
            var existing = scheduleId > 0
                ? await _db.ScheduleDetails.AsNoTracking().Where(detail => detail.ScheduleId == scheduleId).ToListAsync()
                : new List<ScheduleDetail>();

            return new ScheduleDetailsForm
            {
                ScheduleId = scheduleId,
                Details = BuildDayOptions().Select(day =>
                {
                    var dayValue = int.Parse(day.Value);
                    var detail = existing.FirstOrDefault(item => item.DayOfWeek == dayValue);
                    return new ScheduleDetailLineForm
                    {
                        DayOfWeek = dayValue,
                        DayName = day.Text,
                        AmIn = detail?.AmIn,
                        AmOut = detail?.AmOut,
                        PmIn = detail?.PmIn,
                        PmOut = detail?.PmOut
                    };
                }).ToList()
            };
        }

        private async Task<List<SelectListItem>> BuildEmployeeAccountOptionsAsync()
        {
            return await _db.EmployeeAccounts.AsNoTracking()
                .Where(account => account.Status == 1)
                .OrderBy(account => account.FullName)
                .Select(account => new SelectListItem { Value = account.Id.ToString(), Text = $"{account.FullName} ({account.Username})" })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> BuildScheduleOptionsAsync()
        {
            return await _db.Schedules.AsNoTracking()
                .Where(schedule => schedule.Status == 1)
                .OrderBy(schedule => schedule.Name)
                .Select(schedule => new SelectListItem { Value = schedule.Id.ToString(), Text = schedule.Name })
                .ToListAsync();
        }

        private static List<SelectListItem> BuildDayOptions()
        {
            return new List<SelectListItem>
            {
                new() { Value = "1", Text = "Monday" },
                new() { Value = "2", Text = "Tuesday" },
                new() { Value = "3", Text = "Wednesday" },
                new() { Value = "4", Text = "Thursday" },
                new() { Value = "5", Text = "Friday" },
                new() { Value = "6", Text = "Saturday" },
                new() { Value = "7", Text = "Sunday" }
            };
        }

        private async Task<List<ShiftScheduleShiftOption>> BuildShiftScheduleShiftOptionsAsync()
        {
            return await _db.ShiftSettings.AsNoTracking()
                .Where(setting => setting.Status == 1)
                .OrderBy(setting => setting.Name)
                .Select(setting => new ShiftScheduleShiftOption { Id = setting.Id, Name = setting.Name, StartTime = setting.StartTime, EndTime = setting.EndTime })
                .ToListAsync();
        }

        private static string DayName(int dayOfWeek)
        {
            return dayOfWeek switch
            {
                1 => "Monday",
                2 => "Tuesday",
                3 => "Wednesday",
                4 => "Thursday",
                5 => "Friday",
                6 => "Saturday",
                7 => "Sunday",
                _ => string.Empty
            };
        }

        private static bool TimeRangesOverlap(TimeSpan firstStart, TimeSpan firstEnd, TimeSpan secondStart, TimeSpan secondEnd)
        {
            var firstRanges = ExpandTimeRange(firstStart, firstEnd);
            var secondRanges = ExpandTimeRange(secondStart, secondEnd);
            return firstRanges.Any(first => secondRanges.Any(second => first.Start < second.End && second.Start < first.End));
        }

        private static List<(TimeSpan Start, TimeSpan End)> ExpandTimeRange(TimeSpan start, TimeSpan end)
        {
            var dayEnd = TimeSpan.FromDays(1);
            return start < end
                ? new List<(TimeSpan Start, TimeSpan End)> { (start, end) }
                : new List<(TimeSpan Start, TimeSpan End)> { (start, dayEnd), (TimeSpan.Zero, end) };
        }

        private async Task<List<SelectListItem>> BuildProductOptionsAsync()
        {
            return await _db.Products.AsNoTracking().Where(product => product.Status == 1 && product.IsActive).OrderBy(product => product.Name).Select(product => new SelectListItem { Value = product.Id.ToString(), Text = product.Name }).ToListAsync();
        }

        private async Task<List<ProductSearchOption>> BuildProductSearchOptionsAsync()
        {
            var products = await _db.Products.AsNoTracking()
                .Include(product => product.Category)
                .Include(product => product.ProductBatches)
                .Where(product => product.Status == 1 && product.IsActive)
                .OrderBy(product => product.Name)
                .ToListAsync();

            return products.Select(product =>
            {
                var batch = product.ProductBatches
                    .Where(item => item.Status == 1)
                    .OrderByDescending(item => item.Id)
                    .FirstOrDefault();
                var category = product.Category?.Name ?? string.Empty;
                var batchNo = batch?.BatchNo ?? string.Empty;
                var displayParts = new[] { product.Name, category, batchNo }
                    .Where(value => !string.IsNullOrWhiteSpace(value));
                var displayText = string.Join(" - ", displayParts);
                var searchText = string.Join(" ", product.Name, category, batchNo, batch?.CostPrice.ToString("0.##"), batch?.SellingPrice.ToString("0.##"));

                return new ProductSearchOption
                {
                    Id = product.Id,
                    Name = product.Name,
                    Category = category,
                    BatchNo = batchNo,
                    CostPrice = batch?.CostPrice,
                    SellingPrice = batch?.SellingPrice,
                    DisplayText = displayText,
                    SearchText = searchText
                };
            }).ToList();
        }

        private async Task<List<SelectListItem>> BuildProductBatchOptionsAsync()
        {
            return await _db.ProductBatches.AsNoTracking().Where(batch => batch.Status == 1).OrderBy(batch => batch.BatchNo).Select(batch => new SelectListItem { Value = batch.Id.ToString(), Text = batch.BatchNo }).ToListAsync();
        }

        private async Task<List<SelectListItem>> BuildPumpOptionsAsync()
        {
            return await _db.Pumps.AsNoTracking().Where(pump => pump.Status == 1).OrderBy(pump => pump.Name).Select(pump => new SelectListItem { Value = pump.Id.ToString(), Text = pump.Name }).ToListAsync();
        }

        private async Task<List<SelectListItem>> BuildNozzleOptionsAsync()
        {
            return await _db.Nozzles.AsNoTracking().Where(nozzle => nozzle.Status == 1).OrderBy(nozzle => nozzle.NozzleNo).Select(nozzle => new SelectListItem { Value = nozzle.Id.ToString(), Text = nozzle.NozzleNo }).ToListAsync();
        }

        private async Task<List<Nozzle>> BuildActiveNozzlesAsync()
        {
            return await _db.Nozzles.AsNoTracking()
                .Include(nozzle => nozzle.Pump)
                .ThenInclude(pump => pump!.Tank)
                .ThenInclude(tank => tank!.Fuel)
                .Where(nozzle => nozzle.Status == 1
                    && nozzle.Pump != null
                    && nozzle.Pump.Status == 1
                    && nozzle.Pump.Tank != null
                    && nozzle.Pump.Tank.Status == 1
                    && nozzle.Pump.Tank.IsActive
                    && nozzle.Pump.Tank.Fuel != null
                    && nozzle.Pump.Tank.Fuel.Status == 1
                    && nozzle.Pump.Tank.Fuel.IsActive)
                .OrderBy(nozzle => nozzle.NozzleNo)
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> BuildMemberOptionsAsync()
        {
            return await _db.Members.AsNoTracking().Where(member => member.Status == 1).OrderBy(member => member.FullName).Select(member => new SelectListItem { Value = member.Id.ToString(), Text = member.FullName }).ToListAsync();
        }

        private async Task<List<SelectListItem>> BuildRoleOptionsAsync()
        {
            return await _db.Roles.AsNoTracking().Where(role => role.Status == 1).OrderBy(role => role.Name).Select(role => new SelectListItem { Value = role.Id.ToString(), Text = role.Name }).ToListAsync();
        }

        private async Task<string> GenerateReceivingNoAsync()
        {
            var count = await _db.StockReceivings.CountAsync() + 1;
            return $"RCV-{DateTime.UtcNow:yyyyMMdd}-{count:0000}";
        }
    }
}

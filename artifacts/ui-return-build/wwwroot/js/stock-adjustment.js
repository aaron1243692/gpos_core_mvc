(function () {
  function escapeHtml(value) {
    return String(value ?? "")
      .replaceAll("&", "&amp;").replaceAll("<", "&lt;").replaceAll(">", "&gt;")
      .replaceAll('"', "&quot;").replaceAll("'", "&#039;");
  }

  function number(value) {
    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : 0;
  }

  function restoreParentModal() {
    if (document.querySelector(".modal.show")) document.body.classList.add("modal-open");
  }

  document.addEventListener("DOMContentLoaded", function () {
    const root = document.querySelector("[data-stock-adjustment]");
    const selectorElement = document.getElementById("stockAdjustmentTargetSelectorModal");
    if (!root || !selectorElement || !window.bootstrap) return;

    const scope = root.dataset.scope;
    const branch = root.querySelector("[data-adjustment-branch]");
    const warehouseId = root.querySelector("[data-adjustment-warehouse-id]");
    const displayId = root.querySelector("[data-adjustment-display-id]");
    const tankId = root.querySelector("[data-adjustment-tank-id]");
    const targetDisplay = root.querySelector("[data-adjustment-target-display]");
    const product = root.querySelector("[data-adjustment-product]");
    const batch = root.querySelector("[data-adjustment-batch]");
    const current = root.querySelector("[data-adjustment-current]");
    const capacity = root.querySelector("[data-adjustment-capacity]");
    const type = root.querySelector("[data-adjustment-type]");
    const quantity = root.querySelector("[data-adjustment-quantity]");
    const after = root.querySelector("[data-adjustment-after]");
    const activeBatch = root.querySelector("[data-adjustment-active-batch]");
    const difference = root.querySelector("[data-adjustment-difference]");
    const batchCount = root.querySelector("[data-adjustment-batch-count]");
    const inventoryState = root.querySelector("[data-adjustment-inventory-state]");
    const costMode = root.querySelector("[data-reconciliation-cost-mode]");
    const unitCost = root.querySelector("[data-reconciliation-unit-cost]");
    const totalCost = root.querySelector("[data-reconciliation-total-cost]");
    const message = root.querySelector("[data-adjustment-target-message]");
    const search = selectorElement.querySelector("[data-adjustment-selector-search]");
    const searchButton = selectorElement.querySelector("[data-adjustment-selector-search-button]");
    const body = selectorElement.querySelector("[data-adjustment-selector-body]");
    const selectorModal = bootstrap.Modal.getOrCreateInstance(selectorElement);
    const applyElement = document.getElementById("stockAdjustmentApplyModal");
    const applyModal = applyElement ? bootstrap.Modal.getOrCreateInstance(applyElement, { backdrop: "static", keyboard: false }) : null;
    const applyNumber = applyElement?.querySelector("[data-adjustment-apply-number]");
    const applyFeedback = applyElement?.querySelector("[data-adjustment-apply-feedback]");
    const applyConfirm = applyElement?.querySelector("[data-adjustment-apply-confirm]");
    const applyCancelButtons = applyElement ? Array.from(applyElement.querySelectorAll("[data-adjustment-apply-cancel]")) : [];
    let pendingApplyForm = null;

    function targetIdInput() {
      return scope === "Warehouse" ? warehouseId : scope === "Display" ? displayId : tankId;
    }

    function calculate() {
      if (scope === "Fuel" && type.value === "Reconciliation") quantity.value = Math.max(0, number(difference?.value)).toFixed(2);
      const result = scope === "Fuel" && type.value === "Reconciliation" ? number(current.value) : number(current.value) + (type.value === "Increase" ? number(quantity.value) : -number(quantity.value));
      after.value = result.toFixed(3);
      after.classList.toggle("is-invalid", result < 0 || (scope === "Fuel" && number(capacity.value) > 0 && result > number(capacity.value)));
    }

    function clearTarget(clearQuantity) {
      if (warehouseId) warehouseId.value = "";
      if (displayId) displayId.value = "";
      if (tankId) tankId.value = "";
      targetDisplay.value = "";
      product.value = "";
      if (batch) batch.value = "";
      current.value = "0.000";
      if (capacity) capacity.value = "";
      if (activeBatch) activeBatch.value = "0.00";
      if (difference) difference.value = "0.00";
      if (batchCount) batchCount.value = "0";
      if (inventoryState) inventoryState.value = "";
      if (clearQuantity) quantity.value = "";
      calculate();
    }

    function selectTarget(item) {
      targetIdInput().value = item.id;
      if (scope === "Fuel") {
        targetDisplay.value = item.tankName;
        product.value = item.fuelName;
        capacity.value = number(item.capacity).toFixed(3);
        activeBatch.value = number(item.activeBatchLiters).toFixed(2);
        difference.value = number(item.difference).toFixed(2);
        batchCount.value = item.activeBatchCount ?? 0;
        inventoryState.value = item.inventoryState || "";
      } else {
        targetDisplay.value = `${item.productName} - Batch ${item.batchNumber}`;
        product.value = item.productName;
        batch.value = item.batchNumber;
      }
      current.value = number(item.currentQuantity).toFixed(3);
      calculate();
      selectorModal.hide();
      window.setTimeout(restoreParentModal, 150);
    }

    function render(items) {
      if (!items.length) {
        body.innerHTML = `<tr><td colspan="${scope === "Fuel" ? 8 : 8}" class="text-center text-muted">No records found.</td></tr>`;
        return;
      }
      body.innerHTML = "";
      items.forEach(function (item, index) {
        const row = document.createElement("tr");
        if (scope === "Fuel") {
          row.innerHTML = `<td>${index + 1}</td><td>${escapeHtml(item.tankName)}</td><td>${escapeHtml(item.fuelName)}</td><td>${number(item.currentQuantity).toFixed(3)}</td><td>${number(item.capacity).toFixed(3)}</td><td>${number(item.availableSpace).toFixed(3)}</td><td>${escapeHtml(item.status)}</td><td><button type="button" class="btn btn-sm btn-outline-primary">Select</button></td>`;
        } else {
          const expiry = item.expiryDate ? new Date(item.expiryDate).toLocaleDateString() : "-";
          row.innerHTML = `<td>${index + 1}</td><td>${escapeHtml(item.productName)}</td><td>${escapeHtml(item.batchNumber)}</td><td>${number(item.currentQuantity).toFixed(3)}</td><td>${number(item.price).toFixed(2)}</td><td>${escapeHtml(expiry)}</td><td>${escapeHtml(item.status)}</td><td><button type="button" class="btn btn-sm btn-outline-primary">Select</button></td>`;
        }
        row.querySelector("button").addEventListener("click", function () { selectTarget(item); });
        body.appendChild(row);
      });
    }

    async function loadTargets() {
      body.innerHTML = `<tr><td colspan="8" class="text-center text-muted">Loading...</td></tr>`;
      try {
        const response = await fetch(`/Transaction/SearchStockAdjustmentTargets?scope=${encodeURIComponent(scope)}&branchId=${encodeURIComponent(branch.value)}&search=${encodeURIComponent(search.value.trim())}`, { headers: { Accept: "application/json" } });
        if (!response.ok) throw new Error("Search failed");
        render(await response.json());
      } catch {
        body.innerHTML = `<tr><td colspan="8" class="text-center text-danger">Unable to load records.</td></tr>`;
      }
    }

    root.querySelector("[data-adjustment-open-target]").addEventListener("click", function () {
      if (!branch.value || number(branch.value) <= 0) {
        message.classList.remove("d-none");
        return;
      }
      message.classList.add("d-none");
      selectorModal.show();
      loadTargets();
    });
    searchButton.addEventListener("click", loadTargets);
    search.addEventListener("keydown", function (event) { if (event.key === "Enter") { event.preventDefault(); loadTargets(); } });
    selectorElement.addEventListener("hidden.bs.modal", restoreParentModal);
    branch.addEventListener("change", function () { message.classList.add("d-none"); clearTarget(true); });
    quantity.addEventListener("input", calculate);
    type.addEventListener("change", calculate);
    function calculateCost(changed) {
      const liters = Math.max(0, number(difference?.value));
      if (costMode?.value === "UnitCost" && changed !== "total") totalCost.value = (liters * number(unitCost.value)).toFixed(2);
      if (costMode?.value === "TotalCost" && changed !== "unit") unitCost.value = liters > 0 ? (number(totalCost.value) / liters).toFixed(2) : "";
    }
    costMode?.addEventListener("change", function () { calculateCost(); });
    unitCost?.addEventListener("input", function () { calculateCost("unit"); });
    totalCost?.addEventListener("input", function () { calculateCost("total"); });

    if (root.dataset.initialTargetId) {
      targetDisplay.value = root.dataset.initialTargetName || "";
      product.value = root.dataset.initialProduct || "";
      if (batch) batch.value = root.dataset.initialBatch || "";
      current.value = number(root.dataset.initialCurrent).toFixed(3);
      if (capacity) capacity.value = root.dataset.initialCapacity ? number(root.dataset.initialCapacity).toFixed(3) : "";
      if (activeBatch) activeBatch.value = number(root.dataset.initialActiveBatch).toFixed(2);
      if (difference) difference.value = number(root.dataset.initialDifference).toFixed(2);
      if (batchCount) batchCount.value = root.dataset.initialBatchCount || "0";
      if (inventoryState) inventoryState.value = root.dataset.initialInventoryState || "";
    }
    calculate();

    document.querySelectorAll("[data-adjustment-apply-open]").forEach(function (button) {
      button.addEventListener("click", function () {
        pendingApplyForm = button.closest("[data-adjustment-apply-form]");
        if (!pendingApplyForm || !applyModal) return;
        applyNumber.textContent = button.dataset.adjustmentNumber || "this adjustment";
        applyFeedback.textContent = "";
        applyFeedback.classList.add("d-none");
        applyConfirm.disabled = false;
        applyConfirm.textContent = "Apply";
        applyCancelButtons.forEach(function (cancel) { cancel.disabled = false; });
        applyModal.show();
      });
    });

    if (applyConfirm) {
      applyConfirm.addEventListener("click", async function () {
        if (!pendingApplyForm || applyConfirm.disabled) return;
        applyConfirm.disabled = true;
        applyConfirm.textContent = "Applying...";
        applyCancelButtons.forEach(function (cancel) { cancel.disabled = true; });
        applyFeedback.classList.add("d-none");
        try {
          const response = await fetch(pendingApplyForm.action, {
            method: "POST",
            body: new FormData(pendingApplyForm),
            headers: { Accept: "application/json", "X-Requested-With": "XMLHttpRequest" }
          });
          const contentType = response.headers.get("content-type") || "";
          const result = contentType.includes("application/json") ? await response.json() : null;
          if (!response.ok || !result?.success) throw new Error(result?.message || "Unable to apply the adjustment.");
          applyModal.hide();
          window.location.reload();
        } catch (error) {
          applyFeedback.textContent = error instanceof Error ? error.message : "Unable to apply the adjustment.";
          applyFeedback.classList.remove("d-none");
          applyConfirm.disabled = false;
          applyConfirm.textContent = "Apply";
          applyCancelButtons.forEach(function (cancel) { cancel.disabled = false; });
        }
      });
    }
  });
})();

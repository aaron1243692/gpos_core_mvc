(function () {
  const modalSelector = "#branchSelectorModal";
  let activeSelector = null;

  function query(selector) {
    return selector ? document.querySelector(selector) : null;
  }

  function setBranch(selector, branch) {
    const hidden = query(selector.dataset.branchHidden);
    const display = query(selector.dataset.branchDisplay);

    if (hidden) {
      hidden.value = branch ? branch.id : "";
      hidden.dispatchEvent(new Event("change", { bubbles: true }));
    }

    if (display) {
      display.value = branch ? branch.name : selector.dataset.branchEmptyText || "All Branches";
      display.dispatchEvent(new Event("change", { bubbles: true }));
    }
  }

  function renderRows(tbody, branches) {
    tbody.innerHTML = "";

    if (!branches.length) {
      const row = document.createElement("tr");
      row.innerHTML = '<td class="text-center text-muted fw-bold" colspan="6">No branches found.</td>';
      tbody.appendChild(row);
      return;
    }

    branches.forEach((branch, index) => {
      const row = document.createElement("tr");
      row.innerHTML = `
        <td>${index + 1}</td>
        <td>${escapeHtml(branch.name || "-")}</td>
        <td>${escapeHtml(branch.code || "-")}</td>
        <td>${escapeHtml(branch.address || "-")}</td>
        <td>${escapeHtml(branch.status || "-")}</td>
        <td><button class="btn btn-sm btn-outline-primary" type="button" data-branch-select-row>Select</button></td>
      `;

      row.querySelector("[data-branch-select-row]").addEventListener("click", () => {
        if (activeSelector) {
          setBranch(activeSelector, branch);
        }

        const modalElement = document.querySelector(modalSelector);
        if (modalElement && window.bootstrap) {
          bootstrap.Modal.getOrCreateInstance(modalElement).hide();
        }
      });

      tbody.appendChild(row);
    });
  }

  function escapeHtml(value) {
    return String(value)
      .replaceAll("&", "&amp;")
      .replaceAll("<", "&lt;")
      .replaceAll(">", "&gt;")
      .replaceAll('"', "&quot;")
      .replaceAll("'", "&#039;");
  }

  async function searchBranches(modalElement) {
    const input = modalElement.querySelector("[data-branch-search-input]");
    const tbody = modalElement.querySelector("[data-branch-search-results]");
    const queryText = input ? input.value.trim() : "";

    if (!tbody) {
      return;
    }

    tbody.innerHTML = '<tr><td class="text-center text-muted fw-bold" colspan="6">Loading branches...</td></tr>';

    try {
      const response = await fetch(`/Configuration/SearchBranches?search=${encodeURIComponent(queryText)}`, {
        headers: { "Accept": "application/json" }
      });

      if (!response.ok) {
        throw new Error("Branch search failed.");
      }

      renderRows(tbody, await response.json());
    } catch {
      tbody.innerHTML = '<tr><td class="text-center text-danger fw-bold" colspan="6">Unable to load branches.</td></tr>';
    }
  }

  document.addEventListener("DOMContentLoaded", () => {
    const modalElement = document.querySelector(modalSelector);

    document.querySelectorAll("[data-branch-selector]").forEach((selector) => {
      const button = selector.querySelector("[data-branch-open], .branch-select-btn");
      const clearButton = selector.querySelector("[data-branch-clear]");

      if (button) {
        button.addEventListener("click", () => {
          activeSelector = selector;

          if (modalElement && window.bootstrap) {
            bootstrap.Modal.getOrCreateInstance(modalElement).show();
            searchBranches(modalElement);
          }
        });
      }

      if (clearButton) {
        clearButton.addEventListener("click", () => setBranch(selector, null));
      }
    });

    if (!modalElement) {
      return;
    }

    const searchInput = modalElement.querySelector("[data-branch-search-input]");
    const searchButton = modalElement.querySelector("[data-branch-search-button]");

    if (searchButton) {
      searchButton.addEventListener("click", () => searchBranches(modalElement));
    }

    if (searchInput) {
      searchInput.addEventListener("keydown", (event) => {
        if (event.key === "Enter") {
          event.preventDefault();
          searchBranches(modalElement);
        }
      });
    }
  });
})();

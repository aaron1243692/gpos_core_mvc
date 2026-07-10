document.addEventListener('DOMContentLoaded', function () {
    let activeSelector = null;
    const modalElement = document.getElementById('dailyCashSelectorModal');
    const results = modalElement?.querySelector('[data-daily-cash-search-results]');
    const searchInput = modalElement?.querySelector('[data-daily-cash-search-input]');

    const element = (selector) => selector ? document.querySelector(selector) : null;
    const html = (value) => String(value ?? '').replaceAll('&', '&amp;').replaceAll('<', '&lt;').replaceAll('>', '&gt;').replaceAll('"', '&quot;').replaceAll("'", '&#039;');
    const money = (value) => Number(value ?? 0).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    const restoreParentModal = () => { if (document.querySelector('.modal.show')) document.body.classList.add('modal-open'); };

    function clearDailyCash(selector) {
        element(selector.dataset.dailyCashHidden).value = '';
        element(selector.dataset.dailyCashDisplay).value = '';
        element(selector.dataset.businessDateDisplay).value = '';
        element(selector.dataset.shiftDisplay).value = '';
        element(selector.dataset.userDisplay).value = '';
    }

    function setDailyCash(selector, row) {
        element(selector.dataset.dailyCashHidden).value = row.dailyCashId;
        element(selector.dataset.dailyCashDisplay).value = `${row.dailyCashNo} - ${row.businessDate} - ${row.shiftName} - ${row.cashierName}`;
        element(selector.dataset.branchDisplay).value = row.branchName;
        element(selector.dataset.businessDateDisplay).value = row.businessDate;
        element(selector.dataset.shiftDisplay).value = row.shiftName;
        element(selector.dataset.userDisplay).value = row.cashierName;
    }

    async function searchDailyCash() {
        if (!results || !activeSelector) return;
        const branchId = element(activeSelector.dataset.branchHidden)?.value;
        if (!branchId) {
            results.innerHTML = '<tr><td class="text-center text-muted fw-bold" colspan="10">Please select a Branch first.</td></tr>';
            return;
        }

        results.innerHTML = '<tr><td class="text-center text-muted fw-bold" colspan="10">Loading Daily Cash...</td></tr>';
        const query = searchInput?.value.trim() ?? '';

        try {
            const response = await fetch(`/Transaction/SearchDailyCashForCashIn?branchId=${encodeURIComponent(branchId)}&term=${encodeURIComponent(query)}`, { headers: { Accept: 'application/json' } });
            const contentType = response.headers.get('content-type') || '';
            if (!response.ok || !contentType.includes('application/json')) throw new Error('Daily Cash search did not return JSON.');

            const rows = await response.json();
            results.innerHTML = rows.length ? '' : '<tr><td class="text-center text-muted fw-bold" colspan="10">No eligible Daily Cash records found for this Branch.</td></tr>';
            rows.forEach((row, index) => {
                const tr = document.createElement('tr');
                tr.innerHTML = `<td>${index + 1}</td><td>${html(row.dailyCashNo)}</td><td>${html(row.businessDate)}</td><td>${html(row.branchName)}</td><td>${html(row.shiftName)}</td><td>${html(row.cashierName)}</td><td>${html(money(row.openingCash))}</td><td>${html(money(row.expectedCash))}</td><td>${html(row.status)}</td><td><button class="btn btn-sm btn-outline-primary" type="button">Select</button></td>`;
                tr.querySelector('button').addEventListener('click', () => {
                    setDailyCash(activeSelector, row);
                    bootstrap.Modal.getOrCreateInstance(modalElement).hide();
                    setTimeout(restoreParentModal, 150);
                });
                results.appendChild(tr);
            });
        } catch {
            results.innerHTML = '<tr><td class="text-center text-danger fw-bold" colspan="10">Unable to load Daily Cash records.</td></tr>';
        }
    }

    document.querySelectorAll('[data-daily-cash-selector]').forEach((selector) => {
        element(selector.dataset.branchHidden)?.addEventListener('change', () => clearDailyCash(selector));
        selector.querySelector('[data-daily-cash-open]')?.addEventListener('click', (event) => {
            event.preventDefault();
            activeSelector = selector;
            const branchId = element(selector.dataset.branchHidden)?.value;
            if (!branchId) {
                window.alert('Please select a Branch first.');
                return;
            }
            bootstrap.Modal.getOrCreateInstance(modalElement).show();
            searchDailyCash();
        });
    });
    modalElement?.querySelector('[data-daily-cash-search-button]')?.addEventListener('click', searchDailyCash);
    searchInput?.addEventListener('keydown', (event) => { if (event.key === 'Enter') { event.preventDefault(); searchDailyCash(); } });
    modalElement?.addEventListener('hidden.bs.modal', restoreParentModal);
});

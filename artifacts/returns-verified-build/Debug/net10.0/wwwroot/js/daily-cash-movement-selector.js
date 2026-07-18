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
        if (element(selector.dataset.expectedCashDisplay)) element(selector.dataset.expectedCashDisplay).value = '';
        if (element(selector.dataset.actualCashDisplay)) element(selector.dataset.actualCashDisplay).value = '';
        if (element(selector.dataset.remittedAmountInput)) element(selector.dataset.remittedAmountInput).value = '';
        if (element(selector.dataset.differenceDisplay)) element(selector.dataset.differenceDisplay).value = '';
        if (element(selector.dataset.varianceDisplay)) { element(selector.dataset.varianceDisplay).textContent = ''; element(selector.dataset.varianceDisplay).className = 'alert d-none mb-0'; }
    }

    function setDailyCash(selector, row) {
        element(selector.dataset.dailyCashHidden).value = row.dailyCashId;
        element(selector.dataset.dailyCashDisplay).value = `${row.dailyCashNo} - ${row.businessDate} - ${row.shiftName} - ${row.cashierName}`;
        element(selector.dataset.branchDisplay).value = row.branchName;
        element(selector.dataset.businessDateDisplay).value = row.businessDate;
        element(selector.dataset.shiftDisplay).value = row.shiftName;
        element(selector.dataset.userDisplay).value = row.cashierName;
        if (element(selector.dataset.expectedCashDisplay)) element(selector.dataset.expectedCashDisplay).value = Number(row.expectedCash).toFixed(2);
        if (element(selector.dataset.actualCashDisplay)) element(selector.dataset.actualCashDisplay).value = Number(row.actualCash).toFixed(2);
        if (element(selector.dataset.differenceDisplay)) element(selector.dataset.differenceDisplay).value = '';
    }

    async function searchDailyCash() {
        if (!results || !activeSelector) return;
        const branchId = element(activeSelector.dataset.branchHidden)?.value;
        if (!branchId) {
            results.innerHTML = '<tr><td class="text-center text-muted fw-bold" colspan="15">Please select a Branch first.</td></tr>';
            return;
        }

        results.innerHTML = '<tr><td class="text-center text-muted fw-bold" colspan="15">Loading Daily Cash...</td></tr>';
        const query = searchInput?.value.trim() ?? '';

        try {
            const response = await fetch(`/Transaction/SearchDailyCashForCashIn?branchId=${encodeURIComponent(branchId)}&term=${encodeURIComponent(query)}`, { headers: { Accept: 'application/json' } });
            const contentType = response.headers.get('content-type') || '';
            if (!response.ok || !contentType.includes('application/json')) throw new Error('Daily Cash search did not return JSON.');

            const rows = await response.json();
            results.innerHTML = rows.length ? '' : '<tr><td class="text-center text-muted fw-bold" colspan="15">No eligible Daily Cash records found for this Branch.</td></tr>';
            rows.forEach((row, index) => {
                const tr = document.createElement('tr');
                tr.innerHTML = `<td>${index + 1}</td><td>${html(row.dailyCashNo)}</td><td>${html(row.businessDate)}</td><td>${html(row.branchName)}</td><td>${html(row.shiftName)}</td><td>${html(row.cashierName)}</td><td>${html(money(row.openingCash))}</td><td>${html(money(row.cashSales))}</td><td>${html(money(row.cashIn))}</td><td>${html(money(row.cashOut))}</td><td>${html(money(row.expectedCash))}</td><td>${html(money(row.actualCash))}</td><td>${html(money(row.countDifference))}</td><td>${html(row.status)}</td><td><button class="btn btn-sm btn-outline-primary" type="button">Select</button></td>`;
                tr.querySelector('button').addEventListener('click', () => {
                    setDailyCash(activeSelector, row);
                    bootstrap.Modal.getOrCreateInstance(modalElement).hide();
                    setTimeout(restoreParentModal, 150);
                });
                results.appendChild(tr);
            });
        } catch {
            results.innerHTML = '<tr><td class="text-center text-danger fw-bold" colspan="15">Unable to load Daily Cash records.</td></tr>';
        }
    }

    document.querySelectorAll('[data-daily-cash-selector]').forEach((selector) => {
        element(selector.dataset.branchHidden)?.addEventListener('change', () => clearDailyCash(selector));
        element(selector.dataset.remittedAmountInput)?.addEventListener('input', () => {
            const expected = Number(element(selector.dataset.expectedCashDisplay)?.value || 0);
            const remitted = Number(element(selector.dataset.remittedAmountInput)?.value || 0);
            const difference = remitted - expected;
            if (element(selector.dataset.differenceDisplay)) element(selector.dataset.differenceDisplay).value = difference.toFixed(2);
            const variance = element(selector.dataset.varianceDisplay);
            if (variance) {
                variance.className = `alert mb-0 ${difference === 0 ? 'alert-success' : difference < 0 ? 'alert-danger' : 'alert-warning'}`;
                variance.textContent = difference === 0 ? 'This remittance is balanced.' : difference < 0 ? `This remittance has a shortage of ₱${money(Math.abs(difference))}.` : `This remittance has an excess of ₱${money(difference)}.`;
            }
        });
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

document.addEventListener('DOMContentLoaded', () => {
    const c = window.priceAdjustmentSelectorConfig;
    if (!c) return;
    const byId = id => document.getElementById(id);
    const branchModalEl = byId('priceAdjustmentBranchSelector');
    const itemModalEl = byId('priceAdjustmentItemSelector');
    const branchModal = bootstrap.Modal.getOrCreateInstance(branchModalEl);
    const itemModal = bootstrap.Modal.getOrCreateInstance(itemModalEl);
    const esc = value => String(value ?? '').replaceAll('&','&amp;').replaceAll('<','&lt;').replaceAll('>','&gt;').replaceAll('"','&quot;').replaceAll("'",'&#39;');
    const restoreParent = () => setTimeout(() => bootstrap.Modal.getOrCreateInstance(byId(c.parentModal)).show(), 100);
    async function branches() {
        const term = byId('priceAdjustmentBranchSearch').value;
        const response = await fetch(`/Configuration/SearchBranches?search=${encodeURIComponent(term)}`);
        const rows = response.ok ? (await response.json()).filter(row => row.isActive !== false) : [];
        byId('priceAdjustmentBranchResults').innerHTML = rows.map((row,i)=>`<tr><td>${i+1}</td><td>${esc(row.name)}</td><td><button class="btn btn-sm btn-outline-primary" type="button" data-id="${row.id}" data-name="${esc(row.name)}">Select</button></td></tr>`).join('') || '<tr><td colspan="3" class="text-center">No branches found.</td></tr>';
    }
    byId(c.branch.open).addEventListener('click', () => { branchModal.show(); branches(); });
    byId('priceAdjustmentBranchSearchButton').addEventListener('click', branches);
    byId('priceAdjustmentBranchResults').addEventListener('click', e => { const b=e.target.closest('button[data-id]'); if(!b)return; const changed=byId(c.branch.id).value!==b.dataset.id; byId(c.branch.id).value=b.dataset.id; byId(c.branch.name).value=b.dataset.name; if(changed){byId(c.item.id).value='';if(c.item.batchId)byId(c.item.batchId).value='';byId(c.item.name).value='';byId(c.item.price).value='';} branchModal.hide(); });
    async function items() {
        const term=byId('priceAdjustmentItemSearch').value; const branchId=byId(c.branch.id).value;
        const url=c.kind==='fuel'?`/Transaction/SearchPriceAdjustmentFuels?search=${encodeURIComponent(term)}`:`/Transaction/SearchProductPriceAdjustmentItems?branchId=${branchId}&term=${encodeURIComponent(term)}`;
        byId('priceAdjustmentItemResults').innerHTML='<tr><td colspan="7" class="text-center text-muted">Loading products...</td></tr>';
        let response;
        try { response=await fetch(url); } catch { byId('priceAdjustmentItemResults').innerHTML='<tr><td colspan="7" class="text-center text-danger">Unable to load products.</td></tr>'; return; }
        if(!response.ok){byId('priceAdjustmentItemResults').innerHTML='<tr><td colspan="7" class="text-center text-danger">Unable to load products.</td></tr>';return;}
        const rows=await response.json();
        byId('priceAdjustmentItemHead').innerHTML=c.kind==='fuel'?'<tr><th>No</th><th>Fuel</th><th>Current Selling Price</th><th>Action</th></tr>':'<tr><th>No</th><th>Product</th><th>Batch No</th><th>Current Selling Price</th><th>Available Quantity</th><th>Status</th><th>Action</th></tr>';
        byId('priceAdjustmentItemResults').innerHTML=rows.map((r,i)=>c.kind==='fuel'?`<tr><td>${i+1}</td><td>${esc(r.name)}</td><td>${Number(r.price).toFixed(2)}</td><td><button class="btn btn-sm btn-outline-primary" type="button" data-id="${r.id}" data-name="${esc(r.name)}" data-price="${r.price}">Select</button></td></tr>`:`<tr><td>${i+1}</td><td>${esc(r.productName)}</td><td>${esc(r.batchNumber)}</td><td>${Number(r.currentSellingPrice).toFixed(2)}</td><td>${Number(r.availableQuantity).toFixed(2)}</td><td>${esc(r.status)}</td><td><button class="btn btn-sm btn-outline-primary" type="button" data-id="${r.productId}" data-batch="${r.batchId}" data-name="${esc(r.productName+' - '+r.batchNumber)}" data-price="${r.currentSellingPrice}">Select</button></td></tr>`).join('')||(c.kind==='fuel'?'<tr><td colspan="4" class="text-center text-muted">No records found.</td></tr>':'<tr><td colspan="7" class="text-center text-muted">No display product batches found for the selected Branch.</td></tr>');
    }
    byId(c.item.open).addEventListener('click',()=>{if(c.kind==='product'&&!byId(c.branch.id).value){alert('Please select a Branch first.');return;}itemModal.show();items();});
    byId('priceAdjustmentItemSearchButton').addEventListener('click',items);
    byId('priceAdjustmentItemResults').addEventListener('click',e=>{const b=e.target.closest('button[data-id]');if(!b)return;byId(c.item.id).value=b.dataset.id;if(c.item.batchId)byId(c.item.batchId).value=b.dataset.batch;byId(c.item.name).value=b.dataset.name;byId(c.item.price).value=Number(b.dataset.price).toFixed(2);itemModal.hide();});
});

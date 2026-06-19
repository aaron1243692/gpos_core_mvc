# GPOS Core MVC Full System Analysis

Generated: 2026-06-19

Project path: `C:\project\gpos_core_mvc`

Scope: source inspection only. No database changes, code fixes, migrations, or refactors were applied while creating this report.

## 1. Project Overview

- Project name: `gpos_core_mvc`
- Framework used: ASP.NET Core MVC, target framework `net10.0`
- Database provider: MySQL through `Pomelo.EntityFrameworkCore.MySql`
- Authentication: ASP.NET Core cookie authentication plus server-side session
- Main purpose: gas station POS/admin system for setup records, products, fuel hierarchy, employee accounts, discounts/members/points, transactions, reports, and Salesman POS access.
- Startup behavior: `Program.cs` calls `db.Database.Migrate()` and seeds a default admin user/role through `DatabaseSeeder`.

Current modules found:

- Admin dashboard
- Setup / Configuration
- Products and inventory setup
- Fuel setup
- Discounts, members, points, and rebate setup
- Config setup: employees, suppliers, position, branch, department, payment methods, shift settings
- Access setup: users, roles, permissions, role permissions, activity logs
- Transaction pages
- Reports pages
- Salesman login and POS pages

Important current state:

- Many setup pages have real CRUD.
- Many transaction/report pages are view-only placeholders.
- `employee_account` is the active employee profile/login table.
- `employees` model file still exists, but `ApplicationDbContext` does not expose a `DbSet<Employee>` in the current source.
- `station_settings` model and DbSet still exist, but Station Settings page/navigation were removed.

## 2. Current Navigation Structure

Navigation is defined in `Views/Shared/Components/Header/Default.cshtml`.

Setup

- Config
  - Employees
  - Suppliers
  - Position
  - Branch
  - Department
  - Payment Methods
  - Shift Settings
- Products
  - Display
  - Warehouse
  - Categories
  - Product Batches
  - Stock Receiving
  - Product Units
  - Low Stock Settings
- Fuel
  - Fuels
  - Tanks
  - Pumps
  - Nozzles
  - Fuel Deliveries
  - Fuel Price History
  - Pump Meter Readings
- Discounts
  - Discounts
  - Members
  - Rebate
  - Points Ledger
  - Discount Rules
- Access
  - Users
  - Roles
  - Permissions
  - Role Permissions
  - Activity Logs

Transaction

- Sales
  - Product
  - Fuel
- Employee
  - POS
  - Product
  - Fuel
  - Daily Cash
- Stock Transfer
  - Warehouse
  - Display
  - Fuel
- Adjustment
  - Warehouse
  - Product
  - Fuel
- Returns
  - Warehouse
  - Product
  - Fuel
- Void
  - Warehouse
  - Product
  - Fuel

Reports

- Sales Reports
  - Daily Sales
  - Fuel Sales
  - Product Sales
- Shift and Cash Reports
  - Shift Report
  - Cash Report
- Inventory Reports
  - Inventory Report
  - Purchase History
- Daily Stock
  - Warehouse
  - Display
  - Tank

Other pages found but not in main header:

- `/`
- `/Home/Privacy`
- `/SignIn`
- `/Salesman/Login`
- `/Salesman/Dashboard`
- `/Salesman/OpenShift`
- `/Salesman/BeginningCash`
- `/Salesman/PumpTransaction`
- `/Salesman/CustomerSearch`
- `/Salesman/MySales`
- `/Salesman/CloseShift`
- Legacy setup controller actions under `/Setup/*`
- Legacy transaction actions under `/Transactions/*`
- Additional report actions not linked in header, such as Payment Method Report, Discount Report, Points Report, Customer Report, Fuel Tank Report, Fuel Batch Report, Item Batch Report, Cash Difference Report, Void Transaction Report, stock transfer/adjustment reports.

## 3. Page Inventory

| Tab | Group | Page | Route | Controller | Action | View File | Status |
| --- | ----- | ---- | ----- | ---------- | ------ | --------- | ------ |
| Home | Public | Home | `/` | Home | Index | `Views/Home/Index.cshtml` | Working |
| Auth | Admin | Sign In | `/SignIn` | SignIn | Index | `Views/SignIn/Index.cshtml` | Working |
| Auth | Employee | Salesman Login | `/Salesman/Login` | SalesmanAuth | Login | `Views/SalesmanAuth/Login.cshtml` | Working |
| Setup | Products | Display | `/Configuration/DisplayProducts` | Configuration | DisplayProducts | `Views/Configuration/DisplayProducts.cshtml` | Partial |
| Setup | Products | Warehouse | `/Configuration/WarehouseProducts` | Configuration | WarehouseProducts | `Views/Configuration/WarehouseProducts.cshtml` | Partial |
| Setup | Products | Categories | `/Configuration/ProductCategories` | Configuration | ProductCategories | `Views/Configuration/ProductCategories.cshtml` | Partial |
| Setup | Products | Product Batches | `/Configuration/ProductBatches` | Configuration | ProductBatches | `Views/Configuration/ProductBatches.cshtml` | Working |
| Setup | Products | Stock Receiving | `/Configuration/StockReceiving` | Configuration | StockReceiving | `Views/Configuration/StockReceiving.cshtml` | Partial |
| Setup | Products | Product Units | `/Configuration/ProductUnits` | Configuration | ProductUnits | `Views/Configuration/ProductUnits.cshtml` | Working |
| Setup | Products | Low Stock Settings | `/Configuration/LowStockSettings` | Configuration | LowStockSettings | `Views/Configuration/LowStockSettings.cshtml` | Working |
| Setup | Fuel | Fuels | `/Configuration/Fuels` | Configuration | Fuels | `Views/Configuration/Fuels.cshtml` | Working |
| Setup | Fuel | Tanks | `/Configuration/Tanks` | Configuration | Tanks | `Views/Configuration/Tanks.cshtml` | Working |
| Setup | Fuel | Pumps | `/Configuration/Pumps` | Configuration | Pumps | `Views/Configuration/Pumps.cshtml` | Working |
| Setup | Fuel | Nozzles | `/Configuration/Nozzles` | Configuration | Nozzles | `Views/Configuration/Nozzles.cshtml` | Working |
| Setup | Fuel | Fuel Deliveries | `/Configuration/FuelDeliveries` | Configuration | FuelDeliveries | `Views/Configuration/FuelDeliveries.cshtml` | Partial |
| Setup | Fuel | Fuel Price History | `/Configuration/FuelPriceHistory` | Configuration | FuelPriceHistory | `Views/Configuration/FuelPriceHistory.cshtml` | Partial |
| Setup | Fuel | Pump Meter Readings | `/Configuration/PumpMeterReadings` | Configuration | PumpMeterReadings | `Views/Configuration/PumpMeterReadings.cshtml` | Working |
| Setup | Discounts | Discounts | `/Configuration/Discounts` | Configuration | Discounts | `Views/Configuration/Discounts.cshtml` | Working |
| Setup | Discounts | Members | `/Configuration/Members` | Configuration | Members | `Views/Configuration/Members.cshtml` | Working |
| Setup | Discounts | Rebate | `/Configuration/Rebate` | Configuration | Rebate | `Views/Configuration/Rebate.cshtml` | Working |
| Setup | Discounts | Points Ledger | `/Configuration/PointsLedger` | Configuration | PointsLedger | `Views/Configuration/PointsLedger.cshtml` | Partial |
| Setup | Discounts | Discount Rules | `/Configuration/DiscountRules` | Configuration | DiscountRules | `Views/Configuration/DiscountRules.cshtml` | Working |
| Setup | Config | Employees | `/Employees` | Employees | Index | `Views/Employees/Index.cshtml` | Working |
| Setup | Config | Suppliers | `/Suppliers` | Suppliers | Index | `Views/Suppliers/Index.cshtml` | Working |
| Setup | Config | Position | `/Configuration/Position` | Configuration | Position | `Views/Configuration/Position.cshtml` | Working |
| Setup | Config | Branch | `/Configuration/Branch` | Configuration | Branch | `Views/Configuration/Branch.cshtml` | Working |
| Setup | Config | Department | `/Configuration/Department` | Configuration | Department | `Views/Configuration/Department.cshtml` | Working |
| Setup | Config | Payment Methods | `/Configuration/PaymentMethods` | Configuration | PaymentMethods | `Views/Configuration/PaymentMethods.cshtml` | Working |
| Setup | Config | Shift Settings | `/Configuration/ShiftSettings` | Configuration | ShiftSettings | `Views/Configuration/ShiftSettings.cshtml` | Working |
| Setup | Config | Shift Schedule | `/Shift/Schedule` | Shift | Schedule | `Views/Shift/Schedule.cshtml` | UI only |
| Setup | Access | Users | `/Users` | Users | Index | `Views/Users/Index.cshtml` | UI only |
| Setup | Access | Roles | `/Roles` | Roles | Index | `Views/Roles/Index.cshtml` | UI only |
| Setup | Access | Permissions | `/Permissions` | Permissions | Index | `Views/Permissions/Index.cshtml` | UI only |
| Setup | Access | Role Permissions | `/Configuration/RolePermissions` | Configuration | RolePermissions | `Views/Configuration/RolePermissions.cshtml` | Partial |
| Setup | Access | Activity Logs | `/Configuration/ActivityLogs` | Configuration | ActivityLogs | `Views/Configuration/ActivityLogs.cshtml` | Partial |
| Transaction | Sales | Product | `/Transaction/ProductSales` | Transaction | ProductSales | `Views/Transaction/ProductSales.cshtml` | UI only |
| Transaction | Sales | Fuel | `/Transaction/FuelSales` | Transaction | FuelSales | `Views/Transaction/FuelSales.cshtml` | UI only |
| Transaction | Employee | POS | `/Salesman/POS` | Salesman | POS | `Views/Salesman/POS.cshtml` | UI only |
| Transaction | Employee | Product | `/Transaction/EmployeeProductSales` | Transaction | EmployeeProductSales | `Views/Transaction/EmployeeProductSales.cshtml` | UI only |
| Transaction | Employee | Fuel | `/Transaction/EmployeeFuelSales` | Transaction | EmployeeFuelSales | `Views/Transaction/EmployeeFuelSales.cshtml` | UI only |
| Transaction | Employee | Daily Cash | `/Transaction/DailyCash` | Transaction | DailyCash | `Views/Transaction/DailyCash.cshtml` | UI only |
| Transaction | Stock Transfer | Warehouse | `/Transaction/WarehouseTransfer` | Transaction | WarehouseTransfer | `Views/Transaction/WarehouseTransfer.cshtml` | UI only |
| Transaction | Stock Transfer | Display | `/Transaction/DisplayTransfer` | Transaction | DisplayTransfer | `Views/Transaction/DisplayTransfer.cshtml` | UI only |
| Transaction | Stock Transfer | Fuel | `/Transaction/FuelTransfer` | Transaction | FuelTransfer | `Views/Transaction/FuelTransfer.cshtml` | UI only |
| Transaction | Adjustment | Warehouse | `/Transaction/WarehouseAdjustment` | Transaction | WarehouseAdjustment | `Views/Transaction/WarehouseAdjustment.cshtml` | UI only |
| Transaction | Adjustment | Product | `/Transaction/ProductAdjustment` | Transaction | ProductAdjustment | `Views/Transaction/ProductAdjustment.cshtml` | UI only |
| Transaction | Adjustment | Fuel | `/Transaction/FuelAdjustment` | Transaction | FuelAdjustment | `Views/Transaction/FuelAdjustment.cshtml` | UI only |
| Transaction | Returns | Warehouse | `/Transaction/WarehouseReturn` | Transaction | WarehouseReturn | `Views/Transaction/WarehouseReturn.cshtml` | UI only |
| Transaction | Returns | Product | `/Transaction/ProductReturn` | Transaction | ProductReturn | `Views/Transaction/ProductReturn.cshtml` | UI only |
| Transaction | Returns | Fuel | `/Transaction/FuelReturn` | Transaction | FuelReturn | `Views/Transaction/FuelReturn.cshtml` | UI only |
| Transaction | Void | Warehouse | `/Transaction/WarehouseVoid` | Transaction | WarehouseVoid | `Views/Transaction/WarehouseVoid.cshtml` | UI only |
| Transaction | Void | Product | `/Transaction/ProductVoid` | Transaction | ProductVoid | `Views/Transaction/ProductVoid.cshtml` | UI only |
| Transaction | Void | Fuel | `/Transaction/FuelVoid` | Transaction | FuelVoid | `Views/Transaction/FuelVoid.cshtml` | UI only |
| Reports | Sales Reports | Daily Sales | `/Reports/DailySales` | Reports | DailySales | `Views/Reports/DailySales.cshtml` | UI only |
| Reports | Sales Reports | Fuel Sales | `/Reports/FuelSales` | Reports | FuelSales | `Views/Reports/FuelSales.cshtml` | UI only |
| Reports | Sales Reports | Product Sales | `/Reports/ProductSales` | Reports | ProductSales | `Views/Reports/ProductSales.cshtml` | UI only |
| Reports | Shift and Cash | Shift Report | `/Reports/ShiftReport` | Reports | ShiftReport | `Views/Reports/ShiftReport.cshtml` | UI only |
| Reports | Shift and Cash | Cash Report | `/Reports/CashReport` | Reports | CashReport | `Views/Reports/CashReport.cshtml` | UI only |
| Reports | Inventory | Inventory Report | `/Reports/InventoryReport` | Reports | InventoryReport | `Views/Reports/InventoryReport.cshtml` | UI only |
| Reports | Inventory | Purchase History | `/Reports/PurchaseHistory` | Reports | PurchaseHistory | `Views/Reports/PurchaseHistory.cshtml` | UI only |
| Reports | Daily Stock | Warehouse | `/Reports/WarehouseDailyStock` | Reports | WarehouseDailyStock | `Views/Reports/WarehouseDailyStock.cshtml` | UI only |
| Reports | Daily Stock | Display | `/Reports/DisplayDailyStock` | Reports | DisplayDailyStock | `Views/Reports/DisplayDailyStock.cshtml` | UI only |
| Reports | Daily Stock | Tank | `/Reports/TankDailyStock` | Reports | TankDailyStock | `Views/Reports/TankDailyStock.cshtml` | UI only |

## 4. Database Tables

Tables found from `ApplicationDbContext`, models, and migrations:

| Table | Purpose | Key Columns | PK | FKs / Unique Indexes | Status / Soft Delete |
| ----- | ------- | ----------- | -- | -------------------- | -------------------- |
| `users` | Admin/user login accounts | `id int`, `username string`, `email string`, `password_hash string`, `status int`, timestamps | `id` | unique `username`, unique `email` | `status`; user login rejects disabled users |
| `roles` | Admin roles | `id`, `name`, `code`, `status`, timestamps | `id` | unique `code` | `status` |
| `permissions` | Permission catalog | `id`, `name`, `code`, `parent_id`, `status`, timestamps | `id` | self FK `parent_id`, unique `code` | `status` |
| `user_roles` | User-role assignment | `id`, `user_id`, `role_id`, timestamps | `id` | `users.id`, `roles.id`, unique `(user_id, role_id)` | No status found |
| `role_permissions` | Role-permission assignment | `id`, `role_id`, `permission_id`, `status`, timestamps | `id` | `roles.id`, `permissions.id`, unique `(role_id, permission_id)` | `status`; save disables/re-enables assignments |
| `suppliers` | Product/fuel suppliers | `id`, `name`, `email`, `contact_person`, `contact_number`, `address`, `status`, timestamps | `id` | referenced by fuels, batches, stock receiving, fuel deliveries | `status` |
| `fuels` | Fuel products | `id`, `name`, `code`, `supplier_id`, `current_price_per_liter decimal(18,2)`, `is_active`, `status`, timestamps | `id` | `suppliers.id` nullable | `status`, `is_active` |
| `tanks` | Fuel tanks | `id`, `fuel_id`, `tank_no`, `capacity_liters decimal(18,2)`, `current_liters decimal(18,2)`, `is_active`, `status`, timestamps | `id` | `fuels.id` | `status`, `is_active` |
| `pumps` | Pump records under tanks | `id`, `tank_id`, `pump_no`, `name`, `status`, timestamps | `id` | `tanks.id` | `status` |
| `nozzles` | Pump nozzles | `id`, `pump_id`, `nozzle_no`, `status`, timestamps | `id` | `pumps.id` | `status` |
| `fuel_deliveries` | Tank fuel delivery records | `id`, `delivery_no`, `supplier_id`, `fuel_id`, `tank_id`, `delivered_liters`, `cost_per_liter`, `total_cost`, `delivery_date`, `remarks`, `status`, timestamps | `id` | unique `delivery_no`; FKs to supplier/fuel/tank | `status` |
| `fuel_price_history` | Fuel price changes | `id`, `fuel_id`, `old_price`, `new_price`, `effective_at`, `remarks`, `created_by`, `status`, timestamps | `id` | `fuels.id` | `status` |
| `pump_meter_readings` | Meter readings by nozzle | `id`, `pump_id` nullable legacy, `nozzle_id` nullable in model for old data, `shift_id`, `opening_meter`, `closing_meter`, `liters_sold`, `reading_date`, `status`, timestamps | `id` | FKs to `pumps.id`, `nozzles.id` | `status` |
| `product_categories` | Product categories | `id`, `name`, `description`, `is_active`, `status`, timestamps | `id` | products reference it | `status`, `is_active` |
| `product_units` | Product units | `id`, `name`, `abbreviation`, `status`, timestamps | `id` | products reference it | `status` |
| `products` | Product master | `id`, `category_id`, `product_unit_id`, `name`, `is_active`, `status`, timestamps | `id` | `product_categories.id`, `product_units.id` | `status`, `is_active` |
| `product_batches` | Product batch/pricing/supplier | `id`, `product_id`, `supplier_id`, `batch_no`, `cost_price`, `selling_price`, `expiry_date`, `status`, `is_active`, timestamps | `id` | unique `batch_no`; FKs to product/supplier | `status`, `is_active` |
| `stock_receivings` | Receiving header | `id`, `receiving_no`, `supplier_id`, `received_date`, `total_amount`, `remarks`, `status`, timestamps | `id` | unique `receiving_no`; `suppliers.id` nullable | `status` |
| `stock_receiving_items` | Receiving line item | `id`, `stock_receiving_id`, `product_id`, `product_batch_id`, `quantity`, `cost_price`, `selling_price`, `expiry_date`, `subtotal`, timestamps | `id` | receiving/product/batch FKs | No status found |
| `stock_movements` | Product movement ledger | `id`, `product_id`, `product_batch_id`, `source_location`, `destination_location`, `movement_type`, `quantity`, `reference_type`, `reference_id`, `remarks`, `created_by`, `created_at` | `id` | product/batch FKs | No status found |
| `warehouse_stocks` | Warehouse quantity by product/batch | `id`, `product_id`, `batch_id`, `quantity`, timestamps | `id` | product/batch FKs; check quantity >= 0 | No status found |
| `display_stocks` | Display quantity by product/batch | `id`, `product_id`, `batch_id`, `quantity`, timestamps | `id` | product/batch FKs; check quantity >= 0 | No status found |
| `low_stock_settings` | Minimum quantity settings | `id`, `product_id`, `product_batch_id`, `location`, `minimum_quantity`, `status`, timestamps | `id` | product/batch FKs | `status` |
| `discounts` | Member earn rates | `id`, `name`, `earn_rate`, `status`, timestamps | `id` | members reference it | `status` |
| `members` | Customer/member accounts | `id`, `member_no`, `card_no`, `full_name`, contacts, `discount_id`, `points`, `status`, timestamps | `id` | unique `member_no`, unique `card_no`, `discounts.id` nullable | `status` |
| `rebate_rules` | Points redemption/rebate rule | `id`, `name`, `points_required`, `rebate_value`, `status`, timestamps | `id` | None found | `status` |
| `points_ledger` | Points ledger | `id`, `member_id`, `transaction_type`, `points`, `reference_type`, `reference_id`, `remarks`, `created_at` | `id` | `members.id` | No status found |
| `discount_rules` | Discount calculation rules | `id`, `name`, `discount_type`, `discount_value`, `applies_to`, `status`, timestamps | `id` | None found | `status` |
| `branches` | Branch setup | `id`, `name`, `address`, `status`, timestamps | `id` | departments reference it | `status` |
| `departments` | Department under branch | `id`, `branch_id`, `name`, `description`, `status`, timestamps | `id` | `branches.id` | `status` |
| `positions` | Employee positions | `id`, `name`, `description`, `status`, timestamps | `id` | employee accounts reference it | `status` |
| `employee_account` | Employee profile/login | `id`, `username`, `password_hash`, `full_name`, contacts, `department_id`, `position_id`, nullable `role`, `status`, timestamps | `id` | unique `username`, FKs to department/position | `status`; employee login rejects disabled account |
| `station_settings` | Station identity/settings | station name, business name, address, TIN, receipt header/footer, default branch, currency, tax settings, timestamps | `id` | `branches.id` nullable | No status found; page removed |
| `payment_methods` | Payment method setup | `id`, `name`, `code`, `status`, timestamps | `id` | unique `code` | `status` |
| `shift_settings` | Shift template/settings | `id`, `name`, `start_time`, `end_time`, opening/cash flags, approval flag, `status`, timestamps | `id` | None found | `status` |
| `activity_logs` | Activity log | `id`, `user_id`, `username`, `action`, `module`, `description`, `ip_address`, `created_at` | `id` | `users.id` nullable | No status found |

Suggested/requested tables not found in current `ApplicationDbContext`:

- `employee_shift_schedules`: Not found
- `shifts`: Not found
- `sales`: Not found
- `sale_items`: Not found
- `payments`: Not found
- `cash_movements`: Not found
- `returns`: Not found
- `voids`: Not found
- `adjustments`: Not found

## 5. Database Relationships

Relationships found:

- `roles.id -> user_roles.role_id`
- `users.id -> user_roles.user_id`
- `roles.id -> role_permissions.role_id`
- `permissions.id -> role_permissions.permission_id`
- `permissions.id -> permissions.parent_id`
- `suppliers.id -> fuels.supplier_id`
- `fuels.id -> tanks.fuel_id`
- `tanks.id -> pumps.tank_id`
- `pumps.id -> nozzles.pump_id`
- `pumps.id -> pump_meter_readings.pump_id` (legacy/compatibility)
- `nozzles.id -> pump_meter_readings.nozzle_id`
- `suppliers.id -> fuel_deliveries.supplier_id`
- `fuels.id -> fuel_deliveries.fuel_id`
- `tanks.id -> fuel_deliveries.tank_id`
- `fuels.id -> fuel_price_history.fuel_id`
- `product_categories.id -> products.category_id`
- `product_units.id -> products.product_unit_id`
- `products.id -> product_batches.product_id`
- `suppliers.id -> product_batches.supplier_id`
- `suppliers.id -> stock_receivings.supplier_id`
- `stock_receivings.id -> stock_receiving_items.stock_receiving_id`
- `products.id -> stock_receiving_items.product_id`
- `product_batches.id -> stock_receiving_items.product_batch_id`
- `products.id -> warehouse_stocks.product_id`
- `product_batches.id -> warehouse_stocks.batch_id`
- `products.id -> display_stocks.product_id`
- `product_batches.id -> display_stocks.batch_id`
- `products.id -> stock_movements.product_id`
- `product_batches.id -> stock_movements.product_batch_id`
- `products.id -> low_stock_settings.product_id`
- `product_batches.id -> low_stock_settings.product_batch_id`
- `discounts.id -> members.discount_id`
- `members.id -> points_ledger.member_id`
- `branches.id -> departments.branch_id`
- `departments.id -> employee_account.department_id`
- `positions.id -> employee_account.position_id`
- `branches.id -> station_settings.default_branch_id`
- `users.id -> activity_logs.user_id`

Simple relationship form:

- User > User Role > Role
- Role > Role Permission > Permission
- Branch > Department > Employee Account
- Position > Employee Account
- Supplier > Fuel
- Fuel > Tank > Pump > Nozzle > Pump Meter Reading
- Fuel > Fuel Price History
- Supplier > Fuel Delivery
- Fuel > Fuel Delivery
- Tank > Fuel Delivery
- Product Category > Product > Product Batch
- Product Unit > Product
- Supplier > Product Batch
- Supplier > Stock Receiving > Stock Receiving Item
- Product > Warehouse Stock / Display Stock / Stock Movement / Low Stock Setting
- Product Batch > Warehouse Stock / Display Stock / Stock Movement / Low Stock Setting
- Discount > Member > Points Ledger

Missing or incomplete relationships:

- Employee shift schedule relationship: Missing
- Sales/payment/cash transaction relationships: Missing
- Returns/voids/adjustments transaction relationships: Missing
- Shift to pump meter readings via `shift_id`: column exists, relationship/model not found.
- Product sale to stock movement: Missing.
- Fuel sale to tank/nozzle/meter reading: Missing.

## 6. Setup Module Analysis

### Products

Display

- Purpose: create/display display-stock records tied to product and batch.
- Route: `/Configuration/DisplayProducts`
- Tables used: `products`, `product_batches`, `display_stocks`, `product_categories`
- Fields shown: batch, product name, category, cost price, selling price, quantity
- Add/Edit/Delete behavior: Add only; Edit button disabled; no Delete found.
- Search behavior: product, category, batch search.
- Status: Partial
- Problems found: add logic creates product, batch, and display stock in one flow; no edit/delete/adjustment workflow; no stock movement record created.
- Recommended improvements: separate product master from stock quantity changes, create stock movement ledger entries, add edit/audit paths.

Warehouse

- Purpose: create/display warehouse-stock records.
- Route: `/Configuration/WarehouseProducts`
- Tables used: `products`, `product_batches`, `warehouse_stocks`, `product_categories`
- Fields shown: batch, product name, category, prices, quantity
- Add/Edit/Delete behavior: Add only; Edit disabled; no Delete found.
- Search behavior: product, category, batch search.
- Status: Partial
- Problems found: same as Display; no receiving/transfer integration.
- Recommended improvements: make warehouse stock derived from receiving and movements.

Categories

- Purpose: product categories.
- Route: `/Configuration/ProductCategories`
- Table used: `product_categories`
- Fields shown: id, name, description, status.
- Add/Edit/Delete behavior: Add and soft delete found; Edit button is disabled.
- Search behavior: name/description search.
- Status: Partial
- Problems found: no edit workflow despite status/delete; uses both `is_active` and `status`.
- Recommended improvements: add edit modal, consolidate active/status rules.

Product Batches

- Purpose: product batches with supplier, prices, expiry, status.
- Route: `/Configuration/ProductBatches`
- Tables used: `product_batches`, `products`, `suppliers`
- Fields shown: batch no, product, supplier, prices, expiry, status.
- Add/Edit/Delete behavior: add/edit/soft delete.
- Search behavior: batch/product/supplier search.
- Status: Working
- Problems found: duplicate batch number check exists; stock quantity is elsewhere, so batch inventory needs clearer integration.
- Recommended improvements: connect receiving items to batches consistently.

Stock Receiving

- Purpose: receiving header and one line item.
- Route: `/Configuration/StockReceiving`
- Tables used: `stock_receivings`, `stock_receiving_items`, `products`, `suppliers`
- Fields shown: receiving no, supplier, received date, total, status.
- Add/Edit/Delete behavior: add/edit header; soft delete header.
- Search behavior: receiving no/supplier search.
- Status: Partial
- Problems found: form appears to support one product item only; product stock quantities and stock movement ledger are not fully updated in current inspected logic.
- Recommended improvements: multi-line receiving, batch creation/update, warehouse stock updates, stock movement entries.

Product Units

- Purpose: unit master records.
- Route: `/Configuration/ProductUnits`
- Table used: `product_units`
- Fields shown: name, abbreviation, status.
- Add/Edit/Delete behavior: add/edit/soft delete.
- Search behavior: name/abbreviation search.
- Status: Working
- Problems found: products dropdown should always hide disabled units; confirm in future UI.
- Recommended improvements: use units in product forms consistently.

Low Stock Settings

- Purpose: minimum quantity per product/batch/location.
- Route: `/Configuration/LowStockSettings`
- Tables used: `low_stock_settings`, `products`, `product_batches`
- Fields shown: product, batch, location, minimum quantity, status.
- Add/Edit/Delete behavior: add/edit/soft delete.
- Search behavior: location/product/batch search.
- Status: Working
- Problems found: no alert/report logic found.
- Recommended improvements: add report/dashboard warning and location-specific stock calculation.

### Fuel

Fuels

- Purpose: fuel master with current price.
- Route: `/Configuration/Fuels`
- Tables used: `fuels`, `suppliers`
- Fields shown: code, name, supplier, current price, status.
- Add/Edit/Delete behavior: add/edit/soft delete.
- Search behavior: name/code/supplier search.
- Status: Working
- Problems found: current price and price history can diverge if edits are made directly.
- Recommended improvements: enforce price changes through price history or create audit log.

Tanks

- Purpose: tanks under fuel.
- Route: `/Configuration/Tanks`
- Tables used: `tanks`, `fuels`
- Fields shown: tank no, fuel, capacity liters, current liters, status.
- Add/Edit/Delete behavior: add/edit/soft delete.
- Search behavior: tank/fuel search.
- Status: Working
- Problems found: current liters updated on deliveries but reconciliation/sales drawdown not found.
- Recommended improvements: connect fuel sales/meter readings to tank inventory.

Pumps

- Purpose: pumps under tanks.
- Route: `/Configuration/Pumps`
- Tables used: `pumps`, `tanks`, `fuels`
- Fields shown: pump, tank, fuel, status.
- Add/Edit/Delete behavior: add/edit/soft delete.
- Search behavior: pump/tank/fuel search.
- Status: Working
- Problems found: pump name and pump no are both set from form name.
- Recommended improvements: separate Pump No from display name if business requires both.

Nozzles

- Purpose: nozzles under pumps.
- Route: `/Configuration/Nozzles`
- Tables used: `nozzles`, `pumps`, `tanks`, `fuels`
- Fields shown: nozzle no, pump, tank, fuel, status.
- Add/Edit/Delete behavior: add/edit/soft delete.
- Search behavior: nozzle/pump search.
- Status: Working
- Problems found: no nozzle calibration/meter baseline found.
- Recommended improvements: add initial meter and last meter validation per nozzle.

Fuel Deliveries

- Purpose: record fuel delivered into tanks.
- Route: `/Configuration/FuelDeliveries`
- Tables used: `fuel_deliveries`, `suppliers`, `fuels`, `tanks`
- Fields shown: delivery no, supplier, fuel, tank, liters, date, status.
- Add/Edit/Delete behavior: add/edit/soft delete.
- Search behavior: delivery/supplier/fuel/tank search.
- Status: Partial
- Problems found: add increments `tank.current_liters`; edit does not appear to reverse/recalculate prior delivery quantity. Soft delete does not reverse tank liters.
- Recommended improvements: ledger-based tank movements and recalculation.

Fuel Price History

- Purpose: record fuel price change and update current price.
- Route: `/Configuration/FuelPriceHistory`
- Tables used: `fuel_price_history`, `fuels`
- Fields shown: fuel, old/new price, effective date, status.
- Add/Edit/Delete behavior: add and soft delete; edit not found.
- Search behavior: fuel search.
- Status: Partial
- Problems found: no future-effective pricing logic found.
- Recommended improvements: price effective date handling, audit trail.

Pump Meter Readings

- Purpose: nozzle meter readings.
- Route: `/Configuration/PumpMeterReadings`
- Tables used: `pump_meter_readings`, `nozzles`, `pumps`, `tanks`, `fuels`
- Fields shown: nozzle, pump, tank, fuel, opening, closing, liters sold, date, status.
- Add/Edit/Delete behavior: add/edit backend exists; table action buttons removed by UI request; soft delete backend remains.
- Search behavior: nozzle/pump/tank/fuel search.
- Status: Working
- Problems found: `pump_id` remains as nullable legacy data; `shift_id` relationship not implemented.
- Recommended improvements: enforce per-nozzle sequential meter validation and integrate with fuel sales/tank drawdown.

### Discounts

Discounts

- Purpose: earn-rate setup.
- Route: `/Configuration/Discounts`
- Table used: `discounts`
- Status: Working
- Problems found: earn logic not integrated with sales in current inspected code.

Members

- Purpose: member/customer profile with points.
- Route: `/Configuration/Members`
- Tables used: `members`, `discounts`
- Status: Working
- Problems found: points balance exists, but sale integration not found.

Rebate

- Purpose: rebate rules based on points.
- Route: `/Configuration/Rebate`
- Table used: `rebate_rules`
- Status: Working
- Problems found: no redemption transaction logic found.

Points Ledger

- Purpose: manual points ledger entry.
- Route: `/Configuration/PointsLedger`
- Tables used: `points_ledger`, `members`
- Status: Partial
- Problems found: save updates `members.points`; no balancing constraints or sale references enforced.

Discount Rules

- Purpose: discount rule master.
- Route: `/Configuration/DiscountRules`
- Table used: `discount_rules`
- Status: Working
- Problems found: no transaction calculation integration found.

### Config

Employees

- Purpose: employee account profile/login setup.
- Route: `/Employees`
- Table used: `employee_account`
- Status: Working
- Problems found: no shift schedule restriction found.

Suppliers

- Purpose: supplier master.
- Route: `/Suppliers`
- Table used: `suppliers`
- Status: Working

Position / Branch / Department

- Purpose: organizational setup.
- Routes: `/Configuration/Position`, `/Configuration/Branch`, `/Configuration/Department`
- Tables: `positions`, `branches`, `departments`
- Status: Working

Payment Methods

- Purpose: payment method setup.
- Route: `/Configuration/PaymentMethods`
- Table: `payment_methods`
- Status: Working

Shift Settings

- Purpose: shift template flags/times.
- Route: `/Configuration/ShiftSettings`
- Table: `shift_settings`
- Status: Working
- Problems found: no shift transaction table found.

Shift Schedule

- Purpose: employee scheduling.
- Route: `/Shift/Schedule`
- Table: Not found
- Status: UI only
- Problems found: no controller CRUD, table, or employee login restriction found.

### Access

Users / Roles / Permissions

- Routes: `/Users`, `/Roles`, `/Permissions`
- Tables: `users`, `roles`, `permissions`
- Status: UI only
- Problems found: no CRUD controllers beyond `Index`.

Role Permissions

- Route: `/Configuration/RolePermissions`
- Tables: `role_permissions`, `roles`, `permissions`
- Status: Partial
- Problems found: assignments are saved, but permission enforcement across pages is not found.

Activity Logs

- Route: `/Configuration/ActivityLogs`
- Table: `activity_logs`
- Status: Partial
- Problems found: log table exists; widespread writes to activity log were not found.

## 7. Employee Account Analysis

- System uses `employee_account` for employee profile/login setup.
- `employees` model still exists, but `ApplicationDbContext` currently uses `EmployeeAccounts`, not `Employees`.
- Login fields in `employee_account`: username, password hash, full name, status.
- Profile fields: email, contact number, address.
- Department: `employee_account.department_id`.
- Branch: derived through `employee_account.Department.Branch`.
- Position: `employee_account.position_id`.
- Status: `employee_account.status`; employee login rejects non-active accounts.
- Password reset: implemented in `EmployeesController.ResetPassword`.
- Role: `employee_account.role` exists nullable but Employees page does not display/use it.
- Shift schedule login restriction: Not found.
- Salesman login uses `EmployeeAuthService.ValidateSalesmanAsync`; it currently validates username/password/status only.
- Salesman session stores EmployeeId, EmployeeName, EmployeeUsername.
- Potential issue: `BlockSalesmanSessionAttribute` still checks `EmployeeRole == salesman`, but Salesman login no longer sets `EmployeeRole`.

## 8. Product and Inventory Analysis

Current design:

- `product_categories` > `products` > `product_batches`
- `product_units` > `products`
- `warehouse_stocks` and `display_stocks` store quantity by product/batch.
- `stock_receivings` and `stock_receiving_items` store receiving data.
- `stock_movements` exists as a ledger table but integration appears incomplete.
- `low_stock_settings` stores thresholds by product/batch/location.

Problems:

- Display/Warehouse product pages directly add stock rows, not clear inventory movement transactions.
- Stock Receiving form appears single-item oriented in current controller logic.
- Stock movement history table exists, but creation from receiving/transfers/sales was not found.
- Transaction pages for stock transfer/adjustment/return/void are UI-only.
- Reports are UI-only and do not compute inventory from data.
- Some product records use both `is_active` and `status`.

Recommended improvements:

- Make stock quantities derived from movements or carefully maintained through services.
- Implement multi-line stock receiving.
- Create stock movement entries for receiving, transfer, adjustment, sale, return, and void.
- Add transactional consistency for stock changes.
- Add inventory reports that calculate by location, product, and batch.

## 9. Fuel System Analysis

Current relationship:

- Fuel > Tank > Pump > Nozzle > Pump Meter Reading
- Supplier > Fuel
- Supplier/Fuel/Tank > Fuel Delivery
- Fuel > Fuel Price History

Current tank fields:

- `capacity_liters`
- `current_liters`

Pump Meter Reading:

- Page now uses Nozzle as main input.
- Fuel is derived through `Nozzle > Pump > Tank > Fuel`.
- `pump_id` remains nullable legacy/backfill data.

Problems:

- Fuel deliveries increment `tank.current_liters` on add, but edit/delete reversal logic is not complete.
- Fuel sales implementation not found.
- No reconciliation between meter readings, fuel sales, and tank inventory found.
- No per-nozzle sequential meter validation found.
- `shift_id` exists on readings but no shift relationship/model found.

Recommended improvements:

- Add tank movement ledger.
- Link fuel sales to nozzle/tank and decrement tank liters.
- Validate closing meter >= previous closing meter for same nozzle.
- Connect meter readings to shifts.
- Add tank variance/reconciliation reports.

## 10. Discount, Members, Points, and Rebate Analysis

- `discounts` define earn rate.
- `members` store points balance.
- `points_ledger` records manual point changes and updates member balance.
- `rebate_rules` define points required and rebate value.
- `discount_rules` define discount type/value/applicability.

Problems:

- Sales integration for earning points: Not found.
- Sales integration for redeeming points: Not found.
- Discount rule calculation in transactions: Not found.
- Points ledger is not tied to enforced sale/payment tables because sale/payment tables are not found.

Recommended improvements:

- Implement points earning on completed sales.
- Implement rebate redemption with ledger entries.
- Tie discount rules to product/fuel/member/payment contexts.
- Add reports for points earned/redeemed/expired.

## 11. POS and Transaction Analysis

Salesman POS:

- Route: `/Salesman/POS`
- Controller: `SalesmanController.POS`
- Current status: UI only
- Tables used: Not found from controller
- Missing backend logic: cart, product/fuel lookup, sale creation, payment, receipt, stock/tank update.

Product sale:

- Route: `/Transaction/ProductSales`
- Status: UI only
- Missing backend: sale header/items/payments/stock movement.

Fuel sale:

- Route: `/Transaction/FuelSales`
- Status: UI only
- Missing backend: nozzle/tank/fuel selection, liters/amount calculation, tank decrement, meter integration.

Daily cash:

- Route: `/Transaction/DailyCash`
- Status: UI only
- Missing backend: cash movement table not found.

Stock transfer:

- Routes: Warehouse, Display, Fuel transfer under `/Transaction/*Transfer`
- Status: UI only
- Missing backend: stock/tank transfer tables and movements.

Returns:

- Routes: Warehouse/Product/Fuel return
- Status: UI only
- Missing backend: returns table not found.

Void:

- Routes: Warehouse/Product/Fuel void
- Status: UI only
- Missing backend: voids table not found.

Adjustment:

- Routes: Warehouse/Product/Fuel adjustment
- Status: UI only
- Missing backend: adjustments table not found.

Open shift / Close shift:

- Legacy `/Transactions/OpenShift`, `/Transactions/CloseShift` views exist.
- `shift_settings` exists, but `shifts` table not found.
- Status: UI only / Partial settings only.

## 12. Reports Analysis

All inspected report controller actions return views directly. No report data queries were found in `ReportsController`.

| Report | Route | Data Source Tables | Current Status | Missing Filters/Computations |
| ------ | ----- | ------------------ | -------------- | ---------------------------- |
| Daily Sales | `/Reports/DailySales` | sales tables not found | UI only | date, branch, cashier, totals |
| Fuel Sales | `/Reports/FuelSales` | sales/fuel sale tables not found | UI only | fuel/nozzle/tank totals |
| Product Sales | `/Reports/ProductSales` | sales/sale item tables not found | UI only | product/batch/category totals |
| Shift Report | `/Reports/ShiftReport` | shifts table not found | UI only | shift totals, cashier totals |
| Cash Report | `/Reports/CashReport` | cash movements/payments not found | UI only | cash in/out, variance |
| Inventory Report | `/Reports/InventoryReport` | stock tables exist | UI only | stock valuation, filters |
| Warehouse Daily Stock | `/Reports/WarehouseDailyStock` | `warehouse_stocks` | UI only | daily stock computation |
| Display Daily Stock | `/Reports/DisplayDailyStock` | `display_stocks` | UI only | daily stock computation |
| Tank Daily Stock | `/Reports/TankDailyStock` | `tanks`, fuel movement needed | UI only | opening, delivery, sales, variance |
| Purchase History | `/Reports/PurchaseHistory` | `stock_receivings`, `fuel_deliveries` | UI only | date/supplier/product/fuel filters |
| Activity Logs | `/Configuration/ActivityLogs` | `activity_logs` | Partial | log writes missing broadly |

## 13. Authentication and Authorization Analysis

Admin login:

- `/SignIn`
- Uses `UserAuthService`.
- Validates username/email, password hash, and `users.status`.
- Sets session keys `UserId`, `Name`, `Username`, `Role`.
- Adds cookie claims including role.

Employee/Salesman login:

- `/Salesman/Login`
- Uses `EmployeeAuthService`.
- Validates username, password hash, and status.
- Does not enforce position, role, permission, or shift schedule.

Page protection:

- Many admin/setup controllers have `[BlockSalesmanSession]`.
- `BlockSalesmanSessionAttribute` requires session `Role == admin`.
- No general `[Authorize]` attribute was found on controllers inspected.
- Permission-level enforcement not found.

Roles and permissions:

- Tables and role permission assignment exist.
- Only role code appears to be used for admin gate.
- Permission enforcement per page/action: Not found.

Security gaps:

- Default admin password is seeded as `admin123`.
- Connection string with root password exists in `appsettings.json`.
- Admin authorization is session-role based, not policy/permission based.
- Salesman session does not use cookie auth claims.
- Shift schedule restrictions not found.
- Activity logging not consistently implemented.

## 14. Soft Delete Analysis

Tables using `status = 0` soft delete:

- `users`
- `roles`
- `permissions`
- `role_permissions`
- `suppliers`
- `fuels`
- `tanks`
- `pumps`
- `nozzles`
- `fuel_deliveries`
- `fuel_price_history`
- `pump_meter_readings`
- `products`
- `product_categories`
- `product_units`
- `product_batches`
- `stock_receivings`
- `low_stock_settings`
- `discounts`
- `members`
- `rebate_rules`
- `discount_rules`
- `branches`
- `departments`
- `positions`
- `employee_account`
- `payment_methods`
- `shift_settings`

No status column found:

- `user_roles`
- `stock_receiving_items`
- `stock_movements`
- `warehouse_stocks`
- `display_stocks`
- `points_ledger`
- `station_settings`
- `activity_logs`

Hard delete scan:

- No `.Remove(...)`, `RemoveRange(...)`, or `ExecuteDelete` calls found in controllers/services/data after the current soft-delete updates.

Dropdown behavior:

- Many setup dropdowns filter active records.
- Existing historical display uses relationships, so disabled names can still display when included via navigation properties.

Risk:

- Some pages still use both `is_active` and `status`; drift is possible.

## 15. UI/UX Analysis

Header dropdown:

- Green ribbon/dropdown theme.
- Navigation is grouped and mostly consistent.
- Station Settings is not shown.
- Shift Schedule is not shown in current Config group despite requested final structure mentioning it.

Page layout:

- Setup pages use content cards, search/add toolbars, tables, and Bootstrap modals.
- Some views are compressed one-line Razor/HTML, harder to maintain.

Table layout:

- Product Batches was aligned to Display Products style.
- Pump Meter Readings table now has no Action column.
- Several setup pages use mixed status text vs badges; newer pages use badges.

Action buttons:

- Most setup CRUD pages have Edit/Delete actions.
- Pump Meter Readings intentionally has no table actions.
- Display/Warehouse have disabled Edit buttons.
- Product Categories has disabled Edit button but active Delete button.

Modal design:

- Mostly Bootstrap modal forms.
- Some pages lack edit modal behavior.

Search/Add toolbar:

- Common pattern exists.
- Search coverage varies by page.

POS UI:

- Salesman POS views exist, backend not found.

Consistency issues:

- Mixed `Active/Inactive` vs `Active/Disabled`.
- Mixed `IsActive` and `Status`.
- One-line views reduce maintainability.
- Some pages are UI-only placeholders while navigation presents them as complete.

## 16. Missing Features

Setup:

- Full Users/Roles/Permissions CRUD.
- Shift Schedule CRUD and database table.
- Station Settings page intentionally removed, but table remains.

Product Inventory:

- Multi-line receiving.
- Stock movement ledger integration.
- Warehouse/display transfer backend.
- Product sale stock decrement.
- Product return/void/adjustment backend.

Fuel Inventory:

- Fuel sale backend.
- Tank movement ledger.
- Meter-to-sale reconciliation.
- Fuel adjustment/transfer backend.
- Shift-linked meter readings.

POS:

- Cart/order creation.
- Product/fuel item selection.
- Payment handling.
- Receipt generation.
- Member points integration.
- Cash drawer integration.

Transactions:

- Sales tables.
- Sale item tables.
- Payments table.
- Returns/voids/adjustments tables.
- Cash movements table.

Cash and Shift:

- Shift open/close persistence.
- Cash in/out persistence.
- Cash variance calculation.
- Shift schedule enforcement.

Reports:

- Data queries for all reports.
- Filters.
- Export/print.
- Summary totals.

Security:

- Permission enforcement.
- Stronger authorization policies.
- Remove plaintext default credentials from production config.
- Audit log writes.

Audit Logs:

- Systematic activity log service.
- Log all CRUD and transaction operations.

## 17. Recommended Improvements

| Priority | Module | Problem | Suggested Fix | Affected Files | Affected Tables |
| -------- | ------ | ------- | ------------- | -------------- | --------------- |
| High | POS/Transactions | Sales tables not found | Add `sales`, `sale_items`, `payments` and service layer | Transaction controllers/views, new services | sales, sale_items, payments |
| High | Inventory | Stock changes do not consistently write movement ledger | Centralize stock operations and write `stock_movements` | Configuration controllers, transaction services | stock_movements, warehouse_stocks, display_stocks |
| High | Fuel | Fuel sales/tank drawdown not implemented | Add fuel sale workflow tied to nozzle/tank | Transaction, Salesman POS, fuel services | tanks, nozzles, pump_meter_readings, fuel sale tables |
| High | Auth | Permission records not enforced | Add authorization policies/filters based on role permissions | Filters, controllers, services | roles, permissions, role_permissions |
| High | Security | Default admin and DB root credentials in config | Move secrets to user-secrets/env and force password rotation | Program/appsettings/deployment | users |
| High | Employee | Shift schedule table not found | Add employee shift schedule and enforce Salesman login | new model/controller/views/services | employee_shift_schedules |
| Medium | Reports | Report controllers return views only | Add query models and filters | ReportsController, report views | existing + sales tables |
| Medium | Fuel | Delivery edit/delete does not recalculate tank liters | Replace direct tank mutation with fuel movement ledger | Fuel delivery logic | tanks, fuel_movements suggested |
| Medium | Product | Display/Warehouse pages add stock directly | Convert to stock adjustment/receiving/transfer workflows | ConfigurationController | display_stocks, warehouse_stocks, stock_movements |
| Medium | UI | Mixed status labels/badges | Standardize badges and wording to Active/Disabled | Views | all status tables |
| Medium | Access | Users/Roles/Permissions are UI only | Implement CRUD with soft delete | Users/Roles/Permissions controllers/views | users, roles, permissions |
| Low | Maintainability | Large partial controller and one-line views | Split controllers by module and format views | Configuration controllers/views | None |
| Low | Audit | Activity log writes not widespread | Add activity logging service and call from CRUD/transactions | Services/controllers | activity_logs |

## 18. Suggested Final Database Design

| Table | Purpose | Columns / Relationships | Status |
| ----- | ------- | ----------------------- | ------ |
| `employee_account` | Employee profile/login | username, password_hash, full_name, contact fields, department_id, position_id, status | Existing |
| `branches` | Branch master | name, address, status; > departments | Existing |
| `departments` | Department master | branch_id, name, status; > employee_account | Existing |
| `positions` | Position master | name, description, status; > employee_account | Existing |
| `employee_shift_schedules` | Employee schedule | employee_account_id, shift/date/time, status | Missing |
| `suppliers` | Supplier master | name/contact/status | Existing |
| `product_categories` | Product category | name, description, status | Existing |
| `product_units` | Product units | name, abbreviation, status | Existing |
| `products` | Product master | category_id, unit_id, name, status | Existing |
| `product_batches` | Product batch/pricing | product_id, supplier_id, batch_no, cost/sell price, expiry, status | Existing |
| `warehouse_stocks` | Warehouse stock balance | product_id, batch_id, quantity | Existing |
| `display_stocks` | Display stock balance | product_id, batch_id, quantity | Existing |
| `stock_receivings` | Receiving header | supplier_id, received_date, total, status | Existing |
| `stock_receiving_items` | Receiving lines | receiving_id, product_id, batch_id, qty, prices | Existing |
| `stock_movements` | Stock movement ledger | product_id, batch_id, source, destination, type, qty, reference | Existing but underused |
| `low_stock_settings` | Low stock thresholds | product_id, batch_id, location, minimum | Existing |
| `fuels` | Fuel master | supplier_id, code, name, current price, status | Existing |
| `tanks` | Tank inventory | fuel_id, tank_no, capacity_liters, current_liters, status | Existing |
| `pumps` | Pump master | tank_id, pump_no, name, status | Existing |
| `nozzles` | Nozzle master | pump_id, nozzle_no, status | Existing |
| `fuel_deliveries` | Fuel delivery | supplier_id, fuel_id, tank_id, liters, cost, date | Existing |
| `fuel_price_history` | Price history | fuel_id, old/new price, effective date | Existing |
| `pump_meter_readings` | Nozzle meter readings | nozzle_id, opening/closing/liters, shift_id | Existing / Partial |
| `discounts` | Earn rates | name, earn_rate, status | Existing |
| `members` | Member accounts | member_no, card_no, contacts, discount_id, points | Existing |
| `rebate_rules` | Rebate rules | points_required, rebate_value, status | Existing |
| `points_ledger` | Points history | member_id, transaction_type, points, reference | Existing |
| `discount_rules` | Discount rules | type, value, applies_to, status | Existing |
| `payment_methods` | Payment methods | name, code, status | Existing |
| `shift_settings` | Shift templates | times and flags | Existing |
| `shifts` | Open/close shift sessions | employee/account, branch, open/close times, cash | Missing |
| `sales` | Sale header | cashier/employee, branch, totals, status | Missing |
| `sale_items` | Sale lines | sale_id, product/fuel/nozzle, qty/liters, price | Missing |
| `payments` | Payments | sale_id, method_id, amount, reference | Missing |
| `cash_movements` | Cash in/out | shift_id, type, amount, remarks | Missing |
| `returns` | Returns | sale_id, reason, totals, status | Missing |
| `voids` | Voids | sale_id, reason, user, status | Missing |
| `adjustments` | Inventory/fuel adjustments | item/tank, qty/liters, reason | Missing |
| `roles` | Roles | name, code, status | Existing |
| `permissions` | Permissions | code/name/parent/status | Existing |
| `role_permissions` | Assignments | role_id, permission_id, status | Existing |
| `activity_logs` | Audit trail | user, action, module, description, IP | Existing but underused |

## 19. Suggested Final Page Structure

Setup

- Products
  - Product Master
  - Categories
  - Product Units
  - Product Batches
  - Stock Receiving
  - Warehouse Stock
  - Display Stock
  - Low Stock Settings
- Fuel
  - Fuels
  - Tanks
  - Pumps
  - Nozzles
  - Fuel Deliveries
  - Fuel Price History
  - Pump Meter Readings
  - Tank Reconciliation
- Discounts and Members
  - Discounts
  - Members
  - Rebate Rules
  - Points Ledger
  - Discount Rules
- Config
  - Employees
  - Employee Shift Schedules
  - Suppliers
  - Branches
  - Departments
  - Positions
  - Payment Methods
  - Shift Settings
- Access
  - Users
  - Roles
  - Permissions
  - Role Permissions
  - Activity Logs

POS / Transactions

- Salesman POS
- Product Sales
- Fuel Sales
- Payments
- Open Shift
- Close Shift
- Daily Cash
- Cash In / Cash Out
- Stock Transfer
- Adjustments
- Returns
- Voids

Reports

- Daily Sales
- Product Sales
- Fuel Sales
- Shift Report
- Cash Report
- Inventory Report
- Warehouse Daily Stock
- Display Daily Stock
- Tank Daily Stock
- Purchase History
- Points Report
- Discount Report
- Void Report
- Activity Logs

## 20. Bugs and Issues Found

| Issue | File/Area | Severity | Suggested Fix |
| ----- | --------- | -------- | ------------- |
| Transaction pages are mostly view-only | `TransactionController`, `Views/Transaction` | High | Implement transaction models/services/controllers |
| Sales tables not found | Models/DbContext | High | Add sales/payment/cash schema |
| Shift schedule table not found | Shift module | High | Add schedule table and CRUD |
| Shift login restriction not found | `EmployeeAuthService`, Salesman login | High | Enforce schedule if required |
| Permission enforcement not found | Access module | High | Add policy/filter checks |
| `BlockSalesmanSessionAttribute` checks `EmployeeRole`, but Salesman login does not set it | `Filters/BlockSalesmanSessionAttribute.cs`, `SalesmanAuthController.cs` | Medium | Align session keys or remove stale check |
| Fuel delivery edit/delete does not fully reconcile tank liters | Fuel delivery logic | High | Use tank movement ledger |
| Stock movement table exists but is underused | Inventory logic | High | Centralize stock updates |
| Display/Warehouse pages have disabled Edit | Display/Warehouse views | Medium | Implement edit or remove disabled actions |
| Product Categories edit disabled | `ProductCategories.cshtml` | Medium | Add edit workflow |
| Users/Roles/Permissions UI only | Controllers/Views | High | Implement CRUD |
| Activity log writes not found broadly | Services/controllers | Medium | Add audit service |
| Default admin password in seed | `DatabaseSeeder.cs` | High | Require secure initialization |
| Root DB password in config | `appsettings.json` | High | Move to secrets/environment |
| Mixed `status` and `is_active` | Product/fuel/tank/category areas | Medium | Consolidate state rules |
| One-line Razor views reduce maintainability | Several `Views/Configuration/*.cshtml` | Low | Format views |

## 21. Files Reviewed

Controllers reviewed:

- `AdminController.cs`
- `ConfigurationController.cs`
- `ConfigurationMissingSetupController.cs`
- `DashboardController.cs`
- `EmployeesController.cs`
- `ReportsController.cs`
- `SalesmanAuthController.cs`
- `SalesmanController.cs`
- `ShiftController.cs`
- `SignInController.cs`
- `SuppliersController.cs`
- `TransactionController.cs`
- `TransactionsController.cs`
- `UsersController.cs`
- `RolesController.cs`
- `PermissionsController.cs`

Models reviewed:

- All files under `Models/*.cs`
- All files under `Models/ViewModels/*.cs`

Views reviewed:

- `Views/Shared/Components/Header/Default.cshtml`
- `Views/Configuration/*.cshtml`
- `Views/Employees/Index.cshtml`
- `Views/Suppliers/Index.cshtml`
- `Views/Users/Index.cshtml`
- `Views/Roles/Index.cshtml`
- `Views/Permissions/Index.cshtml`
- `Views/Transaction/*.cshtml`
- `Views/Reports/*.cshtml`
- `Views/Salesman/*.cshtml`
- `Views/SalesmanAuth/Login.cshtml`
- `Views/Shift/Schedule.cshtml`

Shared/layout/CSS/JS reviewed:

- `ViewComponents/HeaderViewComponent.cs`
- `Views/Shared/Components/Header/Default.cshtml`
- `wwwroot/css/site.css`
- `wwwroot/js/site.js`

Data/migrations/services reviewed:

- `Data/ApplicationDbContext.cs`
- `Migrations/*.cs`
- `Services/UserAuthService.cs`
- `Services/EmployeeAuthService.cs`
- `Services/DatabaseSeeder.cs`
- `Services/ProductBatchNumberService.cs`

## 22. Summary for Another AI

Copyable summary:

This is an ASP.NET Core MVC gas station POS/admin system named `gpos_core_mvc`, using Pomelo MySQL and cookie/session authentication. Existing modules include Setup for products, fuel, discounts/members, config, and access; Salesman login/POS; transaction pages; and report pages. Real CRUD exists mainly in Setup pages. Transaction and report pages mostly exist as UI-only views.

Existing tables include users, roles, permissions, user_roles, role_permissions, suppliers, fuels, tanks, pumps, nozzles, fuel_deliveries, fuel_price_history, pump_meter_readings, product_categories, product_units, products, product_batches, stock_receivings, stock_receiving_items, stock_movements, warehouse_stocks, display_stocks, low_stock_settings, discounts, members, rebate_rules, points_ledger, discount_rules, branches, departments, positions, employee_account, station_settings, payment_methods, shift_settings, and activity_logs.

Main relationships are Branch > Department > Employee Account, Position > Employee Account, Fuel > Tank > Pump > Nozzle > Pump Meter Reading, Product Category > Product > Product Batch, Product Unit > Product, Supplier > Product Batch/Stock Receiving/Fuel/Fuel Delivery, Discount > Member > Points Ledger, User > User Role > Role, and Role > Role Permission > Permission.

Biggest problems: transaction backend and sales/payment tables are missing; reports are UI-only; inventory/fuel movement ledgers are incomplete; fuel tank reconciliation is missing; user/role/permission CRUD is UI-only; permissions are not enforced; shift schedule and shift session tables are missing; activity logging is underused; default admin credentials and DB password are in source/config.

Best next improvements: implement sales/payment/cash/shift schema and services, centralize stock and fuel movement logic, enforce permissions, add employee shift scheduling and login restrictions, build real report queries, standardize status/soft-delete behavior, and add activity logging for all CRUD and transaction actions.

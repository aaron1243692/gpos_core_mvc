using gpos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace gpos.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var controller = RouteData.Values["controller"]?.ToString() ?? string.Empty;
            var action = RouteData.Values["action"]?.ToString() ?? string.Empty;
            var username = HttpContext.Session.GetString("Username")
                ?? HttpContext.Session.GetString("Name")
                ?? User.Identity?.Name
                ?? "User";

            var model = new HeaderViewModel
            {
                Username = username,
                UserInitials = InitialsFor(username),
                CurrentController = controller,
                CurrentAction = action,
                ActiveTab = ActiveTabFor(controller, action),
                ActivePage = ActivePageFor(controller, action)
            };

            return View(model);
        }

        private static string InitialsFor(string name)
        {
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length >= 2)
            {
                return string.Concat(parts[0][0], parts[1][0]).ToUpperInvariant();
            }

            return name.Length > 0 ? name[0].ToString().ToUpperInvariant() : "U";
        }

        private static string ActiveTabFor(string controller, string action)
        {
            if (Is(controller, "Dashboard"))
            {
                return "Dashboard";
            }

            if ((Is(controller, "Configuration") && IsInventoryConfiguration(action)) || IsLegacyInventoryConfiguration(action))
            {
                return "Inventory";
            }

            if (Is(controller, "Configuration") || Is(controller, "Vouchers") || IsLegacySetupConfiguration(action))
            {
                return "Configuration";
            }

            if (Is(controller, "Transaction") && Is(action, "POS"))
            {
                return "POS";
            }

            if (Is(controller, "Transactions") || Is(controller, "Transaction"))
            {
                return "Transactions";
            }

            if (Is(controller, "Reports"))
            {
                return "Reports";
            }

            if (Is(controller, "Users") || Is(controller, "Suppliers") || Is(controller, "Operations") || Is(controller, "Branches") || Is(controller, "Departments") || Is(controller, "Shift") || Is(controller, "Roles") || Is(controller, "Permissions") || IsLegacySetupUsers(action))
            {
                return "Configuration";
            }

            if (Is(controller, "Customers"))
            {
                return "Users";
            }

            return "Dashboard";
        }

        private static string ActivePageFor(string controller, string action)
        {
            if (Is(controller, "Dashboard"))
            {
                return "Dashboard.Index";
            }

            if (Is(controller, "Configuration"))
            {
                return $"Configuration.{action}";
            }

            if (Is(controller, "Vouchers"))
            {
                return $"Vouchers.{action}";
            }

            if (Is(controller, "Transactions"))
            {
                return $"Transactions.{action}";
            }

            if (Is(controller, "Reports"))
            {
                return $"Reports.{MapReportAction(action)}";
            }

            if (Is(controller, "Shift"))
            {
                return $"Shift.{action}";
            }

            if (Is(controller, "Users") || Is(controller, "Customers") || Is(controller, "Suppliers") || Is(controller, "Operations") || Is(controller, "Branches") || Is(controller, "Departments") || Is(controller, "Roles") || Is(controller, "Permissions"))
            {
                return $"{controller}.Index";
            }

            if (Is(controller, "Setup"))
            {
                return action switch
                {
                    "Products" => "Configuration.Products",
                    "DisplayProducts" => "Configuration.DisplayProducts",
                    "WarehouseProducts" => "Configuration.WarehouseProducts",
                    "Categories" => "Configuration.ProductCategories",
                    "ProductBatches" => "Configuration.ProductBatches",
                    "StockReceiving" => "Configuration.StockReceiving",
                    "ProductUnits" => "Configuration.ProductUnits",
                    "LowStockSettings" => "Configuration.LowStockSettings",
                    "ItemUnits" => "Configuration.ItemUnits",
                    "Fuels" => "Configuration.Fuels",
                    "FuelTypes" => "Configuration.FuelTypes",
                    "FuelPriceHistory" => "Transaction.FuelPriceAdjustment",
                    "Pumps" => "Configuration.Pumps",
                    "Nozzles" => "Configuration.Nozzles",
                    "FuelDeliveries" => "Configuration.FuelDeliveries",
                    "PumpMeterReadings" => "Configuration.PumpMeterReadings",
                    "PumpUnits" => "Configuration.PumpUnits",
                    "FuelTanks" => "Configuration.Tanks",
                    "Discounts" => "Configuration.Discounts",
                    "Members" => "Configuration.Members",
                    "Rebate" => "Configuration.Rebate",
                    "PointsLedger" => "Configuration.PointsLedger",
                    "DiscountRules" => "Configuration.DiscountRules",
                    "Position" => "Configuration.Position",
                    "Branch" => "Configuration.Branch",
                    "Department" => "Configuration.Department",
                    "Users" => "Users.Index",
                    "Suppliers" => "Suppliers.Index",
                    "Operations" => "Operations.Index",
                    "Branches" => "Branches.Index",
                    "Departments" => "Departments.Index",
                    "Roles" => "Roles.Index",
                    "Permissions" => "Permissions.Index",
                    "RolePermissions" => "Configuration.RolePermissions",
                    "ActivityLogs" => "Configuration.ActivityLogs",
                    _ => string.Empty
                };
            }

            if (Is(controller, "Transaction"))
            {
                return $"Transaction.{action}";
            }

            return string.Empty;
        }

        private static string MapReportAction(string action) => action switch
        {
            "DailySalesReport" => "DailySales",
            "ShiftSalesReport" => "ShiftReport",
            "FuelSalesReport" => "FuelSales",
            "ProductSalesReport" => "ProductSales",
            "CashierSalesReport" => "CashReport",
            _ => action
        };

        private static bool IsInventoryConfiguration(string action) => action is "DisplayProducts" or "WarehouseProducts" or "ProductBatches" or "FuelBatches" or "LowStockSettings" or "Tanks";

        private static bool IsLegacyInventoryConfiguration(string action) => action is "Products" or "Categories" or "ItemUnits" or "FuelTanks";

        private static bool IsLegacySetupConfiguration(string action) => action is "Fuels" or "FuelTypes" or "Pumps" or "Nozzles" or "FuelDeliveries" or "PumpMeterReadings" or "PumpUnits" or "Discounts" or "Members" or "Rebate" or "PointsLedger" or "DiscountRules" or "Position" or "Branch" or "Department" or "RolePermissions" or "ActivityLogs";

        private static bool IsLegacySetupUsers(string action) => action is "Users" or "Suppliers" or "Operations" or "Branches" or "Departments" or "Roles" or "Permissions";

        private static bool Is(string left, string right) => string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
    }
}

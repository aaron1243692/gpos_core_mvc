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

            if (Is(controller, "Configuration") || IsLegacySetupConfiguration(action))
            {
                return "Configuration";
            }

            if (Is(controller, "Transactions") || Is(controller, "Transaction") || Is(controller, "Salesman") && Is(action, "POS"))
            {
                return "Transactions";
            }

            if (Is(controller, "Reports"))
            {
                return "Reports";
            }

            if (Is(controller, "Users") || Is(controller, "Employees") || Is(controller, "Suppliers") || Is(controller, "Roles") || Is(controller, "Permissions") || IsLegacySetupUsers(action))
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

            if (Is(controller, "Transactions"))
            {
                return $"Transactions.{action}";
            }

            if (Is(controller, "Salesman") && Is(action, "POS"))
            {
                return "Salesman.POS";
            }

            if (Is(controller, "Reports"))
            {
                return $"Reports.{MapReportAction(action)}";
            }

            if (Is(controller, "Users") || Is(controller, "Employees") || Is(controller, "Customers") || Is(controller, "Suppliers") || Is(controller, "Roles") || Is(controller, "Permissions"))
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
                    "ItemUnits" => "Configuration.ItemUnits",
                    "Fuels" => "Configuration.Fuels",
                    "FuelTypes" => "Configuration.FuelTypes",
                    "FuelUnits" => "Configuration.FuelUnits",
                    "FuelPriceHistory" => "Configuration.FuelPrices",
                    "Pumps" => "Configuration.Pumps",
                    "PumpUnits" => "Configuration.PumpUnits",
                    "FuelTanks" => "Configuration.Tanks",
                    "Discounts" => "Configuration.Discounts",
                    "Members" => "Configuration.Members",
                    "Rebate" => "Configuration.Rebate",
                    "Users" => "Users.Index",
                    "Employees" => "Employees.Index",
                    "Suppliers" => "Suppliers.Index",
                    "Roles" => "Roles.Index",
                    "Permissions" => "Permissions.Index",
                    _ => string.Empty
                };
            }

            if (Is(controller, "Transaction"))
            {
                return action switch
                {
                    "OpenShift" => "Transactions.OpenShift",
                    "DailyClosingBalance" => "Transactions.CloseShift",
                    "PumpTransaction" => "Transactions.FuelTransactions",
                    "PosProductSales" => "Transactions.ProductSales",
                    _ => string.Empty
                };
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

        private static bool IsLegacySetupConfiguration(string action) => action is "Products" or "DisplayProducts" or "WarehouseProducts" or "Categories" or "ItemUnits" or "Fuels" or "FuelTypes" or "FuelUnits" or "FuelPriceHistory" or "Pumps" or "PumpUnits" or "FuelTanks" or "Discounts" or "Members" or "Rebate";

        private static bool IsLegacySetupUsers(string action) => action is "Users" or "Employees" or "Suppliers" or "Roles" or "Permissions";

        private static bool Is(string left, string right) => string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
    }
}

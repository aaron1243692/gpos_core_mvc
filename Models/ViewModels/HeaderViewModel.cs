namespace gpos.Models.ViewModels
{
    public class HeaderViewModel
    {
        public string Username { get; init; } = "User";
        public string UserInitials { get; init; } = "U";
        public string CurrentController { get; init; } = string.Empty;
        public string CurrentAction { get; init; } = string.Empty;
        public string ActiveTab { get; init; } = "Dashboard";
        public string ActivePage { get; init; } = "Dashboard.Index";
    }
}

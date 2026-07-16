namespace gpos.Models.ViewModels
{
    public class VoidPageViewModel
    {
        public string Search { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

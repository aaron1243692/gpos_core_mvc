namespace gpos.Models.ViewModels
{
    public sealed class TankFuelInvariantResult
    {
        public int TankId { get; init; }
        public decimal TankLiters { get; init; }
        public decimal ActiveBatchLiters { get; init; }
        public decimal Difference { get; init; }
        public bool IsBalanced => Difference == 0m;
        public int ActiveBatchCount { get; init; }
        public string InventoryState { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
    }
}

namespace gpos.Models
{
    public class FuelStockMovement
    {
        public int Id { get; set; }
        public int TankId { get; set; }
        public int FuelId { get; set; }
        public int FuelBatchId { get; set; }
        public int BranchId { get; set; }
        public string MovementType { get; set; } = string.Empty;
        public decimal LitersIn { get; set; }
        public decimal LitersOut { get; set; }
        public decimal BatchLitersBefore { get; set; }
        public decimal BatchLitersAfter { get; set; }
        public decimal TankLitersBefore { get; set; }
        public decimal TankLitersAfter { get; set; }
        public decimal UnitCostSnapshot { get; set; }
        public string ReferenceType { get; set; } = string.Empty;
        public int ReferenceId { get; set; }
        public string? Remarks { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

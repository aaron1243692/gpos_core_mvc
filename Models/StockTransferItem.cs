namespace gpos.Models
{
    public class StockTransferItem
    {
        public int Id { get; set; }
        public int StockTransferId { get; set; }
        public int? ProductId { get; set; }
        public int? BatchId { get; set; }
        public int? FuelId { get; set; }
        public int? SourceTankId { get; set; }
        public int? DestinationTankId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Liters { get; set; }
        public decimal? SourceBefore { get; set; }
        public decimal? SourceAfter { get; set; }
        public decimal? DestinationBefore { get; set; }
        public decimal? DestinationAfter { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public StockTransfer? StockTransfer { get; set; }
        public Product? Product { get; set; }
        public ProductBatch? Batch { get; set; }
        public Fuel? Fuel { get; set; }
        public Tank? SourceTank { get; set; }
        public Tank? DestinationTank { get; set; }
    }
}

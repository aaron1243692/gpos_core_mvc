namespace gpos.Models
{
    public class SaleItem
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public int? ProductId { get; set; }
        public int? FuelId { get; set; }
        public int? TankId { get; set; }
        public int? NozzleId { get; set; }
        public int? BatchId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Liters { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
        public string Status { get; set; } = "Completed";
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Sale? Sale { get; set; }
        public Product? Product { get; set; }
        public Fuel? Fuel { get; set; }
        public Tank? Tank { get; set; }
        public Nozzle? Nozzle { get; set; }
        public ProductBatch? Batch { get; set; }
    }
}

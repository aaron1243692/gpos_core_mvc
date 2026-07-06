namespace gpos.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public int Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Fuel> Fuels { get; set; } = new List<Fuel>();
        public ICollection<ProductBatch> ProductBatches { get; set; } = new List<ProductBatch>();
        public ICollection<FuelBatch> FuelBatches { get; set; } = new List<FuelBatch>();
        public ICollection<StockReceiving> StockReceivings { get; set; } = new List<StockReceiving>();
        public ICollection<FuelDelivery> FuelDeliveries { get; set; } = new List<FuelDelivery>();
    }
}

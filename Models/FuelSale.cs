namespace gpos.Models
{
    public class FuelSale
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int FuelId { get; set; }
        public int TankId { get; set; }
        public int? NozzleId { get; set; }
        public int? PumpId { get; set; }
        public int? DispenserId { get; set; }
        public decimal Liters { get; set; }
        public decimal PricePerLiter { get; set; }
        public decimal Subtotal { get; set; }
        public decimal? TankLitersBefore { get; set; }
        public decimal? TankLitersAfter { get; set; }
        public string Status { get; set; } = "Completed";
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Sale? Sale { get; set; }
        public Fuel? Fuel { get; set; }
        public Tank? Tank { get; set; }
        public Nozzle? Nozzle { get; set; }
        public Pump? Pump { get; set; }
        public Dispenser? Dispenser { get; set; }
    }
}

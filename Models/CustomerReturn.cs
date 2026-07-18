namespace gpos.Models;

public class CustomerReturn
{
    public int Id { get; set; }
    public string ReturnNo { get; set; } = string.Empty;
    public int SaleId { get; set; }
    public string OriginalReceiptNo { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public int? MemberId { get; set; }
    public string ReturnType { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending Inspection";
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? InspectedByUserId { get; set; }
    public DateTime? InspectedAt { get; set; }
    public string? InspectionDecision { get; set; }
    public string? InspectionNotes { get; set; }
    public int? ProcessedByUserId { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Sale? Sale { get; set; }
    public Branch? Branch { get; set; }
    public Member? Member { get; set; }
    public User? CreatedByUser { get; set; }
    public User? InspectedByUser { get; set; }
    public User? ProcessedByUser { get; set; }
    public ICollection<CustomerProductReturnItem> ProductItems { get; set; } = new List<CustomerProductReturnItem>();
    public ICollection<CustomerFuelReturnItem> FuelItems { get; set; } = new List<CustomerFuelReturnItem>();
}

public class CustomerProductReturnItem
{
    public int Id { get; set; }
    public int CustomerReturnId { get; set; }
    public int ProductSaleId { get; set; }
    public int ProductId { get; set; }
    public int? OriginalBatchId { get; set; }
    public int? OriginalDisplayStockId { get; set; }
    public decimal Quantity { get; set; }
    public decimal OriginalUnitPrice { get; set; }
    public decimal ReturnAmount { get; set; }
    public string? InspectionResult { get; set; }
    public string? Disposition { get; set; }
    public int? StockMovementId { get; set; }
    public CustomerReturn? CustomerReturn { get; set; }
    public ProductSale? ProductSale { get; set; }
    public Product? Product { get; set; }
    public ProductBatch? OriginalBatch { get; set; }
    public DisplayStock? OriginalDisplayStock { get; set; }
    public StockMovement? StockMovement { get; set; }
}

public class CustomerFuelReturnItem
{
    public int Id { get; set; }
    public int CustomerReturnId { get; set; }
    public int FuelSaleId { get; set; }
    public int FuelId { get; set; }
    public int OriginalTankId { get; set; }
    public int? OriginalPumpId { get; set; }
    public int? OriginalNozzleId { get; set; }
    public decimal Liters { get; set; }
    public decimal OriginalPricePerLiter { get; set; }
    public decimal ReturnAmount { get; set; }
    public string? InspectionResult { get; set; }
    public string? Disposition { get; set; }
    public CustomerReturn? CustomerReturn { get; set; }
    public FuelSale? FuelSale { get; set; }
    public Fuel? Fuel { get; set; }
    public Tank? OriginalTank { get; set; }
    public Pump? OriginalPump { get; set; }
    public Nozzle? OriginalNozzle { get; set; }
}

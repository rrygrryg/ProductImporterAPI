namespace ProductImporterAPI.Models
{
    // Reprezentacja tabeli Inventory
    public class Inventory
    {
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public string Unit { get; set; }
        public int Qty { get; set; }
        public decimal ShippingCost { get; set; }
    }
}
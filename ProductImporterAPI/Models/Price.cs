namespace ProductImporterAPI.Models
{
    // Reprezentacja tabeli Prices
    public class Price
    {
        public string Sku { get; set; }
        public decimal NetPricePerUnit { get; set; }
        public decimal NetPriceAfterDiscount { get; set; }
        public decimal VatRate { get; set; }
        public decimal NetPricePerLogisticUnit { get; set; }
    }
}
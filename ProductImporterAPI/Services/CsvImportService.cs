using CsvHelper;
using ProductImporterAPI.Data;
using ProductImporterAPI.Models;
using System.Globalization;
using Dapper;

namespace ProductImporterAPI.Services
{
    public class CsvImportService
    {
        private readonly DapperContext _context;
        private readonly HttpClient _httpClient;

        public CsvImportService(DapperContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        // Główna funkcja importu danych z trzech plików CSV
        public async Task ImportDataAsync()
        {
            await ImportProducts();
            await ImportInventory();
            await ImportPrices();
        }

        private async Task ImportProducts()
        {
            var url = "https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv";
            using var stream = await _httpClient.GetStreamAsync(url);
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<dynamic>()
                .Where(r =>
                {
                    var dict = r as IDictionary<string, object>;
                    return dict != null &&
                           dict.ContainsKey("shipping") &&
                           int.TryParse(dict["shipping"]?.ToString(), out var s) &&
                           s <= 24;
                })
                .Select(r => new Product
                {
                    Id = int.Parse(r.ID),
                    SKU = r.SKU,
                    Name = r.name,
                    EAN = r.EAN,
                    ProducerName = r.producer_name,
                    Category = r.category,
                    DefaultImage = r.default_image
                }).ToList();

            var sql = @"INSERT INTO Products (Id, SKU, Name, EAN, ProducerName, Category, DefaultImage)
                        VALUES (@Id, @SKU, @Name, @EAN, @ProducerName, @Category, @DefaultImage)";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, records);
        }

        private async Task ImportInventory()
        {
            var url = "https://rekturacjazadanie.blob.core.windows.net/zadanie/Inventory.csv";
            using var stream = await _httpClient.GetStreamAsync(url);
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<dynamic>()
                .Where(r =>
                {
                    var dict = r as IDictionary<string, object>;
                    return dict != null &&
                           dict.ContainsKey("shipping") &&
                           int.TryParse(dict["shipping"]?.ToString(), out var s) &&
                           s <= 24;
                })
                .Select(r => new Inventory
                {
                    ProductId = int.Parse(r.product_id),
                    Sku = r.sku,
                    Unit = r.unit,
                    Qty = int.Parse(r.qty),
                    ShippingCost = decimal.Parse(r.shipping_cost)
                }).ToList();

            var sql = @"INSERT INTO Inventory (ProductId, Sku, Unit, Qty, ShippingCost)
                        VALUES (@ProductId, @Sku, @Unit, @Qty, @ShippingCost)";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, records);
        }

        private async Task ImportPrices()
        {
            var url = "https://rekturacjazadanie.blob.core.windows.net/zadanie/Prices.csv";
            using var stream = await _httpClient.GetStreamAsync(url);
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<dynamic>()
                .Select(r => new Price
                {
                    Sku = r.Column2,
                    NetPricePerUnit = decimal.Parse(r.Column3),
                    NetPriceAfterDiscount = decimal.Parse(r.Column4),
                    VatRate = decimal.Parse(r.Column5),
                    NetPricePerLogisticUnit = decimal.Parse(r.Column6)
                }).ToList();

            var sql = @"INSERT INTO Prices (Sku, NetPricePerUnit, NetPriceAfterDiscount, VatRate, NetPricePerLogisticUnit)
                        VALUES (@Sku, @NetPricePerUnit, @NetPriceAfterDiscount, @VatRate, @NetPricePerLogisticUnit)";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, records);
        }
    }
}
using Dapper;
using ProductImporterAPI.Data;

namespace ProductImporterAPI.Repositories
{
    public class ProductRepository
    {
        private readonly DapperContext _context;

        public ProductRepository(DapperContext context)
        {
            _context = context;
        }

        // Zwraca wszystkie wymagane dane produktu po SKU
        public async Task<dynamic?> GetProductDetailsBySkuAsync(string sku)
        {
            var sql = @"
                SELECT 
                    p.Name, p.EAN, p.ProducerName, p.Category, p.DefaultImage,
                    i.Qty, i.Unit, pr.NetPricePerLogisticUnit, i.ShippingCost
                FROM Products p
                JOIN Inventory i ON p.Id = i.ProductId
                JOIN Prices pr ON p.SKU = pr.Sku
                WHERE p.SKU = @Sku";

            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync(sql, new { Sku = sku });
        }
    }
}
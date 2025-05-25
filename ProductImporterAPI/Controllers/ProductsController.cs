using Microsoft.AspNetCore.Mvc;
using ProductImporterAPI.Repositories;
using ProductImporterAPI.Services;

namespace ProductImporterAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly CsvImportService _importService;
        private readonly ProductRepository _repository;

        public ProductsController(CsvImportService importService, ProductRepository repository)
        {
            _importService = importService;
            _repository = repository;
        }

        // Endpoint do importowania danych z CSV do bazy danych
        [HttpPost("import")]
        public async Task<IActionResult> ImportData()
        {
            await _importService.ImportDataAsync();
            return Ok("Dane zostały zaimportowane.");
        }

        // Endpoint do pobierania szczegółów produktu po SKU
        [HttpGet("{sku}")]
        public async Task<IActionResult> GetBySku(string sku)
        {
            var product = await _repository.GetProductDetailsBySkuAsync(sku);
            return product != null ? Ok(product) : NotFound();
        }
    }
}
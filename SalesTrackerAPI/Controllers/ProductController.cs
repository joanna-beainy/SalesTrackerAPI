using Microsoft.AspNetCore.Mvc;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Interfaces;
using SalesTracker.Application.Services;
using SalesTracker.Shared.Constants;
using SalesTracker.Shared.Responses;


namespace SalesTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IExcelImportService _excelImportService;

        public ProductController(IProductService service, IExcelImportService excelImportService)
        {
            _service = service;
            _excelImportService = excelImportService;
        }

        // GET: api/product
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadProductDto>>>> GetAll()
        {
            var products = await _service.GetAllAsync();

            if (!products.Any())
                return NotFound(ApiResponse<String>.Fail(APIMessages.NoProduct));

            return Ok(ApiResponse<IEnumerable<ReadProductDto>>.Ok(products, APIMessages.ProductsRetrieved));
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ReadProductDto>>> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            return product == null
                ? NotFound(ApiResponse<ReadProductDto>.Fail(APIMessages.ProductNotFound))
                : Ok(ApiResponse<ReadProductDto>.Ok(product, APIMessages.ProductRetrieved));
        }

        // POST: api/product
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddProductDto dto)
        {
            await _service.AddAsync(dto);
            return Ok(ApiResponse<string>.Ok(null, APIMessages.ProductCreated));
        }

        // PUT: api/product/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok(ApiResponse<string>.Ok(null, APIMessages.ProductUpdated));
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            await _service.SoftDeleteAsync(id);
            return Ok(ApiResponse<string>.Ok(null, APIMessages.ProductDeleted));
        }

        // PATCH: api/product/{id}/stock
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto dto)
        {
            await _service.UpdateStockAsync(id, dto);
            return Ok(ApiResponse<string>.Ok(null, APIMessages.StockUpdated));
        }

        // GET: api/product/low-stock
        [HttpGet("low-stock")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadProductDto>>>> GetLowStock()
        {
            var products = await _service.GetLowStockAsync();

            if (!products.Any())
                return Ok(ApiResponse<IEnumerable<ReadProductDto>>.Ok(products, APIMessages.NoLowStock));

            return Ok(ApiResponse<IEnumerable<ReadProductDto>>.Ok(products, APIMessages.LowStockRetrieved));
        }


        // GET: api/product/search?keyword=abc
        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadProductDto>>>> Search([FromQuery] string keyword)
        {
            var results = await _service.SearchAsync(keyword);
            if (!results.Any())
                return NotFound(ApiResponse<IEnumerable<ReadProductDto>>.Fail(APIMessages.SearchKeywordNotFound(keyword)));

            return Ok(ApiResponse<IEnumerable<ReadProductDto>>.Ok(results, APIMessages.SearchResultsRetrieved));
        }


        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            var imported = await _excelImportService.ImportProductsFromExcelAsync(file);
            return Ok(ApiResponse<List<ReadProductDto>>.Ok(imported));
        }
    }
}

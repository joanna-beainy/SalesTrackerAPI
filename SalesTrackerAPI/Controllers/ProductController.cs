using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Interfaces;
using SalesTracker.Application.Services;
using SalesTracker.InfraStructure.Models.Entities;
using SalesTracker.Shared.Constants;
using SalesTracker.Shared.Responses;


namespace SalesTrackerAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IExcelImportService _excelImportService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService service, IExcelImportService excelImportService, ILogger<ProductController> logger)
        {
            _productService = service;
            _excelImportService = excelImportService;
            _logger = logger;
        }

        // GET: api/product
        [Authorize(Roles = "admin, cashier")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadProductDto>>>> GetAll()
        {
            _logger.LogInformation("Retrieving all products");

            var products = await _productService.GetAllAsync();

            if (!products.Any())
            {
                _logger.LogWarning("No products found in the database");
                return NotFound(ApiResponse<string>.Fail(APIMessages.NoProduct));
            }

            return Ok(ApiResponse<IEnumerable<ReadProductDto>>.Ok(products, APIMessages.ProductsRetrieved));

        }

        // GET: api/product/{id}
        [Authorize(Roles = "admin, cashier")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ReadProductDto>>> GetById(int id)
        {
            _logger.LogInformation("Fetching product by ID: {ProductId}", id);

            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", id);
                return NotFound(ApiResponse<ReadProductDto>.Fail(APIMessages.ProductNotFound));
            }

            return Ok(ApiResponse<ReadProductDto>.Ok(product, APIMessages.ProductRetrieved));
        }

        // POST: api/product
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddProductDto dto)
        {
            _logger.LogInformation("Creating new product: {ProductName}", dto.Name);

            await _productService.AddAsync(dto);
            return Ok(ApiResponse<string>.Ok(null, APIMessages.ProductCreated));
        }

        // PUT: api/product/{id}
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            _logger.LogInformation("Updating product ID {ProductId}", id);

            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Update failed: Product ID {ProductId} not found", id);
                return NotFound(ApiResponse<string>.Fail(APIMessages.ProductNotFound));
            }

            await _productService.UpdateAsync(id, dto);
            return Ok(ApiResponse<string>.Ok(null, APIMessages.ProductUpdated));
        }

        // DELETE: api/product/{id}
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            _logger.LogInformation("Soft deleting product ID {ProductId}", id);

            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Soft delete failed: Product ID {ProductId} not found", id);
                return NotFound(ApiResponse<string>.Fail(APIMessages.ProductNotFound));
            }

            await _productService.SoftDeleteAsync(id);
            return Ok(ApiResponse<string>.Ok(null, APIMessages.ProductDeleted));
        }

        // PATCH: api/product/{id}/stock
        [Authorize(Roles = "admin")]
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto dto)
        {

            _logger.LogInformation("PATCH /api/product/{id}/stock - Updating stock for product ID {ProductId} to {Stock}", id, dto.Stock);

            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Stock update failed: Product ID {ProductId} not found", id);
                return NotFound(ApiResponse<string>.Fail(APIMessages.ProductNotFound));
            }

            await _productService.UpdateStockAsync(id, dto);
            return Ok(ApiResponse<string>.Ok(null, APIMessages.StockUpdated));

        }

        // GET: api/product/low-stock
        [Authorize(Roles = "admin,manager")]
        [HttpGet("low-stock")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadProductDto>>>> GetLowStock()
        {
            _logger.LogInformation("Retrieving products with low stock");
            var products = await _productService.GetLowStockAsync();

            if (!products.Any())
            {
                _logger.LogWarning("No products currently in low stock");
                return Ok(ApiResponse<IEnumerable<ReadProductDto>>.Ok(products, APIMessages.NoLowStock));
            }

            return Ok(ApiResponse<IEnumerable<ReadProductDto>>.Ok(products, APIMessages.LowStockRetrieved));
        }

        [HttpGet("categories")]
        [Authorize(Roles = "admin,cashier,manager")]
        public async Task<IActionResult> GetCategories()
        {
            _logger.LogInformation("Fetching product categories");
            var categories = await _productService.GetAllCategoriesAsync();
            return Ok(ApiResponse<List<string>>.Ok(categories, APIMessages.CategorieRetrieved));
        }


        // GET: api/product/search?keyword=abc
        [Authorize(Roles = "admin,manager,user")]
        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadProductDto>>>> Search([FromQuery] string keyword)
        {

            _logger.LogInformation("Searching products with keyword: {Keyword}", keyword);
            var results = await _productService.SearchAsync(keyword);

            if (!results.Any())
            {
                _logger.LogWarning("No products found for keyword: {Keyword}", keyword);
                return NotFound(ApiResponse<IEnumerable<ReadProductDto>>.Fail(APIMessages.SearchKeywordNotFound(keyword)));
            }

            return Ok(ApiResponse<IEnumerable<ReadProductDto>>.Ok(results, APIMessages.SearchResultsRetrieved));
        }

        [Authorize(Roles = "admin")]
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if(file == null || file.Length == 0)
            {
                _logger.LogWarning("No valid Excel file uploaded");
                return BadRequest(ApiResponse<string>.Fail("No file uploaded or file is empty"));
            }
            _logger.LogInformation("Importing products from Excel file: {FileName}", file.FileName);
            var imported = await _excelImportService.ImportProductsFromExcelAsync(file);
            return Ok(ApiResponse<List<ReadProductDto>>.Ok(imported, APIMessages.ProductImported));
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Fetching paged products - Page: {Page}, Size: {PageSize}", page, pageSize);

            if (page < 1 || pageSize <= 0)
            {
                _logger.LogWarning("Invalid pagination request: Page={Page}, PageSize={PageSize}", page, pageSize);
                return BadRequest(ApiResponse<string>.Fail("Invalid pagination parameters"));
            }

            var paginatedProducts = await _productService.GetPaginatedAsync(page, pageSize);

            return Ok(ApiResponse<PaginatedResult<ReadProductDto>>.Ok(paginatedProducts, "Products retrieved successfully"));
        }


    }
}

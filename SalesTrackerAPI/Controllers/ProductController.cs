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

        public ProductController(IProductService service, IExcelImportService excelImportService)
        {
            _productService = service;
            _excelImportService = excelImportService;
        }

        // GET: api/product
        [Authorize(Roles = "admin, cashier")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadProductDto>>>> GetAll()
        {
            var products = await _productService.GetAllAsync();

            if (!products.Any())
                return NotFound(ApiResponse<String>.Fail(APIMessages.NoProduct));

            return Ok(ApiResponse<IEnumerable<ReadProductDto>>.Ok(products, APIMessages.ProductsRetrieved));
        }

        // GET: api/product/{id}
        [Authorize(Roles = "admin, cashier")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ReadProductDto>>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            return product == null
                ? NotFound(ApiResponse<ReadProductDto>.Fail(APIMessages.ProductNotFound))
                : Ok(ApiResponse<ReadProductDto>.Ok(product, APIMessages.ProductRetrieved));
        }

        // POST: api/product
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddProductDto dto)
        {
            await _productService.AddAsync(dto);
            return Ok(ApiResponse<string>.Ok(null, APIMessages.ProductCreated));
        }

        // PUT: api/product/{id}
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            await _productService.UpdateAsync(id, dto);
            return Ok(ApiResponse<string>.Ok(null, APIMessages.ProductUpdated));
        }

        // DELETE: api/product/{id}
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            await _productService.SoftDeleteAsync(id);
            return Ok(ApiResponse<string>.Ok(null, APIMessages.ProductDeleted));
        }

        // PATCH: api/product/{id}/stock
        [Authorize(Roles = "admin")]
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto dto)
        {
            await _productService.UpdateStockAsync(id, dto);
            return Ok(ApiResponse<string>.Ok(null, APIMessages.StockUpdated));
        }

        // GET: api/product/low-stock
        [Authorize(Roles = "admin,manager")]
        [HttpGet("low-stock")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadProductDto>>>> GetLowStock()
        {
            var products = await _productService.GetLowStockAsync();

            if (!products.Any())
                return Ok(ApiResponse<IEnumerable<ReadProductDto>>.Ok(products, APIMessages.NoLowStock));

            return Ok(ApiResponse<IEnumerable<ReadProductDto>>.Ok(products, APIMessages.LowStockRetrieved));
        }

        [HttpGet("categories")]
        [Authorize(Roles = "admin,cashier,manager")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _productService.GetAllCategoriesAsync();
            return Ok(ApiResponse<List<string>>.Ok(categories, APIMessages.CategorieRetrieved));
        }


        // GET: api/product/search?keyword=abc
        [Authorize(Roles = "admin,manager,user")]
        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadProductDto>>>> Search([FromQuery] string keyword)
        {
            var results = await _productService.SearchAsync(keyword);
            if (!results.Any())
                return NotFound(ApiResponse<IEnumerable<ReadProductDto>>.Fail(APIMessages.SearchKeywordNotFound(keyword)));

            return Ok(ApiResponse<IEnumerable<ReadProductDto>>.Ok(results, APIMessages.SearchResultsRetrieved));
        }

        [Authorize(Roles = "admin")]
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            var imported = await _excelImportService.ImportProductsFromExcelAsync(file);
            return Ok(ApiResponse<List<ReadProductDto>>.Ok(imported));
        }

    }
}

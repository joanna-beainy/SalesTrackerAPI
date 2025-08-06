using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Interfaces;
using SalesTracker.Shared;
using SalesTracker.Shared.Constants;
using SalesTracker.Shared.Responses;

namespace SalesTrackerAPI.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/sale")]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        // GET: api/sale
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ReadSaleDto>>>> GetAllAsync()
        {
            var sales = await _saleService.GetAllAsync();

            if (!sales.Any())
                return NotFound(ApiResponse<String>.Fail(APIMessages.SaleNotFound));
            return Ok(ApiResponse<List<ReadSaleDto>>.Ok(sales, APIMessages.SalesRetreived));
        }

        // GET: api/sale/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ReadSaleDto>>> GetByIdAsync(int id)
        {
            var sale = await _saleService.GetByIdAsync(id);
            if (sale == null)
                return NotFound(ApiResponse<ReadSaleDto>.Fail(APIMessages.SaleNotFound));

            return Ok(ApiResponse<ReadSaleDto>.Ok(sale, APIMessages.SaleRetreived));
        }

        // GET: api/sale/date-range?from=2025-07-01&to=2025-07-15
        [Authorize(Roles = "admin,manager")]
        [HttpGet("date-range")]
        public async Task<ActionResult<ApiResponse<List<ReadSaleDto>>>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            var sales = await _saleService.GetByDateRangeAsync(from, to);
            return Ok(ApiResponse<List<ReadSaleDto>>.Ok(sales, APIMessages.SalesRetreived));
        }

        // GET: api/sale/product/{productId}
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<ApiResponse<List<ReadSaleDto>>>> GetByProductIdAsync(int productId)
        {
            var sales = await _saleService.GetByProductIdAsync(productId);
            return Ok(ApiResponse<List<ReadSaleDto>>.Ok(sales, APIMessages.SalesRetreived));
        }

        // GET: api/sale/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<List<ReadSaleDto>>>> GetByUserIdAsync(int userId)
        {
            var sales = await _saleService.GetByUserIdAsync(userId);
            return Ok(ApiResponse<List<ReadSaleDto>>.Ok(sales, APIMessages.SalesRetreived));
        }

        [Authorize(Roles = "admin,manager")]
        [HttpGet("product-report/{productId}")]
        public async Task<IActionResult> GetProductSalesReport(int productId)
        {
            var report = await _saleService.GetProductSalesReportAsync(productId);

            if (report == null || string.IsNullOrWhiteSpace(report.ProductName))
                return NotFound(ApiResponse<ProductSalesReportDto>.Fail(APIMessages.ProductNotFound));

            return Ok(ApiResponse<ProductSalesReportDto>.Ok(report, APIMessages.ProductReportGenerated));
        }


        // POST: api/sale
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ReadSaleDto>>> CreateAsync([FromBody] CreateSaleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ReadSaleDto>.Fail(APIMessages.InvalidSale));

            var sale = await _saleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = sale.Id },
                ApiResponse<ReadSaleDto>.Ok(sale, APIMessages.SaleCreated));
        }

        // PUT: api/sale/return/{saleId}

        [Authorize(Roles = "admin,manager,user")]
        [HttpPost("return/{saleId}")]
        public async Task<ActionResult<ApiResponse<string>>> RecordReturnAsync(int saleId)
        {
            var success = await _saleService.RecordReturnAsync(saleId);
            if (!success)
                return BadRequest(ApiResponse<string>.Fail(APIMessages.SaleNotReturned));

            return Ok(ApiResponse<string>.Ok(APIMessages.SaleReturned));
        }

        [HttpPost("complete/{saleId}")]
        public async Task<IActionResult> MarkAsCompleted(int saleId)
        {
            var success = await _saleService.MarkAsCompletedAsync(saleId);
            if (!success)
                return BadRequest(ApiResponse<string>.Fail(APIMessages.SaleNotCompleted));

            return Ok(ApiResponse<string>.Ok(null, APIMessages.SaleCompleted));
        }

        [HttpPost("cancel/{saleId}")]
        public async Task<IActionResult> CancelSale(int saleId)
        {
            var success = await _saleService.CancelAsync(saleId);
            if (!success)
                return BadRequest(ApiResponse<string>.Fail(APIMessages.SaleNotCancelled));

            return Ok(ApiResponse<string>.Ok(null, APIMessages.SaleCancelled));
        }
    }
}
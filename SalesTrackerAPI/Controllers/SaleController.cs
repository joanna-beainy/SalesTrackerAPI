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
    [Route("api/[controller]")]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;
        private readonly ILogger<SaleController> _logger;

        public SaleController(ISaleService saleService, ILogger<SaleController> logger)
        {
            _saleService = saleService;
            _logger = logger;
        }

        // GET: api/sale
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ReadSaleDto>>>> GetAllAsync()
        {
            _logger.LogInformation("Received request to fetch all sales");
            var sales = await _saleService.GetAllAsync();

            if (!sales.Any())
            {
                _logger.LogWarning("No sales found");
                return NotFound(ApiResponse<string>.Fail(APIMessages.SaleNotFound));
            }

            return Ok(ApiResponse<List<ReadSaleDto>>.Ok(sales, APIMessages.SalesRetreived));
        }

        // GET: api/sale/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ReadSaleDto>>> GetByIdAsync(int id)
        {
            _logger.LogInformation("Received request to fetch sale by ID: {SaleId}", id);
            var sale = await _saleService.GetByIdAsync(id);

            if (sale == null)
            {
                _logger.LogWarning("Sale not found: SaleId={SaleId}", id);
                return NotFound(ApiResponse<ReadSaleDto>.Fail(APIMessages.SaleNotFound));
            }

            return Ok(ApiResponse<ReadSaleDto>.Ok(sale, APIMessages.SaleRetreived));
        }

        // GET: api/sale/date-range?from=2025-07-01&to=2025-07-15
        [Authorize(Roles = "admin,manager")]
        [HttpGet("date-range")]
        public async Task<ActionResult<ApiResponse<List<ReadSaleDto>>>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            _logger.LogInformation("Received request for sales in date range {From}–{To}", from, to);
            var sales = await _saleService.GetByDateRangeAsync(from, to);
            return Ok(ApiResponse<List<ReadSaleDto>>.Ok(sales, APIMessages.SalesRetreived));
        }

        // GET: api/sale/product/{productId}
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<ApiResponse<List<ReadSaleDto>>>> GetByProductIdAsync(int productId)
        {
            _logger.LogInformation("Received request to fetch sales by ProductId: {ProductId}", productId);
            var sales = await _saleService.GetByProductIdAsync(productId);
            return Ok(ApiResponse<List<ReadSaleDto>>.Ok(sales, APIMessages.SalesRetreived));
        }

        // GET: api/sale/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<List<ReadSaleDto>>>> GetByUserIdAsync(int userId)
        {
            _logger.LogInformation("Received request to fetch sales by UserId: {UserId}", userId);
            var sales = await _saleService.GetByUserIdAsync(userId);
            return Ok(ApiResponse<List<ReadSaleDto>>.Ok(sales, APIMessages.SalesRetreived));
        }

        [Authorize(Roles = "admin,manager")]
        [HttpGet("product-report/{productId}")]
        public async Task<IActionResult> GetProductSalesReport(int productId)
        {
            _logger.LogInformation("Generating product sales report: ProductId={ProductId}", productId);
            var report = await _saleService.GetProductSalesReportAsync(productId);

            if (report == null || string.IsNullOrWhiteSpace(report.ProductName))
            {
                _logger.LogWarning("Product report failed — product not found: ProductId={ProductId}", productId);
                return NotFound(ApiResponse<ProductSalesReportDto>.Fail(APIMessages.ProductNotFound));
            }

            _logger.LogInformation("Product report generated successfully: ProductId={ProductId}, QuantitySold={Quantity}", productId, report.TotalQuantitySold);
            return Ok(ApiResponse<ProductSalesReportDto>.Ok(report, APIMessages.ProductReportGenerated));

        }


        // POST: api/sale
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ReadSaleDto>>> CreateAsync([FromBody] CreateSaleDto dto)
        {
            _logger.LogInformation("Received request to create sale for UserId: {UserId}", dto.UserId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid sale model submitted");
                return BadRequest(ApiResponse<ReadSaleDto>.Fail(APIMessages.InvalidSale));
            }

            var sale = await _saleService.CreateAsync(dto);
            _logger.LogInformation("Sale created with SaleId: {SaleId}", sale.Id);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = sale.Id },
                ApiResponse<ReadSaleDto>.Ok(sale, APIMessages.SaleCreated));
        }

        // PUT: api/sale/return/{saleId}

        [HttpPost("return/{saleId}")]
        [Authorize(Roles = "admin,manager,user")]
        public async Task<ActionResult<ApiResponse<string>>> RecordReturnAsync(int saleId)
        {
            _logger.LogInformation("Request to record return for SaleId: {SaleId}", saleId);

            var success = await _saleService.RecordReturnAsync(saleId);
            if (!success)
            {
                _logger.LogWarning("Sale return failed for SaleId: {SaleId}", saleId);
                return BadRequest(ApiResponse<string>.Fail(APIMessages.SaleNotReturned));
            }

            _logger.LogInformation("Sale return recorded for SaleId: {SaleId}", saleId);
            return Ok(ApiResponse<string>.Ok(APIMessages.SaleReturned));
        }

        [HttpPost("complete/{saleId}")]
        public async Task<IActionResult> MarkAsCompleted(int saleId)
        {
            _logger.LogInformation("Request to mark SaleId: {SaleId} as completed", saleId);

            var success = await _saleService.MarkAsCompletedAsync(saleId);
            if (!success)
            {
                _logger.LogWarning("Sale completion failed for SaleId: {SaleId}", saleId);
                return BadRequest(ApiResponse<string>.Fail(APIMessages.SaleNotCompleted));
            }

            _logger.LogInformation("Sale marked as completed: SaleId={SaleId}", saleId);
            return Ok(ApiResponse<string>.Ok(null, APIMessages.SaleCompleted));
        }

        [HttpPost("cancel/{saleId}")]
        public async Task<IActionResult> CancelSale(int saleId)
        {
            _logger.LogInformation("Request to cancel SaleId: {SaleId}", saleId);

            var success = await _saleService.CancelAsync(saleId);
            if (!success)
            {
                _logger.LogWarning("Sale cancellation failed for SaleId: {SaleId}", saleId);
                return BadRequest(ApiResponse<string>.Fail(APIMessages.SaleNotCancelled));
            }

            _logger.LogInformation("Sale cancelled: SaleId={SaleId}", saleId);
            return Ok(ApiResponse<string>.Ok(null, APIMessages.SaleCancelled));
        }
    }
}
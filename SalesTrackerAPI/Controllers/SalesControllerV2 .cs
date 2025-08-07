using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Interfaces;
using SalesTracker.Shared.Constants;
using SalesTracker.Shared.Responses;

namespace SalesTrackerAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/sales")]
    [ApiVersion("2.0")]
    public class SalesControllerV2 : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesControllerV2(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReadSaleV2Dto>>> GetAllSalesV2()
        {
            var sales = await _saleService.GetAllV2Async();
            return Ok(ApiResponse<List<ReadSaleV2Dto>>.Ok(sales, APIMessages.SalesRetreived));
        }
    }
}


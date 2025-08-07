using SalesTracker.Application.DTOs;

namespace SalesTracker.Application.Interfaces
{
    public interface ISaleService
    {
        Task<List<ReadSaleDto>> GetAllAsync();
        Task<ReadSaleDto?> GetByIdAsync(int id);
        Task<ReadSaleDto> CreateAsync(CreateSaleDto dto);

        Task<List<ReadSaleDto>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<List<ReadSaleDto>> GetByProductIdAsync(int productId);
        Task<List<ReadSaleDto>> GetByUserIdAsync(int userId);
        Task<ProductSalesReportDto> GetProductSalesReportAsync(int productId);

        Task<List<ReadSaleV2Dto>> GetAllV2Async();
        Task<bool> RecordReturnAsync(int saleId);
        Task<bool> MarkAsCompletedAsync(int saleId);
        Task<bool> CancelAsync(int saleId);
    }

}

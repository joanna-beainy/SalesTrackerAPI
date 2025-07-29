using SalesTracker.InfraStructure.Models.Entities;

namespace SalesTracker.InfraStructure.Interfaces
{
    public interface ISaleRepository
    {
        Task<List<Sale>> GetAllAsync();
        Task<Sale?> GetByIdAsync(int id);
        Task CreateAsync(Sale sale);

        Task<List<Sale>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<List<Sale>> GetByProductIdAsync(int productId);
        Task<List<Sale>> GetByUserIdAsync(int userId);
        Task<List<SaleItem>> GetProductSaleItemsAsync(int productId);


        Task<bool> RecordReturnAsync(int saleId);
        Task<bool> MarkAsCompletedAsync(int saleId);
        Task<bool> CancelAsync(int saleId);
    }

}

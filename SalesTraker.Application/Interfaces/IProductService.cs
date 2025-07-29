using SalesTracker.Application.DTOs;

namespace SalesTracker.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ReadProductDto>> GetAllAsync();
        Task<ReadProductDto?> GetByIdAsync(int id);
        Task AddAsync(AddProductDto dto);
        Task UpdateAsync(int id, UpdateProductDto dto);
        Task SoftDeleteAsync(int id);
        Task UpdateStockAsync(int id, UpdateStockDto dto);
        Task<List<string>> GetAllCategoriesAsync();

        Task<IEnumerable<ReadProductDto>> GetLowStockAsync();
        Task<IEnumerable<ReadProductDto>> SearchAsync(string keyword);
    }
}

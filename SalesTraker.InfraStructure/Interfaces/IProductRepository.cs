
using SalesTracker.InfraStructure.Models.Entities;


namespace SalesTracker.InfraStructure.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(int id, Product product);
        Task SoftDeleteAsync(int id);
        Task UpdateStockAsync(int id, int newStock);
        Task<IEnumerable<Product>> GetLowStockAsync();
        Task<IEnumerable<Product>> SearchAsync(String keyword);

    }
}

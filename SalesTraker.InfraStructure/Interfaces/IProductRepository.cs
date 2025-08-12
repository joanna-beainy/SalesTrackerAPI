
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
        Task UpdateImageAsync(int id, string imageUrl);
        Task<IEnumerable<Product>> GetLowStockAsync();
        Task<List<string>> GetAllCategoriesAsync();

        Task<IEnumerable<Product>> SearchAsync(String keyword);

        Task BulkInsertAsync(List<Product> products);

    }
}

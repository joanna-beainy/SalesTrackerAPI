using Microsoft.EntityFrameworkCore;
using SalesTracker.InfraStructure.Data;
using SalesTracker.InfraStructure.Models.Entities;
using SalesTracker.InfraStructure.Interfaces;

namespace SalesTracker.InfraStructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Where(p => p.IsActive).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return product?.IsActive == true ? product : null;
        }

        public async Task AddAsync(Product product)
        {
            product.IsActive = true;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, Product updated)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || !product.IsActive) return;

            product.Name = updated.Name;
            product.Category = updated.Category;
            product.Price = updated.Price;
            product.Stock = updated.Stock;

            await _context.SaveChangesAsync();
        }


        public async Task SoftDeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || !product.IsActive) return;

            product.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStockAsync(int id, int stock)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || !product.IsActive) return;

            product.Stock = stock;
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<Product>> GetLowStockAsync()
        {
            return await _context.Products.Where(p => p.IsActive && p.Stock <= 5).ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchAsync(string keyword)
        {
            return await _context.Products.Where(p => p.IsActive && (p.Name.Contains(keyword) || p.Category.Contains(keyword))).ToListAsync();
        }

    }
}

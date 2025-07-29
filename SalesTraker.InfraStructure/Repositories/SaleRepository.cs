using Microsoft.EntityFrameworkCore;
using SalesTracker.InfraStructure.Data;
using SalesTracker.InfraStructure.Interfaces;
using SalesTracker.InfraStructure.Models.Enums;
using SalesTracker.InfraStructure.Models.Entities;

namespace SalesTracker.InfraStructure.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly ApplicationDbContext _context;

        public SaleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sale>> GetAllAsync()
        {
            return await _context.Sales
                .Include(s => s.User)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .ToListAsync();
        }

        public async Task<Sale?> GetByIdAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.User)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Sale>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.Sales
                .Where(s => s.Date >= from && s.Date <= to)
                .Include(s => s.User)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .ToListAsync();
        }

        public async Task<List<Sale>> GetByProductIdAsync(int productId)
        {
            return await _context.Sales
                .Where(s => s.SaleItems.Any(si => si.ProductId == productId))
                .Include(s => s.User)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .ToListAsync();
        }

        public async Task<List<SaleItem>> GetProductSaleItemsAsync(int productId)
        {
            return await _context.SaleItems
                .Include(si => si.Product)
                .Where(si => si.ProductId == productId && si.Sale.Status == SaleStatus.Completed)
                .ToListAsync();
        }


        public async Task<List<Sale>> GetByUserIdAsync(int userId)
        {
            return await _context.Sales
                .Where(s => s.UserId == userId)
                .Include(s => s.User)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .ToListAsync();
        }

       


        public async Task CreateAsync(Sale sale)
        {
            await _context.Sales.AddAsync(sale);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RecordReturnAsync(int saleId)
        {
            var sale = await _context.Sales
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null || sale.Status != SaleStatus.Completed)
                return false;

            sale.Status = SaleStatus.Returned;

            foreach (var item in sale.SaleItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAsCompletedAsync(int saleId)
        {
            var sale = await _context.Sales.FindAsync(saleId);
            if (sale == null || sale.Status != SaleStatus.Pending)
                return false;

            sale.Status = SaleStatus.Completed;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelAsync(int saleId)
        {
            var sale = await _context.Sales
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null || sale.Status == SaleStatus.Completed)
                return false;

            sale.Status = SaleStatus.Cancelled;

            foreach (var item in sale.SaleItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity; 
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

    }

}


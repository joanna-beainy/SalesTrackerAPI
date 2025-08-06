using AutoMapper;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Interfaces;
using SalesTracker.InfraStructure.Interfaces;
using SalesTracker.InfraStructure.Models.Entities;
using SalesTracker.InfraStructure.Models.Enums;
using SalesTracker.InfraStructure.Repositories;

namespace SalesTracker.Application.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepo;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public SaleService(ISaleRepository saleRepo, IProductService productService, IMapper mapper)
        {
            _saleRepo = saleRepo;
            _productService = productService;
            _mapper = mapper;
        }

        public async Task<List<ReadSaleDto>> GetAllAsync()
        {
            var Sales = await _saleRepo.GetAllAsync();
            return _mapper.Map<List<ReadSaleDto>>(Sales);
        }

        public async Task<List<ReadSaleV2Dto>> GetAllV2Async()
        {
            var sales = await _saleRepo.GetAllAsync();
            return _mapper.Map<List<ReadSaleV2Dto>>(sales);
        }

        public async Task<ReadSaleDto?> GetByIdAsync(int id)
        {
            var sale = await _saleRepo.GetByIdAsync(id);
            return sale == null ? null : _mapper.Map<ReadSaleDto>(sale);
        }

        public async Task<ProductSalesReportDto> GetProductSalesReportAsync(int productId)
        {
            var saleItems = await _saleRepo.GetProductSaleItemsAsync(productId);

            var productName = saleItems.FirstOrDefault()?.Product?.Name ?? "Unknown";
            var quantitySold = saleItems.Sum(si => si.Quantity);

            return new ProductSalesReportDto
            {
                ProductId = productId,
                ProductName = productName,
                TotalQuantitySold = quantitySold
            };
        }


        public async Task<ReadSaleDto> CreateAsync(CreateSaleDto dto)
        {
            // Map basic Sale fields (UserId, Date), ignore SaleItems
            var sale = _mapper.Map<Sale>(dto);
            sale.SaleItems = new List<SaleItem>();
            sale.Status = SaleStatus.Pending;

            decimal total = 0;

            foreach (var itemDto in dto.SaleItems)
            {
                var product = await _productService.GetByIdAsync(itemDto.ProductId);
                if (product == null || product.Stock < itemDto.Quantity)
                    throw new Exception($"Product {itemDto.ProductId} is invalid or out of stock.");

                // Deduct stock
                var newStock = product.Stock - itemDto.Quantity;
                await _productService.UpdateStockAsync(product.Id, new UpdateStockDto { Stock = newStock });

                // Map SaleItem and enrich with UnitPrice
                var saleItem = _mapper.Map<SaleItem>(itemDto);
                saleItem.UnitPrice = product.Price;
                saleItem.DiscountPercentage = itemDto.DiscountPercentage;

                sale.SaleItems.Add(saleItem);
                total += saleItem.UnitPrice * saleItem.Quantity;
            }

            sale.TotalAmount = total;

            await _saleRepo.CreateAsync(sale);
            return _mapper.Map<ReadSaleDto>(sale);
        }

        public async Task<List<ReadSaleDto>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            var sales = await _saleRepo.GetByDateRangeAsync(from, to);
            return _mapper.Map<List<ReadSaleDto>>(sales);
        }

        public async Task<List<ReadSaleDto>> GetByProductIdAsync(int productId)
        {
            var sales = await _saleRepo.GetByProductIdAsync(productId);
            return _mapper.Map<List<ReadSaleDto>>(sales);
        }


        public async Task<List<ReadSaleDto>> GetByUserIdAsync(int userId)
        {
            var sales = await _saleRepo.GetByUserIdAsync(userId);
            return _mapper.Map<List<ReadSaleDto>>(sales);
        }


        public async Task<bool> RecordReturnAsync(int saleId)
        {
            return await _saleRepo.RecordReturnAsync(saleId);
        }

        public async Task<bool> MarkAsCompletedAsync(int saleId)
        {
            return await _saleRepo.MarkAsCompletedAsync(saleId);
        }

        public async Task<bool> CancelAsync(int saleId)
        {
            return await _saleRepo.CancelAsync(saleId);
        }
    }
}

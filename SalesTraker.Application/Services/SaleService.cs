using AutoMapper;
using Microsoft.Extensions.Logging;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Interfaces;
using SalesTracker.InfraStructure.Interfaces;
using SalesTracker.InfraStructure.Models.Entities;
using SalesTracker.InfraStructure.Models.Enums;
using SalesTracker.InfraStructure.Repositories;
using SalesTracker.Shared.Exceptions;

namespace SalesTracker.Application.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepo;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly ILogger<SaleService> _logger;

        public SaleService(ISaleRepository saleRepo, IProductService productService, IMapper mapper, ILogger<SaleService> logger)
        {
            _saleRepo = saleRepo;
            _productService = productService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ReadSaleDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all sales");
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
            _logger.LogInformation("Fetching sale by ID: {SaleId}", id);
            var sale = await _saleRepo.GetByIdAsync(id);
            return sale == null ? null : _mapper.Map<ReadSaleDto>(sale);
        }

        public async Task<ProductSalesReportDto> GetProductSalesReportAsync(int productId)
        {
            _logger.LogInformation("Generating product sales report for ProductId: {ProductId}", productId);
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
            _logger.LogInformation("Creating sale for UserId: {UserId}", dto.UserId);
            var sale = _mapper.Map<Sale>(dto);
            sale.SaleItems = new List<SaleItem>();
            sale.Status = SaleStatus.Pending;

            decimal total = 0;

            foreach (var itemDto in dto.SaleItems)
            {
                var product = await _productService.GetByIdAsync(itemDto.ProductId);
                if (product == null || product.Stock < itemDto.Quantity)
                {
                    _logger.LogWarning("Invalid product or insufficient stock: ProductId={ProductId}, Requested={Requested}, Stock={Stock}",
                        itemDto.ProductId, itemDto.Quantity, product?.Stock);
                    throw new AppException($"Product {itemDto.ProductId} is invalid or out of stock.");
                }

                // Deduct stock
                var newStock = product.Stock - itemDto.Quantity;
                await _productService.UpdateStockAsync(product.Id, new UpdateStockDto { Stock = newStock });

                var saleItem = _mapper.Map<SaleItem>(itemDto);
                saleItem.UnitPrice = product.Price;
                saleItem.DiscountPercentage = itemDto.DiscountPercentage;

                sale.SaleItems.Add(saleItem);
                total += saleItem.UnitPrice * saleItem.Quantity;
            }

            sale.TotalAmount = total;

            await _saleRepo.CreateAsync(sale);
            _logger.LogInformation("Sale created successfully. Total={Total}, SaleId={SaleId}", sale.TotalAmount, sale.Id);
            return _mapper.Map<ReadSaleDto>(sale);
        }

        public async Task<List<ReadSaleDto>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            _logger.LogInformation("Fetching sales from {From} to {To}", from, to);
            var sales = await _saleRepo.GetByDateRangeAsync(from, to);
            return _mapper.Map<List<ReadSaleDto>>(sales);
        }

        public async Task<List<ReadSaleDto>> GetByProductIdAsync(int productId)
        {
            _logger.LogInformation("Fetching sales by ProductId: {ProductId}", productId);
            var sales = await _saleRepo.GetByProductIdAsync(productId);
            return _mapper.Map<List<ReadSaleDto>>(sales);
        }


        public async Task<List<ReadSaleDto>> GetByUserIdAsync(int userId)
        {
            _logger.LogInformation("Fetching sales by UserId: {UserId}", userId);
            var sales = await _saleRepo.GetByUserIdAsync(userId);
            return _mapper.Map<List<ReadSaleDto>>(sales);
        }


        public async Task<bool> RecordReturnAsync(int saleId)
        {
            _logger.LogInformation("Recording return for SaleId: {SaleId}", saleId);
            return await _saleRepo.RecordReturnAsync(saleId);
        }

        public async Task<bool> MarkAsCompletedAsync(int saleId)
        {
            _logger.LogInformation("Marking sale as completed: SaleId={SaleId}", saleId);
            return await _saleRepo.MarkAsCompletedAsync(saleId);
        }

        public async Task<bool> CancelAsync(int saleId)
        {
            _logger.LogInformation("Cancelling sale: SaleId={SaleId}", saleId);
            return await _saleRepo.CancelAsync(saleId);
        }
    }
}

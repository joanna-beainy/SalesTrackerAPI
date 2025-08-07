using AutoMapper;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Logging;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Interfaces;
using SalesTracker.InfraStructure.Interfaces;
using SalesTracker.InfraStructure.Models.Entities;
using SalesTracker.InfraStructure.Repositories;
using SalesTracker.Shared.Responses;
using System.Text.Json;

namespace SalesTracker.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;
        private readonly IRedisCacheService _redisCache;
        private readonly ILogger<ProductService> _logger;
        public ProductService(IProductRepository repo, IMapper mapper, IRedisCacheService redisCache, ILogger<ProductService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _redisCache = redisCache;
            _logger = logger;
        }

        public async Task<IEnumerable<ReadProductDto>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all products");

            const string cacheKey = "products:all";
            IEnumerable<ReadProductDto> cached = null;

            try
            {
                cached = await _redisCache.GetAsync<IEnumerable<ReadProductDto>>(cacheKey);
                if (cached != null)
                {
                    _logger.LogInformation("Retrieved products from cache");
                    return cached;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Redis unavailable. Falling back to database. Error: {Message}", ex.Message);
            }

            var products = await _repo.GetAllAsync();
            var mapped = _mapper.Map<IEnumerable<ReadProductDto>>(products);

            try
            {
                await _redisCache.SetAsync(cacheKey, mapped, TimeSpan.FromMinutes(5));
                _logger.LogInformation("Products cached successfully");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to cache products. Redis might still be down. Error: {Message}", ex.Message);
            }

            return mapped;
        }



        public async Task<ReadProductDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching product by ID: {ProductId}", id);

            var product = await _repo.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product ID {ProductId} not found", id);
                return null;
            }

            return _mapper.Map<ReadProductDto>(product);
        }


        public async Task AddAsync(AddProductDto dto)
        {
            _logger.LogInformation("Creating product: {ProductName}", dto.Name);

            var product = _mapper.Map<Product>(dto);
            product.IsActive = true;

            await _repo.AddAsync(product);
            await _redisCache.RemoveAsync("products:all");

            _logger.LogInformation("Product added and cache invalidated: {ProductName}", product.Name);
        }

        public async Task UpdateAsync(int id, UpdateProductDto dto)
        {
            _logger.LogInformation("Updating product ID {ProductId}", id);

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null || !existing.IsActive)
            {
                _logger.LogWarning("Update skipped: Product ID {ProductId} not found or inactive", id);
                return;
            }

            var updatedProduct = _mapper.Map<Product>(dto);
            await _repo.UpdateAsync(id, updatedProduct);
            await _redisCache.RemoveAsync("products:all");

            _logger.LogInformation("Product ID {ProductId} updated and cache cleared", id);
        }

        public async Task SoftDeleteAsync(int id)
        {
            _logger.LogInformation("Soft deleting product ID {ProductId}", id);

            var product = await _repo.GetByIdAsync(id);
            if (product == null || !product.IsActive)
            {
                _logger.LogWarning("Soft delete skipped: Product ID {ProductId} not found or inactive", id);
                return;
            }

            product.IsActive = false;
            await _repo.UpdateAsync(id, product);
            await _redisCache.RemoveAsync("products:all");

            _logger.LogInformation("Product ID {ProductId} soft deleted and cache updated", id);
        }

        public async Task UpdateStockAsync(int id, UpdateStockDto dto)
        {
            _logger.LogInformation("Updating stock for product ID {ProductId} to {Stock}", id, dto.Stock);

            var product = await _repo.GetByIdAsync(id);
            if (product == null || !product.IsActive)
            {
                _logger.LogWarning("Stock update skipped: Product ID {ProductId} not found or inactive", id);
                return;
            }

            await _repo.UpdateStockAsync(id, dto.Stock);
            await _redisCache.RemoveAsync("products:all");

            _logger.LogInformation("Stock updated for product ID {ProductId} and cache cleared", id);
        }

        public async Task<List<string>> GetAllCategoriesAsync()
        {
           _logger.LogInformation("Retrieving all product categories");

            var categories = await _repo.GetAllCategoriesAsync();

            if (!categories.Any())
            {
                _logger.LogWarning("No product categories found");
            }

            return categories;
        }
        public async Task<IEnumerable<ReadProductDto>> GetLowStockAsync()
        {
            _logger.LogInformation("Fetching products with low stock");

            var products = await _repo.GetLowStockAsync();

            if (!products.Any())
            {
                _logger.LogWarning("No low-stock products found");
            }

            return _mapper.Map<IEnumerable<ReadProductDto>>(products);
        }

        public async Task<IEnumerable<ReadProductDto>> SearchAsync(string keyword)
        {
            _logger.LogInformation("Searching for products with keyword: {Keyword}", keyword);

            var products = await _repo.SearchAsync(keyword);

            if (!products.Any())
            {
                _logger.LogWarning("No products found for keyword: {Keyword}", keyword);
            }

            return _mapper.Map<IEnumerable<ReadProductDto>>(products);
        }

        public async Task<PaginatedResult<ReadProductDto>> GetPaginatedAsync(int page, int pageSize)
        {
            _logger.LogInformation("Fetching paginated products - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            var allProducts = await _repo.GetAllAsync();

            var skip = (page - 1) * pageSize;
            var items = allProducts.Skip(skip).Take(pageSize);
            var mapped = _mapper.Map<IEnumerable<ReadProductDto>>(items);

            return new PaginatedResult<ReadProductDto>(mapped, allProducts.Count(), page, pageSize);
        }

    }
}

using AutoMapper;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Interfaces;
using SalesTracker.InfraStructure.Interfaces;
using SalesTracker.InfraStructure.Models.Entities;
using SalesTracker.InfraStructure.Repositories;
using System.Text.Json;

namespace SalesTracker.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;
        private readonly IRedisCacheService _redisCache;
        public ProductService(IProductRepository repo, IMapper mapper, IRedisCacheService redisCache)
        {
            _repo = repo;
            _mapper = mapper;
            _redisCache = redisCache;
        }

        public async Task<IEnumerable<ReadProductDto>> GetAllAsync()
        {
            const string cacheKey = "products:all";
            var cached = await _redisCache.GetAsync<IEnumerable<ReadProductDto>>(cacheKey);

            if (cached != null) return cached;

            var products = await _repo.GetAllAsync();
            var mapped = _mapper.Map<IEnumerable<ReadProductDto>>(products);

            await _redisCache.SetAsync(cacheKey, mapped, TimeSpan.FromMinutes(5));
            return mapped;
        }


        public async Task<ReadProductDto?> GetByIdAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ReadProductDto>(product);
        }


        public async Task AddAsync(AddProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            product.IsActive = true;
            await _repo.AddAsync(product);
            await _redisCache.RemoveAsync("products:all");
        }

        public async Task UpdateAsync(int id, UpdateProductDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null || !existing.IsActive) return;

            var updatedProduct = _mapper.Map<Product>(dto);

            await _repo.UpdateAsync(id, updatedProduct);
            await _redisCache.RemoveAsync("products:all");
        }

        public async Task SoftDeleteAsync(int id)
        {
            await _repo.SoftDeleteAsync(id);
            await _redisCache.RemoveAsync("products:all");
        }

        public async Task UpdateStockAsync(int id, UpdateStockDto dto)
        {
            await _repo.UpdateStockAsync(id, dto.Stock);
            await _redisCache.RemoveAsync("products:all");
        }

        public async Task<IEnumerable<ReadProductDto>> GetLowStockAsync()
        {
            var products = await _repo.GetLowStockAsync();
            return _mapper.Map<IEnumerable<ReadProductDto>>(products);
        }

        public async Task<IEnumerable<ReadProductDto>> SearchAsync(string keyword)
        {
            var products = await _repo.SearchAsync(keyword);
            return _mapper.Map<IEnumerable<ReadProductDto>>(products);
        }
    }
}

using AutoMapper;
using Moq;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Services;
using SalesTracker.InfraStructure.Interfaces;
using SalesTracker.InfraStructure.Models.Entities; 

namespace SalesTracker.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repoMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new ProductService(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnProducts()
        {
            // Arrange
            var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", IsActive = true },
            new Product { Id = 2, Name = "Mouse", IsActive = true }
        };

            var productDtos = new List<ReadProductDto>
        {
            new ReadProductDto { Id = 1, Name = "Laptop" },
            new ReadProductDto { Id = 2, Name = "Mouse" }
        };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
            _mapperMock.Setup(m => m.Map<IEnumerable<ReadProductDto>>(products)).Returns(productDtos);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(productDtos, result);
        }


        [Fact]
        public async Task GetByIdAsync_ShouldReturnProductDto_WhenProductExists()
        {
            // Arrange
            var productId = 1;

            var product = new Product { Id = productId, Name = "Tablet", IsActive = true };
            var productDto = new ReadProductDto { Id = productId, Name = "Tablet" };

            _repoMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ReadProductDto>(product)).Returns(productDto);

            // Act
            var result = await _service.GetByIdAsync(productId);

            // Assert
            Assert.Equal(productDto, result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            var productId = 99;

            _repoMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

            var result = await _service.GetByIdAsync(productId);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ShouldMapDtoAndCallAddAsyncOnce()
        {
            var dto = new AddProductDto { Name = "Monitor", Price = 299 };
            var entity = new Product { Name = "Monitor", Price = 299 };

            _mapperMock.Setup(m => m.Map<Product>(dto)).Returns(entity);
            _repoMock.Setup(r => r.AddAsync(entity)).Returns(Task.CompletedTask);

            await _service.AddAsync(dto);

            _mapperMock.Verify(m => m.Map<Product>(dto), Times.Once);
            _repoMock.Verify(r => r.AddAsync(entity), Times.Once);
        }


        [Fact]
        public async Task SoftDeleteAsync_ShouldCallRepositorySoftDeleteOnce()
        {
            // Arrange
            var productId = 1;

            _repoMock.Setup(r => r.SoftDeleteAsync(productId)).Returns(Task.CompletedTask);

            // Act
            await _service.SoftDeleteAsync(productId);

            // Assert
            _repoMock.Verify(r => r.SoftDeleteAsync(productId), Times.Once);
        }

    }
}



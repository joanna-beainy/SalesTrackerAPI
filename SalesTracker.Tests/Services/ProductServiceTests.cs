using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Interfaces;
using SalesTracker.Application.Services;
using SalesTracker.InfraStructure.Interfaces;
using SalesTracker.InfraStructure.Models.Entities; 

namespace SalesTracker.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRedisCacheService> _redisMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly Mock<IAzureBlobStorageService> _blobStorageMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repoMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _redisMock = new Mock<IRedisCacheService>();
            _loggerMock = new Mock<ILogger<ProductService>>();
            _blobStorageMock = new Mock<IAzureBlobStorageService>();

            _service = new ProductService(
                _repoMock.Object,
                _mapperMock.Object,
                _redisMock.Object,
                _loggerMock.Object,
                _blobStorageMock.Object,
                null
            );
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
        public async Task SoftDeleteAsync_ShouldDeleteBlobAndUpdateProduct()
        {
            var productId = 1;
            var imageUrl = "https://storage.blob.core.windows.net/images/products/1/image.jpg";
            var blobPath = "products/1/image.jpg";

            var product = new Product { Id = productId, Name = "Keyboard", IsActive = true, ImageUrl = imageUrl };

            _repoMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
            _blobStorageMock.Setup(b => b.ExtractBlobPath(imageUrl)).Returns(blobPath);
            _blobStorageMock.Setup(b => b.DeleteAsync(blobPath)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.UpdateAsync(productId, It.IsAny<Product>())).Returns(Task.CompletedTask);
            _redisMock.Setup(r => r.RemoveAsync("products:all")).Returns(Task.CompletedTask);

            await _service.SoftDeleteAsync(productId);

            _blobStorageMock.Verify(b => b.DeleteAsync(blobPath), Times.Once);
            _repoMock.Verify(r => r.UpdateAsync(productId, It.Is<Product>(p => !p.IsActive)), Times.Once);
            _redisMock.Verify(r => r.RemoveAsync("products:all"), Times.Once);
        }

    }
}



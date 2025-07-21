using AutoMapper;
using Moq;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Interfaces;
using SalesTracker.Application.Services;
using SalesTracker.InfraStructure.Interfaces;
using SalesTracker.InfraStructure.Models.Enums;
using SalesTracker.InfraStructure.Models.Entities;

namespace SalesTracker.Tests.Services
{
    public class SaleServiceTests
    {
        private readonly Mock<ISaleRepository> _saleRepoMock = new();
        private readonly Mock<IProductService> _productServiceMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        private readonly SaleService _saleService;

        public SaleServiceTests()
        {
            _saleService = new SaleService(_saleRepoMock.Object, _productServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsSaleDto_WhenFound()
        {
            var sale = new Sale { Id = 1, UserId = 1, Status = SaleStatus.Completed };
            var expectedDto = new ReadSaleDto { Id = 1 };

            _saleRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sale);
            _mapperMock.Setup(m => m.Map<ReadSaleDto>(sale)).Returns(expectedDto);

            var result = await _saleService.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task RecordReturnAsync_ReturnsTrue_WhenSaleCanBeReturned()
        {
            _saleRepoMock.Setup(r => r.RecordReturnAsync(1))
                .ReturnsAsync(true);

            var result = await _saleService.RecordReturnAsync(1);

            Assert.True(result);
            _saleRepoMock.Verify(r => r.RecordReturnAsync(1), Times.Once);
        }


        [Fact]
        public async Task MarkAsCompletedAsync_ReturnsTrue_WhenSaleIsPending()
        {
            _saleRepoMock.Setup(r => r.MarkAsCompletedAsync(1))
                .ReturnsAsync(true);

            var result = await _saleService.MarkAsCompletedAsync(1);

            Assert.True(result);
            _saleRepoMock.Verify(r => r.MarkAsCompletedAsync(1), Times.Once);
        }



        [Fact]
        public async Task CancelAsync_ReturnsTrue_WhenSaleIsNotCompleted()
        {
            _saleRepoMock.Setup(r => r.CancelAsync(1))
                .ReturnsAsync(true);

            var result = await _saleService.CancelAsync(1);

            Assert.True(result);
            _saleRepoMock.Verify(r => r.CancelAsync(1), Times.Once);
        }


        [Fact]
        public async Task CreateAsync_ReturnsMappedDto_AfterSuccessfulCreation()
        {
            var dto = new CreateSaleDto
            {
                UserId = 1,
                SaleItems = new List<CreateSaleItemDto>
                {
                    new CreateSaleItemDto { ProductId = 5, Quantity = 2 }
                }
            };

            // Mocked product from ProductService
            var productDto = new ReadProductDto { Id = 5, Name = "Laptop", Price = 100m, Stock = 10 };
            var productEntity = new Product { Id = 5, Price = 100m, Stock = 10 };

            // Expected sale entity and DTO after creation
            var saleEntity = new Sale { Id = 1, UserId = 1 };
            var saleItemEntity = new SaleItem { ProductId = 5, Quantity = 2 };
            var expectedDto = new ReadSaleDto { Id = 1 };

            // Mocking external calls
            _productServiceMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync(productDto);
            _productServiceMock.Setup(s => s.UpdateStockAsync(5, It.IsAny<UpdateStockDto>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<Sale>(dto)).Returns(saleEntity);
            _mapperMock.Setup(m => m.Map<SaleItem>(It.IsAny<CreateSaleItemDto>())).Returns(saleItemEntity);
            _saleRepoMock.Setup(r => r.CreateAsync(It.IsAny<Sale>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<ReadSaleDto>(It.IsAny<Sale>())).Returns(expectedDto);

            var result = await _saleService.CreateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(expectedDto.Id, result.Id);

            _productServiceMock.Verify(s => s.UpdateStockAsync(5, It.Is<UpdateStockDto>(d => d.Stock == 8)), Times.Once);
            _saleRepoMock.Verify(r => r.CreateAsync(It.IsAny<Sale>()), Times.Once);
        }

    }
}

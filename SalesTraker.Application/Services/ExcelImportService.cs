using AutoMapper;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Interfaces;
using SalesTracker.InfraStructure.Interfaces;
using SalesTracker.InfraStructure.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.Application.Services
{
    public class ExcelImportService : IExcelImportService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ExcelImportService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<ReadProductDto>> ImportProductsFromExcelAsync(IFormFile file)
        {
            var products = new List<Product>();

            ExcelPackage.License.SetNonCommercialPersonal("Joanna");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.First();

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                var name = worksheet.Cells[row, 1].Text;
                var price = decimal.Parse(worksheet.Cells[row, 2].Text);
                var stock = int.Parse(worksheet.Cells[row, 3].Text);
                var category = worksheet.Cells[row, 4].Text;

                products.Add(new Product
                {
                    Name = name,
                    Price = price,
                    Stock = stock,
                    Category = category
                });
            }

            await _productRepository.BulkInsertAsync(products);
            return _mapper.Map<List<ReadProductDto>>(products);

        }
    }
}

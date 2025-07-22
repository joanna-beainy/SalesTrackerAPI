using SalesTracker.Application.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.Application.Interfaces
{
    public interface IExcelImportService
    {
        Task<List<ReadProductDto>> ImportProductsFromExcelAsync(IFormFile file);
    }

}
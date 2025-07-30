using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.Application.DTOs
{
    public class DailySalesDto
    {
        public DateTime SaleDay { get; set; }
        public int ProductId { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}

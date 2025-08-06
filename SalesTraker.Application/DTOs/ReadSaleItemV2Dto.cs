using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.Application.DTOs
{
    public class ReadSaleItemV2Dto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public double DiscountPercentage { get; set; }

        public decimal TotalPriceAfterDiscount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.InfraStructure.Responses
{
    public class DailySalesData
    {
        public decimal TotalSales { get; set; }
        public int QuantitySold { get; set; }
        public string TopProductName { get; set; } = string.Empty;
        public int TopProductQuantity { get; set; }
    }
}

using SalesTracker.InfraStructure.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.Application.DTOs
{
    public class ReadSaleV2Dto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }

        public SaleStatus Status { get; set; }

        public List<ReadSaleItemV2Dto> SaleItems { get; set; }
    }
}

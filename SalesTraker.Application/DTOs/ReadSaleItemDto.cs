﻿
namespace SalesTracker.Application.DTOs
{
    public class ReadSaleItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
    }

}

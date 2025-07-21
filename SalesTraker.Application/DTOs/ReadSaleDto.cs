using SalesTracker.InfraStructure.Models.Enums;

namespace SalesTracker.Application.DTOs
{
    public class ReadSaleDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }

        public SaleStatus Status { get; set; }

        public List<ReadSaleItemDto> SaleItems { get; set; }
    }

}

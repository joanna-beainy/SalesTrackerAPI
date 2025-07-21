
namespace SalesTracker.Application.DTOs
{
    public class CreateSaleDto
    {
        public int UserId { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public List<CreateSaleItemDto> SaleItems { get; set; }


    }

}

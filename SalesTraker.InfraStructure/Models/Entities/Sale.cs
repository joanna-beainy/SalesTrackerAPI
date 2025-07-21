using SalesTracker.InfraStructure.Models.Enums;

namespace SalesTracker.InfraStructure.Models.Entities
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<SaleItem> SaleItems { get; set; }

        public SaleStatus Status { get; set; } = SaleStatus.Completed;
    }

}

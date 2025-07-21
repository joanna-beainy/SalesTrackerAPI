namespace SalesTracker.InfraStructure.Models.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }  // 'admin', 'cashier'
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<User> Users { get; set; }
    }

}

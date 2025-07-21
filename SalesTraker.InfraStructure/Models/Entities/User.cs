
namespace SalesTracker.InfraStructure.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Sale> Sales { get; set; }
    }

}

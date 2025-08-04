namespace ShiftRotaManager.Data.Models
{
    public class Role
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty; // Admin, User, Reader

        // Navigation property for user roles
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
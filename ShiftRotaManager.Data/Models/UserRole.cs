namespace ShiftRotaManager.Data.Models
{
    public class UserRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Foreign keys to TeamMember and Role
        public Guid TeamMemberId { get; set; }
        public TeamMember TeamMember { get; set; } = null!;

        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
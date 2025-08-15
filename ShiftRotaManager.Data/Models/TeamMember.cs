using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftRotaManager.Data.Models
{
    public class TeamMember
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PreferredDaysOfWeek { get; set; }
        

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        // Foreign key for Role
        public Guid RoleId { get; set; }
        public Guid ShiftId { get; set; }

        // Navigation property for roles
        [ForeignKey("RoleId")]
        public Role? Role { get; set; } // Navigation property

        [ForeignKey("ShiftId")]
        public Shift? PreferredShift { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        // Navigation property for rotas
        public ICollection<Rota> Rotas { get; set; } = new List<Rota>();
        // Navigation property for leave/time off
        public ICollection<AnnualLeave> AnnualLeaves { get; set; } = new List<AnnualLeave>();
        public ICollection<TimeOffInLieu> TimeOffInLieu { get; set; } = new List<TimeOffInLieu>();
        public ICollection<Overtime> OvertimeRecords { get; set; } = new List<Overtime>();
        public ICollection<Illness> IllnessRecords { get; set; } = new List<Illness>();
    }
}
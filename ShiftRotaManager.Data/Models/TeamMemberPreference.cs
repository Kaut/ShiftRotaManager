using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftRotaManager.Data.Models
{
    /// <summary>
    /// Represents a team member's preference for a specific shift on a specific day of the week.
    /// </summary>
    public class TeamMemberPreference
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DayOfWeek DayOfWeek { get; set; }

        // Foreign key to TeamMember
        public Guid TeamMemberId { get; set; }
        [ForeignKey("TeamMemberId")]
        public TeamMember TeamMember { get; set; }

        // Foreign key to the preferred Shift for that day
        public Guid ShiftId { get; set; }
        [ForeignKey("ShiftId")]
        public Shift Shift { get; set; }
    }
}
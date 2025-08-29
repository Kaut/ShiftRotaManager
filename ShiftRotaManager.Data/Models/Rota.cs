using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftRotaManager.Data.Models
{
    public class Rota
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public RotaStatus Status { get; set; } // Assigned, Open, Covered, Leave, Illness

        // Foreign keys to Shift and TeamMember
        public Guid ShiftId { get; set; }
        public Shift Shift { get; set; } = null!; // Navigation property

        public Guid? TeamMemberId { get; set; } // Nullable for open shifts
        public TeamMember? TeamMember { get; set; } // Navigation property
        public ICollection<TeamMember> PairedTeamMembers { get; set; } = new List<TeamMember>();

        [NotMapped]
        [Display(Name = "Paired Team Members")]
        public List<Guid> SelectedPairedTeamMemberIds { get; set; } = new List<Guid>();
    }

    public enum RotaStatus
    {
        Assigned,
        Open,
        Covered,
        Leave,
        Illness
    }
}
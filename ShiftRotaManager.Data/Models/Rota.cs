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

        // For pairing new starters
        public Guid? PairedTeamMemberId { get; set; }
        public TeamMember? PairedTeamMember { get; set; } // Navigation property
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
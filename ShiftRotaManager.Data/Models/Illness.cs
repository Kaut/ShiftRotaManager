namespace ShiftRotaManager.Data.Models
{
    public class Illness
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TeamMemberId { get; set; }
        public TeamMember TeamMember { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
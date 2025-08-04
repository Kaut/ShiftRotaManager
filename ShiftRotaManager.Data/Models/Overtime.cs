namespace ShiftRotaManager.Data.Models
{
    public class Overtime
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TeamMemberId { get; set; }
        public TeamMember TeamMember { get; set; } = null!;
        public DateTime Date { get; set; }
        public double Hours { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
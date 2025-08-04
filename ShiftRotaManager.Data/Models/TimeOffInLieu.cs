namespace ShiftRotaManager.Data.Models
{
    public class TimeOffInLieu
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TeamMemberId { get; set; }
        public TeamMember TeamMember { get; set; } = null!;
        public DateTime Date { get; set; }
        public double HoursAccrued { get; set; }
        public double HoursUsed { get; set; }
    }
}
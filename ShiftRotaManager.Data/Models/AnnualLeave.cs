namespace ShiftRotaManager.Data.Models
{
    public class AnnualLeave
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TeamMemberId { get; set; }
        public TeamMember TeamMember { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveStatus Status { get; set; } // Pending, Approved, Rejected
    }

    public enum LeaveStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
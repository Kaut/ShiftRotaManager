namespace ShiftRotaManager.Web.Models;

public class RotaAssignmentModel
{
    public DateTime Date { get; set; }
    public Guid ShiftId { get; set; }
    public Guid TeamMemberId { get; set; }
}
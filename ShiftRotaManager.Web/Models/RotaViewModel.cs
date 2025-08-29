using System;

namespace ShiftRotaManager.Web.Models;

public class RotaViewModel
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public Guid ShiftId { get; set; }
    public Guid? TeamMemberId { get; set; }
    public List<Guid> SelectedPairedTeamMemberIds { get; set; } = [];
}

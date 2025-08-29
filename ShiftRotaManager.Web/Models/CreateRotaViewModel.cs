using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShiftRotaManager.Web.Models;

public class CreateRotaViewModel
{
    [Required(ErrorMessage = "Start Date is required")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.Now.Date;
    [Required(ErrorMessage = "End Date is required")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = DateTime.Now.Date.AddDays(7);
    [Required(ErrorMessage = "Shift is required")]
    public Guid ShiftId { get; set; }
    public Guid? TeamMemberId { get; set; }
    public ICollection<Guid> SelectedPairedTeamMemberIds { get; set; } = [];
    public bool IsOpenShift { get; set; }

    // Dropdowns for the view
    public IEnumerable<SelectListItem>? Shifts { get; set; }
    public IEnumerable<SelectListItem>? TeamMembers { get; set; }

    public Dictionary<string, List<RotaAssignmentModel>>? SuggestedRotas { get; set; }
}

using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShiftRotaManager.Web.Models;
public class TeamMemberViewModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public Guid RoleId { get; set; }

    /// <summary>
    /// A dictionary to hold the preferred shift for each day of the week.
    /// The key is the DayOfWeek, and the value is the nullable ShiftId.
    /// A null value indicates no preference for that day.
    /// This structure is ideal for model binding from a form.
    /// </summary>
    public Dictionary<DayOfWeek, Guid?> PreferredShifts { get; set; } = new();

    // These properties are used to populate the dropdowns in the associated view.
    // They are not part of the data submitted back to the server.
    public SelectList? Roles { get; set; }
    public SelectList? Shifts { get; set; }
}


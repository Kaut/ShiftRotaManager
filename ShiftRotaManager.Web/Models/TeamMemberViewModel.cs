namespace ShiftRotaManager.Web.Models;
public class TeamMemberViewModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string[] PreferredDaysOfWeek { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public Guid RoleId { get; set; }
    public Guid ShiftId { get; set; }
}
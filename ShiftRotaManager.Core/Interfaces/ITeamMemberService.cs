using ShiftRotaManager.Data.Models;
    
namespace ShiftRotaManager.Core.Interfaces
{
    public interface ITeamMemberService
    {
        Task<IEnumerable<TeamMember>> GetAllTeamMembersAsync();
        Task<TeamMember?> GetTeamMemberByIdAsync(Guid id);
        Task AddTeamMemberAsync(TeamMember teamMember);
        Task UpdateTeamMemberAsync(TeamMember teamMember);
        Task DeleteTeamMemberAsync(Guid id);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<ICollection<TeamMember>> GetTeamMembersByIdsAsync(List<Guid> pairedTeamMemberIds);
        Task<ICollection<TeamMember>> GetTeamMembersByPreferredDaysAsync(List<string> days);
    }
}
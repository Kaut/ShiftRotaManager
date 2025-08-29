using ShiftRotaManager.Data.Models;

namespace ShiftRotaManager.Data.Repositories
{
    public interface ITeamMemberRepository : IGenericRepository<TeamMember>
    {
        Task<IEnumerable<TeamMember>> GetTeamMembersWithRolesAsync();
        Task<TeamMember?> GetTeamMemberByIdWithDetailsAsync(Guid id);
        Task<ICollection<TeamMember>> GetTeamMembersByPreferredDaysAsync(string days);
    }
}
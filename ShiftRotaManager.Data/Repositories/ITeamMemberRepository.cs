using ShiftRotaManager.Data.Models;

namespace ShiftRotaManager.Data.Repositories
{
    public interface ITeamMemberRepository : IGenericRepository<TeamMember>
    {
        Task<IEnumerable<TeamMember>> GetTeamMembersWithRolesAsync();
    }
}
using ShiftRotaManager.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShiftRotaManager.Core.Interfaces
{
    public interface ITeamMemberService
    {
        Task<IEnumerable<TeamMember>> GetAllTeamMembersAsync();
        Task<TeamMember?> GetTeamMemberByIdAsync(Guid id);
        Task AddTeamMemberAsync(TeamMember teamMember, Guid roleId);
        Task UpdateTeamMemberAsync(TeamMember teamMember);
        Task DeleteTeamMemberAsync(Guid id);
        Task<IEnumerable<Role>> GetAllRolesAsync();
    }
}
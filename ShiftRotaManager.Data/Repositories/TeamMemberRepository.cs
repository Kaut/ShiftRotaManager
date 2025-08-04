using Microsoft.EntityFrameworkCore;
using ShiftRotaManager.Data.Data;
using ShiftRotaManager.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShiftRotaManager.Data.Repositories
{
    public class TeamMemberRepository : GenericRepository<TeamMember>, ITeamMemberRepository
    {
        public TeamMemberRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TeamMember>> GetTeamMembersWithRolesAsync()
        {
            return await _dbSet.Include(tm => tm.UserRoles).ThenInclude(ur => ur.Role).ToListAsync();
        }
    }
}
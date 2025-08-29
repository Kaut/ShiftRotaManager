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
            // The previous query was loading the many-to-many relationship via UserRoles,
            // but the view and other parts of the application expect the direct 'Role'
            // navigation property to be loaded. This change ensures the correct
            // navigation property is eagerly loaded.
            return await _dbSet.Include(tm => tm.Role).Include(tm => tm.PreferredShift).ToListAsync();
        }
        public async Task<TeamMember?> GetTeamMemberByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(tm => tm.Role) 
                .FirstOrDefaultAsync(tm => tm.Id == id);
        }

        public async Task<ICollection<TeamMember>> GetTeamMembersByPreferredDaysAsync(string days)
        {
            return await _dbSet.Where(t => t.PreferredDaysOfWeek.Contains(days)).ToListAsync();
        }
    }
}
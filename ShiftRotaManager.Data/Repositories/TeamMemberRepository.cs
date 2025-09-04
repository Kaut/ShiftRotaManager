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
            // Eagerly load the Role and the new Preferences collection with their associated Shifts.
            // The PreferredShift property has been removed and replaced by the Preferences collection.
            return await _dbSet
                .Include(tm => tm.Role)
                .Include(tm => tm.Preferences)
                    .ThenInclude(p => p.Shift)
                .ToListAsync();
        }
        public async Task<TeamMember?> GetTeamMemberByIdWithDetailsAsync(Guid id)
        {
            // Also include the new Preferences collection for the details view.
            return await _dbSet
                .Include(tm => tm.Role)
                .Include(tm => tm.Preferences)
                    .ThenInclude(p => p.Shift)
                .FirstOrDefaultAsync(tm => tm.Id == id);
        }

        public async Task<ICollection<TeamMember>> GetTeamMembersByPreferredDayAsync(DayOfWeek day)
        {
            // Find members who have a preference for the given day of the week.
            // Eagerly load the Preferences and the associated Shift to be used in the recommendations.
            return await _dbSet
                .Where(t => t.Preferences.Any(p => p.DayOfWeek == day))
                .Include(t => t.Preferences.Where(p => p.DayOfWeek == day))
                    .ThenInclude(p => p.Shift)
                .ToListAsync();
        }

        public void RemovePreference(TeamMemberPreference preference)
        {
            // Attach the preference if not already tracked
            if (_context.Entry(preference).State == EntityState.Detached)
            {
                _context.Set<TeamMemberPreference>().Attach(preference);
            }
            _context.Set<TeamMemberPreference>().Remove(preference);
            // Note: SaveChanges should be called by the service after all removals/additions
        }
    }
}
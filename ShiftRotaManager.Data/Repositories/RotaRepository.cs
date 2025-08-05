using Microsoft.EntityFrameworkCore;
using ShiftRotaManager.Data.Data;
using ShiftRotaManager.Data.Models;

namespace ShiftRotaManager.Data.Repositories
{
    public class RotaRepository : GenericRepository<Rota>, IRotaRepository
    {
        public RotaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Rota>> GetAllRotasWithDetailsAsync()
        {
            return await _dbSet
                .Include(r => r.Shift)
                .Include(r => r.TeamMember)
                .Include(r => r.PairedTeamMember)
                .OrderBy(r => r.Date) // Order by date for better viewing
                .ToListAsync();
        }

        public async Task<Rota?> GetRotaByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(r => r.Shift)
                .Include(r => r.TeamMember)
                .Include(r => r.PairedTeamMember)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Rota>> GetRotasByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? teamMemberId = null, Guid? shiftId = null, RotaStatus? status = null)
        {
            var query = _dbSet
                .Where(r => r.Date >= startDate.Date && r.Date <= endDate.Date)
                .Include(r => r.Shift)
                .Include(r => r.TeamMember)
                .Include(r => r.PairedTeamMember);

            if (teamMemberId.HasValue)
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Rota, TeamMember?>)query.Where(r => r.TeamMemberId == teamMemberId.Value || r.PairedTeamMemberId == teamMemberId.Value);
            }

            if (shiftId.HasValue)
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Rota, TeamMember?>)query.Where(r => r.ShiftId == shiftId.Value);
            }

            if (status.HasValue)
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Rota, TeamMember?>)query.Where(r => r.Status == status.Value);
            }

            return await query
                .OrderBy(r => r.Date)
                .ThenBy(r => r.Shift.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rota>> GetRotasByTeamMemberIdAsync(Guid teamMemberId)
        {
            return await _dbSet
                .Where(r => r.TeamMemberId == teamMemberId || r.PairedTeamMemberId == teamMemberId)
                .Include(r => r.Shift)
                .Include(r => r.TeamMember)
                .Include(r => r.PairedTeamMember)
                .OrderBy(r => r.Date)
                .ThenBy(r => r.Shift.StartTime)
                .ToListAsync();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using ShiftRotaManager.Data.Data;
using ShiftRotaManager.Data.Models;
using System.Linq.Expressions;
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
                .Include(r => r.PairedTeamMembers)
                .OrderBy(r => r.Date) // Order by date for better viewing
                .ToListAsync();
        }

        public async Task<Rota?> GetRotaByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(r => r.Shift)
                .Include(r => r.TeamMember)
                .Include(r => r.PairedTeamMembers)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Rota>> GetRotasByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var rotas = await _dbSet
                .Where(r => r.Date >= startDate.Date && r.Date <= endDate.Date)
                .Include(r => r.Shift)
                .Include(r => r.TeamMember)
                .Include(r => r.PairedTeamMembers)
                .ToListAsync();

            // The ThenBy clause on a navigation property's TimeSpan is not translatable to SQL for SQLite.
            // We fetch the data first and then perform the sorting in-memory.
            return rotas.OrderBy(r => r.Date).ThenBy(r => r.Shift.StartTime);
        }

        public async Task<IEnumerable<Rota>> GetRotasByTeamMemberIdAsync(Guid teamMemberId)
        {
            var rotas = await _dbSet
                .Where(r => r.TeamMemberId == teamMemberId
                    || r.PairedTeamMembers.Any(x => x.Id == teamMemberId))
                .Include(r => r.Shift)
                .Include(r => r.TeamMember)
                .Include(r => r.PairedTeamMembers)
                .ToListAsync();

            // The ThenBy clause on a navigation property's TimeSpan is not translatable to SQL for SQLite.
            // We fetch the data first and then perform the sorting in-memory.
            return rotas.OrderBy(r => r.Date).ThenBy(r => r.Shift.StartTime);
        }

        public async Task<int> CountAsync(Expression<Func<Rota, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using ShiftRotaManager.Data.Data;
using ShiftRotaManager.Data.Models;

namespace ShiftRotaManager.Data.Repositories
{
    public class ShiftRepository : GenericRepository<Shift>, IShiftRepository
    {
        public ShiftRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Shift>> GetShiftsWithVariantsAsync()
        {
            return await _dbSet.Include(s => s.Variants).ToListAsync();
        }
    }
}
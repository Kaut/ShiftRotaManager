using ShiftRotaManager.Data.Models;

namespace ShiftRotaManager.Data.Repositories
{
    public interface IShiftRepository : IGenericRepository<Shift>
    {
        // Add specific shift-related methods here if needed
        Task<IEnumerable<Shift>> GetShiftsWithVariantsAsync();
    }
}
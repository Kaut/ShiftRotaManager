using ShiftRotaManager.Data.Models;

namespace ShiftRotaManager.Data.Repositories
{
    public interface IRotaRepository : IGenericRepository<Rota>
    {
        Task<IEnumerable<Rota>> GetAllRotasWithDetailsAsync();
        Task<Rota?> GetRotaByIdWithDetailsAsync(Guid id);
        // Updated: Added optional filter parameters
        Task<IEnumerable<Rota>> GetRotasByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? teamMemberId = null, Guid? shiftId = null, RotaStatus? status = null);
        Task<IEnumerable<Rota>> GetRotasByTeamMemberIdAsync(Guid teamMemberId);
    }
}
using ShiftRotaManager.Data.Models;

namespace ShiftRotaManager.Data.Repositories
{
    public interface IRotaRepository : IGenericRepository<Rota>
    {
        Task<IEnumerable<Rota>> GetAllRotasWithDetailsAsync();
        Task<Rota?> GetRotaByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<Rota>> GetRotasByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Rota>> GetRotasByTeamMemberIdAsync(Guid teamMemberId);
    }
}
using ShiftRotaManager.Data.Models;

namespace ShiftRotaManager.Core.Interfaces
{
    public interface IRotaService
    {
        Task<IEnumerable<Rota>> GetAllRotasAsync();
        Task<Rota?> GetRotaByIdAsync(Guid id);
        Task AddRotaAsync(Rota rota);
        Task UpdateRotaAsync(Rota rota);
        Task DeleteRotaAsync(Guid id);
        Task AssignOpenShiftAsync(Guid rotaId, Guid teamMemberId);
        Task<IEnumerable<Rota>> GetOpenRotasAsync();
        Task<IEnumerable<Rota>> GetRotasForTeamMemberAsync(Guid teamMemberId);
        Task<IEnumerable<Rota>> GetRotasForCalendarAsync(DateTime startDate, DateTime endDate);
        Task<int> GetOpenShiftsCountAsync();
         Task CreateRotasForDateRangeAsync(IEnumerable<Rota> rotas);
    }
}
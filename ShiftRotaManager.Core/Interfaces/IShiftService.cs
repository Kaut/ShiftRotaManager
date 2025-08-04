using ShiftRotaManager.Data.Models;

namespace ShiftRotaManager.Core.Interfaces
{
    public interface IShiftService
    {
        Task<IEnumerable<Shift>> GetAllShiftsAsync();
        Task<Shift?> GetShiftByIdAsync(Guid id);
        Task AddShiftAsync(Shift shift);
        Task UpdateShiftAsync(Shift shift);
        Task DeleteShiftAsync(Guid id);
        Task AddShiftVariantAsync(ShiftVariant variant);
        Task<IEnumerable<ShiftVariant>> GetShiftVariantsByBaseShiftIdAsync(Guid baseShiftId);
    }
}
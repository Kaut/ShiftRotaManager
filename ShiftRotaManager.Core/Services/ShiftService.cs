using ShiftRotaManager.Core.Interfaces;
using ShiftRotaManager.Data.Models;
using ShiftRotaManager.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiftRotaManager.Core.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly IGenericRepository<ShiftVariant> _shiftVariantRepository;

        public ShiftService(IShiftRepository shiftRepository, IGenericRepository<ShiftVariant> shiftVariantRepository)
        {
            _shiftRepository = shiftRepository;
            _shiftVariantRepository = shiftVariantRepository;
        }

        public async Task AddShiftAsync(Shift shift)
        {
            // Basic validation example
            if (shift.StartTime >= shift.EndTime && !shift.IsNightShift)
            {
                throw new ArgumentException("End time must be after start time for non-night shifts.");
            }
            await _shiftRepository.AddAsync(shift);
            await _shiftRepository.SaveChangesAsync();
        }

        public async Task UpdateShiftAsync(Shift shift)
        {
            // Basic validation example
            if (shift.StartTime >= shift.EndTime && !shift.IsNightShift)
            {
                throw new ArgumentException("End time must be after start time for non-night shifts.");
            }
            _shiftRepository.Update(shift);
            await _shiftRepository.SaveChangesAsync();
        }

        public async Task DeleteShiftAsync(Guid id)
        {
            var shift = await _shiftRepository.GetByIdAsync(id);
            if (shift != null)
            {
                // Consider checking for associated rotas before deleting a shift
                _shiftRepository.Remove(shift);
                await _shiftRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Shift>> GetAllShiftsAsync()
        {
            return await _shiftRepository.GetAllAsync();
        }

        public async Task<Shift?> GetShiftByIdAsync(Guid id)
        {
            return await _shiftRepository.GetByIdAsync(id);
        }

        public async Task AddShiftVariantAsync(ShiftVariant variant)
        {
            // Ensure the base shift exists
            var baseShift = await _shiftRepository.GetByIdAsync(variant.BaseShiftId);
            if (baseShift == null)
            {
                throw new ArgumentException("Base shift not found for the variant.");
            }

            await _shiftVariantRepository.AddAsync(variant);
            await _shiftVariantRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShiftVariant>> GetShiftVariantsByBaseShiftIdAsync(Guid baseShiftId)
        {
            return await _shiftVariantRepository.FindAsync(sv => sv.BaseShiftId == baseShiftId);
        }
    }
}
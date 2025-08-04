using ShiftRotaManager.Core.Interfaces;
using ShiftRotaManager.Data.Models;
using ShiftRotaManager.Data.Repositories;

namespace ShiftRotaManager.Core.Services
{
    public class RotaService : IRotaService
    {
        private readonly IRotaRepository _rotaRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IShiftRepository _shiftRepository;

        public RotaService(IRotaRepository rotaRepository,
                           ITeamMemberRepository teamMemberRepository,
                           IShiftRepository shiftRepository)
        {
            _rotaRepository = rotaRepository;
            _teamMemberRepository = teamMemberRepository;
            _shiftRepository = shiftRepository;
        }

        public async Task AddRotaAsync(Rota rota)
        {
            // Basic validation: Ensure Shift exists
            var shift = await _shiftRepository.GetByIdAsync(rota.ShiftId);
            if (shift == null)
            {
                throw new ArgumentException("Invalid Shift ID provided.");
            }

            // If a team member is assigned, ensure they exist
            if (rota.TeamMemberId.HasValue)
            {
                var teamMember = await _teamMemberRepository.GetByIdAsync(rota.TeamMemberId.Value);
                if (teamMember == null)
                {
                    throw new ArgumentException("Invalid Team Member ID provided for assignment.");
                }
                rota.Status = RotaStatus.Assigned; // Automatically set status if assigned
            }
            else
            {
                rota.Status = RotaStatus.Open; // Automatically set status if no team member
            }

            // If paired team member is assigned, ensure they exist
            if (rota.PairedTeamMemberId.HasValue)
            {
                var pairedTeamMember = await _teamMemberRepository.GetByIdAsync(rota.PairedTeamMemberId.Value);
                if (pairedTeamMember == null)
                {
                    throw new ArgumentException("Invalid Paired Team Member ID provided.");
                }
            }

            await _rotaRepository.AddAsync(rota);
            await _rotaRepository.SaveChangesAsync();
        }

        public async Task UpdateRotaAsync(Rota rota)
        {
            var existingRota = await _rotaRepository.GetByIdAsync(rota.Id);
            if (existingRota == null)
            {
                throw new ArgumentException("Rota not found for update.");
            }

            // Update only allowed fields to prevent accidental changes to relationships not handled by the form
            existingRota.Date = rota.Date;
            existingRota.ShiftId = rota.ShiftId;
            existingRota.TeamMemberId = rota.TeamMemberId;
            existingRota.PairedTeamMemberId = rota.PairedTeamMemberId;
            existingRota.Status = rota.TeamMemberId.HasValue ? RotaStatus.Assigned : RotaStatus.Open; // Re-evaluate status

            // Basic validation: Ensure Shift exists
            var shift = await _shiftRepository.GetByIdAsync(existingRota.ShiftId);
            if (shift == null)
            {
                throw new ArgumentException("Invalid Shift ID provided.");
            }

            // If a team member is assigned, ensure they exist
            if (existingRota.TeamMemberId.HasValue)
            {
                var teamMember = await _teamMemberRepository.GetByIdAsync(existingRota.TeamMemberId.Value);
                if (teamMember == null)
                {
                    throw new ArgumentException("Invalid Team Member ID provided for assignment.");
                }
            }

            // If paired team member is assigned, ensure they exist
            if (existingRota.PairedTeamMemberId.HasValue)
            {
                var pairedTeamMember = await _teamMemberRepository.GetByIdAsync(existingRota.PairedTeamMemberId.Value);
                if (pairedTeamMember == null)
                {
                    throw new ArgumentException("Invalid Paired Team Member ID provided.");
                }
            }

            _rotaRepository.Update(existingRota);
            await _rotaRepository.SaveChangesAsync();
        }

        public async Task DeleteRotaAsync(Guid id)
        {
            var rota = await _rotaRepository.GetByIdAsync(id);
            if (rota != null)
            {
                _rotaRepository.Remove(rota);
                await _rotaRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Rota>> GetAllRotasAsync()
        {
            return await _rotaRepository.GetAllRotasWithDetailsAsync();
        }

        public async Task<Rota?> GetRotaByIdAsync(Guid id)
        {
            return await _rotaRepository.GetRotaByIdWithDetailsAsync(id);
        }

        public async Task AssignOpenShiftAsync(Guid rotaId, Guid teamMemberId)
        {
            var rota = await _rotaRepository.GetRotaByIdWithDetailsAsync(rotaId);
            if (rota == null)
            {
                throw new ArgumentException("Rota not found.");
            }
            if (rota.Status != RotaStatus.Open)
            {
                throw new InvalidOperationException("This rota is not open for assignment.");
            }

            var teamMember = await _teamMemberRepository.GetByIdAsync(teamMemberId);
            if (teamMember == null)
            {
                throw new ArgumentException("Team Member not found.");
            }

            rota.TeamMemberId = teamMemberId;
            rota.Status = RotaStatus.Assigned;
            _rotaRepository.Update(rota);
            await _rotaRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<Rota>> GetOpenRotasAsync()
        {
            return await _rotaRepository.FindAsync(r => r.Status == RotaStatus.Open);
        }

        public async Task<IEnumerable<Rota>> GetRotasForTeamMemberAsync(Guid teamMemberId)
        {
            return await _rotaRepository.GetRotasByTeamMemberIdAsync(teamMemberId);
        }
    }
}
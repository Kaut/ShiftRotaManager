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

        // get rotas for a calendar view within a date range
         // Updated: Method to get rotas for a calendar view within a date range with filters
        public async Task<IEnumerable<Rota>> GetRotasForCalendarAsync(DateTime startDate, DateTime endDate, Guid? teamMemberId = null, Guid? shiftId = null, RotaStatus? status = null)
        {
            return await _rotaRepository.GetRotasByDateRangeAsync(startDate, endDate, teamMemberId, shiftId, status);
        }

        // NEW: For drag-and-drop rota date/time updates
        public async Task UpdateRotaDateTimeAsync(Guid rotaId, DateTime newDate, TimeSpan newStartTime, TimeSpan newEndTime, Guid? newTeamMemberId = null, Guid? newPairedTeamMemberId = null)
        {
            var rota = await _rotaRepository.GetRotaByIdWithDetailsAsync(rotaId);
            if (rota == null)
            {
                throw new ArgumentException("Rota not found for update.");
            }

            // Find the shift with the new start/end times or create a temporary one
            // In a real app, you might want to find an existing shift that matches these times
            // or create a new user-defined shift if it's truly a unique time.
            // For simplicity, we'll assume the shift itself isn't changing, just the date and its time relative to the shift.
            // A more robust solution would be to find an existing shift that matches the new times or create a new one.
            // For drag-and-drop, we primarily update the date, and potentially the assigned team member if dropped onto a resource.
            // The shift itself (its start/end time definitions) is usually fixed.
            // If the *shift type* is changing, that's a different operation (e.g., edit rota).

            // For drag-and-drop, we assume the shift *definition* (StartTime, EndTime) remains the same,
            // but the *date* of that shift instance changes.
            // If the time of day also changes, it implies a change to the underlying Shift.
            // For this POC, we'll update the Rota's Date and assume the ShiftId remains the same.
            // If the drag-and-drop implies changing the *type* of shift, that's more complex.

            // Let's adjust the logic for drag-and-drop:
            // The eventDrop callback provides new `start` and `end` datetime objects.
            // We need to update the Rota's `Date` and potentially re-assign `ShiftId` if the time changes significantly.
            // For simplicity, we'll update the Rota's Date and try to find a matching shift or keep the original ShiftId.

            // Update the date of the rota
            rota.Date = newDate.Date;

            // If a new team member ID is provided (e.g., from resource view drag-and-drop)
            if (newTeamMemberId.HasValue && rota.TeamMemberId != newTeamMemberId.Value)
            {
                var newTm = await _teamMemberRepository.GetByIdAsync(newTeamMemberId.Value);
                if (newTm == null) throw new ArgumentException("New Team Member not found.");
                rota.TeamMemberId = newTeamMemberId.Value;
                rota.Status = RotaStatus.Assigned; // If assigned, ensure status is assigned
            }
            else if (!newTeamMemberId.HasValue && rota.TeamMemberId.HasValue)
            {
                // If it was assigned and now no team member is specified (e.g., dragged off a resource)
                // This scenario might not be directly supported by standard drag-and-drop to "unassign"
                // but if it happens, we can make it open.
                rota.TeamMemberId = null;
                rota.Status = RotaStatus.Open;
            }

            // If a new paired team member ID is provided
            if (newPairedTeamMemberId.HasValue)
            {
                var newPairedTm = await _teamMemberRepository.GetByIdAsync(newPairedTeamMemberId.Value);
                if (newPairedTm == null) throw new ArgumentException("New Paired Team Member not found.");
                rota.PairedTeamMemberId = newPairedTeamMemberId.Value;
            }
            else if (rota.PairedTeamMemberId.HasValue)
            {
                // If it had a paired member and now none is specified
                rota.PairedTeamMemberId = null;
            }

            // Re-evaluate the shift based on newStartTime and newEndTime
            // This is tricky. If the *time of day* changes, it implies a different shift.
            // For a simple drag-and-drop, we usually just change the DATE.
            // If the user drags an event from 9-5 slot to a 10-6 slot, it implies changing the SHIFT TYPE.
            // For this POC, we'll keep the original ShiftId and just update the Date.
            // A more complex solution would try to find a matching Shift based on newStartTime/newEndTime.
            // For now, we'll assume `newStartTime` and `newEndTime` are just for event display on the calendar
            // and the underlying `ShiftId` (and its defined times) remains the same.
            // If the user wants to change the SHIFT TYPE, they should use the Edit form.

            // The `newStartTime` and `newEndTime` from FullCalendar's eventDrop are the *display* times.
            // The underlying Shift's StartTime and EndTime are fixed.
            // We only update the Rota's Date.
            // This means we don't use newStartTime/newEndTime from the eventDrop for updating the Rota model directly,
            // as the Rota model only links to a Shift by ID, which has fixed times.
            // If the intent is to allow changing the *type* of shift by dragging, the UI and backend need to be more complex.

            // For now, we'll stick to just updating the `Date` and `TeamMemberId`/`PairedTeamMemberId` if provided.
            // The `newStartTime` and `newEndTime` parameters are redundant for this current Rota model design.
            // I'll keep them in the signature for future extensibility if the Shift model changes.

            _rotaRepository.Update(rota);
            await _rotaRepository.SaveChangesAsync();
        }
    }
}
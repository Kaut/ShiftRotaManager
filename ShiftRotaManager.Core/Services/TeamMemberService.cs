using ShiftRotaManager.Core.Interfaces;
using ShiftRotaManager.Data.Models;
using ShiftRotaManager.Data.Repositories;

namespace ShiftRotaManager.Core.Services
{
    public class TeamMemberService : ITeamMemberService
    {
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<TeamMemberPreference> _teamMemberPreferenceRepository;

        public TeamMemberService(ITeamMemberRepository teamMemberRepository,
                                 IGenericRepository<Role> roleRepository,
                                 IGenericRepository<TeamMemberPreference> teamMemberPreferenceRepository)
        {
            _teamMemberRepository = teamMemberRepository;
            _roleRepository = roleRepository;
            _teamMemberPreferenceRepository = teamMemberPreferenceRepository;
        }

        public async Task AddTeamMemberAsync(TeamMember teamMember)
        {
            var role = await _roleRepository.GetByIdAsync(teamMember.RoleId);
            if (role == null)
            {
                throw new ArgumentException("Invalid role ID provided.");
            }

            await _teamMemberRepository.AddAsync(teamMember);
            await _teamMemberRepository.SaveChangesAsync();
        }

        public async Task UpdateTeamMemberAsync(TeamMember teamMember)
        {
            var existingTeamMember = await _teamMemberRepository.GetTeamMemberByIdWithDetailsAsync(teamMember.Id);
            if (existingTeamMember == null)
                throw new ArgumentException("Team Member not found for update.");

            // Update scalar properties
            existingTeamMember.FirstName = teamMember.FirstName;
            existingTeamMember.LastName = teamMember.LastName;
            existingTeamMember.Email = teamMember.Email;
            existingTeamMember.RoleId = teamMember.RoleId;

            // Synchronize preferences
            var incomingPreferences = teamMember.Preferences
                .Select(p => (p.DayOfWeek, p.Shift))
                .ToHashSet();

            // Remove preferences not in the incoming set
            var preferencesToRemove = existingTeamMember.Preferences
                .Where(p => !incomingPreferences.Contains((p.DayOfWeek, p.Shift)))
                .ToList();

            foreach (var preference in preferencesToRemove)
            {
                // Use repository or DbContext to remove
                //_teamMemberRepository.RemovePreference(preference);
                existingTeamMember.Preferences.Remove(preference);
            }

            // Add new preferences
            foreach (var incoming in incomingPreferences)
            {
                if (!existingTeamMember.Preferences.Any(p => p.DayOfWeek == incoming.DayOfWeek && p.Shift == incoming.Shift))
                {
                    existingTeamMember.Preferences.Add(new TeamMemberPreference
                    {
                        TeamMemberId = existingTeamMember.Id,
                        DayOfWeek = incoming.DayOfWeek,
                        Shift = incoming.Shift
                    });
                }
            }

            await _teamMemberRepository.SaveChangesAsync();
        }

        public async Task DeleteTeamMemberAsync(Guid id)
        {
            var teamMember = await _teamMemberRepository.GetByIdAsync(id);
            if (teamMember != null)
            {
                _teamMemberRepository.Remove(teamMember);
                await _teamMemberRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TeamMember>> GetAllTeamMembersAsync()
        {
            return await _teamMemberRepository.GetTeamMembersWithRolesAsync();
        }

        public async Task<TeamMember?> GetTeamMemberByIdAsync(Guid id)
        {
            return await _teamMemberRepository.GetTeamMemberByIdWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<ICollection<TeamMember>> GetTeamMembersByIdsAsync(List<Guid> pairedTeamMemberIds)
        {
            var teamMembers = new List<TeamMember>();
            foreach (var id in pairedTeamMemberIds)
            {
                var teamMember = await _teamMemberRepository.GetByIdAsync(id);
                if (teamMember != null) teamMembers.Add(teamMember);
            }
            return teamMembers;
        }

        public async Task<ICollection<TeamMember>> GetTeamMembersByPreferredDaysAsync(List<string> days)
        {
            var teamMembers = new List<TeamMember>();
            foreach (var day in days)
            {
                await _teamMemberRepository.GetTeamMembersByPreferredDayAsync(Enum.Parse<DayOfWeek>(day.ToLower()));
            }
            return teamMembers;
        }
    }
}
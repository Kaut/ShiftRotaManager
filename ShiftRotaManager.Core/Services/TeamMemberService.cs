using ShiftRotaManager.Core.Interfaces;
using ShiftRotaManager.Data.Models;
using ShiftRotaManager.Data.Repositories;

namespace ShiftRotaManager.Core.Services
{
    public class TeamMemberService : ITeamMemberService
    {
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IGenericRepository<Role> _roleRepository;

        public TeamMemberService(ITeamMemberRepository teamMemberRepository,
                                 IGenericRepository<Role> roleRepository)
        {
            _teamMemberRepository = teamMemberRepository;
            _roleRepository = roleRepository;
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
            var existingTeamMember = await _teamMemberRepository.GetByIdAsync(teamMember.Id);
            if (existingTeamMember == null)
            {
                throw new ArgumentException("Team Member not found for update.");
            }

            existingTeamMember.FirstName = teamMember.FirstName;
            existingTeamMember.LastName = teamMember.LastName;
            existingTeamMember.Email = teamMember.Email;
            existingTeamMember.RoleId = teamMember.RoleId;
            
            _teamMemberRepository.Update(existingTeamMember);
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
            return await _teamMemberRepository.GetByIdAsync(id);
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
            return await _teamMemberRepository.GetTeamMembersByPreferredDaysAsync(string.Join(",",days));
        }
    }
}
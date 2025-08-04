using ShiftRotaManager.Core.Interfaces;
using ShiftRotaManager.Data.Models;
using ShiftRotaManager.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiftRotaManager.Core.Services
{
    public class TeamMemberService : ITeamMemberService
    {
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<UserRole> _userRoleRepository;

        public TeamMemberService(ITeamMemberRepository teamMemberRepository,
                                 IGenericRepository<Role> roleRepository,
                                 IGenericRepository<UserRole> userRoleRepository)
        {
            _teamMemberRepository = teamMemberRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        public async Task AddTeamMemberAsync(TeamMember teamMember, Guid roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new ArgumentException("Invalid role ID provided.");
            }

            await _teamMemberRepository.AddAsync(teamMember);
            await _teamMemberRepository.SaveChangesAsync(); // Save team member first to get its ID

            var userRole = new UserRole { TeamMemberId = teamMember.Id, RoleId = roleId };
            await _userRoleRepository.AddAsync(userRole);
            await _userRoleRepository.SaveChangesAsync();
        }

        public async Task UpdateTeamMemberAsync(TeamMember teamMember)
        {
            _teamMemberRepository.Update(teamMember);
            await _teamMemberRepository.SaveChangesAsync();
        }

        public async Task DeleteTeamMemberAsync(Guid id)
        {
            var teamMember = await _teamMemberRepository.GetByIdAsync(id);
            if (teamMember != null)
            {
                // Remove associated user roles first
                var userRoles = await _userRoleRepository.FindAsync(ur => ur.TeamMemberId == id);
                _userRoleRepository.RemoveRange(userRoles);
                await _userRoleRepository.SaveChangesAsync();

                _teamMemberRepository.Remove(teamMember);
                await _teamMemberRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TeamMember>> GetAllTeamMembersAsync()
        {
            return await _teamMemberRepository.GetAllAsync();
        }

        public async Task<TeamMember?> GetTeamMemberByIdAsync(Guid id)
        {
            return await _teamMemberRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllAsync();
        }
    }
}
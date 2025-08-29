using ShiftRotaManager.Data.Models;

namespace ShiftRotaManager.Core.Services
{
    public interface IRoleService
    {
        Task CreateRoleAsync(Role role);
        Task DeleteRoleAsync(Guid id);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(Guid id);
        Task UpdateRoleAsync(Role role);
    }
}

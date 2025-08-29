
using System.Linq.Expressions;
using ShiftRotaManager.Data.Models;
using ShiftRotaManager.Data.Repositories;

namespace ShiftRotaManager.Web.Data
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<int> CountAsync(Expression<Func<Role, bool>> predicate);
    }
}

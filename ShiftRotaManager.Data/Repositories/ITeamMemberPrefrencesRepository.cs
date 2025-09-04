
using System.Linq.Expressions;
using ShiftRotaManager.Data.Models;
using ShiftRotaManager.Data.Repositories;

namespace ShiftRotaManager.Web.Data
{
    public interface ITeamMemberPrefrencesRepository : IGenericRepository<TeamMemberPreference>
    {
        Task<int> CountAsync(Expression<Func<TeamMemberPreference, bool>> predicate);
    }
}

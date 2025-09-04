
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShiftRotaManager.Data.Data;
using ShiftRotaManager.Data.Models;
using ShiftRotaManager.Data.Repositories;

namespace ShiftRotaManager.Web.Data
{
    public class TeamMemberPrefrencesRepository : GenericRepository<TeamMemberPreference>, ITeamMemberPrefrencesRepository
    {
        public TeamMemberPrefrencesRepository(ApplicationDbContext context): base(context)
        {
            
        }
        public Task<int> CountAsync(Expression<Func<TeamMemberPreference, bool>> predicate)
        {
            return _dbSet.CountAsync(predicate);
        }
    }
}

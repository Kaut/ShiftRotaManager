using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShiftRotaManager.Data.Data;
using ShiftRotaManager.Data.Models;
using ShiftRotaManager.Data.Repositories;

namespace ShiftRotaManager.Web.Data
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {

        public RoleRepository(ApplicationDbContext context) : base(context)
        {

        }
        
         public async Task<int> CountAsync(Expression<Func<Role, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }        
    }
}

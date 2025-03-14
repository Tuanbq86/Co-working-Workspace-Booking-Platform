using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories;

public interface IPromotionRepository : IGenericRepository<Promotion>
{
    Task<Promotion> GetFirstOrDefaultAsync(Expression<Func<Promotion, bool>> predicate);
}

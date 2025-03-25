using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

public class PromotionRepository : GenericRepository<Promotion>, IPromotionRepository
{
    public PromotionRepository() { }
    public PromotionRepository(WorkHiveContext context) => _context = context;

    public async Task<List<Promotion>> GetAllPromotionsByWorkspaceOwnerIdAsync(int workspaceOwnerId)
    {
        return await _context.Promotions.Where(p => p.Workspace.OwnerId == workspaceOwnerId).Include(p => p.Workspace).ToListAsync();
    }

    public async Task<Promotion> GetFirstOrDefaultAsync(Expression<Func<Promotion, bool>> predicate)
    {
        return await _context.Promotions.FirstOrDefaultAsync(predicate);
    }


}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories
{
    public class DetailRepository : GenericRepository<Detail>, IDetailRepository
    {
        public DetailRepository(WorkHiveContext context) : base(context)
        {
        }
        public async Task<List<Detail>> GetDetailsByWorkspaceIdAsync(int workspaceId)
        {
            return await _context.Details
            .Where(i => i.WorkspaceDetails.Any(wi => wi.WorkspaceId == workspaceId))
            .ToListAsync();
        }

        public async Task CreateDetailsAsync(List<Detail> Details)
        {
            if (Details == null || !Details.Any()) return;

            await _context.Details.AddRangeAsync(Details);
            await _context.SaveChangesAsync();
        }
    }
}

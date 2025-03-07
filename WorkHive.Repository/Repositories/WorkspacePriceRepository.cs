using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

public class WorkspacePriceRepository : GenericRepository<WorkspacePrice>, IWorkspacePriceRepository
{
    public WorkspacePriceRepository() { }
    public WorkspacePriceRepository(WorkHiveContext context) => _context = context;

    public async Task CreateWorkspacePricesAsync(List<WorkspacePrice> WorkspacePrices)
    {
        if (WorkspacePrices == null || !WorkspacePrices.Any()) return;

        await _context.WorkspacePrices.AddRangeAsync(WorkspacePrices);
        await _context.SaveChangesAsync();
    }

    public async Task<List<WorkspacePrice>> GetWorkspacePricesByWorkspaceIdAsync(int workspaceId)
    {
        return await _context.WorkspacePrices
                    .Where(i => i.WorkspaceId == workspaceId)
                    .ToListAsync();
    }

}

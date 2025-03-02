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

public class WorkspaceRepository : GenericRepository<Workspace>, IWorkspaceRepository
{
    public WorkspaceRepository() { }
    public WorkspaceRepository(WorkHiveContext context) => _context = context;

    public async Task<List<Workspace>> GetAllWorkSpaceByOwnerIdAsync(int ownerId)
    {
        return await _context.Workspaces
            .Include(w => w.WorkspacePrices)
            .ThenInclude(wp => wp.Price)  
            .Include(w => w.WorkspaceImages)
            .ThenInclude(wi => wi.Image)  
            .ToListAsync();
    }
    //public async Task<List<Workspace>> GetWorkspacesWithPriceAndImagesAsync()
    //{
    //    return await _context.Workspaces
    //        .Include(w => w.WorkspacePrices) 
    //        .Include(w => w.WorkspaceImages) 
    //        .ToListAsync();
    //}

}

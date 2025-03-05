using Microsoft.EntityFrameworkCore;
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
            .Where(w => w.OwnerId == ownerId)
            .ToListAsync();
    }
    public async Task<Workspace?> GetWorkSpaceById(int ownerId)
    {
        return await _context.Workspaces
            .Include(w => w.WorkspacePrices)
            .ThenInclude(wp => wp.Price)
            .Include(w => w.WorkspaceImages)
            .ThenInclude(wi => wi.Image)
            .FirstOrDefaultAsync(w => w.OwnerId == ownerId);
    }

    public async Task<List<Workspace>> GetAllWorkSpaceAsync()
    {
        return await _context.Workspaces
            .Include(w => w.WorkspacePrices)
            .ThenInclude(wp => wp.Price)
            .Include(w => w.WorkspaceImages)
            .ThenInclude(wi => wi.Image).ToListAsync();
    }



}

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
            .Include(w => w.Owner)
            .Include(w => w.WorkspacePrices)
            .ThenInclude(wp => wp.Price)
            .Include(w => w.WorkspaceImages)
            .ThenInclude(wi => wi.Image)
            .Include(w => w.WorkspaceFacilities)
            .ThenInclude(wf => wf.Facility)
            .Include(w => w.WorkspacePolicies)
            .ThenInclude(wp => wp.Policy)
            .Where(w => w.OwnerId == ownerId)
            .Include(w => w.WorkspaceDetails)
            .ThenInclude(wd => wd.Detail)
            .ToListAsync();
    }
    public async Task<Workspace?> GetWorkSpaceById(int Id)
    {
        return await _context.Workspaces
            .Include(w => w.Owner)
            .Include(w => w.WorkspacePrices)
            .ThenInclude(wp => wp.Price)
            .Include(w => w.WorkspaceImages)
            .ThenInclude(wi => wi.Image)
            .Include(w => w.WorkspaceFacilities)
            .ThenInclude(wf => wf.Facility)
            .Include(w => w.WorkspacePolicies)
            .ThenInclude(wp => wp.Policy)
            .Include(w => w.WorkspaceDetails)
            .ThenInclude(wd => wd.Detail)
            .FirstOrDefaultAsync(w => w.Id == Id);
    }

    public async Task<List<Workspace>> GetAllWorkSpaceAsync()
    {
        return await _context.Workspaces
            .Include(w => w.Owner)
            .Include(w => w.WorkspacePrices)
            .ThenInclude(wp => wp.Price)
            .Include(w => w.WorkspaceImages)
            .ThenInclude(wi => wi.Image)
            //.Include(w => w.WorkspaceFacilities)
            //.ThenInclude(wf => wf.Facility)
            //.Include(w => w.WorkspacePolicies)
            //.ThenInclude(wp => wp.Policy)
            //.Include(w => w.WorkspaceDetails)
            //.ThenInclude(wd => wd.Detail)
            .ToListAsync();
    }

    public async Task<Workspace> GetWorkspaceByIdForTime(int workspaceId)
    {
        return await _context.Workspaces.Where(w => w.Id == workspaceId)
            .Include(w => w.WorkspaceTimes).FirstOrDefaultAsync();
    }

    public IQueryable<Workspace> GetWorkspaceForSearch()
    {
        return _context.Workspaces
            .Include(w => w.Owner)
            .Include(w => w.WorkspacePrices)
            .ThenInclude(wp => wp.Price)
            .Include(w => w.WorkspaceImages)
            .ThenInclude(wi => wi.Image)
            .Include(w => w.WorkspaceFacilities)
            .ThenInclude(wf => wf.Facility)
            .Include(w => w.WorkspacePolicies)
            .ThenInclude(wp => wp.Policy)
            .AsQueryable();

    }

    public async Task<Workspace?> GetByNameAsync(string name)
    {
        return await _context.Workspaces
            .FirstOrDefaultAsync(w => w.Name.ToLower() == name.ToLower());
    }

    public async Task<List<Workspace>> GetWorkspaceByRateSearch()
    {
        return await _context.Workspaces
            .Include(w => w.Owner)
            .Include(w => w.WorkspacePrices)
            .ThenInclude(wp => wp.Price)
            .Include(w => w.WorkspaceImages)
            .ThenInclude(wi => wi.Image)
            .Include(w => w.WorkspaceFacilities)
            .ThenInclude(wf => wf.Facility)
            .Include(w => w.WorkspacePolicies)
            .ThenInclude(wp => wp.Policy)
            .Include(w => w.WorkspaceRatings)
            .ThenInclude(wr => wr.Rating)
            .ToListAsync();
    }

    public IQueryable<Workspace> GetWorkspaceByCategorySearch(string Category)
    {
        return _context.Workspaces
            .Include(w => w.Owner)
            .Include(w => w.WorkspacePrices)
            .ThenInclude(wp => wp.Price)
            .Include(w => w.WorkspaceImages)
            .ThenInclude(wi => wi.Image)
            .Include(w => w.WorkspaceFacilities)
            .ThenInclude(wf => wf.Facility)
            .Include(w => w.WorkspacePolicies)
            .ThenInclude(wp => wp.Policy)
            .AsQueryable();
    }
    public async Task<List<Workspace>> GetByOwnerIdAsync(int ownerId)
    {
        return await _context.Workspaces
            .Where(w => w.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<List<Price>> GetPricesByWorkspaceIdAsync(int workspaceId)
{
    return await _context.WorkspacePrices
        .Where(wp => wp.WorkspaceId == workspaceId)
        .Select(wp => wp.Price) 
        .ToListAsync();
}

    public IQueryable<Workspace> GetWorkspaceByWorkspaceNameSearch(string Name)
    {
        return _context.Workspaces
            .Include(w => w.Owner)
            .Include(w => w.WorkspacePrices)
            .ThenInclude(wp => wp.Price)
            .Include(w => w.WorkspaceImages)
            .ThenInclude(wi => wi.Image)
            .Include(w => w.WorkspaceFacilities)
            .ThenInclude(wf => wf.Facility)
            .Include(w => w.WorkspacePolicies)
            .ThenInclude(wp => wp.Policy)
            .AsQueryable();
    }

    public async Task<List<Workspace>> GetWorkspaceForOwnerSearch(int ownerId)
    {
        return await _context.Workspaces
            .Where(w => w.OwnerId == ownerId)
            .Include(w => w.WorkspaceRatings)
            .ThenInclude(wr => wr.Rating)
            .ToListAsync();
    }
}

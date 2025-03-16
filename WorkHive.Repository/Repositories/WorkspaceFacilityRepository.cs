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

public class WorkspaceFacilityRepository : GenericRepository<WorkspaceFacility>, IWorkspaceFacilityRepository
{
    public WorkspaceFacilityRepository() { }
    public WorkspaceFacilityRepository(WorkHiveContext context) => _context = context;

    public async Task CreateWorkspaceFacilitiesAsync(List<WorkspaceFacility> workspaceFacilities)
    {
        if (workspaceFacilities == null || !workspaceFacilities.Any()) return;

        await _context.WorkspaceFacilities.AddRangeAsync(workspaceFacilities);
        await _context.SaveChangesAsync();
    }

    public async Task<List<WorkspaceFacility>> GetWorkspaceFacilitiesByWorkspaceIdAsync(int workspaceId)
    {
        return await _context.WorkspaceFacilities
                    .Where(i => i.WorkspaceId == workspaceId)
                    .ToListAsync();
    }

    public async Task<List<WorkspaceFacility>> GetByWorkspaceIdAsync(int workspaceId)
    {
        return await _context.WorkspaceFacilities
            .Where(wf => wf.WorkspaceId == workspaceId)
            .ToListAsync();
    }

    public async Task DeleteWorkspaceFacilitiesAsync(List<WorkspaceFacility> workspaceFacilities)
    {
        _context.WorkspaceFacilities.RemoveRange(workspaceFacilities);
        await _context.SaveChangesAsync();
    }

}

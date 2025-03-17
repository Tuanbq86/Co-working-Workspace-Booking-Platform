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

public class WorkspacePolicyRepository : GenericRepository<WorkspacePolicy>, IWorkspacePolicyRepository
{
    public WorkspacePolicyRepository() { }
    public WorkspacePolicyRepository(WorkHiveContext context) => _context = context;

    public async Task CreateWorkspacePoliciesAsync(List<WorkspacePolicy> workspacePolicies)
    {
        if (workspacePolicies == null || !workspacePolicies.Any()) return;

        await _context.WorkspacePolicies.AddRangeAsync(workspacePolicies);
        await _context.SaveChangesAsync();
    }

    public async Task<List<WorkspacePolicy>> GetWorkspacePoliciesByWorkspaceIdAsync(int workspaceId)
    {
        return await _context.WorkspacePolicies
                    .Where(i => i.WorkspaceId == workspaceId)
                    .ToListAsync();
    }

    public async Task<List<WorkspacePolicy>> GetByWorkspaceIdAsync(int workspaceId)
    {
        return await _context.WorkspacePolicies
            .Where(wp => wp.WorkspaceId == workspaceId)
            .ToListAsync();
    }

    public async Task DeleteWorkspacePoliciesAsync(List<WorkspacePolicy> workspacePolicies)
    {
        _context.WorkspacePolicies.RemoveRange(workspacePolicies);
        await _context.SaveChangesAsync();
    }

}

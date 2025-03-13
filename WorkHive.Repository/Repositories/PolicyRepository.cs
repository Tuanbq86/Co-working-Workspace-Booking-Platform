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

public class PolicyRepository : GenericRepository<Policy>, IPolicyRepository
{
    public PolicyRepository() { }
    public PolicyRepository(WorkHiveContext context) => _context = context;

    public async Task CreatePoliciesAsync(List<Policy> policies)
    {
        if (policies == null || !policies.Any()) return;

        await _context.Policies.AddRangeAsync(policies);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Policy>> GetPoliciesByWorkspaceIdAsync(int workspaceId)
    {
        return await _context.Policies
            .Where(i => i.WorkspacePolicies.Any(wi => wi.WorkspaceId == workspaceId))
            .ToListAsync();
    }


}

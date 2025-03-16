using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories;

public interface IWorkspacePolicyRepository : IGenericRepository<WorkspacePolicy>
{
    public Task CreateWorkspacePoliciesAsync(List<WorkspacePolicy> workspacePolicies);
    public Task<List<WorkspacePolicy>> GetWorkspacePoliciesByWorkspaceIdAsync(int workspaceId);

    Task<List<WorkspacePolicy>> GetByWorkspaceIdAsync(int workspaceId);
    Task DeleteWorkspacePoliciesAsync(List<WorkspacePolicy> workspacePolicies);
}

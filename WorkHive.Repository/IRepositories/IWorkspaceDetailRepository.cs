using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories
{
    public interface IWorkspaceDetailRepository : IGenericRepository<WorkspaceDetail>
    {
        Task CreateWorkspaceDetailsAsync(List<WorkspaceDetail> workspaceDetails);

        Task<List<WorkspaceDetail>> GetWorkspaceDetailsByWorkspaceIdAsync(int workspaceId);
        Task<List<WorkspaceDetail>> GetByWorkspaceIdAsync(int workspaceId);

        Task DeleteWorkspaceDetailsAsync(List<WorkspaceDetail> workspaceDetails);
    }
}

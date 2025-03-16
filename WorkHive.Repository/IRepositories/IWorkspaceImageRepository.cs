using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories;

public interface IWorkspaceImageRepository : IGenericRepository<WorkspaceImage>
{
    public Task CreateWorkspaceImagesAsync(List<WorkspaceImage> WorkspaceImages);

    public Task<List<WorkspaceImage>> GetWorkspaceImagesByWorkspaceIdAsync(int workspaceId);

    Task<List<WorkspaceImage>> GetByWorkspaceIdAsync(int workspaceId);

    Task DeleteWorkspaceImagesAsync(List<WorkspaceImage> workspaceImages);
}

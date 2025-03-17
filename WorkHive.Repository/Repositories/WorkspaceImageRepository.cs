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

class WorkspaceImageRepository : GenericRepository<WorkspaceImage>, IWorkspaceImageRepository
{
    public WorkspaceImageRepository() { }
    public WorkspaceImageRepository(WorkHiveContext context) => _context = context;

    public async Task CreateWorkspaceImagesAsync(List<WorkspaceImage> WorkspaceImages)
    {
        if (WorkspaceImages == null || !WorkspaceImages.Any()) return;

        await _context.WorkspaceImages.AddRangeAsync(WorkspaceImages);
        await _context.SaveChangesAsync();
    }

    public async Task<List<WorkspaceImage>> GetWorkspaceImagesByWorkspaceIdAsync(int workspaceId)
    {
        return await _context.WorkspaceImages
                    .Where(i => i.WorkspaceId == workspaceId)
                    .ToListAsync();
    }

    public async Task<List<WorkspaceImage>> GetByWorkspaceIdAsync(int workspaceId)
    {
        return await _context.WorkspaceImages
            .Where(wi => wi.WorkspaceId == workspaceId)
            .ToListAsync();
    }

    public async Task DeleteWorkspaceImagesAsync(List<WorkspaceImage> workspaceImages)
    {
        _context.WorkspaceImages.RemoveRange(workspaceImages);
        await _context.SaveChangesAsync();
    }


}

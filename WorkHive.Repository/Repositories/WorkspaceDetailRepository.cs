using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories
{
    public class WorkspaceDetailRepository : GenericRepository<WorkspaceDetail>, IWorkspaceDetailRepository
    {
        public WorkspaceDetailRepository()
        {
            
        }
        public WorkspaceDetailRepository(WorkHiveContext context) : base(context)
        {
        }

        public async Task CreateWorkspaceDetailsAsync(List<WorkspaceDetail> workspaceDetails)
        {
            if (workspaceDetails == null || !workspaceDetails.Any()) return;

            await _context.WorkspaceDetails.AddRangeAsync(workspaceDetails);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWorkspaceDetailsAsync(List<WorkspaceDetail> workspaceDetails)
        {
            _context.WorkspaceDetails.RemoveRange(workspaceDetails);
            await _context.SaveChangesAsync();
        }

        public async Task<List<WorkspaceDetail>> GetByWorkspaceIdAsync(int workspaceId)
        {
            return await _context.WorkspaceDetails
            .Where(wp => wp.WorkspaceId == workspaceId)
            .ToListAsync();
        }

        public async Task<List<WorkspaceDetail>> GetWorkspaceDetailsByWorkspaceIdAsync(int workspaceId)
        {
            return await _context.WorkspaceDetails
            .Where(wp => wp.WorkspaceId == workspaceId)
            .ToListAsync();
        }
    }
}

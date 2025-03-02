using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

public class WorkspaceTimeRepository : GenericRepository<WorkspaceTime>, IWorkspaceTimeRepository
{
    public WorkspaceTimeRepository()
    {
        
    }

    public WorkspaceTimeRepository(WorkHiveContext context) => _context = context;

    public bool IsOverlap(List<WorkspaceTime> workspaceTimes, DateTime startDate, DateTime endDate)
    {
        return workspaceTimes.Any(w =>
        w.StartDate.HasValue && w.EndDate.HasValue &&
        startDate < w.EndDate.Value && endDate > w.StartDate.Value
    );
    }
}

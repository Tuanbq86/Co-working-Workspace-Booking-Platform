using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories;

public interface IWorkspaceTimeRepository : IGenericRepository<WorkspaceTime>
{
    public bool IsOverlap(List<WorkspaceTime> workspaceTimes, DateTime startDate, DateTime endDate);
}

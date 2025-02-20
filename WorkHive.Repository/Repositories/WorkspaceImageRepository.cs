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

    //To do object method


}

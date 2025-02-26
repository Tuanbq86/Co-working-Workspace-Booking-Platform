using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

public class WorkspaceOwnerRepository : GenericRepository<WorkspaceOwner>, IWorkspaceOwnerRepository
{
    public WorkspaceOwnerRepository() { }
    public WorkspaceOwnerRepository(WorkHiveContext context) => _context = context;

    //To do object method
    public WorkspaceOwner RegisterOwnerByPhoneAndEmail(string email, string phone, string password)
    {
        var workspaceOwner = new WorkspaceOwner
        {
            Email = email,
            Phone = phone,
            Password = password
        };
        return workspaceOwner;
    }

}

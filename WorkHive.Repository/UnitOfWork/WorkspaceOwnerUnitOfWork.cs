using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Repositories.Repositories;

namespace WorkHive.Repositories.UnitOfWork;

public class WorkspaceOwnerUnitOfWork : IWorkspaceOwnerUnitOfWork
{
    protected WorkHiveContext _context;
    public IWorkspaceOwnerRepository WorkspaceOwner { get; private set; }
    public WorkspaceOwnerUnitOfWork(WorkHiveContext context)
    {
        _context = context;
        WorkspaceOwner = new WorkspaceOwnerRepository(_context);
    }
    public int Save()
    {
        return _context.SaveChanges();
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }
}

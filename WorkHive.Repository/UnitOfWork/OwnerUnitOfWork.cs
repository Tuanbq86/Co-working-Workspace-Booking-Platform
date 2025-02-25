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

public class OwnerUnitOfWork : IOwnerUnitOfWork
{
    protected WorkHiveContext _context;
    public IOwnerRepository Owner { get; private set; }

    public OwnerUnitOfWork(WorkHiveContext context)
    {
        _context = context;
        Owner = new OwnerRepository(_context);
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

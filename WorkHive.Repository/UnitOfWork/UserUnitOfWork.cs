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

public class UserUnitOfWork : IUserUnitOfWork
{
    protected WorkHiveContext _context;
    public IUserRepository User { get; private set; }
    public IRoleRepository Role { get; private set; }
    public UserUnitOfWork(WorkHiveContext context)
    {
        _context = context;
        User = new UserRepository(_context);
        Role = new RoleRepository(_context);
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

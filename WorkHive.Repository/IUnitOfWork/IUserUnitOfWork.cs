using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Repositories.IRepositories;
namespace WorkHive.Repositories.IUnitOfWork;

//Unit of work is used for one transaction for database
public interface IUserUnitOfWork
{
    IUserRepository User { get; }
    IRoleRepository Role { get; }

    public int Save();
    public Task<int> SaveAsync();
}

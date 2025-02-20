using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository() { }
    public RoleRepository(WorkHiveContext context) => _context = context;

    //To do object method
    public Role GetRoleByUserName(string name)
    {
        var checkRole = _context.Roles.Where(x => x.RoleName.ToLower().Equals(name.ToLower()));
        return (Role)checkRole;
    }



}

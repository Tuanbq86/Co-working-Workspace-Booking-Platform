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

     public WorkspaceOwner FindOwnerByEmail(string email)
    {
        return _context.WorkspaceOwners.Where(x => x.Email.Equals(email)).FirstOrDefault()!;
    }

    public bool FindOwnerByEmailOrPhone(string auth, string password)
    {
        var OwnerEmail = _context.WorkspaceOwners.FirstOrDefault(u => u.Email.Equals(auth));

        if (OwnerEmail != null && BCrypt.Net.BCrypt.EnhancedVerify(password, OwnerEmail.Password))
        {
            return true;
        }

        var OwnerPhone = _context.WorkspaceOwners.FirstOrDefault(u => u.Phone.Equals(auth));

        if (OwnerPhone != null && BCrypt.Net.BCrypt.EnhancedVerify(password, OwnerPhone.Password))
        {
            return true;
        }

        return false;
    }

    public WorkspaceOwner FindOwnerByPhone(string phone)
    {
        return _context.WorkspaceOwners.Where(x => x.Phone.Equals(phone)).FirstOrDefault()!;
    }

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

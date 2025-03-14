using Microsoft.EntityFrameworkCore;
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

    public bool CheckNewAndConfrimPassword(string newPassword, string confirmPassword)
    {
        return newPassword.ToLower().Trim().Equals(confirmPassword.ToLower().Trim());
    }

    public WorkspaceOwner FindWorkspaceOwnerByEmail(string email)
    {
        return _context.WorkspaceOwners.Where(x => x.Email.ToLower().Trim()
        .Equals(email.ToLower().Trim())).FirstOrDefault()!;
    }

    public bool FindWorkspaceOwnerByEmailOrPhone(string auth, string password)
    {
        var WorkspaceOwnerEmail = _context.WorkspaceOwners.FirstOrDefault(u => u.Email.Equals(auth));

        if (WorkspaceOwnerEmail != null && BCrypt.Net.BCrypt.EnhancedVerify(password, WorkspaceOwnerEmail.Password))
        {
            return true;
        }

        var WorkspaceOwnerPhone = _context.WorkspaceOwners.FirstOrDefault(u => u.Phone.Equals(auth));

        if (WorkspaceOwnerPhone != null && BCrypt.Net.BCrypt.EnhancedVerify(password, WorkspaceOwnerPhone.Password))
        {
            return true;
        }

        return false;
    }

    public WorkspaceOwner FindWorkspaceOwnerByPhone(string phone)
    {
        return _context.WorkspaceOwners.Where(x => x.Phone.ToLower().Trim()
        .Equals(phone.ToLower().Trim())).FirstOrDefault()!;
    }

    public WorkspaceOwner RegisterWorkspaceOwner(string email,
        string phone, string password)
    {
        var WorkspaceOwner = new WorkspaceOwner
        {
            Email = email,
            Phone = phone,
            Password = password,
        };

        return WorkspaceOwner;
    }


    //To do object method

    public bool FindUserByEmailOrPhone(string auth, string password)
    {
        var userEmail = _context.Users.FirstOrDefault(u => u.Email.Equals(auth));

        if (userEmail != null && BCrypt.Net.BCrypt.EnhancedVerify(password, userEmail.Password))
        {
            return true;
        }

        var userPhone = _context.Users.FirstOrDefault(u => u.Phone.Equals(auth));

        if (userPhone != null && BCrypt.Net.BCrypt.EnhancedVerify(password, userPhone.Password))
        {
            return true;
        }

        return false;
    }

    public User FindUserByPhone(string phone)
    {
        return _context.Users.Where(x => x.Phone.ToLower().Trim()
        .Equals(phone.ToLower().Trim())).FirstOrDefault()!;
    }

    public User RegisterUser(string name, string email,
        string phone, string password, string sex)
    {
        var user = new User
        {
            Name = name,
            Email = email,
            Phone = phone,
            Password = password,
            Sex = sex
        };

        return user;
    }

    public async Task<List<WorkspaceOwner>> GetOwnersByIdsAsync(List<int> ownerIds)
    {
        return await _context.WorkspaceOwners
            .Where(o => ownerIds.Contains(o.Id))
            .ToListAsync();
    }

}

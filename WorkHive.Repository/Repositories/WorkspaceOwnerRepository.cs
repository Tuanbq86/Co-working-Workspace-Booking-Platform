using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WorkHive.Repositories.Repositories;

public class WorkspaceOwnerRepository : GenericRepository<WorkspaceOwner>, IWorkspaceOwnerRepository
{
    public WorkspaceOwnerRepository() { }
    public WorkspaceOwnerRepository(WorkHiveContext context) => _context = context;

    public bool CheckNewAndConfrimPassword(string newPassword, string confirmPassword)
    {
        return newPassword.ToLower().Trim().Equals(confirmPassword.ToLower().Trim());
    }

    public WorkspaceOwner? FindWorkspaceOwnerByEmail(string email)
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
        return await _context.WorkspaceOwners.Include(o => o.OwnerWallets).ThenInclude(ow => ow.User)
            .Where(o => ownerIds.Contains(o.Id))
            .ToListAsync();
    }

    public async Task<List<WorkspaceOwner>> GetAllOwnersAsync()
    {
        return await _context.WorkspaceOwners.Include(o => o.OwnerWallets).ThenInclude(ow => ow.User)
            .ToListAsync();
    }


    public async Task<List<WorkspaceOwner>> GetOwnersByUserId(int userId)
    {
        return await _context.Bookings
            .Where(b => b.UserId == userId)
            .GroupBy(b => b.Workspace.Owner.Id) 
            .Select(g => g.First().Workspace.Owner)     
            .ToListAsync();
    }

    public async Task<WorkspaceOwner?> FindByEmailAsync(Expression<Func<WorkspaceOwner, bool>> predicate)
    {
        return await _context.Set<WorkspaceOwner>().FirstOrDefaultAsync(predicate);
    }

    public async Task<WorkspaceOwner?> GetOwnerByIdAsync(int id)
    {
        return await _context.WorkspaceOwners
            .Include(o => o.OwnerWallets)
            .ThenInclude(ow => ow.User)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public IQueryable<WorkspaceOwner> GetOwnerForSearch()
    {
        return _context.WorkspaceOwners
            .AsQueryable();
    }

    public IQueryable<WorkspaceOwner> GetOwnerForSearchWithOwnerName(string name)
    {
        return _context.WorkspaceOwners
            .AsNoTracking()
            .Where(w => EF.Functions.Like(w.LicenseName, $"%{name}%"))
            .AsQueryable();
    }
    public async Task<WorkspaceOwner> FindByEmailAsync(string email)
    {
        return await _context.WorkspaceOwners
            .FirstOrDefaultAsync(o => o.Email == email);
    }
}

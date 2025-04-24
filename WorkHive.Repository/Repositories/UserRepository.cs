using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository() { }
    public UserRepository(WorkHiveContext context) => _context = context;

    public bool CheckNewAndConfrimPassword(string newPassword, string confirmPassword)
    {
        return newPassword.ToLower().Trim().Equals(confirmPassword.ToLower().Trim());
    }

    public User FindUserByEmail(string email)
    {
        return _context.Users.Where(x => x.Email.ToLower().Trim()
        .Equals(email.ToLower().Trim())).FirstOrDefault()!;
    }

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
        return _context.Users.Where(x => !string.IsNullOrEmpty(x.Phone) && x.Phone.ToLower().Trim()
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


    public async Task<List<User>> GetUsersByOwnerId(int ownerId)
    {
        return await _context.Users
            .Where(u => u.Bookings.Any(b => b.Workspace.OwnerId == ownerId))
            .GroupBy(u => u.Id) 
            .Select(g => g.First()) 
            .ToListAsync();
    }

    public async Task<List<User>> GetUsersByOwnerIdWithBookingStatus(int ownerId, string status)
    {
        return await _context.Users
            .Include(u => u.Bookings)
            .ThenInclude(b => b.Workspace)
            .ThenInclude(w => w.Owner)
            .Where(u => u.Bookings.Any(b => b.Workspace.OwnerId == ownerId && b.Status == status)) 
            .ToListAsync();
    }

    public async Task<User> FindByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(o => o.Email == email);
    }
}

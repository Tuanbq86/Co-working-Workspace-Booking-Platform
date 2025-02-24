using BCrypt.Net;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository() { }
    public UserRepository(WorkHiveContext context) => _context = context;

    public User FindUserByEmail(string email)
    {
        return _context.Users.Where(x => x.Email.Equals(email)).FirstOrDefault()!;
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
        return _context.Users.Where(x => x.Phone.Equals(phone)).FirstOrDefault()!;
    }

    public User RegisterUserByPhoneAndEmail(string name, string email, 
        string phone, string password)
    {
        var user = new User
        {
            Name = name,
            Email = email,
            Phone = phone,
            Password = password
        };
        
        return user;
    }


}

using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository() { }
    public UserRepository(WorkHiveContext context) => _context = context;

    //To do object method
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

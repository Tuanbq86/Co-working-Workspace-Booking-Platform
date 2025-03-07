using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories;

public interface IUserRepository : IGenericRepository<User>
{
    public User RegisterUser(string name, string email, string phone, string password, string sex);
    public User FindUserByEmail(string email);
    public User FindUserByPhone(string phone);
    public bool FindUserByEmailOrPhone(string auth, string password);
    public bool CheckNewAndConfrimPassword(string newPassword, string confirmPassword);
}

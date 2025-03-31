using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories;

public interface IUserPasswordResetTokenRepository : IGenericRepository<UserPasswordResetToken>
{
    public Task<string> CreatePasswordResetToken(string email);
    public Task<User> ValidatePasswordResetToken(string token);
}

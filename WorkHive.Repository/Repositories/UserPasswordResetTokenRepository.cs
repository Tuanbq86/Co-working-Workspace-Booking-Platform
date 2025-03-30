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

public class UserPasswordResetTokenRepository : GenericRepository<UserPasswordResetToken>, IUserPasswordResetTokenRepository
{
    public UserPasswordResetTokenRepository() { }
    public UserPasswordResetTokenRepository(WorkHiveContext context) => _context = context;

    public async Task<string> CreatePasswordResetToken(string email)
    {
        var user = await _context.Users.Where(x => x.Email.ToLower().Trim()
        .Equals(email.ToLower().Trim())).FirstOrDefaultAsync();

        if (user is null) return null!;

        //Xóa tất cả các token cũ trước khi tạo token mới
        var oldTokens = _context.UserPasswordResetTokens.ToList();
        foreach(var item in oldTokens)
        {
            _context.UserPasswordResetTokens.Remove(item);
        }
        
        //Tạo token để reset password
        var token = DateTime.UtcNow.Ticks.ToString()[^6..];
        var expired_Date = DateTime.Now.AddHours(2);

        var resetToken = new UserPasswordResetToken
        {
            Token = token,
            ExpriedAt = expired_Date,
            UserId = user.Id,
            IsUsed = 0
        };

        _context.UserPasswordResetTokens.Add(resetToken);

        await _context.SaveChangesAsync();

        return token;

    }

    public async Task<User> ValidatePasswordResetToken(string token)
    {
        var resetToken = await _context.UserPasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(
                t => t.Token == token && 
                t.IsUsed == 0 && 
                t.ExpriedAt > DateTime.Now
            );

        return resetToken?.User!;
    }
}

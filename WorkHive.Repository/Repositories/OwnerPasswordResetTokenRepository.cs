using Microsoft.EntityFrameworkCore;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

public class OwnerPasswordResetTokenRepository : GenericRepository<OwnerPasswordResetToken>, IOwnerPasswordResetTokenRepository
{
    public OwnerPasswordResetTokenRepository() { }
    public OwnerPasswordResetTokenRepository(WorkHiveContext context) => _context = context;

    public async Task<string> CreatePasswordResetToken(string email)
    {
        var owner = await _context.WorkspaceOwners.Where(x => x.Email.ToLower().Trim()
        .Equals(email.ToLower().Trim())).FirstOrDefaultAsync();

        if (owner is null) return null!;

        //Xóa tất cả các token cũ trước khi tạo token mới
        var oldTokens = _context.OwnerPasswordResetTokens.ToList();
        foreach (var item in oldTokens)
        {
            _context.OwnerPasswordResetTokens.Remove(item);
        }

        //Tạo token để reset password
        var token = DateTime.UtcNow.Ticks.ToString()[^6..];
        var expired_Date = DateTime.Now.AddMinutes(30);

        var resetToken = new OwnerPasswordResetToken
        {
            Token = token,
            ExpiredAt = expired_Date,
            OwnerId = owner.Id,
            IsUsed = 0
        };

        _context.OwnerPasswordResetTokens.Add(resetToken);

        await _context.SaveChangesAsync();

        return token;
    }

    public async Task<WorkspaceOwner> ValidatePasswordResetToken(string token)
    {
        var resetToken = await _context.OwnerPasswordResetTokens
            .Include(t => t.Owner)
            .FirstOrDefaultAsync(
                t => t.Token == token &&
                t.IsUsed == 0 &&
                t.ExpiredAt > DateTime.Now
            );

        return resetToken?.Owner!;
    }
}

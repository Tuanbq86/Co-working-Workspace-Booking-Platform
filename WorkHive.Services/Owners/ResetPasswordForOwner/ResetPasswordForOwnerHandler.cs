using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.EmailServices;

namespace WorkHive.Services.Owners.ResetPasswordForOwner;

public record ResetPasswordForOwnerCommand(string Token, string NewPassword, string ConfirmPassword) : ICommand<ResetPasswordForOwnerResult>;
public record ResetPasswordForOwnerResult(string Notification);

public class ResetPasswordForOwnerHandler(IWorkspaceOwnerUnitOfWork ownerUnit, IEmailService emailService)
    : ICommandHandler<ResetPasswordForOwnerCommand, ResetPasswordForOwnerResult>
{
    public async Task<ResetPasswordForOwnerResult> Handle(ResetPasswordForOwnerCommand command, 
        CancellationToken cancellationToken)
    {
        //Tài khoản owner có token phải trùng với token đã nhập
        //Trạng thái của token là isUsed là 0
        //Hạn của token phải lớn hơn và sau thời điểm hiện tại khi thực hiện reset
        var owner = ownerUnit.OwnerPasswordResetToken.ValidatePasswordResetToken(command.Token);

        if (owner == null)
        {
            return new ResetPasswordForOwnerResult("Token không hợp lệ hoặc đã hết hạn");
        }

        if (!(command.NewPassword.ToLower().Trim().Equals(command.ConfirmPassword.ToLower().Trim())))
        {
            return new ResetPasswordForOwnerResult("Password mới không khớp với confirm password");
        }

        owner.Result.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(command.NewPassword, 13);
        await ownerUnit.WorkspaceOwner.UpdateAsync(owner.Result);

        var usedToken = ownerUnit.OwnerPasswordResetToken.GetAll().FirstOrDefault(t
            => t.Token.ToLower().Trim().Equals(command.Token.ToLower().Trim()));

        if (usedToken != null)
        {
            usedToken.IsUsed = 1;
            await ownerUnit.OwnerPasswordResetToken.UpdateAsync(usedToken);
        }

        return new ResetPasswordForOwnerResult("Mật khẩu đã được đặt lại thành công");
    }
}

using System.Text;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Users.ResetPasswordForUser;

public record ResetPasswordCommand(string Token, string NewPassword, string ConfirmPassword) : ICommand<ResetPasswordResult>;
public record ResetPasswordResult(string Notification);

//public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
//{
//    public ResetPasswordCommandValidator()
//    {
//        RuleFor(x => x.Token)
//            .NotEmpty().WithMessage("Token is required");

//        RuleFor(x => x.NewPassword)
//            .NotEmpty().WithMessage("Password is required");

//        RuleFor(x => x.ConfirmPassword)
//            .Equal(x => x.NewPassword).WithMessage("Passwords do not match");
//    }
//}

public class ResetPasswordHandler(IUserUnitOfWork userUnit)
    : ICommandHandler<ResetPasswordCommand, ResetPasswordResult>
{
    public async Task<ResetPasswordResult> Handle(ResetPasswordCommand command, 
        CancellationToken cancellationToken)
    {
        //Tài khoản user có token phải trùng với token đã nhập
        //Trạng thái của token là isUsed là 0
        //Hạn của token phải lớn hơn và sau thời điểm hiện tại khi thực hiện reset
        var user = userUnit.PasswordResetToken.ValidatePasswordResetToken(command.Token);

        if (user == null)
        {
            return new ResetPasswordResult("Token không hợp lệ hoặc đã hết hạn");
        }

        if( !(command.NewPassword.ToLower().Trim().Equals(command.ConfirmPassword.ToLower().Trim())))
        {
            return new ResetPasswordResult("Password mới không khớp với confirm password");
        }

        user.Result.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(command.NewPassword, 13);
        await userUnit.User.UpdateAsync(user.Result);

        var usedToken = userUnit.PasswordResetToken.GetAll().FirstOrDefault(t 
            => t.Token.ToLower().Trim().Equals(command.Token.ToLower().Trim()));

        if( usedToken != null)
        {
            usedToken.IsUsed = 1;
            await userUnit.PasswordResetToken.UpdateAsync(usedToken);
        }

        return new ResetPasswordResult("Mật khẩu đã được đặt lại thành công");
    }
}

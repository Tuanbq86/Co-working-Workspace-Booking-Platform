using FluentValidation;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.EmailServices;

namespace WorkHive.Services.Users.ResetPasswordForUser;

public record ForgotPasswordCommand(string Email) : ICommand<ForgotPasswordResult>;
public record ForgotPasswordResult(string Notification);

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A validation email format is required");
    }
}

public class ForgotPasswordHandler(IUserUnitOfWork userUnit, IEmailService emailService)
    : ICommandHandler<ForgotPasswordCommand, ForgotPasswordResult>
{
    public async Task<ForgotPasswordResult> Handle(ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        //Hàm xóa tất cả các token cũ trước khi tạo token mới
        //Tạo token ngẫu nhiên theo thời gian 6 chữ số
        //Hạn token sẽ là thời điểm hiện tại thêm 45p
        var token = await userUnit.PasswordResetToken.CreatePasswordResetToken(command.Email);

        if(token is null)
        {
            return new ForgotPasswordResult("Tài khoản không tồn tại");
        }

        var emailBody = $"Token để đặt lại mật khẩu: {token}";
        await emailService.SendEmailAsync(command.Email, "Đặt lại mật khẩu", emailBody);

        return new ForgotPasswordResult("Nếu tài khoản email hợp lệ, vui lòng vào email xem thông tin");
    }
}

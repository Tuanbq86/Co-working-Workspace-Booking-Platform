using FluentValidation;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.EmailServices;
using WorkHive.Services.Users.ResetPasswordForUser;

namespace WorkHive.Services.Owners.ResetPasswordForOwner;

public record ForgotPasswordForOwnerCommand(string Email) : ICommand<ForgotPasswordForOwnerResult>;
public record ForgotPasswordForOwnerResult(string Notification);

public class ForgotPasswordForOwnerCommandValidator : AbstractValidator<ForgotPasswordForOwnerCommand>
{
    public ForgotPasswordForOwnerCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A validation email format is required");
    }
}

public class ForgotPasswordForOwnerHandler(IWorkspaceOwnerUnitOfWork ownerUnit, IEmailService emailService)
    : ICommandHandler<ForgotPasswordForOwnerCommand, ForgotPasswordForOwnerResult>
{
    public async Task<ForgotPasswordForOwnerResult> Handle(ForgotPasswordForOwnerCommand command, 
        CancellationToken cancellationToken)
    {
        //Hàm xóa tất cả các token cũ trước khi tạo token mới
        //Tạo token ngẫu nhiên theo thời gian 6 chữ số
        //Hạn token sẽ là thời điểm hiện tại thêm 45p
        var token = await ownerUnit.OwnerPasswordResetToken.CreatePasswordResetToken(command.Email);

        if (token is null)
        {
            return new ForgotPasswordForOwnerResult("Tài khoản không tồn tại");
        }

        var emailBody = $"Token để đặt lại mật khẩu: {token}";
        await emailService.SendEmailAsync(command.Email, "Đặt lại mật khẩu", emailBody);

        return new ForgotPasswordForOwnerResult("Nếu tài khoản email hợp lệ, vui lòng vào email xem thông tin");
    }
}

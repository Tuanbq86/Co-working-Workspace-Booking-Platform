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

        var emailBody = GenerateOwnerResetPasswordContent(token);
        await emailService.SendEmailAsync(command.Email, "Đặt lại mật khẩu", emailBody);

        return new ForgotPasswordForOwnerResult("Nếu tài khoản email hợp lệ, vui lòng vào email xem thông tin");
    }

    private string GenerateOwnerResetPasswordContent(string? token)
    {
        var sb = new StringBuilder();

        // Hình ảnh tiêu đề
        sb.AppendLine($@"
    <div style='text-align: center; margin-bottom: 20px;'>
        <img src='https://res.cloudinary.com/dcq99dv8p/image/upload/v1743689429/resetpassword_wfrrlb.jpg' 
             style='width: 100%; max-width: 1350px; height: auto; display: block; margin: 0 auto;' 
             alt='User Reset Password'>
    </div>");

        // Nội dung email
        sb.AppendLine($@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <p style='font-size: 16px;'>Xin chào,</p>
    
    <p style='font-size: 16px;'>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản WorkHive của bạn. Vui lòng sử dụng mã OTP dưới đây để hoàn tất quá trình đặt lại mật khẩu:</p>
    
    <p style='font-size: 20px; font-weight: bold; text-align: center; margin: 30px 0;'>
        🔢 <strong>Mã OTP của bạn: {token}</strong>
    </p>
    
    <p style='font-size: 14px; color: #666;'>Mã OTP này có hiệu lực trong 10 phút. Nếu bạn không yêu cầu thay đổi này, vui lòng bỏ qua email này.</p>
    
    <p style='font-size: 16px; margin-top: 30px;'>Nếu bạn cần hỗ trợ, vui lòng liên hệ với chúng tôi qua email <a href='mailto:workhive.vn.official@gmail.com' style='color: #0066cc;'>workhive.vn.official@gmail.com</a> hoặc hotline <a style='color: #0066cc;'>0867435157</a>.</p>
    
    <p style='font-size: 16px; margin-top: 30px;'>Cảm ơn bạn đã sử dụng WorkHive!</p>
    
    <p style='font-size: 16px; margin-top: 50px;'>Trân trọng,<br>
    <strong>Đội ngũ WorkHive</strong></p>
</div>");

        return sb.ToString();
    }
}

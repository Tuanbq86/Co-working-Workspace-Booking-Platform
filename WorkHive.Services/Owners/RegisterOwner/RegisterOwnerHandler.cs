using System.Text;
using FluentValidation;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.EmailServices;
using WorkHive.Services.Exceptions;
using WorkHive.Services.Users.DTOs;

namespace WorkHive.Services.Owner.RegisterOwner;

public record RegisterOwnerCommand(string Email, string Phone, 
    string Password) : ICommand<RegisterOwnerResult>;

public record RegisterOwnerResult(string Notification);

public class RegisterOwnerValidatior : AbstractValidator<RegisterOwnerCommand>
{
    public RegisterOwnerValidatior()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email format is required");

        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required")
            .Length(10).WithMessage("The number of characterics is exact 10 characterics");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}
public class RegisterOwnerHandler(IWorkspaceOwnerUnitOfWork ownerUnit, IEmailService emailService)
    : ICommandHandler<RegisterOwnerCommand, RegisterOwnerResult>
{
    public async Task<RegisterOwnerResult> Handle(RegisterOwnerCommand command, CancellationToken cancellationToken)
    {
        //Checking exist used email and phone number for registering

        var existEmailAndPhoneOwner = ownerUnit.WorkspaceOwner.GetAll().
            Where(x => x.Email.Trim().ToLower().Equals(command.Email.Trim().ToLower()) ||
            x.Phone.Trim().ToLower().Equals(command.Phone.Trim().ToLower())).FirstOrDefault();

        if (existEmailAndPhoneOwner is not null)
            return new RegisterOwnerResult("Email và số điện thoại đã được sử dụng");

        //Create new Owner for registering

        var tempOwner = ownerUnit.WorkspaceOwner.RegisterWorkspaceOwner(command.Email,
            command.Phone, command.Password);

        var newOwner = new WorkspaceOwner
        {
            Email = tempOwner.Email.Trim(),
            Phone = tempOwner.Phone.Trim(),
            //Using Bcrypt to hash password using SHA-512 algorithm
            //Work factor time so long when increment for safety(13)
            Password = BCrypt.Net.BCrypt.EnhancedHashPassword(tempOwner.Password, 13),
            IsBan = 0,
            CreatedAt = DateTime.Now
            
        };

        ownerUnit.WorkspaceOwner.Create(newOwner);

        await ownerUnit.SaveAsync();

        //Send email to confirm registering
        var emailBody = GenerateOwnerRegisterEmailContent(newOwner);
        await emailService.SendEmailAsync(newOwner.Email, "Đăng ký doanh nghiệp thành công", emailBody);

        return new RegisterOwnerResult("Đăng ký thành công, vui lòng kiểm tra email để xem thông tin chi tiết");
    }

    private string GenerateOwnerRegisterEmailContent(WorkspaceOwner owner)
    {
        var sb = new StringBuilder();

        // Hình ảnh tiêu đề
        sb.AppendLine($@"
    <div style='text-align: center; margin-bottom: 20px;'>
        <img src='https://res.cloudinary.com/dcq99dv8p/image/upload/v1743689429/registerOwner_ds4xjd.jpg' 
             style='width: 100%; max-width: 1350px; height: auto; display: block; margin: 0 auto;' 
             alt='Register successfully'>
    </div>");

        // Nội dung email
        sb.AppendLine($@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <p style='font-size: 16px;'>Xin chào,</p>
    
    <p style='font-size: 16px;'>Chúc mừng bạn đã đăng ký thành công tài khoản trên WorkHive! 🚀</p>
    <p style='font-size: 16px;'>Giờ đây, bạn có thể bắt đầu quản lý không gian làm việc của mình, đăng tải thông tin, theo dõi đặt chỗ, và kết nối với khách hàng một cách dễ dàng.</p>
    
    <p style='font-size: 16px; margin-top: 30px;'><strong>Bắt đầu ngay!</strong></p>
    <p style='font-size: 16px;'>👉 <a href='https://workhive-owners.vercel.app/' style='color: #0066cc;'>Đăng nhập ngay</a> để thiết lập không gian làm việc của bạn.</p>
    
    <p style='font-size: 16px; margin-top: 30px;'>Nếu bạn cần hỗ trợ, vui lòng liên hệ với chúng tôi qua email <a href='mailto:workhive.vn.official@gmail.com' style='color: #0066cc;'>workhive.vn.official@gmail.com</a> hoặc hotline <a style='color: #0066cc;'>0867435157</a>.</p>
    
    <p style='font-size: 16px;'>Chúc bạn có trải nghiệm tuyệt vời cùng WorkHive!</p>
    
    <p style='font-size: 16px; margin-top: 50px;'>Trân trọng,<br>
    🌟 Đội ngũ WorkHive</p>
</div>");

        return sb.ToString();
    }
}

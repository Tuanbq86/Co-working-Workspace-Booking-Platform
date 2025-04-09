using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Services.EmailServices;

namespace WorkHive.Services.Admins.SupportCustomer;

public record SupportCustomerCommand(string Name, string Email, string Phone, string Message) 
    : ICommand<SupportCustomerResult>;
public record SupportCustomerResult(string Notification);

public class SupportCustomerHandler(IEmailService emailService)
    : ICommandHandler<SupportCustomerCommand, SupportCustomerResult>
{
    public async Task<SupportCustomerResult> Handle(SupportCustomerCommand command, 
        CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(command.Name) || string.IsNullOrEmpty(command.Email) 
            || string.IsNullOrEmpty(command.Phone) || string.IsNullOrEmpty(command.Message) || command.Phone.Length == 10)
            return new SupportCustomerResult("Vui lòng nhập đầy đủ thông tin");

        //Send email to confirm registering
        var emailBody = GenerateSupportCustomerEmailContent(command.Name, command.Email, command.Phone, command.Message);
        await emailService.SendEmailAsync(command.Email, "Gửi hỗ trợ thành công", emailBody);

        return new SupportCustomerResult("Gửi hỗ trợ thành công, vui lòng kiểm tra email để xem thông tin chi tiết");
    }

    private string GenerateSupportCustomerEmailContent(string Name, string Email, string Phone, string Message)
    {
        var sb = new StringBuilder();

        // Nội dung email
        sb.AppendLine($@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <p style='font-size: 16px;'>Kính gửi: Bộ phận hỗ trợ của WorkHive.</p>
    
    <p style='font-size: 16px;'>Tôi tên: {Name}</p>
    
    <p style='font-size: 16px;'>Email: {Email}</p>

    <p style='font-size: 16px;'>Số điện thoại: {Phone}</p>
    
    <p style='font-size: 16px;'>Nội dung: {Message}</p>
</div>");

        return sb.ToString();
    }
}

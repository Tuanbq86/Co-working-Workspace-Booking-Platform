using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace WorkHive.Services.EmailServices;

public class EmailService(IConfiguration _configuration) 
    : IEmailService
{
    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");

        using (var client = new SmtpClient(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]!)))
        {
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(
                emailSettings["SmtpUsername"],
                emailSettings["SmtpPassword"]);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["FromEmail"]!, emailSettings["FromName"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}

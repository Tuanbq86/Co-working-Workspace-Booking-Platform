using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.EmailServices;

namespace WorkHive.Services.Staff
{
    public record UpdateOwnerStatusCommand(int Id, int UserId, string Message, string Status) : ICommand<UpdateOwnerStatusResult>;

    public record UpdateOwnerStatusResult(string Notification);

    public class UpdateOwnerStatusHandler(IWalletUnitOfWork unit, IWorkSpaceManageUnitOfWork OUnit, IEmailService emailService) : ICommandHandler<UpdateOwnerStatusCommand, UpdateOwnerStatusResult>
    {
        public async Task<UpdateOwnerStatusResult> Handle(UpdateOwnerStatusCommand command, CancellationToken cancellationToken)
        {
            var owner = await unit.WorkspaceOwner.GetByIdAsync(command.Id);
            if (owner == null) return new UpdateOwnerStatusResult("Owner not found");

            if (command.Status != "Fail" && command.Status != "Success")
                return new UpdateOwnerStatusResult("Invalid status value. Use 'Fail' or 'Success'.");

            owner.Status = command.Status;
            owner.UpdatedAt = DateTime.Now;

            //================================================================
            var verifyRequest = await OUnit.OwnerVerifyRequest.GetByOwnerIdAsync(command.Id, "Handling");

            if (verifyRequest == null)
                return new UpdateOwnerStatusResult("Verify request not found");
            verifyRequest.UserId = command.UserId;
            verifyRequest.Status = command.Status;
            verifyRequest.Message = command.Message;
            verifyRequest.UpdatedAt = DateTime.Now;

            await OUnit.OwnerVerifyRequest.UpdateAsync(verifyRequest);

            //================================================================

            if (command.Status == "Success")
            {
                // Create a new OwnerNotification
                var ownerNotification = new OwnerNotification
                {
                    Description = $"Tài khoản {owner.LicenseName} của bạn đã được phê duyệt và xác thực thành công. Bây giờ bạn có thể truy cập các tính năng đầy đủ.",
                    Status = "Active",
                    OwnerId = command.Id,
                    CreatedAt = DateTime.Now,
                    IsRead = 0,
                    Title = "Xác thực tài khoản thành công"
                };

                await unit.OwnerNotification.CreateAsync(ownerNotification);
            }
            else
            {
                // Create a new OwnerNotification
                var ownerNotification = new OwnerNotification
                {
                    Description = $"Tài khoản {owner.LicenseName} của bạn đã bị từ chối. Vui lòng kiểm tra lại thông tin và gửi yêu cầu xác thực lại.",
                    Status = "Active",
                    OwnerId = command.Id,
                    CreatedAt = DateTime.Now,
                    IsRead = 0,
                    Title = "Xác thực tài khoản không thành công"
                };
                await unit.OwnerNotification.CreateAsync(ownerNotification);
            }

            //================================================================

            var existingWallet = await unit.OwnerWallet.GetByOwnerIdAsync(command.Id);
            var walletStatus = command.Status == "Success" ? "Active" : "Inactive"; 

            if (existingWallet == null)
            {
                var newWallet = new Wallet
                {
                    Balance = 0,
                    Status = walletStatus 
                };

                await unit.Wallet.CreateAsync(newWallet);

                var ownerWallet = new OwnerWallet
                {
                    OwnerId = owner.Id,
                    WalletId = newWallet.Id,
                    UserId = command.UserId,
                    Status = walletStatus 
                };

                await unit.OwnerWallet.CreateAsync(ownerWallet);
            }
            else
            {               
                existingWallet.UserId = command.UserId;
                existingWallet.Status = walletStatus; 
                await unit.OwnerWallet.UpdateAsync(existingWallet);
            }

            //================================================================

            await unit.WorkspaceOwner.UpdateAsync(owner);
            var emailBody = GenerateStatusEmailContent(owner.LicenseName, command.Status);
            var subject = command.Status == "Success" ? "Xác thực tài khoản thành công" : "Xác thực tài khoản không thành công";
            await emailService.SendEmailAsync(owner.Email, subject, emailBody);

            await unit.SaveAsync(); 

            return new UpdateOwnerStatusResult($"Owner status updated to {command.Status} and wallet set to '{walletStatus}'");
        }

        //    private string GenerateStatusEmailContent(string licenseName, string status)
        //    {
        //        var sb = new StringBuilder();

        //        sb.AppendLine($@"
        //<div style='text-align: center; margin-bottom: 20px;'>
        //    <img src='https://res.cloudinary.com/dcq99dv8p/image/upload/v1745459606/XacThucOwner_wyqshs.jpg' 
        //         style='width: 100%; max-width: 1350px; height: auto; display: block; margin: 0 auto;' 
        //         alt='Status Notification'>
        //</div>");


        //        sb.AppendLine("<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>");

        //        if (status == "Success")
        //        {
        //            sb.AppendLine($@"
        //        <p style='font-size: 16px;'>Xin chúc mừng,</p>
        //        <p style='font-size: 16px;'>Tài khoản <strong>{licenseName}</strong> của bạn đã được <strong>phê duyệt và xác thực thành công</strong> ✅</p>
        //        <p style='font-size: 16px;'>Bạn đã có thể truy cập đầy đủ các tính năng của hệ thống WorkHive.</p>
        //    ");
        //        }
        //        else
        //        {
        //            sb.AppendLine($@"
        //        <p style='font-size: 16px;'>Xin chào,</p>
        //        <p style='font-size: 16px;'>Rất tiếc, tài khoản <strong>{licenseName}</strong> của bạn đã bị <strong>từ chối xác thực</strong> ❌</p>
        //        <p style='font-size: 16px;'>Vui lòng kiểm tra lại thông tin và gửi lại yêu cầu xác thực.</p>
        //    ");
        //        }

        //        sb.AppendLine($@"
        //    <p style='font-size: 16px; margin-top: 30px;'>Nếu cần hỗ trợ, vui lòng liên hệ <a href='mailto:workhive.vn.official@gmail.com' style='color: #0066cc;'>workhive.vn.official@gmail.com</a> hoặc gọi đến <a style='color: #0066cc;'>0867435157</a>.</p>
        //    <p style='font-size: 16px;'>Trân trọng,<br>🌟 Đội ngũ WorkHive</p>
        //</div>");

        //        return sb.ToString();
        //    }



        private string GenerateStatusEmailContent(string licenseName, string status)
        {
            var sb = new StringBuilder();

            // Chọn ảnh khác nhau tùy theo status
            string imageUrl = status == "Success"
                ? "https://res.cloudinary.com/dcq99dv8p/image/upload/v1745459606/XacThucOwner_wyqshs.jpg"
                : "https://res.cloudinary.com/dcq99dv8p/image/upload/v1745460365/FailXacThuc_zi8w39.jpg";

            sb.AppendLine($@"
            <div style='text-align: center; margin-bottom: 20px;'>
                <img src='{imageUrl}' 
                     style='width: 100%; max-width: 1350px; height: auto; display: block; margin: 0 auto;' 
                     alt='Status Notification'>
            </div>");

                        sb.AppendLine("<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>");

                        if (status == "Success")
                        {
                            sb.AppendLine($@"
                    <p style='font-size: 16px;'>Xin chúc mừng,</p>
                    <p style='font-size: 16px;'>Tài khoản <strong>{licenseName}</strong> của bạn đã được <strong>phê duyệt và xác thực thành công</strong> ✅</p>
                    <p style='font-size: 16px;'>Bạn đã có thể truy cập đầy đủ các tính năng của hệ thống WorkHive.</p>
                ");
                        }
                        else
                        {
                            sb.AppendLine($@"
                    <p style='font-size: 16px;'>Xin chào,</p>
                    <p style='font-size: 16px;'>Rất tiếc, tài khoản <strong>{licenseName}</strong> của bạn đã bị <strong>từ chối xác thực</strong> ❌</p>
                    <p style='font-size: 16px;'>Vui lòng kiểm tra lại thông tin và gửi lại yêu cầu xác thực.</p>
                ");
                        }

                        sb.AppendLine($@"
                <p style='font-size: 16px; margin-top: 30px;'>Nếu cần hỗ trợ, vui lòng liên hệ <a href='mailto:workhive.vn.official@gmail.com' style='color: #0066cc;'>workhive.vn.official@gmail.com</a> hoặc gọi đến <a style='color: #0066cc;'>0867435157</a>.</p>
                <p style='font-size: 16px;'>Trân trọng,<br>🌟 Đội ngũ WorkHive</p>
            </div>");

            return sb.ToString();
        }


    }
}

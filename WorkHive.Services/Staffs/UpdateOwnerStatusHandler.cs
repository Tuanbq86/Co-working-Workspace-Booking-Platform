using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Staff
{
    public record UpdateOwnerStatusCommand(int Id, int UserId, string Message, string Status) : ICommand<UpdateOwnerStatusResult>;

    public record UpdateOwnerStatusResult(string Notification);

    public class UpdateOwnerStatusHandler(IWalletUnitOfWork unit, IWorkSpaceManageUnitOfWork OUnit) : ICommandHandler<UpdateOwnerStatusCommand, UpdateOwnerStatusResult>
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
            // Create a new OwnerVerifyRequest
            //var newOwnerVerifyRequest = new OwnerVerifyRequest
            //{
            //    OwnerId = command.Id,
            //    UserId = command.UserId,
            //    Status = command.Status,
            //    Message = command.Message,
            //    GoogleMapUrl = owner.GoogleMapUrl,
            //    LicenseName = owner.LicenseName,
            //    LicenseNumber = owner.LicenseNumber,
            //    LicenseAddress = owner.LicenseAddress,
            //    CharterCapital = owner.CharterCapital,
            //    LicenseFile = owner.LicenseFile,
            //    OwnerName = owner.OwnerName,
            //    RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
            //};
            //await OUnit.OwnerVerifyRequest.CreateAsync(newOwnerVerifyRequest);

            //================================================================

            if (command.Status != "Success")
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
            await unit.SaveAsync(); 

            return new UpdateOwnerStatusResult($"Owner status updated to {command.Status} and wallet set to '{walletStatus}'");
        }
    }
}

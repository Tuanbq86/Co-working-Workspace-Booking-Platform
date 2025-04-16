using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.Base_Owner
{
    public record VerifyOwnerCommand(
        int Id,
        string OwnerName,
        string Sex,
        string GoogleMapUrl,
        string LicenseName,
        string LicenseNumber,
        string LicenseAddress,
        decimal? CharterCapital,
        string LicenseFile,
        string? Facebook,
        string? Instagram,
        string? Tiktok,
        DateOnly? RegistrationDate
        ) : ICommand<VerifyOwnerResult>;

    public record VerifyOwnerResult(string Notification);

    public class VerifyOwnerHandler(IWorkSpaceManageUnitOfWork unit) : ICommandHandler<VerifyOwnerCommand, VerifyOwnerResult>
    {
        public async Task<VerifyOwnerResult> Handle(VerifyOwnerCommand command, CancellationToken cancellationToken)
        {
            var owner = await unit.WorkspaceOwner.GetByIdAsync(command.Id);
            if (owner == null) return new VerifyOwnerResult("Owner not found");

            owner.Sex = command.Sex;
            owner.OwnerName = command.OwnerName;
            owner.GoogleMapUrl = command.GoogleMapUrl;
            owner.LicenseName = command.LicenseName;
            owner.LicenseNumber = command.LicenseNumber;
            owner.LicenseAddress = command.LicenseAddress;
            owner.CharterCapital = command.CharterCapital;
            owner.LicenseFile = command.LicenseFile;
            owner.UpdatedAt = DateTime.Now;
            owner.Status = "Handling";
            owner.Facebook = command.Facebook;
            owner.Instagram = command.Instagram;
            owner.Tiktok = command.Tiktok;
            owner.RegistrationDate = command.RegistrationDate;

            var newOwnerVerifyRequest = new OwnerVerifyRequest
            {
                OwnerId = command.Id,
                //UserId = command.UserId,
                //Status = command.Status,
                //Message = command.Message,
                GoogleMapUrl = owner.GoogleMapUrl,
                LicenseName = owner.LicenseName,
                LicenseNumber = owner.LicenseNumber,
                LicenseAddress = owner.LicenseAddress,
                CharterCapital = owner.CharterCapital,
                LicenseFile = owner.LicenseFile,
                OwnerName = owner.OwnerName,
                RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
            };


            await unit.OwnerVerifyRequest.CreateAsync(newOwnerVerifyRequest);


            await unit.WorkspaceOwner.UpdateAsync(owner);
            await unit.SaveAsync();

            return new VerifyOwnerResult("Owner verification updated successfully");
        }
    }
}
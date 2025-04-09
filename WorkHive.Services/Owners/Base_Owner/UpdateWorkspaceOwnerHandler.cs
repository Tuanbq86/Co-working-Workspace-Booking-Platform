using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.Base_Owner
{
    public record UpdateWorkspaceOwnerCommand(int Id, string Phone, string Email, string IdentityName, string IdentityNumber, DateOnly? DateOfBirth, string Sex, string Nationality, string PlaceOfOrigin, string GoogleMapUrl, string Status, string PlaceOfResidence, DateOnly? IdentityExpiredDate, DateOnly? IdentityCreatedDate, string IdentityFile, string LicenseName, string LicenseNumber, string LicenseAddress, decimal? CharterCapital, string LicenseFile, string? Facebook, string? Instagram, string? Tiktok, string PhoneStatus)
        : ICommand<UpdateWorkspaceOwnerResult>;

    public record UpdateWorkspaceOwnerResult(string Notification);

    public class UpdateWorkspaceOwnerHandler(IWorkSpaceManageUnitOfWork unit) : ICommandHandler<UpdateWorkspaceOwnerCommand, UpdateWorkspaceOwnerResult>
    {
        public async Task<UpdateWorkspaceOwnerResult> Handle(UpdateWorkspaceOwnerCommand command, CancellationToken cancellationToken)
        {
            var owner = await unit.WorkspaceOwner.GetByIdAsync(command.Id);
            if (owner == null) return new UpdateWorkspaceOwnerResult("WorkspaceOwner not found");

            owner.Phone = command.Phone;
            owner.Email = command.Email;
            owner.IdentityName = command.IdentityName;
            owner.IdentityNumber = command.IdentityNumber;
            owner.DateOfBirth = command.DateOfBirth;
            owner.Sex = command.Sex;
            owner.Nationality = command.Nationality;
            owner.PlaceOfOrigin = command.PlaceOfOrigin;
            owner.GoogleMapUrl = command.GoogleMapUrl;
            owner.Status = command.Status;
            owner.PlaceOfResidence = command.PlaceOfResidence;
            owner.IdentityExpiredDate = command.IdentityExpiredDate;
            owner.IdentityCreatedDate = command.IdentityCreatedDate;
            owner.IdentityFile = command.IdentityFile;
            owner.LicenseName = command.LicenseName;
            owner.LicenseNumber = command.LicenseNumber;
            owner.LicenseAddress = command.LicenseAddress;
            owner.CharterCapital = command.CharterCapital;
            owner.LicenseFile = command.LicenseFile;
            owner.Facebook = command.Facebook;
            owner.Instagram = command.Instagram;
            owner.Tiktok = command.Tiktok;
            owner.PhoneStatus = command.PhoneStatus;
            owner.UpdatedAt = DateTime.Now;

            await unit.WorkspaceOwner.UpdateAsync(owner);
            await unit.SaveAsync();

            return new UpdateWorkspaceOwnerResult("WorkspaceOwner updated successfully");
        }
    }
}

using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.Base_Owner
{
    public record GetWorkspaceOwnerByIdQuery(int Id) : IQuery<GetWorkspaceOwnerByIdResult>;

    public record GetWorkspaceOwnerByIdResult(
        int Id,
        string Phone,
        string Email,
        string IdentityName,
        string IdentityNumber,
        DateOnly? DateOfBirth,
        string Sex,
        string Nationality,
        string PlaceOfOrigin,
        DateTime? CreatedAt,
        DateTime? UpdatedAt,
        string GoogleMapUrl,
        string Status,
        string PlaceOfResidence,
        DateOnly? IdentityExpiredDate,
        DateOnly? IdentityCreatedDate,
        string IdentityFile,
        string LicenseName,
        string LicenseNumber,
        string LicenseAddress,
        decimal? CharterCapital,
        string LicenseFile,
        string Facebook,
        string Instagram,
        string Tiktok,
        string PhoneStatus,
        string Message,
        int? UserId
    );

    public class GetWorkspaceOwnerByIdValidator : AbstractValidator<GetWorkspaceOwnerByIdQuery>
    {
        public GetWorkspaceOwnerByIdValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid Owner ID");
        }
    }

    public class GetWorkspaceOwnerByIdHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
        : IQueryHandler<GetWorkspaceOwnerByIdQuery, GetWorkspaceOwnerByIdResult>
    {
        public async Task<GetWorkspaceOwnerByIdResult> Handle(GetWorkspaceOwnerByIdQuery query,
            CancellationToken cancellationToken)
        {
            var owner = await workSpaceManageUnit.WorkspaceOwner.GetOwnerByIdAsync(query.Id);
            if (owner == null)
            {
                return null;
            }

            return new GetWorkspaceOwnerByIdResult(
                owner.Id,
                owner.Phone,
                owner.Email,
                owner.IdentityName,
                owner.IdentityNumber,
                owner.DateOfBirth,
                owner.Sex,
                owner.Nationality,
                owner.PlaceOfOrigin,
                owner.CreatedAt,
                owner.UpdatedAt,
                owner.GoogleMapUrl,
                owner.Status,
                owner.PlaceOfResidence,
                owner.IdentityExpiredDate,
                owner.IdentityCreatedDate,
                owner.IdentityFile,
                owner.LicenseName,
                owner.LicenseNumber,
                owner.LicenseAddress,
                owner.CharterCapital,
                owner.LicenseFile,
                owner.Facebook,
                owner.Instagram,
                owner.Tiktok,
                owner.PhoneStatus,
                owner.Message,
                owner.OwnerWallets.FirstOrDefault()?.UserId
            );
        }
    }
}
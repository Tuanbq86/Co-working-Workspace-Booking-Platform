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
        string Sex,
        DateTime? CreatedAt,
        DateTime? UpdatedAt,
        string GoogleMapUrl,
        string Status,
        string LicenseName,
        string LicenseNumber,
        string LicenseAddress,
        decimal? CharterCapital,
        string LicenseFile,
        string Facebook,
        string Instagram,
        string Tiktok,
        string PhoneStatus,
        string Avatar,
        string OwnerName,
        DateOnly? RegistrationDate,
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
                owner.Sex,
                owner.CreatedAt,
                owner.UpdatedAt,
                owner.GoogleMapUrl,
                owner.Status,
                owner.LicenseName,
                owner.LicenseNumber,
                owner.LicenseAddress,
                owner.CharterCapital,
                owner.LicenseFile,
                owner.Facebook,
                owner.Instagram,
                owner.Tiktok,
                owner.PhoneStatus,
                owner.Avatar,
                owner.OwnerName,
                owner.RegistrationDate,
                owner.OwnerWallets.FirstOrDefault()?.UserId
            );
        }
    }
}
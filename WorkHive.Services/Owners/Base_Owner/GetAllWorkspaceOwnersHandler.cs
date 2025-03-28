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

    public record GetAllWorkspaceOwnersQuery() : IQuery<List<GetWorkspaceOwnersResult>>;

    public record GetWorkspaceOwnersResult(
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
        string Message
    );


    public class GetAllWorkspaceOwnersValidator : AbstractValidator<GetAllWorkspaceOwnersQuery>
    {
        public GetAllWorkspaceOwnersValidator()
        {
        }
    }

    public class GetAllWorkspaceOwnersHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
    : IQueryHandler<GetAllWorkspaceOwnersQuery, List<GetWorkspaceOwnersResult>>
    {
        public async Task<List<GetWorkspaceOwnersResult>> Handle(GetAllWorkspaceOwnersQuery query,
            CancellationToken cancellationToken)
        {
            var owners = await workSpaceManageUnit.WorkspaceOwner.GetAllAsync();
            if (owners == null || !owners.Any())
            {
                return new List<GetWorkspaceOwnersResult>();
            }

            return owners.Select(owner => new GetWorkspaceOwnersResult(
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
           owner.Message
       )).ToList();
        }
    }

}

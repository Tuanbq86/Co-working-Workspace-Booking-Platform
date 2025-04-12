using Azure.Core;
using FluentValidation;
using MediatR;
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


    public class GetAllWorkspaceOwnersValidator : AbstractValidator<GetAllWorkspaceOwnersQuery>
    {
        public GetAllWorkspaceOwnersValidator()
        {
        }
    }

    public class GetAllWorkspaceOwnersHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit, IWalletUnitOfWork unit)
    : IQueryHandler<GetAllWorkspaceOwnersQuery, List<GetWorkspaceOwnersResult>>
    {
        public async Task<List<GetWorkspaceOwnersResult>> Handle(GetAllWorkspaceOwnersQuery query,
            CancellationToken cancellationToken)
        {
            var owners = await workSpaceManageUnit.WorkspaceOwner.GetAllOwnersAsync();
            if (owners == null || !owners.Any())
            {
                return new List<GetWorkspaceOwnersResult>();
            }

            return owners.Select(owner => new GetWorkspaceOwnersResult(
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
       )).ToList();
        }
    }

}

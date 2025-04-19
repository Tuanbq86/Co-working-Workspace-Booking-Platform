using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.BuildingBlocks.Exceptions;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Exceptions;
using WorkHive.Services.Owners.ManageWorkSpace.GetAllById;
using WorkHive.Services.Users.LoginUser;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Workspace
{
    public record GetWorkSpaceByIdQuery(int id) : IQuery<GetWorkSpaceByIdResult>;
    public record GetWorkSpaceByIdResult(int Id, string Name, string Description,string Address, int? Capacity, string GoogleMapUrl, string Category, string Status, DateTime? CreatedAt, DateTime? UpdatedAt,int? CleanTime, int? Area, int OwnerId, TimeOnly? OpenTime, TimeOnly? CloseTime, int? Is24h, string LicenseName, string phone, List<WorkspacePriceDTO> Prices,
    List<WorkspaceImageDTO> Images, List<WorkspaceFacilityDT> Facilities, List<WorkspacePolicyDT> Policies, List<WorkspaceDetailDT> Details);

    public record WorkspacePriceDTO(int Id, decimal? Price, string Category);
    public record WorkspaceImageDTO(int Id, string ImgUrl);
    public record WorkspaceFacilityDT(int Id, string FacilityName);
    public record WorkspacePolicyDT(int Id, string PolicyName);
    public record WorkspaceDetailDT(int Id, string DetailName);



    public class GetWorkSpaceByIdValidator : AbstractValidator<GetWorkSpaceByIdQuery>
    {
        public GetWorkSpaceByIdValidator()
        {

        }
    }

    public class GetWorkSpaceByIdHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
    : IQueryHandler<GetWorkSpaceByIdQuery, GetWorkSpaceByIdResult>
    {
        public async Task<GetWorkSpaceByIdResult> Handle(GetWorkSpaceByIdQuery query, CancellationToken cancellationToken)
        {
            var workspace = await workSpaceManageUnit.Workspace.GetWorkSpaceById(query.id);

            if (workspace == null)
            {
                return null;
            }

            WorkspaceOwner owner = await workSpaceManageUnit.WorkspaceOwner.GetByIdAsync(workspace.OwnerId);
            return new GetWorkSpaceByIdResult(
                workspace.Id,
                workspace.Name,              
                workspace.Description,
                owner.LicenseAddress,
                workspace.Capacity,
                owner.GoogleMapUrl,
                workspace.Category,
                workspace.Status,
                workspace.CreatedAt,
                workspace.UpdatedAt,
                workspace.CleanTime,
                workspace.Area,
                workspace.OwnerId,
                workspace.OpenTime,
                workspace.CloseTime,
                workspace.Is24h,
                workspace.Owner.LicenseName,
                workspace.Owner.Phone,
                workspace.WorkspacePrices?.Where(wp => wp != null && wp.Price != null)
                    .Select(wp => new WorkspacePriceDTO(
                        wp.Id,
                        wp.Price!.AveragePrice,
                        wp.Price.Category
                    )).ToList() ?? new List<WorkspacePriceDTO>(),
                workspace.WorkspaceImages?.Where(wi => wi != null && wi.Image != null)
                    .Select(wi => new WorkspaceImageDTO(
                        wi.Image!.Id,
                        wi.Image.ImgUrl ?? string.Empty
                    )).ToList() ?? new List<WorkspaceImageDTO>(),
                workspace.WorkspaceFacilities?.Where(wf => wf != null && wf.Facility != null)
                    .Select(wf => new WorkspaceFacilityDT(
                        wf.Facility!.Id,
                        wf.Facility.Name ?? string.Empty
                    )).ToList() ?? new List<WorkspaceFacilityDT>(),
                workspace.WorkspacePolicies?.Where(wp => wp != null && wp.Policy != null)
                    .Select(wp => new WorkspacePolicyDT(
                        wp.Policy!.Id,
                        wp.Policy.Name ?? string.Empty
                    )).ToList() ?? new List<WorkspacePolicyDT>(),
                workspace.WorkspaceDetails?.Where(wd => wd != null && wd.Detail != null)
                    .Select(wd => new WorkspaceDetailDT(
                        wd.Detail!.Id,
                        wd.Detail.Name ?? string.Empty
                    )).ToList() ?? new List<WorkspaceDetailDT>()


            );
        }
    }

}

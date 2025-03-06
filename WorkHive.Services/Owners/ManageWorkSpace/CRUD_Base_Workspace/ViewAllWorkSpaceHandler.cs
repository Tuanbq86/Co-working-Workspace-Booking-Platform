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
using WorkHive.Services.Owners.LoginOwner;
using WorkHive.Services.Owners.ManageWorkSpace.GetAllById;
using WorkHive.Services.Users.BookingWorkspace;

namespace WorkHive.Services.Owners.ManageWorkSpace.CRUD_Base_Workspace;
public record GetWorkSpacesQuery() : IQuery<List<GetWorkSpacesResult>>;

public record GetWorkSpacesResult(int Id, string Name, string Address, string GoogleMapUrl, string Description, int? Capacity, string Category,
    string Status, int? CleanTime, int? Area, int OwnerId, List<WorkspacesPriceDTO> Prices,
List<WorkspacesImageDTO> Images);

public record WorkspacesPriceDTO(int Id, decimal? Price, string Category);
public record WorkspacesImageDTO(int Id, string ImgUrl);


public class GetWorkSpacesValidator : AbstractValidator<GetWorkSpacesQuery>
{
    public GetWorkSpacesValidator()
    {
    }
}
public class GetWorkSpacesHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
: IQueryHandler<GetWorkSpacesQuery, List<GetWorkSpacesResult>>
{
    public async Task<List<GetWorkSpacesResult>> Handle(GetWorkSpacesQuery Query,
        CancellationToken cancellationToken)
    {
        var workspaces = await workSpaceManageUnit.Workspace.GetAllWorkSpaceAsync();

        var ownerIds = workspaces.Select(ws => ws.OwnerId).Distinct().ToList();
        var owners = await workSpaceManageUnit.WorkspaceOwner.GetOwnersByIdsAsync(ownerIds);
        return workspaces.Select(ws =>
        {
            // Lấy owner tương ứng với workspace
            var owner = owners.FirstOrDefault(o => o.Id == ws.OwnerId);

            return new GetWorkSpacesResult(
                ws.Id,
                ws.Name,
                owner?.LicenseAddress ?? string.Empty,  
                owner?.GoogleMapUrl ?? string.Empty,   
                ws.Description,
                ws.Capacity,
                ws.Category,
                ws.Status,
                ws.CleanTime,
                ws.Area,
                ws.OwnerId,
               ws.WorkspacePrices.Select(wp => new WorkspacesPriceDTO(
                wp.Price.Id,
                wp.Price.Price1,
                wp.Price.Category
            )).ToList(),
            ws.WorkspaceImages.Select(wi => new WorkspacesImageDTO(
                wi.Image.Id,
                wi.Image.ImgUrl
            )).ToList()
            );
        }).ToList();
    }
}
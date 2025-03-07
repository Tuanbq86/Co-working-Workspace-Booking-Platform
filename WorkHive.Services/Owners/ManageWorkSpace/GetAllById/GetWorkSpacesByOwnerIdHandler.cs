using FluentValidation;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.BuildingBlocks.Exceptions;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.GetAllById;

public record GetWorkSpacesByOwnerIdQuery(int Id) : IQuery<List<GetWorkSpaceByOwnerIdResult>>;

public record GetWorkSpaceByOwnerIdResult(int Id, string Name, string Address, string GoogleMapUrl, string Description, int? Capacity, string Category, 
    string Status, int? CleanTime, int? Area, int OwnerId, List<WorkspacesPriceDTO> Prices,
List<WorkspacesImageDTO> Images);

public record WorkspacesPriceDTO(int Id, decimal? Price, string Category);
public record WorkspacesImageDTO(int Id, string ImgUrl);


public class GetWorkSpacesByOwnerIdValidator : AbstractValidator<GetWorkSpacesByOwnerIdQuery>
{
    public GetWorkSpacesByOwnerIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Owner ID must be greater than 0");
    }
}
public class GetWorkSpacesByOwnerIdHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
: IQueryHandler<GetWorkSpacesByOwnerIdQuery, List<GetWorkSpaceByOwnerIdResult>>
{
    public async Task<List<GetWorkSpaceByOwnerIdResult>> Handle(GetWorkSpacesByOwnerIdQuery Query, 
        CancellationToken cancellationToken)
    {
        var workspaces = await workSpaceManageUnit.Workspace.GetAllWorkSpaceByOwnerIdAsync(Query.Id);

        if (workspaces == null || !workspaces.Any())
        {
            throw new NotFoundException($"No workspaces found for OwnerId {Query.Id}");
        }
        WorkspaceOwner owner = await workSpaceManageUnit.WorkspaceOwner.GetByIdAsync(Query.Id);
        return workspaces.Select(ws => new GetWorkSpaceByOwnerIdResult(
            ws.Id,
            ws.Name,
            owner.LicenseAddress,
            owner.GoogleMapUrl,
            ws.Description,
            ws.Capacity,
            ws.Category,
            ws.Status,
            ws.CleanTime,
            ws.Area,
            ws.OwnerId,
            ws.WorkspacePrices.Select(wp => new WorkspacesPriceDTO(
                wp.Price.Id,
                wp.Price.AveragePrice,
                wp.Price.Category
            )).ToList(),
            ws.WorkspaceImages.Select(wi => new WorkspacesImageDTO(
                wi.Image.Id,
                wi.Image.ImgUrl
            )).ToList()
            )).ToList();
    }
}

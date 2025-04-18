using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Users.SearchWorkspace;

public record SearchWorkspaceByOwnerNameQuery(string Name)
    : IQuery<SearchWorkspaceByOwnerNameResult>;
public record SearchWorkspaceByOwnerNameResult(List<WorkspaceSearchByOwnerNameDTO> WorkspaceSearchByOwnerNameDTOs);

public record WorkspaceSearchByOwnerNameDTO(int Id, string Name, string Address, string GoogleMapUrl, string Description, int? Capacity,
    string Category, string Status, DateTime CreatedAt, DateTime UpdatedAt, int? CleanTime,
    int? Area, TimeOnly? OpenTime, TimeOnly? CloseTime, int? Is24h, List<WorkspacesPriceByOwnerNameSearchDTO> Prices,
List<WorkspacesImageByOwnerNameSearchDTO> Images, List<WorkspaceFacilityByOwnerNameSearchDTO> Facilities, List<WorkspacePolicyByOwnerNameSearchDTO> Policies);

public record WorkspacesPriceByOwnerNameSearchDTO(int Id, decimal? Price, string Category);
public record WorkspacesImageByOwnerNameSearchDTO(int Id, string ImgUrl);
public record WorkspaceFacilityByOwnerNameSearchDTO(int Id, string FacilityName);
public record WorkspacePolicyByOwnerNameSearchDTO(int Id, string PolicyName);


public class SearchWorkspaceByOwnerNameHandler(IBookingWorkspaceUnitOfWork bookingUnit)
    : IQueryHandler<SearchWorkspaceByOwnerNameQuery, SearchWorkspaceByOwnerNameResult>
{
    public async Task<SearchWorkspaceByOwnerNameResult> Handle(SearchWorkspaceByOwnerNameQuery query, 
        CancellationToken cancellationToken)
    {
        var workspaces = bookingUnit.workspace.GetWorkspaceByWorkspaceNameSearch(query.Name);

        if (!string.IsNullOrEmpty(query.Name))
        {
            workspaces = workspaces
                .Where(w => EF.Functions.Like(w.Owner.LicenseName, $"%{query.Name}%"));
        }

        List<WorkspaceSearchByOwnerNameDTO> result = new List<WorkspaceSearchByOwnerNameDTO>();

        foreach (var item in workspaces)
        {
            result.Add(new WorkspaceSearchByOwnerNameDTO(
                item.Id,
                item.Name,
                item.Owner.LicenseAddress,
                item.Owner.GoogleMapUrl,
                item.Description,
                item.Capacity,
                item.Category,
                item.Status,
                item.CreatedAt.GetValueOrDefault(DateTime.MinValue),
                item.UpdatedAt.GetValueOrDefault(DateTime.MinValue),
                item.CleanTime ?? 0,
                item.Area ?? 0,
                item.OpenTime,
                item.CloseTime,
                item.Is24h ?? 0,
                item.WorkspacePrices.Select(wp => new WorkspacesPriceByOwnerNameSearchDTO(
                wp.Price.Id,
                wp.Price.AveragePrice,
                wp.Price.Category
                )).ToList(),
                item.WorkspaceImages.Select(wi => new WorkspacesImageByOwnerNameSearchDTO(
                    wi.Image.Id,
                    wi.Image.ImgUrl
                )).ToList(),
                item.WorkspaceFacilities.Select(wf => new WorkspaceFacilityByOwnerNameSearchDTO(
                    wf.Facility.Id,
                    wf.Facility.Name
                )).ToList(),
                item.WorkspacePolicies.Select(wp => new WorkspacePolicyByOwnerNameSearchDTO(
                    wp.Policy.Id,
                    wp.Policy.Name
                )).ToList()));
        }
        return new SearchWorkspaceByOwnerNameResult(result);
    }
}

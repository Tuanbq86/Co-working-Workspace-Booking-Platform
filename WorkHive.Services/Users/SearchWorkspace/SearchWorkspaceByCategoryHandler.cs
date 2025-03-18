using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Users.SearchWorkspace;

public record SearchWorkspaceByCategoryQuery(string Category)
    : IQuery<SearchWorkspaceByCategoryResult>;
public record SearchWorkspaceByCategoryResult(List<WorkspaceSearchByCategoryDTO> WorkspaceSearchByCategoryDTOs);

public record WorkspaceSearchByCategoryDTO(int Id, string Name, string Address, string GoogleMapUrl, string Description, int? Capacity,
    string Category, string Status, DateTime CreatedAt, DateTime UpdatedAt, int? CleanTime,
    int? Area, TimeOnly? OpenTime, TimeOnly? CloseTime, int? Is24h, List<WorkspacesPriceByCategorySearchDTO> Prices,
List<WorkspacesImageByCategorySearchDTO> Images, List<WorkspaceFacilityByCategorySearchDTO> Facilities, List<WorkspacePolicyByCategorySearchDTO> Policies);

public record WorkspacesPriceByCategorySearchDTO(int Id, decimal? Price, string Category);
public record WorkspacesImageByCategorySearchDTO(int Id, string ImgUrl);
public record WorkspaceFacilityByCategorySearchDTO(int Id, string FacilityName);
public record WorkspacePolicyByCategorySearchDTO(int Id, string PolicyName);

public class SearchWorkspaceByCategoryHandler(IBookingWorkspaceUnitOfWork bookingUnit)
    : IQueryHandler<SearchWorkspaceByCategoryQuery, SearchWorkspaceByCategoryResult>
{
    public async Task<SearchWorkspaceByCategoryResult> Handle(SearchWorkspaceByCategoryQuery query, 
        CancellationToken cancellationToken)
    {
        var workspaces = bookingUnit.workspace.GetWorkspaceByCategorySearch(query.Category);

        if (!string.IsNullOrEmpty(query.Category))
        {
            workspaces = workspaces
                .Where(w => EF.Functions.Collate(w.Category.ToLower().Trim(), 
                "SQL_Latin1_General_CP1_CI_AS") 
                .Equals(query.Category.ToLower().Trim()));

        }

        List<WorkspaceSearchByCategoryDTO> result = new List<WorkspaceSearchByCategoryDTO>();

        foreach(var item in workspaces)
        {
            result.Add(new WorkspaceSearchByCategoryDTO(
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
                item.WorkspacePrices.Select(wp => new WorkspacesPriceByCategorySearchDTO(
                wp.Price.Id,
                wp.Price.AveragePrice,
                wp.Price.Category
                )).ToList(),
                item.WorkspaceImages.Select(wi => new WorkspacesImageByCategorySearchDTO(
                    wi.Image.Id,
                    wi.Image.ImgUrl
                )).ToList(),
                item.WorkspaceFacilities.Select(wf => new WorkspaceFacilityByCategorySearchDTO(
                    wf.Facility.Id,
                    wf.Facility.Name
                )).ToList(),
                item.WorkspacePolicies.Select(wp => new WorkspacePolicyByCategorySearchDTO(
                    wp.Policy.Id,
                    wp.Policy.Name
                )).ToList()));
        }
        return new SearchWorkspaceByCategoryResult(result);
    }
}

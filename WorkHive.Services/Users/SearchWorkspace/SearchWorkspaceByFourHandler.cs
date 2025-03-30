using Microsoft.EntityFrameworkCore;
using System.Linq;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Users.SearchWorkspace;

public record SearchWorkspaceByFourQuery(string? Address, string? Category, 
    int? Is24h, int? Capacity) 
    : IQuery<SearchWorkspaceByFourResult>;
public record SearchWorkspaceByFourResult(List<WorkspaceSearch4DTO> Workspaces);
public record WorkspaceSearch4DTO(int Id, string Name, string Address, string GoogleMapUrl, string Description, int? Capacity, 
    string Category, string Status, DateTime CreatedAt, DateTime UpdatedAt, int? CleanTime,
    int? Area, TimeOnly? OpenTime, TimeOnly? CloseTime, int? Is24h, List<WorkspacesPriceFourSearchDTO> Prices,
List<WorkspacesImageFourSearchDTO> Images, List<WorkspaceFacilityFourSearchDTO> Facilities, List<WorkspacePolicyFourSearchDTO> Policies);
public record WorkspacesPriceFourSearchDTO(int Id, decimal? Price, string Category);
public record WorkspacesImageFourSearchDTO(int Id, string ImgUrl);
public record WorkspaceFacilityFourSearchDTO(int Id, string FacilityName);
public record WorkspacePolicyFourSearchDTO(int Id, string PolicyName);

public class SearchWorkspaceByFourHandler(IBookingWorkspaceUnitOfWork bookUnit)
    : IQueryHandler<SearchWorkspaceByFourQuery, SearchWorkspaceByFourResult>
{
    public async Task<SearchWorkspaceByFourResult> Handle(SearchWorkspaceByFourQuery query, 
        CancellationToken cancellationToken)
    {
        var workspaces = bookUnit.workspace.GetWorkspaceForSearch();

        if (!string.IsNullOrEmpty(query.Address))
        {
            workspaces = workspaces
                .Where(w => EF.Functions.Like(w.Owner.LicenseAddress, $"%{query.Address}%"));
        }

        if (!string.IsNullOrEmpty(query.Category))
        {
            workspaces = workspaces
                .Where(w => EF.Functions.Like(w.Category, $"%{query.Category}%"));
        }

        if (query.Is24h.HasValue && (query.Is24h == 1 || query.Is24h == 0))
        {
            workspaces = workspaces.Where(w => w.Is24h.Value.Equals(query.Is24h.Value));
        }

        if (query.Capacity.HasValue)
        {
            workspaces = workspaces.Where(w => w.Capacity == query.Capacity.Value);
        }

        var result = await workspaces.Select(w => new WorkspaceSearch4DTO(
            w.Id,
            w.Name,
            w.Owner.LicenseAddress,
            w.Owner.GoogleMapUrl,
            w.Description,
            w.Capacity,
            w.Category,
            w.Status,
            w.CreatedAt.GetValueOrDefault(DateTime.MinValue),
            w.UpdatedAt.GetValueOrDefault(DateTime.MinValue),
            w.CleanTime ?? 0,
            w.Area ?? 0,
            w.OpenTime,
            w.CloseTime,
            w.Is24h ?? 0,
            w.WorkspacePrices.Select(wp => new WorkspacesPriceFourSearchDTO(
                wp.Price.Id,
                wp.Price.AveragePrice,
                wp.Price.Category
                )).ToList(),
            w.WorkspaceImages.Select(wi => new WorkspacesImageFourSearchDTO(
                wi.Image.Id,
                wi.Image.ImgUrl
                )).ToList(),
            w.WorkspaceFacilities.Select(wf => new WorkspaceFacilityFourSearchDTO(
                wf.Facility.Id,
                wf.Facility.Name
                )).ToList(),
            w.WorkspacePolicies.Select(wp => new WorkspacePolicyFourSearchDTO(
                wp.Policy.Id,
                wp.Policy.Name
                )).ToList()
            )).ToListAsync();

        return new SearchWorkspaceByFourResult(result);

    }
}
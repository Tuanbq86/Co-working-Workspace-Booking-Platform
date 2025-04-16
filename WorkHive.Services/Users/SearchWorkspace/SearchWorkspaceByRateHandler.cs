using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Users.SearchWorkspace;

public record SearchWorkspaceByRateQuery(): IQuery<SearchWorkspaceByRateResult>;
public record SearchWorkspaceByRateResult(List<WorkspaceSearchByRateDTO> Workspaces);

public record WorkspaceSearchByRateDTO(int Id, string Name, string Address, string GoogleMapUrl, string Description, int? Capacity,
    string Category, string Status, DateTime CreatedAt, DateTime UpdatedAt, int? CleanTime, double Rate,
    int? Area, TimeOnly? OpenTime, TimeOnly? CloseTime, int? Is24h, List<WorkspacesPriceByRateDTO> Prices,
List<WorkspacesImageByRateDTO> Images, List<WorkspaceFacilityByRateDTO> Facilities, List<WorkspacePolicyByRateDTO> Policies);
public record WorkspacesPriceByRateDTO(int Id, decimal? Price, string Category);
public record WorkspacesImageByRateDTO(int Id, string ImgUrl);
public record WorkspaceFacilityByRateDTO(int Id, string FacilityName);
public record WorkspacePolicyByRateDTO(int Id, string PolicyName);

public class SearchWorkspaceByRateHandler(IBookingWorkspaceUnitOfWork bookingUnit)
    : IQueryHandler<SearchWorkspaceByRateQuery, SearchWorkspaceByRateResult>
{
    public async Task<SearchWorkspaceByRateResult> Handle(SearchWorkspaceByRateQuery query, 
        CancellationToken cancellationToken)
    {
        var workspaces = await bookingUnit.workspace.GetWorkspaceByRateSearch();
        List<WorkspaceSearchByRateDTO> result = new List<WorkspaceSearchByRateDTO>();

        foreach(var workspace in workspaces)
        {
            int rate = 0;
            int count = 0;
            double star = default;

            foreach(var rateItem in workspace.WorkspaceRatings)
            {
                rate += (int)rateItem.Rating.Rate!;
                count += 1;
            }

            star = (double)rate / count;
            star = Math.Round(star, 1);
            if (star >= 4 && star <= 5)
            {
                result.Add(new WorkspaceSearchByRateDTO(workspace.Id, workspace.Name, workspace.Owner.LicenseAddress, workspace.Owner.GoogleMapUrl,
                    workspace.Description, workspace.Capacity, workspace.Category, workspace.Status, workspace.CreatedAt.GetValueOrDefault(DateTime.MinValue), 
                    workspace.UpdatedAt.GetValueOrDefault(DateTime.MinValue), workspace.CleanTime,
                    star, workspace.Area, workspace.OpenTime, workspace.CloseTime, workspace.Is24h, 
                    workspace.WorkspacePrices.Select(wp => new WorkspacesPriceByRateDTO(
                        wp.Price.Id,
                        wp.Price.AveragePrice,
                        wp.Price.Category)).ToList(),
                    workspace.WorkspaceImages.Select(wi => new WorkspacesImageByRateDTO(
                        wi.Image.Id,
                        wi.Image.ImgUrl)).ToList(),
                    workspace.WorkspaceFacilities.Select(wf => new WorkspaceFacilityByRateDTO(
                        wf.Facility.Id,
                        wf.Facility.Name)).ToList(),
                    workspace.WorkspacePolicies.Select(wp => new WorkspacePolicyByRateDTO(
                        wp.Policy.Id,
                        wp.Policy.Name)).ToList()));
            }
        }

        //Lấy 10 phần tử đầu tiên và sắp xếp dựa vào alphabet của tên workspace
        var sortedTop10 = result
        .OrderBy(w => w.Name)
        .Take(10)
        .ToList();

        return new SearchWorkspaceByRateResult(sortedTop10);
    }
}

using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Owners.ManageWorkSpace.GetAllById;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Workspace
{
    public record GetNearbyWorkspacesQuery(double Lat, double Lng, double RadiusKm = 5) : IQuery<List<WorkspaceNearbyDT>>;

    //public record WorkspaceNearbyDT(int Id, string Name, string Address, double Latitude, double Longitude, double DistanceKm);

    public record WorkspaceNearbyDT(
        int Id,
        string Name,
        string Address,
        string Description,
        int? Capacity,
        string Category,
        string Status,
        DateTime? CreatedAt,
        int? Area,
        int OwnerId,
        string LicenseName,
        string Phone,
        List<WorkspacesPriceDTO> Prices,
        List<WorkspacesImageDTO> Images,
        //List<WorkspaceFacilityDTO> Facilities,
        //List<WorkspacePolicyDTO> Policies,
        //List<WorkspaceDetailDTO> Details,
        double DistanceKm
    );

    public class GetNearbyWorkspacesHandler(IWorkSpaceManageUnitOfWork unit)
        : IQueryHandler<GetNearbyWorkspacesQuery, List<WorkspaceNearbyDT>>
    {
        public async Task<List<WorkspaceNearbyDT>> Handle(GetNearbyWorkspacesQuery query, CancellationToken cancellationToken)
        {
            var allWorkspaces = await unit.Workspace.GetAllWorkSpaceAsync();
            var ownerIds = allWorkspaces.Select(ws => ws.OwnerId).Distinct().ToList();
            var owners = await unit.WorkspaceOwner.GetOwnersByIdsAsync(ownerIds);

            return allWorkspaces
                .Where(w => w.Owner?.Latitude != null && w.Owner?.Longitude != null)
                .Select(w =>
                {
                    var distance = Haversine(query.Lat, query.Lng, w.Owner.Latitude.Value, w.Owner.Longitude.Value);

                    var owner = owners.FirstOrDefault(o => o.Id == w.OwnerId);

                    return new WorkspaceNearbyDT(
                        w.Id,
                        w.Name,
                        owner?.LicenseAddress ?? string.Empty,
                        w.Description,
                        w.Capacity,
                        w.Category,
                        w.Status,
                        w.CreatedAt,
                        w.Area,
                        w.OwnerId,
                        owner?.LicenseName ?? string.Empty,
                        owner?.Phone ?? string.Empty,
                        w.WorkspacePrices.Select(wp => new WorkspacesPriceDTO(
                            wp.Price.Id,
                            wp.Price.AveragePrice,
                            wp.Price.Category
                        )).ToList(),
                        w.WorkspaceImages.Select(wi => new WorkspacesImageDTO(
                            wi.Image.Id,
                            wi.Image.ImgUrl
                        )).ToList(),
                        //w.WorkspaceFacilities.Select(wf => new WorkspaceFacilityDTO(
                        //    wf.Facility.Id,
                        //    wf.Facility.Name
                        //)).ToList(),
                        //w.WorkspacePolicies.Select(wp => new WorkspacePolicyDTO(
                        //    wp.Policy.Id,
                        //    wp.Policy.Name
                        //)).ToList(),
                        //w.WorkspaceDetails.Select(wd => new WorkspaceDetailDTO(
                        //    wd.Detail.Id,
                        //    wd.Detail.Name
                        //)).ToList(),
                        distance
                    );
                })
                .Where(w => w.DistanceKm <= query.RadiusKm)
                .OrderBy(w => w.DistanceKm)
                .ToList();
        }

        private static double Haversine(double lat1, double lng1, double lat2, double lng2)
        {
            var R = 6371; // radius of Earth in km
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lng2 - lng1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}

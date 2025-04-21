using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Workspace
{
    public record GetNearbyWorkspacesQuery(double Lat, double Lng, double RadiusKm = 5) : IQuery<List<WorkspaceNearbyDT>>;

    public record WorkspaceNearbyDT(int Id, string Name, string Address, double Latitude, double Longitude, double DistanceKm);

    public class GetNearbyWorkspacesHandler(IWorkSpaceManageUnitOfWork unit)
        : IQueryHandler<GetNearbyWorkspacesQuery, List<WorkspaceNearbyDT>>
    {
        public async Task<List<WorkspaceNearbyDT>> Handle(GetNearbyWorkspacesQuery query, CancellationToken cancellationToken)
        {
            var allWorkspaces = await unit.Workspace.GetAllWorkSpaceAsync(); 

            return allWorkspaces
                .Where(w => w.Owner?.Latitude != null && w.Owner?.Longitude != null)
                .Select(w =>
                {
                    var distance = Haversine(query.Lat, query.Lng, w.Owner.Latitude.Value, w.Owner.Longitude.Value);
                    return new WorkspaceNearbyDT(
                        w.Id,
                        w.Name,
                        w.Owner.LicenseAddress,
                        w.Owner.Latitude.Value,
                        w.Owner.Longitude.Value,
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

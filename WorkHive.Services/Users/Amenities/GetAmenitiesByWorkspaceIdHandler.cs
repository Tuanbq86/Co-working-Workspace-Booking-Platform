using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Users.Amenities;

public record GetAmenitiesByWorkspaceIdQuery(int Id) : IQuery<GetAmenitiesByWorkspaceIdResult>;
public record GetAmenitiesByWorkspaceIdResult(List<Amenity> Amenities);

public class GetAmenitiesByWorkspaceIdHandler(IBookingWorkspaceUnitOfWork bookingUnit)
    : IQueryHandler<GetAmenitiesByWorkspaceIdQuery, GetAmenitiesByWorkspaceIdResult>
{
    public async Task<GetAmenitiesByWorkspaceIdResult> Handle(GetAmenitiesByWorkspaceIdQuery query, 
        CancellationToken cancellationToken)
    {
        var amenities = bookingUnit.amenity.GetAll()
            .Where(x => x.WorkspaceId == query.Id).ToList();

        return new GetAmenitiesByWorkspaceIdResult(amenities);
    }
}

using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Users.Beverages;

public record GetBeveragesByWorkspaceIdQuery(int Id) : IQuery<GetBeveragesByWorkspaceIdResult>;
public record GetBeveragesByWorkspaceIdResult(List<Beverage> Beverages);

public class GetBeveragesByWorkspaceIdHandler(IBookingWorkspaceUnitOfWork bookingUnit)
    : IQueryHandler<GetBeveragesByWorkspaceIdQuery, GetBeveragesByWorkspaceIdResult>
{
    public async Task<GetBeveragesByWorkspaceIdResult> Handle(GetBeveragesByWorkspaceIdQuery query, 
        CancellationToken cancellationToken)
    {
        var beverages = bookingUnit.beverage.GetAll()
            .Where(x => x.WorkspaceId == query.Id).ToList();

        return new GetBeveragesByWorkspaceIdResult(beverages);
    }
}

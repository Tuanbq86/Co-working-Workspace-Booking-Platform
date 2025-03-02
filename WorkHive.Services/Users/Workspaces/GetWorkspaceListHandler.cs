using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Users.Workspaces;

public record GetWorkspaceListQuery() : IQuery<GetWorkspaceListResult>;
public record GetWorkspaceListResult(List<Workspace> Workspaces);

public class GetWorkspaceListHandler(IBookingWorkspaceUnitOfWork bookingUnit)
    : IQueryHandler<GetWorkspaceListQuery, GetWorkspaceListResult>
{
    public async Task<GetWorkspaceListResult> Handle(GetWorkspaceListQuery query, 
        CancellationToken cancellationToken)
    {
        var workspaces = await bookingUnit.workspace.GetAllAsync();
        return new GetWorkspaceListResult(workspaces);
    }
}

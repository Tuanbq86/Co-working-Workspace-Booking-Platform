using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Exceptions;

namespace WorkHive.Services.Users.Workspaces;

public record GetWorkspaceByIdQuery(int Id): IQuery<GetWorkspaceByIdResult>;
public record GetWorkspaceByIdResult(Workspace Workspace);

public class GetWorkspaceByIdHandler(IBookingWorkspaceUnitOfWork bookingUnit)
    : IQueryHandler<GetWorkspaceByIdQuery, GetWorkspaceByIdResult>
{
    public async Task<GetWorkspaceByIdResult> Handle(GetWorkspaceByIdQuery query, 
        CancellationToken cancellationToken)
    {
        var workspace = bookingUnit.workspace.GetAll().FirstOrDefault(x => x.Id == query.Id);

        if (workspace is null)
            throw new WorkspaceNotFoundException("Workspace", query.Id);

        return new GetWorkspaceByIdResult(workspace);
    }
}

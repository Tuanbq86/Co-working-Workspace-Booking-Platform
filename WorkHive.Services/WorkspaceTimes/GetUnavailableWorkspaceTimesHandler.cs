using FluentValidation;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant;
using WorkHive.Services.Exceptions;

namespace WorkHive.Services.WorkspaceTimes;

public record WorkspaceTimesQuery(int WorkspaceId) : IQuery<WorkspaceTimesResult>;
public record WorkspaceTimesResult(List<WorkspaceTime> WorkspaceTimes);

public class WorkspaceTimesQueryValidator : AbstractValidator<WorkspaceTimesQuery>
{
    public WorkspaceTimesQueryValidator()
    {
        RuleFor(x => x.WorkspaceId).NotEmpty().WithMessage("WorkspaceId is required");
    }
}

public class GetUnavailableWorkspaceTimesHandler(IBookingWorkspaceUnitOfWork bookUnit)
    : IQueryHandler<WorkspaceTimesQuery, WorkspaceTimesResult>
{
    public async Task<WorkspaceTimesResult> Handle(WorkspaceTimesQuery query, 
        CancellationToken cancellationToken)
    {
        var workspace = bookUnit.workspace.GetById(query.WorkspaceId);

        if (workspace is null)
            throw new WorkspaceNotFoundException("Workspace", query.WorkspaceId);

        var results = new List<WorkspaceTime>();
        var workspaceTimeList = bookUnit.workspaceTime.GetAll();

        //Get workspaceTime with workspaceId
        var timesWithWorkspaceId = workspaceTimeList
            .Where(item => item.WorkspaceId.Equals(query.WorkspaceId))
            .ToList();

        //Update EndDate plus clean time of workspace
        foreach (var item in timesWithWorkspaceId)
        {
            if(item.Status.Equals(WorkspaceTimeStatus.InUse) 
                || item.Status.Equals(WorkspaceTimeStatus.Handling))
            {
                results.Add(item);
            }
        }

        //Add clean time for End date
        foreach(var item in results)
        {
            item.EndDate = item.EndDate.GetValueOrDefault()
                .AddMinutes(workspace.CleanTime.GetValueOrDefault());
        }

        return new WorkspaceTimesResult(results);
    }
}

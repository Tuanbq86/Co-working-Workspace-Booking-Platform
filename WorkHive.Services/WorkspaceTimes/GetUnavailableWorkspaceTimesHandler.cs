using FluentValidation;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant;
using WorkHive.Services.Exceptions;
using WorkHive.Services.WorkspaceTimes.DTOs;

namespace WorkHive.Services.WorkspaceTimes;

public record WorkspaceTimesQuery(int WorkspaceId) : IQuery<WorkspaceTimesResult>;
public record WorkspaceTimesResult(List<WorkspaceTimeDto> WorkspaceTimes);

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

        var results = new List<WorkspaceTimeDto>();
        var tempWorkspace = await bookUnit.workspace.GetWorkspacesById(query.WorkspaceId);


        //Update EndDate plus clean time of workspace
        foreach (var item in tempWorkspace.WorkspaceTimes)
        {
            if(item.Status.Equals(WorkspaceTimeStatus.InUse.ToString()) ||
                item.Status.Equals(WorkspaceTimeStatus.Handling.ToString()))
            {
                results.Add(new WorkspaceTimeDto
                {
                    StartDate = (DateTime)item.StartDate!,
                    EndDate = (DateTime)item.EndDate!,
                    Status = item.Status
                });
            }
        }

        //Add clean time for End date
        foreach(var item in results)
        {
            item.EndDate = item.EndDate
                .AddMinutes(workspace.CleanTime.GetValueOrDefault());
        }

        return new WorkspaceTimesResult(results);
    }
}

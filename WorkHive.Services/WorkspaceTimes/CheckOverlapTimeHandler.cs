using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant;
using WorkHive.Services.Exceptions;

namespace WorkHive.Services.WorkspaceTimes;

public record CheckTimesCommand(int WorkspaceId, string StartDate, string EndDate) : ICommand<CheckTimesResult>;
public record CheckTimesResult(string Notification);

public class CheckOverlapTimeHandler(IBookingWorkspaceUnitOfWork bookUnit)
    : ICommandHandler<CheckTimesCommand, CheckTimesResult>
{
    public async Task<CheckTimesResult> Handle(CheckTimesCommand command, 
        CancellationToken cancellationToken)
    {
        var workspace = bookUnit.workspace.GetById(command.WorkspaceId);

        if (workspace is null)
            return new CheckTimesResult("Can not find Workspace");

        var timesOfWorkspaceId = bookUnit.workspaceTime.GetAll()
            .Where(x => x.WorkspaceId.Equals(command.WorkspaceId)).ToList();

        var TimesHandlingOrInuse = timesOfWorkspaceId
            .Where(x => x.Status.Equals(WorkspaceTimeStatus.InUse.ToString()) 
        || x.Status.Equals(WorkspaceTimeStatus.Handling.ToString())).ToList();

        foreach(var item in TimesHandlingOrInuse)
        {
            item.EndDate = item.EndDate.GetValueOrDefault()
                .AddMinutes(workspace.CleanTime.GetValueOrDefault());
        }

        if(bookUnit.workspaceTime.IsOverlap(TimesHandlingOrInuse, 
            DateTime.ParseExact(command.StartDate, "HH:mm dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
            DateTime.ParseExact(command.EndDate, "HH:mm dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)))
        {
            return new CheckTimesResult("Time interval has been used");
        }

        return new CheckTimesResult("Time interval is available");
    }
}

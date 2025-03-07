using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant;
using WorkHive.Services.Exceptions;

namespace WorkHive.Services.WorkspaceTimes;

public record UpdateTimeCommand(string Status, long OrderCode) : ICommand<UpdateTimeResult>;
public record UpdateTimeResult(string Notification);

public class UpdateWorkspaceTimeStatusHandler(IBookingWorkspaceUnitOfWork bookUnit)
    : ICommandHandler<UpdateTimeCommand, UpdateTimeResult>
{
    public async Task<UpdateTimeResult> Handle(UpdateTimeCommand command, CancellationToken cancellationToken)
    {
        var bookWorkspace = bookUnit.booking.GetById((int)command.OrderCode);

        if (!command.Status.Equals(UpdateTimeStatus.PAID.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            throw new BookingBadRequestException("Update failed");
        }

        var workspaceTime = bookUnit.workspaceTime.GetAll()
            .FirstOrDefault(x => x.BookingId.Equals(command.OrderCode));

        workspaceTime!.Status = WorkspaceTimeStatus.InUse.ToString();

        bookUnit.workspaceTime.Update(workspaceTime);
        await bookUnit.SaveAsync();

        return new UpdateTimeResult("Update successfully");
    }
}

using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant;
using WorkHive.Services.Exceptions;

namespace WorkHive.Services.WorkspaceTimes;

public record UpdateTimeCommand(string Status, int BookingId) : ICommand<UpdateTimeResult>;
public record UpdateTimeResult(string Notification);

public class UpdateWorkspaceTimeStatusHandler(IBookingWorkspaceUnitOfWork bookUnit)
    : ICommandHandler<UpdateTimeCommand, UpdateTimeResult>
{
    public async Task<UpdateTimeResult> Handle(UpdateTimeCommand command, CancellationToken cancellationToken)
    {
        var bookWorkspace = bookUnit.booking.GetById(command.BookingId);

        if (!command.Status.Equals(UpdateTimeStatus.PAID.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            throw new BookingBadRequestException("Update failed");
        }

        var workspaceTime = bookUnit.workspaceTime.GetAll()
            .FirstOrDefault(x => x.BookingId.Equals(command.BookingId));

        var booking = bookUnit.booking.GetById(command.BookingId);

        booking.Status = BookingStatus.Success.ToString();
        workspaceTime!.Status = WorkspaceTimeStatus.InUse.ToString();

        bookUnit.booking.Update(booking);
        bookUnit.workspaceTime.Update(workspaceTime);

        await bookUnit.SaveAsync();

        return new UpdateTimeResult("Update successfully");
    }
}

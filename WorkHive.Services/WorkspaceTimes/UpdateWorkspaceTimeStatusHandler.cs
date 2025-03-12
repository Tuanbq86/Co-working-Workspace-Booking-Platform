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
    public async Task<UpdateTimeResult> Handle(UpdateTimeCommand command, 
        CancellationToken cancellationToken)
    {
        var bookWorkspace = bookUnit.booking.GetById(command.BookingId);

        //Pay successfully

        if (command.Status.Equals(UpdateTimeStatus.PAID.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            var workspaceTime = bookUnit.workspaceTime.GetAll()
            .FirstOrDefault(x => x.BookingId.Equals(command.BookingId));
            if(workspaceTime is null)
            {
                return new UpdateTimeResult("Yêu cầu không hợp lệ");
            }


            var booking = bookUnit.booking.GetById(command.BookingId);
            if(booking is null)
            {
                return new UpdateTimeResult("Yêu cầu không hợp lệ");
            }
                

            booking.Status = BookingStatus.Success.ToString();
            workspaceTime!.Status = WorkspaceTimeStatus.InUse.ToString();

            bookUnit.booking.Update(booking);
            bookUnit.workspaceTime.Update(workspaceTime);

            await bookUnit.SaveAsync();
        }

        //Pay failed or expried

        if(command.Status.Equals(UpdateTimeStatus.FAILED.ToString())
            || command.Status.Equals(UpdateTimeStatus.EXPIRED.ToString()))
        {
            var workspaceTime = bookUnit.workspaceTime.GetAll()
                .FirstOrDefault(x => x.BookingId.Equals(command.BookingId));
            if (workspaceTime is null)
            {
                return new UpdateTimeResult("Yêu cầu không hợp lệ");
            }

            bookUnit.workspaceTime.Remove(workspaceTime!);

            var booking = bookUnit.booking.GetById(command.BookingId);
            if (booking is null)
            {
                return new UpdateTimeResult("Yêu cầu không hợp lệ");
            }

            booking.Status = BookingStatus.Fail.ToString();

            bookUnit.booking.Update(booking);

            await bookUnit.SaveAsync();
        }
        

        return new UpdateTimeResult("Cập nhật trạng thái thành công");
    }
}

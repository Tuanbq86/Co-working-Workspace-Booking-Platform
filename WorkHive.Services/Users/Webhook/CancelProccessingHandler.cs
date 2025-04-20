using Microsoft.Extensions.Configuration;
using Net.payOS;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant;

namespace WorkHive.Services.Users.Webhook;

public record CancelProccessingCommand(long OrderCode, string? CancelledReason)
    : ICommand<CancelProccessingResult>;
public record CancelProccessingResult(string Notification);


public class CancelProccessingHandler(IBookingWorkspaceUnitOfWork bookUnit, IConfiguration configuration)
    : ICommandHandler<CancelProccessingCommand, CancelProccessingResult>
{
    private readonly string ClientID = configuration["PayOS:ClientId"]!;
    private readonly string ApiKey = configuration["PayOS:ApiKey"]!;
    private readonly string CheckSumKey = configuration["PayOS:CheckSumKey"]!;
    public async Task<CancelProccessingResult> Handle(CancelProccessingCommand command, 
        CancellationToken cancellationToken)
    {
        PayOS payOS = new PayOS(ClientID, ApiKey, CheckSumKey);

        try
        {
            var orderCodeString = command.OrderCode.ToString();
            // 1 cho booking, 2 cho deposit
            var typeCode = orderCodeString.Substring(0, 1);
            var timestampPart = orderCodeString.Substring(1, 6);
            //Lấy booking Id
            var bookingIdStr = orderCodeString.Substring(7);
            var bookingId = int.Parse(bookingIdStr);

            if(typeCode == "1")
            {
                if (string.IsNullOrEmpty(command.CancelledReason))
                {
                    await payOS.cancelPaymentLink(command.OrderCode);
                }
                else
                {
                    await payOS.cancelPaymentLink(command.OrderCode, command.CancelledReason);
                }

                var bookWorkspace = bookUnit.booking.GetById(bookingId);
                if (bookWorkspace is null)
                {
                    return new CancelProccessingResult("Không tìm thấy booking");
                }
                //Thực hiện xóa thời gian tương ứng với booking và chuyển trạng thái thanh toán thành failed
                var workspaceTime = bookUnit.workspaceTime.GetAll()
                        .FirstOrDefault(x => x.BookingId.Equals(bookWorkspace.Id));
                if (workspaceTime is null)
                {
                    return new CancelProccessingResult("Yêu cầu không hợp lệ");
                }

                bookUnit.workspaceTime.Remove(workspaceTime);

                bookWorkspace.Status = BookingStatus.Fail.ToString();

                bookUnit.booking.Update(bookWorkspace);

                await bookUnit.SaveAsync();
            }
        }
        catch (Exception ex)
        {
            // Handle exception
            throw new Exception("Error processing webhook data", ex);
        }

        return new CancelProccessingResult("Xử lý hủy thành công");
    }
}

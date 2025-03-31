using Microsoft.Extensions.Configuration;
using Net.payOS.Types;
using Net.payOS;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant;
using WorkHive.Services.Exceptions;
using WorkHive.Data.Models;
using WorkHive.Services.Common;

namespace WorkHive.Services.WorkspaceTimes;

public record UpdateTimeCommand(long OrderCode, int BookingId) : ICommand<UpdateTimeResult>;
public record UpdateTimeResult(string Notification);

public class UpdateWorkspaceTimeStatusHandler(IUserUnitOfWork userUnit, IBookingWorkspaceUnitOfWork bookUnit, IConfiguration configuration)
    : ICommandHandler<UpdateTimeCommand, UpdateTimeResult>
{
    private readonly string ClientID = configuration["PayOS:ClientId"]!;
    private readonly string ApiKey = configuration["PayOS:ApiKey"]!;
    private readonly string CheckSumKey = configuration["PayOS:CheckSumKey"]!;
    public async Task<UpdateTimeResult> Handle(UpdateTimeCommand command, 
        CancellationToken cancellationToken)
    {
        var bookWorkspace = bookUnit.booking.GetById(command.BookingId);

        PayOS payOS = new PayOS(ClientID, ApiKey, CheckSumKey);

        PaymentLinkInformation paymentLinkInformation = await payOS.getPaymentLinkInformation(command.OrderCode);

        var Status = paymentLinkInformation.status.ToString();

        //Pay Failed

        if (!(Status.Equals(PayOSStatus.PAID.ToString())))
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


        //Pay successfully
        if (Status.Equals(PayOSStatus.PAID.ToString()))
        {
            var workspaceTime = bookUnit.workspaceTime.GetAll()
                .FirstOrDefault(x => x.BookingId.Equals(command.BookingId));
            if (workspaceTime is null)
            {
                return new UpdateTimeResult("Yêu cầu không hợp lệ");
            }

            var booking = bookUnit.booking.GetById(command.BookingId);
            if (booking is null)
            {
                return new UpdateTimeResult("Yêu cầu không hợp lệ");
            }


            booking.Status = BookingStatus.Success.ToString();
            workspaceTime!.Status = WorkspaceTimeStatus.InUse.ToString();

            bookUnit.booking.Update(booking);
            bookUnit.workspaceTime.Update(workspaceTime);

            await bookUnit.SaveAsync();

            //Cộng tiền 90 cho ví owner và ghi lại lịch sử giao dịch cho bên owner
            var workspace = bookUnit.workspace.GetById(booking.WorkspaceId);
            var owner = userUnit.Owner.GetById(workspace.OwnerId);

            var ownerWallet = await bookUnit.ownerWallet.GetOwnerWalletByOwnerIdForBooking(owner.Id);
            var walletOfOwner = userUnit.Wallet.GetById(ownerWallet.WalletId);
            walletOfOwner.Balance += (booking.Price * 90) / 100;

            await bookUnit.wallet.UpdateAsync(walletOfOwner);

            //create Transaction of user
            var transactionHistoryOfUser = new TransactionHistory
            {
                Amount = bookWorkspace.Price,
                Status = "PAID",
                Description = $"Nội dung:\r\nThanh toán của bạn cho {workspace.Name} đã được xử lý thành công.\r\nSố tiền: {bookWorkspace.Price.ToVnd()}\r\nPhương thức thanh toán: PAYOS\r\nCảm ơn bạn đã sử dụng dịch vụ của chúng tôi!",
                CreatedAt = DateTime.Now,
                Title = "Thanh toán thành công"
            };
            await userUnit.TransactionHistory.CreateAsync(transactionHistoryOfUser);

            //Create Transaction History for owner
            var transactionHistoryOfOwner = new TransactionHistory
            {
                Amount = (booking.Price * 90) / 100,
                Status = "PAID",
                Description = $"Nội dung:\r\nNhận {((booking.Price * 90) / 100).ToVnd()} đơn booking: {booking.Id}",
                CreatedAt = DateTime.Now,
                Title = "Đặt chỗ"
            };
            await userUnit.TransactionHistory.CreateAsync(transactionHistoryOfOwner);

            var ownerTransactionHistory = new OwnerTransactionHistory
            {
                Status = "PAID",
                TransactionHistoryId = transactionHistoryOfOwner.Id,
                OwnerWalletId = ownerWallet.Id
            };
            await userUnit.OwnerTransactionHistory.CreateAsync(ownerTransactionHistory);

            //Tạo thông báo
            var userNotifi = new UserNotification
            {
                UserId = bookWorkspace.UserId,
                IsRead = 0,
                CreatedAt = DateTime.Now,
                Description = $"Nội dung:\r\nBạn đã đặt chỗ thành công cho {workspace.Name} từ {workspaceTime.StartDate} đến {workspaceTime.EndDate}.\r\nVui lòng kiểm tra lại thông tin trong mục Lịch sử đặt chỗ. Chúng tôi mong được phục vụ bạn!",
                Status = "PAID",
                Title = "Đặt chỗ thành công"
            };
            await userUnit.UserNotification.CreateAsync(userNotifi);

            var workspacefornoti = bookUnit.workspace.GetById(bookWorkspace.WorkspaceId);
            var ownerfornoti = bookUnit.Owner.GetAll().FirstOrDefault(o => o.Id.Equals(workspacefornoti.OwnerId));
            var ownerNotifi = new OwnerNotification
            {
                OwnerId = ownerfornoti!.Id,
                CreatedAt = DateTime.Now,
                Description = $"Nội dung:\r\nWorkspace: {bookWorkspace.WorkspaceId} đã được đặt",
                IsRead = 0,
                Status = "PAID",
                Title = "Đặt chỗ"
            };
            await bookUnit.ownerNotification.CreateAsync(ownerNotifi);

        }

        return new UpdateTimeResult("Cập nhật trạng thái thành công");
    }
}

using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Common;
using WorkHive.Services.Constant;

namespace WorkHive.Services.Users.BookingWorkspace.CancelBooking;

public record CancelBookingCommand(int BookingId)
    : ICommand<CancelBookingResult>;
public record CancelBookingResult(string Notification);

public class CancelBookingHandler(IBookingWorkspaceUnitOfWork bookUnit, IUserUnitOfWork userUnit)
    : ICommandHandler<CancelBookingCommand, CancelBookingResult>
{
    public async Task<CancelBookingResult> Handle(CancelBookingCommand command,
        CancellationToken cancellationToken)
    {
        var placeBooking = bookUnit.booking.GetById(command.BookingId);

        //Valid booking không null và trạng thái của booking đó phải thành công
        if (placeBooking is null || placeBooking.Status != BookingStatus.Success.ToString())
        {
            return new CancelBookingResult("Không tìm thấy booking hợp lệ để hủy hoặc trạng thái chưa thành công để hủy");
        }

        //Thời điểm hủy phải nằm trong khoảng thời gian 8 tiếng trước startDate của booking đó
        var now = DateTime.Now;
        if (placeBooking.StartDate!.Value.AddHours(-8) < now)
        {
            return new CancelBookingResult($"Đã quá thời hạn 8 tiếng để hủy booking tính từ startDate của đơn booking: {placeBooking.Id}");
        }
        else
        {
            //Chuyển trạng thái của booking sang hủy
            placeBooking.Status = BookingStatus.Cancelled.ToString();
            await bookUnit.booking.UpdateAsync(placeBooking);

            //Tìm và xóa khoảng thời gian của booking đó
            var workspaceTimeOfPlaceBooking = bookUnit.workspaceTime.GetAll()
                .FirstOrDefault(ws => ws.BookingId.Equals(placeBooking.Id));
            await bookUnit.workspaceTime.RemoveAsync(workspaceTimeOfPlaceBooking!);

            //Tiến hành hoàn tiền 80% giá trị đơn booking vào ví user

            //Cho user
            var customerWallet = bookUnit.customerWallet.GetAll().FirstOrDefault(cw => cw.UserId.Equals(placeBooking.UserId));
            var walletOfUser = bookUnit.wallet.GetById(customerWallet!.WalletId);
            walletOfUser.Balance += (placeBooking.Price * 80) / 100;
            await bookUnit.wallet.UpdateAsync(walletOfUser);

            var transactionHistoryOfUser = new TransactionHistory
            {
                Amount = (placeBooking.Price * 80) / 100,
                Description = $"Hoàn tiền cho đơn booking: {placeBooking.Id}",
                Status = "REFUND",
                CreatedAt = now,
                Title = "Hoàn tiền thành công",
                BeforeTransactionAmount = walletOfUser.Balance - (placeBooking.Price * 80) / 100,
                AfterTransactionAmount = walletOfUser.Balance
            };
            await bookUnit.transactionHistory.CreateAsync(transactionHistoryOfUser);

            var userTransactionHistory = new UserTransactionHistory
            {
                Status = "REFUND",
                TransactionHistoryId = transactionHistoryOfUser.Id,
                CustomerWalletId = customerWallet.Id,
            };
            await bookUnit.userTransactionHistory.CreateAsync(userTransactionHistory);

            //Tạo user notification
            var userNotification = new UserNotification
            {
                CreatedAt = now,
                Description = $"Nội dung:\r\nYêu cầu hoàn tiền của bạn đã được xử lý thành công.\r\nSố tiền hoàn lại: {((placeBooking.Price * 80) / 100).ToVnd()}\r\nVui lòng kiểm tra số dư trong ví hệ thống",
                IsRead = 0,
                Status = "REFUND",
                UserId = placeBooking.UserId,
                Title = "Hoàn tiền thành công"
            };
            await userUnit.UserNotification.CreateAsync(userNotification);

            //Cho owner
            var workspaceBooking = bookUnit.workspace.GetById(placeBooking.WorkspaceId);
            var owner = bookUnit.Owner.GetById(workspaceBooking.OwnerId);
            var ownerWallet = bookUnit.ownerWallet.GetAll().FirstOrDefault(ow => ow.OwnerId.Equals(owner.Id));
            var walletOfOwner = bookUnit.wallet.GetById(ownerWallet!.WalletId);
            //Trừ 90% giá trị đơn booking vào ví owner
            walletOfOwner.Balance -= (placeBooking.Price * 90) / 100;
            await bookUnit.wallet.UpdateAsync(walletOfOwner);
            //Cộng thêm 20% giá trị của đơn booking vào ví owner
            walletOfOwner.Balance += (placeBooking.Price * 20) / 100;
            await bookUnit.wallet.UpdateAsync(walletOfOwner);

            var transactionHistoryOfOwner = new TransactionHistory
            {
                Amount = (placeBooking.Price * 20) / 100,
                Description = $"Hoàn tiền đơn booking: {placeBooking.Id}",
                Status = "REFUND",
                CreatedAt = now,
                Title = "Hoàn tiền",
                BeforeTransactionAmount = walletOfOwner.Balance - ((placeBooking.Price * 20) / 100) + ((placeBooking.Price * 90) / 100),
                AfterTransactionAmount = walletOfOwner.Balance
            };
            await bookUnit.transactionHistory.CreateAsync(transactionHistoryOfOwner);

            var ownerTransactionHistory = new OwnerTransactionHistory
            {
                Status = "REFUND",
                TransactionHistoryId = transactionHistoryOfOwner.Id,
                OwnerWalletId = ownerWallet.Id,
            };
            await bookUnit.ownerTransactionHistory.CreateAsync(ownerTransactionHistory);

            //Tạo owner notification
            var ownerNotification = new OwnerNotification
            {
                CreatedAt = now,
                Description = $"Nội dung:\r\nCộng {((placeBooking.Price * 20) / 100).ToVnd()} hoàn tiền đơn booking {placeBooking.Id}",
                IsRead = 0,
                Status = "REFUND",
                OwnerId = owner.Id,
                Title = "Hoàn tiền"
            };
            await bookUnit.ownerNotification.CreateAsync(ownerNotification);
        }

        return new CancelBookingResult("Hủy booking thành công");
    }
}
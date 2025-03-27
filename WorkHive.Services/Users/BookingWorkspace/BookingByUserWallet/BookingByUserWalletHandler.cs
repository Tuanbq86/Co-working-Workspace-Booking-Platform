using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant;
using WorkHive.Services.Users.DTOs;

namespace WorkHive.Services.Users.BookingWorkspace.BookingByUserWallet;

public record BookingByUserWalletCommand(int UserId, int WorkspaceId, string StartDate, string EndDate,
    List<BookingAmenityDTO> Amenities, List<BookingBeverageDTO> Beverages, string PromotionCode, decimal Price, string WorkspaceTimeCategory) 
    : ICommand<BookingByUserWalletResult>;
public record BookingByUserWalletResult(string Notification);

public class BookingByUserWalletHandler(IBookingWorkspaceUnitOfWork bookingUnit, IUserUnitOfWork userUnit)
    : ICommandHandler<BookingByUserWalletCommand, BookingByUserWalletResult>
{
    public async Task<BookingByUserWalletResult> Handle(BookingByUserWalletCommand command, 
        CancellationToken cancellationToken)
    {
        //Lấy ví của customer để kiểm tra số dư có đủ để thanh toán booking hay không
        var customerWallet = await userUnit.CustomerWallet.GetCustomerWalletByUserId(command.UserId);

        //Kiểm tra mã giảm giá và áp dụng cho price để so sánh
        var bookingPrice = command.Price;

        if (!string.IsNullOrWhiteSpace(command.PromotionCode))
        {
            var promotion = bookingUnit.promotion.GetAll()
                                .FirstOrDefault(p => p.WorkspaceId == command.WorkspaceId
                          && p.Code.Trim().ToLower() == command.PromotionCode.Trim().ToLower());

            if (promotion != null)
            {
                bookingPrice -= (decimal)((bookingPrice * promotion.Discount) / 100)!;
            }
        }
        
        //So sánh số dư trong ví với số tiền sau khi đã áp dụng mã giảm giá
        if(customerWallet.Wallet.Balance < bookingPrice)
        {
            return new BookingByUserWalletResult("Số dư trong ví không đủ để thực hiện booking");
        }

        //Trừ tiền ví user và cộng 90% số tiền vào ví của owner có workspace tương ứng
        var wallet = userUnit.Wallet.GetById(customerWallet.WalletId);
        wallet.Balance -= bookingPrice;
        await userUnit.Wallet.UpdateAsync(wallet);

        var workspaceOfOwner = bookingUnit.workspace.GetById(command.WorkspaceId);
        var owner = userUnit.Owner.GetById(workspaceOfOwner.OwnerId);
        var ownerWallet = await bookingUnit.ownerWallet.GetOwnerWalletByOwnerIdForBooking(owner.Id);
        var walletOfOwner = userUnit.Wallet.GetById(ownerWallet.WalletId);
        walletOfOwner.Balance += (bookingPrice * 90) / 100;
        await bookingUnit.wallet.UpdateAsync(walletOfOwner);

        //Nếu thỏa mãn số tiền thì tiến hành đặt hàng
        var newBooking = new Booking();

        newBooking.UserId = command.UserId;//Add userId for booking
        newBooking.Price = command.Price; //
        newBooking.WorkspaceId = command.WorkspaceId; //
        newBooking.PaymentId = 2; // Payment method: WorkHive Wallet
        newBooking.CreatedAt = DateTime.Now;
        newBooking.Status = BookingStatus.Success.ToString(); //
        newBooking.IsReview = 0; //
        newBooking.Price = bookingPrice; //

        newBooking.StartDate = DateTime.ParseExact(command.StartDate, "HH:mm dd/MM/yyyy",
            System.Globalization.CultureInfo.InvariantCulture); //

        newBooking.EndDate = DateTime.ParseExact(command.EndDate, "HH:mm dd/MM/yyyy",
            System.Globalization.CultureInfo.InvariantCulture); //

        await bookingUnit.booking.CreateAsync(newBooking);

        if (!string.IsNullOrWhiteSpace(command.PromotionCode))
        {
            var promotion = bookingUnit.promotion.GetAll()
                                .FirstOrDefault(p => p.WorkspaceId == command.WorkspaceId
                          && p.Code.Trim().ToLower() == command.PromotionCode.Trim().ToLower());

            if (promotion != null)
            {
                newBooking.PromotionId = promotion.Id;
                await bookingUnit.booking.UpdateAsync(newBooking);
            }
        }

        //Add amenities and beverages for booking

        //Amenity
        if (command.Amenities is not null && command.Amenities.Any())
        {
            foreach (var item in command.Amenities)
            {
                var amenity = bookingUnit.amenity.GetById(item.Id);

                if (amenity is null || item.Quantity > amenity.Quantity)
                    continue;

                var newBookingAmenity = new BookingAmenity
                {
                    Quantity = item.Quantity,
                    BookingId = newBooking.Id,
                    AmenityId = amenity.Id
                };

                await bookingUnit.bookAmenity.CreateAsync(newBookingAmenity);

                amenity.Quantity -= newBookingAmenity.Quantity;

                await bookingUnit.amenity.UpdateAsync(amenity);
            }
        }

        //Beverage
        if (command.Beverages is not null && command.Beverages.Any())
        {
            foreach (var item in command.Beverages)
            {
                var beverage = bookingUnit.beverage.GetById(item.Id);

                if (beverage is null)
                    continue;

                var newBookingBeverage = new BookingBeverage
                {
                    Quantity = item.Quantity,
                    BookingWorkspaceId = newBooking.Id,
                    BeverageId = beverage.Id
                };

                await bookingUnit.bookBeverage.CreateAsync(newBookingBeverage);
            }
        }

        //-------------------------------------------------------------------

        var newWorkspaceTime = new WorkspaceTime
        {
            StartDate = newBooking.StartDate,
            EndDate = newBooking.EndDate,
            Status = WorkspaceTimeStatus.InUse.ToString(),
            WorkspaceId = newBooking.WorkspaceId,
            BookingId = newBooking.Id,
            Category = command.WorkspaceTimeCategory
        };

        await bookingUnit.workspaceTime.CreateAsync(newWorkspaceTime);

        //-------------------------------------------------------------------
        //Create Transaction History for user
        var transactionHistoryOfUser = new TransactionHistory
        {
            Amount = newBooking.Price,
            Status = "PAID",
            Description = $"Thanh toán đơn booking: {newBooking.Id}",
            CreatedAt = DateTime.Now
        };
        await userUnit.TransactionHistory.CreateAsync(transactionHistoryOfUser);

        var userTransactionHistory = new UserTransactionHistory
        {
            Status = "PAID",
            TransactionHistoryId = transactionHistoryOfUser.Id,
            CustomerWalletId = customerWallet.Id
        };
        await userUnit.UserTransactionHistory.CreateAsync(userTransactionHistory);

        //Create Transaction History for owner
        var transactionHistoryOfOwner = new TransactionHistory
        {
            Amount = (newBooking.Price * 90) / 100,
            Status = "PAID",
            Description = $"Nhận tiền đơn booking: {newBooking.Id}",
            CreatedAt = DateTime.Now
        };
        await userUnit.TransactionHistory.CreateAsync(transactionHistoryOfOwner);

        var ownerTransactionHistory = new OwnerTransactionHistory
        {
            Status = "PAID",
            TransactionHistoryId = transactionHistoryOfOwner.Id,
            OwnerWalletId = ownerWallet.Id
        };
        await userUnit.OwnerTransactionHistory.CreateAsync(ownerTransactionHistory);

        var userNotifi = new UserNotification
        {
            UserId = newBooking.UserId,
            IsRead = 0,
            CreatedAt = DateTime.Now,
            Description = $"Đặt chỗ thành công workspace: {newBooking.WorkspaceId}",
            Status = "Active"
        };
        await userUnit.UserNotification.CreateAsync(userNotifi);

        return new BookingByUserWalletResult("booking thành công");
    }
}
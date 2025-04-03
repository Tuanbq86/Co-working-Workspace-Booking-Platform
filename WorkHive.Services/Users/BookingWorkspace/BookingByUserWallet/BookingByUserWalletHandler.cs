using CloudinaryDotNet.Actions;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System.Text;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Common;
using WorkHive.Services.Constant;
using WorkHive.Services.EmailServices;
using WorkHive.Services.Users.DTOs;

namespace WorkHive.Services.Users.BookingWorkspace.BookingByUserWallet;

public record BookingByUserWalletCommand(int UserId, int WorkspaceId, string StartDate, string EndDate,
    List<BookingAmenityDTO> Amenities, List<BookingBeverageDTO> Beverages, string PromotionCode, decimal Price, string WorkspaceTimeCategory) 
    : ICommand<BookingByUserWalletResult>;
public record BookingByUserWalletResult(string Notification);

public class BookingByUserWalletHandler(IBookingWorkspaceUnitOfWork bookingUnit, IUserUnitOfWork userUnit, IEmailService emailService)
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
        newBooking.IsFeedback = 0; //

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
            Description = $"Nội dung:\r\nThanh toán của bạn cho {workspaceOfOwner.Name} đã được xử lý thành công.\r\nSố tiền: {newBooking.Price.ToVnd()}\r\nPhương thức thanh toán: WorkHive wallet\r\nCảm ơn bạn đã sử dụng dịch vụ của chúng tôi!",
            CreatedAt = DateTime.Now,
            Title = "Thanh toán thành công"
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
            Description = $"Nội dung:\r\nNhận {((newBooking.Price * 90) / 100).ToVnd()} đơn booking: {newBooking.Id}",
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

        var userNotifi = new UserNotification
        {
            UserId = newBooking.UserId,
            IsRead = 0,
            CreatedAt = DateTime.Now,
            Description = $"Nội dung:\r\nBạn đã đặt chỗ thành công cho {workspaceOfOwner.Name} từ {command.StartDate} đến {command.EndDate}.\r\nVui lòng kiểm tra lại thông tin trong mục Lịch sử đặt chỗ. Chúng tôi mong được phục vụ bạn!",
            Status = "PAID",
            Title = "Đặt chỗ thành công"
        };
        await userUnit.UserNotification.CreateAsync(userNotifi);

        var workspacefornoti = bookingUnit.workspace.GetById(newBooking.WorkspaceId);
        var ownerfornoti = bookingUnit.Owner.GetAll().FirstOrDefault(o => o.Id.Equals(workspacefornoti.OwnerId));
        var ownerNotifi = new OwnerNotification
        {
            OwnerId = ownerfornoti!.Id,
            CreatedAt = DateTime.Now,
            Description = $"Nội dung:\r\nWorkspace: {newBooking.WorkspaceId} đã được đặt",
            IsRead = 0,
            Status = "PAID",
            Title = "Đặt chỗ"
        };
        await bookingUnit.ownerNotification.CreateAsync(ownerNotifi);

        //Send email to user
        var user = userUnit.User.GetById(newBooking.UserId);

        var bookingOfEmail = new BookingHistory();

        //If null amenities and beverages will assign default list[]
        var amenities = bookingOfEmail.BookingHistoryAmenities ?? new List<BookingHistoryAmenity>();
        var beverages = bookingOfEmail.BookingHistoryBeverages ?? new List<BookingHistoryBeverage>();
        var workspaceImages = bookingOfEmail.BookingHistoryWorkspaceImages ?? new List<BookingHistoryWorkspaceImage>();

        bookingOfEmail.Booking_Id = newBooking.Id;
        bookingOfEmail.Workspace_Id = newBooking.WorkspaceId;
        bookingOfEmail.User_Name = user.Name;
        bookingOfEmail.Booking_StartDate = newBooking.StartDate;
        bookingOfEmail.Booking_EndDate = newBooking.EndDate;
        bookingOfEmail.Booking_Status = newBooking.Status;
        bookingOfEmail.Booking_CreatedAt = newBooking.CreatedAt;
        bookingOfEmail.Payment_Method = "Ví WorkHive";
        bookingOfEmail.License_Name = ownerfornoti.LicenseName;
        bookingOfEmail.License_Address = ownerfornoti.LicenseAddress;
        bookingOfEmail.Workspace_Name = workspaceOfOwner.Name;
        bookingOfEmail.Workspace_Category = workspaceOfOwner.Category;
        bookingOfEmail.Workspace_Capacity = workspaceOfOwner.Capacity;
        bookingOfEmail.Workspace_Area = workspaceOfOwner.Area;

        if (!string.IsNullOrWhiteSpace(command.PromotionCode))
        {
            var promotion = bookingUnit.promotion.GetAll()
                                .FirstOrDefault(p => p.WorkspaceId == command.WorkspaceId
                          && p.Code.Trim().ToLower() == command.PromotionCode.Trim().ToLower());

            if (promotion != null)
            {
                bookingOfEmail.Promotion_Code = promotion.Code;
                bookingOfEmail.Discount = promotion.Discount;
            }
            else
            {
                bookingOfEmail.Promotion_Code = "N/A";
                bookingOfEmail.Discount = 0;
            }
        }

        //amenity
        if(command.Amenities is not null && command.Amenities.Any())
        {
            foreach (var item in command.Amenities)
            {
                var amenity = bookingUnit.amenity.GetById(item.Id);
                if (amenity is null)
                    continue;
                
                var bookingAmenity = bookingUnit.bookAmenity.GetAll()
                    .FirstOrDefault(ba => ba.BookingId == newBooking.Id && ba.AmenityId == amenity.Id);

                amenities.Add(new BookingHistoryAmenity((int)bookingAmenity!.Quantity!, amenity.Name, (decimal)amenity.Price!, amenity.ImgUrl));
            }
        }

        //beverage
        if (command.Beverages is not null && command.Beverages.Any())
        {
            foreach (var item in command.Beverages)
            {
                var beverage = bookingUnit.beverage.GetById(item.Id);
                if (beverage is null)
                    continue;

                var bookingBeverage = bookingUnit.bookBeverage.GetAll()
                    .FirstOrDefault(ba => ba.BookingWorkspaceId == newBooking.Id && ba.BeverageId == beverage.Id);

                beverages.Add(new BookingHistoryBeverage((int)bookingBeverage!.Quantity!, beverage.Name, (decimal)beverage.Price!, beverage.ImgUrl));
            }
        }

        bookingOfEmail.BookingHistoryAmenities = amenities;
        bookingOfEmail.BookingHistoryBeverages = beverages;
        bookingOfEmail.Booking_Price = newBooking.Price;

        var emailBody = GenerateBookingDetailsEmailContent(bookingOfEmail);
        await emailService.SendEmailAsync(user.Email, "Thông tin đặt chỗ", emailBody);

        return new BookingByUserWalletResult("Đặt chỗ thành công, vui lòng kiểm tra email để xem thông tin chi tiết");
    }

    private string GenerateBookingDetailsEmailContent(BookingHistory booking)
    {
        var sb = new StringBuilder();

        // Hình ảnh tiêu đề
        sb.AppendLine($@"
    <div style='text-align: center; margin-bottom: 20px;'>
        <img src='https://res.cloudinary.com/dcq99dv8p/image/upload/v1743492632/mailWorkHive_gmegks.jpg' 
             style='width: 100%; max-width: 1350px; height: auto; display: block; margin: 0 auto;' 
             alt='Booking Image'>
    </div>");

        // Bảng thông tin thanh toán
        sb.AppendLine($@"
    <div style='display: flex; justify-content: center;'>
        <table style='border-collapse: collapse; width: 100%; max-width: 1350px; margin: 0 auto; font-family: Arial, sans-serif; border: 1px solid #ddd;'>
            <tr>
                <th colspan='3' 
                    style='background-color: #f8f3d4; 
                           padding: 15px; 
                           font-size: 22px; 
                           text-align: center;'>
                    THÔNG TIN ĐẶT CHỖ
                </th>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Mã đặt chỗ</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Booking_Id}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Mã không gian</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Workspace_Id}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Tên khách hàng</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.User_Name ?? "N/A"}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Nhận chỗ</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Booking_StartDate}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Trả chỗ</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Booking_EndDate}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Trạng thái</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Booking_Status}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Ngày tạo</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Booking_CreatedAt}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Phương thức thanh toán</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Payment_Method}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Tên quán</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.License_Name}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Địa chỉ quán</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.License_Address}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Tên không gian</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Workspace_Name}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Loại không gian</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Workspace_Category}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Số lượng</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Workspace_Capacity}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Khu vực</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Workspace_Area}</td>
            </tr>");

        // Amenity
        if (booking.BookingHistoryAmenities!.Any())
        {
            sb.AppendLine($@"
        <tr style='background-color: #ffff00;'>
            <td colspan='3' style='padding: 10px; font-size: 16px; font-weight: bold; text-align: center; border: 1px solid #ddd;'>Tiện ích</td>
        </tr>
        <tr>
            <th style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Tên tiện ích</th>
            <th style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Số lượng</th>
            <th style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Đơn giá (VNĐ)</th>
        </tr>");
            foreach (var amenity in booking.BookingHistoryAmenities!)
            {
                sb.AppendLine($@"
            <tr>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;'>{amenity.Name}</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;'>{amenity.Quantity}</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;'>{amenity.UnitPrice}</td>
            </tr>");
            }
        }

        // Beverages
        if (booking.BookingHistoryBeverages!.Any())
        {
            sb.AppendLine($@"
        <tr style='background-color: #ffff00;'>
            <td colspan='3' style='padding: 10px; font-size: 16px; font-weight: bold; text-align: center; border: 1px solid #ddd;'>Thực phẩm</td>
        </tr>
        <tr>
            <th style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Tên thực phẩm</th>
            <th style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Số lượng</th>
            <th style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Đơn giá (VNĐ)</th>
        </tr>");
            foreach (var beverage in booking.BookingHistoryBeverages!)
            {
                sb.AppendLine($@"
            <tr>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;'>{beverage.Name}</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;'>{beverage.Quantity}</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;'>{beverage.UnitPrice}</td>
            </tr>");
            }
        }

        // Tổng kết
        sb.AppendLine($@"
    <tr>
        <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Mã giảm giá</td>
        <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Promotion_Code ?? "N/A"}</td>
    </tr>
    <tr>
        <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Giảm giá</td>
        <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Discount ?? 0}</td>
    </tr>
    <tr>
        <td style='padding: 10px; font-size: 16px; font-weight: bold; color: red; border: 1px solid #ddd;'>Tổng tiền (sau giảm giá)</td>
        <td style='padding: 10px; font-size: 16px; font-weight: bold; color: red; border: 1px solid #ddd;' colspan='2'>{booking.Booking_Price}</td>
    </tr>
    </table>
    </div>");

        // Phần liên hệ (Contact Info)
        sb.AppendLine($@"
    <div style='background-color: #f8d7da; padding: 20px; margin-top: 20px; border-radius: 8px; max-width: 1350px; margin: 0 auto;'>
        <h2 style='text-align: center; font-family: Arial, sans-serif; color: #d63384;'>Liên hệ</h2>
        <p style='text-align: center; font-size: 16px; color: #d63384;'>
            Mail: <a href='mailto:workhive.vn.official@gmail.com' style='color: #d63384;'>workhive.vn.official@gmail.com</a><br>
            Hotline: 0867435157<br>
        </p>
    </div>");

        return sb.ToString();


    }
}
using System.Text;
using MediatR;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Common;
using WorkHive.Services.Constant;
using WorkHive.Services.EmailServices;
using WorkHive.Services.Users.DTOs;
using WorkHive.Services.WorkspaceTimes;

namespace WorkHive.Services.Users.Webhook;
public record ProcessWebhookCommand(WebhookType WebhookData) : ICommand<ProcessWebhookResult>;

public record ProcessWebhookResult(string Notification);

public class WebhookProccessingHandler(IConfiguration configuration,
    IBookingWorkspaceUnitOfWork bookUnit, IUserUnitOfWork userUnit, IEmailService emailService)
    : ICommandHandler<ProcessWebhookCommand, ProcessWebhookResult>
{
    private readonly string ClientID = configuration["PayOS:ClientId"]!;
    private readonly string ApiKey = configuration["PayOS:ApiKey"]!;
    private readonly string CheckSumKey = configuration["PayOS:CheckSumKey"]!;
    public async Task<ProcessWebhookResult> Handle(ProcessWebhookCommand command,
        CancellationToken cancellationToken)
    {
        PayOS payOS = new PayOS(ClientID, ApiKey, CheckSumKey);

        try
        {
            payOS.verifyPaymentWebhookData(command.WebhookData);
            var orderCode = command.WebhookData.data.orderCode;
            var orderCodeString = orderCode.ToString();

            // 1 cho booking, 2 cho deposit
            var typeCode = orderCodeString.Substring(0, 1);
            var timestampPart = orderCodeString.Substring(1, 6);

            if (command.WebhookData.code == "00" && typeCode == "1")
            {
                //Lấy booking Id
                var bookingIdStr = orderCodeString.Substring(7);
                var bookingId = int.Parse(bookingIdStr);

                //Tìm kiếm booking phù hợp
                var bookWorkspace = bookUnit.booking.GetById(bookingId);

                if (bookWorkspace == null)
                {
                    return new ProcessWebhookResult("Yêu cầu không hợp lệ");
                }

                //Nếu trạng thái thanh toán là thành công
                if (command.WebhookData.success == true)
                {
                    var workspaceTime = bookUnit.workspaceTime.GetAll()
                        .FirstOrDefault(x => x.BookingId.Equals(bookingId));

                    if (workspaceTime == null)
                    {
                        return new ProcessWebhookResult("Yêu cầu không hợp lệ");
                    }

                    //Đổi trạng thái của booking và workspaceTime
                    bookWorkspace.Status = BookingStatus.Success.ToString();
                    await bookUnit.booking.UpdateAsync(bookWorkspace);

                    workspaceTime.Status = WorkspaceTimeStatus.InUse.ToString();
                    await bookUnit.workspaceTime.UpdateAsync(workspaceTime);

                    //Cộng 90% giá tiền của đơn booking cho owner và ghi lại lịch sử giao dịch
                    var workspace = bookUnit.workspace.GetById(bookWorkspace.WorkspaceId);
                    var owner = userUnit.Owner.GetById(workspace.OwnerId);

                    var ownerWallet = await bookUnit.ownerWallet.GetOwnerWalletByOwnerIdForBooking(owner.Id);
                    var walletOfOwner = userUnit.Wallet.GetById(ownerWallet.WalletId);
                    walletOfOwner.Balance += (bookWorkspace.Price * 90) / 100;

                    await bookUnit.wallet.UpdateAsync(walletOfOwner);

                    //Tạo lịch sử giao dịch cho owner
                    var transactionHistoryOfOwner = new TransactionHistory
                    {
                        Amount = (bookWorkspace.Price * 90) / 100,
                        Status = "PAID",
                        Description = $"Nhận tiền đơn booking: {bookWorkspace.Id}",
                        CreatedAt = DateTime.Now,
                        Title = "Đặt chỗ",
                        BeforeTransactionAmount = walletOfOwner.Balance - (bookWorkspace.Price * 90) / 100,
                        AfterTransactionAmount = walletOfOwner.Balance
                    };
                    await userUnit.TransactionHistory.CreateAsync(transactionHistoryOfOwner);

                    var ownerTransactionHistory = new OwnerTransactionHistory
                    {
                        Status = "PAID",
                        TransactionHistoryId = transactionHistoryOfOwner.Id,
                        OwnerWalletId = ownerWallet.Id
                    };
                    await userUnit.OwnerTransactionHistory.CreateAsync(ownerTransactionHistory);

                    //Tạo lịch sử giao dịch cho user
                    var transactionHistoryOfUser = new TransactionHistory
                    {
                        Amount = bookWorkspace.Price,
                        Status = "PAID",
                        Description = $"Thanh toán đơn booking: {bookWorkspace.Id}",
                        CreatedAt = DateTime.Now,
                        Title = "Thanh toán thành công"
                    };
                    await userUnit.TransactionHistory.CreateAsync(transactionHistoryOfUser);

                    var userTransactionHistory = new UserTransactionHistory
                    {
                        Status = "PAID",
                        TransactionHistoryId = transactionHistoryOfUser.Id,
                        CustomerWalletId = walletOfOwner.Id
                    };

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
                        Description = $"Workspace: {workspacefornoti.Name} đã được đặt",
                        IsRead = 0,
                        Status = "PAID",
                        Title = "Đặt chỗ"
                    };
                    await bookUnit.ownerNotification.CreateAsync(ownerNotifi);

                    //Gửi mail cho user
                    var user = userUnit.User.GetById(bookWorkspace.UserId);
                    var bookingOfEmail = new BookingHistory();

                    //If null amenities and beverages will assign default list[]
                    var amenities = bookingOfEmail.BookingHistoryAmenities ?? new List<BookingHistoryAmenity>();
                    var beverages = bookingOfEmail.BookingHistoryBeverages ?? new List<BookingHistoryBeverage>();
                    var workspaceImages = bookingOfEmail.BookingHistoryWorkspaceImages ?? new List<BookingHistoryWorkspaceImage>();

                    //Fill thông tin
                    bookingOfEmail.Booking_Id = bookWorkspace.Id;
                    bookingOfEmail.Workspace_Id = bookWorkspace.WorkspaceId;
                    bookingOfEmail.User_Name = user.Name;
                    bookingOfEmail.Booking_StartDate = bookWorkspace.StartDate;
                    bookingOfEmail.Booking_EndDate = bookWorkspace.EndDate;
                    bookingOfEmail.Booking_Status = bookWorkspace.Status;
                    bookingOfEmail.Booking_CreatedAt = bookWorkspace.CreatedAt;
                    bookingOfEmail.Payment_Method = "PAYOS";
                    bookingOfEmail.License_Name = ownerfornoti.LicenseName;
                    bookingOfEmail.License_Address = ownerfornoti.LicenseAddress;
                    bookingOfEmail.Workspace_Name = workspacefornoti.Name;
                    bookingOfEmail.Workspace_Category = workspacefornoti.Category;
                    bookingOfEmail.Workspace_Capacity = workspacefornoti.Capacity;
                    bookingOfEmail.Workspace_Area = workspacefornoti.Area;
                    bookingOfEmail.Booking_Price = bookWorkspace.Price;

                    //Promotion for send email
                    if (bookWorkspace.PromotionId != null)
                    {
                        var promotion = bookUnit.promotion.GetById((int)bookWorkspace.PromotionId);
                        bookingOfEmail.Discount = promotion.Discount;
                        bookingOfEmail.Promotion_Code = promotion.Code;
                    }
                    else
                    {
                        bookingOfEmail.Discount = 0;
                        bookingOfEmail.Promotion_Code = "N/A";
                    }

                    //amenity
                    var amenitiesForSendEmail = bookUnit.bookAmenity.GetAll().Where(b => b.BookingId == bookWorkspace.Id).ToList();
                    if (amenitiesForSendEmail.Count > 0)
                    {
                        foreach (var item in amenitiesForSendEmail)
                        {
                            var amenity = bookUnit.amenity.GetById(item.AmenityId);

                            if (amenity is null)
                                continue;

                            amenities.Add(new BookingHistoryAmenity((int)item.Quantity!, amenity.Name, (decimal)amenity.Price!, amenity.ImgUrl));
                        }
                    }

                    //beverage
                    var beveragesForSendEmail = bookUnit.bookBeverage.GetAll().Where(b => b.BookingWorkspaceId == bookWorkspace.Id).ToList();
                    if (beveragesForSendEmail.Count > 0)
                    {
                        foreach (var item in beveragesForSendEmail)
                        {
                            var beverage = bookUnit.beverage.GetById(item.BeverageId);

                            if (beverage is null)
                                continue;

                            beverages.Add(new BookingHistoryBeverage((int)item.Quantity!, beverage.Name, (decimal)beverage.Price!, beverage.ImgUrl));
                        }
                    }
                    bookingOfEmail.BookingHistoryAmenities = amenities;
                    bookingOfEmail.BookingHistoryBeverages = beverages;

                    var emailBody = GenerateBookingDetailsEmailContent(bookingOfEmail, workspace);
                    await emailService.SendEmailAsync(user.Email, "Thông tin đặt chỗ", emailBody);

                    return new ProcessWebhookResult("Cập nhật trạng thái thành công, vui lòng kiểm tra email để xem thông tin chi tiết");
                }
                else
                {
                    var workspaceTime = bookUnit.workspaceTime.GetAll()
                        .FirstOrDefault(x => x.BookingId.Equals(bookWorkspace.Id));
                    if (workspaceTime is null)
                    {
                        return new ProcessWebhookResult("Yêu cầu không hợp lệ");
                    }

                    bookUnit.workspaceTime.Remove(workspaceTime);

                    var booking = bookUnit.booking.GetById(bookWorkspace.Id);
                    if (booking is null)
                    {
                        return new ProcessWebhookResult("Yêu cầu không hợp lệ");
                    }

                    booking.Status = BookingStatus.Fail.ToString();

                    bookUnit.booking.Update(booking);

                    await bookUnit.SaveAsync();
                }

            }


            if (command.WebhookData.code == "00" && typeCode == "2")
            {
                if (command.WebhookData.success == true)
                {
                    //Lấy customerWalletId
                    var customerWalletIdStr = orderCodeString.Substring(7);
                    var customerWalletId = int.Parse(customerWalletIdStr);

                    //Tìm kiếm customerWallet phù hợp
                    var customerWallet = userUnit.CustomerWallet.GetById(customerWalletId);
                    var wallet = userUnit.Wallet.GetById(customerWallet.WalletId);
                    wallet.Balance += command.WebhookData.data.amount;
                    await userUnit.Wallet.UpdateAsync(wallet);

                    //Create Transaction History for user
                    var transactionHistory = new TransactionHistory
                    {
                        Amount = command.WebhookData.data.amount,
                        Status = PayOSStatus.PAID.ToString(),
                        Description = $"Nạp tiền vào ví đã được xử lý thành công.",
                        CreatedAt = DateTime.Now,
                        Title = "Nạp tiền thành công",
                        BeforeTransactionAmount = wallet.Balance - command.WebhookData.data.amount,
                        AfterTransactionAmount = wallet.Balance,
                    };
                    await userUnit.TransactionHistory.CreateAsync(transactionHistory);

                    var userTransactionHistory = new UserTransactionHistory
                    {
                        Status = PayOSStatus.PAID.ToString(),
                        TransactionHistoryId = transactionHistory.Id,
                        CustomerWalletId = customerWallet.Id
                    };
                    await userUnit.UserTransactionHistory.CreateAsync(userTransactionHistory);

                    //Thông báo cho người dùng
                    var userNotifi = new UserNotification
                    {
                        UserId = customerWallet.UserId,
                        IsRead = 0,
                        CreatedAt = DateTime.Now,
                        Description = $"Nội dung:\r\nBạn đã nạp tiền thành công.\r\nSố tiền: {((decimal)command.WebhookData.data.amount).ToVnd()}\r\nVui lòng kiểm tra lại thông tin trong mục Lịch sử giao dịch. Chúng tôi mong được phục vụ bạn!",
                        Status = "PAID",
                        Title = "Nạp tiền thành công"
                    };
                    await userUnit.UserNotification.CreateAsync(userNotifi);

                    return new ProcessWebhookResult("Cập nhật thành công");
                }
            }

        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error processing webhook: {ex.Message}");
        }

        return new ProcessWebhookResult("Xử lý webhook thành công");
    }

    private string GenerateBookingDetailsEmailContent(BookingHistory booking, Workspace workspace)
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
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Mã bàn</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{workspace.Code}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Tên khách hàng</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.User_Name ?? "N/A"}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Thời gian nhận chỗ</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Booking_StartDate}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Thời gian trả chỗ</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Booking_EndDate}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Trạng thái</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>Thành công</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Thời điểm đặt chỗ</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Booking_CreatedAt}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Phương thức thanh toán</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Payment_Method}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Thương hiệu</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.License_Name}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Địa chỉ</td>
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
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Sức chứa (người)</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{booking.Workspace_Capacity}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Diện tích (m2)</td>
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
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;'>{amenity.UnitPrice.ToVnd()}</td>
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
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;'>{beverage.UnitPrice.ToVnd()}</td>
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
        <td style='padding: 10px; font-size: 16px; font-weight: bold; color: red; border: 1px solid #ddd;' colspan='2'>{booking.Booking_Price.ToVnd()}</td>
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
using FluentValidation;
using Microsoft.AspNetCore.Http;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant;
using WorkHive.Services.Exceptions;
using WorkHive.Repositories.IRepositories;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;

namespace WorkHive.Services.Users.BookingWorkspace;

public record BookingWorkspaceCommand(int WorkspaceId, int PaymentId, DateTime StartDate, DateTime EndDate,
    List<BookingAmenity> Amenities, List<BookingBeverage> Beverages, string PromotionCode, decimal Price)
    : ICommand<BookingWorkspaceResult>;
public record BookingWorkspaceResult(string Bin, string AccountNumber, int Amount, string Description, 
    long OrderCode, string PaymentLinkId, string Status, string CheckoutUrl, string QRCode);

/*
public class BookingWorkspaceValidator : AbstractValidator<BookingWorkspaceCommand>
{
    public BookingWorkspaceValidator()
    {

    }
}
*/

public class BookingWorkspaceHandler(IHttpContextAccessor httpContext, ITokenRepository tokenRepo,
    IBookingWorkspaceUnitOfWork bookingUnitOfWork, IConfiguration configuration)
    : ICommandHandler<BookingWorkspaceCommand, BookingWorkspaceResult>
{
    private readonly string ClientID = configuration["PayOS:ClientId"]!;
    private readonly string ApiKey = configuration["PayOS:ApiKey"]!;
    private readonly string CheckSumKey = configuration["CheckSumKey"]!;
    public async Task<BookingWorkspaceResult> Handle(BookingWorkspaceCommand command, 
        CancellationToken cancellationToken)
    {
        var newBooking = new Booking();

        //Get userId and roleId for Booking in session containing token in a session working
        var token = httpContext.HttpContext!.Session.GetString("token")!.ToString();
        var listInfo = tokenRepo.DecodeJwtToken(token);

        var userId = listInfo[0];
        var roleId = listInfo[1];

        newBooking.UserId = Convert.ToInt32(userId); //Add userId for booking

        //Check validate for start date and end date
        var workspaceTimes = bookingUnitOfWork.workspaceTime.GetAll()
            .Where(x => x.WorkspaceId.Equals(command.WorkspaceId)).ToList();

        if (bookingUnitOfWork.workspaceTime.IsOverlap(workspaceTimes, command.StartDate, command.EndDate))
            throw new BadBookingRequestException("Khoảng thời gian bị trùng với các khoảng đã thuê");
        else
        {
            // Create a workspace time for workspace
            var booingTime = new WorkspaceTime
            {
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                Status = WorkspaceTimeStatus.Handling.ToString(),
                WorkspaceId = command.WorkspaceId,
                BookingId = newBooking.Id
            };

            newBooking.StartDate = booingTime.StartDate; //
            newBooking.EndDate = booingTime.EndDate; //
        }
            
        //Add Amenity and Beverage for Booking
        foreach (var item in command.Amenities)
        {
            if (item.Quantity > bookingUnitOfWork.amenity.GetById(item.AmenityId).Quantity)
                throw new AmenityBadRequestException("Số lượng tiện nghi không được lớn hơn số lượng trong kho");

            var amenity = bookingUnitOfWork.amenity.GetById(item.AmenityId);

            amenity.Quantity = amenity.Quantity - item.Quantity;

            await bookingUnitOfWork.amenity.UpdateAsync(amenity);

            item.BookingId = newBooking.Id; //

            await bookingUnitOfWork.bookAmenity.UpdateAsync(item);
        }

        foreach (var item in command.Beverages)
        {
            item.BookingWorkspaceId = newBooking.Id; //

            await bookingUnitOfWork.bookBeverage.UpdateAsync(item);
        }

        //Add promotion for booking

        var codeDiscount = bookingUnitOfWork.promotion.GetAll().
            Where(p => p.Code.Equals(command.PromotionCode)).FirstOrDefault();

        if (codeDiscount is null)
            throw new PromotionNotFoundException("Mã giảm giá không hợp lệ");

        if (codeDiscount.Status.Equals(PromotionStatus.Expired))
        {
            throw new PromotionNotFoundException("Mã giảm giá đã hết hạn");
        }

        newBooking.PromotionId = codeDiscount.Id; //

        newBooking.Price = command.Price; //
        newBooking.WorkspaceId = command.WorkspaceId; //
        newBooking.PaymentId = command.PaymentId; //
        newBooking.CreatedAt = DateTime.UtcNow;
        newBooking.Status = BookingStatus.Handling.ToString(); //

        bookingUnitOfWork.booking.Create(newBooking);
        await bookingUnitOfWork.SaveAsync();

        //Integrate payOS for booking
        var payOS = new PayOS(ClientID, ApiKey, CheckSumKey);
        var items = new List<ItemData>();

        foreach (var item in command.Amenities)
        {
            var amenity = bookingUnitOfWork.amenity.GetById(item.AmenityId);
            items.Add(new ItemData(name: amenity.Name, quantity: (int)item.Quantity!, price: (int)amenity.Price!));
        }

        foreach (var item in command.Beverages)
        {
            var beverage = bookingUnitOfWork.beverage.GetById(item.BeverageId);
            items.Add(new ItemData(name: beverage.Name, quantity: (int)item.Quantity!, price: (int)beverage.Price!));
        }

        var domain = configuration["PayOS:Domain"]!;
        var paymentLinkRequest = new PaymentData(
                orderCode: newBooking.Id,
                amount: (int)(newBooking.Price * 100),
                description: "Thanh toán không gian làm việc",
                returnUrl: domain + "/success",
                cancelUrl : domain + "/checkout",
                items : items
            );

        var link = await payOS.createPaymentLink(paymentLinkRequest);

        return new BookingWorkspaceResult(link.bin, link.accountNumber, link.amount, link.description, 
            link.orderCode, link.paymentLinkId, link.status, link.checkoutUrl, link.qrCode);
    }
}

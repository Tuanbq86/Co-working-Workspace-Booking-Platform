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
using Microsoft.IdentityModel.JsonWebTokens;
using WorkHive.Services.Users.DTOs;

namespace WorkHive.Services.Users.BookingWorkspace;

public record BookingWorkspaceCommand(int UserId, int WorkspaceId, string StartDate, string EndDate,
    List<BookingAmenityDTO> Amenities, List<BookingBeverageDTO> Beverages, string PromotionCode, decimal Price)
    : ICommand<BookingWorkspaceResult>;
public record BookingWorkspaceResult(int BookingId, string Bin, string AccountNumber, int Amount, string Description, 
    long OrderCode, string PaymentLinkId, string Status, string CheckoutUrl, string QRCode);

/*
public class BookingWorkspaceValidator : AbstractValidator<BookingWorkspaceCommand>
{
    public BookingWorkspaceValidator()
    {

    }
}
*/

public class BookingWorkspaceHandler(IBookingWorkspaceUnitOfWork bookingUnitOfWork, IConfiguration configuration)
    : ICommandHandler<BookingWorkspaceCommand, BookingWorkspaceResult>
{
    private readonly string ClientID = configuration["PayOS:ClientId"]!;
    private readonly string ApiKey = configuration["PayOS:ApiKey"]!;
    private readonly string CheckSumKey = configuration["PayOS:CheckSumKey"]!;
    public async Task<BookingWorkspaceResult> Handle(BookingWorkspaceCommand command, 
        CancellationToken cancellationToken)
    {
        var newBooking = new Booking();

        //Get userId and roleId for Booking in session containing token in a session working
        //var token = httpContext.HttpContext!.Session.GetString("token")!.ToString();
        //var listInfo = tokenRepo.DecodeJwtToken(token);

        //var userId = listInfo[JwtRegisteredClaimNames.Sub];

        newBooking.UserId = command.UserId;//Add userId for booking
        newBooking.Price = command.Price; //
        newBooking.WorkspaceId = command.WorkspaceId; //
        newBooking.PaymentId = 1; //
        newBooking.CreatedAt = DateTime.Now;
        newBooking.Status = BookingStatus.Handling.ToString(); //

        newBooking.StartDate = DateTime.ParseExact(command.StartDate, "HH:mm dd/MM/yyyy",
            System.Globalization.CultureInfo.InvariantCulture); //

        newBooking.EndDate = DateTime.ParseExact(command.EndDate, "HH:mm dd/MM/yyyy",
            System.Globalization.CultureInfo.InvariantCulture); //


        await bookingUnitOfWork.booking.CreateAsync(newBooking);


        //Add promotion for booking

        if (!string.IsNullOrWhiteSpace(command.PromotionCode))
        {
            var codeDiscount = bookingUnitOfWork.promotion.GetAll()
                .Where(p => p.Code.ToLower().Trim().Equals(command.PromotionCode.ToLower().Trim()))
                .FirstOrDefault();

            if (codeDiscount is null)
                throw new PromotionNotFoundException("Mã giảm giá không hợp lệ");

            if (codeDiscount.Status.Equals(PromotionStatus.Expired))
                throw new PromotionNotFoundException("Mã giảm giá đã hết hạn");

            // Nếu mã giảm giá hợp lệ thì lưu PromotionId
            newBooking.PromotionId = codeDiscount.Id;
        }

        //Add List amenity, beverage item for payOS
        var amenityItems = new List<AmenityItemDTO>();
        var beverageItems = new List<BeverageItemDTO>();

        //Add amenities and beverages for booking

        //Amenity
        if (command.Amenities is not null && command.Amenities.Any())
        {
            foreach (var item in command.Amenities)
            {
                var amenity = bookingUnitOfWork.amenity.GetById(item.Id);

                if (amenity is null || item.Quantity > amenity.Quantity)
                    throw new AmenityBadRequestException("Sản phẩm đã hết hàng hoặc số lượng trong kho không đủ");

                var newBookingAmenity = new BookingAmenity
                {
                    Quantity = item.Quantity,
                    BookingId = newBooking.Id,
                    AmenityId = amenity.Id
                };

                await bookingUnitOfWork.bookAmenity.CreateAsync(newBookingAmenity);

                amenity.Quantity -= newBookingAmenity.Quantity;

                await bookingUnitOfWork.amenity.UpdateAsync(amenity);

                amenityItems.Add(new AmenityItemDTO
                {
                    Name = amenity.Name,
                    Quantity = newBookingAmenity.Quantity,
                    Price = (int)((amenity.Price * newBookingAmenity.Quantity) * 100)!
                });
            }
        }

        //Beverage
        if (command.Beverages is not null && command.Beverages.Any())
        {
            foreach (var item in command.Beverages)
            {
                var beverage = bookingUnitOfWork.beverage.GetById(item.Id);

                if (beverage is null)
                {
                    throw new BeverageBadRequestException("Can not find beverage");
                }

                var newBookingBeverage = new BookingBeverage
                {
                    Quantity = item.Quantity,
                    BookingWorkspaceId = newBooking.Id,
                    BeverageId = beverage.Id
                };

                await bookingUnitOfWork.bookBeverage.CreateAsync(newBookingBeverage);

                beverageItems.Add(new BeverageItemDTO
                {
                    Name = beverage.Name,
                    Quantity = newBookingBeverage.Quantity,
                    Price = (int)((beverage.Price * newBookingBeverage.Quantity) * 100)!
                });
            }
        }

        //-------------------------------------------------------------------

        var newWorkspaceTime = new WorkspaceTime
        {
            StartDate = newBooking.StartDate,
            EndDate = newBooking.EndDate,
            Status = WorkspaceTimeStatus.Handling.ToString(),
            WorkspaceId = newBooking.WorkspaceId,
            BookingId = newBooking.Id
        };

        bookingUnitOfWork.workspaceTime.Create(newWorkspaceTime);

        await bookingUnitOfWork.SaveAsync();

        //Integrate payOS for booking
        var payOS = new PayOS(ClientID, ApiKey, CheckSumKey);
        var items = new List<ItemData>();

        if (!amenityItems.Count.Equals(0))
        {
            foreach (var item in amenityItems)
                items.Add(new ItemData(name: item.Name, quantity: (int)item.Quantity!, price: item.Price));
        }

        if (!beverageItems.Count.Equals(0))
        {
            foreach (var item in beverageItems)
                items.Add(new ItemData(name: item.Name, quantity: (int)item.Quantity!, price: item.Price));

        }

        //create order code with time increasing by time
        var orderCode = long.Parse(DateTime.UtcNow.Ticks.ToString()[^10..]);

        var domain = configuration["PayOS:Domain"]!;
        var paymentLinkRequest = new PaymentData(
                orderCode: orderCode,
                amount: (int)newBooking.Price,
                description: "WorkHive",
                returnUrl: domain + "/success",
                cancelUrl : domain + "/fail",
                items : items
            );

        var link = await payOS.createPaymentLink(paymentLinkRequest);

        return new BookingWorkspaceResult(newBooking.Id, link.bin, link.accountNumber, link.amount, link.description, 
            link.orderCode, link.paymentLinkId, link.status, link.checkoutUrl, link.qrCode);
    }
}
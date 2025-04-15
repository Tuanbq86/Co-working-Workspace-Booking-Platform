using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Net.payOS.Types;
using Net.payOS;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant;
using WorkHive.Services.Users.DTOs;

namespace WorkHive.Services.Users.BookingWorkspace.BookingForMobile;

public record BookingForMobileCommand(int UserId, int WorkspaceId, string StartDate, string EndDate,
    List<BookingAmenityDTO> Amenities, List<BookingBeverageDTO> Beverages, string PromotionCode, decimal Price, string WorkspaceTimeCategory)
    : ICommand<BookingForMobileResult>;
public record BookingForMobileResult(int BookingId, string Bin, string AccountNumber, int Amount, string Description,
    long OrderCode, string PaymentLinkId, string Status, string CheckoutUrl, string QRCode);

public class BookingForMobileHandler(IBookingWorkspaceUnitOfWork bookingUnitOfWork, IConfiguration configuration)
    : ICommandHandler<BookingForMobileCommand, BookingForMobileResult>
{
    private readonly string ClientID = configuration["PayOS:ClientId"]!;
    private readonly string ApiKey = configuration["PayOS:ApiKey"]!;
    private readonly string CheckSumKey = configuration["PayOS:CheckSumKey"]!;
    public async Task<BookingForMobileResult> Handle(BookingForMobileCommand command, 
        CancellationToken cancellationToken)
    {
        var newBooking = new Booking();

        newBooking.UserId = command.UserId;//Add userId for booking
        newBooking.Price = command.Price; //
        newBooking.WorkspaceId = command.WorkspaceId; //
        newBooking.PaymentId = 1; //
        newBooking.CreatedAt = DateTime.Now;
        newBooking.Status = BookingStatus.Handling.ToString(); //
        newBooking.IsReview = 0; //
        newBooking.IsFeedback = 0; //

        newBooking.StartDate = DateTime.ParseExact(command.StartDate, "HH:mm dd/MM/yyyy",
            System.Globalization.CultureInfo.InvariantCulture); //

        newBooking.EndDate = DateTime.ParseExact(command.EndDate, "HH:mm dd/MM/yyyy",
            System.Globalization.CultureInfo.InvariantCulture); //


        await bookingUnitOfWork.booking.CreateAsync(newBooking);


        //Add promotion for booking
        if (!string.IsNullOrWhiteSpace(command.PromotionCode))
        {
            var promotion = bookingUnitOfWork.promotion.GetAll()
                                .FirstOrDefault(p => p.WorkspaceId == command.WorkspaceId
                          && p.Code.Trim().ToLower() == command.PromotionCode.Trim().ToLower());

            if (promotion != null)
            {
                newBooking.PromotionId = promotion.Id;
                newBooking.Price -= (newBooking.Price * promotion.Discount) / 100;
                await bookingUnitOfWork.booking.UpdateAsync(newBooking);
            }
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
                    continue;

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
                    Price = (int)((amenity.Price * newBookingAmenity.Quantity))!
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
                    continue;

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
                    Price = (int)((beverage.Price * newBookingBeverage.Quantity))!
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
            BookingId = newBooking.Id,
            Category = command.WorkspaceTimeCategory
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
        var timestamp = DateTime.UtcNow.Ticks.ToString()[^6..]; // Lấy 6 chữ số cuối của timestamp
        var orderCode = long.Parse($"{newBooking.Id}{timestamp}"); // Kết hợp bookingId và timestamp
        //Tạo thời gian hết hạn cho link thanh toán
        var expiredAt = DateTimeOffset.Now.AddMinutes(15).ToUnixTimeSeconds();

        //Return url and cancel url
        var returnurl = $"mobile://success?OrderCode={orderCode}&BookingId={newBooking.Id}";
        var cancelurl = $"mobile://cancel";

        var domain = configuration["PayOS:Domain"]!;
        var paymentLinkRequest = new PaymentData(
                orderCode: orderCode,
                amount: (int)newBooking.Price!,
                description: $"bookpayment",
                returnUrl: returnurl,
                cancelUrl: cancelurl,
                items: items,
                expiredAt: expiredAt
            );

        var link = await payOS.createPaymentLink(paymentLinkRequest);

        return new BookingForMobileResult(newBooking.Id, link.bin, link.accountNumber, link.amount, link.description,
            link.orderCode, link.paymentLinkId, link.status, link.checkoutUrl, link.qrCode);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.DTOService;

namespace WorkHive.Services.Users.BookingWorkspace.GetBookingById;

public record GetBookingByBookingIdQuery(int BookingId) : IQuery<GetBookingByBookingIdResult>;
public record GetBookingByBookingIdResult(BookingByBookingIdDTO BookingByBookingIdDTO);

public class GetBookingByIdHandler(IBookingWorkspaceUnitOfWork bookingUnit)
    : IQueryHandler<GetBookingByBookingIdQuery, GetBookingByBookingIdResult>
{
    public async Task<GetBookingByBookingIdResult> Handle(GetBookingByBookingIdQuery query, 
        CancellationToken cancellationToken)
    {
        var booking = await bookingUnit.booking.GetByIdAsync(query.BookingId);

        BookingByBookingIdDTO bookingByBookingIdDTO = new BookingByBookingIdDTO();

        bookingByBookingIdDTO.BookingId = booking.Id;
        bookingByBookingIdDTO.Start_Date = booking.StartDate;
        bookingByBookingIdDTO.End_Date = booking.EndDate;
        bookingByBookingIdDTO.Price = booking.Price;
        bookingByBookingIdDTO.Status = booking.Status;
        bookingByBookingIdDTO.Created_At = booking.CreatedAt;
        bookingByBookingIdDTO.UserId = booking.UserId;
        bookingByBookingIdDTO.WorkspaceId = booking.WorkspaceId;

        bookingByBookingIdDTO.PromotionId = booking.PromotionId;

        var payment = bookingUnit.payment.GetById(booking.PaymentId);
        bookingByBookingIdDTO.Payment_Method = payment.Method;

        var bookingAmenities = await bookingUnit.bookAmenity.GetAllBookingAmenityByBookingId(booking.Id);
        var bookingbeverages = await bookingUnit.bookBeverage.GetAllBookingBeverageByBookingId(booking.Id);

        List<BookingAmenityByBookingId> bookingAmenityByBookingIds = new List<BookingAmenityByBookingId>();
        List<BookingBeverageByBookingId> bookingBeverageByBookingIds = new List<BookingBeverageByBookingId>();

        foreach (var bookAmenitybooking in bookingAmenities)
        {
            bookingAmenityByBookingIds.Add(new BookingAmenityByBookingId(bookAmenitybooking.Amenity.Id,
                (int)bookAmenitybooking.Quantity!, bookAmenitybooking.Amenity.Name, bookAmenitybooking.Amenity.ImgUrl, (decimal)bookAmenitybooking.Amenity.Price!));
        }

        foreach (var bookBeveragebooking in bookingbeverages)
        {
            bookingBeverageByBookingIds.Add(new BookingBeverageByBookingId(bookBeveragebooking.Beverage.Id, (
                int)bookBeveragebooking.Quantity!, bookBeveragebooking.Beverage.Name, bookBeveragebooking.Beverage.ImgUrl, (decimal)bookBeveragebooking.Beverage.Price!));
        }

        bookingByBookingIdDTO.Amenities = bookingAmenityByBookingIds;
        bookingByBookingIdDTO.Beverages = bookingBeverageByBookingIds;

        return new GetBookingByBookingIdResult(bookingByBookingIdDTO);
    }
}

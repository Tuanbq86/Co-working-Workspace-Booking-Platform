using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using WorkHive.APIs.Users.Booking.GetBookingById;
using WorkHive.Services.DTOService;
using WorkHive.Services.Users.BookingWorkspace.GetBookingById;
using FluentAssertions;

namespace UnitTestAPI.Booking;

public class GetBookingByIdEndpointTest
{
    [Fact]
    public async Task GetBookingById_ReturnsOk_WithExpectedData()
    {
        // Arrange
        var fakeSender = A.Fake<ISender>();
        int bookingId = 1;

        var expectedBooking = new BookingByBookingIdDTO
        {
            BookingId = bookingId,
            Start_Date = DateTime.Parse("2025-04-24 09:00"),
            End_Date = DateTime.Parse("2025-04-24 12:00"),
            Price = 300000,
            Status = "Success",
            Created_At = DateTime.Parse("2025-04-22"),
            UserId = 2,
            WorkspaceId = 2,
            PromotionId = 5,
            Payment_Method = "PayOS",
            Amenities = new List<BookingAmenityByBookingId>
                {
                    new BookingAmenityByBookingId(1, 2, "Máy chiếu", "img_url_1", 100000),
                    new BookingAmenityByBookingId(2, 1, "Bảng trắng", "img_url_2", 50000)
                },
            Beverages = new List<BookingBeverageByBookingId>
                {
                    new BookingBeverageByBookingId(1, 1, "Cà phê sữa", "img_url_cafe", 25000),
                    new BookingBeverageByBookingId(2, 2, "Matcha latter", "img_url_tea", 15000)
                }
        };

        A.CallTo(() => fakeSender.Send(
            A<GetBookingByBookingIdQuery>.That.Matches(q => q.BookingId == bookingId),
            default)).Returns(new GetBookingByBookingIdResult(expectedBooking));

        var endpointDelegate = async (int id, ISender sender) =>
        {
            var result = await sender.Send(new GetBookingByBookingIdQuery(id));
            var response = new GetBookingByBookingIdResponse(result.BookingByBookingIdDTO);
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(bookingId, fakeSender);

        // Assert
        result.Should().BeOfType<Ok<GetBookingByBookingIdResponse>>();

        var okResult = result as Ok<GetBookingByBookingIdResponse>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value;
        response.Should().NotBeNull();
        response.BookingByBookingIdDTO.Should().BeEquivalentTo(expectedBooking);
    }
}

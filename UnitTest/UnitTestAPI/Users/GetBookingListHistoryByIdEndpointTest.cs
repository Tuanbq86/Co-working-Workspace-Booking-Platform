using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using WorkHive.APIs.Users.Booking;
using WorkHive.Services.Users.BookingWorkspace;
using WorkHive.Services.Users.DTOs;
using Mapster;
using FluentAssertions;

namespace UnitTestAPI.Users;

public class GetBookingHistoryListByIdTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new GetBookingHistoryListByIdRequest(1),
            new List<BookingHistory>
            {
                new BookingHistory
                {
                    Booking_Id = 1001,
                    Booking_StartDate = DateTime.Parse("2025-01-01"),
                    Booking_EndDate = DateTime.Parse("2025-01-01").AddHours(2),
                    Booking_Price = 100,
                    Booking_Status = "Confirmed",
                    Booking_CreatedAt = DateTime.Parse("2024-12-25"),
                    Payment_Method = "Wallet",
                    Workspace_Name = "Room A",
                    Workspace_Capacity = 10,
                    Workspace_Category = "Meeting Room",
                    Workspace_Description = "Nice Room",
                    Workspace_Area = 20,
                    Workspace_Id = 5,
                    Workspace_CleanTime = 30,
                    Promotion_Code = "NEWYEAR25",
                    Discount = 25,
                    IsReview = 1,
                    IsFeedback = 1,
                    License_Name = "WorkHive Inc.",
                    License_Address = "123 Street, City",
                    BookingHistoryAmenities = new List<BookingHistoryAmenity>
                    {
                        new(1, "Projector", 20, "https://example.com/projector.jpg")
                    },
                    BookingHistoryBeverages = new List<BookingHistoryBeverage>
                    {
                        new(2, "Coffee", 5, "https://example.com/coffee.jpg")
                    },
                    BookingHistoryWorkspaceImages = new List<BookingHistoryWorkspaceImage>
                    {
                        new("https://example.com/room.jpg")
                    }
                }
            }
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}


public class GetBookingListHistoryByIdEndpointTest
{
    [Theory]
    [ClassData(typeof(GetBookingHistoryListByIdTestData))]
    public async Task GetBookingHistoryListByIdEndpoint_ReturnsExpectedResult(
    GetBookingHistoryListByIdRequest request,
    List<BookingHistory> expectedHistories)
    {
        // Arrange
        var mockSender = A.Fake<ISender>();

        var result = new GetBookingHistoryListByIdResult(expectedHistories);

        A.CallTo(() => mockSender.Send(A<GetBookingHistoryListByIdQuery>._, default))
            .Returns(result);

        var endpointDelegate = async ([AsParameters] GetBookingHistoryListByIdRequest req, ISender sender) =>
        {
            var query = req.Adapt<GetBookingHistoryListByIdQuery>();
            var res = await sender.Send(query);
            var response = new GetBookingHistoryListByIdResponse(res.BookingHistories);
            return Results.Ok(response);
        };

        // Act
        var actualResult = await endpointDelegate(request, mockSender);

        // Assert
        actualResult.Should().BeOfType<Ok<GetBookingHistoryListByIdResponse>>();

        var okResult = actualResult as Ok<GetBookingHistoryListByIdResponse>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value;
        response.Should().NotBeNull();
        response!.BookingHistories.Should().BeEquivalentTo(expectedHistories);
    }
}

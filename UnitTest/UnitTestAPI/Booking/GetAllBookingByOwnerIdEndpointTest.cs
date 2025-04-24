using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using WorkHive.APIs.Users.Booking.GetAllBookingByOwnerId;
using WorkHive.Services.DTOService;
using WorkHive.Services.Users.BookingWorkspace.GetAllBookingByOwnerId;
using FluentAssertions;

namespace UnitTestAPI.Booking;

public class GetAllBookingByOwnerIdEndpointTest
{
    [Fact]
    public async Task GetAllBookingByOwnerId_ReturnsOk_WithExpectedBookings()
    {
        // Arrange
        var fakeSender = A.Fake<ISender>();
        int ownerId = 1;

        var expectedBookings = new List<BookingByOwnerIdDTO>
            {
                new BookingByOwnerIdDTO
                {
                    BookingId = 1,
                    Start_Date = DateTime.Parse("2025-04-24 08:00"),
                    End_Date = DateTime.Parse("2025-04-24 10:00"),
                    Price = 200000,
                    Status = "Success",
                    Created_At = DateTime.Parse("2025-04-20"),
                    UserId = 2,
                    WorkspaceId = 6
                },
                new BookingByOwnerIdDTO
                {
                    BookingId = 12,
                    Start_Date = DateTime.Parse("2025-04-25 13:00"),
                    End_Date = DateTime.Parse("2025-04-25 15:00"),
                    Price = 150000,
                    Status = "Pending",
                    Created_At = DateTime.Parse("2025-04-21"),
                    UserId = 2,
                    WorkspaceId = 1
                }
            };

        A.CallTo(() => fakeSender.Send(A<GetAllBookingByOwnerIdQuery>.That.Matches(q => q.OwnerId == ownerId), default))
            .Returns(new GetAllBookingByOwnerIdResult(expectedBookings));

        var endpointDelegate = async (int id, ISender sender) =>
        {
            var result = await sender.Send(new GetAllBookingByOwnerIdQuery(id));
            var response = new GetAllBookingByOwnerIdResponse(result.BookingByOwnerIdDTOs);
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(ownerId, fakeSender);

        // Assert
        result.Should().BeOfType<Ok<GetAllBookingByOwnerIdResponse>>();

        var okResult = result as Ok<GetAllBookingByOwnerIdResponse>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value;
        response.Should().NotBeNull();
        response.BookingByOwnerIdDTOs.Should().HaveCount(2);
        response.BookingByOwnerIdDTOs.Should().BeEquivalentTo(expectedBookings);
    }
}

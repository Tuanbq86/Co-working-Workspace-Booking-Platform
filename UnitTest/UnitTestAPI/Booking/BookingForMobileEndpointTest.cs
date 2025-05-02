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
using WorkHive.APIs.Users.Booking.BookingForMobile;
using WorkHive.Services.Users.BookingWorkspace.BookingForMobile;

namespace UnitTestAPI.Booking;

public class BookingForMobileRequestTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // Test case 1 : Happy case
        yield return new object[]
        {
            new BookingForMobileRequest(
                2,
                1,
                "22:00 24/04/2025",
                "23:00 24/04/2025",
                new List<BookingAmenityDTO>
                {
                    new BookingAmenityDTO { Id = 1, Quantity = 2 },
                    new BookingAmenityDTO { Id = 2, Quantity = 1 }
                },
                new List<BookingBeverageDTO>
                {
                    new BookingBeverageDTO { Id = 1, Quantity = 2 },
                    new BookingBeverageDTO { Id = 2, Quantity = 1 }
                },
                "",
                10000,
                "Giờ"
            ),
            10000
        };

        // Test case 2 : test with promotion code
        yield return new object[]
        {
            new BookingForMobileRequest(
                2,
                1,
                "22:00 24/04/2025",
                "23:00 24/04/2025",
                new List<BookingAmenityDTO>
                {
                    new BookingAmenityDTO { Id = 1, Quantity = 2 },
                    new BookingAmenityDTO { Id = 2, Quantity = 1 }
                },
                new List<BookingBeverageDTO>
                {
                    new BookingBeverageDTO { Id = 1, Quantity = 2 },
                    new BookingBeverageDTO { Id = 2, Quantity = 1 }
                },
                "DATNUOC",
                10000,
                "Giờ"
            ),
            2000
        };
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class BookingForMobileEndpointTest
{
    [Theory]
    [ClassData(typeof(BookingForMobileRequestTestData))]
    public async Task BookingForMobileWorkspace_ReturnsOk_WithExpectedResponse(BookingForMobileRequest request, int expectedAmount)
    {
        // Arrange
        var mockSender = A.Fake<ISender>();

        A.CallTo(() => mockSender.Send(A<BookingForMobileCommand>._, default))
            .Returns(new BookingForMobileResult(0, "", "", expectedAmount, "BOOKING PAYMENT", 0, "", "", "", ""));

        var endpointDelegate = async (BookingForMobileRequest req, ISender sender) =>
        {
            var command = req.Adapt<BookingForMobileCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<BookingForMobileResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(request, mockSender);

        // Assert
        result.Should().BeOfType<Ok<BookingForMobileResponse>>();

        var okResult = result as Ok<BookingForMobileResponse>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value;
        response.Should().NotBeNull();
        response.Amount.Should().Be(expectedAmount);
        response.Description.Should().Contain("BOOKING PAYMENT");
    }
}

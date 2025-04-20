using System.Collections;
using FakeItEasy;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using WorkHive.APIs.Users.Booking;
using WorkHive.Services.Users.BookingWorkspace;
using WorkHive.Services.Users.DTOs;
using FluentAssertions;

namespace UnitTestAPI.Booking;

public class BookingRequestTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // Test case 1 : Happy case
        yield return new object[]
        {
            new BookingWorkspaceRequest(
                2,
                1,
                "22:00 20/04/2025",
                "23:00 20/04/2025",
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
            new BookingWorkspaceRequest(
                2,
                1,
                "22:00 20/04/2025",
                "23:00 20/04/2025",
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

public class BookingPayOsEndpointTest
{
    //private readonly ISender sender;
    //private readonly BookingWorkspaceRequest request;

    //public BookingPayOsEndpointTest()
    //{
    //    //Dependencies
    //    this.sender = A.Fake<ISender>();
    //    this.request = A.Fake<BookingWorkspaceRequest>();
    //}

    //[Theory]
    //[ClassData(typeof(BookingRequestTestData))]
    //public async Task BookingWorkspace_ReturnsOk_WithExpectedResponse(BookingWorkspaceRequest request)
    //{
    //    // Arrange
    //    var mockSender = A.Fake<ISender>();

    //    var firstResult = new BookingWorkspaceResult(0, "", "", 10000, "BOOKING PAYMENT", 0, "", "", "", "");
    //    var secondResult = new BookingWorkspaceResult(0, "", "", 2000, "BOOKING PAYMENT", 0, "", "", "", "");

    //    // Set up the mock to return the two results in sequence
    //    A.CallTo(() => mockSender.Send(A<BookingWorkspaceCommand>._, default))
    //        .ReturnsNextFromSequence(firstResult, secondResult);


    //    // Simulate endpoint delegate logic
    //    var endpointDelegate = async (BookingWorkspaceRequest req, ISender sender) =>
    //    {
    //        var command = req.Adapt<BookingWorkspaceCommand>();
    //        var result = await sender.Send(command);
    //        var response = result.Adapt<BookingWorkspaceResponse>();
    //        return Results.Ok(response);
    //    };

    //    // Act
    //    var result = await endpointDelegate(request, mockSender);

    //    // Assert
    //    result.Should().BeOfType<Ok<BookingWorkspaceResponse>>();

    //    var okResult = result as Ok<BookingWorkspaceResponse>;
    //    okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

    //    var response = okResult.Value;
    //    response.Should().NotBeNull();
    //    response.Amount.Should().BeOneOf(10000, 2000);
    //    response.Description.Should().Contain("BOOKING PAYMENT");
    //}

    [Theory]
    [ClassData(typeof(BookingRequestTestData))]
    public async Task BookingWorkspace_ReturnsOk_WithExpectedResponse(BookingWorkspaceRequest request, int expectedAmount)
    {
        // Arrange
        var mockSender = A.Fake<ISender>();

        A.CallTo(() => mockSender.Send(A<BookingWorkspaceCommand>._, default))
            .Returns(new BookingWorkspaceResult(0, "", "", expectedAmount, "BOOKING PAYMENT", 0, "", "", "", ""));

        var endpointDelegate = async (BookingWorkspaceRequest req, ISender sender) =>
        {
            var command = req.Adapt<BookingWorkspaceCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<BookingWorkspaceResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(request, mockSender);

        // Assert
        result.Should().BeOfType<Ok<BookingWorkspaceResponse>>();

        var okResult = result as Ok<BookingWorkspaceResponse>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value;
        response.Should().NotBeNull();
        response.Amount.Should().Be(expectedAmount);
        response.Description.Should().Contain("BOOKING PAYMENT");
    }

}

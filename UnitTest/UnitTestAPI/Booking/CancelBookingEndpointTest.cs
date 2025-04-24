using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using WorkHive.APIs.Users.Booking.CancelBooking;
using WorkHive.Services.Users.BookingWorkspace.CancelBooking;
using Mapster;
using FluentAssertions;

namespace UnitTestAPI.Booking;

public class CancelBookingEndpointTest
{
    [Fact]
    public async Task CancelBooking_ReturnsOk_WhenSuccess()
    {
        // Arrange
        var request = new CancelBookingRequest(1);
        var mockSender = A.Fake<ISender>();

        A.CallTo(() => mockSender.Send(A<CancelBookingCommand>.That.Matches(c => c.BookingId == 1), A<CancellationToken>._))
            .Returns(new CancelBookingResult("Hủy booking thành công"));

        var endpoint = async (CancelBookingRequest req, ISender sender) =>
        {
            var command = req.Adapt<CancelBookingCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CancelBookingResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpoint(request, mockSender);

        // Assert
        result.Should().BeOfType<Ok<CancelBookingResponse>>();

        var okResult = result as Ok<CancelBookingResponse>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult!.Value!.Notification.Should().Be("Hủy booking thành công");
    }

    [Fact]
    public async Task CancelBooking_ReturnsOk_WhenBookingNotFoundOrInvalidStatus()
    {
        // Arrange
        var request = new CancelBookingRequest(99); // giả định booking không tồn tại
        var mockSender = A.Fake<ISender>();

        A.CallTo(() => mockSender.Send(A<CancelBookingCommand>.That.Matches(c => c.BookingId == 99), A<CancellationToken>._))
            .Returns(new CancelBookingResult("Không tìm thấy booking hợp lệ để hủy hoặc trạng thái chưa thành công để hủy"));

        var endpoint = async (CancelBookingRequest req, ISender sender) =>
        {
            var command = req.Adapt<CancelBookingCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CancelBookingResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpoint(request, mockSender);

        // Assert
        var okResult = result as Ok<CancelBookingResponse>;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult!.Value!.Notification.Should().Contain("Không tìm thấy booking hợp lệ");
    }

    [Fact]
    public async Task CancelBooking_ReturnsOk_WhenPast8HoursDeadline()
    {
        // Arrange
        var request = new CancelBookingRequest(2); // giả định booking quá hạn hủy
        var mockSender = A.Fake<ISender>();

        A.CallTo(() => mockSender.Send(A<CancelBookingCommand>.That.Matches(c => c.BookingId == 2), A<CancellationToken>._))
            .Returns(new CancelBookingResult("Đã quá thời hạn 8 tiếng để hủy booking"));

        var endpoint = async (CancelBookingRequest req, ISender sender) =>
        {
            var command = req.Adapt<CancelBookingCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CancelBookingResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpoint(request, mockSender);

        // Assert
        var okResult = result as Ok<CancelBookingResponse>;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult!.Value!.Notification.Should().Contain("Đã quá thời hạn 8 tiếng");
    }
}

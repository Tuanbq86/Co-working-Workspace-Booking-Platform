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
using System.Collections;

namespace UnitTestAPI.Booking;

public class CancelBookingRequestTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // Test case 1: Hủy thành công
        yield return new object[]
        {
            new CancelBookingRequest(1),
            "Hủy booking thành công",
            0
        };

        // Test case 2: Booking không hợp lệ
        yield return new object[]
        {
            new CancelBookingRequest(2),
            "Không tìm thấy booking hợp lệ để hủy hoặc trạng thái chưa thành công để hủy",
            0
        };

        // Test case 3: Ví bị khóa
        yield return new object[]
        {
            new CancelBookingRequest(3),
            "Ví đã bị khóa để thực hiện yêu cầu rút tiền",
            1
        };

        // Test case 4: Quá hạn hủy 8 tiếng
        yield return new object[]
        {
            new CancelBookingRequest(4),
            "Đã quá thời hạn 8 tiếng để hủy booking tính từ startDate của đơn booking: 4",
            0
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}


public class CancelBookingEndpointTest
{
    [Theory]
    [ClassData(typeof(CancelBookingRequestTestData))]
    public async Task CancelBookingEndpoint_ReturnsOk(
        CancelBookingRequest request,
        string expectedNotification,
        int expectedIsLock)
    {
        // Arrange
        var mockSender = A.Fake<ISender>();

        var result = new CancelBookingResult(
            Notificationn: expectedNotification,
            IsLock: expectedIsLock
        );

        A.CallTo(() => mockSender.Send(A<CancelBookingCommand>._, default))
            .Returns(result);

        var endpointDelegate = async (CancelBookingRequest req, ISender sender) =>
        {
            var command = req.Adapt<CancelBookingCommand>();
            var res = await sender.Send(command);
            var response = new CancelBookingResponse(res.Notificationn, res.IsLock);
            return Results.Ok(response);
        };

        // Act
        var actualResult = await endpointDelegate(request, mockSender);

        // Assert
        actualResult.Should().BeOfType<Ok<CancelBookingResponse>>();

        var okResult = actualResult as Ok<CancelBookingResponse>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value;
        response.Should().NotBeNull();
        response.Notification.Should().Be(expectedNotification);
        response.IsLock.Should().Be(expectedIsLock);
    }
}

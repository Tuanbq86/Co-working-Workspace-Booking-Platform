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
using WorkHive.Services.WorkspaceTimes;
using Mapster;
using FluentAssertions;

namespace UnitTestAPI.Users;

public class CheckTimesRequestTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // Trường hợp workspace không phải 24h, đặt nguyên ngày hợp lệ
        yield return new object[]
        {
            new CheckTimesRequest(1, "08:00 01/05/2025", "17:00 03/05/2025"),
            "Khoảng thời gian phù hợp"
        };

        // Trường hợp workspace không phải 24h, đặt giờ lẻ ngoài khung giờ mở cửa
        yield return new object[]
        {
            new CheckTimesRequest(1, "07:00 01/05/2025", "18:00 01/05/2025"),
            "Thời gian đặt phải trong cùng một ngày và trong giờ mở cửa của workspace"
        };

        // Trường hợp workspace là 24h, thời gian bị trùng
        yield return new object[]
        {
            new CheckTimesRequest(2, "10:00 01/05/2025", "12:00 01/05/2025"),
            "Khoảng thời gian đã được sử dụng"
        };

        // Trường hợp workspace là 24h, thời gian không trùng
        yield return new object[]
        {
            new CheckTimesRequest(2, "15:00 02/05/2025", "17:00 02/05/2025"),
            "Khoảng thời gian phù hợp"
        };

        // Trường hợp không hợp lệ (không phải 24h và không thỏa điều kiện nào)
        yield return new object[]
        {
            new CheckTimesRequest(3, "08:00 01/05/2025", "08:00 02/05/2025"),
            "Yêu cầu không phù hợp"
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}


public class CheckOverlapTimeEndpointTest
{
    [Theory]
    [ClassData(typeof(CheckTimesRequestTestData))]
    public async Task CheckOverlapTimeEndpoint_ReturnsExpectedNotification(
    CheckTimesRequest request,
    string expectedNotification)
    {
        // Arrange
        var mockSender = A.Fake<ISender>();

        var result = new CheckTimesResult(expectedNotification);

        A.CallTo(() => mockSender.Send(A<CheckTimesCommand>._, default))
            .Returns(result);

        var endpointDelegate = async (CheckTimesRequest req, ISender sender) =>
        {
            var command = req.Adapt<CheckTimesCommand>();
            var res = await sender.Send(command);
            var response = new CheckTimesResponse(res.Notification);
            return Results.Ok(response.Notification);
        };

        // Act
        var actualResult = await endpointDelegate(request, mockSender);

        // Assert
        actualResult.Should().BeOfType<Ok<string>>();

        var okResult = actualResult as Ok<string>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().Be(expectedNotification);
    }
}

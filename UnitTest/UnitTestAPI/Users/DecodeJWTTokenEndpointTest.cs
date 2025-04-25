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
using Mapster;
using FluentAssertions;

namespace UnitTestAPI.Users;

public class DecodeJwtRequestTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new DecodeJwtRequest("valid.jwt.token"),
            new Dictionary<string, string>
            {
                { "sub", "1" },
                { "email", "quochuyho220603@gmail.com" },
                { "role", "Customer" }
            },
            "https://example.com/avatar.png"
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}


public class DecodeJWTTokenEndpointTest
{
    [Theory]
    [ClassData(typeof(DecodeJwtRequestTestData))]
    public async Task DecodeJwtTokenEndpoint_ReturnsExpectedResult(
    DecodeJwtRequest request,
    Dictionary<string, string> expectedClaims,
    string expectedAvatarUrl)
    {
        // Arrange
        var mockSender = A.Fake<ISender>();

        var result = new DecodeJwtResult(expectedClaims, expectedAvatarUrl);

        A.CallTo(() => mockSender.Send(A<DecodeJwtCommand>._, default))
            .Returns(result);

        var endpointDelegate = async (DecodeJwtRequest req, ISender sender) =>
        {
            var command = req.Adapt<DecodeJwtCommand>();
            var res = await sender.Send(command);
            var response = new DecodeJwtResponse(res.Claims, res.AvatarUrl);
            return Results.Ok(response);
        };

        // Act
        var actualResult = await endpointDelegate(request, mockSender);

        // Assert
        actualResult.Should().BeOfType<Ok<DecodeJwtResponse>>();

        var okResult = actualResult as Ok<DecodeJwtResponse>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value;
        response.Should().NotBeNull();
        response.AvatarUrl.Should().Be(expectedAvatarUrl);
        response.Claims.Should().BeEquivalentTo(expectedClaims);
    }
}

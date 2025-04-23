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
using WorkHive.APIs.Admins.BanOwner;
using WorkHive.Services.Admins.BanOwner;
using Mapster;
using FluentAssertions;

namespace UnitTestAPI.Admins.BanOwner;

public class UnBanOwnerTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // Không tìm thấy tài khoản hoặc Role sai
        yield return new object[]
        {
            new UnBanOwnerCommand(OwnerId: 99),
            new UnBanOwnerResult("Không tìm thấy tài khoản owner phù hợp", 0)
        };

        // Tìm thấy tài khoản, cập nhật thành công
        yield return new object[]
        {
            new UnBanOwnerCommand(OwnerId: 7),
            new UnBanOwnerResult("Cập nhật trạng thái tài khoản thành công", 1)
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class UnBanOwnerEndpointTest
{
    [Theory]
    [ClassData(typeof(UnBanOwnerTestData))]
    public async Task UnBanOwner_ReturnsExpectedResult(UnBanOwnerCommand command, UnBanOwnerResult expectedResult)
    {
        // Arrange
        var mockSender = A.Fake<ISender>();

        A.CallTo(() => mockSender.Send(A<UnBanOwnerCommand>.That.Matches(c => c.OwnerId == command.OwnerId), default))
            .Returns(expectedResult);

        var endpointDelegate = async (int ownerId, ISender sender) =>
        {
            var result = await sender.Send(new UnBanOwnerCommand(ownerId));
            var response = result.Adapt<UnBanOwnerResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(command.OwnerId, mockSender);

        // Assert
        result.Should().BeOfType<Ok<UnBanOwnerResponse>>();
        var okResult = result as Ok<UnBanOwnerResponse>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value;
        response!.Notification.Should().Be(expectedResult.Notification);
        response.IsBan.Should().Be(expectedResult.IsBan);
    }
}

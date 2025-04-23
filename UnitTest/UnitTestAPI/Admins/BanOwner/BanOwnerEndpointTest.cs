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
using UnitTestAPI.Admins.BanCustomer;
using WorkHive.APIs.Admins.BanCustomer;
using WorkHive.Services.Admins.BanCustomer;
using WorkHive.Services.Admins.BanOwner;
using Mapster;
using WorkHive.APIs.Admins.BanOwner;
using FluentAssertions;

namespace UnitTestAPI.Admins.BanOwner;

public class BanOwnerTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // Không tìm thấy tài khoản hoặc Role sai
        yield return new object[]
        {
            new BanOwnerCommand(OwnerId: 99),
            new BanOwnerResult("Không tìm thấy tài khoản owner phù hợp", 0)
        };

        // Tìm thấy tài khoản, cập nhật thành công
        yield return new object[]
        {
            new BanOwnerCommand(OwnerId: 3),
            new BanOwnerResult("Cập nhật trạng thái tài khoản thành công", 1)
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class BanOwnerEndpointTest
{
    [Theory]
    [ClassData(typeof(BanOwnerTestData))]
    public async Task BanOwner_ReturnsExpectedResult(BanOwnerCommand command, BanOwnerResult expectedResult)
    {
        // Arrange
        var mockSender = A.Fake<ISender>();

        A.CallTo(() => mockSender.Send(A<BanOwnerCommand>.That.Matches(c => c.OwnerId == command.OwnerId), default))
            .Returns(expectedResult);

        var endpointDelegate = async (int ownerId, ISender sender) =>
        {
            var result = await sender.Send(new BanOwnerCommand(ownerId));
            var response = result.Adapt<BanOwnerResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(command.OwnerId, mockSender);

        // Assert
        result.Should().BeOfType<Ok<BanOwnerResponse>>();
        var okResult = result as Ok<BanOwnerResponse>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value;
        response!.Notification.Should().Be(expectedResult.Notification);
        response.IsBan.Should().Be(expectedResult.IsBan);
    }
}

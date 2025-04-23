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
using WorkHive.APIs.Admins.BanCustomer;
using WorkHive.Services.Admins.BanCustomer;
using FluentAssertions;
using Mapster;

namespace UnitTestAPI.Admins.BanCustomer;

public class UnBanCustomerTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // Không tìm thấy tài khoản hoặc Role sai
        yield return new object[]
        {
            new UnBanCustomerCommand(CustomerId: 99),
            new UnBanCusomerResult("Không tìm thấy tài khoản khách hàng phù hợp", 0)
        };

        // Tìm thấy tài khoản, cập nhật thành công
        yield return new object[]
        {
            new UnBanCustomerCommand(CustomerId: 18),
            new UnBanCusomerResult("Cập nhật trạng thái tài khoản thành công", 0)
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class UnBanCustomerEndpointTest
{
    [Theory]
    [ClassData(typeof(UnBanCustomerTestData))]
    public async Task UnBanCustomer_ReturnsExpectedResult(UnBanCustomerCommand command, UnBanCusomerResult expectedResult)
    {
        // Arrange
        var mockSender = A.Fake<ISender>();

        A.CallTo(() => mockSender.Send(A<UnBanCustomerCommand>.That.Matches(c => c.CustomerId == command.CustomerId), default))
            .Returns(expectedResult);

        var endpointDelegate = async (int customerId, ISender sender) =>
        {
            var result = await sender.Send(new UnBanCustomerCommand(customerId));
            var response = result.Adapt<UnBanCusomerResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(command.CustomerId, mockSender);

        // Assert
        result.Should().BeOfType<Ok<UnBanCusomerResponse>>();
        var okResult = result as Ok<UnBanCusomerResponse>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value;
        response!.Notification.Should().Be(expectedResult.Notification);
        response.IsBan.Should().Be(expectedResult.IsBan);
    }
}

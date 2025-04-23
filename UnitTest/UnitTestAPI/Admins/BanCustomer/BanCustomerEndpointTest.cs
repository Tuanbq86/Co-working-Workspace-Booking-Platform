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
using WorkHive.APIs.Users.Booking.BookingByUserWallet;
using WorkHive.Services.Admins.BanCustomer;
using WorkHive.Services.Users.DTOs;
using Mapster;
using FluentAssertions;

namespace UnitTestAPI.Admins.BanCustomer;

public class BanCustomerTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // Không tìm thấy tài khoản hoặc Role sai
        yield return new object[]
        {
            new BanCustomerCommand(CustomerId: 99),
            new BanCusomerResult("Không tìm thấy tài khoản khách hàng phù hợp", 0)
        };

        // Tìm thấy tài khoản, cập nhật thành công
        yield return new object[]
        {
            new BanCustomerCommand(CustomerId: 3),
            new BanCusomerResult("Cập nhật trạng thái tài khoản thành công", 1)
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class BanCustomerEndpointTest
{
    [Theory]
    [ClassData(typeof(BanCustomerTestData))]
    public async Task BanCustomer_ReturnsExpectedResult(BanCustomerCommand command, BanCusomerResult expectedResult)
    {
        // Arrange
        var mockSender = A.Fake<ISender>();

        A.CallTo(() => mockSender.Send(A<BanCustomerCommand>.That.Matches(c => c.CustomerId == command.CustomerId), default))
            .Returns(expectedResult);

        var endpointDelegate = async (int customerId, ISender sender) =>
        {
            var result = await sender.Send(new BanCustomerCommand(customerId));
            var response = result.Adapt<BanCusomerResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(command.CustomerId, mockSender);

        // Assert
        result.Should().BeOfType<Ok<BanCusomerResponse>>();
        var okResult = result as Ok<BanCusomerResponse>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value;
        response!.Notification.Should().Be(expectedResult.Notification);
        response.IsBan.Should().Be(expectedResult.IsBan);
    }
}

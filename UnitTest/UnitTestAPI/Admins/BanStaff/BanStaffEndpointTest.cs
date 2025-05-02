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
using WorkHive.Services.Admins.BanStaff;
using FluentAssertions;
using Mapster;
using WorkHive.APIs.Admins.BanStaff;

namespace UnitTestAPI.Admins.BanStaff;

public class BanStaffTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // Không tìm thấy tài khoản hoặc Role sai
        yield return new object[]
        {
            new BanStaffCommand(StaffId: 2),
            new BanStaffResult("Không tìm thấy tài khoản staff phù hợp", 0)
        };

        // Tìm thấy tài khoản, cập nhật thành công
        yield return new object[]
        {
            new BanStaffCommand(StaffId: 5),
            new BanStaffResult("Cập nhật trạng thái tài khoản thành công", 1)
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class BanStaffEndpointTest
{
    [Theory]
    [ClassData(typeof(BanStaffTestData))]
    public async Task BanStaff_ReturnsExpectedResult(BanStaffCommand command, BanStaffResult expectedResult)
    {
        // Arrange
        var mockSender = A.Fake<ISender>();

        A.CallTo(() => mockSender.Send(A<BanStaffCommand>.That.Matches(c => c.StaffId == command.StaffId), default))
            .Returns(expectedResult);

        var endpointDelegate = async (int staffId, ISender sender) =>
        {
            var result = await sender.Send(new BanStaffCommand(staffId));
            var response = result.Adapt<BanStaffResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(command.StaffId, mockSender);

        // Assert
        result.Should().BeOfType<Ok<BanStaffResponse>>();
        var okResult = result as Ok<BanStaffResponse>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value;
        response!.Notification.Should().Be(expectedResult.Notification);
        response.IsBan.Should().Be(expectedResult.IsBan);
    }
}

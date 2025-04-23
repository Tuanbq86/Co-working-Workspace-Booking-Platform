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
using WorkHive.APIs.Admins.BanStaff;
using WorkHive.Services.Admins.BanStaff;
using Mapster;
using FluentAssertions;

namespace UnitTestAPI.Admins.BanStaff;

public class UnBanStaffTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // Không tìm thấy tài khoản hoặc Role sai
        yield return new object[]
        {
            new UnBanStaffCommand(StaffId: 2),
            new UnBanStaffResult("Không tìm thấy tài khoản staff phù hợp", 0)
        };

        // Tìm thấy tài khoản, cập nhật thành công
        yield return new object[]
        {
            new UnBanStaffCommand(StaffId: 5),
            new UnBanStaffResult("Cập nhật trạng thái tài khoản thành công", 1)
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class UnBanStaffEndpointTest
{
    [Theory]
    [ClassData(typeof(UnBanStaffTestData))]
    public async Task UnBanStaff_ReturnsExpectedResult(UnBanStaffCommand command, UnBanStaffResult expectedResult)
    {
        // Arrange
        var mockSender = A.Fake<ISender>();

        A.CallTo(() => mockSender.Send(A<UnBanStaffCommand>.That.Matches(c => c.StaffId == command.StaffId), default))
            .Returns(expectedResult);

        var endpointDelegate = async (int staffId, ISender sender) =>
        {
            var result = await sender.Send(new UnBanStaffCommand(staffId));
            var response = result.Adapt<UnBanStaffResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(command.StaffId, mockSender);

        // Assert
        result.Should().BeOfType<Ok<UnBanStaffResponse>>();
        var okResult = result as Ok<UnBanStaffResponse>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value;
        response!.Notification.Should().Be(expectedResult.Notification);
        response.IsBan.Should().Be(expectedResult.IsBan);
    }
}

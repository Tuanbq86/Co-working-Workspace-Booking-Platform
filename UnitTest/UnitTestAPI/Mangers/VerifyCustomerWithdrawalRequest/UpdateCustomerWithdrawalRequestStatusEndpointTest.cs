using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using WorkHive.APIs.Managers.VerifyCustomerWithdrawalRequest;
using WorkHive.Services.Managers.VerifyCustomerWithdrawalRequest;

namespace UnitTestAPI.Mangers.VerifyCustomerWithdrawalRequest;

public class UpdateCustomerWithdrawalRequestStatusEndpointTest
{
    [Fact]
    public async Task UpdateWithdrawalRequest_NotFound_ReturnsExpectedMessage()
    {
        // Arrange
        var mockSender = A.Fake<ISender>();
        var request = new UpdateCustomerWithdrawalRequestStatusRequest(99, 4, "Không thấy yêu cầu", "Fail");

        A.CallTo(() => mockSender.Send(
            A<UpdateCustomerWithdrawalRequestStatusCommand>.That.Matches(x => x.CustomerWithdrawalRequestId == request.CustomerWithdrawalRequestId),
            A<CancellationToken>.Ignored))
        .Returns(new UpdateCustomerWithdrawalRequestStatusResult("Không tìm thấy yêu cầu rút tiền nào của người dùng"));

        var endpointDelegate = async (UpdateCustomerWithdrawalRequestStatusRequest req, ISender sender) =>
        {
            var command = req.Adapt<UpdateCustomerWithdrawalRequestStatusCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<UpdateCustomerWithdrawalRequestStatusResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(request, mockSender);

        // Assert
        result.Should().BeOfType<Ok<UpdateCustomerWithdrawalRequestStatusResponse>>();
        var okResult = result as Ok<UpdateCustomerWithdrawalRequestStatusResponse>;
        okResult!.Value!.Notification.Should().Be("Không tìm thấy yêu cầu rút tiền nào của người dùng");
    }

    [Fact]
    public async Task UpdateWithdrawalRequest_Success_ReturnsExpectedMessage()
    {
        // Arrange
        var mockSender = A.Fake<ISender>();
        var request = new UpdateCustomerWithdrawalRequestStatusRequest(29, 4, "Xác nhận ok", "Success");

        A.CallTo(() => mockSender.Send(
            A<UpdateCustomerWithdrawalRequestStatusCommand>.That.Matches(x => x.CustomerWithdrawalRequestId == request.CustomerWithdrawalRequestId),
            A<CancellationToken>.Ignored))
        .Returns(new UpdateCustomerWithdrawalRequestStatusResult("Yêu cầu rút tiền đã được cập nhật thành 'Success'"));

        var endpointDelegate = async (UpdateCustomerWithdrawalRequestStatusRequest req, ISender sender) =>
        {
            var command = req.Adapt<UpdateCustomerWithdrawalRequestStatusCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<UpdateCustomerWithdrawalRequestStatusResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(request, mockSender);

        // Assert
        result.Should().BeOfType<Ok<UpdateCustomerWithdrawalRequestStatusResponse>>();
        var okResult = result as Ok<UpdateCustomerWithdrawalRequestStatusResponse>;
        okResult!.Value!.Notification.Should().Be("Yêu cầu rút tiền đã được cập nhật thành 'Success'");
    }
}

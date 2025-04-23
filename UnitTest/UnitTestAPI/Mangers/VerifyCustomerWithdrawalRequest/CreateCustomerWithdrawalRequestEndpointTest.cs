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

public class CreateCustomerWithdrawalRequestEndpointTest
{
    [Fact]
    public async Task CreateWithdrawalRequest_CustomerWalletNotFound_ReturnsNotFoundMessage()
    {
        // Arrange
        var mockSender = A.Fake<ISender>();
        var request = new CreateCustomerWithdrawalRequestRequest(
            Title: "Rút tiền",
            Description: "Tôi muốn rút toàn bộ số dư",
            CustomerId: 99
        );

        A.CallTo(() => mockSender.Send(
            A<CreateCustomerWithdrawalRequestCommand>.That.Matches(c => c.CustomerId == request.CustomerId),
            A<CancellationToken>.Ignored))
        .Returns(new CreateCustomerWithdrawalRequestResult("Không tìm thấy ví người dùng để lấy thông tin tạo yêu cầu", 0));

        var endpointDelegate = async (CreateCustomerWithdrawalRequestRequest req, ISender sender) =>
        {
            var command = req.Adapt<CreateCustomerWithdrawalRequestCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CreateCustomerWithdrawalRequestResponse>();
            return Results.Created("/customer-withdrawal-requests", response);
        };

        // Act
        var result = await endpointDelegate(request, mockSender);

        // Assert
        result.Should().BeOfType<Created<CreateCustomerWithdrawalRequestResponse>>();
        var createdResult = result as Created<CreateCustomerWithdrawalRequestResponse>;
        createdResult!.Value!.Notification.Should().Be("Không tìm thấy ví người dùng để lấy thông tin tạo yêu cầu");
        createdResult.Value.IsLock.Should().Be(0);
    }

    [Fact]
    public async Task CreateWithdrawalRequest_Success_ReturnsSuccessMessage()
    {
        // Arrange
        var mockSender = A.Fake<ISender>();
        var request = new CreateCustomerWithdrawalRequestRequest(
            Title: "Rút tiền",
            Description: "Rút 500k",
            CustomerId: 2
        );

        A.CallTo(() => mockSender.Send(
            A<CreateCustomerWithdrawalRequestCommand>.That.Matches(c => c.CustomerId == request.CustomerId),
            A<CancellationToken>.Ignored))
        .Returns(new CreateCustomerWithdrawalRequestResult("Yêu cầu rút tiền của người dùng đã được tạo thành công", 1));

        var endpointDelegate = async (CreateCustomerWithdrawalRequestRequest req, ISender sender) =>
        {
            var command = req.Adapt<CreateCustomerWithdrawalRequestCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CreateCustomerWithdrawalRequestResponse>();
            return Results.Created("/customer-withdrawal-requests", response);
        };

        // Act
        var result = await endpointDelegate(request, mockSender);

        // Assert
        result.Should().BeOfType<Created<CreateCustomerWithdrawalRequestResponse>>();
        var createdResult = result as Created<CreateCustomerWithdrawalRequestResponse>;
        createdResult!.Value!.Notification.Should().Be("Yêu cầu rút tiền của người dùng đã được tạo thành công");
        createdResult.Value.IsLock.Should().Be(1);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using WorkHive.APIs.Managers.VerifyCustomerWithdrawalRequest;
using WorkHive.Services.Managers.VerifyCustomerWithdrawalRequest;

namespace UnitTestAPI.Mangers.VerifyCustomerWithdrawalRequest;

public class GetCustomerWithdrawalRequestByIdEndpointTest
{
    [Fact]
    public async Task GetCustomerWithdrawalRequestById_ValidId_ReturnsCorrectData()
    {
        // Arrange
        var mockSender = A.Fake<ISender>();
        int requestId = 100;

        var expectedDto = new CustomerWithdrawalRequestByIdDTO(
            Id: 100,
            Title: "Rút tiền",
            Description: "Muốn rút tiền từ ví",
            Status: "Processing",
            CreatedAt: DateTime.Now,
            CustomerId: 10,
            ManagerId: null,
            BankName: "ACB",
            BankNumber: "123456789",
            BankAccountName: "Nguyen Van A",
            Balance: 500000,
            ManagerResponse: "N/A",
            UpdatedAt: null
        );

        A.CallTo(() => mockSender.Send(
            A<GetCustomerWithdrawalRequestByIdQuery>.That.Matches(q => q.CustomerRequestId == requestId),
            A<CancellationToken>._))
        .Returns(new GetCustomerWithdrawalRequestByIdResult(expectedDto));

        var endpointDelegate = async (int id, ISender sender) =>
        {
            var result = await sender.Send(new GetCustomerWithdrawalRequestByIdQuery(id));
            var response = new GetCustomerWithdrawalRequestByIdResponse(result.CustomerWithdrawalRequestByIdDTOs);
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(requestId, mockSender);

        // Assert
        result.Should().BeOfType<Ok<GetCustomerWithdrawalRequestByIdResponse>>();
        var okResult = result as Ok<GetCustomerWithdrawalRequestByIdResponse>;
        okResult!.Value!.CustomerWithdrawalRequestByIdDTOs.Id.Should().Be(requestId);
    }

    [Fact]
    public async Task GetCustomerWithdrawalRequestById_InvalidId_ReturnsDefaultDTO()
    {
        // Arrange
        var mockSender = A.Fake<ISender>();
        int requestId = 999;

        var defaultDto = new CustomerWithdrawalRequestByIdDTO(
            Id: 0,
            Title: "N/A",
            Description: "N/A",
            Status: "N/A",
            CreatedAt: null,
            CustomerId: 0,
            ManagerId: null,
            BankName: "N/A",
            BankNumber: "N/A",
            BankAccountName: "N/A",
            Balance: 0,
            ManagerResponse: "N/A",
            UpdatedAt: null
        );

        A.CallTo(() => mockSender.Send(
            A<GetCustomerWithdrawalRequestByIdQuery>.That.Matches(q => q.CustomerRequestId == requestId),
            A<CancellationToken>._))
        .Returns(new GetCustomerWithdrawalRequestByIdResult(defaultDto));

        var endpointDelegate = async (int id, ISender sender) =>
        {
            var result = await sender.Send(new GetCustomerWithdrawalRequestByIdQuery(id));
            var response = new GetCustomerWithdrawalRequestByIdResponse(result.CustomerWithdrawalRequestByIdDTOs);
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(requestId, mockSender);

        // Assert
        result.Should().BeOfType<Ok<GetCustomerWithdrawalRequestByIdResponse>>();
        var okResult = result as Ok<GetCustomerWithdrawalRequestByIdResponse>;
        okResult!.Value!.CustomerWithdrawalRequestByIdDTOs.Id.Should().Be(0);
        okResult.Value.CustomerWithdrawalRequestByIdDTOs.Title.Should().Be("N/A");
    }
}

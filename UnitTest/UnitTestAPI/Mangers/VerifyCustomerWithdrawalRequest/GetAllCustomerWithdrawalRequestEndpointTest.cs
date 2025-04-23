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

public class GetAllCustomerWithdrawalRequestEndpointTest
{
    [Fact]
    public async Task GetAllCustomerWithdrawalRequests_HasData_ReturnsListGreaterThanZero()
    {
        // Arrange
        var mockSender = A.Fake<ISender>();
        int customerId = 99;

        var mockData = new List<CustomerWithdrawalRequestDTO>
        {
            new CustomerWithdrawalRequestDTO(
                Id: 1,
                Title: "Rút tiền",
                Description: "Rút 1 triệu",
                Status: "Handling",
                CreatedAt: DateTime.Now,
                CustomerId: customerId,
                ManagerId: null,
                BankName: "VietinBank",
                BankNumber: "0123456789",
                BankAccountName: "Tran Van B",
                Balance: 1000000,
                ManagerResponse: "N/A",
                UpdatedAt: null
            )
        };

        A.CallTo(() => mockSender.Send(
                A<GetAllCustomerWithdrawalRequestByCustomerIdQuery>.That.Matches(q => q.CustomerId == customerId),
                A<CancellationToken>._))
            .Returns(new GetAllCustomerWithdrawalRequestByCustomerIdResult(mockData));

        var endpointDelegate = async (int id, ISender sender) =>
        {
            var query = new GetAllCustomerWithdrawalRequestByCustomerIdQuery(id);
            var result = await sender.Send(query);
            var response = new GetAllCustomerWithdrawalRequestByCustomerIdResponse(result.CustomerWithdrawalRequests);
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(customerId, mockSender);

        // Assert
        result.Should().BeOfType<Ok<GetAllCustomerWithdrawalRequestByCustomerIdResponse>>();
        var okResult = result as Ok<GetAllCustomerWithdrawalRequestByCustomerIdResponse>;
        okResult!.Value!.CustomerWithdrawalRequests.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAllCustomerWithdrawalRequests_NoData_ReturnsEmptyList()
    {
        // Arrange
        var mockSender = A.Fake<ISender>();
        int customerId = 1;

        A.CallTo(() => mockSender.Send(
                A<GetAllCustomerWithdrawalRequestByCustomerIdQuery>.That.Matches(q => q.CustomerId == customerId),
                A<CancellationToken>._))
            .Returns(new GetAllCustomerWithdrawalRequestByCustomerIdResult(new List<CustomerWithdrawalRequestDTO>()));

        var endpointDelegate = async (int id, ISender sender) =>
        {
            var query = new GetAllCustomerWithdrawalRequestByCustomerIdQuery(id);
            var result = await sender.Send(query);
            var response = new GetAllCustomerWithdrawalRequestByCustomerIdResponse(result.CustomerWithdrawalRequests);
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(customerId, mockSender);

        // Assert
        result.Should().BeOfType<Ok<GetAllCustomerWithdrawalRequestByCustomerIdResponse>>();
        var okResult = result as Ok<GetAllCustomerWithdrawalRequestByCustomerIdResponse>;
        okResult!.Value!.CustomerWithdrawalRequests.Should().BeEmpty();
    }
}

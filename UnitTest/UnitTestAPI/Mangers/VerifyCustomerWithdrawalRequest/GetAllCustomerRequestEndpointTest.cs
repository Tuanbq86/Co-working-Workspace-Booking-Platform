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

public class GetAllCustomerRequestEndpointTest
{
    [Fact]
    public async Task GetAllCustomerRequests_HasData_ReturnsListGreaterThanZero()
    {
        // Arrange
        var mockSender = A.Fake<ISender>();

        var mockData = new List<CustomerRequestDTO>
        {
            new CustomerRequestDTO(
                Id: 1,
                Title: "Rút tiền",
                Description: "Rút 100k",
                Status: "Handling",
                CreatedAt: DateTime.Now,
                CustomerId: 101,
                ManagerId: null,
                BankName: "Vietcombank",
                BankNumber: "123456789",
                BankAccountName: "Nguyen Van A",
                Balance: 100000,
                ManagerResponse: "N/A",
                UpdatedAt: null
            )
        };

        A.CallTo(() => mockSender.Send(A<GetAllCustomerRequestQuery>._, A<CancellationToken>._))
            .Returns(new GetAllCustomerRequestResult(mockData));

        var endpointDelegate = async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllCustomerRequestQuery());
            var response = new GetAllCustomerRequestResponse(result.CustomerRequests);
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(mockSender);

        // Assert
        result.Should().BeOfType<Ok<GetAllCustomerRequestResponse>>();
        var okResult = result as Ok<GetAllCustomerRequestResponse>;
        okResult!.Value!.CustomerRequests.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAllCustomerRequests_NoData_ReturnsEmptyList()
    {
        // Arrange
        var mockSender = A.Fake<ISender>();

        A.CallTo(() => mockSender.Send(A<GetAllCustomerRequestQuery>._, A<CancellationToken>._))
            .Returns(new GetAllCustomerRequestResult(new List<CustomerRequestDTO>()));

        var endpointDelegate = async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllCustomerRequestQuery());
            var response = new GetAllCustomerRequestResponse(result.CustomerRequests);
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(mockSender);

        // Assert
        result.Should().BeOfType<Ok<GetAllCustomerRequestResponse>>();
        var okResult = result as Ok<GetAllCustomerRequestResponse>;
        okResult!.Value!.CustomerRequests.Should().BeEmpty();
    }
}

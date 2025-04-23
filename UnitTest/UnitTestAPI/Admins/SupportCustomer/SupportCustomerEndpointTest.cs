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
using WorkHive.APIs.Admins.SupportCustomer;
using WorkHive.Services.Admins.SupportCustomer;

namespace UnitTestAPI.Admins.SupportCustomer;

public class SupportCustomerEndpointTest
{
    [Fact]
    public async Task SupportCustomer_InvalidRequest_ReturnsValidationError()
    {
        // Arrange
        var mockSender = A.Fake<ISender>();
        var request = new SupportCustomerRequest(
            Name: "", // thiếu thông tin
            Email: "quochuyho220603@gmail.com",
            Phone: "0867435157",
            Message: ""
        );

        A.CallTo(() => mockSender.Send(
            A<SupportCustomerCommand>.That.Matches(c =>
                c.Email == request.Email &&
                c.Name == request.Name),
            A<CancellationToken>.Ignored))
        .Returns(new SupportCustomerResult("Vui lòng nhập đầy đủ thông tin"));

        var endpointDelegate = async (SupportCustomerRequest req, ISender sender) =>
        {
            var command = req.Adapt<SupportCustomerCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<SupportCustomerResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(request, mockSender);

        // Assert
        result.Should().BeOfType<Ok<SupportCustomerResponse>>();
        var okResult = result as Ok<SupportCustomerResponse>;
        okResult!.Value!.Notification.Should().Be("Vui lòng nhập đầy đủ thông tin");
    }

    [Fact]
    public async Task SupportCustomer_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var mockSender = A.Fake<ISender>();
        var request = new SupportCustomerRequest(
            Name: "Hồ Quốc Huy",
            Email: "quochuyho220603@gmail.com",
            Phone: "0867435157",
            Message: "Tôi cần được hỗ trợ"
        );

        A.CallTo(() => mockSender.Send(
            A<SupportCustomerCommand>.That.Matches(c =>
                c.Email == request.Email &&
                c.Name == request.Name),
            A<CancellationToken>.Ignored))
        .Returns(new SupportCustomerResult("Gửi hỗ trợ thành công, vui lòng kiểm tra email để xem thông tin chi tiết"));

        var endpointDelegate = async (SupportCustomerRequest req, ISender sender) =>
        {
            var command = req.Adapt<SupportCustomerCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<SupportCustomerResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(request, mockSender);

        // Assert
        result.Should().BeOfType<Ok<SupportCustomerResponse>>();
        var okResult = result as Ok<SupportCustomerResponse>;
        okResult!.Value!.Notification.Should().Be("Gửi hỗ trợ thành công, vui lòng kiểm tra email để xem thông tin chi tiết");
    }
}

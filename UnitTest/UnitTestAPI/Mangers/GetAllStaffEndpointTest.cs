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
using WorkHive.APIs.Managers;
using WorkHive.Services.DTOService;
using WorkHive.Services.Managers;

namespace UnitTestAPI.Mangers;

public class GetAllStaffEndpointTest
{
    [Fact]
    public async Task GetAllStaff_ShouldReturnStaffList_WhenStaffExist()
    {
        // Arrange
        var fakeSender = A.Fake<ISender>();

        var fakeStaffList = new List<UserDTOForManager>
            {
                new UserDTOForManager
                {
                    Id = 1,
                    Name = "Nguyen Van A",
                    Phone = "0909123456",
                    Email = "a@example.com",
                    Status = "Active",
                    Avatar = "avatar.png",
                    Location = "HCM",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
                    CreatedAt = DateTime.Now.AddMonths(-1),
                    UpdatedAt = DateTime.Now,
                    Sex = "Male",
                    RoleName = "Staff",
                    IsBan = 0
                }
            };

        A.CallTo(() => fakeSender.Send(A<GetAllStaffQuery>._, A<CancellationToken>._))
            .Returns(new GetAllStaffResult(fakeStaffList));

        var endpointDelegate = async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllStaffQuery());
            var response = result.Adapt<GetAllStaffResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(fakeSender);

        // Assert
        result.Should().BeOfType<Ok<GetAllStaffResponse>>();
        var okResult = result as Ok<GetAllStaffResponse>;
        okResult!.Value.Should().NotBeNull();
        okResult.Value!.Staffs.Should().NotBeNull();
        okResult.Value.Staffs.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetAllStaff_ShouldReturnEmptyList_WhenNoStaffExist()
    {
        // Arrange
        var fakeSender = A.Fake<ISender>();

        A.CallTo(() => fakeSender.Send(A<GetAllStaffQuery>._, A<CancellationToken>._))
            .Returns(new GetAllStaffResult(new List<UserDTOForManager>()));

        var endpointDelegate = async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllStaffQuery());
            var response = result.Adapt<GetAllStaffResponse>();
            return Results.Ok(response);
        };

        // Act
        var result = await endpointDelegate(fakeSender);

        // Assert
        result.Should().BeOfType<Ok<GetAllStaffResponse>>();
        var okResult = result as Ok<GetAllStaffResponse>;
        okResult!.Value!.Should().NotBeNull();
        okResult.Value!.Staffs.Should().BeEmpty();
    }
}

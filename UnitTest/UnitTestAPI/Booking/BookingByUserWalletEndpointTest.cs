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
using WorkHive.APIs.Users.Booking;
using WorkHive.APIs.Users.Booking.BookingByUserWallet;
using WorkHive.Services.Users.BookingWorkspace.BookingByUserWallet;
using WorkHive.Services.Users.DTOs;
using Mapster;
using FluentAssertions;

namespace UnitTestAPI.Booking;

public class BookingByUserWalletRequestTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // Test case 1 : Happy case
        yield return new object[]
        {
            new BookingByUserWalletRequest(
                2,
                1,
                "10:00 23/04/2025",
                "11:00 23/04/2025",
                new List<BookingAmenityDTO>
                {
                    new BookingAmenityDTO { Id = 1, Quantity = 2 },
                    new BookingAmenityDTO { Id = 2, Quantity = 1 }
                },
                new List<BookingBeverageDTO>
                {
                    new BookingBeverageDTO { Id = 1, Quantity = 2 },
                    new BookingBeverageDTO { Id = 2, Quantity = 1 }
                },
                "",
                10000,
                "Giờ"
            ),
            "Đặt chỗ thành công, vui lòng kiểm tra email để xem thông tin chi tiết",
            0
        };

        // Test case 2 : test with promotion code
        yield return new object[]
        {
            new BookingByUserWalletRequest(
                2,
                1,
                "12:00 23/04/2025",
                "13:00 23/04/2025",
                new List<BookingAmenityDTO>
                {
                    new BookingAmenityDTO { Id = 1, Quantity = 2 },
                    new BookingAmenityDTO { Id = 2, Quantity = 1 }
                },
                new List<BookingBeverageDTO>
                {
                    new BookingBeverageDTO { Id = 1, Quantity = 2 },
                    new BookingBeverageDTO { Id = 2, Quantity = 1 }
                },
                "DATNUOC",
                10000,
                "Giờ"
            ),
            "Đặt chỗ thành công, vui lòng kiểm tra email để xem thông tin chi tiết",
            0
        };

        // Test case 3 : test with empty of wallet
        yield return new object[]
        {
            new BookingByUserWalletRequest(
                3,
                1,
                "14:00 23/04/2025",
                "15:00 23/04/2025",
                new List<BookingAmenityDTO>
                {
                    new BookingAmenityDTO { Id = 1, Quantity = 2 },
                    new BookingAmenityDTO { Id = 2, Quantity = 1 }
                },
                new List<BookingBeverageDTO>
                {
                    new BookingBeverageDTO { Id = 1, Quantity = 2 },
                    new BookingBeverageDTO { Id = 2, Quantity = 1 }
                },
                "DATNUOC",
                10000,
                "Giờ"
            ),
            "Số dư trong ví không đủ để thực hiện booking",
            0
        };

        // Test case 4 : wallet is lock
        yield return new object[]
        {
            new BookingByUserWalletRequest(
                10,
                1,
                "15:00 23/04/2025",
                "16:00 23/04/2025",
                new List<BookingAmenityDTO>
                {
                    new BookingAmenityDTO { Id = 1, Quantity = 2 },
                    new BookingAmenityDTO { Id = 2, Quantity = 1 }
                },
                new List<BookingBeverageDTO>
                {
                    new BookingBeverageDTO { Id = 1, Quantity = 2 },
                    new BookingBeverageDTO { Id = 2, Quantity = 1 }
                },
                "DATNUOC",
                10000,
                "Giờ"
            ),
            "Ví của bạn đã bị khóa",
            1
        };
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class BookingByUserWalletEndpointTest
{
    [Theory]
    [ClassData(typeof(BookingByUserWalletRequestTestData))]
    public async Task BookingByWallet_ReturnsOk_WhenWalletInsufficient(
        BookingByUserWalletRequest request,
        string expectedMessage,
        int expectedIsLock)
    {
        // Arrange
        var mockSender = A.Fake<ISender>();

        var result = new BookingByUserWalletResult(
            Notification: expectedMessage,
            IsLock: expectedIsLock
        );

        A.CallTo(() => mockSender.Send(A<BookingByUserWalletCommand>._, default))
            .Returns(result);

        var endpointDelegate = async (BookingByUserWalletRequest req, ISender sender) =>
        {
            var command = req.Adapt<BookingByUserWalletCommand>();
            var res = await sender.Send(command);
            var response = res.Adapt<BookingByUserWalletResponse>();
            return Results.Ok(response);
        };

        // Act
        var actualResult = await endpointDelegate(request, mockSender);

        // Assert
        actualResult.Should().BeOfType<Ok<BookingByUserWalletResponse>>();

        var okResult = actualResult as Ok<BookingByUserWalletResponse>;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value;
        response.Should().NotBeNull();
        response.Notification.Should().Be(expectedMessage);
        response.IsLock.Should().Be(expectedIsLock);
    }
}

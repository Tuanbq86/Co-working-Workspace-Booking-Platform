using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using MediatR;
using WorkHive.APIs.Users.Booking;

namespace UnitTestAPI.Booking;

public class BookingPayOsEndpointTest
{
    private readonly ISender sender;
    private readonly BookingWorkspaceRequest request;
    private readonly BookingWorkspaceEndpoint endpoint;

    public BookingPayOsEndpointTest()
    {
        //Dependencies
        this.sender = A.Fake<ISender>();
        this.request = A.Fake<BookingWorkspaceRequest>();
        //SUT
        //this.endpoint = new BookingWorkspaceEndpoint(request, sender);
    }

}

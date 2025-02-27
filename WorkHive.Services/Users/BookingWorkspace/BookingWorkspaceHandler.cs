using FluentValidation;
using Microsoft.AspNetCore.Http;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.Repositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant;
using WorkHive.Services.Exceptions;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Services.Users.BookingWorkspace;

public record BookingWorkspaceCommand(int WorkspaceId, DateTime startDate, DateTime endDate,
    List<BookingAmenity> Amenities, List<BookingBeverage> Beverages, string promotionCode)
    : ICommand<BookingWorkspaceResult>;
public record BookingWorkspaceResult(string Notification);

public class BookingWorkspaceValidator : AbstractValidator<BookingWorkspaceCommand>
{
    public BookingWorkspaceValidator()
    {
        
    }
}
public class BookingWorkspaceHandler(IHttpContextAccessor httpContext, ITokenRepository tokenRepo,
    IBookingWorkspaceUnitOfWork bookingUnitOfWork)
    : ICommandHandler<BookingWorkspaceCommand, BookingWorkspaceResult>
{
    public async Task<BookingWorkspaceResult> Handle(BookingWorkspaceCommand command, 
        CancellationToken cancellationToken)
    {
        var newBooking = new Booking();

        //Get userId and roleId for Booking in session containing token in a session working
        var token = httpContext.HttpContext!.Session.GetString("token")!.ToString();
        var listInfo = tokenRepo.DecodeJwtToken(token);

        var userId = listInfo[0];
        var roleId = listInfo[1];

        newBooking.UserId = Convert.ToInt32(userId);

        //if workspace available
        if (bookingUnitOfWork.workspace.GetById(command.WorkspaceId).Status.Equals(WorkspaceStatus.Available))
        {
            if(command.startDate < DateTime.UtcNow)
            {
                throw new BookingBadRequestException("start date have to equal present moment");
            }

            newBooking.StartDate = command.startDate;

            if (command.endDate <= command.startDate)
                throw new BookingBadRequestException("end date have to greater than start date");

            newBooking.EndDate = command.endDate;

            newBooking.WorkspaceId = command.WorkspaceId;
        }
        else
        {
            //check time for booking
            var bookings = await bookingUnitOfWork.booking.GetAllAsync();

            var booking = new Booking();
            foreach (var item in bookings)
            {
                if (item.WorkspaceId == command.WorkspaceId && item.Status.Equals(WorkspaceStatus.InUse))
                    booking = item;
            }

            if (command.startDate < booking.EndDate.GetValueOrDefault())
            {
                throw new BookingBadRequestException("Start date have to greater than or equal to last booking");
            }

            if (bookingUnitOfWork.workspace.GetById(command.WorkspaceId)
                .Status.Equals(WorkspaceStatus.InUse) && command.startDate == booking.EndDate)
            {
                newBooking.StartDate = booking.EndDate.GetValueOrDefault()
                          .AddMinutes(bookingUnitOfWork.workspace.GetById(command.WorkspaceId).
                          CleanTime.GetValueOrDefault());

                newBooking.EndDate = command.endDate;
            }

            if (bookingUnitOfWork.workspace.GetById(command.WorkspaceId).Status.Equals(WorkspaceStatus.InUse)
                && command.startDate > booking.EndDate.GetValueOrDefault().
                AddMinutes(bookingUnitOfWork.workspace.GetById(command.WorkspaceId).
                CleanTime.GetValueOrDefault()))
            {
                newBooking.StartDate = command.startDate;

                newBooking.EndDate = command.endDate;
            }

            newBooking.WorkspaceId = command.WorkspaceId;
        }

        //Add Amenity and Beverage for Booking
        foreach(var item in command.Amenities)
        {
            item.BookingId = newBooking.Id;
        }

        foreach (var item in command.Beverages)
        {
            item.BookingWorkspaceId = newBooking.Id;
        }

        //Add promotion for booking

        var codeDiscount = bookingUnitOfWork.promotion.GetAll().
            Where(p => p.Code.Equals(command.promotionCode)).FirstOrDefault();

        if (codeDiscount is null)
            throw new PromotionNotFoundException("Invalid promotion code");

        if (codeDiscount.Status.Equals(PromotionStatus.Expired))
        {
            throw new PromotionNotFoundException("Expired code");
        }

        newBooking.PromotionId = codeDiscount.Id;



        throw new NotImplementedException();
    }
}

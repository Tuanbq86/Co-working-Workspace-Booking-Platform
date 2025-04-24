using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.DTOService;

namespace WorkHive.Services.Users.BookingWorkspace.GetAllBookingByOwnerId;

public record GetAllBookingByOwnerIdQuery(int OwnerId) : IQuery<GetAllBookingByOwnerIdResult>;
public record GetAllBookingByOwnerIdResult(List<BookingByOwnerIdDTO> BookingByOwnerIdDTOs);

public class GetAllBookingByOwnerIdHandler(IBookingWorkspaceUnitOfWork bookUnit)
    : IQueryHandler<GetAllBookingByOwnerIdQuery, GetAllBookingByOwnerIdResult>
{
    public async Task<GetAllBookingByOwnerIdResult> Handle(GetAllBookingByOwnerIdQuery query,
        CancellationToken cancellationToken)
    {
        var workspaces = bookUnit.workspace.GetAll()
            .Where(w => w.OwnerId.Equals(query.OwnerId)).ToList();

        var bookings = new List<Booking>();

        foreach(var item in workspaces)
        {
            var bookingsByWorkspaceId = bookUnit.booking.GetAll()
                .Where(b => b.WorkspaceId.Equals(item.Id)).ToList();

            foreach(var bookItem in bookingsByWorkspaceId)
            {
                bookings.Add(bookItem);
            }
        }

        List<BookingByOwnerIdDTO> result = new List<BookingByOwnerIdDTO>();

        foreach(var item in bookings)
        {
            BookingByOwnerIdDTO bookingByOwnerIdDTO = new BookingByOwnerIdDTO();

            bookingByOwnerIdDTO.BookingId = item.Id;
            bookingByOwnerIdDTO.Start_Date = item.StartDate;
            bookingByOwnerIdDTO.End_Date = item.EndDate;
            bookingByOwnerIdDTO.Price = item.Price;
            bookingByOwnerIdDTO.Status = item.Status;
            bookingByOwnerIdDTO.Created_At = item.CreatedAt;
            bookingByOwnerIdDTO.UserId = item.UserId;
            bookingByOwnerIdDTO.WorkspaceId = item.WorkspaceId;

            result.Add(bookingByOwnerIdDTO);
        }


        return new GetAllBookingByOwnerIdResult(result);
    }
}
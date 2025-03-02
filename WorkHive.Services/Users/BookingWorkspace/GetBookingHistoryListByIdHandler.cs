using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.BookingDTO;
using WorkHive.Services.Exceptions;

namespace WorkHive.Services.Users.BookingWorkspace;

public record GetBookingHistoryListByIdQuery(string Token) 
    : IQuery<GetBookingHistoryListByIdResult>;
public record GetBookingHistoryListByIdResult(List<BookingHistory> BookingHistories);


public class GetBookingHistoryListByIdHandler(IBookingWorkspaceUnitOfWork bookingUnit, ITokenRepository tokenRepo)
    : IQueryHandler<GetBookingHistoryListByIdQuery, GetBookingHistoryListByIdResult>
{
    public async Task<GetBookingHistoryListByIdResult> Handle(GetBookingHistoryListByIdQuery query, 
        CancellationToken cancellationToken)
    {
        var results = new List<BookingHistory>();

        var listInfo = tokenRepo.DecodeJwtToken(query.Token);

        var userId = listInfo[0];

        var bookings = bookingUnit.booking.GetAll()
            .Where(b => b.UserId == Convert.ToInt32(userId)).ToList();

        if (bookings.Count.Equals(0))
            throw new BookingNotFoundException("Can not find any booking");

        foreach(var item in bookings)
        {
            var history = new BookingHistory();

            history.StartDate = (DateTime)item.StartDate!;
            history.EndDate = (DateTime)item.EndDate!;
            history.CreatedAt = (DateTime)item.CreatedAt!;
            history.Status = item.Status;
            history.Price = (decimal)item.Price!;

            results.Add(history);
        }

        return new GetBookingHistoryListByIdResult(results);
    }
}

using FluentValidation;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;

namespace WorkHive.Services.Users.BookingWorkspace;

public record BookingWorkspaceCommand(int WorkspaceId, int UserId,
    List<BookingAmenity> Amenities, List<BookingBeverage> Beverages )
    : ICommand<BookingWorkspaceResult>;
public record BookingWorkspaceResult();

public class BookingWorkspaceValidator : AbstractValidator<BookingWorkspaceCommand>
{
    public BookingWorkspaceValidator()
    {
        
    }
}
public class BookingWorkspaceHandler()
    : ICommandHandler<BookingWorkspaceCommand, BookingWorkspaceResult>
{
    public Task<BookingWorkspaceResult> Handle(BookingWorkspaceCommand command, 
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

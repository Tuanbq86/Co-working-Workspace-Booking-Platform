using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.DTOService;

namespace WorkHive.Services.Users.NumberOfBooking;

public record NumberBookingOfAmenitiesQuery(int OwnerId)
    : IQuery<NumberBookingOfAmenitiesResult>;
public record NumberBookingOfAmenitiesResult(List<NumberOfBookingAmenitiesDTO> NumberOfBookingAmenitiesDTOs);

public class NumberBookingOfAmenitiesHandler(IBookingWorkspaceUnitOfWork bookingUnit)
    : IQueryHandler<NumberBookingOfAmenitiesQuery, NumberBookingOfAmenitiesResult>
{
    public async Task<NumberBookingOfAmenitiesResult> Handle(NumberBookingOfAmenitiesQuery query, 
        CancellationToken cancellationToken)
    {
        var amenities = await bookingUnit.amenity.GetNumberOfBookingAmenity(query.OwnerId);

        List<NumberOfBookingAmenitiesDTO> result = new List<NumberOfBookingAmenitiesDTO>();

        foreach (var item in amenities)
        {
            result.Add(new NumberOfBookingAmenitiesDTO
            {
                AmenityId = item.AmenityId,
                AmenityName = item.AmenityName,
                Category = item.Category,
                Description = item.Description,
                Img_Url = item.Img_Url,
                UnitPrice = item.UnitPrice,
                NumberOfBooking = item.NumberOfBooking
            });
        }

        return new NumberBookingOfAmenitiesResult(result);
    }
}

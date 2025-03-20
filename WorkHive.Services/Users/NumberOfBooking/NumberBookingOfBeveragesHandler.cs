using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.DTOService;

namespace WorkHive.Services.Users.NumberOfBooking;

public record NumberBookingOfBeveragesQuery(int OwnerId)
    : IQuery<NumberBookingOfBeveragesResult>;
public record NumberBookingOfBeveragesResult(List<NumberOfBookingBeveragesDTO> NumberOfBookingBeveragesDTOs);


public class NumberBookingOfBeveragesHandler(IBookingWorkspaceUnitOfWork bookingUnit)
    : IQueryHandler<NumberBookingOfBeveragesQuery, NumberBookingOfBeveragesResult>
{
    public async Task<NumberBookingOfBeveragesResult> Handle(NumberBookingOfBeveragesQuery query, 
        CancellationToken cancellationToken)
    {
        var beverages = await bookingUnit.beverage.GetNumberOfBookingBeverage(query.OwnerId);

        List<NumberOfBookingBeveragesDTO> result = new List<NumberOfBookingBeveragesDTO>();

        foreach (var item in beverages)
        {
            result.Add(new NumberOfBookingBeveragesDTO
            {
                BeverageId = item.BeverageId,
                BeverageName = item.BeverageName,
                Category = item.Category,
                Description = item.Description,
                Img_Url = item.Img_Url,
                UnitPrice = item.UnitPrice,
                NumberOfBooking = item.NumberOfBooking
            });
        }

        return new NumberBookingOfBeveragesResult(result);
    }
}

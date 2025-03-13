using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.BuildingBlocks.Exceptions;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Amenity
{
    public record GetAllAmenitiesQuery() : IQuery<List<AmenityDT>>;

    public record AmenityDT(int Id, string Name, decimal? Price, int? Quantity, string ImgUrl, string Description, string Category, string Status);

    public class GetAllAmenitiesValidator : AbstractValidator<GetAllAmenitiesQuery>
    {
        public GetAllAmenitiesValidator()
        {
        }
    }

    public class GetAllAmenitiesHandler(IWorkSpaceManageUnitOfWork unit) : IQueryHandler<GetAllAmenitiesQuery, List<AmenityDT>>
    {
        public async Task<List<AmenityDT>> Handle(GetAllAmenitiesQuery query, CancellationToken cancellationToken)
        {
            var amenities = await unit.Amenity.GetAllAsync();

            if (amenities == null || !amenities.Any())
            {
                return new List<AmenityDT>();
            }

            return amenities.Select(am => new AmenityDT(
                am.Id,
                am.Name,
                am.Price,
                am.Quantity,
                am.ImgUrl,
                am.Description,
                am.Category,
                am.Status
            )).ToList();
        }
    }
}

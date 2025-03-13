using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.BuildingBlocks.Exceptions;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.GetAllById
{
    public record GetAmenitiesByOwnerIdQuery(int OwnerId) : IQuery<List<AmenityDTO>>;

    public record AmenityDTO(int Id, string Name, decimal? Price, int? Quantity, string ImgUrl, string Description, string Category, string Status);

    public class GetAmenitiesByOwnerIdValidator : AbstractValidator<GetAmenitiesByOwnerIdQuery>
    {
        public GetAmenitiesByOwnerIdValidator()
        {
            RuleFor(x => x.OwnerId)
                .GreaterThan(0).WithMessage("Owner ID must be greater than 0");
        }
    }
    public class GetAmenitiesByOwnerIdHandler(IWorkSpaceManageUnitOfWork OwnerManageUnit)
   : IQueryHandler<GetAmenitiesByOwnerIdQuery, List<AmenityDTO>>
    {
        public async Task<List<AmenityDTO>> Handle(GetAmenitiesByOwnerIdQuery query, CancellationToken cancellationToken)
        {
            var amenities = await OwnerManageUnit.Amenity.GetAmenitiesByOwnerIdAsync(query.OwnerId);

            if (amenities == null || !amenities.Any())
            {
                return null;
            }

            return amenities.Select(am => new AmenityDTO(
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

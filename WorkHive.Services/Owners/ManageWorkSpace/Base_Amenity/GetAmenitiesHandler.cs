using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Amenity
{
    public record GetAllAmenitiesQuery() : IQuery<List<Amenity>>;

    public class GetAllAmenitiesValidator : AbstractValidator<GetAllAmenitiesQuery>
    {
        public GetAllAmenitiesValidator()
        {
        }
    }

    class GetAllAmenitiesHandler(IWorkSpaceManageUnitOfWork unit) : IQueryHandler<GetAllAmenitiesQuery, List<Amenity>>
    {
        public async Task<List<Amenity>> Handle(GetAllAmenitiesQuery query, CancellationToken cancellationToken)
        {
            var amenities = await unit.Amenity.GetAllAsync();
            return amenities?.Any() == true ? amenities : new List<Amenity>();
        }
    }
}

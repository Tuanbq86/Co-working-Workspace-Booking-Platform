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
    public record GetAmenitiesByWorkSpaceIdCommand(int WorkSpaceId) : ICommand<List<AmenityDTO>>;

    public record AmenityDTO(int Id, string Name, decimal? Price, int? Quantity, string ImgUrl, string Description, string Category, string Status);

    public class GetAmenitiesByWorkSpaceIdValidator : AbstractValidator<GetAmenitiesByWorkSpaceIdCommand>
    {
        public GetAmenitiesByWorkSpaceIdValidator()
        {
            RuleFor(x => x.WorkSpaceId)
                .GreaterThan(0).WithMessage("WorkSpace ID must be greater than 0");
        }
    }
    public class GetAmenitiesByWorkSpaceIdHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
   : ICommandHandler<GetAmenitiesByWorkSpaceIdCommand, List<AmenityDTO>>
    {
        public async Task<List<AmenityDTO>> Handle(GetAmenitiesByWorkSpaceIdCommand command, CancellationToken cancellationToken)
        {
            var amenities = await workSpaceManageUnit.Amenity.GetAmenitiesByWorkSpaceIdAsync(command.WorkSpaceId);

            if (amenities == null || !amenities.Any())
            {
                throw new NotFoundException($"No amenities found for WorkSpaceId {command.WorkSpaceId}");
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

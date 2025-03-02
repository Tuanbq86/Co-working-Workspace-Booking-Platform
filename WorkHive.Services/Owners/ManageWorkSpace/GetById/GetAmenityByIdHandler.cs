using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.BuildingBlocks.Exceptions;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.GetById
{
    public record GetAmenityByIdCommand(int id) : ICommand<GetAmenityByIdResult>;
    public record GetAmenityByIdResult(int Id, string Name, decimal? Price, int? Quantity, string ImgUrl, string Description, string Category, string Status);


    public class GetAmenityByIdValidator : AbstractValidator<GetAmenityByIdCommand>
    {
        public GetAmenityByIdValidator()
        {
            RuleFor(x => x.id).GreaterThan(0).WithMessage("Id must be greater than 0");
        }
    }

    public class GetAmenityByIdHandler(IWorkSpaceManageUnitOfWork AmenityManageUnit)
    : ICommandHandler<GetAmenityByIdCommand, GetAmenityByIdResult>
    {
        public async Task<GetAmenityByIdResult> Handle(GetAmenityByIdCommand command, CancellationToken cancellationToken)
        {
            var amenity = await AmenityManageUnit.Amenity.GetByIdAsync(command.id);
            if (amenity == null)
            {
                throw new NotFoundException("Amenity not found!");
            }

            return new GetAmenityByIdResult(
                amenity.Id,
                amenity.Name,
                amenity.Price,
                amenity.Quantity,
                amenity.ImgUrl,
                amenity.Description,
                amenity.Category,
                amenity.Status
            );
        }

    }
}

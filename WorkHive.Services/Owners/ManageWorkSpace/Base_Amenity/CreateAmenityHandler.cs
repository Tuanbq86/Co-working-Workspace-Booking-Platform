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
    public record CreateAmenityCommand(string Name, decimal? Price, int? Quantity, string ImgUrl, string Description, string Category, string Status, int OwnerId)
        : ICommand<CreateAmenityResult>;

    public record CreateAmenityResult(string Notification);

    public class CreateAmenityHandler(IWorkSpaceManageUnitOfWork unit) : ICommandHandler<CreateAmenityCommand, CreateAmenityResult>
    {
        public async Task<CreateAmenityResult> Handle(CreateAmenityCommand command, CancellationToken cancellationToken)
        {
            var newAmenity = new Amenity
            {
                Name = command.Name,
                Price = command.Price,
                Quantity = command.Quantity,
                ImgUrl = command.ImgUrl,
                Description = command.Description,
                Category = command.Category,
                Status = command.Status,
                OwnerId = command.OwnerId,
                CreatedAt = DateTime.Now
            };

            await unit.Amenity.CreateAsync(newAmenity);
            await unit.SaveAsync();

            return new CreateAmenityResult("Amenity created successfully");
        }
    }
}

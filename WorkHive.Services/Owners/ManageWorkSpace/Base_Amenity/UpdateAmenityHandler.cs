using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Amenity
{
    public record UpdateAmenityCommand(int Id, string Name, string Description, string Category, string Status, string ImgUrl, int? Quantity, decimal? Price)
        : ICommand<UpdateAmenityResult>;

    public record UpdateAmenityResult(string Notification);

    public class UpdateAmenityHandler(IWorkSpaceManageUnitOfWork unit) : ICommandHandler<UpdateAmenityCommand, UpdateAmenityResult>
    {
        public async Task<UpdateAmenityResult> Handle(UpdateAmenityCommand command, CancellationToken cancellationToken)
        {
            var amenity = await unit.Amenity.GetByIdAsync(command.Id);
            if (amenity == null) return new UpdateAmenityResult("Amenity not found");

            amenity.Name = command.Name;
            amenity.Price = command.Price;
            amenity.Quantity = command.Quantity;
            amenity.Description = command.Description;
            amenity.Category = command.Category;
            amenity.Status = command.Status;
            amenity.ImgUrl = command.ImgUrl;
            amenity.UpdatedAt = DateTime.Now;

            await unit.Amenity.UpdateAsync(amenity);
            await unit.SaveAsync();

            return new UpdateAmenityResult("Amenity updated successfully");
        }
    }
}

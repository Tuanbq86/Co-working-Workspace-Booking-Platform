using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Amenity
{
    public record DeleteAmenityCommand(int Id) : ICommand<DeleteAmenityResult>;

    public record DeleteAmenityResult(string Notification);

    public class DeleteAmenityHandler(IWorkSpaceManageUnitOfWork unit) : ICommandHandler<DeleteAmenityCommand, DeleteAmenityResult>
    {
        public async Task<DeleteAmenityResult> Handle(DeleteAmenityCommand command, CancellationToken cancellationToken)
        {
            var amenity = await unit.Amenity.GetByIdAsync(command.Id);
            if (amenity == null) return new DeleteAmenityResult("Amenity not found");

            await unit.Amenity.RemoveAsync(amenity);
            await unit.SaveAsync();

            return new DeleteAmenityResult("Amenity deleted successfully");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Beverage
{
    public record UpdateBeverageCommand(int Id, string Name, decimal Price, string ImgUrl, string Description, string Category, string Status)
        : ICommand<UpdateBeverageResult>;


    public record UpdateBeverageResult(string Notification);

    public class UpdateBeverageHandler(IWorkSpaceManageUnitOfWork unit) : ICommandHandler<UpdateBeverageCommand, UpdateBeverageResult>
    {
        public async Task<UpdateBeverageResult> Handle(UpdateBeverageCommand command, CancellationToken cancellationToken)
        {
            var beverage = await unit.Beverage.GetByIdAsync(command.Id);
            if (beverage == null) return new UpdateBeverageResult("Beverage not found");

            beverage.Name = command.Name;
            beverage.Price = command.Price;
            beverage.ImgUrl = command.ImgUrl;
            beverage.Description = command.Description;
            beverage.Category = command.Category;
            beverage.Status = command.Status;
            beverage.UpdatedAt = DateTime.UtcNow;

            await unit.Beverage.UpdateAsync(beverage);
            await unit.SaveAsync();

            return new UpdateBeverageResult("Beverage updated successfully");
        }
    }
}

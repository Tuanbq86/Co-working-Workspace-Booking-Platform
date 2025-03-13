using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Beverage
{
    public record CreateBeverageCommand(string Name, decimal Price, string ImgUrl, string Description, string Category, string Status, int OwnerId) : ICommand<CreateBeverageResult>;

    public record CreateBeverageResult(string Notification);

    public class CreateBeverageHandler(IWorkSpaceManageUnitOfWork unit) : ICommandHandler<CreateBeverageCommand, CreateBeverageResult>
    {
        public async Task<CreateBeverageResult> Handle(CreateBeverageCommand command, CancellationToken cancellationToken)
        {
            var newBeverage = new Beverage
            {
                Name = command.Name,
                Price = command.Price,
                ImgUrl = command.ImgUrl,
                Description = command.Description,
                Category = command.Category,
                Status = command.Status,
                OwnerId = command.OwnerId,
                CreatedAt = DateTime.UtcNow
            };

            await unit.Beverage.CreateAsync(newBeverage);
            await unit.SaveAsync();

            return new CreateBeverageResult("Beverage created successfully");
        }
    }
}

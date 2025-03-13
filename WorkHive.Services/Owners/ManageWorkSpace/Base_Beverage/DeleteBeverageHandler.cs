using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Beverage
{
    public record DeleteBeverageCommand(int Id) : ICommand<DeleteBeverageResult>;

    public record DeleteBeverageResult(string Notification);

    public class DeleteBeverageHandler(IWorkSpaceManageUnitOfWork unit) : ICommandHandler<DeleteBeverageCommand, DeleteBeverageResult>
    {
        public async Task<DeleteBeverageResult> Handle(DeleteBeverageCommand command, CancellationToken cancellationToken)
        {
            var beverage = await unit.Beverage.GetByIdAsync(command.Id);
            if (beverage == null) return new DeleteBeverageResult("Beverage not found");

            await unit.Beverage.RemoveAsync(beverage);
            await unit.SaveAsync();

            return new DeleteBeverageResult("Beverage deleted successfully");
        }
    }
}

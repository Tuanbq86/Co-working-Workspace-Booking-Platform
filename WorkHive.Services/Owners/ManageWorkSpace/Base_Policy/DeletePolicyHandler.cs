using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Policy
{
    public record DeletePolicyCommand(int Id) : ICommand<DeletePolicyResult>;
    public record DeletePolicyResult(string Notification);

    public class DeletePolicyHandler(IWorkSpaceManageUnitOfWork unit)
        : ICommandHandler<DeletePolicyCommand, DeletePolicyResult>
    {
        public async Task<DeletePolicyResult> Handle(DeletePolicyCommand command, CancellationToken cancellationToken)
        {
            var policy = await unit.Policy.GetByIdAsync(command.Id);
            if (policy == null) return new DeletePolicyResult("Policy not found");

            await unit.Policy.RemoveAsync(policy);
            await unit.SaveAsync();
            return new DeletePolicyResult("Policy deleted successfully");
        }
    }
}

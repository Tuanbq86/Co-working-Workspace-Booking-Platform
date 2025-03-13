using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Policy
{
    public record UpdatePolicyCommand(int Id, string Name) : ICommand<UpdatePolicyResult>;
    public record UpdatePolicyResult(string Notification);

    public class UpdatePolicyHandler(IWorkSpaceManageUnitOfWork unit)
        : ICommandHandler<UpdatePolicyCommand, UpdatePolicyResult>
    {
        public async Task<UpdatePolicyResult> Handle(UpdatePolicyCommand command, CancellationToken cancellationToken)
        {
            var policy = await unit.Policy.GetByIdAsync(command.Id);
            if (policy == null) return new UpdatePolicyResult("Policy not found");

            policy.Name = command.Name;
            await unit.Policy.UpdateAsync(policy);
            await unit.SaveAsync();
            return new UpdatePolicyResult("Policy updated successfully");
        }
    }
}
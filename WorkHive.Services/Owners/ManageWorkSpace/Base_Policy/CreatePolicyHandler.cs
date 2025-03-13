using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Policy
{
    public record CreatePolicyCommand
    (string Name) : ICommand<CreatePolicyResult>;

    public record CreatePolicyResult
    (string Notification);  

    public class CreatePolicyValidator
    {

    }
    class CreatePolicyHandler( IWorkSpaceManageUnitOfWork unit ) : ICommandHandler<CreatePolicyCommand, CreatePolicyResult>
    {
        public async Task<CreatePolicyResult> Handle(CreatePolicyCommand command, CancellationToken cancellationToken)
        {
            Policy newPolicy = new Policy
            {
                Name = command.Name
            };
            await unit.Policy.CreateAsync(newPolicy);
            return new CreatePolicyResult("Policy created successfully");
        }
    }
}

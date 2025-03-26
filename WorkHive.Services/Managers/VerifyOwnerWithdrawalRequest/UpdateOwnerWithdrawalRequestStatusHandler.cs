using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Managers.VerifyOwnerWithdrawalRequest
{
    public record UpdateOwnerWithdrawalRequestStatusCommand(int Id, string Status) : ICommand<UpdateOwnerWithdrawalRequestStatusResult>;

    public record UpdateOwnerWithdrawalRequestStatusResult(string Notification);

    class UpdateOwnerWithdrawalRequestStatusHandler(IWalletUnitOfWork unit) : ICommandHandler<UpdateOwnerWithdrawalRequestStatusCommand, UpdateOwnerWithdrawalRequestStatusResult>
    {
        public async Task<UpdateOwnerWithdrawalRequestStatusResult?> Handle(UpdateOwnerWithdrawalRequestStatusCommand command, CancellationToken cancellationToken)
        {
            var request = await unit.OwnerWithdrawalRequest.GetByIdAsync(command.Id);
            if (request == null) return null;

            request.Status = command.Status;
            await unit.OwnerWithdrawalRequest.UpdateAsync(request);
            await unit.SaveAsync();

            return new UpdateOwnerWithdrawalRequestStatusResult($"Owner withdrawal request updated to {command.Status}");
        }
    }
}
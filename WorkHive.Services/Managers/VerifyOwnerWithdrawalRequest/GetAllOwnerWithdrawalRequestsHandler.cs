using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Managers.VerifyOwnerWithdrawalRequest
{
    public record GetAllOwnerWithdrawalRequestsQuery() : IQuery<List<OwnerWithdrawalRequestDTO>>;

    public record OwnerWithdrawalRequestDTO(int Id, string Title, string Description, string Status, DateTime? CreatedAt, int WorkspaceOwnerId, int? UserId);

    class GetAllOwnerWithdrawalRequestsHandler(IWalletUnitOfWork unit) : IQueryHandler<GetAllOwnerWithdrawalRequestsQuery, List<OwnerWithdrawalRequestDTO>>
    {
        public async Task<List<OwnerWithdrawalRequestDTO>> Handle(GetAllOwnerWithdrawalRequestsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var requests = await unit.OwnerWithdrawalRequest.GetAllAsync();
                return requests?.Select(r => new OwnerWithdrawalRequestDTO(r.Id, r.Title, r.Description, r.Status, r.CreatedAt, r.WorkspaceOwnerId, r.UserId)).ToList() ?? new List<OwnerWithdrawalRequestDTO>();
            }
            catch
            {
                return new List<OwnerWithdrawalRequestDTO>();
            }
        }
    }
}

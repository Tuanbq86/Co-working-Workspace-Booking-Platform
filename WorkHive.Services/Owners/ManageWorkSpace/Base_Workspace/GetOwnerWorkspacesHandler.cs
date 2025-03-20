using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Workspace
{
    public record GetOwnerWorkspacesQuery(int OwnerId) : IQuery<List<GetWorkspaceRevenueResult>>;

    public record GetWorkspaceRevenueResult(
        int WorkspaceId,
        string WorkspaceName,
        decimal Revenue
    );

    public class GetOwnerWorkspacesValidator : AbstractValidator<GetOwnerWorkspacesQuery>
    {
        public GetOwnerWorkspacesValidator()
        {
            RuleFor(x => x.OwnerId).GreaterThan(0);
        }
    }

    public class GetOwnerWorkspacesHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
        : IQueryHandler<GetOwnerWorkspacesQuery, List<GetWorkspaceRevenueResult>>
    {
        public async Task<List<GetWorkspaceRevenueResult>> Handle(GetOwnerWorkspacesQuery query,
            CancellationToken cancellationToken)
        {
            var workspaces = await workSpaceManageUnit.Workspace.GetByOwnerIdAsync(query.OwnerId);
            if (workspaces == null || !workspaces.Any())
            {
                return new List<GetWorkspaceRevenueResult>();
            }

            var results = new List<GetWorkspaceRevenueResult>();
            foreach (var workspace in workspaces)
            {
                var revenue = (await workSpaceManageUnit.Booking
                .GetTotalRevenueByWorkspaceIdAsync(workspace.Id, "Success")).GetValueOrDefault();

                results.Add(new GetWorkspaceRevenueResult(workspace.Id, workspace.Name, revenue));
            }

            return results;
        }
    }
}

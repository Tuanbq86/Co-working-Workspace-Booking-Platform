using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Promotion
{
    public record GetPromotionsByWorkspaceIdQuery(int WorkspaceID) : IQuery<List<PromotionDT>>;
    public class GetPromotionsByWorkspaceIdValidator : AbstractValidator<GetPromotionsByWorkspaceIdQuery>
    {
        public GetPromotionsByWorkspaceIdValidator()
        {
            RuleFor(query => query.WorkspaceID)
                .GreaterThan(0)
                .WithMessage("WorkspaceID phải lớn hơn 0");
        }
    }
    class GetPromotionsByWorkspaceIdHandler(IWorkSpaceManageUnitOfWork unit)
        : IQueryHandler<GetPromotionsByWorkspaceIdQuery, List<PromotionDT>>
    {
        public async Task<List<PromotionDT>> Handle(GetPromotionsByWorkspaceIdQuery query, CancellationToken cancellationToken)
        {
            var promotions = await unit.Promotion.GetAllAsync();

            if (promotions == null || !promotions.Any())
            {
                return new List<PromotionDT>();
            }

            return promotions
                .Where(pr => pr.WorkspaceId == query.WorkspaceID) 
                .Select(pr => new PromotionDT(
                    pr.Id,
                    pr.Code,
                    pr.Discount,
                    pr.StartDate,
                    pr.EndDate,
                    pr.Status,
                    pr.WorkspaceId,
                    pr.Description
                )).ToList();
        }
    }
}

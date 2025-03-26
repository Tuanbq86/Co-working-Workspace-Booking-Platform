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
    public record GetPromotionsByWorkspaceOwnerIdQuery(int WorkspaceOwnerID) : IQuery<List<PromotionsByOwnerIdDT>>;
    public record PromotionsByOwnerIdDT(int Id, string Code, int? Discount, DateTime? StartDate, DateTime? EndDate, string Status, int WorkspaceID, string Description);
    public class GetPromotionsByWorkspaceOwnerIdValidator : AbstractValidator<GetPromotionsByWorkspaceIdQuery>
    {
        //public GetPromotionsByWorkspaceOwnerIdValidator()
        //{
        //    RuleFor(query => query.WorkspaceOwnerID)
        //        .GreaterThan(0)
        //        .WithMessage("WorkspaceOwnerID phải lớn hơn 0");
        //}
    }
    class GetPromotionsByWorkspaceOwnerIdHandler(IWorkSpaceManageUnitOfWork unit)
        : IQueryHandler<GetPromotionsByWorkspaceOwnerIdQuery, List<PromotionsByOwnerIdDT>>
    {
        public async Task<List<PromotionsByOwnerIdDT>> Handle(GetPromotionsByWorkspaceOwnerIdQuery query, CancellationToken cancellationToken)
        {
            var promotions = await unit.Promotion.GetAllPromotionsByWorkspaceOwnerIdAsync(query.WorkspaceOwnerID);
            
            if (promotions == null || !promotions.Any())
            {
                return new List<PromotionsByOwnerIdDT>();
            }

            return promotions
                .Select(pr => new PromotionsByOwnerIdDT(
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

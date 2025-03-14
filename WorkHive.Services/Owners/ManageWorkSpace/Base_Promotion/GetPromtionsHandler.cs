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
    public record GetAllPromotionsQuery() : IQuery<List<PromotionDT>>;

    public record PromotionDT(int Id, string Code, int? Discount, DateTime? StartDate, DateTime? EndDate, string Status, int WorkspaceID);

    public class GetAllPromotionsValidator : AbstractValidator<GetAllPromotionsQuery>
    {
        public GetAllPromotionsValidator()
        {
        }
    }

    class GetAllPromotionsHandler(IWorkSpaceManageUnitOfWork unit) : IQueryHandler<GetAllPromotionsQuery, List<PromotionDT>>
    {
        public async Task<List<PromotionDT>> Handle(GetAllPromotionsQuery query, CancellationToken cancellationToken)
        {
            var promotions = await unit.Promotion.GetAllAsync();

            if (promotions == null || !promotions.Any())
            {
                return new List<PromotionDT>();
            }

            return promotions.Select(pr => new PromotionDT(
                pr.Id,
                pr.Code,
                pr.Discount,
                pr.StartDate,
                pr.EndDate,
                pr.Status,
                pr.WorkspaceId
            )).ToList();
        }
    }
}

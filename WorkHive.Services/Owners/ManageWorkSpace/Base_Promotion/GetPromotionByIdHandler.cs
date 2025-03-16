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
    public record GetPromotionByIdQuery(int Id) : IQuery<GetPromotionByIdResult>;
    public record GetPromotionByIdResult(int Id, string Code, int? Discount, DateTime? StartDate, DateTime? EndDate, DateTime? CreatedAt, DateTime? UpdatedAt, string Status, int WorkspaceId, string Description);

    public class GetPromotionByIdValidator : AbstractValidator<GetPromotionByIdQuery>
    {
        public GetPromotionByIdValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be greater than 0");
        }
    }

    public class GetPromotionByIdHandler(IWorkSpaceManageUnitOfWork PromotionManageUnit)
        : IQueryHandler<GetPromotionByIdQuery, GetPromotionByIdResult>
    {
        public async Task<GetPromotionByIdResult> Handle(GetPromotionByIdQuery query, CancellationToken cancellationToken)
        {
            var promotion = await PromotionManageUnit.Promotion.GetByIdAsync(query.Id);
            if (promotion == null)
            {
                return null;
            }

            return new GetPromotionByIdResult(
                promotion.Id,
                promotion.Code,
                promotion.Discount,
                promotion.StartDate,
                promotion.EndDate,
                promotion.CreatedAt,
                promotion.UpdatedAt,
                promotion.Status,
                promotion.WorkspaceId,
                promotion.Description
            );
        }
    }
}

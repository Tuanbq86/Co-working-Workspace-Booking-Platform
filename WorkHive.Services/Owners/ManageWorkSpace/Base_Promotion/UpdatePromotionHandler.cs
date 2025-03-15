using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Promotion
{
    public record UpdatePromotionCommand(int Id, string Code, int Discount, DateTime StartDate, DateTime EndDate, string Status, string Description)
        : ICommand<UpdatePromotionResult>;

    public record UpdatePromotionResult(string Notification);

    public class UpdatePromotionHandler(IWorkSpaceManageUnitOfWork unit) : ICommandHandler<UpdatePromotionCommand, UpdatePromotionResult>
    {
        public async Task<UpdatePromotionResult> Handle(UpdatePromotionCommand command, CancellationToken cancellationToken)
        {
            var promotion = await unit.Promotion.GetByIdAsync(command.Id);
            if (promotion == null)
            {
                return new UpdatePromotionResult("Không tìm thấy mã khuyến mãi");
            }

            var existingPromotion = await unit.Promotion.GetFirstOrDefaultAsync(p => p.Code == command.Code && p.Id != command.Id);
            if (existingPromotion != null)
            {
                return new UpdatePromotionResult("Mã khuyến mãi đã tồn tại");
            }

            promotion.Code = command.Code;
            promotion.Discount = command.Discount;
            promotion.StartDate = command.StartDate;
            promotion.EndDate = command.EndDate;
            promotion.Status = command.Status;
            promotion.Description = command.Description;
            promotion.UpdatedAt = DateTime.UtcNow;

            await unit.Promotion.UpdateAsync(promotion);
            await unit.SaveAsync();

            return new UpdatePromotionResult("Promotion updated successfully");
        }
    }
}

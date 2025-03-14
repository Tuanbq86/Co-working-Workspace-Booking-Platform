using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Promotion
{
    public record DeletePromotionCommand(int Id) : ICommand<DeletePromotionResult>;

    public record DeletePromotionResult(string Notification);

    public class DeletePromotionHandler(IWorkSpaceManageUnitOfWork unit) : ICommandHandler<DeletePromotionCommand, DeletePromotionResult>
    {
        public async Task<DeletePromotionResult> Handle(DeletePromotionCommand command, CancellationToken cancellationToken)
        {
            var promotion = await unit.Promotion.GetByIdAsync(command.Id);
            if (promotion == null) return new DeletePromotionResult("Promotion not found");

            await unit.Promotion.RemoveAsync(promotion);
            await unit.SaveAsync();

            return new DeletePromotionResult("Xóa mã giảm giá thành công");
        }
    }
}


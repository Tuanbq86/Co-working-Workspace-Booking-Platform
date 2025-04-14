using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Promotion
{
    public record CreatePromotionCommand(string Code, int Discount, DateTime StartDate, DateTime EndDate, string Status, int WorkspaceId, string Description) : ICommand<CreatePromotionResult>;

    public record CreatePromotionResult(string Notification);

    public class CreatePromotionHandler(IWorkSpaceManageUnitOfWork unit) : ICommandHandler<CreatePromotionCommand, CreatePromotionResult>
    {
        public async Task<CreatePromotionResult> Handle(CreatePromotionCommand command, CancellationToken cancellationToken)
        {
            var workspace = await unit.Workspace.GetByIdAsync(command.WorkspaceId);
            if (workspace == null)
            {
                return new CreatePromotionResult("Không tìm thấy workspace.");
            }

            //var ownerId = workspace.OwnerId;

            // Kiểm tra mã khuyến mãi đã tồn tại cho cùng Owner chưa
            var existingPromotion = await unit.Promotion
                .GetFirstOrDefaultAsync(p => p.Code == command.Code && p.Workspace.Id == command.WorkspaceId);

            if (existingPromotion != null)
            {
                return new CreatePromotionResult("Mã khuyến mãi đã tồn tại cho workspace này");
            }
            var newPromotion = new Promotion
            {
                Code = command.Code,
                Discount = command.Discount,
                Description = command.Description,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                Status = command.Status,
                WorkspaceId = command.WorkspaceId,
                CreatedAt = DateTime.Now
            };

            await unit.Promotion.CreateAsync(newPromotion);
            await unit.SaveAsync();

            return new CreatePromotionResult("Tạo mã khuyến mãi thành công");
        }
    }
}

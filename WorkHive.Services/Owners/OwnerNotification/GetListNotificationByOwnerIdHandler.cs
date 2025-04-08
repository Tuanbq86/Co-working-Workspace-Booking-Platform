using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.OwnerNotification
{
    public record GetListNotificationByOwnerIdQuery(int OwnerId)
    : IQuery<GetListNotificationByOwnerIdResult>;
    public record GetListNotificationByOwnerIdResult(List<OwnerNotificationDTO> OwnerNotificationDTOs);
    public record OwnerNotificationDTO(int OwnerNotificationId, string Description, string Status, int OwnerId,
        DateTime? CreateAt, int? IsRead, string Title);

    public class GetListNotificationByOwnerIdHandler(IWorkspaceOwnerUnitOfWork OwnerUnit)
        : IQueryHandler<GetListNotificationByOwnerIdQuery, GetListNotificationByOwnerIdResult>
    {
        public async Task<GetListNotificationByOwnerIdResult> Handle(GetListNotificationByOwnerIdQuery query,
            CancellationToken cancellationToken)
        {
            var OwnerNotificationList = OwnerUnit.OwnerNotification.GetAll()
                .Where(un => un.OwnerId.Equals(query.OwnerId)).ToList();

            List<OwnerNotificationDTO> result = new List<OwnerNotificationDTO>();

            foreach (var item in OwnerNotificationList)
            {
                result.Add(new OwnerNotificationDTO(item.Id, item.Description, item.Status, item.OwnerId,
                    item.CreatedAt, item.IsRead, item.Title));
            }

            return new GetListNotificationByOwnerIdResult(result);
        }
    }

}

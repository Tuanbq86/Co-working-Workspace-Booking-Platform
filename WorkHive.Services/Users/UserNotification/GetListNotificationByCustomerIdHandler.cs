using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Users.UserNotification;

public record GetListNotificationByCustomerIdQuery(int CustomerId)
    : IQuery<GetListNotificationByCustomerIdResult>;
public record GetListNotificationByCustomerIdResult(List<CustomerNotificationDTO> CustomerNotificationDTOs);
public record CustomerNotificationDTO(int UserNotificationId, string Description, string Status, int UserId, 
    DateTime? CreateAt, int? IsRead);

public class GetListNotificationByCustomerIdHandler(IUserUnitOfWork userUnit)
    : IQueryHandler<GetListNotificationByCustomerIdQuery, GetListNotificationByCustomerIdResult>
{
    public async Task<GetListNotificationByCustomerIdResult> Handle(GetListNotificationByCustomerIdQuery query, 
        CancellationToken cancellationToken)
    {
        var userNotificationList = userUnit.UserNotification.GetAll()
            .Where(un => un.UserId.Equals(query.CustomerId)).ToList();

        List<CustomerNotificationDTO> result = new List<CustomerNotificationDTO>();

        foreach (var item in userNotificationList)
        {
            result.Add(new CustomerNotificationDTO(item.Id, item.Description, item.Status, item.UserId, 
                item.CreatedAt, item.IsRead));
        }

        return new GetListNotificationByCustomerIdResult(result);
    }
}

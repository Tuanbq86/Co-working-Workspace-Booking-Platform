using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Users.FirebaseServices;

namespace WorkHive.Services.Users.NotificationMobile;

public record SendNotificationForMobileCommand(string FcmToken, string Title, string Body)
    : ICommand<SendNotificationForMobileResult>;
public record SendNotificationForMobileResult(string Notification);

public class SendNotificationForMobileHandler(IFirebaseNotificationService firebase)
    : ICommandHandler<SendNotificationForMobileCommand, SendNotificationForMobileResult>
{
    public async Task<SendNotificationForMobileResult> Handle(SendNotificationForMobileCommand query,
        CancellationToken cancellationToken)
    {
        await firebase.SendNotificationAsync(query.FcmToken, query.Title, query.Body);
        return new SendNotificationForMobileResult("Gửi thông báo thành công");
    }
}


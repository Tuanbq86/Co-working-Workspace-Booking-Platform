using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Users.FirebaseServices;

namespace WorkHive.Services.Users.NotificationMobile;

public record SendNotificationForMobileCommand(string fcmToken, string title, string body)
    : ICommand<SendNotificationForMobileResult>;
public record SendNotificationForMobileResult(string Notification);

public class SendNotificationForMobileHandler(IFirebaseNotificationService firebase)
    : ICommandHandler<SendNotificationForMobileCommand, SendNotificationForMobileResult>
{
    public async Task<SendNotificationForMobileResult> Handle(SendNotificationForMobileCommand query,
        CancellationToken cancellationToken)
    {
        await firebase.SendNotificationAsync(query.fcmToken, query.title, query.body);
        return new SendNotificationForMobileResult("Gửi thông báo thành công");
    }
}


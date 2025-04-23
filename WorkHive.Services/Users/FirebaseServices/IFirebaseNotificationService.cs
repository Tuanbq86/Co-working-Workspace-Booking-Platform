using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHive.Services.Users.FirebaseServices;

public interface IFirebaseNotificationService
{
    public Task SendNotificationAsync(string fcmToken, string title, string body);
}

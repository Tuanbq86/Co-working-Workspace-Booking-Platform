using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;

namespace WorkHive.Services.Users.FirebaseServices;

public class FirebaseNotificationService : IFirebaseNotificationService
{
    private readonly FirebaseApp _firebaseApp;

    public FirebaseNotificationService(IWebHostEnvironment env)
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            _firebaseApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(
                    Path.Combine(env.ContentRootPath, "firebase-adminsdk.json"))
            });
        }
        else
        {
            _firebaseApp = FirebaseApp.DefaultInstance;
        }
    }

    public async Task SendNotificationAsync(string fcmToken, string title, string body)
    {
        var message = new Message()
        {
            Token = fcmToken,
            Notification = new Notification
            {
                Title = title,
                Body = body
            },
            Android = new AndroidConfig
            {
                Priority = Priority.High
            }
        };

        await FirebaseMessaging.DefaultInstance.SendAsync(message);
    }
}


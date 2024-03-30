using Android.App;
using Android.OS;
using Android.Widget;
using CustomProgressBar.Droid.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotificationService))]
namespace CustomProgressBar.Droid.Services
{
    public class NotificationService : INotificationService
    {
        private const string CHANNEL_ID = "localNotification";
        private const string CHANNEL_NAME = "channelName";
        private const string CHANNEL_DESCRIPTION = "channelDescription";
        private const int PROGRESS_BAR_MAX = 100;
        private bool isChannelInitialized = false;
        NotificationManager notificationManager;

        public async Task ShowNotification()
        {
            if (!isChannelInitialized)
            {
                CreateChannel();
            }

            // Define your custom layout using RemoteViews
            RemoteViews remoteViews = new RemoteViews(
                Android.App.Application.Context.PackageName, Resource.Layout.activity_main);

            // Create the notification
            var notificationBuilder = new Notification.Builder(
                Android.App.Application.Context, CHANNEL_ID)
                .SetSmallIcon(Resource.Drawable.notification)
                .SetCustomContentView(remoteViews);

            // Build the notification
            var notification = notificationBuilder.Build();

            for (int i = 10; i <= 100; i += 10)
            {
                // Pause execution for 1 second
                await Task.Delay(1001);

                remoteViews.SetTextViewText(Resource.Id.textView2, $"{i}%");
                remoteViews.SetProgressBar(Resource.Id.progressBar, PROGRESS_BAR_MAX, i, false);

                // Show the notification
                notificationManager.Notify(0, notification);
            }
        }

        // Create the channel (for Android 8.0 and above)
        private void CreateChannel()
        {
            try
            {
                notificationManager = (NotificationManager)Android.App.Application.Context
                    .GetSystemService(Android.Content.Context.NotificationService);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var channelNameJava = new Java.Lang.String(CHANNEL_NAME);
                    var channel = new NotificationChannel(CHANNEL_ID, channelNameJava,
                        NotificationImportance.Default)
                    {
                        Description = CHANNEL_DESCRIPTION
                    };
                    notificationManager.CreateNotificationChannel(channel);
                }

                isChannelInitialized = true;
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                isChannelInitialized = false;
            }
        }
    }
}
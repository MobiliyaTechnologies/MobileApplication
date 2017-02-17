using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Util;
using Firebase.Messaging;
using CSU_PORTABLE.Droid.UI;
using CSU_PORTABLE.Droid.Utils;
using CSU_PORTABLE.Models;

namespace CSU_PORTABLE.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            try
            {
                Log.Debug(TAG, "From: " + message.From);
                Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);

                var preferenceHandler = new PreferenceHandler();
                bool isLoggedIn = preferenceHandler.IsLoggedIn();
                if(isLoggedIn)
                {
                    int roleId = preferenceHandler.GetUserDetails().Role_Id;
                    if (roleId == (int)CSU_PORTABLE.Utils.Constants.USER_ROLE.ADMIN)
                    {
                        SendNotification(message.GetNotification().Body);
                    }
                }
            } catch (Exception e) 
            {
                Log.Debug(TAG, e.Message);
            }
        }

        void SendNotification(string messageBody)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.ic_stat_ic_notification)
                .SetContentTitle("FCM Message")
                .SetContentText(messageBody)
                .SetAutoCancel(true)
                .SetPriority((int)NotificationPriority.Max)
                .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}
using System;
using Android.App;
using Firebase.Iid;
using Android.Util;
using Android.Widget;
using Firebase.Messaging;
using EM_PORTABLE.Utils;

namespace CSU_Android_App
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";
        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "MyFirebaseIIDService() Refreshed token: " + refreshedToken);
            //SendRegistrationToServer(refreshedToken);
            if (Constants.IsDemoMode)
            {
                FirebaseMessaging.Instance.SubscribeToTopic("AlertsDemo");
            }
            else
            {
                FirebaseMessaging.Instance.SubscribeToTopic("Alerts");
            }

        }


        void SendRegistrationToServer(string token)
        {
            Log.Debug(TAG, "SendRegistrationToServer token: " + token);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net;

namespace CSU_PORTABLE.Droid.Utils
{
    public class Utils
    {
        public const string ALERT_BROADCAST = "com.mobiliya.csu.Alerts";

        public static bool IsNetworkEnabled(Context context)
        {
            bool isOnline = false;
            ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;
            if (networkInfo != null)
            {
                isOnline = networkInfo.IsConnected;
            }
            return isOnline;
        }
    }
}
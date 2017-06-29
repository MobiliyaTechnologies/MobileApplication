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
        private static Toast toast;

        /// <summary>
        /// Common fuction to check Internet Connectivity
        /// </summary>
        /// <param name="context">Activity from where function is invoked</param>
        /// <returns>True if connected</returns>
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

        /// <summary>
        /// Show Toast message for all screens
        /// </summary>
        /// <param name="context"> Context on which toast is to be displayed</param>
        /// <param name="message"> Message to be shown in Toast</param>
        public static void ShowToast(Context context, string message)
        {
            if (toast != null)
            {
                toast.Cancel();
            }
            toast = Toast.MakeText(context, message, ToastLength.Short);
            toast.Show();
        }

        /// <summary>
        /// Common Dialog option
        /// </summary>
        /// <param name="context">Activiy context which invokes the Dialog </param>
        /// <param name="message">Message to be showed in Dialog</param>
        public static void ShowDialog(Context context, string message)
        {
            var builder = new AlertDialog.Builder(context);
            builder.SetTitle("Energy Management");
            builder.SetMessage(message);
            builder.SetPositiveButton("OK", (sender, args) => { /* do stuff on OK */ });
            //builder.SetNegativeButton("No", (sender, args) => { cmd = "cancel"; });
            builder.SetCancelable(false);
            builder.Show();
        }

    }
}
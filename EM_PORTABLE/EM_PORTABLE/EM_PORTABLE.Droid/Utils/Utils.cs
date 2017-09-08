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
using EM_PORTABLE.Utils;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using EM_PORTABLE.Droid.UI;
using static EM_PORTABLE.Utils.Constants;
using System.Net;
using EM_PORTABLE.Models;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Android.Graphics;

namespace EM_PORTABLE.Droid.Utils
{
    public class Utils
    {
        public const string ALERT_BROADCAST = "com.mobiliya.em.Alerts";
        public const string ALERT_BROADCAST_DEMO = "com.mobiliya.em.AlertsDemo";
        private static Toast toast;

        public static DemoStage CurrentStage;

        public static Color PrimaryColor = Color.Rgb(70, 78, 120);
        public static Color SecondaryColor = Color.Rgb(53, 172, 207);
        public static Color VeryCold = Color.Rgb(193, 235, 244);
        public static Color Cold = Color.Rgb(148, 221, 242);
        public static Color Normal = Color.Rgb(150, 197, 245);
        public static Color Hot = Color.Rgb(210, 207, 235);
        public static Color VeryHot = Color.Rgb(235, 230, 207);

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
            builder.SetCancelable(false);
            builder.Show();
        }


        public static void RedirectToLogin(Context context)
        {
            PreferenceHandler.setLoggedIn(false);
            PreferenceHandler.SetToken(string.Empty);
            PreferenceHandler.SetRefreshToken(string.Empty);
            Intent intent = new Intent(Application.Context, typeof(LoginNewActivity));
            intent.PutExtra(LoginNewActivity.KEY_SHOW_PAGE, (int)SignInType.SIGN_IN);
            context.StartActivity(intent);
        }

        public static async Task GetToken()
        {
            string tokenURL = string.Format(B2CConfig.TokenURL, B2CConfig.Tenant, B2CPolicy.SignInPolicyId, B2CConfig.ClientId, PreferenceHandler.GetAccessCode());
            var response = await InvokeApi.Authenticate(tokenURL, string.Empty, HttpMethod.Post);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string strContent = await response.Content.ReadAsStringAsync();
                var tokenNew = JsonConvert.DeserializeObject<AccessToken>(strContent);
                PreferenceHandler.SetToken(tokenNew.id_token);
                PreferenceHandler.SetRefreshToken(tokenNew.refresh_token);
            }
        }

        public static async Task RefreshToken(Context context)
        {
            string tokenURL = string.Format(B2CConfig.RefreshTokenURL, B2CConfig.Tenant, B2CPolicy.SignInPolicyId, B2CConfig.ClientId, PreferenceHandler.GetRefreshToken());
            var response = await InvokeApi.Authenticate(tokenURL, string.Empty, HttpMethod.Post);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string strContent = await response.Content.ReadAsStringAsync();
                var tokenNew = JsonConvert.DeserializeObject<AccessToken>(strContent);
                PreferenceHandler.SetToken(tokenNew.id_token);
                PreferenceHandler.SetRefreshToken(tokenNew.refresh_token);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                RedirectToLogin(context);
            }
        }


    }
}
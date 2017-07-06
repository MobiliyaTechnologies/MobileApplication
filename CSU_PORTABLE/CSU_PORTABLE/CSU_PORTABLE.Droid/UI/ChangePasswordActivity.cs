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
using Android.Util;
using CSU_PORTABLE.Droid.Utils;
using Android.Support.V7.App;
using Newtonsoft.Json;
using System.Threading.Tasks;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using System.Net.Http;
using Android.Webkit;
using Android.Graphics;
using Android.Content.PM;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "Change Password", MainLauncher = false, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
    public class ChangePasswordActivity : AppCompatActivity
    {
        const string TAG = "ChangePasswordActivity";
        private WebView localWebView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (!Utils.Utils.IsNetworkEnabled(this))
            {
                RunOnUiThread(() =>
                {
                    Utils.Utils.ShowDialog(this, "Internet not available.");
                });
                StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                Finish();
            }
            else
            {
                string strLogin = string.Format(B2CConfig.ChangePasswordURL, B2CConfig.Tenant, B2CPolicy.ChangePasswordPolicyId, B2CConfig.ClientId, B2CConfig.Redirect_Uri);
                SetContentView(Resource.Layout.LoginNew);
                localWebView = FindViewById<WebView>(Resource.Id.LocalWebView);

                localWebView.SetWebViewClient(new ChangePasswordView()); // stops request going to Web Browser
                localWebView.Settings.JavaScriptEnabled = true;
                localWebView.LoadUrl(strLogin);
            }
        }

    }

    public class ChangePasswordView : WebViewClient
    {
        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            //var preferenceHandler = new PreferenceHandler();
            if (url.Contains("id_token="))
            {
                string token = Common.FunGetValuefromQueryString(url, "id_token");
                PreferenceHandler.SetToken(token);
                //view.Context.StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                Intent intent = new Intent(Application.Context, typeof(MainActivity));
                intent.PutExtra(MainActivity.KEY_USER_ROLE, (int)Constants.USER_ROLE.STUDENT);
                view.Context.StartActivity(intent);
            }
            if (url.Contains("error="))
            {
                //Utils.Utils.ShowToast(view.Context, "Failed to change password.Please try again later.");
                Intent intent = new Intent(Application.Context, typeof(AdminDashboardActivity));
                //intent.PutExtra(MainActivity.KEY_USER_ROLE, (int)Constants.USER_ROLE.STUDENT);
                view.Context.StartActivity(intent);
                view.Dispose();
            }
        }

    }

}
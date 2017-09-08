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
using EM_PORTABLE.Droid.Utils;
using Android.Support.V7.App;
using Newtonsoft.Json;
using System.Threading.Tasks;
using EM_PORTABLE.Models;
using EM_PORTABLE.Utils;
using System.Net.Http;
using Android.Webkit;
using Android.Graphics;
using Android.Content.PM;

namespace EM_PORTABLE.Droid.UI
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
        public LinearLayout layoutProgress;
        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            if (url.Contains("id_token="))
            {
                string token = Common.FunGetValuefromQueryString(url, "id_token");
                PreferenceHandler.SetToken(token);
                Intent intent = new Intent(Application.Context, typeof(MainActivity));
                intent.PutExtra(MainActivity.KEY_USER_ROLE, (int)Constants.USER_ROLE.STUDENT);
                view.Context.StartActivity(intent);
            }
            if (url.Contains("error="))
            {
                Intent intent = new Intent(Application.Context, typeof(AdminDashboardActivity));
                view.Context.StartActivity(intent);
                view.Dispose();
            }
            if (layoutProgress != null)
            {
                layoutProgress.Visibility = ViewStates.Gone;
            }
        }

        public override void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
            layoutProgress = view.FindViewById<LinearLayout>(Resource.Id.layout_progress);
            layoutProgress.Visibility = ViewStates.Visible;
            layoutProgress.Enabled = true;
        }
    }

}
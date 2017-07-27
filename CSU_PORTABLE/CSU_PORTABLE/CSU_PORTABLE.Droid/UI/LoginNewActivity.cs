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
using Microsoft.Identity.Client;
using Android.Webkit;
using Android.Graphics;
using CSU_PORTABLE.Utils;
using System.Net.Http;
using Newtonsoft.Json;
using CSU_PORTABLE.Droid.Utils;
using CSU_PORTABLE.Models;
using static CSU_PORTABLE.Utils.Constants;
using Android.Content.PM;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "CSU APP", MainLauncher = false, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
    public class LoginNewActivity : Activity
    {
        private WebView localWebView;
        public static bool IsSuccess = false;
        public Context localContext;
        public static string KEY_SHOW_PAGE = "SIGNUP";
        private SignInType signInType;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            localContext = this;
            base.OnCreate(savedInstanceState);
            if (!Utils.Utils.IsNetworkEnabled(this))
            {
                Utils.Utils.ShowDialog(this, "Internet not available.");
                StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                Finish();
            }
            else
            {
                if (Intent.Extras != null)
                {
                    foreach (var key in Intent.Extras.KeySet())
                    {
                        if (key.Equals(KEY_SHOW_PAGE))
                        {
                            signInType = (SignInType)Intent.Extras.GetInt(key);
                        }

                    }
                }

                string strLogin = string.Format(B2CConfig.AuthorizeURL, B2CConfig.Tenant, (signInType == SignInType.SIGN_IN ? B2CPolicy.SignInPolicyId : B2CPolicy.SignUpPolicyId), B2CConfig.ClientId, B2CConfig.Redirect_Uri);
                SetContentView(Resource.Layout.LoginNew);
                localWebView = FindViewById<WebView>(Resource.Id.LocalWebView);
                localWebView.SetWebViewClient(new MyWebView()); // stops request going to Web Browser
                localWebView.ClearCache(true);
                localWebView.Settings.JavaScriptEnabled = true;
                localWebView.LoadUrl(strLogin);
            }
        }

    }

    public class MyWebView : WebViewClient
    {

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);

        }

        public override async void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
            if (url.Contains("&code="))
            {
                Intent intent = new Intent(Application.Context, typeof(AdminDashboardActivity));
                intent.PutExtra(MainActivity.KEY_USER_ROLE, (int)Constants.USER_ROLE.STUDENT);
                view.Context.StartActivity(intent);
                string code = Common.FunGetValuefromQueryString(url, "code");
                PreferenceHandler.SetAccessCode(code);
                PreferenceHandler.setLoggedIn(true);
                
            }
        }
    }
}
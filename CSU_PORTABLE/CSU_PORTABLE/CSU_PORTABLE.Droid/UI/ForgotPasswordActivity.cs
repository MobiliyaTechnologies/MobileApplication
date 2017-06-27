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
using Android.Support.V7.App;
using Android.Util;
using CSU_PORTABLE.Droid.Utils;
using CSU_PORTABLE.Models;
using Newtonsoft.Json;
using CSU_PORTABLE.Utils;
using System.Net.Http;
using Android.Webkit;
using Android.Content.PM;

namespace CSU_PORTABLE.Droid.UI
{

    [Activity(Label = "Forgot Password", MainLauncher = false, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
    class ForgotPasswordActivity : AppCompatActivity
    {
        const string TAG = "MainActivity";
        private WebView localWebView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Forgot_password_view);
            string strLogin = string.Format(B2CConfig.ChangePasswordURL, B2CConfig.Tenant, B2CPolicy.ChangePasswordPolicyId, B2CConfig.ClientId, B2CConfig.Redirect_Uri);
            SetContentView(Resource.Layout.LoginNew);
            localWebView = FindViewById<WebView>(Resource.Id.LocalWebView);

            localWebView.SetWebViewClient(new ChangePasswordView()); // stops request going to Web Browser
            localWebView.Settings.JavaScriptEnabled = true;
            localWebView.LoadUrl(strLogin);
        }


    }
}
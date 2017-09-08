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
using EM_PORTABLE.Utils;
using System.Net.Http;
using Newtonsoft.Json;
using EM_PORTABLE.Droid.Utils;
using EM_PORTABLE.Models;
using static EM_PORTABLE.Utils.Constants;
using Android.Content.PM;
using System.Net;
using System.Threading.Tasks;

namespace EM_PORTABLE.Droid.UI
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
        public LinearLayout layoutProgress;
        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            if (layoutProgress != null && !url.Contains("&code="))
            {
                layoutProgress.Visibility = ViewStates.Gone;
            }
        }

        public override async void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
            layoutProgress = view.FindViewById<LinearLayout>(Resource.Id.layout_progress);
            layoutProgress.Visibility = ViewStates.Visible;
            layoutProgress.Enabled = true;
            //view.DispatchFinishTemporaryDetach();
            if (url.Contains("&code="))
            {
                string code = Common.FunGetValuefromQueryString(url, "code");
                PreferenceHandler.SetAccessCode(code);
                PreferenceHandler.setLoggedIn(true);

                string tokenURL = string.Format(B2CConfig.TokenURL, B2CConfig.Tenant, B2CPolicy.SignInPolicyId, B2CConfig.ClientId, PreferenceHandler.GetAccessCode());
                var response = await InvokeApi.Authenticate(tokenURL, string.Empty, HttpMethod.Post);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string strContent = await response.Content.ReadAsStringAsync();
                    var tokenNew = JsonConvert.DeserializeObject<AccessToken>(strContent);
                    PreferenceHandler.SetToken(tokenNew.id_token);
                    PreferenceHandler.SetRefreshToken(tokenNew.refresh_token);
                    layoutProgress.Visibility = ViewStates.Visible;
                    await GetUserDetails(view);
                }

            }
        }

        private async void GetCurrentUserResponse(HttpResponseMessage responseUser, WebView view)
        {
            if (responseUser != null && responseUser.StatusCode == System.Net.HttpStatusCode.OK && responseUser.Content != null)
            {
                string strContent = await responseUser.Content.ReadAsStringAsync();
                UserDetails user = JsonConvert.DeserializeObject<UserDetails>(strContent);
                PreferenceHandler.SaveUserDetails(user);
                Intent intent = new Intent(Application.Context, typeof(AdminDashboardActivity));
                intent.PutExtra(MainActivity.KEY_USER_ROLE, (int)Constants.USER_ROLE.STUDENT);
                view.Context.StartActivity(intent);
                layoutProgress.Visibility = ViewStates.Gone;
            }
        }

        public async Task GetUserDetails(WebView view)
        {
            var responseUser = await InvokeApi.Invoke(Constants.API_GET_CURRENTUSER, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken());
            if (responseUser.StatusCode == HttpStatusCode.OK)
            {
                GetCurrentUserResponse(responseUser, view);
            }
        }
    }
}
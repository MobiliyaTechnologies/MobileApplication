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

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "CSU APP", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyTheme")]
    public class LoginNewActivity : Activity
    {
        private WebView localWebView;
        public static bool IsSuccess = false;
        public Context localContext;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            localContext = this;
            base.OnCreate(savedInstanceState);
            //string strLogin = "https://login.microsoftonline.com/csub2c.onmicrosoft.com/oauth2/v2.0/authorize?p=b2c_1_b2csignin&client_id=3bdf8223-746c-42a2-ba5e-0322bfd9ff76&response_type=code&redirect_uri=http://localhost:65328&response_mode=query&scope=openid&state=arbitrary_data_you_can_receive_in_the_response";
            string strLogin = string.Format(B2CConfig.AuthorizeURL, B2CConfig.Tenant, B2CPolicy.SignInPolicyId, B2CConfig.ClientId, B2CConfig.Redirect_Uri);
            SetContentView(Resource.Layout.LoginNew);
            localWebView = FindViewById<WebView>(Resource.Id.LocalWebView);
            localWebView.SetWebViewClient(new MyWebView()); // stops request going to Web Browser
            localWebView.Settings.JavaScriptEnabled = true;
            localWebView.LoadUrl(strLogin);
        }

       
    }

    public class MyWebView : WebViewClient
    {
        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);

        }

        public async override void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);

            if (url.Contains("&code="))
            {

                string code = Common.FunGetValuefromQueryString(url, "code");
                var preferenceHandler = new PreferenceHandler();
                preferenceHandler.SetAccessCode(code);

                string tokenURL = string.Format(B2CConfig.TokenURL, B2CConfig.Tenant, B2CPolicy.SignInPolicyId, B2CConfig.Grant_type, B2CConfig.ClientSecret, B2CConfig.ClientId, code);
                var response = await InvokeApi.Authenticate(tokenURL, string.Empty, HttpMethod.Post);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string strContent = await response.Content.ReadAsStringAsync();
                    var token = JsonConvert.DeserializeObject<AccessToken>(strContent);
                    preferenceHandler.SetToken(token.id_token);

                }

                //view.Context.StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                Intent intent = new Intent(Application.Context, typeof(MainActivity));
                intent.PutExtra(MainActivity.KEY_USER_ROLE, (int)Constants.USER_ROLE.STUDENT);
                view.Context.StartActivity(intent);


            }
        }

    }
}
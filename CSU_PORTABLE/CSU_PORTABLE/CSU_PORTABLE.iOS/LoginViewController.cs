﻿using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using SafariServices;
using System;
using System.Net.Http;
using UIKit;
using WebKit;

namespace CSU_PORTABLE.iOS
{
    public partial class LoginViewController : BaseController
    {
        private UIWebView webView;
        PreferenceHandler preferenceHandler;
        UserDetails userDetails;
        private LoadingOverlay loadingOverlay;

        public LoginViewController(IntPtr handle) : base(handle)
        {
            preferenceHandler = new PreferenceHandler();
            userDetails = preferenceHandler.GetUserDetails();
        }



        public override void ViewDidLoad()
        {
            webView = new UIWebView(View.Bounds);
            var wkConfig = new WKWebViewConfiguration();
            webView.ScalesPageToFit = false;
            View.AddSubview(webView);
            string strLogin = string.Format(B2CConfig.AuthorizeURL, B2CConfig.Tenant, B2CPolicy.SignInPolicyId, B2CConfig.ClientId, B2CConfig.Redirect_Uri);
            NSUrlRequest request = new NSUrlRequest(new NSUrl(strLogin));

            //NSUrlRequest request = new NSUrlRequest(new NSUrl("https://login.microsoftonline.com/CSUB2C.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_b2cSignup&client_Id=3bdf8223-746c-42a2-ba5e-0322bfd9ff76&nonce=defaultNonce&redirect_uri=com.onmicrosoft.csu%3A%2F%2Fiosresponse%2F&scope=openid&response_type=id_token&prompt=login"));
            //NSUrlRequest request = new NSUrlRequest(new NSUrl("https://login.microsoftonline.com/CSUB2C.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_b2cSignin&client_Id=3bdf8223-746c-42a2-ba5e-0322bfd9ff76&nonce=defaultNonce&redirect_uri=com.onmicrosoft.csu%3A%2F%2Fiosresponse%2F&scope=openid&response_type=id_token&prompt=login"));
            //NSUrlRequest request = new NSUrlRequest(new NSUrl("https://login.microsoftonline.com/CSUB2C.onmicrosoft.com/oauth2/v2.0/logout?p=b2c_1_sign_in&post_logout_redirect_uri=com.onmicrosoft.csu://iosresponse/"));
            //webView.UserInteractionEnabled = true;
            // Action ResetSession = () =>
            //{

            //};
            // NSUrlSession.SharedSession.Reset(ResetSession);
            webView.LoadRequest(request);
            webView.LoadError += WebView_LoadError;

        }

        private async void WebView_LoadError(object sender, UIWebErrorArgs e)
        {
            var URL = (NSObject)e.Error.UserInfo.Values[2];

            string req = URL.ToString();

            if (req.Contains("&code="))
            {
                string code = Common.FunGetValuefromQueryString(req, "code");
                string tokenURL = string.Format(B2CConfig.TokenURLIOS, B2CConfig.Tenant, B2CPolicy.SignInPolicyId, B2CConfig.Grant_type, B2CConfig.ClientId, code);
                var response = await InvokeApi.Authenticate(tokenURL, string.Empty, HttpMethod.Post);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string strContent = await response.Content.ReadAsStringAsync();
                    var token = JsonConvert.DeserializeObject<AccessToken>(strContent);
                    preferenceHandler.SetToken(token.id_token);

                }
            }

            if (req.Contains("id_token="))
            {
                string token = Common.FunGetValuefromQueryString(req, "id_token");
                preferenceHandler.SetToken(token);
            }

            var responseUser = await InvokeApi.Invoke(Constants.API_GET_CURRENTUSER, string.Empty, HttpMethod.Get, preferenceHandler.GetToken());
            if (responseUser.StatusCode != 0)
            {
                InvokeOnMainThread(() =>
                {
                    GetCurrentUserResponse(responseUser);
                });
            }

        }

        private void ShowClassRooms()
        {
            FeedbackViewController FeedbackView = this.Storyboard.InstantiateViewController("FeedbackViewController") as FeedbackViewController;
            FeedbackView.NavigationItem.SetHidesBackButton(true, false);

            this.NavController.PushViewController(FeedbackView, false);
            var menuController = (MyMenuController)Storyboard.InstantiateViewController("MyMenuController");
            SidebarController.ChangeMenuView(menuController);
            SidebarController.MenuWidth = 250;
            SidebarController.ReopenOnRotate = false;
        }

        private void ShowMap()
        {
            // Launches a new instance of CallHistoryController
            MapViewController mapView = this.Storyboard.InstantiateViewController("MapViewController") as MapViewController;
            if (mapView != null)
            {
                mapView.NavigationItem.SetHidesBackButton(true, false);
                this.NavController.PushViewController(mapView, false);
                var menuController = (MyMenuController)Storyboard.InstantiateViewController("MyMenuController");
                SidebarController.ChangeMenuView(menuController);
                SidebarController.MenuWidth = 250;
                SidebarController.ReopenOnRotate = false;
            }
        }

        private async void GetCurrentUserResponse(HttpResponseMessage responseUser)
        {
            if (responseUser != null && responseUser.StatusCode == System.Net.HttpStatusCode.OK && responseUser.Content != null)
            {
                string strContent = await responseUser.Content.ReadAsStringAsync();
                UserDetails user = JsonConvert.DeserializeObject<UserDetails>(strContent);
                preferenceHandler.SaveUserDetails(user);
                ShowDashboard(user);
            }
            else
            {
                IOSUtil.ShowMessage("User details not found", loadingOverlay, this);
            }
        }

        private void ShowDashboard(UserDetails user)
        {
            if (user.RoleId == 2)
            {
                ShowClassRooms();
            }
            else
            {
                ShowMap();
            }
        }

    }
}
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using UIKit;
using WebKit;


namespace CSU_PORTABLE.iOS
{
    public partial class LoginViewController : BaseController
    {
        private UIWebView webView;
        //PreferenceHandler preferenceHandler;
        UserDetails userDetails;
        private LoadingOverlay loadingOverlay;

        public LoginViewController(IntPtr handle) : base(handle)
        {
            userDetails = PreferenceHandler.GetUserDetails();
        }



        public override void ViewDidLoad()
        {
            webView = new UIWebView(View.Bounds);
            var wkConfig = new WKWebViewConfiguration();
            webView.ScalesPageToFit = false;
            View.AddSubview(webView);
            string strLogin = string.Format(B2CConfig.AuthorizeURL, B2CConfig.Tenant, B2CPolicy.SignInPolicyId, B2CConfig.ClientId, B2CConfig.Redirect_Uri);
            NSUrlRequest request = new NSUrlRequest(new NSUrl(Uri.EscapeUriString(strLogin)));
            webView.LoadRequest(request);
            webView.LoadError += WebView_LoadError;
            webView.LoadStarted += WebView_LoadStarted;
            webView.LoadFinished += WebView_LoadFinished;
        }

        private void WebView_LoadFinished(object sender, EventArgs e)
        {
            if (loadingOverlay != null)
            {
                loadingOverlay.Hide();
            }
        }

        private void WebView_LoadStarted(object sender, EventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                var bounds = UIScreen.MainScreen.Bounds;
                loadingOverlay = new LoadingOverlay(bounds);
                View.Add(loadingOverlay);
            });
        }

        private async void WebView_LoadError(object sender, UIWebErrorArgs e)
        {
            var URL = (NSObject)e.Error.UserInfo.Values[2];

            string req = URL.ToString();

            if (req.Contains("&code="))
            {
                string code = Common.FunGetValuefromQueryString(req, "code");
                PreferenceHandler.SetAccessCode(code);
                string tokenURL = string.Format(B2CConfig.TokenURLIOS, B2CConfig.Tenant, B2CPolicy.SignInPolicyId, B2CConfig.Grant_type, B2CConfig.ClientId, code);
                var response = await InvokeApi.Authenticate(tokenURL, string.Empty, HttpMethod.Post);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string strContent = await response.Content.ReadAsStringAsync();
                    var token = JsonConvert.DeserializeObject<AccessToken>(strContent);
                    PreferenceHandler.SetToken(token.id_token);
                    PreferenceHandler.SetRefreshToken(token.refresh_token);
                }
            }

            if (req.Contains("id_token="))
            {
                string token = Common.FunGetValuefromQueryString(req, "id_token");
                PreferenceHandler.SetToken(token);
                //PreferenceHandler.SetRefreshToken(token.refresh_token);
            }

            var responseUser = await InvokeApi.Invoke(Constants.API_GET_CURRENTUSER, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken());
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
                PreferenceHandler.SaveUserDetails(user);
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
            if (loadingOverlay != null)
            {
                loadingOverlay.Hide();
            }
        }

    }
}

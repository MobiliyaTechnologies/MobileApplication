using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using System;
using UIKit;
using System.Net.Http;
using SidebarNavigation;

namespace CSU_PORTABLE.iOS
{
    public partial class ChangePasswordController : BaseController
    {
       // PreferenceHandler prefHandler = null;
        UserDetails User = null;
        LoadingOverlay loadingOverlay;
        private UIWebView webView;
        public ChangePasswordController(IntPtr handle) : base(handle)
        {
            //this.prefHandler = new PreferenceHandler();
            User = PreferenceHandler.GetUserDetails();

        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            webView = new UIWebView(View.Bounds);
            webView.ScalesPageToFit = false;
            View.AddSubview(webView);
            NSUrlRequest request = new NSUrlRequest(new NSUrl("https://login.microsoftonline.com/CSUB2C.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_b2cSSPR&client_Id=3bdf8223-746c-42a2-ba5e-0322bfd9ff76&nonce=defaultNonce&redirect_uri=com.onmicrosoft.csu://iosresponse/&scope=openid&response_type=id_token&prompt=login"));
            webView.LoadRequest(request);
            webView.LoadError += WebView_LoadError;
        }

        private void WebView_LoadError(object sender, UIWebErrorArgs e)
        {
            var URL = (NSObject)e.Error.UserInfo.Values[2];
            string req = URL.ToString();
            if (req.Contains("id_token="))
            {
                string token = Common.FunGetValuefromQueryString(req, "id_token");
                PreferenceHandler.SetToken(token);
            }
            else
            {
                IOSUtil.ShowAlert("Failed to change password.Please try again later.");

            }
            var ViewController = (ViewController)Storyboard.InstantiateViewController("ViewController");
            ViewController.NavigationItem.SetHidesBackButton(true, false);
            NavController.PushViewController(ViewController, false);
            SidebarController.MenuWidth = 0;
            SidebarController.CloseMenu();
        }
        
    }
}
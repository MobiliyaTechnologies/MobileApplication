using Foundation;
using System;
using UIKit;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Newtonsoft.Json;
using CSU_PORTABLE.iOS.Utils;
using System.Net.Http;

namespace CSU_PORTABLE.iOS
{
    public partial class ForgotPasswordController : BaseController
    {
        LoadingOverlay loadingOverlay;
        private UIWebView webView;
        //PreferenceHandler prefHandler = null;
        UserDetails User = null;
        public ForgotPasswordController(IntPtr handle) : base(handle)
        {
            //this.prefHandler = new PreferenceHandler();
            User = PreferenceHandler.GetUserDetails();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            base.ViewDidLoad();
            webView = new UIWebView(View.Bounds);
            webView.ScalesPageToFit = false;
            View.AddSubview(webView);
            NSUrlRequest request = new NSUrlRequest(new NSUrl("https://login.microsoftonline.com/CSUB2C.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_b2cSSPR&client_Id=3bdf8223-746c-42a2-ba5e-0322bfd9ff76&nonce=defaultNonce&redirect_uri=com.onmicrosoft.csu://iosresponse/&scope=openid&response_type=id_token&prompt=login"));
            webView.LoadRequest(request);
            webView.LoadError += WebView_LoadError;
            //this.NavigationController.NavigationBarHidden = false;
            //this.NavigationController.NavigationBar.TintColor = UIColor.White;
            //this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(33, 77, 43);
            //this.NavigationController.NavigationBar.BarStyle = UIBarStyle.BlackTranslucent;
            //Email.AutocorrectionType = UITextAutocorrectionType.No;
            //Email.ShouldReturn = delegate
            //{
            //    // Changed this slightly to move the text entry to the next field.
            //    Email.ResignFirstResponder();
            //    return true;
            //};
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

        //partial void ForgotButton_TouchUpInside(UIButton sender)
        //{
        //    // Added for showing loading screen
        //    var bounds = UIScreen.MainScreen.Bounds;
        //    // show the loading overlay on the UI thread using the correct orientation sizing
        //    loadingOverlay = new LoadingOverlay(bounds);
        //    View.Add(loadingOverlay);

        //    if (string.IsNullOrEmpty(Email.Text.Trim()))
        //    {
        //        loadingOverlay.Hide();
        //        IOSUtil.ShowMessage("Enter valid Email.", loadingOverlay, this);
        //    }
        //    else
        //    {
        //        SubmitEmail(new ForgotPasswordModel(Email.Text.Trim()));
        //    }
        //}

        //public async void SubmitEmail(ForgotPasswordModel objModel)
        //{
        //    var response = await InvokeApi.Invoke(Constants.API_FORGOT_PASSWORD, JsonConvert.SerializeObject(objModel), HttpMethod.Post);
        //    if (response.StatusCode != 0)
        //    {
        //        InvokeOnMainThread(() =>
        //        {
        //            ForgotPasswordResponse(response);
        //            loadingOverlay.Hide();
        //        });
        //    }
        //}

        //private async void ForgotPasswordResponse(HttpResponseMessage restResponse)
        //{
        //    if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
        //    {
        //        string strContent = await restResponse.Content.ReadAsStringAsync();
        //        ForgotPasswordResponseModel response = JsonConvert.DeserializeObject<ForgotPasswordResponseModel>(strContent);
        //        if (response != null && response.Status_Code == Constants.STATUS_CODE_SUCCESS)
        //        {

        //            PreferenceHandler preferenceHandler = new PreferenceHandler();
        //            preferenceHandler.setLoggedIn(false);
        //            IOSUtil.ShowMessage("Please check your Email.", loadingOverlay, this);
        //        }
        //        else
        //        {
        //            IOSUtil.ShowMessage("Please try again.", loadingOverlay, this);
        //        }
        //    }
        //    else
        //    {
        //        IOSUtil.ShowMessage("Please try again.", loadingOverlay, this);
        //    }
        //}
    }
}
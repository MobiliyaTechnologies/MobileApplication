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
        PreferenceHandler prefHandler = null;
        UserDetails User = null;
        LoadingOverlay loadingOverlay;
        private UIWebView webView;
        public ChangePasswordController(IntPtr handle) : base(handle)
        {
            this.prefHandler = new PreferenceHandler();
            User = prefHandler.GetUserDetails();

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
            //Username.Text = User.Email;

            //Password.AutocorrectionType = UITextAutocorrectionType.No;
            //ConfirmPassword.AutocorrectionType = UITextAutocorrectionType.No;
            //Password.ShouldReturn = delegate
            //{
            //    // Changed this slightly to move the text entry to the next field.
            //    ConfirmPassword.BecomeFirstResponder();
            //    return true;
            //};
            //ConfirmPassword.ShouldReturn = delegate
            //{
            //    // Changed this slightly to move the text entry to the next field.
            //    ConfirmPassword.ResignFirstResponder();
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
                prefHandler.SetToken(token);
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

        //partial void Submit_TouchUpInside(UIButton sender)
        //{
        //    // Added for showing loading screen
        //    var bounds = UIScreen.MainScreen.Bounds;
        //    // show the loading overlay on the UI thread using the correct orientation sizing
        //    loadingOverlay = new LoadingOverlay(bounds);
        //    View.Add(loadingOverlay);

        //    Submit.Enabled = false;

        //    if (string.IsNullOrEmpty(User.Email))
        //    {
        //        IOSUtil.ShowMessage("Enter valid Email.", loadingOverlay, this);
        //    }
        //    else
        //    {
        //        string password = Password.Text.Trim();
        //        string confirmPassword = ConfirmPassword.Text.Trim();
        //        if (password != null && password.Length > 2)
        //        {
        //            if (confirmPassword != null && confirmPassword.Length > 2)
        //            {
        //                ChangePasswordModel model = new ChangePasswordModel();
        //                model.Email = User.Email;
        //                model.Password = password;
        //                model.New_Password = confirmPassword;
        //                ChangePassword(model);
        //            }
        //            else
        //            {
        //                IOSUtil.ShowMessage("Enter valid confirm password", loadingOverlay, this);
        //            }
        //        }
        //        else
        //        {
        //            IOSUtil.ShowMessage("Enter valid password", loadingOverlay, this);
        //        }
        //    }
        //    Submit.Enabled = true;
        //}



        //#region "Custom Functions"

        //private async void ChangePassword(ChangePasswordModel model)
        //{
        //    var response = await InvokeApi.Invoke(Constants.API_CHANGE_PASSWORD, JsonConvert.SerializeObject(model), HttpMethod.Post);
        //    if (response.StatusCode != 0)
        //    {
        //        InvokeOnMainThread(() =>
        //        {
        //            ChangePasswordResponse(response);
        //        });
        //    }
        //}

        //private async void ChangePasswordResponse(HttpResponseMessage restResponse)
        //{
        //    if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
        //    {
        //        string strContent = await restResponse.Content.ReadAsStringAsync();
        //        ChangePasswordResponseModel response = JsonConvert.DeserializeObject<ChangePasswordResponseModel>(strContent);

        //        if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
        //        {
        //            IOSUtil.ShowMessage("Password Changed Successfully. Please check your Email.", loadingOverlay, this);
        //        }
        //        else
        //        {
        //            IOSUtil.ShowMessage("Failed to change password, Plesase try again later.", loadingOverlay, this);
        //        }
        //    }
        //    else
        //    {
        //        IOSUtil.ShowMessage("Error in changing password. Please try again.", loadingOverlay, this);
        //    }
        //}
        //#endregion
    }
}
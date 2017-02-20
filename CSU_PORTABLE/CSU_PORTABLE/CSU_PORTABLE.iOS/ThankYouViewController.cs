using CoreGraphics;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using RestSharp;
using SidebarNavigation;
using System;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class ThankYouViewController : BaseController
    {

        UILabel ThankYouHeader, ThankYouSubHeader;

        public ThankYouViewController(IntPtr handle) : base(handle)
        {
        }

        //// provide access to the navigation controller to all inheriting controllers
        //protected NavController NavController
        //{
        //    get
        //    {
        //        return (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.NavController;
        //    }
        //}

        //// provide access to the storyboard to all inheriting controllers
        //public override UIStoryboard Storyboard
        //{
        //    get
        //    {
        //        return (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.Storyboard;
        //    }
        //}




        #region " Events "

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            GetThankYouView();

        }

        private void BtnDone_TouchUpInside(object sender, EventArgs e)
        {
            var FeedbackViewController = Storyboard.InstantiateViewController("FeedbackViewController") as FeedbackViewController;
            FeedbackViewController.NavigationItem.SetHidesBackButton(true, false);
            NavController.PushViewController(FeedbackViewController, true);
        }

        #endregion

        #region " Custom Functions "

        private void GetThankYouView()
        {
            this.View.BackgroundColor = UIColor.FromRGB(30, 77, 43);
            ThankYouHeader = new UILabel()
            {
                Font = UIFont.FromName("Futura-Medium", 18f),
                TextColor = UIColor.White,
                BackgroundColor = UIColor.Clear,
                Frame = new CGRect(50, 250, View.Bounds.Width - 120, 100),
                Text = "Thank You so much for your Valueable feedback.",
                TextAlignment = UITextAlignment.Center,
                Lines = 3,
                LineBreakMode = UILineBreakMode.WordWrap,
                AutosizesSubviews = true
            };


            UIButton btnDone = new UIButton(UIButtonType.Custom);
            btnDone.SetTitle("Done", UIControlState.Normal);
            btnDone.Font = UIFont.FromName("Futura-Medium", 15f);
            btnDone.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Normal);
            btnDone.SetTitleColor(UIColor.Green, UIControlState.Focused);
            btnDone.TouchUpInside += BtnDone_TouchUpInside;
            btnDone.Frame = new CGRect((View.Bounds.Width / 2) - 40, 450, 80, 40);
            btnDone.BackgroundColor = UIColor.White;

            //UIButton btnLogout = new UIButton(UIButtonType.Custom);
            //btnLogout.SetTitle("Logout", UIControlState.Normal);
            //btnLogout.Font = UIFont.FromName("Futura-Medium", 15f);
            //btnLogout.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Normal);
            //btnLogout.SetTitleColor(UIColor.Green, UIControlState.Focused);
            //btnLogout.TouchUpInside += BtnLogout_TouchUpInside;
            //btnLogout.Frame = new CGRect((View.Bounds.Width / 2) + 20, 550, 80, 40);
            //btnLogout.BackgroundColor = UIColor.White;

            View.AddSubviews(ThankYouHeader, btnDone);
        }

        //private void BtnLogout_TouchUpInside(object sender, EventArgs e)
        //{
        //    PreferenceHandler prefs = new PreferenceHandler();
        //    Logout(new LogoutModel(prefs.GetUserDetails().Email));
        //}

        //private void Logout(LogoutModel logoutModel)
        //{


        //    RestClient client = new RestClient(Constants.SERVER_BASE_URL);

        //    var request = new RestRequest(Constants.API_SIGN_OUT, Method.POST);
        //    request.RequestFormat = DataFormat.Json;
        //    request.AddBody(logoutModel);

        //    client.ExecuteAsync(request, response =>
        //    {
        //        Console.WriteLine(response);
        //        if (response.StatusCode != 0)
        //        {
        //            InvokeOnMainThread(() =>
        //            {
        //                LogoutResponse((RestResponse)response);
        //            });
        //        }
        //    });
        //}

        //private void LogoutResponse(RestResponse restResponse)
        //{


        //    if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
        //    {
        //        GeneralResponseModel response = JsonConvert.DeserializeObject<GeneralResponseModel>(restResponse.Content);

        //        if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
        //        {
        //            PreferenceHandler preferenceHandler = new PreferenceHandler();
        //            preferenceHandler.setLoggedIn(false);

        //            var ViewController = (ViewController)Storyboard.InstantiateViewController("ViewController");
        //            ViewController.NavigationItem.SetHidesBackButton(true, false);
        //            NavController.PushViewController(ViewController, false);
        //        }
        //        else
        //        {
        //            ShowMessage("Failed to logout, Please try later.");
        //        }
        //    }
        //    else
        //    {
        //        ShowMessage("Failed to logout, Please try later.");
        //    }
        //}

        //private void ShowMessage(string v)
        //{
        //    UIAlertController alertController = UIAlertController.Create("Message", v, UIAlertControllerStyle.Alert);
        //    alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));
        //    PresentViewController(alertController, true, null);
        //}

        #endregion
    }
}
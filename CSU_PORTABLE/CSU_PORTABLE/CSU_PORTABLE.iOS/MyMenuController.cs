using CoreGraphics;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class MyMenuController : BaseController
    {
        #region " Variables "

        LoadingOverlay loadingOverlay;
        PreferenceHandler prefHandler;
        UserDetails userdetail;

        #endregion


        public MyMenuController(IntPtr handle) : base(handle)
        {
        }





        #region " Events "

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            GenerateMenu();
        }

        private void AlertsButton_TouchUpInside(object sender, EventArgs e)
        {
            var AlertsViewController = (AlertsViewController)Storyboard.InstantiateViewController("AlertsViewController");
            NavController.PushViewController(AlertsViewController, false);
            SidebarController.CloseMenu();
        }

        private void ReportsButton_TouchUpInside(object sender, EventArgs e)
        {
            var ReportController = (ReportController)Storyboard.InstantiateViewController("ReportController");
            NavController.PushViewController(ReportController, false);
            SidebarController.CloseMenu();
        }

        private void ChangePasswordButton_TouchUpInside(object sender, EventArgs e)
        {
            var ChangePasswordController = (ChangePasswordController)Storyboard.InstantiateViewController("ChangePasswordController");
            NavController.PushViewController(ChangePasswordController, false);
            SidebarController.CloseMenu();
        }

        private void LogOutButton_TouchUpInside(object sender, EventArgs e)
        {
            // Added for showing loading screen
            var bounds = UIScreen.MainScreen.Bounds;
            // show the loading overlay on the UI thread using the correct orientation sizing
            loadingOverlay = new LoadingOverlay(bounds);
            View.Add(loadingOverlay);


            PreferenceHandler preferenceHandler = new PreferenceHandler();
            preferenceHandler.setLoggedIn(false);

            Action ResetSession = () =>
           {

           };
            NSUrlSession.SharedSession.Reset(ResetSession);

            var ViewController = (ViewController)Storyboard.InstantiateViewController("ViewController");
            ViewController.NavigationItem.SetHidesBackButton(true, false);
            NavController.PushViewController(ViewController, false);
            SidebarController.MenuWidth = 0;
            SidebarController.CloseMenu();
            loadingOverlay.Hide();

            Logout(new LogoutModel(preferenceHandler.GetUserDetails().Email));

        }

        private void InsightsButton_TouchUpInside(object sender, EventArgs e)
        {
            var InsightsViewController = (InsightsViewController)Storyboard.InstantiateViewController("InsightsViewController");
            NavController.PushViewController(InsightsViewController, false);
            SidebarController.CloseMenu();
        }

        #endregion

        #region " Functions "


        public void GenerateMenu()
        {

            prefHandler = new PreferenceHandler();
            userdetail = prefHandler.GetUserDetails();

            double profileRadius = 100;
            double profileViewHeight = 230;

            UIView viewProfile = new UIView(new CGRect(0, 0, View.Bounds.Width, profileViewHeight));
            viewProfile.BackgroundColor = UIColor.FromRGB(33, 77, 43);

            UIImageView imgProfile = new UIImageView()
            {
                Frame = new CGRect(125 - (profileRadius / 2), 40, profileRadius, profileRadius),
                Image = UIImage.FromBundle("Logo_01.png"),

            };
            imgProfile.ClipsToBounds = true;
            imgProfile.Layer.CornerRadius = (float)profileRadius / 2;
            imgProfile.Layer.BorderColor = UIColor.White.CGColor;
            imgProfile.Layer.BorderWidth = 2;



            UILabel lblProfileName = new UILabel()
            {
                Frame = new CGRect(0, 140, 260, 50),
                Font = UIFont.FromName("Futura-Medium", 20f),
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Center,
                Text = userdetail.FirstName + " " + userdetail.LastName,
                TextColor = UIColor.White,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,

            };

            UIButton LogOutButton = new UIButton()
            {
                Font = UIFont.FromName("Futura-Medium", 14f),
                BackgroundColor = UIColor.Clear,
                Frame = new CGRect(60, 190, 130, 25)
            };
            LogOutButton.SetTitle("Logout", UIControlState.Normal);
            LogOutButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            LogOutButton.SetTitleColor(UIColor.White, UIControlState.Selected);
            LogOutButton.Layer.BorderColor = UIColor.White.CGColor;
            LogOutButton.Layer.BorderWidth = 1f;
            LogOutButton.Layer.CornerRadius = 13;
            viewProfile.AddSubviews(lblProfileName, imgProfile, LogOutButton);


            //UIView seperator = new UIView()
            //{
            //    Frame = new CGRect(0, 41, viewProfile.Bounds.Width, 1),
            //    BackgroundColor = UIColor.DarkTextColor
            //};

            UIButton ReportsButton = new UIButton()
            {
                Frame = new CGRect(0, profileViewHeight, 250, 40),
                Font = UIFont.FromName("Futura-Medium", 14f),
                BackgroundColor = UIColor.Clear,
                //HorizontalAlignment = UIControlContentHorizontalAlignment.Left
            };
            ReportsButton.SetTitle("REPORTS", UIControlState.Normal);
            ReportsButton.SetTitleColor(UIColor.DarkTextColor, UIControlState.Normal);
            ReportsButton.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Selected);
            ReportsButton.BackgroundColor = UIColor.LightTextColor;
            ReportsButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            //ReportsButton.Layer.BorderColor = UIColor.DarkGray.CGColor;
            //ReportsButton.Layer.BorderWidth = 1f;

            //View.InsertSubviewBelow(ReportsButton, seperator);

            UIButton AlertsButton = new UIButton()
            {
                Font = UIFont.FromName("Futura-Medium", 14f),
                BackgroundColor = UIColor.Clear
            };
            AlertsButton.SetTitle("ALERTS", UIControlState.Normal);
            AlertsButton.SetTitleColor(UIColor.DarkTextColor, UIControlState.Normal);
            AlertsButton.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Selected);

            UIButton InsightsButton = new UIButton()
            {
                Font = UIFont.FromName("Futura-Medium", 14f),
                BackgroundColor = UIColor.Clear
            };
            InsightsButton.SetTitle("INSIGHTS", UIControlState.Normal);
            InsightsButton.SetTitleColor(UIColor.DarkTextColor, UIControlState.Normal);
            InsightsButton.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Selected);


            UIButton ChangePasswordButton = new UIButton()
            {
                Font = UIFont.FromName("Futura-Medium", 14f),
                BackgroundColor = UIColor.Clear
            };
            ChangePasswordButton.SetTitle("CHANGE PASSWORD", UIControlState.Normal);
            ChangePasswordButton.SetTitleColor(UIColor.DarkTextColor, UIControlState.Normal);
            ChangePasswordButton.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Selected);



            InsightsButton.TouchUpInside += InsightsButton_TouchUpInside;
            ChangePasswordButton.TouchUpInside += ChangePasswordButton_TouchUpInside;
            LogOutButton.TouchUpInside += LogOutButton_TouchUpInside;
            ReportsButton.TouchUpInside += ReportsButton_TouchUpInside;
            AlertsButton.TouchUpInside += AlertsButton_TouchUpInside;


            UIView seperator = new UIView()
            {
                Frame = new CGRect(0, 41, viewProfile.Bounds.Width, 1),
                BackgroundColor = UIColor.LightGray
            };
            UIView seperatorAlerts = new UIView()
            {
                Frame = new CGRect(0, 41, viewProfile.Bounds.Width, 1),
                BackgroundColor = UIColor.LightGray
            };
            UIView seperatorPassword = new UIView()
            {
                Frame = new CGRect(0, 41, viewProfile.Bounds.Width, 1),
                BackgroundColor = UIColor.LightGray
            };
            UIView seperatorInsights = new UIView()
            {
                Frame = new CGRect(0, 41, viewProfile.Bounds.Width, 1),
                BackgroundColor = UIColor.LightGray
            };

            ReportsButton.InsertSubview(seperator, 1);
            ChangePasswordButton.InsertSubview(seperatorPassword, 1);
            AlertsButton.InsertSubview(seperatorAlerts, 1);
            InsightsButton.InsertSubview(seperatorInsights, 1);

            if (userdetail.RoleId == 2)
            {
                ChangePasswordButton.Frame = new CGRect(0, profileViewHeight + 40, 250, 40);
                View.AddSubviews(viewProfile, ChangePasswordButton, LogOutButton);
            }
            else
            {
                AlertsButton.Frame = new CGRect(0, profileViewHeight + 40, 250, 40);
                InsightsButton.Frame = new CGRect(0, profileViewHeight + 80, 250, 40);
                ChangePasswordButton.Frame = new CGRect(0, profileViewHeight + 120, 250, 40);
                View.AddSubviews(viewProfile, ChangePasswordButton, ReportsButton, AlertsButton, InsightsButton);
            }
        }



        private async void Logout(LogoutModel logoutModel)
        {
            var response = await InvokeApi.Invoke(Constants.API_SIGN_OUT, JsonConvert.SerializeObject(logoutModel), HttpMethod.Post);
            //if (response.StatusCode != 0)
            //{
            //    InvokeOnMainThread(() =>
            //    {
            //        LogoutResponse(response);
            //    });
            //}
        }

        //private void LogoutResponse(HttpResponseMessage restResponse)
        //{
        //    if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
        //    {
        //    }

        //}

        //private void ShowMessage(string v)
        //{
        //    loadingOverlay.Hide();
        //    UIAlertController alertController = UIAlertController.Create("Message", v, UIAlertControllerStyle.Alert);
        //    alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));
        //    PresentViewController(alertController, true, null);
        //}

        #endregion
    }
}
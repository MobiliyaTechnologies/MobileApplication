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
        //PreferenceHandler prefHandler;
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
            NavController.PushViewController(AlertsViewController, true);
            SidebarController.CloseMenu();
        }

        private void DashboardButton_TouchUpInside(object sender, EventArgs e)
        {
            if (PreferenceHandler.GetUserDetails().RoleId == 1)
            {
                var MapViewController = (MapViewController)Storyboard.InstantiateViewController("MapViewController");
                NavController.PushViewController(MapViewController, true);
            }
            else
            {
                FeedbackViewController FeedbackView = this.Storyboard.InstantiateViewController("FeedbackViewController") as FeedbackViewController;
                FeedbackView.NavigationItem.SetHidesBackButton(true, false);
                this.NavController.PushViewController(FeedbackView, true);
            }
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
            PreferenceHandler.setLoggedIn(false);

            Action ResetSession = () =>
           {

           };
            NSUrlSession.SharedSession.Reset(ResetSession);
            var ViewController = (ViewController)Storyboard.InstantiateViewController("ViewController");
            ViewController.NavigationItem.SetHidesBackButton(true, false);
            NavController.PushViewController(ViewController, true);
            SidebarController.MenuWidth = 0;
            SidebarController.CloseMenu();
            loadingOverlay.Hide();
            Logout(new LogoutModel(PreferenceHandler.GetUserDetails().Email));

        }

        private void InsightsButton_TouchUpInside(object sender, EventArgs e)
        {
            var InsightsViewController = (InsightsViewController)Storyboard.InstantiateViewController("InsightsViewController");
            NavController.PushViewController(InsightsViewController, true);
            SidebarController.CloseMenu();
        }

        #endregion

        #region " Functions "


        public void GenerateMenu()
        {

            //prefHandler = new PreferenceHandler();
            userdetail = PreferenceHandler.GetUserDetails();

            double profileRadius = 100;
            double profileViewHeight = 230;

            UIView viewProfile = new UIView(new CGRect(0, 0, View.Bounds.Width, profileViewHeight));
            viewProfile.BackgroundColor = UIColor.FromRGB(0, 102, 153);

            UIImageView imgProfile = new UIImageView()
            {
                Frame = new CGRect(125 - (profileRadius / 2), 40, profileRadius, profileRadius),
                Image = UIImage.FromBundle("logo.png"),

            };
            imgProfile.ClipsToBounds = true;
            imgProfile.Layer.CornerRadius = (float)profileRadius / 2;
            imgProfile.Layer.BorderColor = UIColor.Clear.CGColor;
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



            UIButton DashboardButton = new UIButton()
            {
                Frame = new CGRect(0, profileViewHeight, 250, 40),
                Font = UIFont.FromName("Futura-Medium", 14f),
                BackgroundColor = UIColor.Clear,
            };
            DashboardButton.SetTitle("DASHBOARD", UIControlState.Normal);
            DashboardButton.SetTitleColor(UIColor.DarkTextColor, UIControlState.Normal);
            DashboardButton.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Selected);
            DashboardButton.BackgroundColor = UIColor.LightTextColor;
            DashboardButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;

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
            DashboardButton.TouchUpInside += DashboardButton_TouchUpInside;
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

            DashboardButton.InsertSubview(seperator, 1);
            ChangePasswordButton.InsertSubview(seperatorPassword, 1);
            AlertsButton.InsertSubview(seperatorAlerts, 1);
            InsightsButton.InsertSubview(seperatorInsights, 1);

            if (userdetail.RoleId == 2)
            {
                ChangePasswordButton.Frame = new CGRect(0, profileViewHeight + 40, 250, 40);
                View.AddSubviews(viewProfile, DashboardButton, ChangePasswordButton, LogOutButton);
            }
            else
            {
                AlertsButton.Frame = new CGRect(0, profileViewHeight + 40, 250, 40);
                InsightsButton.Frame = new CGRect(0, profileViewHeight + 80, 250, 40);
                ChangePasswordButton.Frame = new CGRect(0, profileViewHeight + 120, 250, 40);
                View.AddSubviews(viewProfile, DashboardButton, ChangePasswordButton, AlertsButton, InsightsButton);
            }
        }



        private async void Logout(LogoutModel logoutModel)
        {
            var response = await InvokeApi.Invoke(Constants.API_SIGN_OUT, JsonConvert.SerializeObject(logoutModel), HttpMethod.Post);

        }


        #endregion
    }
}
using CoreGraphics;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using RestSharp;
using System;
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
            PreferenceHandler prefs = new PreferenceHandler();
            Logout(new LogoutModel(prefs.GetUserDetails().Email));

        }

        #endregion

        #region " Functions "


        public void GenerateMenu()
        {

            prefHandler = new PreferenceHandler();
            userdetail = prefHandler.GetUserDetails();
            //ProfileName.SetTitle(userdetail.First_Name + " " + userdetail.Last_Name, UIControlState.Normal);
            //ProfileName.Enabled = false;

            UIView viewProfile = new UIView(new CGRect(0, 0, View.Bounds.Width, 180));
            viewProfile.BackgroundColor = UIColor.FromRGB(33, 77, 43);

            UILabel lblProfileName = new UILabel()
            {
                Frame = new CGRect(0, 40, 260, 100),
                Font = UIFont.FromName("Futura-Medium", 20f),
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Center,
                Text = userdetail.First_Name + " " + userdetail.Last_Name,
                TextColor = UIColor.White,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3

            };
            viewProfile.AddSubview(lblProfileName);

            UIButton ReportsButton = new UIButton()
            {
                Frame = new CGRect(50, 210, 150, 40),
                Font = UIFont.FromName("Futura-Medium", 15f),
                BackgroundColor = UIColor.Clear
            };
            ReportsButton.SetTitle("Reports", UIControlState.Normal);
            ReportsButton.SetTitleColor(UIColor.DarkTextColor, UIControlState.Normal);
            ReportsButton.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Selected);

            UIButton AlertsButton = new UIButton()
            {
                Font = UIFont.FromName("Futura-Medium", 15f),
                BackgroundColor = UIColor.Clear
            };
            AlertsButton.SetTitle("Alerts", UIControlState.Normal);
            AlertsButton.SetTitleColor(UIColor.DarkTextColor, UIControlState.Normal);
            AlertsButton.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Selected);

            UIButton ChangePasswordButton = new UIButton()
            {
                Font = UIFont.FromName("Futura-Medium", 15f),
                BackgroundColor = UIColor.Clear
            };
            ChangePasswordButton.SetTitle("Change Password", UIControlState.Normal);
            ChangePasswordButton.SetTitleColor(UIColor.DarkTextColor, UIControlState.Normal);
            ChangePasswordButton.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Selected);


            UIButton LogOutButton = new UIButton()
            {
                Font = UIFont.FromName("Futura-Medium", 15f),
                BackgroundColor = UIColor.Clear,
            };
            LogOutButton.SetTitle("Log Out", UIControlState.Normal);
            LogOutButton.SetTitleColor(UIColor.DarkTextColor, UIControlState.Normal);
            LogOutButton.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Selected);

            ChangePasswordButton.TouchUpInside += ChangePasswordButton_TouchUpInside;
            LogOutButton.TouchUpInside += LogOutButton_TouchUpInside;
            ReportsButton.TouchUpInside += ReportsButton_TouchUpInside;
            AlertsButton.TouchUpInside += AlertsButton_TouchUpInside;

            if (userdetail.Role_Id == 2)
            {
                ChangePasswordButton.Frame = new CGRect(50, 260, 150, 40);
                LogOutButton.Frame = new CGRect(50, 310, 150, 40);
                View.AddSubviews(viewProfile, ChangePasswordButton, ReportsButton, LogOutButton);
            }
            else
            {
                AlertsButton.Frame = new CGRect(50, 260, 150, 40);
                ChangePasswordButton.Frame = new CGRect(50, 310, 150, 40);
                LogOutButton.Frame = new CGRect(50, 360, 150, 40);
                View.AddSubviews(viewProfile, ChangePasswordButton, ReportsButton, AlertsButton, LogOutButton);
            }



        }

        private void Logout(LogoutModel logoutModel)
        {


            RestClient client = new RestClient(Constants.SERVER_BASE_URL);

            var request = new RestRequest(Constants.API_SIGN_OUT, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(logoutModel);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    InvokeOnMainThread(() =>
                    {
                        LogoutResponse((RestResponse)response);
                    });
                }
            });
        }

        private void LogoutResponse(RestResponse restResponse)
        {


            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                GeneralResponseModel response = JsonConvert.DeserializeObject<GeneralResponseModel>(restResponse.Content);

                if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    PreferenceHandler preferenceHandler = new PreferenceHandler();
                    preferenceHandler.setLoggedIn(false);

                    var ViewController = (ViewController)Storyboard.InstantiateViewController("ViewController");
                    ViewController.NavigationItem.SetHidesBackButton(true, false);
                    NavController.PushViewController(ViewController, false);
                    SidebarController.MenuWidth = 0;
                    SidebarController.CloseMenu();
                    loadingOverlay.Hide();

                }
                else
                {
                    ShowMessage("Failed to logout, Please try later.");
                }
            }
            else
            {
                ShowMessage("Failed to logout, Please try later.");
            }
        }

        private void ShowMessage(string v)
        {
            loadingOverlay.Hide();
            UIAlertController alertController = UIAlertController.Create("Message", v, UIAlertControllerStyle.Alert);
            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));
            PresentViewController(alertController, true, null);
        }

        #endregion
    }
}
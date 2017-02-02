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
        LoadingOverlay loadingOverlay;

        public MyMenuController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            PreferenceHandler preferenceHandler = new PreferenceHandler();
            UserDetails User = preferenceHandler.GetUserDetails();
            ProfileName.SetTitle(User.First_Name + " " + User.Last_Name, UIControlState.Normal);
            ProfileName.Enabled = false;

            ChangePasswordButton.TouchUpInside += ChangePasswordButton_TouchUpInside;
            LogOutButton.TouchUpInside += LogOutButton_TouchUpInside;
            ReportsButton.TouchUpInside += ReportsButton_TouchUpInside;
            AlertsButton.TouchUpInside += AlertsButton_TouchUpInside;
        }



        private void AlertsButton_TouchUpInside(object sender, EventArgs e)
        {
            var AlertsController = (AlertsController)Storyboard.InstantiateViewController("AlertsController");
            NavController.PushViewController(AlertsController, false);
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
            /*PreferenceHandler preferenceHandler = new PreferenceHandler();
            preferenceHandler.setLoggedIn(false);
            var ViewController = (ViewController)Storyboard.InstantiateViewController("ViewController");
            ViewController.NavigationItem.SetHidesBackButton(true, false);
            NavController.PushViewController(ViewController, false);
            SidebarController.MenuWidth = 0;
            SidebarController.CloseMenu();*/
            // Added for showing loading screen
            var bounds = UIScreen.MainScreen.Bounds;
            // show the loading overlay on the UI thread using the correct orientation sizing
            loadingOverlay = new LoadingOverlay(bounds);
            View.Add(loadingOverlay);
            PreferenceHandler prefs = new PreferenceHandler();
            Logout(new LogoutModel(prefs.GetUserDetails().Email));
            
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
    }
}
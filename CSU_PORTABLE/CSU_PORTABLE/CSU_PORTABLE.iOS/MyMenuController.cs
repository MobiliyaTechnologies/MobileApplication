using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using Foundation;
using System;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class MyMenuController : BaseController
    {
        public MyMenuController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            PreferenceHandler preferenceHandler = new PreferenceHandler();
            UserDetails User = preferenceHandler.GetUserDetails();
            ProfileName.SetTitle(User.First_Name + " " + User.Last_Name, UIControlState.Normal);
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
            PreferenceHandler preferenceHandler = new PreferenceHandler();
            preferenceHandler.setLoggedIn(false);
            var ViewController = (ViewController)Storyboard.InstantiateViewController("ViewController");
            ViewController.NavigationItem.SetHidesBackButton(true, false);
            NavController.PushViewController(ViewController, false);
            SidebarController.MenuWidth = 0;
            SidebarController.CloseMenu();

        }


    }
}
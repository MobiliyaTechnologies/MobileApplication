using System;
using System.Drawing;

using CoreFoundation;
using UIKit;
using Foundation;
using CSU_PORTABLE.iOS.Utils;
using CoreGraphics;
using CSU_PORTABLE.Models;
using static CSU_PORTABLE.Utils.Constants;

namespace CSU_PORTABLE.iOS
{

    public partial class BaseController : UIViewController
    {
        BadgeBarButtonItem btnAlertsBadge;
        UIButton btnBadge;
        // provide access to the sidebar controller to all inheriting controllers
        protected SidebarNavigation.SidebarController SidebarController
        {
            get
            {
                return (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.SidebarController;
            }
        }

        // provide access to the navigation controller to all inheriting controllers
        protected NavController NavController
        {
            get
            {
                return (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.NavController;
            }
        }

        // provide access to the storyboard to all inheriting controllers
        public override UIStoryboard Storyboard
        {
            get
            {
                return (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.Storyboard;
            }
        }

        public BaseController(IntPtr handle) : base(handle)
        {
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.SetLeftBarButtonItem(
                new UIBarButtonItem(UIImage.FromBundle("threelines.png")
                    , UIBarButtonItemStyle.Plain
                    , (sender, args) =>
                    {
                        SidebarController.ToggleMenu();
                    }), true);


            //PreferenceHandler prefHandler = new PreferenceHandler();
            UserDetails userDetail = PreferenceHandler.GetUserDetails();

            if (PreferenceHandler.GetUserDetails().RoleId == (int)USER_ROLE.ADMIN)
            {
                btnBadge = new UIButton()
                {
                    Frame = new CGRect(0, 0, 25, 25),
                };
                btnBadge.SetBackgroundImage(UIImage.FromBundle("Notification_Icon.png"), UIControlState.Normal);
                btnBadge.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnBadge.TouchUpInside += BtnBadge_TouchUpInside;
                btnAlertsBadge = new BadgeBarButtonItem(btnBadge);
                btnAlertsBadge.BadgeValue = UIApplication.SharedApplication.ApplicationIconBadgeNumber.ToString();
                btnAlertsBadge.Style = UIBarButtonItemStyle.Plain;
                btnAlertsBadge.ShouldHideBadgeAtZero = true;
                btnAlertsBadge.BadgeOriginX = 10;
                NavigationItem.SetRightBarButtonItem(btnAlertsBadge, true);
            }
        }


        private void BtnBadge_TouchUpInside(object sender, EventArgs e)
        {
            var btnBadge = (UIButton)sender;
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            btnAlertsBadge.BadgeValue = "0";
            var AlertsViewController = (AlertsViewController)Storyboard.InstantiateViewController("AlertsViewController");
            NavController.PushViewController(AlertsViewController, false);
        }
    }
}
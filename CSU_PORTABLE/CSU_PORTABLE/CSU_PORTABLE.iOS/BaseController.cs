using System;
using System.Drawing;

using CoreFoundation;
using UIKit;
using Foundation;


namespace CSU_PORTABLE.iOS
{

    public partial class BaseController : UIViewController
    {
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
            NavigationItem.SetRightBarButtonItem(
                new UIBarButtonItem(UIImage.FromBundle("Notification_Icon.png")
                    , UIBarButtonItemStyle.Plain
                    , (sender, args) =>
                    {

                    }), true);

        }
    }
}
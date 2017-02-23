using System;
using System.Drawing;

using CoreFoundation;
using UIKit;
using Foundation;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;

namespace CSU_PORTABLE.iOS
{



    public partial class RootViewController : UIViewController
    {
        // the sidebar controller for the app
        public SidebarNavigation.SidebarController SidebarController { get; private set; }

        // the navigation controller
        public NavController NavController { get; private set; }

        public RootViewController()
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        private UIStoryboard _storyboard;
        // the storyboard
        public override UIStoryboard Storyboard
        {
            get
            {
                if (_storyboard == null)
                    _storyboard = UIStoryboard.FromName("Main", null);
                return _storyboard;
            }
        }

        public override void ViewDidLoad()
        {

            base.ViewDidLoad();

            PreferenceHandler preferenceHandler = new PreferenceHandler();
            var menuController = (MyMenuController)Storyboard.InstantiateViewController("MyMenuController");

            // create a slideout navigation controller with the top navigation controller and the menu view controller
            NavController = new NavController();
            Boolean IsLogged = preferenceHandler.IsLoggedIn();
            UserDetails userDetail = preferenceHandler.GetUserDetails();
            if (IsLogged)
            {
                if (userDetail.Role_Id == 2)
                {
                    //var ClassRoomController = (ClassRoomController)Storyboard.InstantiateViewController("ClassRoomController");
                    //NavController.PushViewController(ClassRoomController, false);

                    var FeedbackViewController = Storyboard.InstantiateViewController("FeedbackViewController") as FeedbackViewController;
                    NavController.PushViewController(FeedbackViewController, false);
                }
                else
                {
                    var MapViewController = (MapViewController)Storyboard.InstantiateViewController("MapViewController");
                    NavController.PushViewController(MapViewController, false);
                }
            }
            else
            {
                var ViewController = (ViewController)Storyboard.InstantiateViewController("ViewController");
                NavController.PushViewController(ViewController, false);

            }

            SidebarController = new SidebarNavigation.SidebarController(this, NavController, menuController);
            SidebarController.MenuWidth = (IsLogged ? 250 : 0);
            SidebarController.ReopenOnRotate = false;
            SidebarController.MenuLocation = SidebarNavigation.MenuLocations.Left;
        }
    }
}
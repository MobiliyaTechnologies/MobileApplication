using System;
using System.Drawing;

using CoreFoundation;
using UIKit;
using Foundation;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Newtonsoft.Json;

namespace CSU_PORTABLE.iOS
{



    public partial class RootViewController : UIViewController
    {
        // the sidebar controller for the app
        public SidebarNavigation.SidebarController SidebarController { get; private set; }
        //PreferenceHandler preferenceHandler;
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

            //preferenceHandler = new PreferenceHandler();
            var menuController = (MyMenuController)Storyboard.InstantiateViewController("MyMenuController");

            // create a slideout navigation controller with the top navigation controller and the menu view controller
            NavController = new NavController();
            Boolean IsLogged = PreferenceHandler.IsLoggedIn();
            UserDetails userDetail = PreferenceHandler.GetUserDetails();

            if (string.IsNullOrEmpty(PreferenceHandler.GetDomainKey()))
            {
                var ConfigurationController = Storyboard.InstantiateViewController("ConfigurationController") as ConfigurationController;
                NavController.PushViewController(ConfigurationController, true);
            }
            else
            {
                InvokeApi.SetDomainUrl(PreferenceHandler.GetDomainKey());
                if (string.IsNullOrEmpty(PreferenceHandler.GetConfig()))
                {
                    var ConfigurationController = Storyboard.InstantiateViewController("ConfigurationController") as ConfigurationController;
                    NavController.PushViewController(ConfigurationController, true);
                }
                else
                {
                    var config = JsonConvert.DeserializeObject<B2CConfiguration>(PreferenceHandler.GetConfig());
                    B2CConfigManager.GetInstance().Initialize(config);
                    if (IsLogged)
                    {

                        if (userDetail.RoleId == 2)
                        {
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


                }
            }
            SidebarController = new SidebarNavigation.SidebarController(this, NavController, menuController);
            SidebarController.MenuWidth = (IsLogged ? 250 : 0);
            SidebarController.ReopenOnRotate = false;
            SidebarController.MenuLocation = SidebarNavigation.MenuLocations.Left;
        }
    }
}
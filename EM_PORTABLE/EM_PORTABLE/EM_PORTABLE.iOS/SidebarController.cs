using System;
using System.Drawing;

using CoreFoundation;
using UIKit;
using Foundation;


namespace CSU_PORTABLE.iOS
{

    [Register("SidebarController")]
    public class SidebarController : UIViewController
    {
        public SidebarController()
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            View = new UniversalView();

            base.ViewDidLoad();

            // Perform any additional setup after loading the view
        }
    }
}
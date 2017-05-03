using CSU_PORTABLE.iOS.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace CSU_PORTABLE.iOS.Utils
{
    public class IOSUtil
    {
        public static void ShowMessage(string message, LoadingOverlay loadingOverlay, UIViewController viewController)
        {
            if (loadingOverlay != null)
            {
                loadingOverlay.Hide();
            }
            UIAlertController alertController = UIAlertController.Create("Message", message, UIAlertControllerStyle.Alert);
            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));
            viewController.PresentViewController(alertController, true, null);
        }

        public static void ShowAlert(string message)
        {
            var alert = new UIAlertView(message, "", null, "OK");
            alert.Show();
        }
    }
}

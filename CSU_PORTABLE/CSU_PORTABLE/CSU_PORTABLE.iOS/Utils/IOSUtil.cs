using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace CSU_PORTABLE.iOS.Utils
{
    public class IOSUtil
    {
        public static NavController NavController { get; private set; }
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

        public static void RefreshToken(UIViewController viewController, LoadingOverlay loadingOverlay)
        {
            if (loadingOverlay != null)
            {
                loadingOverlay.Hide();
            }
            var LoginViewController = (LoginViewController)viewController.Storyboard.InstantiateViewController("LoginViewController");
            viewController.NavigationController.PushViewController(LoginViewController, false);

        }

        public static MapPoints ConvertBuildingToPoints(BuildingModel model)
        {
            return new MapPoints()
            {
                Id = model.BuildingID,
                Name = model.BuildingName,
                Description = model.BuildingDesc,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                MonthlyConsumption = model.MonthlyConsumption
            };
        }

        public static MapPoints ConvertCampusToPoints(CampusModel model)
        {
            return new MapPoints()
            {
                Id = model.CampusID,
                Name = model.CampusName,
                Description = model.CampusDesc,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                MonthlyConsumption = model.MonthlyConsumption
            };
        }


        public static MapPoints ConvertMetersToPoints(MeterDetails model)
        {
            return new MapPoints()
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                MonthlyConsumption = model.MonthlyConsumption
            };
        }


    }
}

using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
            var alert = new UIAlertView("Energy Management", message, null, null, "OK");
            alert.Show();
        }


        public async static Task RefreshToken(UIViewController viewController, LoadingOverlay loadingOverlay)
        {
            string tokenURL = string.Format(B2CConfig.TokenURL, B2CConfig.Tenant, B2CPolicy.SignInPolicyId, B2CConfig.ClientId, PreferenceHandler.GetAccessCode());
            var response = await InvokeApi.Authenticate(tokenURL, string.Empty, HttpMethod.Post);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string strContent = await response.Content.ReadAsStringAsync();
                var tokenNew = JsonConvert.DeserializeObject<AccessToken>(strContent);
                PreferenceHandler.SetToken(tokenNew.id_token);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                RedirectToLogin(viewController, loadingOverlay);
            }
        }

        public static void RedirectToLogin(UIViewController viewController, LoadingOverlay loadingOverlay)
        {
            PreferenceHandler.setLoggedIn(false);
            PreferenceHandler.SetToken(string.Empty);
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

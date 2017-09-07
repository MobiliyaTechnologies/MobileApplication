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
        public static DemoStage CurrentStage = DemoStage.None;
        public static double LayoutWidth;

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

        public static async Task GetToken()
        {
            string tokenURL = string.Format(B2CConfig.TokenURL, B2CConfig.Tenant, B2CPolicy.SignInPolicyId, B2CConfig.ClientId, PreferenceHandler.GetAccessCode());
            var response = await InvokeApi.Authenticate(tokenURL, string.Empty, HttpMethod.Post);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string strContent = await response.Content.ReadAsStringAsync();
                var tokenNew = JsonConvert.DeserializeObject<AccessToken>(strContent);
                PreferenceHandler.SetToken(tokenNew.id_token);
                PreferenceHandler.SetRefreshToken(tokenNew.refresh_token);
            }
        }

        public static async Task RefreshToken(UIViewController viewController, LoadingOverlay loadingOverlay)
        {
            string tokenURL = string.Format(B2CConfig.RefreshTokenURL, B2CConfig.Tenant, B2CPolicy.SignInPolicyId, B2CConfig.ClientId, PreferenceHandler.GetRefreshToken());
            var response = await InvokeApi.Authenticate(tokenURL, string.Empty, HttpMethod.Post);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string strContent = await response.Content.ReadAsStringAsync();
                var tokenNew = JsonConvert.DeserializeObject<AccessToken>(strContent);
                PreferenceHandler.SetToken(tokenNew.id_token);
                PreferenceHandler.SetRefreshToken(tokenNew.refresh_token);
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


        #region " CUSTOM COLOR"

        public static UIColor PrimaryColor = UIColor.FromRGB(70, 78, 120);
        public static UIColor SecondaryColor = UIColor.FromRGB(53, 172, 207);
        public static UIColor VeryCold = UIColor.FromRGB(193, 235, 244);
        public static UIColor Cold = UIColor.FromRGB(148, 221, 242);
        public static UIColor Normal = UIColor.FromRGB(150, 197, 245);
        public static UIColor Hot = UIColor.FromRGB(210, 207, 235);
        public static UIColor VeryHot = UIColor.FromRGB(235, 230, 207);
        #endregion
    }
}

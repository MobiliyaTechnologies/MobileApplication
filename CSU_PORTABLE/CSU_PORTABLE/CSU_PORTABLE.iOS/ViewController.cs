using CoreGraphics;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class ViewController : BaseController
    {
        LoadingOverlay loadingOverlay;
        //UITextField TextFieldUsername, TextFieldPassword;
        UIButton ButtonLogin;
        private UIButton ButtonSignUp;

        // the sidebar controller for the app
        //public SidebarNavigation.SidebarController SidebarController { get; private set; }

        public ViewController(IntPtr handle) : base(handle)
        {

        }


        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.View.BackgroundColor = IOSUtil.PrimaryColor;
            this.NavigationController.NavigationBarHidden = true;
            this.Title = string.Empty;

            NavigationItem.SetRightBarButtonItem(
                 new UIBarButtonItem(UIImage.FromBundle("a")
                     , UIBarButtonItemStyle.Plain
                     , (sender, args) =>
                     {
                         SidebarController.ToggleMenu();
                     }), true);

            NavigationItem.SetLeftBarButtonItem(
               new UIBarButtonItem(UIImage.FromBundle("a")
                   , UIBarButtonItemStyle.Plain
                   , (sender, args) =>
                   {
                       SidebarController.ToggleMenu();
                   }), true);


            UIImageView imgLogin = new UIImageView()
            {
                Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height),
                Image = UIImage.FromBundle("Login_BG.jpg"),
                ClipsToBounds = true,
            };

            UIView viewWhiteBG = new UIView()
            {
                Frame = new CGRect(25, 170, View.Bounds.Width - 50, 300),
                BackgroundColor = UIColor.White,
            };


            UIImageView imgLogo = new UIImageView()
            {
                Frame = new CGRect((View.Bounds.Width / 2) - 50, 120, 100, 100),
                Image = UIImage.FromBundle("logo.png")
            };

            UIView topLine = new UIView(new CGRect(25, 170, View.Bounds.Width - 50, 10));
            topLine.BackgroundColor = IOSUtil.PrimaryColor;

            ButtonLogin = new UIButton(UIButtonType.Custom);
            ButtonLogin.SetTitle("SIGN IN", UIControlState.Normal);
            ButtonLogin.Font = UIFont.FromName("Futura-Medium", 15f);
            ButtonLogin.SetTitleColor(UIColor.White, UIControlState.Normal);
            ButtonLogin.SetTitleColor(UIColor.White, UIControlState.Focused);
            ButtonLogin.Frame = new CGRect((View.Bounds.Width / 2) - 120, 250, 240, 50);
            ButtonLogin.BackgroundColor = IOSUtil.SecondaryColor;
            ButtonLogin.TouchUpInside += delegate
             {
                 // Added for showing loading screen
                 var bounds = UIScreen.MainScreen.Bounds;
                 // show the loading overlay on the UI thread using the correct orientation sizing
                 loadingOverlay = new LoadingOverlay(bounds);
                 View.Add(loadingOverlay);
                 ShowLogin();
             };

            ButtonSignUp = new UIButton(UIButtonType.Custom);
            ButtonSignUp.SetTitle("SIGN UP", UIControlState.Normal);
            ButtonSignUp.Font = UIFont.FromName("Futura-Medium", 15f);
            ButtonSignUp.SetTitleColor(UIColor.White, UIControlState.Normal);
            ButtonSignUp.SetTitleColor(UIColor.White, UIControlState.Focused);
            ButtonSignUp.Frame = new CGRect((View.Bounds.Width / 2) - 120, 330, 240, 50);
            ButtonSignUp.BackgroundColor = IOSUtil.SecondaryColor;
            ButtonSignUp.TouchUpInside += ButtonSignUp_TouchUpInside;

            UIButton btnForgotPassword = new UIButton(UIButtonType.Custom);
            btnForgotPassword.SetTitle("Forgot password", UIControlState.Normal);
            btnForgotPassword.Font = UIFont.FromName("Futura-Medium", 13f);
            btnForgotPassword.SetTitleColor(IOSUtil.SecondaryColor, UIControlState.Normal);
            btnForgotPassword.SetTitleColor(UIColor.White, UIControlState.Selected);
            btnForgotPassword.Frame = new CGRect((View.Bounds.Width / 2) - 120, 410, 240, 50);
            btnForgotPassword.BackgroundColor = UIColor.Clear;
            btnForgotPassword.TouchUpInside += BtnForgotPassword_TouchUpInside;
            View.AddSubviews(imgLogin, viewWhiteBG, topLine, imgLogo, ButtonLogin, ButtonSignUp, btnForgotPassword);


        }

        private void ButtonSignUp_TouchUpInside(object sender, EventArgs e)
        {
            ShowLogin();
        }

        private void BtnForgotPassword_TouchUpInside(object sender, EventArgs e)
        {
            var ForgotPasswordController = (ForgotPasswordController)Storyboard.InstantiateViewController("ForgotPasswordController");
            NavController.PushViewController(ForgotPasswordController, true);

        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }


        #region " Custom Functions" 
        public async void Login(LoginModel loginModel)
        {
            var response = await InvokeApi.Invoke(Constants.API_SIGN_IN, JsonConvert.SerializeObject(loginModel), HttpMethod.Post);
            if (response.StatusCode != 0)
            {
                InvokeOnMainThread(() =>
                {
                    LoginResponse(response);
                });
            }

        }

        private async void LoginResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                string strContent = await restResponse.Content.ReadAsStringAsync();
                UserDetails response = JsonConvert.DeserializeObject<UserDetails>(strContent);

                if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    SaveUserData(response);
                }
                else
                {
                    IOSUtil.ShowMessage("Login Failed", loadingOverlay, this);
                }
            }
            else
            {
                IOSUtil.ShowMessage("Login Failed", loadingOverlay, this);
            }
        }

        private void SaveUserData(UserDetails userDetails)
        {
            //store data in preferences

            //PreferenceHandler preferenceHandler = new PreferenceHandler();
            PreferenceHandler.SaveUserDetails(userDetails);
            if (userDetails.RoleId == 2)
            {
                ShowClassRooms();
            }
            else
            {
                ShowMap();
            }
        }

        private void ShowLogin()
        {
            LoginViewController LoginView = this.Storyboard.InstantiateViewController("LoginViewController") as LoginViewController;
            LoginView.NavigationItem.SetHidesBackButton(true, false);

            this.NavController.PushViewController(LoginView, false);
            var menuController = (MyMenuController)Storyboard.InstantiateViewController("MyMenuController");
            SidebarController.ChangeMenuView(menuController);
            SidebarController.MenuWidth = 250;
            SidebarController.ReopenOnRotate = false;
        }

        private void ShowClassRooms()
        {
            FeedbackViewController FeedbackView = this.Storyboard.InstantiateViewController("FeedbackViewController") as FeedbackViewController;
            FeedbackView.NavigationItem.SetHidesBackButton(true, false);

            this.NavController.PushViewController(FeedbackView, false);
            var menuController = (MyMenuController)Storyboard.InstantiateViewController("MyMenuController");
            SidebarController.ChangeMenuView(menuController);
            SidebarController.MenuWidth = 250;
            SidebarController.ReopenOnRotate = false;
        }

        private void ShowMap()
        {
            // Launches a new instance of CallHistoryController
            MapViewController mapView = this.Storyboard.InstantiateViewController("MapViewController") as MapViewController;
            if (mapView != null)
            {
                mapView.NavigationItem.SetHidesBackButton(true, false);
                this.NavController.PushViewController(mapView, false);
                var menuController = (MyMenuController)Storyboard.InstantiateViewController("MyMenuController");
                SidebarController.ChangeMenuView(menuController);
                SidebarController.MenuWidth = 250;
                SidebarController.ReopenOnRotate = false;
            }
        }
        #endregion
    }
}


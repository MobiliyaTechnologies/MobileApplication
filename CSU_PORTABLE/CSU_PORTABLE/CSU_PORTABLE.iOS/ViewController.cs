using CoreGraphics;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Newtonsoft.Json;
using RestSharp;
using System;

using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class ViewController : BaseController
    {
        LoadingOverlay loadingOverlay;
        UITextField TextFieldUsername, TextFieldPassword;
        UIButton ButtonLogin;

        // the sidebar controller for the app
        //public SidebarNavigation.SidebarController SidebarController { get; private set; }

        public ViewController(IntPtr handle) : base(handle)
        {

        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.View.BackgroundColor = UIColor.FromRGB(30, 77, 43);


            NavigationItem.SetRightBarButtonItem(
                 new UIBarButtonItem(UIImage.FromBundle("a")
                     , UIBarButtonItemStyle.Plain
                     , (sender, args) =>
                     {
                         SidebarController.ToggleMenu();
                     }), true);


            UIImageView imgLogo = new UIImageView()
            {
                Frame = new CGRect((View.Bounds.Width / 2) - 50, 120, 100, 100),
                Image = UIImage.FromBundle("CSU_logo.png")
            };

            UIImageView imgEmail = new UIImageView()
            {
                Frame = new CGRect((View.Bounds.Width / 2) - 120, 255, 20, 20),
                Image = UIImage.FromBundle("Mail_Icon_White.png")
            };

            UIImageView imgPassword = new UIImageView()
            {
                Frame = new CGRect((View.Bounds.Width / 2) - 120, 305, 20, 20),
                Image = UIImage.FromBundle("Lock_Icon_White.png")
            };


            //TextFieldUsername.Text = "aaa@111.com";
            //TextFieldPassword.Text = "111";
            UIView paddingView = new UIView(new CGRect(5, 5, 5, 20));
            TextFieldUsername = new UITextField()
            {
                Font = UIFont.FromName("Helvetica", 15f),
                TextColor = UIColor.White,
                BackgroundColor = UIColor.Clear,
                Frame = new CGRect((View.Bounds.Width / 2) - 80, 250, 200, 30),
                Placeholder = "Email",
                TextAlignment = UITextAlignment.Left,
                AutocorrectionType = UITextAutocorrectionType.No,
                LeftView = paddingView,
                LeftViewMode = UITextFieldViewMode.Always,
                BorderStyle = UITextBorderStyle.RoundedRect,
                TintColor = UIColor.White
            };

            UIView bottomLineEmail = new UIView(new CGRect((View.Bounds.Width / 2) - 120, 285, 240, 1));
            bottomLineEmail.BackgroundColor = UIColor.White;

            UIView paddingViewPassword = new UIView(new CGRect(5, 5, 5, 20));
            UIView bottomLinePwd = new UIView(new CGRect((View.Bounds.Width / 2) - 120, 335, 240, 1));
            bottomLinePwd.BackgroundColor = UIColor.White;

            TextFieldPassword = new UITextField()
            {
                Font = UIFont.FromName("Helvetica", 15f),
                TextColor = UIColor.White,
                BackgroundColor = UIColor.Clear,
                Frame = new CGRect((View.Bounds.Width / 2) - 80, 300, 200, 30),
                Placeholder = "Password",
                TextAlignment = UITextAlignment.Left,
                SecureTextEntry = true,
                AutocorrectionType = UITextAutocorrectionType.No,
                LeftView = paddingViewPassword,
                LeftViewMode = UITextFieldViewMode.Always,
                BorderStyle = UITextBorderStyle.RoundedRect,
                TintColor = UIColor.White
            };


            TextFieldUsername.BecomeFirstResponder();

            TextFieldUsername.ShouldReturn = delegate
            {
                // Changed this slightly to move the text entry to the next field.
                TextFieldPassword.BecomeFirstResponder();
                return true;
            };

            TextFieldPassword.ShouldReturn = delegate
            {
                TextFieldPassword.ResignFirstResponder();
                return true;
            };

            ButtonLogin = new UIButton(UIButtonType.Custom);
            ButtonLogin.SetTitle("Log In", UIControlState.Normal);
            ButtonLogin.Font = UIFont.FromName("Futura-Medium", 15f);
            ButtonLogin.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Normal);
            ButtonLogin.SetTitleColor(UIColor.FromRGB(30, 77, 43), UIControlState.Focused);
            ButtonLogin.Frame = new CGRect((View.Bounds.Width / 2) - 120, 350, 240, 30);
            ButtonLogin.BackgroundColor = UIColor.FromRGB(200, 195, 114);
            ButtonLogin.TouchUpInside += delegate
             {
                 // Added for showing loading screen
                 var bounds = UIScreen.MainScreen.Bounds;
                 // show the loading overlay on the UI thread using the correct orientation sizing
                 loadingOverlay = new LoadingOverlay(bounds);
                 View.Add(loadingOverlay);
                 string username = TextFieldUsername.Text;
                 string password = TextFieldPassword.Text;

                 if (username != null && username.Length > 1 && password != null && password.Length > 1)
                 {
                     //buttonLogin.Visibility = ViewStates.Gone;
                     //progressBar.Visibility = ViewStates.Visible;
                     //MessageLabel.Text = "Logging in...";
                     Login(new LoginModel(username, password));
                 }
                 else
                 {
                     ShowMessage("Enter valid email and password");
                 }
             };
            UIButton btnForgotPassword = new UIButton(UIButtonType.Custom);
            btnForgotPassword.SetTitle("Forgot your password?", UIControlState.Normal);
            btnForgotPassword.Font = UIFont.FromName("Futura-Medium", 13f);
            btnForgotPassword.SetTitleColor(UIColor.LightTextColor, UIControlState.Normal);
            btnForgotPassword.SetTitleColor(UIColor.White, UIControlState.Selected);
            btnForgotPassword.Frame = new CGRect((View.Bounds.Width / 2) - 120, 400, 240, 20);
            btnForgotPassword.BackgroundColor = UIColor.Clear;
            btnForgotPassword.TouchUpInside += BtnForgotPassword_TouchUpInside;
            View.AddSubviews(imgLogo, TextFieldUsername, TextFieldPassword, bottomLinePwd, bottomLineEmail, ButtonLogin, btnForgotPassword, imgEmail, imgPassword);


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
        public void Login(LoginModel loginModel)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);

            var request = new RestRequest(Constants.API_SIGN_IN, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(loginModel);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    InvokeOnMainThread(() =>
                    {
                        LoginResponse((RestResponse)response);
                    });
                }
            });
        }

        private void LoginResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                UserDetails response = JsonConvert.DeserializeObject<UserDetails>(restResponse.Content);

                if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    SaveUserData(response);
                }
                else
                {
                    ShowMessage("Login Failed");
                }
            }
            else
            {
                ShowMessage("Login Failed");
            }
        }

        private void SaveUserData(UserDetails userDetails)
        {
            //store data in preferences

            PreferenceHandler preferenceHandler = new PreferenceHandler();
            preferenceHandler.SaveUserDetails(userDetails);
            if (userDetails.Role_Id == 2)
            {
                ShowClassRooms();
            }
            else
            {
                ShowMap();
            }
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

        private void ShowMessage(string v)
        {
            //BTProgressHUD.ShowToast("Hello from Toast");
            loadingOverlay.Hide();
            //MessageLabel.Text = " " + v;
            UIAlertController alertController = UIAlertController.Create("Message", v, UIAlertControllerStyle.Alert);

            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));

            PresentViewController(alertController, true, null);

        }

        #endregion
    }
}


using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Newtonsoft.Json;
using RestSharp;
using System;

using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class ViewController : UIViewController
    {

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //TextFieldUsername.Text = "aaa@111.com";
            //TextFieldPassword.Text = "111";
            MessageLabel.Text = " ";

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

            ButtonLogin.TouchUpInside += delegate {

                string username = TextFieldUsername.Text;
                string password = TextFieldPassword.Text;

                if (username != null && username.Length > 1 && password != null && password.Length > 1)
                {
                    //buttonLogin.Visibility = ViewStates.Gone;
                    //progressBar.Visibility = ViewStates.Visible;
                    MessageLabel.Text = "Logging in...";
                    Login(new LoginModel(username, password));
                }
                else
                {
                    ShowMessage("Enter valid username and password");
                }
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

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
                    InvokeOnMainThread(() => {
                        LoginResponse((RestResponse)response);
                    });
                }
            });
        }

        private void LoginResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                //Log.Debug(TAG, restResponse.Content.ToString());
                UserDetails response = JsonConvert.DeserializeObject<UserDetails>(restResponse.Content);

                if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    //Log.Debug(TAG, "Login Successful");
                    //ShowMessage("Login Successful");
                    SaveUserData(response);
                    //progressBar.Visibility = ViewStates.Gone;
                    //StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                }
                else
                {
                    //Log.Debug(TAG, "Login Failed");
                    ShowMessage("Login Failed");
                    //progressBar.Visibility = ViewStates.Gone;
                    //buttonLogin.Visibility = ViewStates.Visible;
                    //ShowToast("Either username or password is incorrect !");
                }
            }
            else
            {
                //Log.Debug(TAG, "Login Failed");
                ShowMessage("Login Failed");
                //progressBar.Visibility = ViewStates.Gone;
                //buttonLogin.Visibility = ViewStates.Visible;
                //ShowToast("Error while login. Please try again.");
            }
        }

        private void SaveUserData(UserDetails userDetails)
        {
            //store data in preferences

            PreferenceHandler preferenceHandler = new PreferenceHandler();
            preferenceHandler.SaveUserDetails(userDetails);
            ShowMap();
        }

        private void ShowMap()
        {
            // Launches a new instance of CallHistoryController
            MapViewController mapView = this.Storyboard.InstantiateViewController("MapViewController") as MapViewController;
            if (mapView != null)
            {
                this.NavigationController.PushViewController(mapView, true);
            }
        }
        private void ShowMessage(string v)
        {
            //BTProgressHUD.ShowToast("Hello from Toast");
            MessageLabel.Text = " " + v;
            UIAlertController alertController = UIAlertController.Create("Message", v, UIAlertControllerStyle.Alert);

            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));

            PresentViewController(alertController, true, null);

        }
    }
}


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

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
            TextFieldUsername.Text = "aaa@111.com";
            TextFieldPassword.Text = "111";
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

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

        public void Login(LoginModel loginModel)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            //Log.Debug(TAG, "Login() " + loginModel.ToString());

            var request = new RestRequest(Constants.API_SIGN_IN, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(loginModel);

            //progressBar.Visibility = ViewStates.Visible;
            //buttonLogin.Visibility = ViewStates.Gone;
            //RestResponse restResponse = (RestResponse)client.Execute(request);
            //LoginResponse(restResponse);
            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    //Log.Debug(TAG, "async Response : " + response.ToString());
                    //RunOnUiThread(() => {
                    //    LoginResponse((RestResponse)response);
                    //});

                    LoginResponse((RestResponse)response);
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
                    ShowMessage("Login Successful");
                    PreferenceHandler preferenceHandler = new PreferenceHandler();
                    preferenceHandler.SaveUserDetails(response);
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

        private void ShowMessage(string v)
        {
            //BTProgressHUD.ShowToast("Hello from Toast");
            MessageLabel.Text = v;
            /*UIAlertController alertController = UIAlertController.Create("Message", v, UIAlertControllerStyle.Alert);

            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));

            PresentViewController(alertController, true, null);
            */
        }
    }
}


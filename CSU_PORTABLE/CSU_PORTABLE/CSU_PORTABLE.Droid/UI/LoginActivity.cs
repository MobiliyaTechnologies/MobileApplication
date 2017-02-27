using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using CSU_PORTABLE.Droid.Utils;
using Android.Support.V7.App;
using RestSharp;
using Newtonsoft.Json;
using System.Threading.Tasks;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "CSU APP", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyTheme")]
    class LoginActivity : Activity
    {
        const string TAG = "MainActivity";
        private EditText etUsername;
        private EditText etPassword;
        private Button buttonLogin;
        private ProgressBar progressBar;
        private Toast toast;
        private TextView tvForgotPassword;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login_view);
            etUsername = FindViewById<EditText>(Resource.Id.editTextUsername);
            etPassword = FindViewById<EditText>(Resource.Id.editTextPassword);
            buttonLogin = FindViewById<Button>(Resource.Id.loginButton);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            tvForgotPassword = FindViewById<TextView>(Resource.Id.textViewForgotPassword);

            progressBar.Visibility = ViewStates.Gone;

            tvForgotPassword.Click += delegate
            {
                Log.Debug(TAG, "ForgotPassword()");
                StartActivity(new Intent(Application.Context, typeof(ForgotPasswordActivity)));
            };
            buttonLogin.Click += delegate
            {
                Log.Debug(TAG, "Login()");

                string username = etUsername.Text.ToString();
                string password = etPassword.Text.ToString();
                if (username != null && username.Length > 1 && password != null && password.Length > 1)
                {
                    buttonLogin.Visibility = ViewStates.Gone;
                    progressBar.Visibility = ViewStates.Visible;

                    bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(this);
                    if (isNetworkEnabled)
                    {
                        Login(new LoginModel(username, password));
                    } else
                    {
                        ShowToast("Please enable your internet connection !");
                    }
                }
                else
                {
                    ShowToast("Enter valid username and password");
                }
            };

        }

        public void Login(LoginModel loginModel)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            Log.Debug(TAG, "Login() " + loginModel.ToString());

            var request = new RestRequest(Constants.API_SIGN_IN, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(loginModel);

            progressBar.Visibility = ViewStates.Visible;
            buttonLogin.Visibility = ViewStates.Gone;
            //RestResponse restResponse = (RestResponse)client.Execute(request);
            //LoginResponse(restResponse);
            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    RunOnUiThread(() => {
                        LoginResponse((RestResponse)response);
                    });
                }
            });
        }
        
        private void LoginResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                UserDetails response = JsonConvert.DeserializeObject<UserDetails>(restResponse.Content);

                if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    Log.Debug(TAG, "Login Successful");
                    progressBar.Visibility = ViewStates.Gone;
                    SaveUserData(response);
                }
                else
                {
                    Log.Debug(TAG, "Login Failed");
                    progressBar.Visibility = ViewStates.Gone;
                    buttonLogin.Visibility = ViewStates.Visible;
                    ShowToast("Either username or password is incorrect !");
                }
            }
            else
            {
                Log.Debug(TAG, "Login Failed");
                progressBar.Visibility = ViewStates.Gone;
                buttonLogin.Visibility = ViewStates.Visible;
                ShowToast("Error while login. Please try again.");
            }
        }

        private void SaveUserData(UserDetails userDetails)
        {
            //store data in preferences

            PreferenceHandler preferenceHandler = new PreferenceHandler();
            preferenceHandler.SaveUserDetails(userDetails);
            if (userDetails.Role_Id == (int)Constants.USER_ROLE.STUDENT)
            {
                ShowStudentDashboard();
            }
            else
            {
                ShowAdminDashboard();
            }


        }

        private void ShowStudentDashboard()
        {
            Intent intent = new Intent(Application.Context, typeof(MainActivity));
            intent.PutExtra(MainActivity.KEY_USER_ROLE, (int)Constants.USER_ROLE.STUDENT);
            StartActivity(intent);
            Finish();
        }

        private void ShowAdminDashboard()
        {
            Intent intent = new Intent(Application.Context, typeof(MainActivity));
            intent.PutExtra(MainActivity.KEY_USER_ROLE, (int)Constants.USER_ROLE.ADMIN);
            StartActivity(intent);
            Finish();
        }

        private void ShowToast(string message)
        {
            if (toast != null)
            {
                toast.Cancel();
            }
            toast = Toast.MakeText(this, message, ToastLength.Short);
            toast.Show();
        }
    }
}
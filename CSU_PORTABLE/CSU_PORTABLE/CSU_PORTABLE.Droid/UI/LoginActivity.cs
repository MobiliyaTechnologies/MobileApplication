using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Util;
using CSU_PORTABLE.Droid.Utils;
using Newtonsoft.Json;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using System.Net.Http;
using Android.Webkit;
using static CSU_PORTABLE.Utils.Constants;
using Android.Content.PM;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "CSU APP", MainLauncher = false, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
    class LoginActivity : Activity
    {
        const string TAG = "LoginActivity";
        //private EditText etUsername;
        //private EditText etPassword;
        private Button buttonSignIn;
        private Button buttonSignUp;
        private ProgressBar progressBar;
        private TextView tvForgotPassword;
        PreferenceHandler preferenceHandler;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login_view);
            //etUsername = FindViewById<EditText>(Resource.Id.editTextUsername);
            //etPassword = FindViewById<EditText>(Resource.Id.editTextPassword);
            preferenceHandler = new Utils.PreferenceHandler();
            if (string.IsNullOrEmpty(preferenceHandler.GetConfig()))
            {
                StartActivity(new Intent(Application.Context, typeof(ConfigActivity)));
                Finish();
            }

            buttonSignIn = FindViewById<Button>(Resource.Id.SignInButton);
            buttonSignUp = FindViewById<Button>(Resource.Id.SignUpButton);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            tvForgotPassword = FindViewById<TextView>(Resource.Id.textViewForgotPassword);

            progressBar.Visibility = ViewStates.Gone;

            // Delete Existing User Details from cache
            CookieManager.Instance.RemoveAllCookie();
            tvForgotPassword.Click += delegate
            {
                Log.Debug(TAG, "ForgotPassword()");
                StartActivity(new Intent(Application.Context, typeof(ForgotPasswordActivity)));
            };
            buttonSignIn.Click += delegate
            {
                Log.Debug(TAG, "Login()");
                Intent intent = new Intent(Application.Context, typeof(LoginNewActivity));
                intent.PutExtra(LoginNewActivity.KEY_SHOW_PAGE, (int)SignInType.SIGN_IN);
                StartActivity(intent);
                Finish();
            };
            buttonSignUp.Click += ButtonSignUp_Click;
        }

        private void ButtonSignUp_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Application.Context, typeof(LoginNewActivity));
            intent.PutExtra(LoginNewActivity.KEY_SHOW_PAGE, (int)SignInType.SIGN_UP);
            StartActivity(intent);
            Finish();
        }

        #region "Old Text"
        //public async void Login(LoginModel loginModel)
        //{
        //    Log.Debug(TAG, "Login() " + loginModel.ToString());
        //    var response = await InvokeApi.Invoke(Constants.API_SIGN_IN, JsonConvert.SerializeObject(loginModel), HttpMethod.Post);
        //    progressBar.Visibility = ViewStates.Visible;
        //    buttonSignIn.Visibility = ViewStates.Gone;
        //    Console.WriteLine(response.ReasonPhrase);
        //    if (response.StatusCode != 0)
        //    {
        //        Log.Debug(TAG, "async Response : " + response.ToString());
        //        RunOnUiThread(() =>
        //        {
        //            LoginResponse(response);
        //        });
        //    }
        //}

        //private async void LoginResponse(HttpResponseMessage restResponse)
        //{
        //    if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
        //    {
        //        Log.Debug(TAG, restResponse.Content.ToString());
        //        string strContent = await restResponse.Content.ReadAsStringAsync();
        //        UserDetails response = JsonConvert.DeserializeObject<UserDetails>(strContent);

        //        if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
        //        {
        //            Log.Debug(TAG, "Login Successful");
        //            progressBar.Visibility = ViewStates.Gone;
        //            SaveUserData(response);
        //        }
        //        else
        //        {
        //            Log.Debug(TAG, "Login Failed");
        //            progressBar.Visibility = ViewStates.Gone;
        //            buttonSignIn.Visibility = ViewStates.Visible;
        //            Utils.Utils.ShowToast(this, "Either username or password is incorrect !");

        //        }
        //    }
        //    else
        //    {
        //        Log.Debug(TAG, "Login Failed");
        //        progressBar.Visibility = ViewStates.Gone;
        //        buttonSignIn.Visibility = ViewStates.Visible;
        //        Utils.Utils.ShowToast(this, "Error while login. Please try again.");
        //    }
        //}

        //private void SaveUserData(UserDetails userDetails)
        //{
        //    //store data in preferences

        //    PreferenceHandler preferenceHandler = new PreferenceHandler();
        //    preferenceHandler.SaveUserDetails(userDetails);
        //    if (userDetails.RoleId == (int)Constants.USER_ROLE.STUDENT)
        //    {
        //        ShowStudentDashboard();
        //    }
        //    else
        //    {
        //        ShowAdminDashboard();
        //    }


        //}

        //private void ShowStudentDashboard()
        //{
        //    Intent intent = new Intent(Application.Context, typeof(MainActivity));
        //    intent.PutExtra(MainActivity.KEY_USER_ROLE, (int)Constants.USER_ROLE.STUDENT);
        //    StartActivity(intent);
        //    Finish();
        //}

        //private void ShowAdminDashboard()
        //{
        //    Intent intent = new Intent(Application.Context, typeof(MainActivity));
        //    intent.PutExtra(MainActivity.KEY_USER_ROLE, (int)Constants.USER_ROLE.ADMIN);
        //    StartActivity(intent);
        //    Finish();
        //}
        #endregion
    }
}
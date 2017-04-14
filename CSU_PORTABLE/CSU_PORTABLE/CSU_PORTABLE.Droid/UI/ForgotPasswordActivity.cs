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
using Android.Support.V7.App;
using Android.Util;
using CSU_PORTABLE.Droid.Utils;
using CSU_PORTABLE.Models;
using Newtonsoft.Json;
using CSU_PORTABLE.Utils;
using System.Net.Http;

namespace CSU_PORTABLE.Droid.UI
{

    [Activity(Label = "Forgot Password", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyTheme")]
    class ForgotPasswordActivity : AppCompatActivity
    {
        const string TAG = "MainActivity";
        private EditText etUsername;
        private Button buttonSubmit;
        private ProgressBar progressBar;
        //private Toast toast;
        private TextView tvResponse;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Forgot_password_view);
            etUsername = FindViewById<EditText>(Resource.Id.editTextUsername);
            buttonSubmit = FindViewById<Button>(Resource.Id.submitButton);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            tvResponse = FindViewById<TextView>(Resource.Id.textViewResponse);

            progressBar.Visibility = ViewStates.Gone;

            buttonSubmit.Click += delegate
            {
                Log.Debug(TAG, "Submit()");

                string username = etUsername.Text.ToString();

                if (username != null && username.Length > 1)
                {
                    bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(this);
                    if (isNetworkEnabled)
                    {
                        SubmitEmail(new ForgotPasswordModel(username));
                    }
                    else
                    {
                        Utils.Utils.ShowToast(this, "Please enable your internet connection !");
                    }
                }
                else
                {
                    Utils.Utils.ShowToast(this, "Enter valid Email Id");
                }
            };
        }

        public async void SubmitEmail(ForgotPasswordModel objModel)
        {
            Log.Debug(TAG, "SubmitEmail() " + objModel.ToString());
            progressBar.Visibility = ViewStates.Visible;
            buttonSubmit.Visibility = ViewStates.Gone;

            var response = await InvokeApi.Invoke(Constants.API_FORGOT_PASSWORD, JsonConvert.SerializeObject(objModel), HttpMethod.Post);
            if (response.StatusCode != 0)
            {
                Log.Debug(TAG, "async Response : " + response.ToString());
                RunOnUiThread(() =>
                {
                    ForgotPasswordResponse(response);
                });
            }
        }

        private async void ForgotPasswordResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                var strContent = await restResponse.Content.ReadAsStringAsync();
                ForgotPasswordResponseModel response = JsonConvert.DeserializeObject<ForgotPasswordResponseModel>(strContent);

                if (response != null && response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    Log.Debug(TAG, "Response Success !");
                    PreferenceHandler preferenceHandler = new PreferenceHandler();
                    preferenceHandler.setLoggedIn(false);
                    progressBar.Visibility = ViewStates.Gone;
                    tvResponse.Text = "Please check your Email.";
                    tvResponse.SetTextColor(Resources.GetColor(Resource.Color.text_green));
                }
                else
                {
                    Log.Debug(TAG, "Response Failed !");
                    progressBar.Visibility = ViewStates.Gone;
                    buttonSubmit.Visibility = ViewStates.Visible;
                    tvResponse.Text = response.Message;
                    tvResponse.SetTextColor(Resources.GetColor(Resource.Color.text_red));
                }
            }
            else
            {
                Log.Debug(TAG, "Response Failed  !");
                progressBar.Visibility = ViewStates.Gone;
                buttonSubmit.Visibility = ViewStates.Visible;
                Utils.Utils.ShowToast(this, "Please try again.");
            }
        }
    }
}
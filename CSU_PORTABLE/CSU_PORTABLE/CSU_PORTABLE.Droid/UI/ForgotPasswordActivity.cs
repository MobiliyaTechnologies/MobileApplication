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
using RestSharp;
using Newtonsoft.Json;
using CSU_PORTABLE.Utils;

namespace CSU_PORTABLE.Droid.UI
{

    [Activity(Label = "Forgot Password", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyTheme")]
    class ForgotPasswordActivity : AppCompatActivity
    {
        const string TAG = "MainActivity";
        private EditText etUsername;
        private Button buttonSubmit;
        private ProgressBar progressBar;
        private Toast toast;
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
            
            buttonSubmit.Click += delegate {
                Log.Debug(TAG, "Submit()");

                string username = etUsername.Text.ToString();

                if (username != null && username.Length > 1)
                {
                    SubmitEmail(new ForgotPasswordModel(username));
                }
                else
                {
                    ShowToast("Enter valid Email Id");
                }
            };
        }

        public void SubmitEmail(ForgotPasswordModel objModel)
        {

            progressBar.Visibility = ViewStates.Visible;
            buttonSubmit.Visibility = ViewStates.Gone;

            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            Log.Debug(TAG, "SubmitEmail() " + objModel.ToString());

            var request = new RestRequest(Constants.API_FORGOT_PASSWORD, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(objModel);

            //RestResponse restResponse = (RestResponse)client.Execute(request);
            //ForgotPasswordResponse(restResponse);
            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    RunOnUiThread(() => {
                        ForgotPasswordResponse((RestResponse)response);
                    });
                }
            });
        }

        private void ForgotPasswordResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                ForgotPasswordResponseModel response = JsonConvert.DeserializeObject<ForgotPasswordResponseModel>(restResponse.Content);

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
                    //ShowToast("Please try again.");
                }
            }
            else
            {
                Log.Debug(TAG, "Response Failed  !");
                progressBar.Visibility = ViewStates.Gone;
                buttonSubmit.Visibility = ViewStates.Visible;
                ShowToast("Please try again.");
            }
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
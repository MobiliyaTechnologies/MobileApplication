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
    [Activity(Label = "Change Password", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyTheme")]
    public class ChangePasswordActivity : AppCompatActivity
    {
        const string TAG = "ChangePasswordActivity";
        private TextView tvUsername;
        private EditText etPassword;
        private EditText etConfirmPassword;
        private Button buttonSubmit;
        private ProgressBar progressBar;
        private TextView tvMessage;
        private UserDetails userDetails;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ChangePasword);

            tvUsername = FindViewById<TextView>(Resource.Id.textViewUsername);
            etPassword = FindViewById<EditText>(Resource.Id.editTextPassword);
            etConfirmPassword = FindViewById<EditText>(Resource.Id.editTextConfirmPassword);
            buttonSubmit = FindViewById<Button>(Resource.Id.submitButton);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            tvMessage = FindViewById<TextView>(Resource.Id.textViewMessage);

            progressBar.Visibility = ViewStates.Gone;
            tvMessage.Visibility = ViewStates.Gone;

            PreferenceHandler prefs = new PreferenceHandler();
            userDetails= prefs.GetUserDetails();
            if (userDetails.Email == null)
            {
                Utils.Utils.ShowToast(this, "Invalid Email Id !");
                //ShowToast("Invalid Email Id !");
            } else
            {
                tvUsername.Text = userDetails.Email;
                EnableButton(buttonSubmit);
                buttonSubmit.Click += delegate
                {
                    Log.Debug(TAG, "Login()");

                    string password = etPassword.Text.ToString();
                    string confirmPassword = etConfirmPassword.Text.ToString();
                    if (password != null && password.Length > 2)
                    {
                        if (confirmPassword != null && confirmPassword.Length > 2)
                        {
                            ChangePasswordModel model = new ChangePasswordModel();
                            model.Email = userDetails.Email;
                            model.Password = etPassword.Text.ToString();
                            model.New_Password = etConfirmPassword.Text.ToString();

                            bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(this);
                            if (isNetworkEnabled)
                            {
                                ChangePassword(model);
                            }
                            else
                            {
                                Utils.Utils.ShowToast(this, "Please enable your internet connection !");
                                //ShowToast("Please enable your internet connection !");
                            }
                        }
                        else
                        {
                            Utils.Utils.ShowToast(this, "Enter valid new password");
                            //ShowToast("Enter valid new password");
                        }
                    }
                    else
                    {
                        Utils.Utils.ShowToast(this, "Enter valid password");
                        //ShowToast("Enter valid password");
                    }
                };
            }

        }

        private void DisableButton(View view)
        {
            view.Enabled = false;
            view.Alpha = 0.3f;
        }
        private void EnableButton(View view)
        {
            view.Enabled = true;
            view.Alpha = 1f;
        }
        
        private void ChangePassword(ChangePasswordModel model)
        {
            DisableButton(buttonSubmit);
            progressBar.Visibility = ViewStates.Visible;

            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            Log.Debug(TAG, "ChangePassword() " + model.ToString());

            var request = new RestRequest(Constants.API_CHANGE_PASSWORD, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(model);
            
            //RestResponse restResponse = (RestResponse)client.Execute(request);
            //LoginResponse(restResponse);
            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    RunOnUiThread(() => {
                        ChangePasswordResponse((RestResponse)response);
                    });
                }
            });
        }

        private void ChangePasswordResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                ChangePasswordResponseModel response = JsonConvert.DeserializeObject<ChangePasswordResponseModel>(restResponse.Content);

                if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    Log.Debug(TAG, "Password Changed Successfully.");
                    tvMessage.Text = "Password Changed Successfully.";
                    tvMessage.Visibility = ViewStates.Visible;
                    progressBar.Visibility = ViewStates.Gone;
                    EnableButton(buttonSubmit);
                }
                else
                {
                    Log.Debug(TAG, "Failed to change password");
                    tvMessage.Text = response.Message;
                    tvMessage.Visibility = ViewStates.Visible;
                    progressBar.Visibility = ViewStates.Gone;
                    EnableButton(buttonSubmit);
                    Utils.Utils.ShowToast(this, "Failed to change password, Plesase try again later.");
                    //ShowToast("Failed to change password, Plesase try again later.");
                }
            }
            else
            {
                Log.Debug(TAG, "Login Failed");
                progressBar.Visibility = ViewStates.Gone;
                EnableButton(buttonSubmit);
                tvMessage.Text = "Error in changing password. Please try again.";
                tvMessage.Visibility = ViewStates.Visible;
                Utils.Utils.ShowToast(this, "Error in changing password. Please try again.");
                //ShowToast("Error in changing password. Please try again.");
            }
        }

        //private void ShowToast(string message)
        //{
        //    if (toast != null)
        //    {
        //        toast.Cancel();
        //    }
        //    toast = Toast.MakeText(this, message, ToastLength.Short);
        //    toast.Show();
        //}
    }
}
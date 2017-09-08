using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Util;
using EM_PORTABLE.Droid.Utils;
using Newtonsoft.Json;
using EM_PORTABLE.Models;
using EM_PORTABLE.Utils;
using System.Net.Http;
using Android.Webkit;
using static EM_PORTABLE.Utils.Constants;
using Android.Content.PM;
using Android.Net;
using Microsoft.Identity.Client;

namespace EM_PORTABLE.Droid.UI
{
    [Activity(Label = "Energy Management", MainLauncher = false, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
    class LoginActivity : Activity
    {
        const string TAG = "LoginActivity";
        private Button buttonSignIn;
        private Button buttonSignUp;
        private ProgressBar progressBar;
        private TextView tvForgotPassword;
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login_view);
            if (string.IsNullOrEmpty(PreferenceHandler.GetConfig()))
            {
                StartActivity(new Intent(Application.Context, typeof(ConfigActivity)));
                Finish();
            }
            
            buttonSignIn = FindViewById<Button>(Resource.Id.SignInButton);
            buttonSignUp = FindViewById<Button>(Resource.Id.SignUpButton);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            tvForgotPassword = FindViewById<TextView>(Resource.Id.textViewForgotPassword);
            progressBar.Visibility = ViewStates.Gone;
            tvForgotPassword.Click += delegate
            {
                Log.Debug(TAG, "ForgotPassword()");
                if (!Utils.Utils.IsNetworkEnabled(this))
                {
                    RunOnUiThread(() =>
                    {
                        Utils.Utils.ShowDialog(this, "Internet not available.");
                    });
                }
                else
                {
                    StartActivity(new Intent(Application.Context, typeof(ForgotPasswordActivity)));
                }
            };
            buttonSignIn.Click += delegate
            {
                Log.Debug(TAG, "Login()");
                if (!Utils.Utils.IsNetworkEnabled(this))
                {
                    RunOnUiThread(() =>
                    {
                        Utils.Utils.ShowDialog(this, "Internet not available.");
                    });
                }
                else
                {
                    Intent intent = new Intent(Application.Context, typeof(LoginNewActivity));
                    intent.PutExtra(LoginNewActivity.KEY_SHOW_PAGE, (int)SignInType.SIGN_IN);
                    StartActivity(intent);
                    Finish();
                }
            };
            buttonSignUp.Click += ButtonSignUp_Click;
        }

        private void ButtonSignUp_Click(object sender, EventArgs e)
        {
            if (!Utils.Utils.IsNetworkEnabled(this))
            {
                RunOnUiThread(() =>
                {
                    Utils.Utils.ShowDialog(this, "Internet not available.");
                });

            }
            else
            {
                Intent intent = new Intent(Application.Context, typeof(LoginNewActivity));
                intent.PutExtra(LoginNewActivity.KEY_SHOW_PAGE, (int)SignInType.SIGN_UP);
                StartActivity(intent);
                Finish();
            }
        }

        
    }
}
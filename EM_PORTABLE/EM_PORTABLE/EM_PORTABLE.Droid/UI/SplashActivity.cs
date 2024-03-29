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
using System.Threading.Tasks;
using EM_PORTABLE.Droid.Utils;
using EM_PORTABLE.Models;
using EM_PORTABLE;
using EM_PORTABLE.Utils;
using Newtonsoft.Json;
using Android.Content.PM;
using Microsoft.Identity.Client;
using UserDetailsClient;

namespace EM_PORTABLE.Droid.UI
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, NoHistory = true)]
    class SplashActivity : Activity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            Log.Debug(TAG, "SplashActivity.OnCreate");
        }

        protected override void OnResume()
        {
            base.OnResume();

            Task startupWork = new Task(() =>
            {
                Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
                Task.Delay(3000);  // Simulate a bit of startup work.
                Log.Debug(TAG, "Working in the background - important stuff.");
            });

            startupWork.ContinueWith(async t =>
            {
                Log.Debug(TAG, "Work is finished.");


                if (string.IsNullOrEmpty(PreferenceHandler.GetDomainKey()))
                {
                    StartActivity(new Intent(Application.Context, typeof(ConfigActivity)));
                    Finish();
                }
                else
                {
                    InvokeApi.SetDomainUrl(PreferenceHandler.GetDomainKey());
                    if (string.IsNullOrEmpty(PreferenceHandler.GetConfig()))
                    {
                        StartActivity(new Intent(Application.Context, typeof(ConfigActivity)));
                        Finish();
                    }
                    else
                    {
                        var config = JsonConvert.DeserializeObject<B2CConfiguration>(PreferenceHandler.GetConfig());
                        B2CConfigManager.GetInstance().Initialize(config);
                        if (PreferenceHandler.IsLoggedIn())
                        {
                            await Utils.Utils.RefreshToken(this);
                            Intent intent = new Intent(Application.Context, typeof(AdminDashboardActivity));
                            intent.PutExtra(MainActivity.KEY_USER_ROLE, (int)Constants.USER_ROLE.ADMIN);
                            StartActivity(intent);
                            Finish();
                        }
                        else
                        {
                            StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                            Finish();
                        }
                    }
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }



    }
}
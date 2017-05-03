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
using CSU_PORTABLE.Droid.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
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

            Task startupWork = new Task(() => {
                Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
                Task.Delay(3000);  // Simulate a bit of startup work.
                Log.Debug(TAG, "Working in the background - important stuff.");
            });

            startupWork.ContinueWith(t => {
                Log.Debug(TAG, "Work is finished.");

                PreferenceHandler prefHandler = new PreferenceHandler();
                if (prefHandler.IsLoggedIn())
                {
                    UserDetails userDetails = prefHandler.GetUserDetails();
                    if (userDetails.RoleId == (int)Constants.USER_ROLE.ADMIN)
                    {
                        Intent intent = new Intent(Application.Context, typeof(MainActivity));
                        intent.PutExtra(MainActivity.KEY_USER_ROLE, (int)Constants.USER_ROLE.ADMIN);
                        StartActivity(intent);
                        Finish();
                    }
                    else
                    {
                        Intent intent = new Intent(Application.Context, typeof(MainActivity));
                        intent.PutExtra(MainActivity.KEY_USER_ROLE, (int)Constants.USER_ROLE.STUDENT);
                        StartActivity(intent);
                        Finish();
                    }
                } else
                {
                    StartActivity(new Intent(Application.Context, typeof(LoginNewActivity)));
                    Finish();
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }
    }
}
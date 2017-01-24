using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "Alerts", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyTheme")]
    public class AlertsActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AlertsView);
            // Create your application here
        }
    }
}
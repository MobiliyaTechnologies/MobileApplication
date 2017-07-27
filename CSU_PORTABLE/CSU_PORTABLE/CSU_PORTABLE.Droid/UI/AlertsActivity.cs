using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Android.Util;
using CSU_PORTABLE.Utils;
using CSU_PORTABLE.Droid.Utils;
using Newtonsoft.Json.Linq;
using CSU_PORTABLE.Models;
using Android.Support.V7.Widget;
using Android.Views;
using System.Net.Http;
using Android.Content.PM;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "Alerts", MainLauncher = false, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
    public class AlertsActivity : AppCompatActivity
    {

        const string TAG = "AlertsActivity";
        private TextView textViewLoading;
        LinearLayout layoutProgress;
        List<AlertModel> alertList = null;
        RecyclerView mRecyclerView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AlertsView);

            textViewLoading = FindViewById<TextView>(Resource.Id.textViewLoading);
            textViewLoading.Visibility = ViewStates.Gone;
            layoutProgress = FindViewById<LinearLayout>(Resource.Id.layout_progress);
            layoutProgress.Visibility = ViewStates.Visible;

            int userId = PreferenceHandler.GetUserDetails().UserId;
            if (userId != -1)
            {
                bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(this);
                if (isNetworkEnabled)
                {
                    GetAlertList(userId);
                }
                else
                {
                    Utils.Utils.ShowToast(this, "Please enable your internet connection !");
                    layoutProgress.Visibility = ViewStates.Gone;
                    textViewLoading.Visibility = ViewStates.Visible;
                }
            }
            else
            {
                Utils.Utils.ShowToast(this, "Invalid User Id. Please Login Again !");
                layoutProgress.Visibility = ViewStates.Gone;
                textViewLoading.Visibility = ViewStates.Visible;
            }

            ResetNotificationCount();
        }

        private void ResetNotificationCount()
        {
            PreferenceHandler.setUnreadNotificationCount(0);
        }

        private async void GetAlertList(int userId)
        {
            var response = await InvokeApi.Invoke(Constants.API_GET_ALL_ALERTS, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken());
            if (response.StatusCode != 0)
            {
                Log.Debug(TAG, "async Response : " + response.ToString());
                RunOnUiThread(() =>
                {
                    GetAlertListResponse(response);
                });
            }
        }

        private async void GetAlertListResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                string strContent = await restResponse.Content.ReadAsStringAsync();
                JArray array = JArray.Parse(strContent);
                alertList = array.ToObject<List<AlertModel>>();
                ShowAlerts();
            }
            else
            {
                Log.Debug(TAG, "getAlertListResponse() Failed");
                Utils.Utils.ShowToast(this, "Please try again later !");
                layoutProgress.Visibility = ViewStates.Gone;
                textViewLoading.Visibility = ViewStates.Visible;
            }
        }

        private void ShowAlerts()
        {
            if (alertList != null)
            {
                mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

                // Plug in the linear layout manager:
                RecyclerView.LayoutManager mLayoutManager = new LinearLayoutManager(this);
                mRecyclerView.SetLayoutManager(mLayoutManager);

                // Plug in my adapter:
                AlertListAdapter mAdapter = new AlertListAdapter(this, alertList, true);
                mRecyclerView.SetAdapter(mAdapter);

                layoutProgress.Visibility = ViewStates.Gone;
                textViewLoading.Visibility = ViewStates.Gone;
            }
            else
            {
                layoutProgress.Visibility = ViewStates.Gone;
                textViewLoading.Visibility = ViewStates.Visible;
            }
        }
       
    }
}
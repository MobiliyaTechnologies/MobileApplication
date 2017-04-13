using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using RestSharp;
using Android.Util;
using CSU_PORTABLE.Utils;
using CSU_PORTABLE.Droid.Utils;
using Newtonsoft.Json.Linq;
using CSU_PORTABLE.Models;
using Android.Support.V7.Widget;
using Android.Views;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "Alerts", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyTheme")]
    public class AlertsActivity : AppCompatActivity
    {

        const string TAG = "AlertsActivity";
        private TextView textViewLoading;
        LinearLayout layoutProgress;
        Toast toast;
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

            var preferenceHandler = new PreferenceHandler();
            int userId = preferenceHandler.GetUserDetails().User_Id;
            if (userId != -1)
            {
                bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(this);
                if (isNetworkEnabled)
                {
                    getAlertList(userId);
                }
                else
                {
                    Utils.Utils.ShowToast(this, "Please enable your internet connection !");
                    //ShowToast("Please enable your internet connection !");
                    layoutProgress.Visibility = ViewStates.Gone;
                    textViewLoading.Visibility = ViewStates.Visible;
                }
            }
            else
            {
                Utils.Utils.ShowToast(this, "Invalid User Id. Please Login Again !");
                //ShowToast("Invalid User Id. Please Login Again !");
                layoutProgress.Visibility = ViewStates.Gone;
                textViewLoading.Visibility = ViewStates.Visible;
            }

            ResetNotificationCount();
        }

        private void ResetNotificationCount()
        {
            PreferenceHandler prefs = new PreferenceHandler();
            prefs.setUnreadNotificationCount(0);
        }

        private void getAlertList(int userId)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            Log.Debug(TAG, "getAlertList()");

            var request = new RestRequest(Constants.API_GET_ALL_ALERTS + "/" + userId, Method.GET);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    RunOnUiThread(() =>
                    {
                        getAlertListResponse((RestResponse)response);
                    });
                }
            });
        }

        private void getAlertListResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());

                JArray array = JArray.Parse(restResponse.Content);
                alertList = array.ToObject<List<AlertModel>>();

                showAlerts();
            }
            else
            {
                Log.Debug(TAG, "getAlertListResponse() Failed");
                Utils.Utils.ShowToast(this, "Please try again later !");
                //ShowToast("Please try again later !");
                layoutProgress.Visibility = ViewStates.Gone;
                textViewLoading.Visibility = ViewStates.Visible;
            }
        }

        private void showAlerts()
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
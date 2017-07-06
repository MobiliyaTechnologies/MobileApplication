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
using CSU_PORTABLE.Models;
using Android.Support.V7.Widget;
using CSU_PORTABLE.Droid.Utils;
using CSU_PORTABLE.Utils;
using Android.Util;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Android.Support.V7.App;
using System.Net.Http;
using Android.Content.PM;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "Insights", MainLauncher = false, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
    public class InsightsActivity : AppCompatActivity
    {
        const string TAG = "InsightsActivity";
        private TextView textViewLoading;
        LinearLayout layoutProgress;
        List<AlertModel> alertList = null;
        RecyclerView mRecyclerView;
        //PreferenceHandler preferenceHandler;
        LinearLayout LayoutInsightData;
        TextView textViewConsumed;
        TextView textViewExpected;
        TextView textViewOverused;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //preferenceHandler = new PreferenceHandler();
            SetContentView(Resource.Layout.insights);

            textViewLoading = FindViewById<TextView>(Resource.Id.textViewLoading);
            textViewLoading.Visibility = ViewStates.Gone;
            layoutProgress = FindViewById<LinearLayout>(Resource.Id.layout_progress);
            layoutProgress.Visibility = ViewStates.Visible;

            LayoutInsightData = FindViewById<LinearLayout>(Resource.Id.layout_insight_data);
            textViewConsumed = FindViewById<TextView>(Resource.Id.tv_top_consumed);
            textViewExpected = FindViewById<TextView>(Resource.Id.tv_top_expected);
            textViewOverused = FindViewById<TextView>(Resource.Id.tv_top_overused);

            int userId = PreferenceHandler.GetUserDetails().UserId;
            if (userId != -1)
            {
                bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(this);
                if (isNetworkEnabled)
                {
                    ShowInsights(null);
                    GetInsights(userId);
                    GetRecommendationsList(userId);
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
        }

        private async void GetRecommendationsList(int userId)
        {
            Log.Debug(TAG, "getAlertList()");
            var response = await InvokeApi.Invoke(Constants.API_GET_RECOMMENDATIONS, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken());
            Console.WriteLine(response);
            if (response.StatusCode != 0)
            {
                Log.Debug(TAG, "async Response : " + response.ToString());
                RunOnUiThread(() =>
                {
                    GetRecommendationsListResponse(response);
                });
            }
        }

        private async void GetRecommendationsListResponse(HttpResponseMessage restResponse)
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
                Log.Debug(TAG, "GetRecommendationsListResponse() Failed");
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
                AlertListAdapter mAdapter = new AlertListAdapter(this, alertList, false);
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

        private async void GetInsights(int userId)
        {
            Log.Debug(TAG, "GetInsights()");
            var response = await InvokeApi.Invoke(Constants.API_GET_INSIGHT_DATA, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken());
            if (response.StatusCode != 0)
            {
                Log.Debug(TAG, "async Response : " + response.ToString());
                RunOnUiThread(() =>
                {
                    GetInsightDataResponse(response);
                });
            }
        }

        private async void GetInsightDataResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                string strContent = await restResponse.Content.ReadAsStringAsync();
                InsightDataModel response = JsonConvert.DeserializeObject<InsightDataModel>(strContent);
                ShowInsights(response);
            }
            else
            {
                Log.Debug(TAG, "Login Failed");
                ShowInsights(null);
            }
        }

        private void ShowInsights(InsightDataModel response)
        {
            if (response == null)
            {
                LayoutInsightData.Visibility = ViewStates.Gone;
            }
            else
            {
                LayoutInsightData.Visibility = ViewStates.Visible;
                textViewConsumed.Text = Convert.ToString(Math.Round((response.ConsumptionValue / 1000), 2)) + " K";
                textViewExpected.Text = Convert.ToString(Math.Round((response.PredictedValue / 1000), 2)) + " K";
                textViewOverused.Text = Convert.ToString(Math.Round((response.ConsumptionValue - response.PredictedValue) / 1000, 2));
            }
        }


    }
}
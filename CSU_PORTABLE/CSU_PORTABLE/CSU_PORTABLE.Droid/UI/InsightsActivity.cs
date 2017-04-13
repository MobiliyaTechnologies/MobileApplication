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
using RestSharp;
using CSU_PORTABLE.Utils;
using Android.Util;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Android.Support.V7.App;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "Insights", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyTheme")]
    public class InsightsActivity : AppCompatActivity
    {
        const string TAG = "InsightsActivity";
        private TextView textViewLoading;
        LinearLayout layoutProgress;
        //Toast toast;
        List<AlertModel> alertList = null;
        RecyclerView mRecyclerView;

        LinearLayout LayoutInsightData;
        TextView textViewConsumed;
        TextView textViewExpected;
        TextView textViewOverused;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.insights);

            textViewLoading = FindViewById<TextView>(Resource.Id.textViewLoading);
            textViewLoading.Visibility = ViewStates.Gone;
            layoutProgress = FindViewById<LinearLayout>(Resource.Id.layout_progress);
            layoutProgress.Visibility = ViewStates.Visible;

            LayoutInsightData = FindViewById<LinearLayout>(Resource.Id.layout_insight_data);
            textViewConsumed = FindViewById<TextView>(Resource.Id.tv_top_consumed);
            textViewExpected = FindViewById<TextView>(Resource.Id.tv_top_expected);
            textViewOverused = FindViewById<TextView>(Resource.Id.tv_top_overused);


            var preferenceHandler = new PreferenceHandler();
            int userId = preferenceHandler.GetUserDetails().User_Id;
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

        private void GetRecommendationsList(int userId)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            Log.Debug(TAG, "getAlertList()");

            var request = new RestRequest(Constants.API_GET_RECOMMENDATIONS + "/" + userId, Method.GET);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    RunOnUiThread(() =>
                    {
                        GetRecommendationsListResponse((RestResponse)response);
                    });
                }
            });
        }

        private void GetRecommendationsListResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());

                JArray array = JArray.Parse(restResponse.Content);
                alertList = array.ToObject<List<AlertModel>>();

                ShowAlerts();
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

        private void GetInsights(int userId)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            Log.Debug(TAG, "GetInsights()");

            var request = new RestRequest(Constants.API_GET_INSIGHT_DATA + "/" + userId, Method.GET);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    RunOnUiThread(() =>
                    {
                        GetInsightDataResponse((RestResponse)response);
                    });
                }
            });
        }

        private void GetInsightDataResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                InshghtDataModel response = JsonConvert.DeserializeObject<InshghtDataModel>(restResponse.Content);


                ShowInsights(response);
            }
            else
            {
                Log.Debug(TAG, "Login Failed");
                ShowInsights(null);
            }
        }

        private void ShowInsights(InshghtDataModel response)
        {
            if (response == null)
            {
                LayoutInsightData.Visibility = ViewStates.Gone;
            }
            else
            {

                LayoutInsightData.Visibility = ViewStates.Visible;

                textViewConsumed.Text = "" + response.ConsumptionValue;
                textViewExpected.Text = "" + response.PredictedValue;
                float ovr = response.ConsumptionValue - response.PredictedValue;
                //textViewOverused.Text = "" + ((ovr < 0 ? 0 : ovr));
                textViewOverused.Text = ovr + "";
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
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

namespace CSU_PORTABLE.Droid.UI
{
    class StudentFragment : Fragment
    {
        const string TAG = "StudentFragment";
        private TextView textViewInfo;
        LinearLayout layoutProgress;
        Toast toast;
        List<ClassRoomModel> classList = null;
        RecyclerView mRecyclerView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.student_dashboard, container, false);

            textViewInfo = view.FindViewById<TextView>(Resource.Id.textViewInfo);
            textViewInfo.Visibility = ViewStates.Gone;
            layoutProgress = view.FindViewById<LinearLayout>(Resource.Id.layout_progress);
            layoutProgress.Visibility = ViewStates.Visible;
            mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            var preferenceHandler = new PreferenceHandler();
            int userId = preferenceHandler.GetUserDetails().User_Id;
            if (userId != -1)
            {
                bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(this.Activity);
                if (isNetworkEnabled)
                {
                    getClassList(userId);
                }
                else
                {
                    ShowToast("Please enable your internet connection !");
                    layoutProgress.Visibility = ViewStates.Gone;
                    textViewInfo.Visibility = ViewStates.Visible;
                }
            }
            else
            {
                ShowToast("Invalid User Id. Please Login Again !");
                layoutProgress.Visibility = ViewStates.Gone;
                textViewInfo.Visibility = ViewStates.Visible;
            }

            return view;
        }

        private void getClassList(int userId)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            Log.Debug(TAG, "getAlertList()");

            var request = new RestRequest(Constants.API_GET_CLASS_ROOMS + "/" + userId, Method.GET);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    this.Activity.RunOnUiThread(() => {
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
                classList = array.ToObject<List<ClassRoomModel>>();

                showAlerts();
            }
            else
            {
                Log.Debug(TAG, "getAlertListResponse() Failed");
                ShowToast("getAlertListResponse() Failed");
                layoutProgress.Visibility = ViewStates.Gone;
                textViewInfo.Visibility = ViewStates.Visible;
            }
        }

        private void showAlerts()
        {
            if (classList != null)
            {

                // Plug in the linear layout manager:
                RecyclerView.LayoutManager mLayoutManager = new LinearLayoutManager(this.Activity);
                mRecyclerView.SetLayoutManager(mLayoutManager);

                // Plug in my adapter:
                StudentDashboardAdapter mAdapter = new StudentDashboardAdapter(classList);
                mRecyclerView.SetAdapter(mAdapter);

                layoutProgress.Visibility = ViewStates.Gone;
                textViewInfo.Visibility = ViewStates.Gone;
            }
            else
            {
                layoutProgress.Visibility = ViewStates.Gone;
                textViewInfo.Visibility = ViewStates.Visible;
            }
        }

        private void ShowToast(string message)
        {
            if (toast != null)
            {
                toast.Cancel();
            }
            toast = Toast.MakeText(this.Activity, message, ToastLength.Short);
            toast.Show();
        }
    }
}
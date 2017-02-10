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
using Android.Support.V7.Widget;
using CSU_PORTABLE.Models;
using static Android.Views.View;
using RestSharp;
using Android.Util;
using CSU_PORTABLE.Utils;
using CSU_PORTABLE.Droid.Utils;
using Newtonsoft.Json;
using Android.Content.Res;
using Android.Graphics;
using Java.Text;

namespace CSU_PORTABLE.Droid.UI
{
    class AlertListAdapter : RecyclerView.Adapter
    {
        const string TAG = "AlertListAdapter";
        public List<AlertModel> mAlertModels;
        Context mContext;
        Toast toast;

        public AlertListAdapter(Context context, List<AlertModel> alertModels)
        {
            mContext = context;
            mAlertModels = alertModels;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.alert_item, parent, false);
            AlertViewHolder vh = new AlertViewHolder(itemView);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AlertViewHolder vh = holder as AlertViewHolder;
            vh.textViewAlert.Text = mAlertModels[position].Alert_Desc;
            vh.textViewClass.Text = mAlertModels[position].Class_Desc;
            string dt = getFormatedDate(mAlertModels[position].Timestamp);
            if (dt == null)
            {
                dt = mAlertModels[position].Timestamp;
            }
            vh.textViewTime.Text = dt;
            vh.alertId = mAlertModels[position].Alert_Id;
            if(mAlertModels[position].Is_Acknowledged)
            {
                vh.textViewAck.Visibility = ViewStates.Gone;
                vh.divider.Visibility = ViewStates.Gone;
            } else
            {
                vh.textViewAck.Visibility = ViewStates.Visible;
                vh.textViewAck.Click += delegate {

                    var preferenceHandler = new PreferenceHandler();
                    UserDetails userDetails = preferenceHandler.GetUserDetails();
                    int userId = userDetails.User_Id;

                    AlertAcknowledgeModel ackModel = new AlertAcknowledgeModel();
                    ackModel.Alert_Id = vh.alertId;
                    ackModel.Acknowledged_By = userDetails.First_Name + " " + userDetails.Last_Name;

                    vh.textViewAck.SaveEnabled = false;
                    vh.textViewAck.SetTextColor(Color.LightGray);
                    mAlertModels[position].Is_Acknowledged = true;

                    bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(mContext);
                    if (isNetworkEnabled)
                    {
                        acknowledgeAlert(ackModel, userId);
                    }
                    else
                    {
                        ShowToast("Please enable your internet connection !");
                    }
                };
            }
        }

        public override int ItemCount
        {
            get { return mAlertModels.Count(); }
        }

        //view holder class
        public class AlertViewHolder : RecyclerView.ViewHolder
        {
            public TextView textViewAlert { get; set; }
            public TextView textViewClass { get; set; }
            public TextView textViewTime { get; set; }
            public TextView textViewAck { get; set; }
            public View divider { get; set; }

            public int alertId;

            public AlertViewHolder(View itemView) : base (itemView)
            {
                textViewAlert = itemView.FindViewById<TextView>(Resource.Id.textViewAlert);
                textViewClass = itemView.FindViewById<TextView>(Resource.Id.textViewClass);
                textViewTime = itemView.FindViewById<TextView>(Resource.Id.textViewTime);
                textViewAck = itemView.FindViewById<TextView>(Resource.Id.textViewAcknowledge);
                divider = itemView.FindViewById(Resource.Id.divider);
            }
        }
        
        private string getFormatedDate(string date)
        {
            //string format = "MM/dd/yyyy HH:mm:ss";
            string format = "dd MMM hh:mm a";
            SimpleDateFormat sdf = new SimpleDateFormat(format);
            string dt = sdf.Format(new Java.Util.Date(date));
            return dt;
        }

        private void acknowledgeAlert(AlertAcknowledgeModel acknowledgeModel, int userId)
        {
            if (userId != -1)
            {
                RestClient client = new RestClient(Constants.SERVER_BASE_URL);
                Log.Debug(TAG, "getAlertList()");

                var request = new RestRequest(Constants.API_ACKNOWLWDGE_ALERTS + "/" + userId, Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(acknowledgeModel);

                client.ExecuteAsync(request, response =>
                {
                    Console.WriteLine(response);
                    if (response.StatusCode != 0)
                    {
                        Log.Debug(TAG, "async Response : " + response.ToString());
                        //RunOnUiThread(() => {
                        acknowledgeAlertResponse((RestResponse)response);
                        //});
                    }
                });
            }
            else
            {
                Log.Debug(TAG, "Invalid User Id. Please Login Again !");
                ShowToast("Invalid User Id. Please Login Again !");
            }
        }

        private void acknowledgeAlertResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());

                AlertAcknowledgementResponseModel response = JsonConvert.DeserializeObject<AlertAcknowledgementResponseModel>(restResponse.Content);

                if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    Log.Debug(TAG, "acknowledgement Successful");
                    ShowToast("Acknowlwdged successfully.");
                    //update list

                }
                else
                {
                    Log.Debug(TAG, "Acknowledgement Failed");
                    ShowToast("Failed to acknowlwdge. Please try later !");
                }
            }
            else
            {
                Log.Debug(TAG, "acknowledgeAlertResponse() Failed");
                ShowToast("Failed to acknowlwdge. Please try later !");
            }
        }

        private void ShowToast(string message)
        {
            if (toast != null)
            {
                toast.Cancel();
            }
            toast = Toast.MakeText(mContext, message, ToastLength.Short);
            toast.Show();
        }
    }
}
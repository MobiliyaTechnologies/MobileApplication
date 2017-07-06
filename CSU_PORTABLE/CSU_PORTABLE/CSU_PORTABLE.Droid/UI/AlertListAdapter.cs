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
using Android.Util;
using CSU_PORTABLE.Utils;
using CSU_PORTABLE.Droid.Utils;
using Newtonsoft.Json;
using Android.Content.Res;
using Android.Graphics;
using Java.Text;
using System.Net.Http;

namespace CSU_PORTABLE.Droid.UI
{
    class AlertListAdapter : RecyclerView.Adapter
    {
        const string TAG = "AlertListAdapter";
        public List<AlertModel> mAlertModels;
        Context mContext;
        bool isAcknowledgementEnabled = true;

        public AlertListAdapter(Context context, List<AlertModel> alertModels, bool enableAcknowledgement)
        {
            mContext = context;
            mAlertModels = alertModels;
            isAcknowledgementEnabled = enableAcknowledgement;
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
            string dt = GetFormatedDate(mAlertModels[position].Timestamp);
            if (dt == null)
            {
                dt = mAlertModels[position].Timestamp;
            }
            vh.textViewTime.Text = dt;
            vh.alertId = mAlertModels[position].Alert_Id;
            if (!isAcknowledgementEnabled || mAlertModels[position].Is_Acknowledged)
            {
                vh.textViewAck.Visibility = ViewStates.Gone;
                vh.divider.Visibility = ViewStates.Gone;
            }
            else
            {
                vh.textViewAck.Visibility = ViewStates.Visible;
                vh.textViewAck.Click += delegate
                {

                    //var preferenceHandler = new PreferenceHandler();
                    UserDetails userDetails = PreferenceHandler.GetUserDetails();
                    int userId = userDetails.UserId;

                    AlertAcknowledgeModel ackModel = new AlertAcknowledgeModel();
                    ackModel.Alert_Id = vh.alertId;
                    ackModel.Acknowledged_By = userDetails.FirstName + " " + userDetails.LastName;

                    vh.textViewAck.SaveEnabled = false;
                    vh.textViewAck.SetTextColor(Color.LightGray);
                    mAlertModels[position].Is_Acknowledged = true;

                    bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(mContext);
                    if (isNetworkEnabled)
                    {
                        AcknowledgeAlert(ackModel, userId);
                    }
                    else
                    {
                        Utils.Utils.ShowToast(mContext, "Please enable your internet connection !");
                        //ShowToast("Please enable your internet connection !");
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

            public AlertViewHolder(View itemView) : base(itemView)
            {
                textViewAlert = itemView.FindViewById<TextView>(Resource.Id.textViewAlert);
                textViewClass = itemView.FindViewById<TextView>(Resource.Id.textViewClass);
                textViewTime = itemView.FindViewById<TextView>(Resource.Id.textViewTime);
                textViewAck = itemView.FindViewById<TextView>(Resource.Id.textViewAcknowledge);
                divider = itemView.FindViewById(Resource.Id.divider);
            }
        }

        private string GetFormatedDate(string date)
        {
            //string format = "MM/dd/yyyy HH:mm:ss";
            string format = "dd MMM hh:mm a";
            SimpleDateFormat sdf = new SimpleDateFormat(format);
            string dt = sdf.Format(new Java.Util.Date(date));
            return dt;
        }

        private async void AcknowledgeAlert(AlertAcknowledgeModel acknowledgeModel, int userId)
        {
            if (userId != -1)
            {
                Log.Debug(TAG, "getAlertList()");
                var response = await InvokeApi.Invoke(Constants.API_ACKNOWLWDGE_ALERTS + "/" + userId, JsonConvert.SerializeObject(acknowledgeModel), HttpMethod.Post);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    AcknowledgeAlertResponse(response);
                }
            }
            else
            {
                Log.Debug(TAG, "Invalid User Id. Please Login Again !");
                Utils.Utils.ShowToast(mContext, "Invalid User Id. Please Login Again !");
                //ShowToast("Invalid User Id. Please Login Again !");
            }
        }

        private async void AcknowledgeAlertResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                string strContent = await restResponse.Content.ReadAsStringAsync();
                AlertAcknowledgementResponseModel response = JsonConvert.DeserializeObject<AlertAcknowledgementResponseModel>(strContent);

                if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    Log.Debug(TAG, "acknowledgement Successful");
                    Utils.Utils.ShowToast(mContext, "Acknowlwdged successfully.");
                }
                else
                {
                    Log.Debug(TAG, "Acknowledgement Failed");
                    Utils.Utils.ShowToast(mContext, "Failed to acknowlwdge. Please try later !");
                }
            }
            else
            {
                Log.Debug(TAG, "acknowledgeAlertResponse() Failed");
                Utils.Utils.ShowToast(mContext, "Failed to acknowlwdge. Please try later !");
            }
        }
    }
}
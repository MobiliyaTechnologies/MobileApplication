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
using EM_PORTABLE.Models;
using static Android.Views.View;
using Android.Util;
using EM_PORTABLE.Utils;
using EM_PORTABLE.Droid.Utils;
using Newtonsoft.Json;
using Android.Content.Res;
using Android.Graphics;
using Java.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace EM_PORTABLE.Droid.UI
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
            AlertViewHolder vhAlerts = new AlertViewHolder(itemView);
            return vhAlerts;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AlertViewHolder vhAlerts = holder as AlertViewHolder;
            vhAlerts.textViewAlert.Text = mAlertModels[position].Alert_Desc;
            vhAlerts.textViewClass.Text = mAlertModels[position].Class_Desc;

            string dt = GetFormatedDate(mAlertModels[position].Timestamp);
            if (dt == null)
            {
                dt = mAlertModels[position].Timestamp;
            }
            vhAlerts.textViewTime.Text = dt;
            vhAlerts.alertId = mAlertModels[position].Alert_Id;
            if (!isAcknowledgementEnabled || mAlertModels[position].Is_Acknowledged)
            {
                vhAlerts.textViewAck.Visibility = ViewStates.Gone;
                vhAlerts.divider.Visibility = ViewStates.Gone;
            }
            else
            {

                vhAlerts.textViewAck.Click += async delegate
                {
                    vhAlerts.textViewAck.SystemUiVisibility = StatusBarVisibility.Hidden;
                    vhAlerts.textViewAck.Visibility = ViewStates.Gone;
                    vhAlerts.textViewAck.Text = string.Empty;
                    AlertAcknowledgeModel ackModel = new AlertAcknowledgeModel();
                    ackModel.Alert_Id = vhAlerts.alertId;
                    ackModel.Acknowledged_By = PreferenceHandler.GetUserDetails().FirstName + " " + PreferenceHandler.GetUserDetails().LastName;

                    vhAlerts.textViewAck.SaveEnabled = false;
                    vhAlerts.textViewAck.SetTextColor(Color.LightGray);
                    mAlertModels[position].Is_Acknowledged = true;

                    bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(mContext);
                    if (isNetworkEnabled)
                    {
                        var restResponse = await AcknowledgeAlert(ackModel);
                        if (restResponse.StatusCode != 0)
                        {
                            Log.Debug(TAG, "async Response : " + restResponse.ToString());
                            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
                            {
                                Log.Debug(TAG, restResponse.Content.ToString());
                                string strContent = await restResponse.Content.ReadAsStringAsync();
                                AlertAcknowledgementResponseModel responseAlert = JsonConvert.DeserializeObject<AlertAcknowledgementResponseModel>(strContent);

                                if (responseAlert.Status_Code != Constants.STATUS_CODE_SUCCESS)
                                {
                                    Log.Debug(TAG, "Acknowledged Failed");
                                    Utils.Utils.ShowToast(mContext, "Failed to Acknowledged. Please try later !");
                                }
                            }
                            else
                            {
                                Log.Debug(TAG, "acknowledgeAlertResponse() Failed");
                                Utils.Utils.ShowToast(mContext, "Failed to Acknowledged. Please try later !");

                            }
                        }
                        else
                        {
                            vhAlerts.textViewAck.Visibility = ViewStates.Visible;
                        }
                    }
                    else
                    {
                        Utils.Utils.ShowToast(mContext, "Please enable your internet connection !");
                    }
                };
            }
        }

        public override int ItemCount
        {
            get { return mAlertModels.Count(); }
        }

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

        private async Task<HttpResponseMessage> AcknowledgeAlert(AlertAcknowledgeModel acknowledgeModel)
        {
            return await InvokeApi.Invoke(Constants.API_ACKNOWLWDGE_ALERTS, JsonConvert.SerializeObject(acknowledgeModel), HttpMethod.Put, PreferenceHandler.GetToken());
        }

    }
}
using CoreGraphics;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class AlertsViewController : BaseController
    {
        //PreferenceHandler prefHandler = null;
        UserDetails User = null;
        LoadingOverlay loadingOverlay;
        private AlertsSource alertsSource;

        public AlertsViewController(IntPtr handle) : base(handle)
        {
            //this.prefHandler = new PreferenceHandler();
            User = PreferenceHandler.GetUserDetails();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var bounds = UIScreen.MainScreen.Bounds;
            // show the loading overlay on the UI thread using the correct orientation sizing
            loadingOverlay = new LoadingOverlay(bounds);
            View.Add(loadingOverlay);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            GetAlerts();

        }

        public async void GetAlerts()
        {
            var response = await InvokeApi.Invoke(Constants.API_GET_ALL_ALERTS, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken());
            if (response.StatusCode != 0)
            {
                InvokeOnMainThread(() =>
                {
                    CheckAlertsResponse(response);
                    loadingOverlay.Hide();
                });
            }
            //RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            //var request = new RestRequest(Constants.API_GET_ALL_ALERTS + "/" + User.User_Id, Method.GET);
            //request.RequestFormat = DataFormat.Json;
            //client.ExecuteAsync(request, response =>
            //{
            //    Console.WriteLine(response);

            //});
        }

        public void AcknowledgeAlert(string Alert_Id)
        {
            //ShowMessage(alertId.Text);
        }


        public async void CheckAlertsResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                string strContent = await restResponse.Content.ReadAsStringAsync();
                List<AlertModel> alertsList = JsonConvert.DeserializeObject<List<AlertModel>>(strContent);
                if (alertsList.Count > 0)
                {
                    BindAlerts(alertsList);
                }
                else
                {
                    UILabel lblRemark = new UILabel()
                    {
                        Frame = new CGRect(0, this.NavigationController.NavigationBar.Bounds.Bottom + 20, View.Bounds.Width, 40),
                        Text = "No alerts found!",
                        Font = UIFont.FromName("Futura-Medium", 15f),
                        TextColor = UIColor.White,
                        BackgroundColor = UIColor.FromRGB(0, 102, 153),
                        LineBreakMode = UILineBreakMode.WordWrap,
                        Lines = 1,
                        TextAlignment = UITextAlignment.Center
                    };
                    View.AddSubviews(lblRemark);
                }
            }
            else
            {
                IOSUtil.ShowMessage("No Alerts.", loadingOverlay, this);
            }

        }

        private void BindAlerts(List<AlertModel> alertsList)
        {
            UITableView _table;

            _table = new UITableView
            {
                Frame = new CoreGraphics.CGRect(0, 65, View.Bounds.Width, View.Bounds.Height - 65),
                RowHeight = 100,
                Source = new AlertsSource(alertsList)
            };
            _table.ReloadData();
            View.AddSubview(_table);
        }


    }


}
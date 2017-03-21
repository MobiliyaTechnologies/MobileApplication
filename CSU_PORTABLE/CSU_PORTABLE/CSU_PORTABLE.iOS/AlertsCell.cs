using CoreGraphics;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;


namespace CSU_PORTABLE.iOS
{
    public class AlertsCell : UITableViewCell
    {
        UILabel lblSensorId, lblSensorLogId, lblClassId, lblClassDesc, lblAlert_Id, lblAlertType, LblDescription, lblTimeStamp, lblIsAcknowledged;
        UIImage SensorImage;
        UIButton Acknowledge;

        public AlertsCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            SelectionStyle = UITableViewCellSelectionStyle.Gray;
            ContentView.BackgroundColor = UIColor.FromRGB(245, 245, 245);


            lblAlert_Id = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear
            };
            lblSensorId = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear
            };
            lblSensorLogId = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear
            };
            lblClassId = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear
            };
            lblClassDesc = new UILabel()
            {
                Font = UIFont.FromName("Futura-Medium", 12f),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear
            };
            lblAlertType = new UILabel()
            {
                Font = UIFont.FromName("Futura-Medium", 12f),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear
            };
            LblDescription = new UILabel()
            {
                Font = UIFont.FromName("Futura-Medium", 15f),
                TextColor = UIColor.DarkTextColor,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Left,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
            };
            lblTimeStamp = new UILabel()
            {
                Font = UIFont.FromName("Futura-Medium", 12f),
                TextColor = UIColor.DarkTextColor,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Right,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
            };
            lblIsAcknowledged = new UILabel()
            {
                Font = UIFont.FromName("Futura-Medium", 12f),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear
            };

            Acknowledge = new UIButton(UIButtonType.RoundedRect)
            {
                Font = UIFont.FromName("Futura-Medium", 12f),
                BackgroundColor = UIColor.Clear,
            };
            Acknowledge.SetTitleColor(UIColor.Blue, UIControlState.Normal);
            Acknowledge.SetTitleColor(UIColor.Blue, UIControlState.Selected);
            Acknowledge.TouchUpInside += Acknowledge_TouchUpInside;

            ContentView.AddSubviews(new UIView[] { lblAlert_Id, lblSensorId, lblSensorLogId, lblClassId, lblClassDesc, lblAlertType, LblDescription, lblTimeStamp, lblIsAcknowledged, Acknowledge });
            //ContentView.AddSubviews(new UIView[] { lblAlert_Id, lblSensorId, lblSensorLogId, LblDescription, lblTimeStamp, Acknowledge });
        }




        #region "Acknowledge Alert"

        private void Acknowledge_TouchUpInside(object sender, EventArgs e)
        {
            var selectedRow = Acknowledge.Tag;
            var subView = (UILabel)ContentView.Subviews[0];
            var superView = Acknowledge.Superview;
            var preferenceHandler = new PreferenceHandler();
            UserDetails userDetails = preferenceHandler.GetUserDetails();
            int userId = userDetails.User_Id;
            AlertAcknowledgeModel ackModel = new AlertAcknowledgeModel();
            ackModel.Alert_Id = Convert.ToInt32(subView.Text);
            ackModel.Acknowledged_By = userDetails.First_Name + " " + userDetails.Last_Name;

            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            var request = new RestRequest(Constants.API_ACKNOWLWDGE_ALERTS + "/" + userId, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(ackModel);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    //AcknowledgeAlertResponse((RestResponse)response);
                }
            });
        }

        #endregion



        public void UpdateCell(string sensorId, int sensorLogId, string classId, string classDesc, string alertType, string Description, string AlertTimestamp, bool isAcknowledged, int Alert_Id)
        {
            lblAlert_Id.Text = Convert.ToString(Alert_Id);
            lblSensorId.Text = sensorId;
            lblSensorLogId.Text = Convert.ToString(sensorLogId);
            lblClassId.Text = classId;
            lblClassDesc.Text = classDesc;
            lblAlertType.Text = alertType;
            LblDescription.Text = Description;
            lblTimeStamp.Text = Convert.ToDateTime(AlertTimestamp).ToString("MM/dd/yyyy hh:mm:ss");
            //lblTimeStamp.Text = AlertTimestamp;
            lblIsAcknowledged.Text = sensorId;
            Acknowledge.Hidden = isAcknowledged;

            //Acknowledge.Tag = Alert_Id;
            Acknowledge.SetTitle("Acknowledge", UIControlState.Normal);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            LblDescription.Frame = new CGRect(20, 5, ContentView.Bounds.Width - 40, ContentView.Bounds.Height - 40);
            //lblClassDesc.Frame = new CGRect(10, 50, ContentView.Bounds.Width - 150, 10);
            lblTimeStamp.Frame = new CGRect((ContentView.Bounds.Width / 2), ContentView.Bounds.Height - 30, (ContentView.Bounds.Width / 2) - 20, 25);
            Acknowledge.Frame = new CGRect(20, ContentView.Bounds.Height - 30, (ContentView.Bounds.Width / 2) - 40, 25);
        }
    }
}

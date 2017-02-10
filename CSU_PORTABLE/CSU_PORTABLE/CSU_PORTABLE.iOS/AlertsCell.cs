using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;


namespace CSU_PORTABLE.iOS
{
    public class AlertsCell : UITableViewCell
    {
        UILabel lblSensorId, lblSensorLogId, lblClassId, lblClassDesc, lblAlertType, LblDescription, lblTimeStamp, lblIsAcknowledged;
        UIImage SensorImage;
        UIButton Acknowledge;

        public AlertsCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            SelectionStyle = UITableViewCellSelectionStyle.Gray;
            ContentView.BackgroundColor = UIColor.FromRGB(245, 245, 245);

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
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear
            };
            lblAlertType = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear
            };
            LblDescription = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 15f),
                TextColor = UIColor.FromRGB(0, 120, 255),
                BackgroundColor = UIColor.Clear
            };
            lblTimeStamp = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear
            };
            lblIsAcknowledged = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear
            };

            Acknowledge = new UIButton()
            {
                Font = UIFont.FromName("AmericanTypewriter", 12f),
                BackgroundColor = UIColor.Blue
            };
            Acknowledge.TouchUpInside += Acknowledge_TouchUpInside;

            ContentView.AddSubviews(new UIView[] { lblSensorId, lblSensorLogId, lblClassId, lblClassDesc, lblAlertType, LblDescription, lblTimeStamp, lblIsAcknowledged, Acknowledge });

        }

        private void Acknowledge_TouchUpInside(object sender, EventArgs e)
        {

        }

        public void UpdateCell(string sensorId, int sensorLogId, string classId, string classDesc, string alertType, string Description, string AlertTimestamp, bool isAcknowledged)
        {
            lblSensorId.Text = sensorId;
            lblSensorLogId.Text = Convert.ToString(sensorLogId);
            lblClassId.Text = classId;
            lblClassDesc.Text = classDesc;
            lblAlertType.Text = alertType;
            LblDescription.Text = Description;
            lblTimeStamp.Text = Convert.ToDateTime(AlertTimestamp).ToString("dd/MM/yyyy hh:mm tt");
            lblIsAcknowledged.Text = sensorId;
            Acknowledge.SetTitle("Acknowledge", UIControlState.Normal);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            LblDescription.Frame = new CGRect(10, 5, ContentView.Bounds.Width, 30);
            //lblSensorId.Frame = new CGRect(10, 35, 150, 10);
            //lblSensorLogId.Frame = new CGRect(2, 2, 20, 10);
            //lblSensorLogId.Enabled = false;
            //lblAlertType.Frame = new CGRect(ContentView.Bounds.Width - 100, 35, 90, 10);
            lblClassDesc.Frame = new CGRect(10, 50, ContentView.Bounds.Width - 150, 10);
            lblTimeStamp.Frame = new CGRect(ContentView.Bounds.Width - 150, 50, 140, 10);
            //Acknowledge.Frame = new CGRect(10, 60, 140, 10);
            //lblIsAcknowledged.Frame = new CGRect(ContentView.Bounds.Width / 2, 32, ContentView.Bounds.Width / 2, 10);
            //this.AccessoryView = Acknowledge;
        }
    }
}

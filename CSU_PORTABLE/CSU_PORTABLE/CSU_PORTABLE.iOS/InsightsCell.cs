using CoreGraphics;
using CSU_PORTABLE.Models;
using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public class InsightsCell : UITableViewCell
    {
        UILabel lblInsightsDetails;
        //UILabel lblClassRoom;
        UILabel lblTimeStamp;

        public InsightsCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            SelectionStyle = UITableViewCellSelectionStyle.Default;
            ContentView.BackgroundColor = UIColor.FromRGB(245, 245, 245);

            UIView paddingView = new UIView(new CGRect(5, 5, 5, 5));

            lblInsightsDetails = new UILabel()
            {
                Font = UIFont.FromName("Futura-Medium", 14f),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Justified,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
            };

            //lblClassRoom = new UILabel()
            //{
            //    Font = UIFont.FromName("Futura-Medium", 14f),
            //    TextColor = UIColor.Black,
            //    BackgroundColor = UIColor.Clear,
            //    TextAlignment = UITextAlignment.Justified,
            //    Lines = 0,
            //    LineBreakMode = UILineBreakMode.WordWrap,
            //};

            lblTimeStamp = new UILabel()
            {
                Font = UIFont.FromName("Futura-Medium", 12f),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Justified,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
            };

            lblInsightsDetails.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
            ContentView.AddSubviews(lblTimeStamp, lblInsightsDetails);
        }

        public void UpdateCell(AlertModel insightText)
        {
            lblInsightsDetails.Text = insightText.Alert_Desc;
            //lblClassRoom.Text = insightText.Class_Id;
            //lblClassRoom.TextAlignment = UITextAlignment.Left;
            //DateTime result = DateTime.Now;
            //var timeStamp = DateTime.TryParse(insightText.Timestamp, out result);
            // lblTimeStamp.Text = Convert.ToDateTime(insightText.Timestamp).ToString("dd/MM/yyyy hh:mm tt");
            lblTimeStamp.Text = insightText.Timestamp;
            lblTimeStamp.TextAlignment = UITextAlignment.Right;
        }


        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            lblTimeStamp.Frame = new CGRect(ContentView.Bounds.Width / 2, ContentView.Bounds.Height - 30, (ContentView.Bounds.Width / 2) - 20, 20);
            //lblClassRoom.Frame = new CGRect(10, 5, (ContentView.Bounds.Width / 2) - 10, 20);
            lblInsightsDetails.Frame = new CGRect(20, 10, ContentView.Bounds.Width - 40, ContentView.Bounds.Height - 40);
            //lblInsightsDetails.SizeToFit();



        }
    }
}

using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public class ConsumptionCell : UITableViewCell
    {

        public ConsumptionCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            SelectionStyle = UITableViewCellSelectionStyle.Default;
            ContentView.BackgroundColor = UIColor.FromRGB(245, 245, 245);

            double insightsHeight = 500;

            UIButton btnInsights = new UIButton()
            {
                //Frame = new CGRect(0, insightsHeight + 20, View.Bounds.Width, 50),
                BackgroundColor = UIColor.FromRGB(228, 228, 228),
                Font = UIFont.FromName("Futura-Medium", 12f),
            };

            UILabel.Appearance.Font = UIFont.FromName("Futura-Medium", 20f);

            //double lblWidth = (View.Bounds.Width / 3) - 10;
            //string strConsumed = Convert.ToString(Math.Round(insightDM.ConsumptionValue / 1000, 2)) + " k";
            //string strExpected = Convert.ToString(Math.Round(insightDM.PredictedValue / 1000, 2)) + " k";
            //string strOverused = Convert.ToString(Math.Round((insightDM.ConsumptionValue - insightDM.PredictedValue) / 1000, 2)) + " k";

            UIImageView imgConsumed = new UIImageView()
            {
                Frame = new CGRect(5, 5, 10, 20),
                Image = UIImage.FromBundle("Arrow_Blue.png"),
            };

            UILabel lblConsumedCount = new UILabel()
            {
                //Frame = new CGRect(30, 0, lblWidth, 30),
                //Text = strConsumed,
                Font = UIFont.PreferredTitle2,
                TextColor = UIColor.DarkTextColor,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center

            };

            UIImageView imgExpected = new UIImageView()
            {
                Frame = new CGRect(5, 5, 10, 20),
                Image = UIImage.FromBundle("Arrow_Green.png"),
            };

            UILabel lblExpectedCount = new UILabel()
            {
                //Frame = new CGRect(lblWidth + 30, 0, lblWidth, 30),
                //Text = strExpected,
                Font = UIFont.PreferredTitle2,
                TextColor = UIColor.DarkTextColor,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };

            UIImageView imgOverused = new UIImageView()
            {
                Frame = new CGRect(5, 5, 10, 20),
                Image = UIImage.FromBundle("Arrow_Red.png"),
            };

            UILabel lblOverusedCount = new UILabel()
            {
                //Frame = new CGRect((lblWidth * 2) + 20, 0, lblWidth, 30),
                //Text = strOverused,
                Font = UIFont.PreferredTitle2,
                TextColor = UIColor.DarkTextColor,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };

            lblConsumedCount.AddSubview(imgConsumed);
            lblExpectedCount.AddSubview(imgExpected);
            lblOverusedCount.AddSubview(imgOverused);

            UILabel lblConsumed = new UILabel()
            {
                //Frame = new CGRect(10, 25, lblWidth, 30),
                Text = "CONSUMED",
                Font = UIFont.FromName("Futura-Medium", 10f),
                TextColor = UIColor.Gray,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };

            UILabel lblExpected = new UILabel()
            {
                //Frame = new CGRect(new CGPoint(lblWidth + 20, 25), new CGSize(lblWidth, 30)),
                Text = "EXPECTED",
                Font = UIFont.FromName("Futura-Medium", 10f),
                TextColor = UIColor.Gray,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };

            UILabel lblOverused = new UILabel()
            {
                //Frame = new CGRect(new CGPoint((lblWidth * 2) + 10, 25), new CGSize(lblWidth, 30)),
                Text = "OVERUSED",
                Font = UIFont.FromName("Futura-Medium", 10f),
                TextColor = UIColor.Gray,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };

            //lblOverused.Text = ((Math.Round((insightDM.ConsumptionValue - insightDM.PredictedValue) / 1000, 2)) > 0 ? "OVERUSED" : "UNDERUSED");

            btnInsights.AddSubviews(lblConsumed, lblExpected, lblOverused, lblConsumedCount, lblExpectedCount, lblOverusedCount);
            ContentView.AddSubviews(btnInsights);
        }
    }
}

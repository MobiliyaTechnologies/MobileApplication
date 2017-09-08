using CoreGraphics;
using EM_PORTABLE.iOS.Utils;
using EM_PORTABLE.Models;
using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace EM_PORTABLE.iOS
{
    public class ConsumptionCell : UITableViewCell
    {
        UIButton btnInsights;
        UILabel lblConsumptionName;
        UILabel lblConsumedCount;
        UILabel lblExpectedCount;
        UILabel lblOverusedCount;
        UILabel lblOverused;

        public ConsumptionCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            SelectionStyle = UITableViewCellSelectionStyle.Default;
            btnInsights = new UIButton()
            {
                Frame = new CGRect((IOSUtil.LayoutWidth - Bounds.Width) / 2, 0, Bounds.Width, 98),
                BackgroundColor = UIColor.Clear,
                Font = UIFont.FromName("Futura-Medium", 12f),
            };

            UILabel.Appearance.Font = UIFont.FromName("Futura-Medium", 20f);

            double lblWidth = (Bounds.Width / 3) - 10;
            UIImageView imgConsumed = new UIImageView()
            {
                Frame = new CGRect(-2, 5, 10, 20),
                Image = UIImage.FromBundle("Arrow_Blue.png"),
            };


            lblConsumptionName = new UILabel()
            {
                Frame = new CGRect(0, 5, Bounds.Width, 25),
                Font = UIFont.FromName("Futura-Medium", 20f),
                TextColor = UIColor.FromRGB(0, 102, 153),
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.Clip,
                Lines = 1,
                TextAlignment = UITextAlignment.Center
            };

            lblConsumedCount = new UILabel()
            {
                Frame = new CGRect(10, 35, lblWidth, 30),
                Font = UIFont.FromName("Futura-Medium", 18f),
                TextColor = UIColor.DarkTextColor,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center

            };

            UIImageView imgExpected = new UIImageView()
            {
                Frame = new CGRect(-2, 5, 10, 20),
                Image = UIImage.FromBundle("Arrow_Green.png"),
            };

            lblExpectedCount = new UILabel()
            {
                Frame = new CGRect(lblWidth + 12, 35, lblWidth, 30),
                Font = UIFont.FromName("Futura-Medium", 18f),
                TextColor = UIColor.DarkTextColor,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };

            //UIImageView imgOverused = new UIImageView()
            //{
            //    Frame = new CGRect(0, 5, 10, 20),
            //    Image = UIImage.FromBundle("Arrow_Red.png"),
            //    Tag = 1
            //};

            lblOverusedCount = new UILabel()
            {
                Frame = new CGRect((lblWidth * 2) + 12, 35, lblWidth, 30),
                Font = UIFont.FromName("Futura-Medium", 18f),
                TextColor = UIColor.DarkTextColor,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };

            lblConsumedCount.AddSubview(imgConsumed);
            lblExpectedCount.AddSubview(imgExpected);
            //lblOverusedCount.AddSubview(imgOverused);

            UILabel lblConsumed = new UILabel()
            {
                Frame = new CGRect(5, 60, lblWidth, 30),
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
                Frame = new CGRect(new CGPoint(lblWidth + 15, 60), new CGSize(lblWidth, 30)),
                Text = "EXPECTED",
                Font = UIFont.FromName("Futura-Medium", 10f),
                TextColor = UIColor.Gray,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };

            lblOverused = new UILabel()
            {
                Frame = new CGRect(new CGPoint((lblWidth * 2) + 20, 60), new CGSize(lblWidth, 30)),
                //Text = "OVERUSED",
                Font = UIFont.FromName("Futura-Medium", 10f),
                TextColor = UIColor.Gray,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };

            UIView seperator = new UIView()
            {
                Frame = new CGRect(0, 99, IOSUtil.LayoutWidth, 2),
                BackgroundColor = UIColor.Gray
            };

            btnInsights.AddSubviews(lblConsumptionName, lblConsumed, lblExpected, lblOverused, lblConsumedCount, lblExpectedCount, lblOverusedCount);
            ContentView.AddSubviews(btnInsights, seperator);
        }

        public void UpdateCell(ConsumptionModel consumptionText)
        {
            lblConsumptionName.Text = consumptionText.Name;
            lblConsumedCount.Text = consumptionText.Consumed;
            lblExpectedCount.Text = consumptionText.Expected;
            lblOverusedCount.Text = consumptionText.Overused;

            double overused = Convert.ToDouble(consumptionText.Overused.Replace('K', ' ').Trim());
            if (overused >= 0)
            {
                lblOverusedCount.Text = consumptionText.Overused;
                lblOverused.Text = "UNDERUSED";
                UIImageView ImgView = new UIImageView()
                {
                    Frame = new CGRect(-2, 5, 10, 20),
                    Image = UIImage.FromBundle("Arrow_Green_Down.png"),
                };

                lblOverusedCount.AddSubview(ImgView);
            }
            else
            {
                lblOverusedCount.Text = Convert.ToString((-1) * overused) + " K";
                lblOverused.Text = "OVERUSED";
                var ImgView = lblOverusedCount.ViewWithTag(1);
                UIImageView ImgViewRed = new UIImageView()
                {
                    Frame = new CGRect(-2, 5, 10, 20),
                    Image = UIImage.FromBundle("Arrow_Red.png"),
                };
                lblOverusedCount.AddSubview(ImgViewRed);
            }


        }


        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            btnInsights.SizeToFit();
        }
    }
}

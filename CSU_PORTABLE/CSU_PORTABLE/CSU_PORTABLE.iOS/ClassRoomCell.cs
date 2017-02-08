using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public class ClassRoomCell : UITableViewCell
    {
        UILabel lblClassRoomDesc, lblClassRoomId, lblSensorId;

        public ClassRoomCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            SelectionStyle = UITableViewCellSelectionStyle.Gray;
            ContentView.BackgroundColor = UIColor.LightGray;

            lblClassRoomDesc = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 18f),
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear
            };
            lblClassRoomId = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 15f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };
            lblSensorId = new UILabel()
            {
                Font = UIFont.FromName("AmericanTypewriter", 15f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };
            ContentView.AddSubviews(new UIView[] { lblClassRoomDesc, lblClassRoomId, lblSensorId });
        }

        public void UpdateCell(string classRoomDesc, string classRoomId, string sensorId)
        {
            lblClassRoomDesc.Text = classRoomDesc;
            lblClassRoomId.Text = classRoomId;
            lblSensorId.Text = sensorId;

        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            lblClassRoomDesc.Frame = new CGRect(10, 2, ContentView.Bounds.Width - 150, 40);
            lblClassRoomId.Frame = new CGRect(ContentView.Bounds.Width - 150, 4, 150, 15);
            lblSensorId.Frame = new CGRect(ContentView.Bounds.Width - 150, 20, 150, 15);
            lblClassRoomDesc.TextAlignment = UITextAlignment.Left;
            lblSensorId.TextAlignment = UITextAlignment.Left;
            lblClassRoomId.TextAlignment = UITextAlignment.Left;
        }
    }
}

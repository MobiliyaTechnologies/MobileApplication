using CoreGraphics;
using CSU_PORTABLE.Models;
using Foundation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public class InsightsSource : UITableViewSource
    {
        List<AlertModel> _insights;
        NSString cellIdentifier = (NSString)"InsightsCell";

        public InsightsSource(List<AlertModel> insights)
        {
            _insights = insights;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(cellIdentifier) as InsightsCell;
            if (cell == null)
                cell = new InsightsCell(cellIdentifier);
            cell.UpdateCell(_insights[indexPath.Row]);
            var a = cell.Bounds.Height;
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            //return base.GetHeightForRow(tableView, indexPath);
            //var text = _insights[indexPath.Row].Alert_Desc.StringSize(UIFont.FromName("Futura-Medium", 15f), new CGSize(tableView.Bounds.Width - 50, tableView.Bounds.Height - 20), UILineBreakMode.WordWrap);

            //return text.Height;
            double siz = MeasureTextSize(_insights[indexPath.Row].Alert_Desc, tableView.Bounds.Width - 40, 14f, "Futura-Medium");
            //return 100f;
            return (nfloat)siz;
        }

        public double MeasureTextSize(string text, double width, double fontSize, string fontName = null)
        {

            var nsText = new NSString(text);
            var boundSize = new SizeF((float)width, float.MaxValue);
            var options = NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin;

            if (fontName == null)
            {
                fontName = "HelveticaNeue";
            }
            var attributes = new UIStringAttributes
            {
                Font = UIFont.FromName(fontName, (float)fontSize)
            };
            var sizeF = nsText.GetBoundingRect(boundSize, options, attributes, null).Size;
            //return new Xamarin.Forms.Size((double)sizeF.Width, (double)sizeF.Height);
            return (double)sizeF.Height + 45;

        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _insights.Count;
        }
    }
}

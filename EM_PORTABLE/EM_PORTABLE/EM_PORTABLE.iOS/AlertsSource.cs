using System;
using System.Collections.Generic;
using System.Text;
using Foundation;
using UIKit;
using EM_PORTABLE.Models;
using System.Drawing;

namespace EM_PORTABLE.iOS
{
    public class AlertsSource : UITableViewSource
    {

        List<AlertModel> AlertsList = null;
        NSString cellIdentifier = (NSString)"AlertCell";

        public AlertsSource(List<AlertModel> alerts)
        {
            AlertsList = alerts;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {

            var cell = tableView.DequeueReusableCell(cellIdentifier) as AlertsCell;
            if (cell == null)
                cell = new AlertsCell(cellIdentifier);
            cell.UpdateCell("Sensor: " + AlertsList[indexPath.Row].Sensor_Id
                , AlertsList[indexPath.Row].Sensor_Log_Id
                , AlertsList[indexPath.Row].Class_Id
                , AlertsList[indexPath.Row].Class_Desc
                , AlertsList[indexPath.Row].Alert_Type
                , AlertsList[indexPath.Row].Alert_Desc
                , AlertsList[indexPath.Row].Timestamp
                , AlertsList[indexPath.Row].Is_Acknowledged
                , AlertsList[indexPath.Row].Alert_Id
                );

            return cell;

        }

        //public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        //{
        //    // base.RowSelected(tableView, indexPath);
        //    var AlertsViewController = new AlertsViewController(this);
        //    AlertsViewController.AcknowledgeAlert(tableView, indexPath);

        //}

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            //return base.GetHeightForRow(tableView, indexPath);
            //var text = _insights[indexPath.Row].Alert_Desc.StringSize(UIFont.FromName("Futura-Medium", 15f), new CGSize(tableView.Bounds.Width - 50, tableView.Bounds.Height - 20), UILineBreakMode.WordWrap);

            //return text.Height;
            double siz = MeasureTextSize(AlertsList[indexPath.Row].Alert_Desc, tableView.Bounds.Width - 40, 14f, "Futura-Medium");
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
            return (double)sizeF.Height + 50;

        }




        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return AlertsList.Count;
        }
    }
}

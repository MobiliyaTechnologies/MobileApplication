using System;
using System.Collections.Generic;
using System.Text;
using Foundation;
using UIKit;
using CSU_PORTABLE.Models;

namespace CSU_PORTABLE.iOS
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
            cell.UpdateCell("Sensor: "+ AlertsList[indexPath.Row].Sensor_Id
                , AlertsList[indexPath.Row].Sensor_Log_Id
                , AlertsList[indexPath.Row].Class_Id
                , AlertsList[indexPath.Row].Class_Desc
                , AlertsList[indexPath.Row].Alert_Type
                , AlertsList[indexPath.Row].Alert_Desc
                , AlertsList[indexPath.Row].Timestamp
                , AlertsList[indexPath.Row].Is_Acknowledged
                );

            return cell;

        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return AlertsList.Count;
        }
    }
}

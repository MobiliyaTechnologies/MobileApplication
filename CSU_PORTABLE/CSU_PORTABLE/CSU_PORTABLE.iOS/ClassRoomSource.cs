using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using CSU_PORTABLE.Models;
using Foundation;

namespace CSU_PORTABLE.iOS
{
    public class ClassRoomSource : UITableViewSource
    {
        List<ClassRoomModel> classRoomsList = null;
        NSString classRoomCellIdentifier = (NSString)"TableCell";

        public ClassRoomSource(List<ClassRoomModel> classRooms)
        {
            this.classRoomsList = classRooms;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(classRoomCellIdentifier) as ClassRoomCell;
            if (cell == null)
                cell = new ClassRoomCell(classRoomCellIdentifier);
            cell.UpdateCell(classRoomsList[indexPath.Row].ClassDescription
                , " Class Room Id:" + classRoomsList[indexPath.Row].ClassId
                , " Sensor Id:" + classRoomsList[indexPath.Row].SensorId);
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return classRoomsList.Count;
        }

        //public override string TitleForFooter(UITableView tableView, nint section)
        //{
        //    return base.TitleForFooter(tableView, section);
        //}

        //public override string TitleForHeader(UITableView tableView, nint section)
        //{
        //    return base.TitleForHeader(tableView, section);
        //}
    }
}

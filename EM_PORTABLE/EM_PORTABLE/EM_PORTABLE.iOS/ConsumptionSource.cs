using EM_PORTABLE.Models;
using EM_PORTABLE.Utils;
using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace EM_PORTABLE.iOS
{
    public class ConsumptionSource : UITableViewSource
    {
        List<ConsumptionModel> _consumptionModels;
        NSString cellIdentifier = (NSString)"ConsumptionCell";
        MapViewController _dashboard;


        public ConsumptionSource(List<ConsumptionModel> consumptionModels, MapViewController dashboard)
        {
            _consumptionModels = consumptionModels;
            _dashboard = dashboard;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(cellIdentifier) as ConsumptionCell;
            if (cell == null)
                cell = new ConsumptionCell(cellIdentifier);
            cell.UpdateCell(_consumptionModels[indexPath.Row]);
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (_dashboard.CurrentConsumption != ConsumptionFor.Meters)
            {
                switch (_dashboard.CurrentConsumption)
                {
                    case ConsumptionFor.Premises:
                        _dashboard.CurrentPremisesId = _consumptionModels[indexPath.Row].Id;
                        _dashboard.CurrentConsumption = ConsumptionFor.Buildings;
                        _dashboard.lblHeader.Text = ConsumptionFor.Buildings.ToString();
                        break;
                    case ConsumptionFor.Buildings:
                        _dashboard.CurrentConsumption = ConsumptionFor.Meters;
                        _dashboard.lblHeader.Text = ConsumptionFor.Meters.ToString();
                        break;
                }
                _dashboard.btnBack.Hidden = false;
                _dashboard.GetConsumptionDetails(_dashboard.CurrentConsumption, _consumptionModels[indexPath.Row].Id);
            }
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _consumptionModels.Count;
        }
    }
}
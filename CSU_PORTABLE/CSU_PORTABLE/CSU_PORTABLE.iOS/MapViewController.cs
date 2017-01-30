using CoreLocation;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using MapKit;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class MapViewController : UIViewController
    {
        MKMapView map;
        List<MeterDetails> meterList = null;
        List<MonthlyConsumptionDetails> monthlyConsumptionList = null;

        public MapViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            addPinAndCircle();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            map = new MKMapView(UIScreen.MainScreen.Bounds);
            map.MapType = MKMapType.Standard; //road map    
            map.ZoomEnabled = true;
            map.ScrollEnabled = true;

            CLLocationCoordinate2D coordinate = new CLLocationCoordinate2D(40.571276, -105.085522);

            MKCoordinateSpan span = new MKCoordinateSpan(0.004, 0.004);
            map.Region = new MKCoordinateRegion(coordinate, span);

            var mapViewDelegate = new MyMapDelegate();
            mapViewDelegate.AnnotationTapped += TheMapView_OnAnnotationTapped;
            map.Delegate = mapViewDelegate;
            
            View = map;

            var preferenceHandler = new PreferenceHandler();
            int userId = preferenceHandler.GetUserDetails().User_Id;
            if (userId != -1)
            {
                GetMeterDetails(userId);
                GetMonthlyConsumptionDetails(userId);
            }
            else
            {
                ShowMessage("Invalid User Id. Please Login Again !");
            }
        }

        private void TheMapView_OnAnnotationTapped(object sender, EventArgs args)
        {
            var annotView = sender as MKAnnotationView;
            if (annotView != null) { }
            var meterAnotation = annotView.Annotation as MKPointAnnotation;

            if (meterAnotation != null)
            {
                string serial = meterAnotation.Subtitle;
                if (serial != null && serial.Length > 0)
                {
                    ShowMeterReports(meterAnotation.Title, serial);
                }
            }
        }
        
        private void ShowMessage(string v)
        {
            UIAlertController alertController = UIAlertController.Create("Message", v, UIAlertControllerStyle.Alert);

            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));

            PresentViewController(alertController, true, null);

        }

        //for api call
        private void GetMonthlyConsumptionDetails(int userId)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            
            var request = new RestRequest(Constants.API_GET_MONTHLY_CONSUMPTION + "/" + userId, Method.GET);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    InvokeOnMainThread(() => {
                        GetMonthlyConsumptionResponse((RestResponse)response);
                    });
                }
            });
        }

        private void GetMeterDetails(int userId)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            
            var request = new RestRequest(Constants.API_GET_METER_LIST + "/" + userId, Method.GET);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    InvokeOnMainThread(() => {
                        GetMeterDetailsResponse((RestResponse)response);
                    });
                }
            });
        }

        private void GetMeterDetailsResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                JArray array = JArray.Parse(restResponse.Content);
                meterList = array.ToObject<List<MeterDetails>>();

                addPinAndCircle();
            }
            else
            {
                ShowMessage("Failed to get details. Please try again later.");
            }
        }

        private void GetMonthlyConsumptionResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                JArray array = JArray.Parse(restResponse.Content);
                monthlyConsumptionList = array.ToObject<List<MonthlyConsumptionDetails>>();

                addPinAndCircle();
            }
            else
            {
                ShowMessage("Failed to get details. Please try again later.");
            }
        }

        private void ShowMeterReports(string meterName, string meterSerial)
        {
            // Launches a new instance of CallHistoryController
            MeterReportController repotrView = this.Storyboard.InstantiateViewController("MeterReportController") as MeterReportController;
            if (repotrView != null)
            {
                repotrView.meterName = meterName;
                repotrView.meterSerialNumber = meterSerial;
                this.NavigationController.PushViewController(repotrView, true);
            }
        }

        private void addPinAndCircle()
        {
            if (meterList != null && monthlyConsumptionList != null && map != null)
            {
                IMKAnnotation[] an = map.Annotations;
                if (an != null)
                {
                    map.RemoveAnnotations(an);
                }
                IMKOverlay[] ov = map.Overlays;
                if (ov != null)
                {
                    map.RemoveOverlays(map.Overlays);
                }
                for (int i = 0; i < meterList.Count; i++)
                {
                    var meter = meterList[i];

                    CLLocationCoordinate2D coordinate = new CLLocationCoordinate2D(meter.Latitude, meter.Longitude);
                    map.AddAnnotations(new MKPointAnnotation()
                    {
                        Title = meter.Name,
                        Subtitle = meter.Serial,
                        Coordinate = coordinate
                    });

                    var circleOverlay = MKCircle.Circle(coordinate, getRadius(meter));
                    circleOverlay.Subtitle = meter.Serial;
                    map.AddOverlay(circleOverlay);
                }
            }
        }

        private double getRadius(MeterDetails meter)
        {
            double Monthly_KWH_Consumption = 0;

            for (int i = 0; i < monthlyConsumptionList.Count; i++)
            {
                if (monthlyConsumptionList[i].Powerscout.Equals(meter.Serial))
                {
                    Monthly_KWH_Consumption = monthlyConsumptionList[i].Monthly_KWH_Consumption;
                    break;
                }
            }

            double radius = 0;
            if (Monthly_KWH_Consumption == 0)
            {
                radius = 2;
            }
            else if (Monthly_KWH_Consumption > 0 && Monthly_KWH_Consumption <= 1000)
            {
                if (Monthly_KWH_Consumption < 500)
                {
                    //Minimum radius for the circle
                    radius = 10;
                }
                else
                {
                    radius = Monthly_KWH_Consumption / 50;
                }
            }
            else if (Monthly_KWH_Consumption > 1000 && Monthly_KWH_Consumption <= 10000)
            {
                if (Monthly_KWH_Consumption < 5250)
                {
                    //Minimum radius for the circle
                    radius = 21;
                }
                else
                {
                    radius = Monthly_KWH_Consumption / 250;
                }
            }
            else if (Monthly_KWH_Consumption > 10000 && Monthly_KWH_Consumption <= 38000)
            {
                if (Monthly_KWH_Consumption < 25625)
                {
                    //Minimum radius for the circle
                    radius = 41;
                }
                else
                {
                    radius = Monthly_KWH_Consumption / 625;
                }
            }
            else
            {
                if (Monthly_KWH_Consumption < 61000)
                {
                    //Minimum radius for the circle
                    radius = 61;
                }
                else
                {
                    radius = Monthly_KWH_Consumption / 1000;
                }
            }
            return radius;
        }
        
        //for overlay
        public class SearchResultsUpdator : UISearchResultsUpdating
        {
            public event Action<string> UpdateSearchResults = delegate { };

            public override void UpdateSearchResultsForSearchController(UISearchController searchController)
            {
                this.UpdateSearchResults(searchController.SearchBar.Text);
            }
        }

        class MyMapDelegate : MKMapViewDelegate
        {
            public event EventHandler AnnotationTapped;
            string pId = "PinAnnotation";

            public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
            {
                if (annotation is MKUserLocation)
                    return null;

                // create pin annotation view
                MKAnnotationView pinView = (MKPinAnnotationView)mapView.DequeueReusableAnnotation(pId);

                if (pinView == null)
                    pinView = new MKPinAnnotationView(annotation, pId);

                ((MKPinAnnotationView)pinView).PinColor = MKPinAnnotationColor.Red;
                pinView.CanShowCallout = true;
                pinView.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);
                return pinView;
            }

            public override void CalloutAccessoryControlTapped(MKMapView mapView, MKAnnotationView view, UIControl control)
            {

                if (AnnotationTapped != null)
                {
                    AnnotationTapped(view, new EventArgs());
                } 
            }

            public override MKOverlayView GetViewForOverlay(MKMapView mapView, IMKOverlay overlay)
            {
                var circleOverlay = overlay as MKCircle;
                var circleView = new MKCircleView(circleOverlay);
                var serial = circleOverlay.GetSubtitle();
                circleView.FillColor = getColor(serial);
                circleView.Alpha = 0.4f;
                return circleView;
            }

            private UIColor getColor(string meterName)
            {
                UIColor fillColor = UIColor.Blue;
                if (meterName.Equals("P371602077"))
                {
                    fillColor = UIColor.Cyan;
                }
                else if (meterName.Equals("P371602079"))
                {
                    fillColor = UIColor.Orange;
                }
                else if (meterName.Equals("P371602073"))
                {
                    fillColor = UIColor.Yellow;
                }
                else if (meterName.Equals("P371602072"))
                {
                    fillColor = UIColor.Red;
                }
                else if (meterName.Equals("P371602070"))
                {
                    fillColor = UIColor.Purple;
                }
                else if (meterName.Equals("P371602075"))
                {
                    fillColor = UIColor.DarkGray;
                }
                return fillColor;
            }
        }

    }
}
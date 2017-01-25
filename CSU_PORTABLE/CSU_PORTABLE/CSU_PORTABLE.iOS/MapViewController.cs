using CoreLocation;
using Foundation;
using MapKit;
using System;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class MapViewController : UIViewController
    {
        public MapViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MKMapView map = new MKMapView(UIScreen.MainScreen.Bounds);
            map.MapType = MKMapType.Standard; //road map    
            map.ZoomEnabled = true;
            map.ScrollEnabled = true;

            map.AddAnnotations(new MKPointAnnotation()
            {
                Title = "CSU Annotation",
                Coordinate = new CLLocationCoordinate2D(40.571276, -105.085522)
            });
            MKCoordinateSpan span = new MKCoordinateSpan(MilesToLatitudeDegrees(20), MilesToLongitudeDegrees(20, 40.571276));
            map.Region = new MKCoordinateRegion(new CLLocationCoordinate2D(40.571276, -105.085522), span);

            View = map;
        }

        public double MilesToLatitudeDegrees(double miles)
        {
            double earthRadius = 3960.0; // in miles
            double radiansToDegrees = 180.0 / Math.PI;
            return (miles / earthRadius) * radiansToDegrees;
        }

        public double MilesToLongitudeDegrees(double miles, double atLatitude)
        {
            double earthRadius = 3960.0; // in miles
            double degreesToRadians = Math.PI / 180.0;
            double radiansToDegrees = 180.0 / Math.PI;
            // derive the earth's radius at that point in latitude
            double radiusAtLatitude = earthRadius * Math.Cos(atLatitude * degreesToRadians);
            return (miles / radiusAtLatitude) * radiansToDegrees;
        }
    }
}
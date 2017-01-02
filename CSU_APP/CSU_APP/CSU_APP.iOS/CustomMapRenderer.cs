using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms;
using CSU_APP;
using CSU_APP.iOS;
using MapKit;
using Xamarin.Forms.Platform.iOS;
using CSU_APP.Models;
using UIKit;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace CSU_APP.iOS
{
    class CustomMapRenderer : MapRenderer
    {
        List<MKCircle> circleOverlays = new List<MKCircle>();
        CustomCircle circle;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                nativeMap.OverlayRenderer = null;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                var nativeMap = Control as MKMapView;
                var circleList = formsMap.CircleList;

                if (circleList != null)
                {
                    try
                    {
                        for (int i = 0; i < circleList.Count; i++)
                        {
                            nativeMap.OverlayRenderer = (m, o) =>
                            {
                                MKCircleRenderer circleRenderer = new MKCircleRenderer(o as MKCircle);
                                circleRenderer.FillColor = getColor(o.GetTitle());
                                circleRenderer.Alpha = 0.4f;

                                return circleRenderer;
                            };

                            circle = circleList[i];

                            CoreLocation.CLLocationCoordinate2D circCoords = new CoreLocation.CLLocationCoordinate2D(
                                circle.Position.Latitude, circle.Position.Longitude);

                            MKCircle cir = MKCircle.Circle(circCoords, circle.Radius);
                            cir.Title = circle.Color;

                            circleOverlays.Add(cir);

                            nativeMap.AddOverlay(circleOverlays[i]);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: {0}", ex);
                        //do nothing
                    }
                }
                
            }
        }

        private UIColor getColor(String meterName)
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
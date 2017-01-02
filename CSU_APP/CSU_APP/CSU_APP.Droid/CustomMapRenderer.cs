using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using CSU_APP;
using CSU_APP.Droid;
using CSU_APP.Models;
using Java.Lang;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace CSU_APP.Droid
{
    public class CustomMapRenderer : MapRenderer, IOnMapReadyCallback
    {
        GoogleMap map;
        List<CustomCircle> circleList;

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                circleList = formsMap.CircleList;

                ((MapView)Control).GetMapAsync(this);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;

            var circleOptions = new CircleOptions();
            if (circleOptions != null && circleList != null)
            {
                try
                {
                    for (int i = 0; i < circleList.Count; i++)
                    {
                        CustomCircle circle = circleList[i];
                        circleOptions.InvokeCenter(new LatLng(circle.Position.Latitude, circle.Position.Longitude));
                        circleOptions.InvokeRadius(circle.Radius);
                        circleOptions.InvokeFillColor(getColor(circle.Color));
                        circleOptions.InvokeStrokeColor(getColor(circle.Color));
                        circleOptions.InvokeStrokeWidth(0);
                        map.AddCircle(circleOptions);
                    }
                } catch (Exception ex)
                {
                    //do nothing
                }
            }
        }

        private int getColor(string meterName)
        {
            int fillColor = 0X773CA2E0;//2000462560;
            if (meterName.Equals("P371602077"))
            {
                fillColor = 0X778AD4EB;
            }
            else if (meterName.Equals("P371602079"))
            {
                fillColor = 0X77FE9666;
            }
            else if (meterName.Equals("P371602073"))
            {
                fillColor = 0X77F2C80F;
            }
            else if (meterName.Equals("P371602072"))
            {
                fillColor = 0X77FD625E;
            }
            else if (meterName.Equals("P371602070"))
            {
                fillColor = 0X773CA2E0;
            }
            else if (meterName.Equals("P371602075"))
            {
                fillColor = 0X775F6B6D;
            }
            return fillColor;
        }
    }
}
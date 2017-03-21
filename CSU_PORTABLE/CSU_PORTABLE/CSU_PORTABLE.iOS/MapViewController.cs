using CoreGraphics;
using CoreLocation;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using MapKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class MapViewController : BaseController
    {
        MKMapView map;
        List<MeterDetails> meterList = null;
        List<MonthlyConsumptionDetails> monthlyConsumptionList = null;
        private PreferenceHandler prefHandler;
        private UserDetails userdetail;
        private LoadingOverlay loadingOverlay;

        public MapViewController(IntPtr handle) : base(handle)
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
            //this.SidebarController.MenuWidth = 250;




            // GenerateInsightsHeader();
            this.NavigationController.NavigationBarHidden = false;
            this.NavigationController.NavigationBar.TintColor = UIColor.White;
            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(33, 77, 43);
            this.NavigationController.NavigationBar.BarStyle = UIBarStyle.BlackTranslucent;




            prefHandler = new PreferenceHandler();
            userdetail = prefHandler.GetUserDetails();
            GetInsights(userdetail.User_Id);
            //GenerateInsightsHeader();

            double mapHeight = NavigationController.NavigationBar.Bounds.Bottom + 160;
            //map = new MKMapView(UIScreen.MainScreen.Bounds);
            map = new MKMapView(new CGRect(0, mapHeight, View.Bounds.Width, View.Bounds.Height - mapHeight));
            map.MapType = MKMapType.Standard; //road map    
            map.ZoomEnabled = true;
            map.ScrollEnabled = true;


            CLLocationCoordinate2D coordinate = new CLLocationCoordinate2D(40.571276, -105.085522);

            MKCoordinateSpan span = new MKCoordinateSpan(0.004, 0.004);
            map.Region = new MKCoordinateRegion(coordinate, span);

            var mapViewDelegate = new MyMapDelegate();
            mapViewDelegate.AnnotationTapped += TheMapView_OnAnnotationTapped;
            map.Delegate = mapViewDelegate;

            View.AddSubviews(map);

            var preferenceHandler = new PreferenceHandler();
            int userId = preferenceHandler.GetUserDetails().User_Id;
            if (userId != -1)
            {
                GetMeterDetails(userId);
                GetMonthlyConsumptionDetails(userId);
            }
            else
            {
                ShowMessage("Invalid Email. Please Login Again !");
            }
            InvokeOnMainThread(() =>
            {
                // Added for showing loading screen
                var bounds = UIScreen.MainScreen.Bounds;
                // show the loading overlay on the UI thread using the correct orientation sizing
                loadingOverlay = new LoadingOverlay(bounds);
                View.Add(loadingOverlay);
            });
        }

        private void GenerateInsightsHeader(InshghtDataModel insightDM)
        {
            double insightsHeight = NavigationController.NavigationBar.Bounds.Bottom;

            UILabel lblInsightsHeader = new UILabel()
            {
                Frame = new CGRect(0, 0, View.Bounds.Width - 40, 30),
                Text = "University Insights",
                Font = UIFont.FromName("Futura-Medium", 15f),
                TextColor = UIColor.White,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextAlignment = UITextAlignment.Right
            };

            UIImageView imgInsights = new UIImageView()
            {
                Frame = new CGRect(0, insightsHeight + 20, View.Bounds.Width, 90),
                Image = UIImage.FromBundle("Insights_BG.png")
            };

            imgInsights.AddSubview(lblInsightsHeader);


            UIButton btnInsights = new UIButton()
            {
                Frame = new CGRect(0, insightsHeight + 110, View.Bounds.Width, 50),
                BackgroundColor = UIColor.FromRGB(228, 228, 228),
                Font = UIFont.FromName("Futura-Medium", 12f),
            };
            btnInsights.TouchUpInside += BtnInsights_TouchUpInside;

            UILabel.Appearance.Font = UIFont.FromName("Futura-Medium", 20f);

            double lblWidth = (View.Bounds.Width / 3) - 10;

            string strConsumed = Convert.ToString(Math.Round(insightDM.ConsumptionValue / 1000, 2)) + " k";
            string strExpected = Convert.ToString(Math.Round(insightDM.PredictedValue / 1000, 2)) + " k";
            string strOverused = Convert.ToString(Math.Round((insightDM.ConsumptionValue - insightDM.PredictedValue) / 1000, 2)) + " k";



            UIImageView imgConsumed = new UIImageView()
            {
                Frame = new CGRect(5, 5, 10, 20),
                Image = UIImage.FromBundle("Arrow_Blue.png"),
            };

            UILabel lblConsumedCount = new UILabel()
            {
                Frame = new CGRect(30, 0, lblWidth, 30),
                Text = strConsumed,
                Font = UIFont.PreferredTitle2,
                TextColor = UIColor.DarkTextColor,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center

            };

            UIImageView imgExpected = new UIImageView()
            {
                Frame = new CGRect(5, 5, 10, 20),
                Image = UIImage.FromBundle("Arrow_Green.png"),
            };

            UILabel lblExpectedCount = new UILabel()
            {
                Frame = new CGRect(lblWidth + 30, 0, lblWidth, 30),
                Text = strExpected,
                Font = UIFont.PreferredTitle2,
                TextColor = UIColor.DarkTextColor,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };

            UIImageView imgOverused = new UIImageView()
            {
                Frame = new CGRect(5, 5, 10, 20),
                Image = UIImage.FromBundle("Arrow_Red.png"),
            };

            UILabel lblOverusedCount = new UILabel()
            {
                Frame = new CGRect((lblWidth * 2) + 20, 0, lblWidth, 30),
                Text = strOverused,
                Font = UIFont.PreferredTitle2,
                TextColor = UIColor.DarkTextColor,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };


            lblConsumedCount.AddSubview(imgConsumed);
            lblExpectedCount.AddSubview(imgExpected);
            lblOverusedCount.AddSubview(imgOverused);

            UILabel lblConsumed = new UILabel()
            {
                Frame = new CGRect(10, 25, lblWidth + 20, 30),
                Text = "CONSUMED IN LAST WEEK",
                Font = UIFont.FromName("Futura-Medium", 10f),
                TextColor = UIColor.Gray,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };

            UILabel lblExpected = new UILabel()
            {
                Frame = new CGRect(new CGPoint(lblWidth + 20, 25), new CGSize(lblWidth, 30)),
                Text = "EXPECTED",
                Font = UIFont.FromName("Futura-Medium", 10f),
                TextColor = UIColor.Gray,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };

            UILabel lblOverused = new UILabel()
            {
                Frame = new CGRect(new CGPoint((lblWidth * 2) + 10, 25), new CGSize(lblWidth, 30)),
                Text = "OVERUSED",
                Font = UIFont.FromName("Futura-Medium", 10f),
                TextColor = UIColor.Gray,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };

            lblOverused.Text = ((Math.Round((insightDM.ConsumptionValue - insightDM.PredictedValue) / 1000, 2)) > 0 ? "OVERUSED" : "UNDERUSED");
            btnInsights.AddSubviews(lblConsumed, lblExpected, lblOverused, lblConsumedCount, lblExpectedCount, lblOverusedCount);
            View.AddSubviews(btnInsights, imgInsights);
            HideOverlay();
        }

        private void HideOverlay()
        {
            if (loadingOverlay != null)
            {
                loadingOverlay.Hide();
            }
        }

        private void BtnInsights_TouchUpInside(object sender, EventArgs e)
        {
            var InsightsViewController = (InsightsViewController)Storyboard.InstantiateViewController("InsightsViewController");
            NavController.PushViewController(InsightsViewController, false);
            SidebarController.CloseMenu();
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

        #region INSIGHTS

        private void GetInsights(int userId)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            var request = new RestRequest(Constants.API_GET_INSIGHT_DATA + "/" + userId, Method.GET);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    InvokeOnMainThread(() =>
                    {
                        GetInsightDataResponse((RestResponse)response);
                    });
                }
            });
        }

        private void GetInsightDataResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                InshghtDataModel response = JsonConvert.DeserializeObject<InshghtDataModel>(restResponse.Content);
                GenerateInsightsHeader(response);
            }
            else
            {
                HideOverlay();
            }
        }

        #endregion


        #region " Maps "
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
                    InvokeOnMainThread(() =>
                    {
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
                    InvokeOnMainThread(() =>
                    {
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

            /*ReportController reportView = this.Storyboard.InstantiateViewController("ReportController") as ReportController;
            if (reportView != null)
            {
                this.NavigationController.PushViewController(reportView, true);
            }*/
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

        #endregion

    }
}
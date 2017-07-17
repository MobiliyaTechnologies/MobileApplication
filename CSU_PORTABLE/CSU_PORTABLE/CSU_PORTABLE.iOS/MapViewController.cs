using BarChart;
using CoreGraphics;
using CoreLocation;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using MapKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class MapViewController : BaseController
    {
        MKMapView map;
        List<MeterDetails> meterList = null;
        List<MonthlyConsumptionDetails> monthlyConsumptionList = null;
        //private PreferenceHandler prefHandler;
        private UserDetails userdetail;
        private LoadingOverlay loadingOverlay;
        private string localToken = string.Empty;
        public ConsumptionFor CurrentConsumption = ConsumptionFor.Premises;
        public int CurrentPremisesId = 0;
        private BarChartView chart;
        public UILabel lblHeader;
        public UIButton btnBack;

        public MapViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.NavigationController.NavigationBarHidden = false;
            this.NavigationController.NavigationBar.TintColor = UIColor.White;
            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(0, 102, 153);
            this.NavigationController.NavigationBar.BarStyle = UIBarStyle.BlackTranslucent;


            //IOSUtil.RedirectToLogin(this, loadingOverlay);

            //prefHandler = new PreferenceHandler();

            //await IOSUtil.RefreshToken(this, loadingOverlay);
            userdetail = PreferenceHandler.GetUserDetails();
            //this.localToken = PreferenceHandler.GetToken();
            //GetInsights(userdetail.UserId);
            lblHeader = new UILabel()
            {
                Frame = new CGRect(0, this.NavigationController.NavigationBar.Bounds.Bottom + 20, View.Bounds.Width, 40),
                Text = "Premises",
                Font = UIFont.FromName("Futura-Medium", 15f),
                TextColor = UIColor.White,
                BackgroundColor = UIColor.FromRGB(0, 102, 153),
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 1,
                TextAlignment = UITextAlignment.Center
            };

            btnBack = new UIButton()
            {
                Frame = new CGRect(0, this.NavigationController.NavigationBar.Bounds.Bottom + 20, 80, 40),
                Font = UIFont.FromName("Futura-Medium", 15f),
                BackgroundColor = UIColor.FromRGB(0, 102, 153),
            };
            btnBack.SetTitle("< BACK", UIControlState.Normal);
            btnBack.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnBack.SetTitleShadowColor(UIColor.FromRGB(0, 102, 180), UIControlState.Normal);
            btnBack.TouchUpInside += BtnBack_TouchUpInside;
            btnBack.Hidden = true;
            View.AddSubviews(lblHeader, btnBack);
            GetConsumptionDetails(CurrentConsumption, 0);
            //InvokeOnMainThread(() =>
            //{
            //    // Added for showing loading screen
            //    var bounds = UIScreen.MainScreen.Bounds;
            //    // show the loading overlay on the UI thread using the correct orientation sizing
            //    loadingOverlay = new LoadingOverlay(bounds);
            //    View.Add(loadingOverlay);
            //});
        }

        private void BtnBack_TouchUpInside(object sender, EventArgs e)
        {
            btnBack.Hidden = false;
            switch (CurrentConsumption)
            {
                case ConsumptionFor.Buildings:
                    CurrentConsumption = ConsumptionFor.Premises;
                    GetConsumptionDetails(CurrentConsumption, 0);
                    CurrentPremisesId = 0;
                    btnBack.Hidden = true;
                    break;
                case ConsumptionFor.Meters:
                    CurrentConsumption = ConsumptionFor.Buildings;
                    GetConsumptionDetails(CurrentConsumption, CurrentPremisesId);

                    break;
                case ConsumptionFor.Premises:
                    btnBack.Hidden = true;
                    break;
            }
            lblHeader.Text = CurrentConsumption.ToString();
        }

        #region " Consumption "

        public async void GetConsumptionDetails(ConsumptionFor currentConsumption, int Id)
        {
            InvokeOnMainThread(() =>
            {
                // Added for showing loading screen
                var bounds = UIScreen.MainScreen.Bounds;
                // show the loading overlay on the UI thread using the correct orientation sizing
                loadingOverlay = new LoadingOverlay(bounds);
                View.Add(loadingOverlay);
            });

            string url = GetConsumptionURL(currentConsumption);
            if (Id != 0)
            {
                url = url + "/" + Convert.ToString(Id);
            }
            var responseConsumption = await InvokeApi.Invoke(url, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken());
            if (responseConsumption.StatusCode == HttpStatusCode.OK)
            {
                GetConsumptionResponse(responseConsumption);

            }
            else if (responseConsumption.StatusCode == HttpStatusCode.BadRequest || responseConsumption.StatusCode == HttpStatusCode.Unauthorized)
            {
                await IOSUtil.RefreshToken(this, loadingOverlay);
            }
        }

        private string GetConsumptionURL(ConsumptionFor currentConsumption)
        {
            switch (currentConsumption)
            {
                case ConsumptionFor.Premises:
                    return Constants.API_GET_ALLPREMISES;
                case ConsumptionFor.Buildings:
                    return Constants.API_GET_ALLBUILDINGS_BY_PREMISEID;
                case ConsumptionFor.Meters:
                    return Constants.API_GET_ALLMETERS_BY_BUILDINGID;
                default:
                    return Constants.API_GET_ALLPREMISES;
            }
        }


        private List<ConsumptionModel> GetConsumptionModels(string consumptionContent)
        {
            List<ConsumptionModel> conModels = new List<ConsumptionModel>();
            switch (CurrentConsumption)
            {
                case ConsumptionFor.Premises:
                    var modelPremise = JsonConvert.DeserializeObject<List<Premise>>(consumptionContent);
                    conModels = modelPremise.ConvertAll(x => new ConsumptionModel()
                    {
                        Id = x.PremiseID,
                        Name = CurrentConsumption.ToString() + " - " + x.PremiseName,
                        Consumed = Convert.ToString(Math.Round((x.MonthlyConsumption / 1000), 2)) + " K",
                        Expected = Convert.ToString(Math.Round((x.MonthlyPrediction / 1000), 2)) + " K",
                        Overused = Convert.ToString(Math.Round((x.MonthlyPrediction - x.MonthlyConsumption) / 1000, 2)) + " K"
                    });
                    break;
                case ConsumptionFor.Buildings:
                    var modelBuilding = JsonConvert.DeserializeObject<List<Building>>(consumptionContent);
                    return modelBuilding.ConvertAll(x => new ConsumptionModel()
                    {
                        Id = x.BuildingID,
                        Name = CurrentConsumption.ToString() + " - " + x.BuildingName,
                        Consumed = Convert.ToString(Math.Round((x.MonthlyConsumption / 1000), 2)) + " K",
                        Expected = Convert.ToString(Math.Round((x.MonthlyPrediction / 1000), 2)) + " K",
                        Overused = Convert.ToString(Math.Round((x.MonthlyPrediction - x.MonthlyConsumption) / 1000, 2)) + " K"
                    });
                case ConsumptionFor.Meters:
                    var modelMeter = JsonConvert.DeserializeObject<List<Meter>>(consumptionContent);
                    return modelMeter.ConvertAll(x => new ConsumptionModel()
                    {
                        Id = x.Id,
                        Name = CurrentConsumption.ToString() + " - " + x.PowerScout,
                        Consumed = Convert.ToString(Math.Round((x.MonthlyConsumption / 1000), 2)) + " K",
                        Expected = Convert.ToString(Math.Round((x.MonthlyPrediction / 1000), 2)) + " K",
                        Overused = Convert.ToString(Math.Round((x.MonthlyPrediction - x.MonthlyConsumption) / 1000, 2)) + " K"
                    });
            }
            return conModels;
        }


        private async void GetConsumptionResponse(HttpResponseMessage responseConsumption)
        {
            if (responseConsumption != null && responseConsumption.StatusCode == System.Net.HttpStatusCode.OK && responseConsumption.Content != null)
            {
                string strContent = await responseConsumption.Content.ReadAsStringAsync();
                List<ConsumptionModel> consumptions = GetConsumptionModels(strContent);
                SetConsumptions(consumptions);
            }
            else
            {
                await IOSUtil.RefreshToken(this, loadingOverlay);
            }
        }

        private void SetConsumptions(List<ConsumptionModel> consumpModels)
        {
            SetConsumptionBarChart(consumpModels);
            UITableView _table = new UITableView();

            _table = new UITableView
            {
                Frame = new CoreGraphics.CGRect(0, 340, View.Bounds.Width, View.Bounds.Height - 340),
                RowHeight = 100,
                BackgroundColor = UIColor.FromRGBA(193, 214, 218, 0.3f),
                Source = new ConsumptionSource(consumpModels, this)
            };
            _table.ReloadData();
            _table.ClipsToBounds = true;
            View.AddSubview(_table);
            loadingOverlay.Hide();
        }

        private void SetConsumptionBarChart(List<ConsumptionModel> consumpModels)
        {
            if (consumpModels.Count > 0)
            {
                chart = new BarChartView()
                {
                    Frame = new CGRect(0, this.NavigationController.NavigationBar.Bounds.Bottom + 60, View.Bounds.Width, 235),
                    BarCaptionInnerColor = UIColor.White,
                    BarCaptionOuterColor = UIColor.White,
                    BarWidth = 80,
                    BarOffset = 40,
                    BackgroundColor = UIColor.LightGray,
                    LayoutMargins = new UIEdgeInsets(5, 5, 5, 5),
                    MinimumValue = 0
                };
                //#pragma warning disable CS0618 // Type or member is obsolete
                //var layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
                //#pragma warning restore CS0618 // Type or member is obsolete
                //layoutParams.SetMargins(5, 5, 5, 5);
                //chart.LayoutParameters = layoutParams;
                //chart.MinimumValue = -1;
                //chart.MaximumValue = 2;
                //chart.BarCaptionInnerColor = UIColor.Blue.CGColor;
                //chart.BarCaptionOuterColor = UIColor.Blue;
                //chart.SetBackgroundColor(UIColor.Blue);
                chart.BarClick += Chart_BarClick; ;

                var chartData = new List<BarModel>();
                bool ShowChart = false;
                foreach (var item in consumpModels)
                {

                    BarModel chartItem = new BarModel();
                    try
                    {
                        chartItem.Legend = item.Name.Split('-')[1];
                    }
                    catch (Exception)
                    {
                        chartItem.Legend = item.Name;
                    }

                    chartItem.ValueCaption = item.Id.ToString();
                    chartItem.ValueCaptionHidden = true;
                    chartItem.Value = float.Parse(item.Consumed.Replace('K', ' ').Trim());
                    if (float.Parse(item.Consumed.Replace('K', ' ').Trim()) > 0)
                    {
                        ShowChart = true;
                    }
                    //chartItem.Value = (float.Parse(item.Consumed.Replace('K', ' ').Trim()) == 0 ? 0.01f : float.Parse(item.Consumed.Replace('K', ' ').Trim()));
                    chartItem.Color = UIColor.Blue;
                    chartData.Add(chartItem);
                }
                chart.AutoLevelsEnabled = true;
                //chart.BarCaptionFontSize = 20;
                if (ShowChart)
                {
                    chart.ItemsSource = chartData;
                    View.AddSubview(chart);
                    //chart.TextAlignment = TextAlignment.TextEnd;
                    //chart.TextDirection = TextDirection.Rtl;
                    //remark.Visibility = ViewStates.Invisible;
                    //FindViewById<LinearLayout>(Resource.Id.dashboard).RemoveAllViewsInLayout();
                    //FindViewById<LinearLayout>(Resource.Id.dashboard).AddView(chart);
                }
                else
                {
                    //var remarks = new TextView(this);
                    //remarks.Text = "No Records Found!";
                    //var remarksLayoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);

                    //layoutParams.SetMargins(5, 5, 5, 5);
                    //remarks.LayoutParameters = remarksLayoutParams;
                    //remarks.TextDirection = TextDirection.FirstStrongRtl;
                    //remarks.TextAlignment = TextAlignment.Center;
                    //remarks.SetTextColor(Color.LightGray);
                    //remarks.Gravity = GravityFlags.Center;
                    //remarks.TextSize = 20;
                    //remark.Text = "No records found!";
                    //remark.Visibility = ViewStates.Visible;
                    //FindViewById<LinearLayout>(Resource.Id.dashboard).RemoveAllViewsInLayout();
                    //FindViewById<LinearLayout>(Resource.Id.dashboard).AddView(remarks);
                }
            }
            else
            {
                //Utils.Utils.ShowToast(this, "No records found!");
            }
        }

        private void Chart_BarClick(object sender, BarClickEventArgs e)
        {
            if (CurrentConsumption != ConsumptionFor.Meters)
            {
                switch (CurrentConsumption)
                {
                    case ConsumptionFor.Premises:
                        CurrentPremisesId = Convert.ToInt32(e.Bar.ValueCaption);
                        CurrentConsumption = ConsumptionFor.Buildings;
                        lblHeader.Text = ConsumptionFor.Buildings.ToString();
                        break;
                    case ConsumptionFor.Buildings:
                        CurrentConsumption = ConsumptionFor.Meters;
                        lblHeader.Text = ConsumptionFor.Meters.ToString();
                        break;
                }
                btnBack.Hidden = false;
                GetConsumptionDetails(CurrentConsumption, Convert.ToInt32(e.Bar.ValueCaption));
            }
        }
        #endregion

        private void GenerateInsightsHeader(InsightDataModel insightDM)
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

        private void ShowMessage(string v)
        {
            UIAlertController alertController = UIAlertController.Create("Message", v, UIAlertControllerStyle.Alert);

            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));

            PresentViewController(alertController, true, null);

        }

        #region INSIGHTS

        private async void GetInsights(int userId)
        {
            var response = await InvokeApi.Invoke(Constants.API_GET_INSIGHT_DATA, string.Empty, HttpMethod.Get, localToken);
            Console.WriteLine(response);
            if (response.StatusCode != 0)
            {
                InvokeOnMainThread(() =>
                {
                    GetInsightDataResponse(response);
                });
            }
        }

        private async void GetInsightDataResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                string strContent = await restResponse.Content.ReadAsStringAsync();
                InsightDataModel response = JsonConvert.DeserializeObject<InsightDataModel>(strContent);
                GenerateInsightsHeader(response);
            }
            else if (restResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                IOSUtil.RefreshToken(this, loadingOverlay);
            }
            else
            {
                HideOverlay();
            }
        }

        #endregion


    }
}
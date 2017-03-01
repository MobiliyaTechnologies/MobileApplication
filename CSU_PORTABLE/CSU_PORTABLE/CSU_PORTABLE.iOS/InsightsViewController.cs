using CoreGraphics;
using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class InsightsViewController : UIViewController
    {
        private List<AlertModel> lstRecommendations;
        private LoadingOverlay loadingOverlay;
        PreferenceHandler prefHandler;
        UserDetails userdetail;
        nfloat yAxisRecomendation = 70;

        public InsightsViewController(IntPtr handle) : base(handle)
        {
        }


        #region " Events "

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Added for showing loading screen
            var bounds = UIScreen.MainScreen.Bounds;
            // show the loading overlay on the UI thread using the correct orientation sizing
            loadingOverlay = new LoadingOverlay(bounds);
            View.Add(loadingOverlay);
            yAxisRecomendation = yAxisRecomendation + NavigationController.NavigationBar.Bounds.Bottom;
            prefHandler = new PreferenceHandler();
            userdetail = prefHandler.GetUserDetails();
            GetInsights(userdetail.User_Id);
            getRecommendationsList(userdetail.User_Id);
        }



        #endregion


        #region " Custom Functions "
        private void GenerateInsightsHeader(InshghtDataModel insightDM)
        {
            double insightsHeight = NavigationController.NavigationBar.Bounds.Bottom;

            UIButton btnInsights = new UIButton()
            {
                Frame = new CGRect(0, insightsHeight + 20, View.Bounds.Width, 50),
                BackgroundColor = UIColor.FromRGB(228, 228, 228),
                Font = UIFont.FromName("Futura-Medium", 12f),
            };

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
                Frame = new CGRect(10, 25, lblWidth, 30),
                Text = "CONSUMED",
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
            btnInsights.AddSubviews(lblConsumed, lblExpected, lblOverused, lblConsumedCount, lblExpectedCount, lblOverusedCount);
            View.AddSubviews(btnInsights);
        }

        private void GetRecommendations(List<AlertModel> _insights)
        {
            UITableView _table = new UITableView();

            _table = new UITableView
            {
                Frame = new CoreGraphics.CGRect(0, yAxisRecomendation, View.Bounds.Width, View.Bounds.Height - yAxisRecomendation),
                RowHeight = UITableView.AutomaticDimension,
                EstimatedRowHeight = 80f,
                Source = new InsightsSource(_insights)
            };
            _table.ReloadData();
            View.AddSubview(_table);


            HideOverlay();
        }

        private void HideOverlay()
        {
            if (loadingOverlay == null)
            {
                loadingOverlay.Hide();
            }
        }

        private void getRecommendationsList(int userId)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            var request = new RestRequest(Constants.API_GET_RECOMMENDATIONS + "/" + userId, Method.GET);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    InvokeOnMainThread(() =>
                    {
                        getRecommendationsListResponse((RestResponse)response);
                    });
                }
            });
        }

        private void getRecommendationsListResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {

                JArray array = JArray.Parse(restResponse.Content);
                lstRecommendations = array.ToObject<List<AlertModel>>();
                GetRecommendations(lstRecommendations);
            }
            else
            {
                ShowMessage("Please try again later !");
            }
        }


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
                yAxisRecomendation = NavigationController.NavigationBar.Bounds.Bottom + 20;
            }
        }

        private void ShowMessage(string v)
        {
            //BTProgressHUD.ShowMessage("Hello from Toast");
            loadingOverlay.Hide();
            //MessageLabel.Text = " " + v;
            UIAlertController alertController = UIAlertController.Create("Message", v, UIAlertControllerStyle.Alert);

            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));

            PresentViewController(alertController, true, null);

        }

        #endregion
    }
}
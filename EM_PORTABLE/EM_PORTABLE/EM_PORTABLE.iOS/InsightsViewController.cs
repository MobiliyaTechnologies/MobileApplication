﻿using CoreGraphics;
using EM_PORTABLE.iOS.Utils;
using EM_PORTABLE.Models;
using EM_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using UIKit;

namespace EM_PORTABLE.iOS
{
    public partial class InsightsViewController : BaseController
    {
        private List<AlertModel> lstRecommendations;
        private LoadingOverlay loadingOverlay;
        //PreferenceHandler prefHandler;
        UserDetails userdetail;
        nfloat yAxisRecomendation = 70;

        public InsightsViewController(IntPtr handle) : base(handle)
        {
            //prefHandler = new PreferenceHandler();
            userdetail = PreferenceHandler.GetUserDetails();
        }


        #region " Events "

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Added for showing loading screen
            var bounds = UIScreen.MainScreen.Bounds;
            // show the loading overlay on the UI thread using the correct orientation sizing
            loadingOverlay = new LoadingOverlay(bounds);
            View.Add(loadingOverlay);
            yAxisRecomendation = yAxisRecomendation + NavigationController.NavigationBar.Bounds.Bottom;
            GetInsights();
            getRecommendationsList();
        }



        #endregion


        #region " Custom Functions "
        private void GenerateInsightsHeader(InsightDataModel insightDM)
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
                Font = UIFont.FromName("Futura-Medium", 18f),
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
                Font = UIFont.FromName("Futura-Medium", 18f),
                TextColor = UIColor.DarkTextColor,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };



            UILabel lblOverusedCount = new UILabel()
            {
                Frame = new CGRect((lblWidth * 2) + 20, 0, lblWidth, 30),
                Text = strOverused,
                Font = UIFont.FromName("Futura-Medium", 18f),
                TextColor = UIColor.DarkTextColor,
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 3,
                TextAlignment = UITextAlignment.Center
            };

            lblConsumedCount.AddSubview(imgConsumed);
            lblExpectedCount.AddSubview(imgExpected);


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

            if ((Math.Round((insightDM.ConsumptionValue - insightDM.PredictedValue) / 1000, 2)) > 0)
            {
                lblOverused.Text = "OVERUSED";
                UIImageView imgOverused = new UIImageView()
                {
                    Frame = new CGRect(5, 5, 10, 20),
                    Image = UIImage.FromBundle("Arrow_Red.png"),
                };
                lblOverusedCount.AddSubview(imgOverused);
            }
            else
            {
                lblOverused.Text = "UNDERUSED";
                UIImageView imgOverused = new UIImageView()
                {
                    Frame = new CGRect(5, 5, 10, 20),
                    Image = UIImage.FromBundle("Arrow_Green_Down.png"),
                };
                lblOverusedCount.AddSubview(imgOverused);
            }

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

        private async void getRecommendationsList()
        {
            var response = await InvokeApi.Invoke(Constants.API_GET_RECOMMENDATIONS, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken(), IOSUtil.CurrentStage);
            if (response.StatusCode != 0)
            {
                InvokeOnMainThread(() =>
                {
                    getRecommendationsListResponse(response);
                });
            }
        }

        private async void getRecommendationsListResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                string strContent = await restResponse.Content.ReadAsStringAsync();
                JArray array = JArray.Parse(strContent);
                lstRecommendations = array.ToObject<List<AlertModel>>();
                if (lstRecommendations.Count > 0)
                {
                    GetRecommendations(lstRecommendations);
                }
                else
                {
                    UILabel lblRemark = new UILabel()
                    {
                        Frame = new CGRect(0, this.NavigationController.NavigationBar.Bounds.Bottom + 70, View.Bounds.Width, 40),
                        Text = "No recommendations found!",
                        Font = UIFont.FromName("Futura-Medium", 15f),
                        TextColor = UIColor.White,
                        BackgroundColor = UIColor.FromRGB(0, 102, 153),
                        LineBreakMode = UILineBreakMode.WordWrap,
                        Lines = 1,
                        TextAlignment = UITextAlignment.Center
                    };
                    View.AddSubviews(lblRemark);
                    loadingOverlay.Hide();
                }

            }
            else
            {
                IOSUtil.ShowMessage("Please try again later !", loadingOverlay, this);
            }
        }


        private async void GetInsights()
        {
            var response = await InvokeApi.Invoke(Constants.API_GET_INSIGHT_DATA, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken(), IOSUtil.CurrentStage);
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
            else
            {
                yAxisRecomendation = NavigationController.NavigationBar.Bounds.Bottom + 20;
            }
        }


        #endregion
    }
}
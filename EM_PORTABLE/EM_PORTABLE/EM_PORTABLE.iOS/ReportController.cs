﻿using EM_PORTABLE.iOS.Utils;
using EM_PORTABLE.Models;
using EM_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using UIKit;

namespace EM_PORTABLE.iOS
{
    public partial class ReportController : UIViewController
    {
        //PreferenceHandler prefHandler = null;
        UserDetails User = null;
        private UIWebView webView;
        private GlobalReportsModel reports;

        public ReportController(IntPtr handle) : base(handle)
        {
            //this.prefHandler = new PreferenceHandler();
            this.User = PreferenceHandler.GetUserDetails();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            webView = new UIWebView(View.Bounds);
            webView.ScalesPageToFit = false;

            String body = "<html><body>Loading reports...</b></body></html>";
            View.AddSubview(webView);
            showContentOnWebView(body);

            //var preferenceHandler = new PreferenceHandler();
            int userId = PreferenceHandler.GetUserDetails().UserId;
            if (userId != -1)
            {
                getReports();
            }
            else
            {
                IOSUtil.ShowAlert("Invalid Email. Please Login Again !");
            }

        }

        private async void getReports()
        {
            var response = await InvokeApi.Invoke(Constants.API_GET_GLOBAL_REPORTS, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken());

            if (response.StatusCode != 0)
            {
                Console.WriteLine("async Response : " + response.ToString());
                InvokeOnMainThread(() =>
                {
                    GetReportsResponse(response);
                });
            }
        }

        private async void GetReportsResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Console.WriteLine(restResponse.Content.ToString());
                string strContent = await restResponse.Content.ReadAsStringAsync();
                reports = JsonConvert.DeserializeObject<GlobalReportsModel>(strContent);

                if (reports != null)
                {
                    GetAccessToken();
                }
                else
                {
                    Console.WriteLine("GetReportsResponse() Failed");
                    IOSUtil.ShowAlert("Reports are not available");

                    String body = "<html><body>Failed to load reports.</b></body></html>";
                    showContentOnWebView(body);
                }
            }
            else
            {
                Console.WriteLine("GetReportsResponse() Failed");
                IOSUtil.ShowAlert("Failed to load reports");

                String body = "<html><body>Failed to load reports.</b></body></html>";
                showContentOnWebView(body);
            }
        }

        private async void GetAccessToken()
        {
            var response = await InvokeApi.Invoke(Constants.API_GET_TOKEN, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken());
            if (response.StatusCode != 0)
            {
                Console.WriteLine("async Response : " + response.ToString());
                InvokeOnMainThread(() =>
                {
                    GetAccessTokenResponse(response);
                });
            }
        }

        private async void GetAccessTokenResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Console.WriteLine(restResponse.Content.ToString());
                string strContent = await restResponse.Content.ReadAsStringAsync();
                AccessTokenResponse response = JsonConvert.DeserializeObject<AccessTokenResponse>(strContent);

                if (response != null)
                {
                    LoadReports(reports, response.tokens.AccessToken);
                }
            }
            else
            {
                Console.WriteLine("GetAccessTokenResponse() Failed");
                IOSUtil.ShowAlert("Authentication Token not available");

                String body = "<html><body>Failed to load reports.</b></body></html>";
                showContentOnWebView(body);
            }
        }

        private void showContentOnWebView(String body)
        {
            string contentDirectoryPath = Path.Combine(NSBundle.MainBundle.BundlePath, "Content/");
            webView.LoadHtmlString(body, new NSUrl(contentDirectoryPath, true));
        }

        //private void ShowAlert(string message)
        //{
        //    var alert = new UIAlertView(message, "", null, "OK");
        //    alert.Show();
        //}

        private void LoadReports(GlobalReportsModel reports, String token)
        {
            string html = "<html> Reports\n" +
                "\n\n<script type=\"text/javascript\"> " +
                "var ReportUrl1 = \"" + reports.MonthlyConsumptionKWh + "\"; " +
                "var ReportUrl2 = \"" + reports.MonthlyConsumptionCost + "\"; " +

                "\n\nvar iframe; " +
                "\n\nvar accessToken = " + "\'" + token + "\'" + " ; " +
                "\n\nvar height=200;" +
                "\n\nvar width=300; " +

                "function loadEmbededTiles() \n\n{ " +
                    "embedReportTile1(); " +
                    "embedReportTile2();" +
                 "} " +

                //Tile 1----------------------------------------

                "function embedReportTile1() { " +
                    "var embedTileUrl = ReportUrl1; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('IFrame1').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('IFrame1'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadTile1; " +
                "} " +

                "function postActionLoadTile1() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('IFrame1'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +

                //Tile 2----------------------------------------

                "function embedReportTile2() { " +
                    "var embedTileUrl = ReportUrl2; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('IFrame2').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('IFrame2'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadTile2; " +
                "} " +

                "function postActionLoadTile2() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('IFrame2'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +

                //----------------------------------------

                "</script> " +

                "<body " + "onload=\"loadEmbededTiles()\">" +
                "<iframe id=\"IFrame1\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "<iframe id=\"IFrame2\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "</body> " +
                "\n\n</html>\n";

            //Debug.WriteLine("Html Content : " + html);

            showContentOnWebView(html);
        }
    }
}
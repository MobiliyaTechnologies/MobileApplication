using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using CSU_PORTABLE.Droid.Utils;
using Android.Util;
using Newtonsoft.Json;
using CSU_PORTABLE.Models;
using Android.Support.V7.App;
using CSU_PORTABLE.Utils;
using System.Net.Http;
using Android.Content.PM;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "Global Reports", MainLauncher = false, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
    public class GlobalReportsActivity : AppCompatActivity
    {
        const string TAG = "GlobalReportsActivity";
        private WebView localWebView;
        private GlobalReportsModel reports;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MeterReportView);

            localWebView = FindViewById<WebView>(Resource.Id.LocalWebView);
            localWebView.SetWebViewClient(new WebViewClient()); // stops request going to Web Browser
            localWebView.Settings.JavaScriptEnabled = true;

            String body = "<html><body>Loading reports...</body></html>";
            showContentOnWebView(body);

            //var preferenceHandler = new PreferenceHandler();
            int userId = PreferenceHandler.GetUserDetails().UserId;
            if (userId != -1)
            {
                bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(this);
                if (isNetworkEnabled)
                {
                    getReports(userId);
                }
                else
                {
                    Utils.Utils.ShowToast(this, "Please enable your internet connection !");
                }
            }
            else
            {
                Utils.Utils.ShowToast(this, "Invalid Email. Please Login Again !");
            }
        }

        private async void getReports(int userId)
        {
            var response = await InvokeApi.Invoke(Constants.API_GET_GLOBAL_REPORTS + "/" + userId, string.Empty, HttpMethod.Get);
            if (response.StatusCode != 0)
            {
                Log.Debug(TAG, "async Response : " + response.ToString());
                RunOnUiThread(() =>
                {
                    GetReportsResponse(response);
                });
            }
        }

        private async void GetReportsResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                string strContent = await restResponse.Content.ReadAsStringAsync();
                reports = JsonConvert.DeserializeObject<GlobalReportsModel>(strContent);

                if (reports != null)
                {
                    GetAccessToken();
                }
                else
                {
                    Log.Debug(TAG, "GetReportsResponse() Failed");
                    Utils.Utils.ShowToast(this, "Reports are not available");
                    String body = "<html><body>Failed to load reports.</b></body></html>";
                    showContentOnWebView(body);
                }
            }
            else
            {
                Log.Debug(TAG, "GetReportsResponse() Failed");
                Utils.Utils.ShowToast(this, "Failed to load reports");
                String body = "<html><body>Failed to load reports.</b></body></html>";
                showContentOnWebView(body);
            }
        }

        private async void GetAccessToken()
        {
            Log.Debug(TAG, "GetAccessToken()");
            var response = await InvokeApi.Invoke(Constants.API_GET_TOKEN, string.Empty, HttpMethod.Get);
            if (response.StatusCode != 0)
            {
                Log.Debug(TAG, "async Response : " + response.ToString());
                RunOnUiThread(() =>
                {
                    GetAccessTokenResponse(response);
                });
            }
        }

        private async void GetAccessTokenResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                string strContent = await restResponse.Content.ReadAsStringAsync();
                AccessTokenResponse response = JsonConvert.DeserializeObject<AccessTokenResponse>(strContent);
                if (response != null)
                {
                    LoadReports(reports, response.tokens.AccessToken);
                }

            }
            else
            {
                Log.Debug(TAG, "GetAccessTokenResponse() Failed");
                Utils.Utils.ShowToast(this, "Authentication Token not available");
                String body = "<html><body>Failed to load reports.</body></html>";
                showContentOnWebView(body);
            }
        }

        private void showContentOnWebView(String body)
        {
            localWebView.LoadData(body, "text/html", null);
        }

        private void showErrorMessage()
        {
            String body = "<html><body>Reports are not available.</body></html>";
            showContentOnWebView(body);
        }
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
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
using EM_PORTABLE.Droid.Utils;
using Android.Util;
using Newtonsoft.Json;
using EM_PORTABLE.Models;
using Android.Support.V7.App;
using EM_PORTABLE.Utils;
using System.Net.Http;
using Android.Content.PM;

namespace EM_PORTABLE.Droid.UI
{
    [Activity(Label = "Meter Reports", MainLauncher = false, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
    public class MeterReportActivity : AppCompatActivity
    {
        const string TAG = "MeterReportActivity";
        public static String KEY_METER_NAME = "meter_name";
        public static String KEY_METER_SERIAL = "meter_serial";
        private String meterName;
        private String meterSerialNumber;
        private WebView localWebView;
        private MeterReports meterReports;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MeterReportView);

            localWebView = FindViewById<WebView>(Resource.Id.LocalWebView);
            localWebView.SetWebViewClient(new WebViewClient()); // stops request going to Web Browser
            localWebView.Settings.JavaScriptEnabled = true;

            meterName = Intent.GetStringExtra(KEY_METER_NAME) ?? "Meter";
            meterSerialNumber = Intent.GetStringExtra(KEY_METER_SERIAL) ?? null;

            if (meterSerialNumber == null)
            {
                showErrorMessage();
            }
            else
            {
                String body = "<html><body>Loading reports for <b>" + meterName + "...</b></body></html>";
                showContentOnWebView(body);

                //var preferenceHandler = new PreferenceHandler();
                int userId = PreferenceHandler.GetUserDetails().UserId;
                if (userId != -1)
                {
                    bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(this);
                    if (isNetworkEnabled)
                    {
                        getMeterReports(userId, meterSerialNumber);
                    }
                    else
                    {
                        Utils.Utils.ShowToast(this, "Please enable your internet connection !");
                    }
                }
                else
                {
                    Utils.Utils.ShowToast(this, "Invalid User Id. Please Login Again !");
                }
            }
        }

        private async void getMeterReports(int userId, string serialNumber)
        {
            Log.Debug(TAG, "getMeterDetails()");
            var response = await InvokeApi.Invoke(Constants.API_GET_METER_REPORTS + "/" + userId + "/" + serialNumber, string.Empty, HttpMethod.Get);
            if (response.StatusCode != 0)
            {
                Log.Debug(TAG, "async Response : " + response.ToString());
                RunOnUiThread(() =>
                {
                    GetMeterReportsResponse(response);
                });
            }
        }

        private async void GetMeterReportsResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                string strContent = await restResponse.Content.ReadAsStringAsync();
                meterReports = JsonConvert.DeserializeObject<MeterReports>(strContent);

                if (meterReports != null)
                {
                    GetAccessToken();
                }
                else
                {
                    Log.Debug(TAG, "GetMeterReportsResponse() Failed");
                    Utils.Utils.ShowToast(this, "Reports are not available");
                    String body = "<html><body>Failed to load reports for <b>" + meterName + ".</b></body></html>";
                    showContentOnWebView(body);
                }
            }
            else
            {
                Log.Debug(TAG, "GetMeterReportsResponse() Failed");
                Utils.Utils.ShowToast(this, "Failed to load reports");
                String body = "<html><body>Failed to load reports for <b>" + meterName + ".</b></body></html>";
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
                if (response != null && meterReports != null)
                {
                    LoadReports(meterReports, response.tokens.AccessToken);
                }
            }
            else
            {
                Log.Debug(TAG, "GetAccessTokenResponse() Failed");
                Utils.Utils.ShowToast(this, "Authentication Token not available");
                String body = "<html><body>Failed to load reports for <b>" + meterName + ".</b></body></html>";
                showContentOnWebView(body);
            }
        }

        private void showContentOnWebView(String body)
        {
            localWebView.LoadData(body, "text/html", null);
        }

        private void showErrorMessage()
        {
            String body = "<html><body>Reports are not available for <b>" + meterName + "</b>.</body></html>";
            showContentOnWebView(body);
        }

        private void LoadReports(MeterReports meterReports, String token)
        {
            string urlWeather = meterReports.Weather;
            string urlMonthlyKWh = meterReports.MonthlyKWh;
            string urlMonthlyCost = meterReports.MonthlyCost;
            string urlDayWiseConsumption = meterReports.DayWiseConsumption;
            string urlPeriodWiseConsumption = meterReports.PeriodWiseConsumption;
            string urlDailyConsumption = meterReports.DailyConsumption;
            string urlYesterdayConsumption = meterReports.YesterdayConsumption;
            string urlWeeklyConsumption = meterReports.WeeklyConsumption;
            string urlLastWeeklyConsumption = meterReports.LastWeeklyConsumption;
            string urlMonthlyConsumption = meterReports.MonthlyConsumption;
            string urlLastMonthlyConsumption = meterReports.LastMonthlyConsumption;
            string urlQuarterlyConsumption = meterReports.QuarterlyConsumption;
            string urlLastQuarterlyConsumption = meterReports.LastQuarterlyConsumption;

            string html = "<html> " + meterName + " reports\n" +
                "\n\n<script type=\"text/javascript\"> " +
                "var WeatherUrl = \"" + urlWeather + "\"; " +
                "var MonthlyKWhUrl = \"" + urlMonthlyKWh + "\"; " +
                "var MonthlyCostUrl = \"" + urlMonthlyCost + "\"; " +
                "var DayWiseConsumptionUrl = \"" + urlDayWiseConsumption + "\"; " +
                "var PeriodWiseConsumptionUrl = \"" + urlPeriodWiseConsumption + "\"; " +
                "var DailyConsumptionUrl = \"" + urlDailyConsumption + "\"; " +
                "var YesterdayConsumptionUrl = \"" + urlYesterdayConsumption + "\"; " +
                "var WeeklyConsumptionUrl = \"" + urlWeeklyConsumption + "\"; " +
                "var LastWeeklyConsumptionUrl = \"" + urlLastWeeklyConsumption + "\"; " +
                "var MonthlyConsumptionUrl = \"" + urlMonthlyConsumption + "\"; " +
                "var LastMonthlyConsumptionUrl = \"" + urlLastMonthlyConsumption + "\"; " +
                "var QuarterlyConsumptionUrl = \"" + urlQuarterlyConsumption + "\"; " +
                "var LastQuarterlyConsumptionUrl = \"" + urlLastQuarterlyConsumption + "\"; " +

                "\n\nvar iframe; " +
                "\n\nvar accessToken = " + "\'" + token + "\'" + " ; " +
                "\n\nvar height=200;" +
                "\n\nvar width=300; " +

                "function loadEmbededTiles() \n\n{ " +
                    "embedWeatherTile(); " +
                    "embedMonthlyKWhTile();" +
                    "embedMonthlyCostTile();" +
                    "embedDayWiseConsumptionTile();" +
                    "embedPeriodWiseConsumptionTile();" +
                    "embedDailyConsumptionTile();" +
                    "embedYesterdayConsumptionTile();" +
                    "embedWeeklyConsumptionTile();" +
                    "embedLastWeeklyConsumptionTile();" +
                    "embedMonthlyConsumptionTile();" +
                    "embedLastMonthlyConsumptionTile();" +
                    "embedQuarterlyConsumptionTile();" +
                    "embedLastQuarterlyConsumptionTile();" +
                "} " +

                //Weather----------------------------------------

                "function embedWeatherTile() { " +
                    "var embedTileUrl = WeatherUrl; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('WeatherIFrame').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('WeatherIFrame'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadWeatherTile; " +
                "} " +

                "function postActionLoadWeatherTile() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('WeatherIFrame'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +

                //MonthlyKWh----------------------------------------

                "function embedMonthlyKWhTile() { " +
                    "var embedTileUrl = MonthlyKWhUrl; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('MonthlyKWhIFrame').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('MonthlyKWhIFrame'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadMonthlyKWhTile; " +
                "} " +

                "function postActionLoadMonthlyKWhTile() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('MonthlyKWhIFrame'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +

                //MonthlyCost----------------------------------------

                "function embedMonthlyCostTile() { " +
                    "var embedTileUrl = MonthlyCostUrl; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('MonthlyCostIFrame').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('MonthlyCostIFrame'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadMonthlyCostTile; " +
                "} " +

                "function postActionLoadMonthlyCostTile() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('MonthlyCostIFrame'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +

                //DayWiseConsumption----------------------------------------

                "function embedDayWiseConsumptionTile() { " +
                    "var embedTileUrl = DayWiseConsumptionUrl; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('DayWiseConsumptionIFrame').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('DayWiseConsumptionIFrame'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadDayWiseConsumptionTile; " +
                "} " +

                "function postActionLoadDayWiseConsumptionTile() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('DayWiseConsumptionIFrame'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +

                //PeriodWiseConsumption----------------------------------------

                "function embedPeriodWiseConsumptionTile() { " +
                    "var embedTileUrl = PeriodWiseConsumptionUrl; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('PeriodWiseConsumptionIFrame').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('PeriodWiseConsumptionIFrame'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadPeriodWiseConsumptionTile; " +
                "} " +

                "function postActionLoadPeriodWiseConsumptionTile() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('PeriodWiseConsumptionIFrame'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +

                //DailyConsumption----------------------------------------

                "function embedDailyConsumptionTile() { " +
                    "var embedTileUrl = DailyConsumptionUrl; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('DailyConsumptionIFrame').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('DailyConsumptionIFrame'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadDailyConsumptionTile; " +
                "} " +

                "function postActionLoadDailyConsumptionTile() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('DailyConsumptionIFrame'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +

                //YesterdayConsumption----------------------------------------

                "function embedYesterdayConsumptionTile() { " +
                    "var embedTileUrl = YesterdayConsumptionUrl; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('YesterdayConsumptionIFrame').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('YesterdayConsumptionIFrame'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadYesterdayConsumptionTile; " +
                "} " +

                "function postActionLoadYesterdayConsumptionTile() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('YesterdayConsumptionIFrame'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +

                //WeeklyConsumption----------------------------------------

                "function embedWeeklyConsumptionTile() { " +
                    "var embedTileUrl = WeeklyConsumptionUrl; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('WeeklyConsumptionIFrame').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('WeeklyConsumptionIFrame'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadWeeklyConsumptionTile; " +
                "} " +

                "function postActionLoadWeeklyConsumptionTile() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('WeeklyConsumptionIFrame'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +

                //LastWeeklyConsumption----------------------------------------

                "function embedLastWeeklyConsumptionTile() { " +
                    "var embedTileUrl = LastWeeklyConsumptionUrl; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('LastWeeklyConsumptionIFrame').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('LastWeeklyConsumptionIFrame'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadLastWeeklyConsumptionTile; " +
                "} " +

                "function postActionLoadLastWeeklyConsumptionTile() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('LastWeeklyConsumptionIFrame'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +

                //MonthlyConsumption----------------------------------------

                "function embedMonthlyConsumptionTile() { " +
                    "var embedTileUrl = MonthlyConsumptionUrl; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('MonthlyConsumptionIFrame').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('MonthlyConsumptionIFrame'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadMonthlyConsumptionTile; " +
                "} " +

                "function postActionLoadMonthlyConsumptionTile() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('MonthlyConsumptionIFrame'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +


                //LastMonthlyConsumption----------------------------------------

                "function embedLastMonthlyConsumptionTile() { " +
                    "var embedTileUrl = LastMonthlyConsumptionUrl; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('LastMonthlyConsumptionIFrame').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('LastMonthlyConsumptionIFrame'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadLastMonthlyConsumptionTile; " +
                "} " +

                "function postActionLoadLastMonthlyConsumptionTile() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('LastMonthlyConsumptionIFrame'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +


                //QuarterlyConsumption----------------------------------------

                "function embedQuarterlyConsumptionTile() { " +
                    "var embedTileUrl = QuarterlyConsumptionUrl; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('QuarterlyConsumptionIFrame').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('QuarterlyConsumptionIFrame'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadQuarterlyConsumptionTile; " +
                "} " +

                "function postActionLoadQuarterlyConsumptionTile() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('QuarterlyConsumptionIFrame'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +

                //LastQuarterlyConsumption----------------------------------------

                "function embedLastQuarterlyConsumptionTile() { " +
                    "var embedTileUrl = LastQuarterlyConsumptionUrl; " +
                    "if (\"\" === embedTileUrl) { " +
                        "console.log(\"No embed URL found\"); " +
                        "document.getElementById('LastQuarterlyConsumptionIFrame').style.display = 'none'; " +
                        "return; " +
                    "} " +
                    "iframe = document.getElementById('LastQuarterlyConsumptionIFrame'); " +
                    "iframe.src = embedTileUrl + \"&width=\" + width + \"&height=\" + height; " +
                    "iframe.onload = postActionLoadLastQuarterlyConsumptionTile; " +
                "} " +

                "function postActionLoadLastQuarterlyConsumptionTile() { " +
                    "if (\"\" === accessToken) { " +
                        "console.log(\"Access token not found\"); " +
                        "return; " +
                    "} " +
                    "var h = height; " +
                    "var w = width; " +
                    "var m = { action: \"loadTile\", accessToken: accessToken, height: h, width: w }; " +
                    "var message = JSON.stringify(m); " +
                    "iframe = document.getElementById('LastQuarterlyConsumptionIFrame'); " +
                    "iframe.contentWindow.postMessage(message, \"*\");; " +
                "} " +

                //----------------------------------------


                "</script> " +

                "<body " + "onload=\"loadEmbededTiles()\">" +
                "<iframe id=\"WeatherIFrame\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "<iframe id=\"MonthlyKWhIFrame\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "<iframe id=\"MonthlyCostIFrame\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "<iframe id=\"DayWiseConsumptionIFrame\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "<iframe id=\"PeriodWiseConsumptionIFrame\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "<iframe id=\"DailyConsumptionIFrame\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "<iframe id=\"YesterdayConsumptionIFrame\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "<iframe id=\"WeeklyConsumptionIFrame\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "<iframe id=\"LastWeeklyConsumptionIFrame\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "<iframe id=\"MonthlyConsumptionIFrame\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "<iframe id=\"LastMonthlyConsumptionIFrame\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "<iframe id=\"QuarterlyConsumptionIFrame\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "<iframe id=\"LastQuarterlyConsumptionIFrame\" src=\"\" style=\"height:40vh;width:300px;\" frameborder=\"0\" seamless></iframe> " +
                "</body> " +
                "\n\n</html>\n";

            //Debug.WriteLine("Html Content : " + html);

            showContentOnWebView(html);
        }
    }
}
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
using RestSharp;
using Android.Util;
using Newtonsoft.Json;
using CSU_PORTABLE.Models;
using Android.Support.V7.App;
using CSU_PORTABLE.Utils;

namespace CSU_PORTABLE.Droid.UI
{
    [Activity(Label = "Global Reports", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyTheme")]
    public class GlobalReportsActivity : AppCompatActivity
    {
        const string TAG = "GlobalReportsActivity";
        private WebView localWebView;
        private Toast toast;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MeterReportView);

            localWebView = FindViewById<WebView>(Resource.Id.LocalWebView);
            localWebView.SetWebViewClient(new WebViewClient()); // stops request going to Web Browser
            localWebView.Settings.JavaScriptEnabled = true;

            String body = "<html><body>Loading reports...</body></html>";
            showContentOnWebView(body);

            GetAccessToken();
        }
                
        private void GetAccessToken()
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL_FOR_TOKEN);
            Log.Debug(TAG, "GetAccessToken()");

            var request = new RestRequest(Constants.API_GET_TOKEN, Method.GET);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    RunOnUiThread(() => {
                        GetAccessTokenResponse((RestResponse)response);
                    });
                }
            });
        }

        private void GetAccessTokenResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                AccessTokenResponse response = JsonConvert.DeserializeObject<AccessTokenResponse>(restResponse.Content);

                if (response != null)
                {
                    LoadReports(response.tokens.AccessToken);
                }
            }
            else
            {
                Log.Debug(TAG, "GetAccessTokenResponse() Failed");
                ShowToast("Authentication Token not available");

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
        private void ShowToast(string message)
        {
            if (toast != null)
            {
                toast.Cancel();
            }
            toast = Toast.MakeText(this, message, ToastLength.Short);
            toast.Show();
        }

        private void LoadReports(String token)
        {
            string html = "<html> Reports\n" +
                "\n\n<script type=\"text/javascript\"> " +
                "var ReportUrl1 = \"" + Constants.URL_GLOBAL_REPORT_1 + "\"; " +
                "var ReportUrl2 = \"" + Constants.URL_GLOBAL_REPORT_1 + "\"; " +
                
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
                    "iframe = document.getElementById('IFrameq'); " +
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
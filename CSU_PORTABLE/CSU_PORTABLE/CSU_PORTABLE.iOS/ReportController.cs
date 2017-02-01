using CSU_PORTABLE.iOS.Utils;
using CSU_PORTABLE.Models;
using CSU_PORTABLE.Utils;
using Foundation;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using UIKit;

namespace CSU_PORTABLE.iOS
{
    public partial class ReportController : UIViewController
    {
        private UIWebView webView;

        public ReportController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            webView = new UIWebView(View.Bounds);
            webView.ScalesPageToFit = false;

            String body = "<html><body>Loading reports...</b></body></html>";
            View.AddSubview(webView);
            showContentOnWebView(body);

            var preferenceHandler = new PreferenceHandler();
            int userId = preferenceHandler.GetUserDetails().User_Id;
            if (userId != -1)
            {
                GetAccessToken();
            }
            else
            {
                ShowAlert("Invalid Email. Please Login Again !");
            }

        }

        private void GetAccessToken()
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL_FOR_TOKEN);
            Console.WriteLine("GetAccessToken()");

            var request = new RestRequest(Constants.API_GET_TOKEN, Method.GET);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Console.WriteLine("async Response : " + response.ToString());
                    InvokeOnMainThread(() => {
                        GetAccessTokenResponse((RestResponse)response);
                    });
                }
            });
        }

        private void GetAccessTokenResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Console.WriteLine(restResponse.Content.ToString());
                AccessTokenResponse response = JsonConvert.DeserializeObject<AccessTokenResponse>(restResponse.Content);

                if (response != null)
                {
                    LoadReports(response.tokens.AccessToken);
                }
            }
            else
            {
                Console.WriteLine("GetAccessTokenResponse() Failed");
                ShowAlert("Authentication Token not available");

                String body = "<html><body>Failed to load reports.</b></body></html>";
                showContentOnWebView(body);
            }
        }

        private void showContentOnWebView(String body)
        {
            string contentDirectoryPath = Path.Combine(NSBundle.MainBundle.BundlePath, "Content/");
            webView.LoadHtmlString(body, new NSUrl(contentDirectoryPath, true));
        }

        private void ShowAlert(string message)
        {
            var alert = new UIAlertView(message, "", null, "OK");
            alert.Show();
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